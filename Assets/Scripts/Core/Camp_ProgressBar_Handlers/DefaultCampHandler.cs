using UnityEngine;

public class DefaultCampHandler : ICampActionHandler
{
    public void UpdateProgress(CampActionEntry entry)
    {
        if (entry.Slot == null) return;

        float progress = entry.GetProgress();
        entry.Slot.UpdateProgressBar(progress);
    }

    public bool IsCompleted(CampActionEntry entry)
    {
        return entry.IsCompleted(); // Use the actual completion check from the entry
    }
    public void RestartTimer(CampActionData entry)
    {
       
       
    }

}