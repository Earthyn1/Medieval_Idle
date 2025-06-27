using UnityEngine;

public class DefaultCampHandler : ICampActionHandler
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
       
        // Reset the start time to now to restart the timer
        entry.StartTime = System.DateTime.Now;
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