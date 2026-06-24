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
        var storyElement = storyDatas[_storyIndex].stories[_textIndex];

        if (storyElement.CharacterImage == null)//debaggu
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

            //ボタン1の処理（常に表示)
            btn1.gameObject.SetActive(true);
            btn1.GetComponentInChildren<TextMeshProUGUI>().text = storyElement.choiceText1;
            btn1.onClick.RemoveAllListeners();
            btn1.onClick.AddListener(() => ChangeStory(storyElement.targetIndex1));

            //ボタン2の処理（文字が入っている時だけ表示）
            if (string.IsNullOrEmpty(storyElement.choiceText2))
            {
                btn2.gameObject.SetActive(false); 
            }
            else
            {
                btn2.gameObject.SetActive(true); 
                btn2.GetComponentInChildren<TextMeshProUGUI>().text = storyElement.choiceText2;
                btn2.onClick.RemoveAllListeners();
                btn2.onClick.AddListener(() => ChangeStory(storyElement.targetIndex2));
            }
        }
        else
        {
            choicePanel.SetActive(false); // 選択肢がない画面ならパネルごと消す
        }
      
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

    private void Update()//ページ送り機能
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            textIndex++; // 次の行へ

            if (textIndex >= storyDatas[storyIndex].stories.Count)
            {
               
                ProgressionStory(storyIndex);
            }
            else
            {
              
                SetStoryElement(storyIndex, textIndex);
            }
        }
      
    }

    private void ProgressionStory(int _storyIndex)
    {
        textIndex++;

        
        if (textIndex < storyDatas[_storyIndex].stories.Count)  //つぎのテキストが残っていればSetStoryを読んでつぎのページに行く
        {
            SetStoryElement(_storyIndex, textIndex);
        }
        else//じゃなければ次のストーリーに自動で行く。
        {
        
            textIndex = 0;
            storyIndex++;
            SetStoryElement(storyIndex, textIndex);
        }
    }

   

}