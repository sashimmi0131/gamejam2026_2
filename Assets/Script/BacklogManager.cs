using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BacklogManager : MonoBehaviour
{
    [System.Serializable]
    private class BacklogEntry
    {
        public string speaker;
        public string text;

        public BacklogEntry(string speaker, string text)
        {
            this.speaker = speaker;
            this.text = text;
        }
    }

    [Header("UI")]
    [SerializeField] private GameObject backlogPanel;
    [SerializeField] private TextMeshProUGUI combinedLogText;
    [SerializeField] private ScrollRect backlogScrollRect;
    [SerializeField] private RectTransform logContent;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.B;
    [SerializeField] private bool enableKeyboardToggle = true;

    [Header("Sound")]
    [SerializeField] private AudioSource backlogAudioSource;
    [SerializeField] private AudioClip backlogButtonSound;

    [Header("Log")]
    [SerializeField] private int maxLogCount = 50;
    [SerializeField] private int visibleLogCount = 5;
    [SerializeField] private string choicePrefix = "> ";

    private readonly List<BacklogEntry> logs = new List<BacklogEntry>();
    private TMP_FontAsset logFont;

    public event Action BeforeBacklogOpen;

    public bool IsOpen
    {
        get
        {
            return backlogPanel != null && backlogPanel.activeSelf;
        }
    }

    private void Awake()
    {
        ConfigureScrollRect();
        ApplyTextSettings();

        if (backlogPanel != null)
        {
            backlogPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (enableKeyboardToggle && Input.GetKeyDown(toggleKey))
        {
            ToggleBacklog();
        }
    }

    public void AddLog(string speaker, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        AddEntry(speaker, text);
    }

    public void AddChoiceLog(string choiceText)
    {
        if (string.IsNullOrWhiteSpace(choiceText))
        {
            return;
        }

        AddEntry("", choicePrefix + choiceText);
    }

    public void SetLogFont(TMP_FontAsset fontAsset)
    {
        logFont = fontAsset;

        if (combinedLogText != null && logFont != null)
        {
            combinedLogText.font = logFont;
        }
    }

    public void ToggleBacklog()
    {
        if (backlogPanel == null)
        {
            return;
        }

        bool willOpen = !backlogPanel.activeSelf;

        if (willOpen)
        {
            BeforeBacklogOpen?.Invoke();
        }

        backlogPanel.SetActive(willOpen);

        if (backlogPanel.activeSelf)
        {
            RebuildLogText();
        }

        PlayBacklogButtonSound();
    }

    public void OnBacklogButtonClicked()
    {
        ToggleBacklog();
    }

    public void OpenBacklog()
    {
        if (backlogPanel == null)
        {
            return;
        }

        BeforeBacklogOpen?.Invoke();
        backlogPanel.SetActive(true);
        RebuildLogText();
        PlayBacklogButtonSound();
    }

    public void OnBacklogOpenButtonClicked()
    {
        OpenBacklog();
    }

    public void CloseBacklog()
    {
        if (backlogPanel != null)
        {
            backlogPanel.SetActive(false);
            PlayBacklogButtonSound();
        }
    }

    public void OnBacklogCloseButtonClicked()
    {
        CloseBacklog();
    }

    public void ClearBacklog()
    {
        logs.Clear();
        RebuildLogText();
    }

    private void AddEntry(string speaker, string text)
    {
        if (logs.Count > 0)
        {
            BacklogEntry lastLog = logs[logs.Count - 1];

            if (lastLog.speaker == speaker && lastLog.text == text)
            {
                return;
            }
        }

        logs.Add(new BacklogEntry(speaker, text));

        while (logs.Count > Mathf.Max(1, maxLogCount))
        {
            logs.RemoveAt(0);
        }

        if (IsOpen)
        {
            RebuildLogText();
        }
    }

    private void RebuildLogText()
    {
        if (combinedLogText == null)
        {
            return;
        }

        ConfigureScrollRect();
        ApplyTextSettings();
        combinedLogText.text = "";

        int startIndex = Mathf.Max(0, logs.Count - Mathf.Max(1, visibleLogCount));

        for (int i = startIndex; i < logs.Count; i++)
        {
            combinedLogText.text += FormatLogEntry(logs[i]) + "\n\n";
        }

        ResizeLogContent();
        ScrollToLatestLog();
    }

    private void ApplyTextSettings()
    {
        if (combinedLogText == null)
        {
            return;
        }

        combinedLogText.enableWordWrapping = true;
        combinedLogText.overflowMode = TextOverflowModes.Overflow;

        if (logFont != null)
        {
            combinedLogText.font = logFont;
        }
    }

    private void ConfigureScrollRect()
    {
        if (backlogScrollRect == null && backlogPanel != null)
        {
            backlogScrollRect = backlogPanel.GetComponentInChildren<ScrollRect>(true);
        }

        if (backlogScrollRect == null)
        {
            return;
        }

        if (logContent == null)
        {
            logContent = backlogScrollRect.content;
        }

        backlogScrollRect.horizontal = false;
        backlogScrollRect.vertical = true;
        backlogScrollRect.movementType = ScrollRect.MovementType.Clamped;

        if (backlogScrollRect.horizontalScrollbar != null)
        {
            backlogScrollRect.horizontalScrollbar.gameObject.SetActive(false);
            backlogScrollRect.horizontalScrollbar = null;
        }

        if (backlogScrollRect.verticalScrollbar != null)
        {
            backlogScrollRect.verticalScrollbar.gameObject.SetActive(true);
            backlogScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
        }
    }

    private void ResizeLogContent()
    {
        if (combinedLogText == null)
        {
            return;
        }

        RectTransform textRect = combinedLogText.rectTransform;
        float viewportWidth = GetViewportWidth();

        if (viewportWidth > 0f)
        {
            if (logContent != null)
            {
                Vector2 fixedContentSize = logContent.sizeDelta;
                fixedContentSize.x = viewportWidth;
                logContent.sizeDelta = fixedContentSize;
            }

            Vector2 fixedWidthSize = textRect.sizeDelta;
            fixedWidthSize.x = viewportWidth;
            textRect.sizeDelta = fixedWidthSize;
        }

        combinedLogText.ForceMeshUpdate();

        float viewportHeight = GetViewportHeight();
        float preferredHeight = Mathf.Max(combinedLogText.preferredHeight, viewportHeight);
        Vector2 textSize = textRect.sizeDelta;
        textSize.y = preferredHeight;
        textRect.sizeDelta = textSize;

        if (logContent != null)
        {
            Vector2 contentSize = logContent.sizeDelta;
            contentSize.x = viewportWidth > 0f ? viewportWidth : textRect.sizeDelta.x;
            contentSize.y = preferredHeight;
            logContent.sizeDelta = contentSize;
        }
    }

    private float GetViewportWidth()
    {
        if (backlogScrollRect == null || backlogScrollRect.viewport == null)
        {
            return 0f;
        }

        return backlogScrollRect.viewport.rect.width;
    }

    private float GetViewportHeight()
    {
        if (backlogScrollRect == null || backlogScrollRect.viewport == null)
        {
            return 0f;
        }

        return backlogScrollRect.viewport.rect.height;
    }

    private void ScrollToLatestLog()
    {
        if (backlogScrollRect == null)
        {
            return;
        }

        Canvas.ForceUpdateCanvases();
        backlogScrollRect.verticalNormalizedPosition = 0f;
    }

    private void PlayBacklogButtonSound()
    {
        if (backlogAudioSource == null || backlogButtonSound == null)
        {
            return;
        }

        backlogAudioSource.PlayOneShot(backlogButtonSound);
    }

    private string FormatLogEntry(BacklogEntry log)
    {
        if (string.IsNullOrWhiteSpace(log.speaker))
        {
            return log.text;
        }

        return log.speaker + "\n" + log.text;
    }
}
