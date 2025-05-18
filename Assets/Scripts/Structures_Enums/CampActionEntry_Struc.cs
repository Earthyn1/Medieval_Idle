using System;
using UnityEngine;

public class CampActionEntry
{
    public CampActionData CampData { get; set; }
    public DateTime StartTime { get; set; }
    public Camp_Resource_Slot Slot { get; set; }  // Can be null

    public CampActionEntry(CampActionData campData, DateTime timeStamp)
    {
        CampData = campData;
        StartTime = DateTime.Now;
        Slot = null;  // Start with no slot linked

    }

    // Calculate progress (0 to 1)
    public float GetProgress()
    {
        float elapsedTime = (float)(DateTime.Now - StartTime).TotalSeconds;
        return Mathf.Clamp01(elapsedTime / CampData.completeTime);
    }

    public bool IsCompleted() => GetProgress() >= 1.0f;

    public void RestartTimer() => StartTime = DateTime.Now;

    public void SetSlot(Camp_Resource_Slot newSlot) => Slot = newSlot;

}