using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Blacksmith_FuelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator animator;
    public DropDownMenu DropDownPanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.Play("IdleState", 0, 0f);
        animator.ResetTrigger("Select");
        animator.SetTrigger("Select");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.Play("IdleState", 0, 0f);
        animator.ResetTrigger("Deselect");
        animator.SetTrigger("Deselect");
    }

    public void OnClicked()
    {
        DropDownPanel.PlayAnimation_Open();
        DropDownPanel.PopulateSlots();
    }
}
