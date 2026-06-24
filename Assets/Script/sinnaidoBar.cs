using UnityEngine;
using UnityEngine.UI;

public class sinnaidoBarScript : MonoBehaviour
{
    public Image barImage;

    // 0～3の画像を順番に登録
    public Sprite[] barSprites;

    [Range(0, 3)]
    public int sinnaido = 0;

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

    private void UpdateBar()
    {
        barImage.sprite = barSprites[sinnaido];
    }
}