using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private TextMeshProUGUI autoModeLabel;
    [SerializeField] private Toggle autoModeToggle;
    [SerializeField] private Slider autoModeSpeedSlider;
    [SerializeField] private TextMeshProUGUI autoModeSpeedLabel;
    [SerializeField] private TMP_FontAsset settingsFont;
    [SerializeField] private TextMeshProUGUI[] extraJapaneseTexts;

    [Header("Story")]
    [SerializeField] private StoryManager2 storyManager;

    [Header("Auto Mode Speed")]
    [SerializeField] private float minAutoModeInterval = 0.5f;
    [SerializeField] private float maxAutoModeInterval = 4f;

    [Header("Scene")]
    [SerializeField] private string titleSceneName = "Title";

    [Header("Sound")]
    [SerializeField] private AudioSource menuAudioSource;
    [SerializeField] private AudioClip menuButtonSound;

    private const string AutoOnText = "\u81ea\u52d5\u8aad\u307f\u4e0a\u3052\uff1a\u30aa\u30f3";
    private const string AutoOffText = "\u81ea\u52d5\u8aad\u307f\u4e0a\u3052\uff1a\u30aa\u30d5";
    private const string AutoSpeedPrefix = "\u8aad\u307f\u4e0a\u3052\u9593\u9694\uff1a";
    private const string NotSetText = "\u672a\u8a2d\u5b9a";
    private const string SecondsText = "\u79d2";

    public static bool IsAnySettingsOpen { get; private set; }

    public bool IsOpen
    {
        get
        {
            return settingsPanel != null && settingsPanel.activeSelf;
        }
    }

    private void Awake()
    {
        IsAnySettingsOpen = false;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        ApplySettingsFont();
        SetupAutoModeSpeedSlider();
        RefreshAutoModeView();
        RefreshAutoModeSpeedView();
    }

    private void OnDestroy()
    {
        if (IsOpen)
        {
            IsAnySettingsOpen = false;
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel == null)
        {
            return;
        }

        settingsPanel.SetActive(true);
        IsAnySettingsOpen = true;
        ApplySettingsFont();
        RefreshAutoModeView();
        RefreshAutoModeSpeedView();
        PlayMenuButtonSound();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        IsAnySettingsOpen = false;
        PlayMenuButtonSound();
    }

    public void ToggleSettings()
    {
        if (IsOpen)
        {
            CloseSettings();
        }
        else
        {
            OpenSettings();
        }
    }

    public void ToggleAutoMode()
    {
        if (storyManager == null)
        {
            return;
        }

        storyManager.SetAutoMode(!storyManager.IsAutoMode);
        RefreshAutoModeView();
        PlayMenuButtonSound();
    }

    public void SetAutoMode(bool isOn)
    {
        if (storyManager == null)
        {
            return;
        }

        storyManager.SetAutoMode(isOn);
        RefreshAutoModeView();
        PlayMenuButtonSound();
    }

    public void SetAutoModeSpeed(float value)
    {
        if (storyManager == null)
        {
            return;
        }

        float interval = Mathf.Lerp(maxAutoModeInterval, minAutoModeInterval, value);
        storyManager.SetAutoModeInterval(interval);
        RefreshAutoModeSpeedView();
    }

    public void ReturnToTitle()
    {
        PlayMenuButtonSound();

        if (string.IsNullOrWhiteSpace(titleSceneName))
        {
            Debug.LogWarning("Title scene name is empty.");
            return;
        }

        IsAnySettingsOpen = false;
        SceneManager.LoadScene(titleSceneName);
    }

    public void QuitGame()
    {
        PlayMenuButtonSound();
        IsAnySettingsOpen = false;

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void RefreshAutoModeView()
    {
        bool isAutoMode = storyManager != null && storyManager.IsAutoMode;

        if (autoModeLabel != null)
        {
            autoModeLabel.text = isAutoMode ? AutoOnText : AutoOffText;
        }

        if (autoModeToggle != null)
        {
            autoModeToggle.SetIsOnWithoutNotify(isAutoMode);
        }
    }

    private void SetupAutoModeSpeedSlider()
    {
        if (autoModeSpeedSlider == null)
        {
            return;
        }

        autoModeSpeedSlider.minValue = 0f;
        autoModeSpeedSlider.maxValue = 1f;
        autoModeSpeedSlider.onValueChanged.RemoveListener(SetAutoModeSpeed);
        autoModeSpeedSlider.onValueChanged.AddListener(SetAutoModeSpeed);

        if (storyManager == null)
        {
            return;
        }

        float interval = Mathf.Clamp(storyManager.AutoModeInterval, minAutoModeInterval, maxAutoModeInterval);
        float sliderValue = Mathf.InverseLerp(maxAutoModeInterval, minAutoModeInterval, interval);
        autoModeSpeedSlider.SetValueWithoutNotify(sliderValue);
    }

    private void RefreshAutoModeSpeedView()
    {
        if (storyManager == null)
        {
            if (autoModeSpeedLabel != null)
            {
                autoModeSpeedLabel.text = AutoSpeedPrefix + NotSetText;
            }

            return;
        }

        if (autoModeSpeedSlider != null)
        {
            float interval = Mathf.Clamp(storyManager.AutoModeInterval, minAutoModeInterval, maxAutoModeInterval);
            float sliderValue = Mathf.InverseLerp(maxAutoModeInterval, minAutoModeInterval, interval);
            autoModeSpeedSlider.SetValueWithoutNotify(sliderValue);
        }

        if (autoModeSpeedLabel != null)
        {
            autoModeSpeedLabel.text = AutoSpeedPrefix + storyManager.AutoModeInterval.ToString("0.0") + SecondsText;
        }
    }

    private void ApplySettingsFont()
    {
        if (settingsFont == null)
        {
            return;
        }

        ApplyFont(autoModeLabel);
        ApplyFont(autoModeSpeedLabel);

        if (extraJapaneseTexts == null)
        {
            return;
        }

        foreach (TextMeshProUGUI text in extraJapaneseTexts)
        {
            ApplyFont(text);
        }
    }

    private void ApplyFont(TextMeshProUGUI text)
    {
        if (text != null)
        {
            text.font = settingsFont;
        }
    }

    private void PlayMenuButtonSound()
    {
        if (menuAudioSource == null || menuButtonSound == null)
        {
            return;
        }

        menuAudioSource.PlayOneShot(menuButtonSound);
    }
}
