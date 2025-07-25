﻿using UnityEngine;

public class BlackSmithCampHandler : ICampActionHandler
{
    public void UpdateProgress(CampActionEntry entry)
    {
       // Debug.Log("Are we working?");
        if (entry.Slot == null || !entry.IsActive) return;

        if (IsCompleted(entry))
        {
            // Optionally, update UI to full and do nothing more
            entry.Slot.UpdateProgressBar(1f);
            return; // Don't increase progress or call complete again
        }
       // Debug.Log("Are we tracking??");
        float progress = entry.GetProgress();
        entry.Slot.UpdateProgressBar(progress);
      


    }

    public bool IsCompleted(CampActionEntry entry)
    {
        return entry.IsCompleted(); // Use the actual completion check from the entry
    }
    public void RestartTimer(CampActionEntry entry)
    {
        if (!entry.IsActive) return;
      //  entry.Slot.progressBar.transform.parent.gameObject.SetActive(true);
        Debug.Log("Activate Slot! PLease!!");
        // Reset the start time to now to restart the timer
        entry.StartTime = System.DateTime.UtcNow;
        entry.Progress = 0f;

        // Optionally update the UI progress immediately on restart
        if (entry.Slot != null)
            entry.Slot.UpdateProgressBar(0f);

    }

    public void CompleteAction(CampActionEntry entry)
    {
        // ✅ Handle reward, bait, particles, XP, etc. here

    }


    public bool HasEnoughCampSpecificResources(CampActionEntry entry)
    {
        if (!DataGameManager.instance.blacksmithCampModuleData.TryGetValue(entry?.SlotKey, out var data))
        {
            return false;
        }

        int currentFuel = DataGameManager.instance.currentBlacksmithFuel;
        bool hasEnoughFuel = currentFuel >= data.fuelRequired;

        if (!hasEnoughFuel)
        {
            var slot = entry?.Slot;
            var prefab = slot?.CampSpecificPrefab;

            if (slot != null && prefab != null && prefab.TryGetComponent<CampUISlotInterface>(out var moduleInterface))
            {
                moduleInterface.OnUISlotSingleCall(); // This calls the blacksmith fuel red flasher to indicate fuel is missing
            }
        }

        return hasEnoughFuel;
    }





    public void RemoveCampSpecificResources(CampActionEntry entry)
    {
        var data = DataGameManager.instance.blacksmithCampModuleData[entry.SlotKey];
        DataGameManager.instance.currentBlacksmithFuel = Mathf.Max(0, DataGameManager.instance.currentBlacksmithFuel - data.fuelRequired);

        UpperPanel_Blacksmith upperPanel_Blacksmith = DataGameManager.instance.upperPanelManager.blacksmithCamp_Buttons.GetComponent<UpperPanel_Blacksmith>();
        upperPanel_Blacksmith.SetupFuelBar();

        if (DataGameManager.instance.currentActiveCamp == CampType.Blacksmith)
        {
           DataGameManager.instance.populate_Camp_Slots.UpdateCampSpecific_UI();
        }
    }

    public void ReturnCampSpecificResources(CampActionEntry entry)
    {
        // TODO: Implement return of unused camp-specific resources
    }
}