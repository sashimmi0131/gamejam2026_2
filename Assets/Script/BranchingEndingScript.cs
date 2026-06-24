using UnityEngine;

public class BranchingEndingScript : MonoBehaviour
{
    public sinnaidoBarScript sinnaidoScript;

    public StoryManager2 storyManager;

    [Header("各エンディングのStoryData番号(要素番号)")]
    public int happyEndingIndex = 6;  // 好感度3以上
    public int friendEndingIndex = 7; // 好感度2
    public int badEndingIndex = 8;    // 好感度1以下

    // ★ストーリーの最後（告白タイミング）でこのメソッドを呼び出す
    public void GoToEndingStory()
    {
        int currentSinnaido = sinnaidoScript.sinnaido;

        if (currentSinnaido >= 3)
        {
            storyManager.ChangeStory(happyEndingIndex);
        }
        else if (currentSinnaido == 2)
        {
            storyManager.ChangeStory(friendEndingIndex);
        }
        else
        {
            storyManager.ChangeStory(badEndingIndex);
        }

        gameObject.SetActive(false);
    }
}