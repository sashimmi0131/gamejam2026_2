using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NovelManager : MonoBehaviour
{
    [System.Serializable]
    public class ScenarioLine
    {
        public string speaker;
        public string text;

        public string background;
        public string characterImage;
    }

    [Header("Text UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI messageText;

    [Header("Image UI")]
    public Image backgroundImage;
    public Image characterImage;

    [Header("Typewriter")]
    public float textSpeed = 0.05f;

    private int currentLine = 0;

    private bool isTyping = false;
    private bool canNext = false;

    private Coroutine typingCoroutine;

    [Header("Scenario")]
    public ScenarioLine[] scenario =
    {
        new ScenarioLine
        {
            speaker = "",
            text = "春の日だった。",
            background = "school"
        },

        new ScenarioLine
        {
            speaker = "美咲",
            text = "おはよう！",
            background = "school",
            characterImage = "misaki"
        },

        new ScenarioLine
        {
            speaker = "主人公",
            text = "おはよう。",
            background = "school"
        },

        new ScenarioLine
        {
            speaker = "タケシ",
            text = "よう！",
            background = "school",
            characterImage = "takeshi"
        }
    };

    private void Start()
    {
        ShowLine();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        if (isTyping)
        {
            CompleteText();
            return;
        }

        if (canNext)
        {
            NextLine();
        }
    }

    private void ShowLine()
    {
        ScenarioLine line = scenario[currentLine];

        nameText.text = line.speaker;

        UpdateVisuals(line);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(line.text));
    }

    private void UpdateVisuals(ScenarioLine line)
    {
        // 背景変更
        if (!string.IsNullOrEmpty(line.background))
        {
            Sprite bg = Resources.Load<Sprite>(
                "Backgrounds/" + line.background
            );

            if (bg != null)
            {
                backgroundImage.sprite = bg;
            }
        }

        // キャラクター変更
        if (!string.IsNullOrEmpty(line.characterImage))
        {
            Sprite character = Resources.Load<Sprite>(
                "Characters/" + line.characterImage
            );

            if (character != null)
            {
                characterImage.sprite = character;
                characterImage.enabled = true;
            }
        }
        else
        {
            characterImage.enabled = false;
        }
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        canNext = false;

        messageText.text = "";

        foreach (char c in text)
        {
            messageText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
        canNext = true;
    }

    private void CompleteText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        messageText.text = scenario[currentLine].text;

        isTyping = false;
        canNext = true;
    }

    private void NextLine()
    {
        currentLine++;

        if (currentLine >= scenario.Length)
        {
            Debug.Log("シナリオ終了");
            return;
        }

        ShowLine();
    }
}