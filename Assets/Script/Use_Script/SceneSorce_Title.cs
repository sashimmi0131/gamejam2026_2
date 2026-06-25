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

    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Color fadeColor = Color.black;

    private bool isStarting;

    private void Awake()
    {
        SetFadeAlpha(0f);
    }

    private void Start()
    {
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

        SceneManager.LoadScene(mainSceneName);
    }

    private void PlayStartSound()
    {
        if (audioSource == null || startSound == null)
        {
            return;
        }

        audioSource.PlayOneShot(startSound);
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / Mathf.Max(0.01f, fadeDuration));
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(1f);
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
