using UnityEngine;
using UnityEngine.EventSystems; // これが必要です

public class ChoiceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // マウスが乗った時
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ここで色を変えたり、拡大したりできます
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    // マウスが離れた時
    public void OnPointerExit(PointerEventData eventData)
    {
        // 元のサイズに戻す
        transform.localScale = Vector3.one;
    }
}