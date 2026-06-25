using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "StoryData")]
public class StoryData : ScriptableObject
{
    [Header("BGM")]
    public AudioClip bgm;
    public bool stopBgm;

    public List<Story> stories = new List<Story>();
}
[System.Serializable]
public class Story
{
    public Sprite Background;
    public Sprite CharacterImage;
    [TextArea]
    public string StoryText;
    public string CharacterName;

    public bool isChoice;          // 選択肢が出る画面かどうか
    public string choiceText1;     // 選択肢1の文字
    public int targetIndex1;       // 選んだら何番目のシーン(storyIndex)に飛ぶか
    public string choiceText2;     // 選択肢2の文字
    public int targetIndex2;       // 選んだら何番目のシーン(storyIndex)に飛ぶか
}
