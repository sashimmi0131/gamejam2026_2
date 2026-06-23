using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private StoryData[] storyDatas;
    [SerializeField] private Image background;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private float typewriterSpeed = 0.05f;

    private Coroutine typewriterCoroutine;
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
        }
        else
        {
            textIndex++;
            storyText.text = "";
            characterName.text = "";
            ProgressionStory(storyIndex);
        }
    }

    private void ProgressionStory(int _storyIndex)
    {
        if (_storyIndex >= storyDatas.Length)
        {
            Debug.Log("All stories finished.");
            return;
        }

        if (textIndex < storyDatas[_storyIndex].stories.Count)
        {
            SetStoryElement(_storyIndex, textIndex);
        }
        else
        {
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
    }

    private void SetStoryElement(int _storyIndex, int _textIndex)
    {
        Story storyElement = storyDatas[_storyIndex].stories[_textIndex];

        if (storyElement.CharacterImage == null)
        {
            Debug.Log("Character image is not set.");
        }

        background.sprite = storyElement.Background;
        characterImage.sprite = storyElement.CharacterImage;
        characterName.text = storyElement.CharacterName;

        StartTypewriterText(storyElement.StoryText);
    }

    private void StartTypewriterText(string text)
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        currentStoryText = text;
        typewriterCoroutine = StartCoroutine(TypewriterText());
    }

    private IEnumerator TypewriterText()
    {
        isTyping = true;
        storyText.text = "";

        foreach (char character in currentStoryText)
        {
            storyText.text += character;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
        typewriterCoroutine = null;
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
    }
}
