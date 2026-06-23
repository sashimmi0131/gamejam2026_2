using UnityEngine;
using UnityEngine.UI;

public class sinnaidoBarScript : MonoBehaviour
{
    public Image barImage;

    public Sprite[] barSprites;

    public int sinnaido = 0;

    public void ChangeSinnaido(int value)
    {
        sinnaido += value;

        sinnaido = Mathf.Clamp(sinnaido, 0, barSprites.Length - 1);

        barImage.sprite = barSprites[sinnaido];
    }
}