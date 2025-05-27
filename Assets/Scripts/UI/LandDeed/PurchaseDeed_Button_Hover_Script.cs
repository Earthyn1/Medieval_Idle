using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_Hover_Script : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CanvasGroup puchaseDeedCanvasGroup;
    public Image puchaseDeedShadow;
    public GameObject purchaseDeedParent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        puchaseDeedCanvasGroup.alpha = 1;
        puchaseDeedShadow.color = new Color(puchaseDeedShadow.color.r, puchaseDeedShadow.color.g, puchaseDeedShadow.color.b, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        puchaseDeedCanvasGroup.alpha = 0.7f;
        puchaseDeedShadow.color = new Color(puchaseDeedShadow.color.r, puchaseDeedShadow.color.g, puchaseDeedShadow.color.b, 0.1f);

    }

    public void OnButtonClicked()
    {
        LandDeed_Manager landDeed_Manager = purchaseDeedParent.GetComponent<LandDeed_Manager>();
        landDeed_Manager.PurchaseDeed();
        
    }
}

