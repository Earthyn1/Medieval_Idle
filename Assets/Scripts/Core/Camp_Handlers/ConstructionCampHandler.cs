using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System.Globalization;
using UnityEngine.Rendering.Universal;

public class ConstructionCampHandler : ICampActionHandler
{

    public void UpdateProgress(CampActionEntry entry)
    {
        if (entry.Slot == null || !entry.IsActive) return;

        if (IsCompleted(entry))
        {
           
            entry.Slot.UpdateProgressBar(1f);
          //  CompleteAction(entry);
            return; // Don't increase progress or call complete again
        }
        float progress = entry.GetProgress();
        entry.Slot.UpdateProgressBar(progress);
       
    }

    public bool IsCompleted(CampActionEntry entry)
    {
        bool completed = entry.IsCompleted();
        return completed;
    }

    public void RestartTimer(CampActionEntry entry)
    {
        if (!entry.IsActive) return;
        entry.StartTime = DateTime.UtcNow;
        entry.Progress = 0f;
        CampActionData data = DataGameManager.instance.campDictionaries[entry.CampType][entry.SlotKey];
        float duration = data.completeTime;

        if (entry.Slot != null)
            entry.Slot.UpdateProgressBar(0f);
    }


    public void CompleteAction(CampActionEntry entry)
    {
       
        string slotKey = entry.SlotKey;
        var data = DataGameManager.instance.constructionCampModuleData[slotKey];
        
        if (data.BuildingIDUnlocked != null)
        {
           
            if (Enum.TryParse<CampType>(data.BuildingIDUnlocked, out CampType campType))
            {
                DataGameManager.instance.SetCampLockedStatus(campType, false); //Updates the Camps Locked statu
                DataGameManager.instance.campButtonUpdater.UpdateCampButtonAsUnlocked(campType); //Sets side button as unlocked
            }

            MoreBuilds(data.BuildingIDUnlocked);

            if (data.SingleUseSlot && DataGameManager.instance.constructionCampModuleData.TryGetValue(slotKey, out var module)) //Sets oneSlotUse as hidden
            {
             
                DataGameManager.instance.OneSlotUseActions.Add(slotKey, new OneSlotUseActions_Struc(slotKey)); //Add this slot to the OneSlotUse!
                DataGameManager.instance.actionCampHandler.RemoveCampAction(slotKey, CampType.ConstructionCamp);

                if (DataGameManager.instance.currentActiveCamp == CampType.ConstructionCamp)
                {
                    var campDataDict = DataGameManager.instance.GetCampData(CampType.ConstructionCamp); //update the camps visuals to reflect this camp now gone!
                    DataGameManager.instance.populate_Camp_Slots.PopulateSlots(campDataDict);
                }
            }
        }
    }

    public void MoreBuilds(string BuildingID)
    {
        if (BuildingID == "Simple Cabin")
        {
            Debug.Log("Made log cabin");
            DataGameManager.instance.MaxVillagerCapacity += 2;
            DataGameManager.instance.CurrentVillagerCount += 2;

            DataGameManager.instance.topPanelManager.UpdateTownPopulation();
            XPManager.levelUpNotification.IncreasedPop("+2");
        }

        if (BuildingID == "Storage Upgrade 1")
        {
            for (int i = 0; i < 6; i++)
            {
                DataGameManager.instance.TownStorage_List.Add(new StorageSlot
                {
                    ItemID = "",
                    Quantity = 0,
                    IsTutorialSlot = false,
                });
            }

          

            if (DataGameManager.instance.currentActiveCamp == CampType.TownStorage)
            {
                foreach (Transform child in DataGameManager.instance.campButtonUpdater.campsVerticalLayout.transform)
                {
                    CampButtonSetup childscript = child.GetComponent<CampButtonSetup>();
                    if (childscript.campData.campType == CampType.TownStorage)
                    {
                        childscript.HandleTownStorage();
                       
                    }
                }
            }

            DataGameManager.instance.MaxInventorySlots += 6;
            TownStorageManager.UpdateTownStorage_Count();
            XPManager.levelUpNotification.IncreasedStorage("Storage +6!");

          

        }
    }

    public bool HasEnoughCampSpecificResources(CampActionEntry entry)
    {
      //  Debug.Log("Checking the land deed!");
        var data = DataGameManager.instance.constructionCampModuleData[entry.SlotKey];
        return DataGameManager.instance.CurrentLandDeedsOwned >= data.landDeed;
        // TODO: Implement check for camp-specific resources
    }

    public void RemoveCampSpecificResources(CampActionEntry entry)
    {
        Debug.Log("Removing the land deed!");
        var data = DataGameManager.instance.constructionCampModuleData[entry.SlotKey];
        DataGameManager.instance.CurrentLandDeedsOwned -= data.landDeed;
    }

    public void ReturnCampSpecificResources(CampActionEntry entry)
    {
        Debug.Log("Returning the land deed!");
        var data = DataGameManager.instance.constructionCampModuleData[entry.SlotKey];
        DataGameManager.instance.CurrentLandDeedsOwned += data.landDeed;

    }

}