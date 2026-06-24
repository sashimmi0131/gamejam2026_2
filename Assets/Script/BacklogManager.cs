using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.B;
    [SerializeField] private bool enableKeyboardToggle = true;

    [Header("Log")]
    [SerializeField] private int maxLogCount = 50;
    [SerializeField] private int visibleLogCount = 5;
    [SerializeField] private string choicePrefix = "> ";

    private readonly List<BacklogEntry> logs = new List<BacklogEntry>();
    private TMP_FontAsset logFont;

    public bool IsOpen
    {
        get
        {
            return backlogPanel != null && backlogPanel.activeSelf;
        }
    }

    private void Awake()
    {
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

        backlogPanel.SetActive(!backlogPanel.activeSelf);

        if (backlogPanel.activeSelf)
        {
            RebuildLogText();
        }
    }

    public void OpenBacklog()
    {
        if (backlogPanel == null)
        {
            return;
        }

        backlogPanel.SetActive(true);
        RebuildLogText();
    }

    public void CloseBacklog()
    {
        if (backlogPanel != null)
        {
            backlogPanel.SetActive(false);
        }
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

        ApplyTextSettings();
        combinedLogText.text = "";

        int startIndex = Mathf.Max(0, logs.Count - Mathf.Max(1, visibleLogCount));

        for (int i = startIndex; i < logs.Count; i++)
        {
            combinedLogText.text += FormatLogEntry(logs[i]) + "\n\n";
        }
    }

    private void ApplyTextSettings()
    {
        if (combinedLogText == null)
        {
            return;
        }

        combinedLogText.enableWordWrapping = true;
        combinedLogText.overflowMode = TextOverflowModes.Truncate;

        if (logFont != null)
        {
            combinedLogText.font = logFont;
        }
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
