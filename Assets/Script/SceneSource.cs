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

    [SerializeField] private SceneButton[] sceneButtons;

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

            SceneID sceneID = sceneButton.nextScene;
            if (sceneID == SceneID.Back)
            {
                sceneButton.button.onClick.AddListener(() => BackScene());
                continue;
            }
            sceneButton.button.onClick.AddListener(() => ChangeScene(sceneID));
        }
    }

    private void ChangeScene(SceneID sceneID)
    {
        Time.timeScale = 1f;
        SceneChange(sceneID);
    }
    private void BackScene()
    {
        Time.timeScale = 1f;
        SceneBack();
    }
}