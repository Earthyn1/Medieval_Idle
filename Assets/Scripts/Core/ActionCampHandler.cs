using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using static UnityEngine.EventSystems.EventTrigger;

public class ActionCampHandler : MonoBehaviour
{

    private void Start()
    {
        DataGameManager.instance.actionCampHandler = this;
    }

    void Update()
    {
        foreach (var entry in DataGameManager.instance.activeCamps.ToList())
        {
            float progress = entry.GetProgress();            

            if (entry.Slot != null)
            {
                entry.Slot.UpdateProgressBar(progress); //This updates the slot progress bar.
                
            }

            if (entry.IsCompleted())
            {
                CompleteCampAction(entry.SlotKey, entry.CampType);
                CampActionData campActionData = DataGameManager.instance.campDictionaries[entry.CampType][entry.SlotKey];

                entry.Slot.CheckForDialogs();

                if (HasEnoughResources(campActionData) && HasEnoughCampSpecificResources(campActionData))
                {
                    RemoveRequiredCampResources(campActionData);
                    RemoveRequiredSpecificCampResources(campActionData);
                    entry.RestartTimer();  // Automatically restart the timer
                }
                else
                {
                    Debug.Log("Not enough resource to restart action");
                    RemoveCampAction(entry.SlotKey, entry.CampType);

                    if (entry.Slot != null)
                    {
                        entry.Slot.DeactivateActionSlot();
                    }
                }
            }
        }
    }

    public void RemoveCampAction(string Key, CampType campType)
    {
        CampActionData campData = DataGameManager.instance.GetCampActionData(campType, Key);

        var entry = DataGameManager.instance.activeCamps
            .FirstOrDefault(c => c.SlotKey == Key);

        if (entry != null)
        {
            // DataGameManager.instance.activeCamps.Remove(campActionEntry);
            DataGameManager.instance.CurrentVillagerCount = DataGameManager.instance.CurrentVillagerCount + campData.populationCost;
            DataGameManager.instance.topPanelManager.UpdateTownPopulation();

            DataGameManager.instance.activeCamps.Remove(entry);
        }

       
    }
    public bool TryToAddCampSlot(string key, CampType campType, Camp_Resource_Slot slot)
    {
        CampActionData campActionData = DataGameManager.instance.campDictionaries[campType][key];

        if (DataGameManager.instance.CurrentVillagerCount - campActionData.populationCost >= 0)
        {
            if (HasEnoughResources(campActionData) && HasEnoughCampSpecificResources(campActionData))
            {
                RemoveRequiredCampResources(campActionData);
                RemoveRequiredSpecificCampResources(campActionData);

                CampActionEntry campActionEntry = new CampActionEntry(key, campType, DateTime.Now);
                campActionEntry.Slot = slot; // Assuming this is called from the Camp_Resource_Slot instance
                DataGameManager.instance.activeCamps.Add(campActionEntry);
                DataGameManager.instance.CurrentVillagerCount = DataGameManager.instance.CurrentVillagerCount - campActionData.populationCost;
                DataGameManager.instance.topPanelManager.UpdateTownPopulation();       
                return true;
            }
            else
            {
                Debug.Log("Not enough resources");
                return false;
            }
        }
        else
        {
            Debug.Log("Not enough villagers available");
            return false;
        }
    }
    void CompleteCampAction(string Key, CampType campType)
    {
        CampActionData campData = DataGameManager.instance.GetCampActionData(campType, Key);

        XPManager.AddXP(campType, campData.xpGiven);

        Objective_Manager.UpdateObjectives(Key,1);

        float progress = XPManager.GetLevelProgress(campType);
        //Debug.Log($"{campType} is {progress * 100f}%");

        RollForProducedItem(Key, campType);

        if (campData.campSpecificLogic == null)
        {
          //  Debug.Log("No campspecificLogic!!"); 
        }
        else
        {
            campData.campSpecificLogic.OnCompletedCampSpecificAction(Key);
        }


    }

    void RollForProducedItem(string Key, CampType campType)
    {
        CampActionData campData = DataGameManager.instance.GetCampActionData(campType, Key);

        // Generate a random number between 1 and 100
        int roll = UnityEngine.Random.Range(1, 101);
       // Debug.Log($"Rolled: {roll}");

        float accumulatedChance = 0;

        // Sort produced items by drop chance (if not already sorted)
        campData.ProducedItems.Sort((a, b) => a.dropChance.CompareTo(b.dropChance));

        foreach (var producedItem in campData.ProducedItems)
        {
            // Accumulate the drop chance
            accumulatedChance += producedItem.dropChance;

            // Check if the roll falls within the current accumulated range
            if (roll <= accumulatedChance)
            {
             //   Debug.Log($"Item acquired: {producedItem.item}, Qty: {producedItem.qty}");
    
                // Add item to inventory
                TownStorageManager.AddItem(producedItem.item, producedItem.qty, campType);
                return;
            }
        }

        // Fallback if no item matched (edge case)
       // Debug.Log("No item acquired. Drop chances may not sum up to 100.");
    }

    public void RemoveRequiredCampResources(CampActionData campData)
    {
        if (campData.RequiredItems.Count > 0)
        {
            foreach (SimpleItemData item in campData.RequiredItems)
            {
                TownStorageManager.RemoveItem(item.item, item.qty);
            }
        }
    }

    public bool HasEnoughResources(CampActionData campData)
    {
        bool hasEnough = campData.RequiredItems.All(item =>
            DataGameManager.instance.TownStorage_List.Any(slot =>
                slot.ItemID == item.item && slot.Quantity >= item.qty));

        return hasEnough;
    }

    public bool HasEnoughCampSpecificResources(CampActionData campData)
    {
        if (campData.campSpecificLogic == null)
        {
          //  Debug.Log("No campspecificLogic!!");
            return true;
        }

        bool hasEnough = campData.campSpecificLogic.HasEnoughCampSpecificResources(campData.resourceName);
        if (!hasEnough)
            Debug.Log("Not enough specific resource");

        return hasEnough;
    }

    public void RemoveRequiredSpecificCampResources(CampActionData campData)
    {

        if (campData.campSpecificLogic == null)
        {
         //   Debug.Log("No campspecificLogic!!");
          
        }
        else
        {
            campData.campSpecificLogic.RemoveCampSpecificResources(campData.resourceName);
        }
    }


}