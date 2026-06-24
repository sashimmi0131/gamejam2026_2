using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class SceneSource : SceneBase
{
    [System.Serializable]
    private class SceneButton
    {
        public Button button;
        public SceneID nextScene;
    }

    StoryManager2 storyIndex;
    [SerializeField] private sinnaidoBarScript barScript;
    [SerializeField] private SceneButton[] sceneButtons;
    [SerializeField] private StoryManager2 storyManager;
    [SerializeField] private Button END;
    private void Update()
    {
        // 例: 特定の条件を満たしたら自動で表示する
        if (storyManager.storyIndex == 10 && storyManager.textIndex == 5)
        { 
            END.gameObject.SetActive(true);
        }
        else
        {
           END.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        for (int i = 0; i < sceneButtons.Length; i++)
        {
            SceneButton sceneButton = sceneButtons[i];

            if (sceneButton.button == null)
            {
                Debug.LogWarning($"Button が設定されていません。Index: {i}", this);
                continue;
            }
            sceneButton.button.onClick.AddListener(() =>
            {
                Debug.Log(barScript.Sinnaido);
                Debug.Log("error");
                if (sinnaidoBarScript.sinnaido >= 3)
                {
                    SceneChange(SceneID.HappyEND);
                    return;
                }
                else if (sinnaidoBarScript.sinnaido <= 0)
                {
                    SceneChange(SceneID.SinnyuuEND);
                    return;
                }
                else
                {
                    SceneChange(SceneID.BadEND);
                    return;
                }
            });
        } 
    }
    
   
}
   



 
