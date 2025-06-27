using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering;


public class Populate_Camp_Slots : MonoBehaviour
{

    public Transform parentContainer;
    public GameObject slotPrefab;  // Public reference to your slot prefab
    public GameObject CategoryButtonPrefab;  // Public reference to your slot prefab
    public GameObject scrollWindow;
    public GameObject requiredResourcePrefab; // Public reference to your slot prefab
    public GameObject Camp_Categorys_Parent;
    public List<CampCategoryData> allCategoryData;

    public List<CampModuleEntry> campSpecificModulePrefabs;


    public void PopulateSlots(Dictionary<string, CampActionData> campData)
    {
        ActivateScrollWindow();

        ClearOldSlots();

        foreach (var pair in campData)
        {
            var key = pair.Key;
            var data = pair.Value;

            if (ShouldSkipSlot(data)) continue;

            var slotGO = Instantiate(slotPrefab, parentContainer);
            var slotUI = slotGO.GetComponent<Camp_Resource_Slot>();
            slotGO.name = $"Slot_{key}";

            SetupSlotVisuals(slotUI, data, key);
            SetupRequiredResources(slotUI, data);

            var activeEntry = FindActiveCampEntry(key, data.campType);
            BindSlotToActiveEntry(slotUI, activeEntry, key, data);

            AttachCampSpecificModule(slotUI, data);
        }
    }

    private void ActivateScrollWindow()
    {
        scrollWindow.SetActive(true);
        DataGameManager.instance.populate_Storage_Slots.scrollWindow.SetActive(false);

        ScrollRect scrollRect = scrollWindow.GetComponentInParent<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void ClearOldSlots()
    {
        foreach (Transform child in parentContainer)
            Destroy(child.gameObject);

        foreach (Transform child in DataGameManager.instance.populate_Storage_Slots.parentContainer)
            Destroy(child.gameObject);
    }

    private bool ShouldSkipSlot(CampActionData data)
    {
        return data.levelUnlocked > DataGameManager.instance.CurrentContentLevelAvailable ||
               (data.campCategory != DataGameManager.instance.currentCampCategory &&
                data.campCategory != CampCategorys.NA) ||
               DataGameManager.instance.OneSlotUseActions.ContainsKey(data.resourceName);
    }

    private void SetupSlotVisuals(Camp_Resource_Slot slot, CampActionData data, string key)
    {
        slot.slotkey = key;
        slot.campType = data.campType;
        slot.actionName.text = data.resourceName;
        slot.FGImage.sprite = data.image2D;
        slot.BGImage.sprite = data.bgImage;
        slot.popCount.text = data.populationCost.ToString();
        slot.lvlUnlocked.text = data.levelUnlocked.ToString();
        slot.popCount.color = (data.populationCost > DataGameManager.instance.CurrentVillagerCount) ? Color.red : Color.white;

        if (DataGameManager.instance.campXPDictionaries.TryGetValue(data.campType, out var xpData))
        {
            slot.isLocked = data.levelUnlocked > xpData.currentLevel;
            slot.Locked_Panel.SetActive(slot.isLocked);
            slot.Unlocked_Panel.SetActive(!slot.isLocked);
        }
    }

    private void SetupRequiredResources(Camp_Resource_Slot slot, CampActionData data)
    {
        if (data.RequiredItems == null || data.RequiredItems.Count == 0) return;

        foreach (var item in data.RequiredItems)
        {
            GameObject resourceGO = Instantiate(requiredResourcePrefab, slot.requiredResource_Parent.transform);
            var reqSlot = resourceGO.GetComponent<Required_Resource_Slot>();
            var itemData = DataGameManager.instance.itemData_Array[item.item];

            resourceGO.name = "RequiredResource-" + itemData.ItemID;
            reqSlot.itemimage.sprite = itemData.ItemImage;
            reqSlot.itemimage_2.sprite = itemData.ItemImage;
            reqSlot.itemqty.text = item.qty.ToString();
            reqSlot.itemID = item.item;

            bool hasEnough = DataGameManager.instance.TownStorage_List
                .Any(s => s.ItemID == item.item && s.Quantity >= item.qty);

            reqSlot.itemqty.color = hasEnough ? Color.green : Color.red;
        }
    }

    private CampActionEntry FindActiveCampEntry(string key, CampType type)
    {
        return DataGameManager.instance.activeCamps
            .FirstOrDefault(e => e.SlotKey == key && e.CampType == type && e.IsActive);
    }

    private void BindSlotToActiveEntry(Camp_Resource_Slot slot, CampActionEntry entry, string key, CampActionData data)
    {
        if (entry != null)
        {
            Debug.Log($"Found active entry for {key}");
            entry.SetSlot(slot);
            entry.IsActive = true;

            slot.UpdateProgressBar(entry.GetProgress());
            slot.isActive = true;
            slot.requiredResource_Parent.SetActive(false);
            slot.progressBar.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log($"No active entry found for {key}");
            slot.UpdateProgressBar(0f);
            slot.isActive = false;
            slot.requiredResource_Parent.SetActive(true);
        }
    }

    private void AttachCampSpecificModule(Camp_Resource_Slot slot, CampActionData data)
    {
        var moduleEntry = campSpecificModulePrefabs.Find(e => e.campType == data.campType);
        if (moduleEntry?.campModulePrefab != null)
        {
            GameObject module = Instantiate(moduleEntry.campModulePrefab, slot.Unlocked_Panel.transform);
            slot.CampSpecificPrefab = module;

            module.GetComponent<CampUISlotInterface>()?.OnUISlotUpdate(data.resourceName);
        }
        else
        {
            Debug.LogWarning($"No prefab found for camp type: {data.campType}");
        }
    }



    public void UpdateRequiredResource_Colors()
    {
        foreach (Transform slot in parentContainer)
        {
            Camp_Resource_Slot slotScript = slot.GetComponent<Camp_Resource_Slot>();
     
            CampActionData campActionData = DataGameManager.instance.GetCampActionData(slotScript.campType, slotScript.slotkey);

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
                        var requiredItem = campActionData.RequiredItems
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

    public void UpdateCampSpecific_UI()
    {
        foreach (Transform slot in parentContainer)
        {
            Camp_Resource_Slot slotScript = slot.GetComponent<Camp_Resource_Slot>();
            if (slotScript == null || slotScript.CampSpecificPrefab == null)
                continue;

            CampUISlotInterface moduleInterface = slotScript.CampSpecificPrefab.GetComponent<CampUISlotInterface>();
            if (moduleInterface != null)
            {
                moduleInterface.OnUISlotUpdate("Test");
            }
        }
    }


    public void UpdateRequiredVillager_Colors()
    {
        foreach (Transform slot in parentContainer)
        {
            Camp_Resource_Slot slotScript = slot.GetComponent<Camp_Resource_Slot>();

            CampActionData campActionData = DataGameManager.instance.GetCampActionData(slotScript.campType, slotScript.slotkey);

            if (campActionData.populationCost > DataGameManager.instance.CurrentVillagerCount)
            {
                slotScript.popCount.color = Color.red;
            }
            else
            {
                slotScript.popCount.color = Color.white;
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
                newCategory.name = "Category_" + matchedData.name;
                Category_Base slotScript = newCategory.GetComponent<Category_Base>();
                slotScript.CampCategoryData = matchedData;

                // Use the matched data to set the image
                slotScript.Category_Image.sprite = matchedData.categorysImage;

                slotScript.SetAsUnSelected();

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




