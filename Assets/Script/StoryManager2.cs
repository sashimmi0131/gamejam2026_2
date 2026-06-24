using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private AudioSource choiceAudioSource;
    [SerializeField] private AudioClip choiceSound;

    [Header("Backlog")]
    [SerializeField] private BacklogManager backlogManager;

    [Header("Typewriter")]
    [SerializeField] private float typewriterSpeed = 0.05f;
    [SerializeField] private AudioSource typewriterAudioSource;
    [SerializeField] private AudioClip typewriterSound;
    [SerializeField] private int soundInterval = 1;

    private Coroutine typewriterCoroutine;
    private Story currentStoryElement;
    private string currentStoryText = "";
    private bool isTyping;
    private int loggedStoryIndex = -1;
    private int loggedTextIndex = -1;

    public int storyIndex { get; private set; }
    public int textIndex { get; private set; }

    private void Start()
    {
        ApplyBacklogFont();

        if (backlogManager != null)
        {
            backlogManager.BeforeBacklogOpen += AddCurrentStoryToBacklog;
        }

        SetStoryElement(storyIndex, textIndex);
    }

    private void OnDestroy()
    {
        if (backlogManager != null)
        {
            backlogManager.BeforeBacklogOpen -= AddCurrentStoryToBacklog;
        }
    }

    private void Update()
    {
        if (ShouldReadNext())
        {
            ReadNext();
        }
    }

    private bool ShouldReadNext()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            return true;
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return false;
        }

        if (IsPointerOverControlUi())
        {
            return false;
        }

        return true;
    }

    private bool IsPointerOverControlUi()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            GameObject hitObject = result.gameObject;

            if (hitObject.GetComponentInParent<Button>() != null ||
                hitObject.GetComponentInParent<Toggle>() != null ||
                hitObject.GetComponentInParent<Slider>() != null ||
                hitObject.GetComponentInParent<Scrollbar>() != null ||
                hitObject.GetComponentInParent<TMP_Dropdown>() != null ||
                hitObject.GetComponentInParent<ScrollRect>() != null)
            {
                return true;
            }
        }

        return false;
    }

    private void ReadNext()
    {
        if (backlogManager != null && backlogManager.IsOpen)
        {
            return;
        }

        if (isTyping)
        {
            CompleteTypewriterText();
            return;
        }

        if (currentStoryElement != null && currentStoryElement.isChoice)
        {
            return;
        }

        AddCurrentStoryToBacklog();
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

    private void SelectChoice(int nextStoryIndex, string choiceText)
    {
        AddCurrentStoryToBacklog();
        AddChoiceToBacklog(choiceText);
        PlayChoiceSound();
        ChangeStory(nextStoryIndex);
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
        btn1.onClick.AddListener(() => SelectChoice(currentStoryElement.targetIndex1, currentStoryElement.choiceText1));

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
            btn2.onClick.AddListener(() => SelectChoice(currentStoryElement.targetIndex2, currentStoryElement.choiceText2));
        }
    }

    private void AddCurrentStoryToBacklog()
    {
        if (backlogManager == null || currentStoryElement == null)
        {
            return;
        }

        if (loggedStoryIndex == storyIndex && loggedTextIndex == textIndex)
        {
            return;
        }

        backlogManager.AddLog(currentStoryElement.CharacterName, currentStoryElement.StoryText);
        loggedStoryIndex = storyIndex;
        loggedTextIndex = textIndex;
    }

    private void AddChoiceToBacklog(string choiceText)
    {
        if (backlogManager != null)
        {
            backlogManager.AddChoiceLog(choiceText);
        }
    }

    private void ApplyBacklogFont()
    {
        if (backlogManager != null && storyText != null)
        {
            backlogManager.SetLogFont(storyText.font);
        }
    }

    private void PlayChoiceSound()
    {
        if (choiceAudioSource == null || choiceSound == null)
        {
            return;
        }

        choiceAudioSource.PlayOneShot(choiceSound);
    }

    private void HideChoicePanel()
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
    }
}
