using UnityEngine;
using System;
using System.Globalization;

public class MiningCampHandler : ICampActionHandler
{
    public void UpdateProgress(CampActionEntry baseEntry)
    {
        
        var miningEntry = baseEntry as MiningActionEntry;
       
        if (miningEntry == null) return;

        if (miningEntry.IsSearching)
        {
            float searchProgress = miningEntry.GetSearchProgress();

            if (searchProgress >= 1f)
            {
                StartNewVein(miningEntry);
                miningEntry.IsSearching = false;
            }
            return;
        }

        // Only tracks data — UI is updated elsewhere (in the Module).
        float progress = miningEntry.GetProgress();
    }

    private void StartNewVein(MiningActionEntry miningEntry)
    {
        miningEntry.GenerateNewVein();
     
       // RestartTimer(miningEntry);
    }

    public void RestartTimer(CampActionEntry baseEntry)
    {
        var miningEntry = baseEntry as MiningActionEntry;
        if (miningEntry == null) return;

        if (miningEntry.VeinRemaining > 0)
        {
            Debug.Log($"[RestartTimer] Continuing vein for slot {miningEntry.Slot.slotkey} with {miningEntry.VeinRemaining} ore left");
            miningEntry.IsSearching = false;
        }
        else
        {
            Debug.Log($"[RestartTimer] Starting search for new vein in slot {miningEntry.Slot.slotkey}");
            miningEntry.IsSearching = true;
            miningEntry.SearchStartTime = DateTime.Now;
        }

        miningEntry.StartTime = DateTime.Now;
    }


    public bool IsCompleted(CampActionEntry baseEntry)
    {
        var miningEntry = baseEntry as MiningActionEntry;
        if (miningEntry == null)
            return false;

        if (miningEntry.IsSearching)
            return false;

        if (miningEntry.IsCompleted())
        {
            miningEntry.VeinRemaining--;

            if (miningEntry.VeinRemaining > 0)
            {
                RestartTimer(miningEntry);
            }
            else
            {
                miningEntry.IsSearching = true;
                miningEntry.SearchStartTime = DateTime.Now;
                miningEntry.Slot.UpdateProgressBar(0f);
              
            }

            return true;
        }

        return false;
    }

    public void CompleteAction(CampActionEntry entry)
    {
        // ✅ Handle reward, bait, particles, XP, etc. here

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
