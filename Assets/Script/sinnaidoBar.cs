using UnityEngine;
using UnityEngine.UI;

public class sinnaidoBarScript : MonoBehaviour
{
    public Image barImage;

    // 0～3の画像を順番に登録
    public Sprite[] barSprites;

    [SerializeField,Range(0, 3)]
    public static int sinnaido = 0;
    public int Sinnaido => sinnaido;//外部から変数を読み取れるように書き換えました。

    private void Start()
    {
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
        Debug.Log("マイナスボタンが押されました！現在の親愛度: " + sinnaido);
        ChangeSinnaido(-1); // 1減らす
    }

    private void UpdateBar()
    {
        if (barSprites != null && sinnaido < barSprites.Length && barImage != null)
        {
            barImage.sprite = barSprites[sinnaido];
        }
    }
}