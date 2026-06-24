using UnityEngine;
using UnityEngine.UI; // Imageコンポーネントを操作するために必要

public class BranchingEndingScript : MonoBehaviour
{
    public sinnaidoBarScript sinnaidoScript;

    [Header("全画面表示用の一枚絵Image")]
    public Image endingPanelImage;

    [Header("各エンディングの画像（スチル）")]
    public Sprite happyEndingSprite;  // 好感度3以上
    public Sprite friendEndingSprite; // 好感度2
    public Sprite badEndingSprite;    // 好感度1以下

    // ★ストーリーの最後（告白タイミング）でこのメソッドを呼び出す
    public void ShowEnding()
    {
        // 1. まず一枚絵のオブジェクトを表示（アクティブに）する
        endingPanelImage.gameObject.SetActive(true);

        // 2. 現在の親愛度（好感度）を取得
        int currentSinnaido = sinnaidoScript.sinnaido;

        // 3. 好感度に応じて画像を切り替える
        if (currentSinnaido >= 3)
        {
            // 🌸 HAPPY END（卒業式告白スチル）
            endingPanelImage.sprite = happyEndingSprite;
            Debug.Log("HAPPY END の一枚絵を表示しました");
        }
        else if (currentSinnaido == 2)
        {
            // 😢 ヒロインBAD END（過去形の告白スチル）
            endingPanelImage.sprite = friendEndingSprite;
            Debug.Log("ヒロインBAD END の一枚絵を表示しました");
        }
        else
        {
            // 👬 親友 END（相棒と校门を出るスチル）
            endingPanelImage.sprite = badEndingSprite;
            Debug.Log("親友 END の一枚絵を表示しました");
        }
    }
}