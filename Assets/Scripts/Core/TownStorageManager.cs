using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;




public static class TownStorageManager
{
    public static Text storageQtyText;
    public static StorageSellManager storageSellManager;
    public static GameObject currentlySelectedInventorySlot;
    private static Coroutine uiRefreshCoroutine;

    public static bool AddItem(string itemID, int amount, CampType campType)
    {
        if (!DataGameManager.instance.itemData_Array.TryGetValue(itemID, out ItemData_Struc item))
        {
            Debug.LogWarning("Item ID not found: " + itemID);
            return false;
        }

        int remaining = amount;

        // Try to fill existing stacks first
        for (int i = 0; i < DataGameManager.instance.TownStorage_List.Count; i++)
        {
            var slot = DataGameManager.instance.TownStorage_List[i];
            if (slot.IsTutorialSlot) continue;

            if (slot.ItemID == itemID && slot.Quantity < item.MaxStack)
            {
                int space = item.MaxStack - slot.Quantity;

                if (remaining <= space)
                {
                    slot.Quantity += remaining;
                    DataGameManager.instance.TownStorage_List[i] = slot;
                    remaining = 0;
                    break;
                }
                else
                {
                    slot.Quantity = item.MaxStack;
                    DataGameManager.instance.TownStorage_List[i] = slot;
                    remaining -= space;
                }
            }
        }

        // Try to place the remaining items in empty slots
        for (int i = 0; i < DataGameManager.instance.TownStorage_List.Count && remaining > 0; i++)
        {
            var slot = DataGameManager.instance.TownStorage_List[i];
            if (slot.IsTutorialSlot) continue ;

            if (string.IsNullOrEmpty(slot.ItemID) || slot.Quantity == 0)
            {
                int toAdd = Mathf.Min(remaining, item.MaxStack);
                slot.ItemID = itemID;
                slot.Quantity = toAdd;
                DataGameManager.instance.TownStorage_List[i] = slot;
                remaining -= toAdd;
                
            }
        }

        bool success = remaining <= 0;

        if (!success)
        {
            Debug.LogWarning("Inventory full! Could not add all items.");
        }

        // Log and feed regardless of success (optional: wrap these in `if (success)` if needed)
        DataGameManager.instance.item_XP_FeedManager.AddItemFeedSlot(itemID, amount - remaining, campType);
        RefreshAllSlotsUI();
        Objective_Manager.UpdateObjectives(itemID, amount - remaining);

        return success;
    }

    public static bool Tutorial_AddItem(string itemID, int amount, CampType campType)
    {
        if (!DataGameManager.instance.itemData_Array.TryGetValue(itemID, out ItemData_Struc item))
        {
            Debug.LogWarning("Item ID not found: " + itemID);
            return false;
        }

        // Create a new tutorial slot
        StorageSlot tutorialSlot = default;
        tutorialSlot.ItemID = itemID;
        tutorialSlot.Quantity = amount;
        tutorialSlot.IsTutorialSlot = true;

        DataGameManager.instance.TownStorage_List.Add(tutorialSlot);

        // Log or update systems
        DataGameManager.instance.item_XP_FeedManager.AddItemFeedSlot(itemID, amount, campType);
        RefreshAllSlotsUI();
        Objective_Manager.UpdateObjectives(itemID, amount);
     


        return true;
    }

    public static void ClearTutorialSlots()
    {
        var storage = DataGameManager.instance.TownStorage_List;
        int beforeCount = storage.Count;

        storage.RemoveAll(slot => slot.IsTutorialSlot);

        int afterCount = storage.Count;
        Debug.Log($"[Tutorial] Cleared {beforeCount - afterCount} tutorial slot(s) from inventory.");

        RefreshAllSlotsUI(); // If your UI reflects storage, refresh it
    }


    public static void RemoveItem(string itemID, int amountToRemove)
    {
      
        int remainingToRemove = amountToRemove;

        // Get all slots with this itemID, sorted by quantity ascending, then index descending
        var slotsToRemoveFrom = DataGameManager.instance.TownStorage_List
            .Select((slot, index) => new { Slot = slot, Index = index })
            .Where(x => x.Slot.ItemID == itemID && x.Slot.Quantity > 0)
            .OrderBy(x => x.Slot.Quantity)      // smallest stacks first
            .ThenByDescending(x => x.Index)     // highest slot index first among equal qty
            .ToList();

        foreach (var entry in slotsToRemoveFrom)
        {
            if (remainingToRemove <= 0) break;

            // Get a reference to the slot from the list
            var slot = DataGameManager.instance.TownStorage_List[entry.Index];

            int qtyInSlot = slot.Quantity;

            if (qtyInSlot <= remainingToRemove)
            {
                // Remove entire slot quantity
                remainingToRemove -= qtyInSlot;
                slot.Quantity = 0;
                slot.ItemID = null; // Clear the item ID for empty slot
                DataGameManager.instance.TownStorage_List[entry.Index] = slot;
               // Debug.Log($"Removed full stack from slot {entry.Index}");
            }
            else
            {
                // Remove only part of slot quantity
                slot.Quantity -= remainingToRemove;
                remainingToRemove = 0;
                DataGameManager.instance.TownStorage_List[entry.Index] = slot;
               // Debug.Log($"Removed {amountToRemove} from slot {entry.Index}, remaining: {slot.Quantity}");
            }
        }

        // Refresh the UI after removal
        RefreshAllSlotsUI();
    }


