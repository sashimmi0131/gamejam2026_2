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
    [SerializeField] private SceneButton[] sceneButtons;
    [SerializeField] private StoryManager2 storyManager;
    [SerializeField] private Button END;
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
                Debug.Log("error");
                SceneChange(sceneButton.nextScene);
            });
            
        }
    } 
    
 

    private void Update()
    {
        if (storyManager != null)
        {
            Debug.Log(storyManager.storyIndex);
            Debug.Log(storyManager.textIndex);
            if (storyManager.storyIndex == 10 && storyManager.textIndex == 8)
            {
                END.gameObject.SetActive(true);
            }
        }
    }
}