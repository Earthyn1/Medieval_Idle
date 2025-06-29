using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DropDownMenu : MonoBehaviour
{
    public GameObject MenuVerticalHolder;
    public Animator Animator;
    public GameObject BGDimmer;
    public GameObject slotPrefab;
    public GameObject parentButton;
    public Text blacksmithHelperText;
    public Text fishingHelperText;

    void Start()
    {
        DataGameManager.instance.DropDownMenu = this;
    }

    public void PlayAnimation_Open()
    {
        DataGameManager.instance.IsDropDownMenuOpen = true;
        Animator.Play("IdleState", 0, 0f);
        Animator.ResetTrigger("PlayAnimation_Open");
        Animator.SetTrigger("PlayAnimation_Open");
        MenuVerticalHolder.SetActive(true);
    }
    public void PlayAnimation_Close()
    {
        DataGameManager.instance.IsDropDownMenuOpen = false;
        Animator.Play("IdleState", 0, 0f);
        Animator.ResetTrigger("PlayAnimation_Close");
        Animator.SetTrigger("PlayAnimation_Close");
        BGDimmer.SetActive(false);
        MenuVerticalHolder.SetActive(false);
        Bait_ToolTipUI.instance.Hide();
    }

    public void SetasDefault()
    {
        Debug.Log("Setasdefault");
        Animator.Play("IdleHere", 0, 0f);
   
        Animator.Update(0f); // <— this is key!
    }

    public void CampSpecificHelperPanels()
    {
        switch (DataGameManager.instance.currentActiveCamp)
        {
            case CampType.Blacksmith:
                // Do something specific for Blacksmith
                blacksmithHelperText.transform.gameObject.SetActive(true);
                fishingHelperText.transform.gameObject.SetActive(false);

                break;

            case CampType.FishingCamp:
                // Do something specific for Fishing
                blacksmithHelperText.transform.gameObject.SetActive(false);
                fishingHelperText.transform.gameObject.SetActive(false);
                break;

        }
    }

        public void PopulateSlots()
    {
        // Clear existing UI slots
        foreach (Transform child in MenuVerticalHolder.transform)
            Destroy(child.gameObject);

        // Get the item type filter for the current camp
        ItemUse? filterType = GetFilterTypeForCamp(DataGameManager.instance.currentActiveCamp);
        if (filterType == null)
            return;

        // Step 1: Combine quantities by ItemID
        Dictionary<string, int> combinedItems = new Dictionary<string, int>();

        foreach (StorageSlot storageslot in DataGameManager.instance.TownStorage_List)
        {
            if (storageslot.ItemID == null) continue;

            if (DataGameManager.instance.itemData_Array.TryGetValue(storageslot.ItemID, out ItemData_Struc itemdata))
            {
                if (itemdata.ItemUse == filterType)
                {
                    if (combinedItems.ContainsKey(storageslot.ItemID))
                        combinedItems[storageslot.ItemID] += storageslot.Quantity;
                    else
                        combinedItems[storageslot.ItemID] = storageslot.Quantity;
                }
            }
        }

        // Step 2: Create a dropdown slot for each unique item
        foreach (var kvp in combinedItems)
        {
            string itemID = kvp.Key;
            int totalQuantity = kvp.Value;

            if (DataGameManager.instance.itemData_Array.TryGetValue(itemID, out ItemData_Struc itemdata))
            {
                GameObject newSlot = Instantiate(slotPrefab, MenuVerticalHolder.transform);

                // ✅ Set the name of the GameObject
                newSlot.name = $"DropDownSlot_{itemID}";

                var slotScript = newSlot.GetComponent<DropDownMenu_Slot>();
                slotScript.SetupSlot(itemdata, totalQuantity, DataGameManager.instance.currentActiveCamp, parentButton);
                slotScript.dropDownMenu = this;
                slotScript.itemID = itemID;
                Debug.Log($"Created: {newSlot.name}");

            }
        }

        BGDimmer.SetActive(true);
    }

    private ItemUse? GetFilterTypeForCamp(CampType camp)
    {
        return camp switch
        {
            CampType.FishingCamp => ItemUse.Bait,
            CampType.Blacksmith => ItemUse.Fuel,
            _ => null
        };
    }
}

