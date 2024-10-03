using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private LTDescr delay;
    public string header;
    public string content;
    public int price;

    public void OnPointerEnter(PointerEventData eventData)
    {
        delay = LeanTween.delayedCall(0.2f, () =>
        {
            TooltipSystem.Show(header, content, UIManager.FormatNumberAsK(price));
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delay.uniqueId);
        TooltipSystem.Hide();
    }
}
