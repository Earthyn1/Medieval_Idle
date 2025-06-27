using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
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
            if (entry.IsActive)
            {
                // Let the handler update the progress bar however it wants
                entry.CampTypeHandler.UpdateProgress(entry);

                if (entry.CampTypeHandler.IsCompleted(entry))
                {
                    CompleteCampAction(entry.SlotKey, entry.CampType);
                    CampActionData campActionData = DataGameManager.instance.campDictionaries[entry.CampType][entry.SlotKey];
                    entry.CampTypeHandler.CompleteAction(entry); //Camp specific Complete

                    entry.Slot.CheckForDialogs();

                    if (HasEnoughResources(campActionData) && HasEnoughCampSpecificResources(entry))
                    {
                        RemoveRequiredCampResources(campActionData);
                        RemoveRequiredSpecificCampResources(entry);
                      
                        entry.CampTypeHandler.RestartTimer(entry);
                    }
                    else
                    {

                        Debug.Log("Not enough resource to restart action");
                        RemoveCampAction(entry.SlotKey, entry.CampType);

                        if (entry.Slot != null)
                        {
                            entry.Slot.DeactivateActionSlot_WithoutReturningResources();
                            entry.Slot.NotEnoughResourceFlash();
                        }
                    }
                }
            }            
        }
    }


    public void ReturnResources(string Key, CampType campType)
    {
        Debug.Log("Returning a resource specifc!");
        CampActionData campData = DataGameManager.instance.GetCampActionData(campType, Key);
        var entry = DataGameManager.instance.activeCamps
            .FirstOrDefault(c => c.SlotKey == Key && c.CampType == campType); // ← Added campType check

        foreach (SimpleItemData simpleItemData in campData.RequiredItems)
        {
            bool added = TownStorageManager.AddItem(simpleItemData.item, simpleItemData.qty, campType);

            if (!added)
            {
                Debug.LogWarning($"Could not add all of {simpleItemData.item} (wanted to add {simpleItemData.qty}). Inventory may be full.");
            }
        }
        entry.CampTypeHandler.ReturnCampSpecificResources(entry);
    }
    public void RemoveCampAction(string key, CampType campType)
    {
        CampActionData campData = DataGameManager.instance.GetCampActionData(campType, key);

        var entry = DataGameManager.instance.activeCamps
            .FirstOrDefault(c => c.SlotKey == key && c.CampType == campType); // ← Added campType check

        if (entry != null)
        {
            // Refund villagers
            DataGameManager.instance.CurrentVillagerCount += campData.populationCost;
            DataGameManager.instance.topPanelManager.UpdateTownPopulation();

            // Update UI
            DataGameManager.instance.campButtonUpdater.UpdateCampUsageGreenDots(campType, -campData.populationCost);

            // Mark inactive
            entry.IsActive = false;
        }
    }

    public bool TryToAddCampSlot(string key, CampType campType, Camp_Resource_Slot slot)
    {
        var campData = DataGameManager.instance.campDictionaries[campType][key];

        if (!HasEnoughVillagers(campData, slot))
            return false;

        foreach (CampActionEntry campaction in DataGameManager.instance.activeCamps)
        {
            if (campaction.CampType == campType & campaction.IsActive)
            {
                DataGameManager.instance.Game_Text_Alerts.PlayAlert("Cannot have more than 1 action per camp");
                return false; 
            }


        }
        var existingEntry = FindCampEntry(key, campType);

        if (existingEntry != null)
        {
            return TryReactivateExistingEntry(existingEntry, campData, slot);
        }
        else
        {
            return TryCreateNewEntry(key, campType, campData, slot);
        }
    }

    private bool HasEnoughVillagers(CampActionData data, Camp_Resource_Slot slot)
    {
        if (DataGameManager.instance.CurrentVillagerCount < data.populationCost)
        {
            slot.NotEnoughVillagerFlash();
            return false;
        }
        return true;
    }

    private bool HasRequiredResources(CampActionData data, Camp_Resource_Slot slot, CampActionEntry entry)
    {
        if (!HasEnoughResources(data) || !HasEnoughCampSpecificResources(entry))
        {
            slot.NotEnoughResourceFlash();
            return false;
        }
        return true;
    }

    private CampActionEntry FindCampEntry(string key, CampType campType)
    {
        return DataGameManager.instance.activeCamps
            .FirstOrDefault(e => e.SlotKey == key && e.CampType == campType);
    }

    private bool TryReactivateExistingEntry(CampActionEntry entry, CampActionData data, Camp_Resource_Slot slot)
    {
        if (entry.IsActive)
        {
            ShowCampBusyAlert(entry.CampType);
            return false;
        }

        if (!HasRequiredResources(data, slot, entry))
            return false;

        ConsumeResources(data, entry);

        // Reactivate
        entry.Slot = slot;
        if (entry.CampTypeHandler == null)
            entry.CampTypeHandler = CampActionHandlerFactory.GetHandler(entry.CampType);

        entry.StartTime = DateTime.Now;
        entry.Progress = 0f;
        entry.CampTypeHandler.RestartTimer(entry);
        entry.IsActive = true;

        UpdateSlotVisuals(slot);
        UpdateTownState(data);

        return true;
    }

    private bool TryCreateNewEntry(string key, CampType campType, CampActionData data, Camp_Resource_Slot slot)
    {
        var newEntry = CreateCampActionEntry(key, campType);

        if (!HasRequiredResources(data, slot, newEntry))
            return false;

        ConsumeResources(data, newEntry);

        
        newEntry.StartTime = DateTime.Now;
        newEntry.Progress = 0f;
        newEntry.Slot = slot;
        newEntry.CampTypeHandler = CampActionHandlerFactory.GetHandler(campType);
        newEntry.IsActive = true;

        DataGameManager.instance.activeCamps.Add(newEntry);

        UpdateSlotVisuals(slot);
        UpdateTownState(data);

        return true;
    }

    private void ConsumeResources(CampActionData data, CampActionEntry entry)
    {
        RemoveRequiredCampResources(data);
        RemoveRequiredSpecificCampResources(entry);
    }

    private void UpdateSlotVisuals(Camp_Resource_Slot slot)
    {
        slot.UpdateProgressBar(0f);
        slot.isActive = true;
        slot.requiredResource_Parent.SetActive(false);
        slot.progressBar.transform.parent.gameObject.SetActive(true);
    }

    private void UpdateTownState(CampActionData data)
    {
        DataGameManager.instance.CurrentVillagerCount -= data.populationCost;
        DataGameManager.instance.topPanelManager.UpdateTownPopulation();
        DataGameManager.instance.campButtonUpdater.UpdateCampUsageGreenDots(data.campType, data.populationCost);
    }

    private void ShowCampBusyAlert(CampType campType)
    {
        string campName = DataGameManager.instance.campTypeDataList
            .FirstOrDefault(c => c.campType == campType)?.campName;
        DataGameManager.instance.Game_Text_Alerts.PlayAlert(campName + " is busy!");
    }




    void CompleteCampAction(string Key, CampType campType)
    {
        CampActionData campData = DataGameManager.instance.GetCampActionData(campType, Key);

        XPManager.AddXP(campType, campData.xpGiven);

        Objective_Manager.UpdateObjectives(Key,1);

        float progress = XPManager.GetLevelProgress(campType);
       

        RollForProducedItem(Key, campType);

       

        

        if (campData.campSpecificLogic == null)
        {
       // Debug.Log("No campspecificLogic!!"); 
        }
        else
        {
            campData.campSpecificLogic.OnCompletedCampSpecificAction(Key);
        }
    }

    public CampActionEntry CreateCampActionEntry(string key, CampType campType)
    {
        CampActionEntry entry;

        switch (campType)
        {
            case CampType.MiningCamp:
                if (DataGameManager.instance.miningCampModuleData.TryGetValue(key, out VeinData veindata))
                {
                    var miningEntry = new MiningActionEntry(key, campType, DateTime.Now, 0f, veindata)
                    {
                        IsSearching = true,
                        SearchStartTime = DateTime.Now
                    };
                    entry = miningEntry;
                }
                else
                {
                    entry = new CampActionEntry(key, campType, DateTime.Now, 0f);
                }
                break;

            
             case CampType.Blacksmith:
                if (DataGameManager.instance.blacksmithCampModuleData.TryGetValue(key, out BlacksmithCampFuelData fueldata))
                {
                    Debug.Log("Adding blacksmith camp entry");
                    var BlacksmithEntry = new BlacksmithActionEntry(key, campType, DateTime.Now, 0f, fueldata)
                    {
                
                    };
                    entry = BlacksmithEntry;
                }
                else
                {
                    entry = new CampActionEntry(key, campType, DateTime.Now, 0f);
                }

                break;

            default:
                entry = new CampActionEntry(key, campType, DateTime.Now, 0f);
                break;
        }

        return entry;
    }


    void RollForProducedItem(string key, CampType campType)
    {
        CampActionData campData = DataGameManager.instance.GetCampActionData(campType, key);
        var boosts = DataGameManager.instance.boostsManager.GetMergedBoosts(campType);

        //Here we can add other camps boost names that do the same thing!
        float dropChanceBoost = GetBoostAmount(boosts, "Catch Chance");
        float doubleDropChance = GetBoostAmount(boosts, "Double Catch", "Double Craft", "Twin Harvest");

        int roll = UnityEngine.Random.Range(1, 101);
        float accumulatedChance = 0;

        campData.ProducedItems.Sort((a, b) => a.dropChance.CompareTo(b.dropChance));

        foreach (var producedItem in campData.ProducedItems)
        {
            float boostedDropChance = producedItem.dropChance + dropChanceBoost;
            accumulatedChance += boostedDropChance;

            if (roll <= accumulatedChance)
            {
                int finalQty = CalculateFinalQuantity(producedItem.qty, campType);

                if (UnityEngine.Random.Range(0f, 100f) < doubleDropChance)
                {
                    finalQty *= 2;
                    Debug.Log($"DOUBLE DROP! {producedItem.item} x{finalQty}");
                }

                bool added = TownStorageManager.AddItem(producedItem.item, finalQty, campType);
                if (!added)
                {
                    Debug.LogWarning($"Could not add {producedItem.item} x{finalQty} to storage. Inventory may be full.");
                }

                AddToCampTierResource(producedItem.item, finalQty, campType);

                return;
            }
        }

        Debug.Log($"[Drop Result] Rolled: {roll} | No item matched.");
    }

    float GetBoostAmount(List<CampBoost_Class> boosts, params string[] boostNames)
    {
        return boosts
            .Where(b => boostNames.Contains(b.boostName))
            .Sum(b => b.boostAmount);
    }

    int CalculateFinalQuantity(int baseQty, CampType campType)
    {
        if (campType == CampType.FishingCamp && baseQty > 1)
        {
            int variation = UnityEngine.Random.Range(-1, 2); // -1, 0, or 1
            return Mathf.Max(1, baseQty + variation);
        }

        return baseQty;
    }


    public void AddToCampTierResource(string key, int qty, CampType campType)
    {
        Debug.Log("Are we adding!");
        var boostData = DataGameManager.instance.GetBoostData(campType);
        if (boostData == null) return;

        var tierKey = (boostData.CurrentTier + 1).ToString();
        if (!DataGameManager.instance.allCampTiers.TryGetValue(campType, out var campTiers)) return;
        if (!campTiers.TryGetValue(tierKey, out var tierData)) return;

        boostData.AddResources(key, qty, tierData);

        if (DataGameManager.instance.tierSystem.gameObject.activeInHierarchy & DataGameManager.instance.currentActiveCamp == campType)
        {

            DataGameManager.instance.tierSystem.SetupTierPanel();
        }

        if (boostData.IsResourceComplete(tierData))
        {
            // Trigger tier unlock logic here
        }
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

    public bool HasEnoughCampSpecificResources(CampActionEntry entry)
    {
        bool hasEnough = entry.CampTypeHandler.HasEnoughCampSpecificResources(entry);
        if (!hasEnough)
            Debug.Log("Not enough specific resource");

        return hasEnough;
    }

    public void RemoveRequiredSpecificCampResources(CampActionEntry entry)
    {
        entry.CampTypeHandler.RemoveCampSpecificResources(entry);
        
    }


}