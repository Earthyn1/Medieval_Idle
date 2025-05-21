using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;


public class Populate_Camp_Slots : MonoBehaviour
{

    public Transform parentContainer;
    public GameObject slotPrefab;  // Public reference to your slot prefab
    public GameObject CategoryButtonPrefab;  // Public reference to your slot prefab
    public GameObject scrollWindow;
    public GameObject requiredResourcePrefab; // Public reference to your slot prefab
    public GameObject Camp_Categorys_Parent;
    public List<CampCategoryData> allCategoryData;


    public void PopulateSlots(Dictionary<string, CampActionData> campData)
    {
      

        scrollWindow.SetActive(true);
        DataGameManager.instance.populate_Storage_Slots.scrollWindow.SetActive(false);
        ScrollRect scrollRect = scrollWindow.GetComponentInParent<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 1f;  // Top

        foreach (Transform child in parentContainer) // Clear existing slots
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in DataGameManager.instance.populate_Storage_Slots.parentContainer) // Clear existing slots
        {
            Destroy(child.gameObject);
        }

            foreach (var slot in campData)
            {
            if (slot.Value.levelUnlocked > DataGameManager.instance.CurrentContentLevelAvailable) //keep current content to up to lvl 30
            {
                continue;
            }
            CampCategorys nocategory = CampCategorys.NA;

            if (slot.Value.campCategory != DataGameManager.instance.currentCampCategory && slot.Value.campCategory != nocategory) //Here we only use the camps within current category.
            {
           //     Debug.Log(slot.Value.campCategory + ", " + DataGameManager.instance.currentCampCategory); 
                continue;
            }

            GameObject newSlot = Instantiate(slotPrefab, parentContainer);
            Camp_Resource_Slot slotScript = newSlot.GetComponent<Camp_Resource_Slot>();  // Access the Camp_Resource_Slot script on the newly instantiated slot             

            slotScript.actionName.text = slot.Value.resourceName;
            slotScript.FGImage.sprite = slot.Value.image2D;
            slotScript.BGImage.sprite = slot.Value.bgImage;
            slotScript.popCount.text = slot.Value.populationCost.ToString();
            newSlot.name = $"Slot_{slot.Key}";
            slotScript.lvlUnlocked.text = slot.Value.levelUnlocked.ToString();

            if (DataGameManager.instance.campXPDictionaries.TryGetValue(slot.Value.campType, out CampXPData campXPData)) // checks we have the data
            {

                int currentLevel = campXPData.currentLevel;  // Access the current level from the retrieved data
                if (slot.Value.levelUnlocked <= currentLevel) //Here we setup the Locked UI
                {
                    slotScript.isLocked = false;
                }
                else
                {
                    slotScript.Unlocked_Panel.SetActive(false);
                    slotScript.Locked_Panel.SetActive(true);
                    slotScript.isLocked = true;
                }

            }

            // Check if the slot has required items
            if (slot.Value.RequiredItems != null && slot.Value.RequiredItems.Count > 0)
            {
                foreach (SimpleItemData item in slot.Value.RequiredItems)
                {
                    // Instantiate the required resource prefab
                    GameObject resourceGO = Instantiate(requiredResourcePrefab, slotScript.requiredResource_Parent.transform);

                    Required_Resource_Slot required_Resource_SlotScript = resourceGO.GetComponent<Required_Resource_Slot>();
                    ItemData_Struc requireditem_Data = DataGameManager.instance.itemData_Array[item.item];

                    required_Resource_SlotScript.itemimage.sprite = requireditem_Data.ItemImage;
                    required_Resource_SlotScript.itemimage_2.sprite = requireditem_Data.ItemImage;
                    required_Resource_SlotScript.itemqty.text = item.qty.ToString();
                    required_Resource_SlotScript.itemID = item.item;

                    required_Resource_SlotScript.itemqty.color =
                    DataGameManager.instance.TownStorage_List.Any(slot =>
                    slot.ItemID == item.item && slot.Quantity >= item.qty)
                    ? Color.green
                    : Color.red;
                }
            }
            else
            {
                // Debug.LogWarning("No required items found for slot: " + slot.Key);
            }

            // Create a new CampActionData for this slot
            CampActionData campActionData = new CampActionData(
                slot.Value.resourceName,
                slot.Value.description,
                slot.Value.populationCost,
                slot.Value.levelUnlocked,
                slot.Value.xpGiven,
                slot.Value.completeTime,
                slot.Value.image2D,
                slot.Value.bgImage,
                slot.Value.campType,
                slot.Value.campCategory,
                slot.Value.ProducedItems,
                slot.Value.RequiredItems
                );

            // Try to find existing active entry by matching resourceName or other unique key
            CampActionEntry existingEntry = null;
            foreach (var entry in DataGameManager.instance.activeCamps)
            {
                if (entry.CampData.resourceName == campActionData.resourceName)
                {
                    existingEntry = entry;
                    Debug.Log(entry.CampData.resourceName);
                    slotScript.progressBar.transform.parent.gameObject.SetActive(true);
                    break;
                }
            }
            if (existingEntry != null) //This found an active slot so we assign the slot to the active camp.
            {
                slotScript.campActionData = existingEntry;
                existingEntry.SetSlot(slotScript);
                slotScript.UpdateProgressBar(existingEntry.GetProgress());
                slotScript.isActive = true;
            }
            else
            {
                CampActionEntry campActionEntry = new CampActionEntry(campActionData, DateTime.Now);  // Create a new CampActionEntry with the current time
                slotScript.campActionData = campActionEntry; // Assign the CampActionData to the slot
                campActionEntry.SetSlot(slotScript); //Adds this new slot ref to itself.
            }
        }
    }

    public void UpdateRequiredResource_Colors()
    {
        foreach (Transform slot in parentContainer)
        {
            Camp_Resource_Slot slotScript = slot.GetComponent<Camp_Resource_Slot>();

            if (slotScript != null && slotScript.campActionData != null)
            {
                
            }
            else
            {
                
                Debug.LogWarning("slotScript or campActionData is null!");

            }
            CampActionData slotcampData = slotScript.campActionData.CampData;

            // Check for required resource slots
            if (slotScript.requiredResource_Parent.transform.childCount > 0)
            {
                foreach (Transform child in slotScript.requiredResource_Parent.transform)
                {

                    // Get the script from each child slot
                    Required_Resource_Slot childScript = child.GetComponent<Required_Resource_Slot>();

                    if (childScript != null) // Check if the script exists
                    {
                        // Find the matching item from the required items list
                        var requiredItem = slotcampData.RequiredItems
                            .FirstOrDefault(item => item.item == childScript.itemID);

                        if (requiredItem != null)
                        {
                            // Update the item quantity text
                            childScript.itemqty.text = requiredItem.qty.ToString();

                            // Check if the player has enough items and update the text color
                            bool hasEnough = DataGameManager.instance.TownStorage_List.Any(slot =>
                                slot.ItemID == requiredItem.item && slot.Quantity >= requiredItem.qty);

                            childScript.itemqty.color = hasEnough ? Color.green : Color.red;
                        }
                    }
                }
            }
        }
    }

    public IEnumerator SetupCampCategorys(CampType campType)
    {
        foreach (Transform child in Camp_Categorys_Parent.transform) // Clear existing slots
        {
            Destroy(child.gameObject);
        }
 
        yield return null; //wait frame for children to destroy

        // Find all matching camp type data
        var matchedDataList = allCategoryData.Where(data => data.campType == campType).ToList();

        if (matchedDataList.Any())
        {
            Category_Base lastSlotScript = null;

            foreach (var matchedData in matchedDataList)
            {
                GameObject newCategory = Instantiate(CategoryButtonPrefab, Camp_Categorys_Parent.transform);
                Category_Base slotScript = newCategory.GetComponent<Category_Base>();
                slotScript.CampCategoryData = matchedData;

                // Use the matched data to set the image
                slotScript.Category_Image.sprite = matchedData.categorysImage;

                // Keep reference to the last one
                lastSlotScript = slotScript;
            }

            // After loop, assign using the last slot
            if (lastSlotScript != null)
            {
                DataGameManager.instance.currentCampCategory = lastSlotScript.CampCategoryData.campCategory;
            }
        }
        else
        {
            Debug.Log("No matching camp types found.");
        }
    }
}




