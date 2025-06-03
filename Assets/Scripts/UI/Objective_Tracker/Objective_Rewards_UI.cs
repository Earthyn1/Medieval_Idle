using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Objective_Rewards_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text item_XP_text;
    public Image image;
    public string itemName;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.instance.ShowTooltipBelow_Name(transform as RectTransform, itemName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.instance.Hide();
    }

}
