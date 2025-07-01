using System;
using UnityEngine;

public class LumberCampHandler : ICampActionHandler
{
    private float delayTimer;
    private float chopCooldown = 1f; // 1 second between visual chops
    private float nextChopTime;
    private float displayedProgress = 0f; // Progress shown on the bar

    public void RestartTimer(CampActionEntry entry)
    {
       // delayTimer = entry.completeTime;
        nextChopTime = Time.time + chopCooldown;
        displayedProgress = 0f;
    }

    public void UpdateProgress(CampActionEntry entry)
    {
        if (entry.Slot == null) return;

        float elapsed = (float)(DateTime.UtcNow - entry.StartTime).TotalSeconds;

        // Calculate true continuous progress (0 to 1)
        float trueProgress = Mathf.Clamp01(elapsed / delayTimer);

        // Update displayed progress only every chopCooldown seconds
        if (Time.time >= nextChopTime && displayedProgress < 1f)
        {
            // Jump progress up by a chunk or to trueProgress, whichever is smaller
            float chunk = 0.2f; // fixed chunk size for visual jump, e.g. 20%
            displayedProgress = Mathf.Min(displayedProgress + chunk, trueProgress, 1f);
            nextChopTime = Time.time + chopCooldown;
        }

        // Update progress bar with the discrete visual progress
        entry.Slot.UpdateProgressBar(displayedProgress);
    }

    public bool IsCompleted(CampActionEntry entry)
    {
        float elapsed = (float)(DateTime.UtcNow - entry.StartTime).TotalSeconds;
        return elapsed >= delayTimer;
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


