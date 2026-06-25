using NUnit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sinnaidoBarScript : MonoBehaviour
{
    [SerializeField] private StoryManager2 storyManager;
    public Image barImage;


    // 0～3の画像を順番に登録
    public Sprite[] barSprites;



    [SerializeField, Range(0, 3)]
    public static int sinnaido = 0;
    public int Sinnaido => sinnaido;//外部から変数を読み取れるように書き換えました。

    private void Start()
    {
        sinnaido = 0;
        UpdateBar();
    }

    public void ChangeSinnaido(int value)
    {
        sinnaido += value;
        sinnaido = Mathf.Clamp(sinnaido, 0, 3);
        UpdateBar();
    }

    public void IncreaseSinnaido()
    {

        Debug.Log("プラスボタンが押されました！現在の親愛度: " + sinnaido);
        ChangeSinnaido(1); // 1増やす
    }

    public void DecreaseSinnaido()
    {
        if (storyManager.storyIndex == 1 && storyManager.textIndex == 10
            || storyManager.storyIndex == 4 && storyManager.textIndex == 22
            || storyManager.storyIndex == 7 && storyManager.textIndex == 24
            || storyManager.storyIndex == 2 && storyManager.textIndex == 2
            || storyManager.storyIndex == 3 && storyManager.textIndex == 2
            || storyManager.storyIndex == 5 && storyManager.textIndex == 2
            || storyManager.storyIndex == 6 && storyManager.textIndex == 2
            || storyManager.storyIndex == 8 && storyManager.textIndex == 2
            || storyManager.storyIndex == 9 && storyManager.textIndex == 2)
        {
            //return;
            Debug.Log("マイナスボタンが押されました！現在の親愛度: " + sinnaido);
            ChangeSinnaido(-1); // 1減らす
        }



    }

    private void UpdateBar()
    {
        Debug.Log(sinnaido);
        if (barSprites != null && sinnaido < barSprites.Length && barImage != null)
        {
            barImage.sprite = barSprites[sinnaido];
        }
    }
}