using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager2 : MonoBehaviour
{
    [SerializeField] private StoryData[] storyDatas;
    [SerializeField] private Image background;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI characterName;

    [Header("Choice")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button btn1;
    [SerializeField] private Button btn2;

    [Header("Typewriter")]
    [SerializeField] private float typewriterSpeed = 0.05f;
    [SerializeField] private AudioSource typewriterAudioSource;
    [SerializeField] private AudioClip typewriterSound;
    [SerializeField] private int soundInterval = 1;

    private Coroutine typewriterCoroutine;
    private Story currentStoryElement;
    private string currentStoryText = "";
    private bool isTyping;

    public int storyIndex { get; private set; }
    public int textIndex { get; private set; }

    private void Start()
    {
        SetStoryElement(storyIndex, textIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            ReadNext();
        }
    }

    private void ReadNext()
    {
        if (isTyping)
        {
            CompleteTypewriterText();
            return;
        }

        if (currentStoryElement != null && currentStoryElement.isChoice)
        {
            return;
        }

        textIndex++;
        ProgressionStory();
    }

    private void SetStoryElement(int _storyIndex, int _textIndex)
    {
        if (_storyIndex < 0 || _storyIndex >= storyDatas.Length)
        {
            Debug.Log("Story index is out of range.");
            return;
        }

        if (_textIndex < 0 || _textIndex >= storyDatas[_storyIndex].stories.Count)
        {
            Debug.Log("Text index is out of range.");
            return;
        }

        currentStoryElement = storyDatas[_storyIndex].stories[_textIndex];

        if (currentStoryElement.CharacterImage == null)
        {
            Debug.Log("Character image is not set.");
        }

        background.sprite = currentStoryElement.Background;
        characterImage.sprite = currentStoryElement.CharacterImage;
        characterName.text = currentStoryElement.CharacterName;

        HideChoicePanel();
        StartTypewriterText(currentStoryElement.StoryText);
    }

    public void ChangeStory(int nextStoryIndex)
    {
        storyIndex = nextStoryIndex;
        textIndex = 0;
        HideChoicePanel();
        SetStoryElement(storyIndex, textIndex);
    }

    private void ProgressionStory()
    {
        if (storyIndex < 0 || storyIndex >= storyDatas.Length)
        {
            Debug.Log("Story index is out of range.");
            return;
        }

        if (textIndex < storyDatas[storyIndex].stories.Count)
        {
            SetStoryElement(storyIndex, textIndex);
            return;
        }

        textIndex = 0;
        storyIndex++;

        if (storyIndex < storyDatas.Length)
        {
            SetStoryElement(storyIndex, textIndex);
        }
        else
        {
            Debug.Log("All stories finished.");
        }
    }

    private void StartTypewriterText(string text)
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        currentStoryText = text ?? "";
        typewriterCoroutine = StartCoroutine(TypewriterText());
    }

    private IEnumerator TypewriterText()
    {
        isTyping = true;
        storyText.text = "";

        int visibleCharacterCount = 0;

        foreach (char character in currentStoryText)
        {
            storyText.text += character;
            visibleCharacterCount++;
            PlayTypewriterSound(visibleCharacterCount, character);

            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
        typewriterCoroutine = null;
        ShowChoicePanelIfNeeded();
    }

    private void CompleteTypewriterText()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        storyText.text = currentStoryText;
        isTyping = false;
        ShowChoicePanelIfNeeded();
    }

    private void PlayTypewriterSound(int visibleCharacterCount, char character)
    {
        if (typewriterAudioSource == null || typewriterSound == null)
        {
            return;
        }

        if (char.IsWhiteSpace(character))
        {
            return;
        }

        int interval = Mathf.Max(1, soundInterval);

        if (visibleCharacterCount % interval == 0)
        {
            typewriterAudioSource.PlayOneShot(typewriterSound);
        }
    }

    private void ShowChoicePanelIfNeeded()
    {
        if (currentStoryElement == null || !currentStoryElement.isChoice)
        {
            HideChoicePanel();
            return;
        }

        if (choicePanel == null || btn1 == null || btn2 == null)
        {
            Debug.Log("Choice UI is not set.");
            return;
        }

        choicePanel.SetActive(true);

        btn1.gameObject.SetActive(true);
        btn1.GetComponentInChildren<TextMeshProUGUI>().text = currentStoryElement.choiceText1;
        btn1.onClick.RemoveAllListeners();
        btn1.onClick.AddListener(() => ChangeStory(currentStoryElement.targetIndex1));

        if (string.IsNullOrEmpty(currentStoryElement.choiceText2))
        {
            btn2.gameObject.SetActive(false);
            btn2.onClick.RemoveAllListeners();
        }
        else
        {
            btn2.gameObject.SetActive(true);
            btn2.GetComponentInChildren<TextMeshProUGUI>().text = currentStoryElement.choiceText2;
            btn2.onClick.RemoveAllListeners();
            btn2.onClick.AddListener(() => ChangeStory(currentStoryElement.targetIndex2));
        }
    }

    private void HideChoicePanel()
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
    }
}
