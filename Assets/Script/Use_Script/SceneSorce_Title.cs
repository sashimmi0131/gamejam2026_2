using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSorce_Title : SceneBase
{
    [SerializeField] private Button startButton;
    [SerializeField] private string mainSceneName = "MainScene";

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startSound;

    [Header("Title BGM")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioClip titleBgm;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 1f;
    [SerializeField] private bool fadeOutBgmOnStart = true;

    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Color fadeColor = Color.black;

    private bool isStarting;

    private void Awake()
    {
        ApplySavedVolumes();
        SetFadeAlpha(0f);
    }

    private void Start()
    {
        PlayTitleBgm();

        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
    }

    public void StartGame()
    {
        if (isStarting)
        {
            return;
        }

        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        isStarting = true;

        if (startButton != null)
        {
            startButton.interactable = false;
        }

        PlayStartSound();

        if (fadeImage != null)
        {
            yield return FadeOut();
        }
        else if (fadeOutBgmOnStart && bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            yield return FadeBgmVolume(bgmAudioSource.volume, 0f, fadeDuration);
        }

        SceneManager.LoadScene(mainSceneName);
    }

    private void PlayTitleBgm()
    {
        ApplySavedVolumes();

        if (titleBgm == null)
        {
            return;
        }

        AudioSource source = GetBgmAudioSource();

        if (source == null)
        {
            return;
        }

        source.clip = titleBgm;
        source.loop = true;
        source.playOnAwake = false;
        source.volume = bgmVolume;
        source.Play();
    }

    private AudioSource GetBgmAudioSource()
    {
        if (bgmAudioSource == null)
        {
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
        }

        return bgmAudioSource;
    }

    private void PlayStartSound()
    {
        if (audioSource == null || startSound == null)
        {
            return;
        }

        audioSource.volume = SettingsMenuManager.CurrentSeVolume;
        audioSource.PlayOneShot(startSound);
    }

    private void ApplySavedVolumes()
    {
        SettingsMenuManager.LoadSavedSettings();
        bgmVolume = SettingsMenuManager.CurrentBgmVolume;

        if (audioSource != null)
        {
            audioSource.volume = SettingsMenuManager.CurrentSeVolume;
        }

        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = bgmVolume;
        }
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        float startBgmVolume = bgmAudioSource != null ? bgmAudioSource.volume : 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / Mathf.Max(0.01f, fadeDuration));
            SetFadeAlpha(alpha);

            if (fadeOutBgmOnStart && bgmAudioSource != null && bgmAudioSource.isPlaying)
            {
                bgmAudioSource.volume = Mathf.Lerp(startBgmVolume, 0f, alpha);
            }

            yield return null;
        }

        SetFadeAlpha(1f);

        if (fadeOutBgmOnStart && bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.volume = 0f;
        }
    }

    private IEnumerator FadeBgmVolume(float from, float to, float duration)
    {
        float timer = 0f;
        float safeDuration = Mathf.Max(0.01f, duration);

        while (timer < safeDuration)
        {
            timer += Time.deltaTime;

            if (bgmAudioSource != null)
            {
                bgmAudioSource.volume = Mathf.Lerp(from, to, timer / safeDuration);
            }

            yield return null;
        }

        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = to;
        }
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage == null)
        {
            return;
        }

        Color color = fadeColor;
        color.a = alpha;
        fadeImage.color = color;
        fadeImage.raycastTarget = alpha > 0f;
    }
}
