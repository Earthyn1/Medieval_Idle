using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Required_Resource_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image itemimage;
    public Image itemimage_2;
    public Text itemqty;
    public string itemID;
    public Animator animator;

    public void FlashRedAnimation()
    {
        animator.Play("IdleState", 0, 0f);
        animator.ResetTrigger("FlashRed");
        animator.SetTrigger("FlashRed");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.instance.ShowTooltipBelow(transform as RectTransform, itemID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.instance.Hide();
    }

}
