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

    public float Progress = 0f; // <-- add this

    public ICampActionHandler CampTypeHandler { get; set; }  

    public CampActionEntry(string slotKey, CampType campType, DateTime timeStamp, float progress)
    {
        SlotKey = slotKey;
        CampType = campType;    
        StartTime = DateTime.Now;
        Slot = null;  // Start with no slot linked
        CampTypeHandler = CampActionHandlerFactory.GetHandler(campType);  // <-- Initialize logic
        Progress = progress;
    }

    // Calculate progress (0 to 1)
    public float GetProgress()
    {
        float elapsedTime = (float)(DateTime.Now - StartTime).TotalSeconds;

        if (DataGameManager.instance.DEVspeedMultiplier != 1)
        {
            elapsedTime *= DataGameManager.instance.DEVspeedMultiplier;
        }

        CampActionData campActionData = DataGameManager.instance.campDictionaries[CampType][SlotKey];

        return Mathf.Clamp01(elapsedTime / campActionData.completeTime);
    }

    public bool IsCompleted() => GetProgress() >= 1.0f;

    public void RestartTimer() => StartTime = DateTime.Now;

    public void SetSlot(Camp_Resource_Slot newSlot) => Slot = newSlot;


    public override bool Equals(object obj)
    {
        if (obj is not CampActionEntry other) return false;
        return this.SlotKey == other.SlotKey; // Replace with a unique identifier or proper equality logic
    }

    public override int GetHashCode()
    {
        return SlotKey.GetHashCode(); // Same here — base it on unique ID or composite key
    }
}

