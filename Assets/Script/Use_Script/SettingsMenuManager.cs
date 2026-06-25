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
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private TextMeshProUGUI bgmVolumeLabel;
    [SerializeField] private Slider seVolumeSlider;
    [SerializeField] private TextMeshProUGUI seVolumeLabel;
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
    [SerializeField] private AudioSource[] seAudioSources;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float seVolume = 1f;

    private const string AutoOnText = "\u81ea\u52d5\u8aad\u307f\u4e0a\u3052\uff1a\u30aa\u30f3";
    private const string AutoOffText = "\u81ea\u52d5\u8aad\u307f\u4e0a\u3052\uff1a\u30aa\u30d5";
    private const string AutoSpeedPrefix = "\u8aad\u307f\u4e0a\u3052\u9593\u9694\uff1a";
    private const string BgmVolumePrefix = "BGM\u97f3\u91cf\uff1a";
    private const string SeVolumePrefix = "SE\u97f3\u91cf\uff1a";
    private const string NotSetText = "\u672a\u8a2d\u5b9a";
    private const string SecondsText = "\u79d2";
    private const string PercentText = "%";

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
        SetupVolumeSliders();
        ApplyVolumeSettings();
        RefreshAutoModeView();
        RefreshAutoModeSpeedView();
        RefreshVolumeView();
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
        RefreshVolumeView();
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

    public void SetBgmVolume(float value)
    {
        bgmVolume = Mathf.Clamp01(value);

        if (storyManager != null)
        {
            storyManager.SetBgmVolume(bgmVolume);
        }

        RefreshVolumeView();
    }

    public void SetSeVolume(float value)
    {
        seVolume = Mathf.Clamp01(value);
        ApplySeVolume();
        RefreshVolumeView();
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

    private void SetupVolumeSliders()
    {
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.minValue = 0f;
            bgmVolumeSlider.maxValue = 1f;
            bgmVolumeSlider.onValueChanged.RemoveListener(SetBgmVolume);
            bgmVolumeSlider.onValueChanged.AddListener(SetBgmVolume);
            bgmVolumeSlider.SetValueWithoutNotify(bgmVolume);
        }

        if (seVolumeSlider != null)
        {
            seVolumeSlider.minValue = 0f;
            seVolumeSlider.maxValue = 1f;
            seVolumeSlider.onValueChanged.RemoveListener(SetSeVolume);
            seVolumeSlider.onValueChanged.AddListener(SetSeVolume);
            seVolumeSlider.SetValueWithoutNotify(seVolume);
        }
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

    private void ApplyVolumeSettings()
    {
        if (storyManager != null)
        {
            storyManager.SetBgmVolume(bgmVolume);
        }

        ApplySeVolume();
    }

    private void ApplySeVolume()
    {
        if (menuAudioSource != null)
        {
            menuAudioSource.volume = seVolume;
        }

        if (seAudioSources == null)
        {
            return;
        }

        foreach (AudioSource audioSource in seAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.volume = seVolume;
            }
        }
    }

    private void RefreshVolumeView()
    {
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.SetValueWithoutNotify(bgmVolume);
        }

        if (seVolumeSlider != null)
        {
            seVolumeSlider.SetValueWithoutNotify(seVolume);
        }

        if (bgmVolumeLabel != null)
        {
            bgmVolumeLabel.text = BgmVolumePrefix + Mathf.RoundToInt(bgmVolume * 100f) + PercentText;
        }

        if (seVolumeLabel != null)
        {
            seVolumeLabel.text = SeVolumePrefix + Mathf.RoundToInt(seVolume * 100f) + PercentText;
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
        ApplyFont(bgmVolumeLabel);
        ApplyFont(seVolumeLabel);

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
