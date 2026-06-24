using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSorce_Title : SceneBase
{
    [SerializeField] private Button startButton;
 


    private void Start()
    {
            startButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MainScene");
            });
    }
}

   