using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CampActionEntry
{
    public string SlotKey { get; set; }  // Store just the key instead of full data
    public CampType CampType { get; set; }  
    public DateTime StartTime { get; set; }
    public Camp_Resource_Slot Slot { get; set; }  // Can be null

    public CampActionEntry(string slotKey, CampType campType, DateTime timeStamp)
    {
        SlotKey = slotKey;
        CampType = campType;    
        StartTime = DateTime.Now;
        Slot = null;  // Start with no slot linked

    }

    // Calculate progress (0 to 1)
    public float GetProgress()
    {
        float elapsedTime = (float)(DateTime.Now - StartTime).TotalSeconds;
        CampActionData campActionData = DataGameManager.instance.campDictionaries[CampType][SlotKey];

        return Mathf.Clamp01(elapsedTime / campActionData.completeTime);
    }

    public bool IsCompleted() => GetProgress() >= 1.0f;

    public void RestartTimer() => StartTime = DateTime.Now;

    public void SetSlot(Camp_Resource_Slot newSlot) => Slot = newSlot;

}