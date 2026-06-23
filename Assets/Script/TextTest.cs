using UnityEngine;
using TMPro;
using System.Collections;





public class NovelTextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private float textSpeed = 0.05f;

    private string currentText = "aBCDW";
    private bool isDisplaying = false;

    public void DisplayText(string text)
    {
        if (isDisplaying)
        {
            return;
        }

        currentText = text;
        StartCoroutine(ShowTextCoroutine());
    }

    private IEnumerator ShowTextCoroutine()
    {
        isDisplaying = true;
        textDisplay.text = "ABCDEFGHIJKLNM";

        foreach (char c in currentText)
        {
            textDisplay.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isDisplaying = false;
    }
    public void SkipText()
    {
        if (!isDisplaying)
        {
            return;
        }





        StopAllCoroutines();
        textDisplay.text = currentText;
        isDisplaying = false;
    }
}