    public static void RemoveItemFromSlot(int slotIndex, int amountToRemove)
    {
        // Validate the slot index
        if (slotIndex < 0 || slotIndex >= DataGameManager.instance.TownStorage_List.Count)
        {
            Debug.LogWarning("RemoveItemFromSlot: slotIndex out of range.");
            return;
        }

        var slot = DataGameManager.instance.TownStorage_List[slotIndex];

        if (slot.ItemID == null || slot.Quantity <= 0)
        {
            Debug.LogWarning($"RemoveItemFromSlot: Slot {slotIndex} is empty or invalid.");
            return;
        }

        if (amountToRemove >= slot.Quantity)
        {
            // Remove entire quantity from slot
            slot.ItemID = null;
            slot.Quantity = 0;

            // Update the slot with empty data instead of removing it
            DataGameManager.instance.TownStorage_List[slotIndex] = slot;
            Debug.Log($"Removed all items from slot {slotIndex}.");
        }
        else
        {
            // Remove partial quantity and update
            slot.Quantity -= amountToRemove;
            DataGameManager.instance.TownStorage_List[slotIndex] = slot;
            Debug.Log($"Removed {amountToRemove} from slot {slotIndex}. Remaining: {slot.Quantity}");
        }

        // Refresh the UI after removal
        RefreshAllSlotsUI();
    }


    public static void RefreshAllSlotsUI()
    {
        UpdateTownStorage_Count();
        DataGameManager.instance.populate_Camp_Slots.UpdateRequiredResource_Colors();

        if (DataGameManager.instance.currentActiveCamp != CampType.TownStorage) return;

        var storageList = DataGameManager.instance.TownStorage_List;
        int slotCount = DataGameManager.instance.populate_Storage_Slots.parentContainer.childCount;

        // Clear all slots first
        for (int i = 0; i < slotCount; i++)
        {
            var slotUI = DataGameManager.instance.populate_Storage_Slots.parentContainer.GetChild(i).GetComponent<Item_Slot_Base>();
            slotUI.SetAsEmpty();
        }

        // Now fill UI slots based on storageList indices, keeping them 1:1
        for (int i = 0; i < storageList.Count && i < slotCount; i++)
        {
            var slot = storageList[i];
            var slotUI = DataGameManager.instance.populate_Storage_Slots.parentContainer.GetChild(i).GetComponent<Item_Slot_Base>();

            if (!string.IsNullOrEmpty(slot.ItemID) && slot.Quantity > 0)
            {
                slotUI.itemDataBasic = slot;
                slotUI.itemQty.text = slot.Quantity.ToString();
                slotUI.SetupAsActive();
            }
            else
            {
                slotUI.SetAsEmpty();
            }
        }  
    }

    public static int FindItemIndexByID(string itemID)
    {
        int index = 0;
        foreach (var kvp in DataGameManager.instance.TownStorage_List)
        {
            if (kvp.ItemID == itemID)
            {
                return index;
            }
            index++;
        }

        Debug.LogWarning($"Item ID '{itemID}' not found in itemData_Array.");
        return -1; // Not found
    }

    public static int GetCurrentQuantity(string itemID)
    {
        int total = 0;

        for (int i = 0; i < DataGameManager.instance.TownStorage_List.Count; i++)
        {
            if (DataGameManager.instance.TownStorage_List[i].ItemID == itemID)
            {
                total += DataGameManager.instance.TownStorage_List[i].Quantity;
            }
        }

        return total;
    }

    public static void UpdateTownStorage_Count()

    {
        var storageList = DataGameManager.instance.TownStorage_List;
        int occupiedCount = storageList.Count(s => !string.IsNullOrEmpty(s.ItemID) && s.Quantity > 0); // Update the storage count text 
        storageQtyText.text = $"{occupiedCount}/{DataGameManager.instance.MaxInventorySlots}";
        storageSellManager.UpdateUI();
    }

    public static bool CanAddItem(string itemID, int amount)
    {
        if (!DataGameManager.instance.itemData_Array.TryGetValue(itemID, out ItemData_Struc item))
            return false;

        int remaining = amount;

        // Check existing stacks
        foreach (var slot in DataGameManager.instance.TownStorage_List)
        {
            if (slot.ItemID == itemID && slot.Quantity < item.MaxStack)
            {
                int space = item.MaxStack - slot.Quantity;
                remaining -= space;
                if (remaining <= 0)
                    return true;
            }
        }

        // Check for empty slots
        foreach (var slot in DataGameManager.instance.TownStorage_List)
        {
            if (string.IsNullOrEmpty(slot.ItemID) || slot.Quantity == 0)
            {
                remaining -= item.MaxStack;
                if (remaining <= 0)
                    return true;
            }
        }

        return false;
    }




}


