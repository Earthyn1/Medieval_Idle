using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CampBoost_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text boostAmount;
    public Image boostImage;
    public string boostName;
    public string boostDescription;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetBoost(CampBoost_Class boost)
    {
        boostImage.sprite = boost.boostSprite;
 
        boostDescription = boost.boostDescription;
        boostName = boost.boostName;
        string formatted = boost.GetFormattedAmount();  // "25%"
        boostAmount.text = formatted;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.instance.ShowBoostInfoBelow(transform as RectTransform, boostName, boostDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.instance.Hide();
    }
}
