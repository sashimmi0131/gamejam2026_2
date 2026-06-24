using UnityEngine;
using UnityEngine.SceneManagement;

public class BranchingEndingScript : MonoBehaviour
{
    public sinnaidoBarScript sinnaidoScript;

    
    public void GoToEnding()
    {
        // 現在の親愛度の数値をチェックする
        int currentSinnaido = sinnaidoScript.sinnaido;

        // 【条件分岐】
        if (currentSinnaido >= 3)
        {
            // 親愛度3以上：ハッピーエンド
            SceneManager.LoadScene("HappyEnding");
        }
        else if (currentSinnaido == 2)
        {
            // 親愛度2ちょうど：親友エンド
            SceneManager.LoadScene("SinyuEnding");
        }
        else
        {
            // それ以下（0または1）：バッドエンド
            SceneManager.LoadScene("BadEnding");
        }
    }
}