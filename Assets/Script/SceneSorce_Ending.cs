using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSorce_Ending : SceneBase
{
    [SerializeField] private Button returnTitleButton;
    [SerializeField] private StoryManager2 storyManager;

    [SerializeField] private int targetStoryIndex = 0;
    [SerializeField] private int targetTextIndex = 8;

    private void Start()
    {
     
        if (returnTitleButton != null)
        {
            returnTitleButton.gameObject.SetActive(false);

            returnTitleButton.onClick.AddListener(() => {
                SceneManager.LoadScene("Title");
            });
        }
    }

    private void Update()
    {
       
        if (storyManager != null && !returnTitleButton.gameObject.activeSelf)
        {
           
            if (storyManager.storyIndex == targetStoryIndex && storyManager.textIndex == targetTextIndex)
            {
                returnTitleButton.gameObject.SetActive(true);
            }
        }
    }
}