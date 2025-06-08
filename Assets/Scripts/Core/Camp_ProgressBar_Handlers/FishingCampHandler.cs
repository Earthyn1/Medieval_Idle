using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System.Globalization;

public class FishingCampHandler : ICampActionHandler
{

    public void UpdateProgress(CampActionEntry entry)
    {
        if (entry.Slot == null || !entry.IsActive) return;

        if (IsCompleted(entry))
        {
          
            entry.Slot.UpdateProgressBar(1f);
            CompleteAction(entry);
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
        entry.StartTime = DateTime.Now;
        entry.Progress = 0f;
        CampActionData data = DataGameManager.instance.campDictionaries[entry.CampType][entry.SlotKey];
        float duration = data.completeTime;
        

        if (entry.Slot != null)
            entry.Slot.UpdateProgressBar(0f);
    }


    public void CompleteAction(CampActionEntry entry)
    {
       
        // ✅ Handle reward, bait, particles, XP, etc. here
        if (DataGameManager.instance.currentFishingBaitEquipped.item != "")
        {
            DataGameManager.instance.currentFishingBaitEquipped.qty = DataGameManager.instance.currentFishingBaitEquipped.qty - 1;
            if (DataGameManager.instance.currentFishingBaitEquipped.qty <= 0)
            {
                DataGameManager.instance.currentFishingBaitEquipped.item = "";
            }

            FishingCamp_UpperPanel_Module upperpanelfishingCamp = DataGameManager.instance.upperPanelManager.fishingCamp_Buttons.GetComponent<FishingCamp_UpperPanel_Module>();
            upperpanelfishingCamp.UpdateBaitButton();
        }

    }

    public bool HasEnoughCampSpecificResources(CampActionEntry entry)
    {
        return true;
        // TODO: Implement check for camp-specific resources
    }

    public void RemoveCampSpecificResources(CampActionEntry entry)
    {
        // TODO: Implement removal of camp-specific resources
    }

    public void ReturnCampSpecificResources(CampActionEntry entry)
    {
        // TODO: Implement return of unused camp-specific resources
    }


}