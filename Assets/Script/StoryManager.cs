using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private GameObject choicePanel; // ボタン2つをまとめたパネル
    [SerializeField] private Button btn1;
    [SerializeField] private Button btn2;
  

    private void SetStoryElement(int _storyIndex, int _textIndex)
    {
        if (_storyIndex < 0 || _storyIndex >= storyDatas.Length)
        {
            Debug.Log("Indexが範囲外です");
            return;
        }
        if(_textIndex <0 ||  _textIndex >= storyDatas[_storyIndex].stories.Count)
        {
            Debug.Log("テキストINDEXが範囲外です");
            return;
        }

        var storyElement = storyDatas[_storyIndex].stories[_textIndex];

        if (storyElement.CharacterImage == null)
        {
            Debug.Log("キャラ画像がありません");
        }

        background.sprite = storyElement.Background;
        characterImage.sprite = storyElement.CharacterImage;
        storyText.text = storyElement.StoryText;
        characterName.text = storyElement.CharacterName;
        // 選択肢かどうかチェック
        if (storyElement.isChoice)
        {
            choicePanel.SetActive(true);

            // --- ボタン1の処理（常に表示） ---
            btn1.gameObject.SetActive(true);
            btn1.GetComponentInChildren<TextMeshProUGUI>().text = storyElement.choiceText1;
            btn1.onClick.RemoveAllListeners();
            btn1.onClick.AddListener(() => ChangeStory(storyElement.targetIndex1));

            // --- ボタン2の処理（文字が入っている時だけ表示） ---
            if (string.IsNullOrEmpty(storyElement.choiceText2))
            {
                btn2.gameObject.SetActive(false); // 空なら消す
            }
            else
            {
                btn2.gameObject.SetActive(true); // 文字があるなら出す
                btn2.GetComponentInChildren<TextMeshProUGUI>().text = storyElement.choiceText2;
                btn2.onClick.RemoveAllListeners();
                btn2.onClick.AddListener(() => ChangeStory(storyElement.targetIndex2));
            }
        }
        else
        {
            choicePanel.SetActive(false); // 選択肢がない画面ならパネルごと消す
        }
        // ...（画像やテキストの反映処理はそのまま）
    }

    // 分岐後の処理
    public void ChangeStory(int nextStoryIndex)
    {
        storyIndex = nextStoryIndex;
        textIndex = 0;
        choicePanel.SetActive(false);
        SetStoryElement(storyIndex, textIndex);
    }

    public int storyIndex { get; private set; }
    public int textIndex { get; private set; }
    //Startで呼び出そう
    private void Start()
    {
        SetStoryElement(storyIndex, textIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            textIndex++; // 次の行へ

            // もし今の章のセリフ数を超えてしまったら…
            if (textIndex >= storyDatas[storyIndex].stories.Count)
            {
                // 次の章へ！
                ProgressionStory(storyIndex);
            }
            else
            {
                // まだセリフがあるなら今の章を表示
                SetStoryElement(storyIndex, textIndex);
            }
        }
      
    }

    private void ProgressionStory(int _storyIndex)
    {
        textIndex++;

        // 今の章の続きがあるか？
        if (textIndex < storyDatas[_storyIndex].stories.Count)
        {
            SetStoryElement(_storyIndex, textIndex);
        }
        else
        {
            // ここがポイント！今のデータの「次へ行く先」をインスペクターで持たせることもできます
            // もしくは単純に章を＋１する今のロジックでもOKです
            textIndex = 0;
            storyIndex++;
            SetStoryElement(storyIndex, textIndex);
        }
    }

    //呼び出しメソッド

}