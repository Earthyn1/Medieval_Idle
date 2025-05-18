using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;
using System;
using Unity.VisualScripting;
using System.Linq;

public class Populate_Storage_Slots : MonoBehaviour
{
    public Transform parentContainer;
    public GameObject scrollWindow;
    public GameObject slotPrefab;  // Public reference to your slot prefab

    public void PopulateItemSlots()
    { 
        DataGameManager.instance.populate_Camp_Slots.scrollWindow.SetActive(false);
        scrollWindow.SetActive(true);
        ScrollRect scrollRect = scrollWindow.GetComponentInParent<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 1f;  // Set scroll to top

        // Clear existing slots in the UI
        foreach (Transform child in DataGameManager.instance.populate_Camp_Slots.parentContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in parentContainer)
        {
            Destroy(child.gameObject);
        }
       
        int slotCount = 0;

        // Cycle through the pre-initialized storage list
        foreach (var item in DataGameManager.instance.TownStorage_List)
        {
            GameObject newSlot = Instantiate(slotPrefab, parentContainer);
            Item_Slot_Base slotScript = newSlot.GetComponent<Item_Slot_Base>();

            if (!string.IsNullOrEmpty(item.ItemID) && item.Quantity > 0) // Check if the slot has a valid item
            {
                newSlot.name = $"Slot_{item.ItemID}";
                slotScript.itemDataBasic = item;
                slotScript.SetupAsActive();
                slotCount++;
            }
            else
            {
                newSlot.name = $"Slot_Empty_{slotCount}";
                slotScript.SetAsEmpty();
            }
        }

    }



}
