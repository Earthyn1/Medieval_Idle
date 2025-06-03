using System.Linq;
using UnityEngine;

public class DropDownMenu : MonoBehaviour
{
    public GameObject MenuVerticalHolder;
    public Animator Animator;
    public GameObject BGDimmer;
    public GameObject slotPrefab;
    public GameObject parentButton;
    void Start()
    {
        DataGameManager.instance.DropDownMenu = this;
    }

    public void PlayAnimation_Open()
    {
        Animator.Play("IdleState", 0, 0f);
        Animator.ResetTrigger("PlayAnimation_Open");
        Animator.SetTrigger("PlayAnimation_Open");
    }
    public void PlayAnimation_Close()
    {
        Animator.Play("IdleState", 0, 0f);
        Animator.ResetTrigger("PlayAnimation_Close");
        Animator.SetTrigger("PlayAnimation_Close");
        BGDimmer.SetActive(false);
    }

    public void PopulateSlots()
    {
        foreach (Transform child in MenuVerticalHolder.transform) // Clear existing slots
        {
            Destroy(child.gameObject);
        }

        foreach (StorageSlot storageslot in DataGameManager.instance.TownStorage_List)
        {
            int CurrentAmount = storageslot.Quantity;

            ItemData_Struc itemdata;
            if(storageslot.ItemID != null)
            {
                if (DataGameManager.instance.itemData_Array.TryGetValue(storageslot.ItemID, out itemdata))
                {
                    if (itemdata.ItemType == ItemType.Bait)
                    {
                        GameObject newSlot = Instantiate(slotPrefab, MenuVerticalHolder.transform);
                        DropDownMenu_Slot slotScript = newSlot.GetComponent<DropDownMenu_Slot>();
                        slotScript.SetupSlot(itemdata, CurrentAmount, CampType.FishingCamp, parentButton);
                        slotScript.dropDownMenu = this;
                        slotScript.itemID = storageslot.ItemID;
                    }
                }

            }

            BGDimmer.SetActive(true);
        }
    }
}
