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
    //ストーリーのエレメント配列番号が必要なのでプロパティを
    public int storyIndex { get; private set; }
    public int textIndex { get; private set; }
    //Startで呼び出そう
    private void Start()
    {
        SetStoryElement(storyIndex, textIndex);
    }

    private void Update()
    {   
        //ページ送り
        if(Input.GetKeyDown(KeyCode.Return))
        {
            textIndex++;
            storyText.text = "";
            characterName.text = "";
            SetStoryElement(storyIndex, textIndex);
        }
    }

    private void ProgressionStory(int _storyIndex)
    {
        //ストーリーインデックスよりも大きいテキストは存在しないのでチェックして対応
        //最後まで行ったなら、次のお話などに進めたいですよね
        if (textIndex < storyDatas[_storyIndex].stories.Count)
        {
            //まだ大きくないなら次のインデックスを表示
            SetStoryElement(_storyIndex, textIndex);
        }
        else
        {
            //シーンチェンジや選択肢の表示。スクリプタブルオブジェクトを呼んだり。
            textIndex = 0;
            storyIndex++;//次のシーンへ
            SetStoryElement(storyIndex, textIndex);
        }
    }

    //呼び出しメソッド
    private void SetStoryElement(int _storyIndex, int _textIndex)
    {
        //同じ言葉をまとめておくためのvar
        var storyElement = storyDatas[_storyIndex].stories[_textIndex];
        if(storyElement.CharacterImage == null)
        {
            Debug.Log("キャラ画像がありません");
        }
        //どのストーリーデータの、どのバックグランドか
        background.sprite = storyElement.Background;
        //どのストーリーデータの、どのキャラクタか
        characterImage.sprite = storyElement.CharacterImage;
        //どのストーリーデータの、どのテキストか
        storyText.text = storyElement.StoryText;
        //どのストーリーデータの、どのキャラ名か
        characterName.text = storyElement.CharacterName;
    }
}