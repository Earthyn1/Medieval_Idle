using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.EventSystems.EventTrigger;

public class CampActionEntry
{
    public string SlotKey { get; set; }  // Store just the key instead of full data
    public CampType CampType { get; set; }  
    public DateTime StartTime { get; set; }
    public Camp_Resource_Slot Slot { get; set; }  // Can be null

    public float Progress = 0f; 

    public bool IsActive { get; set; }

    public SideActionSlot SideActionSlot { get; set; }

    public ICampActionHandler CampTypeHandler { get; set; }  

    public CampActionEntry(string slotKey, CampType campType, DateTime timeStamp, float progress)
    {
        SlotKey = slotKey;
        CampType = campType;    
        StartTime = DateTime.UtcNow;
        Slot = null;  // Start with no slot linked
        CampTypeHandler = CampActionHandlerFactory.GetHandler(campType);  // <-- Initialize logic
        Progress = progress;
        IsActive = true;
        SideActionSlot = null;
    }

    // Calculate progress (0 to 1)
    public float GetProgress()
    {
     
        float elapsedTime = (float)(DateTime.UtcNow - StartTime).TotalSeconds;

        if (DataGameManager.instance.DEVspeedMultiplier != 1)
        {
            elapsedTime *= DataGameManager.instance.DEVspeedMultiplier;
        }

        CampActionData campActionData = DataGameManager.instance.campDictionaries[CampType][SlotKey];

        float speedincrease = 0f;

        var boosts = DataGameManager.instance.boostsManager.GetMergedBoosts(campActionData.campType);
        var boostNames = new List<string> { "Swift Fishing", "Swift Construction", "Rapid Woodcutting", "Rapid Mining", "Swift Blacksmithing" }; //Here we add the bonus speed for all camps with bonus speed
        var dropBoost = boosts.FirstOrDefault(b => boostNames.Contains(b.boostName));
        if (dropBoost != null)
            speedincrease = dropBoost.boostAmount; // we get the merged boosts for fishing camp

        //
        //Debug.Log($"[{SlotKey}] Elapsed Time: {elapsedTime} seconds | StartTime: {StartTime} | Now: {DateTime.UtcNow}");
     //   DebugProgressInfo();

        return Mathf.Clamp01(elapsedTime / (campActionData.completeTime - speedincrease));
    }

    public bool IsCompleted() => GetProgress() >= 1.0f;

    public void RestartTimer() => StartTime = DateTime.UtcNow;

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

    public void DebugProgressInfo()
    {
        float elapsedTime = (float)(DateTime.UtcNow - StartTime).TotalSeconds;
        float devMultiplier = DataGameManager.instance.DEVspeedMultiplier;
        float adjustedElapsedTime = elapsedTime * (devMultiplier != 1 ? devMultiplier : 1);

        CampActionData campActionData = DataGameManager.instance.campDictionaries[CampType][SlotKey];

        float speedIncrease = 0f;
        var boosts = DataGameManager.instance.boostsManager.GetMergedBoosts(campActionData.campType);
        var boostNames = new List<string> { "Swift Fishing", "Swift Construction", "Rapid Woodcutting", "Rapid Mining", "Swift Blacksmithing" };
        var dropBoost = boosts.FirstOrDefault(b => boostNames.Contains(b.boostName));
        if (dropBoost != null)
            speedIncrease = dropBoost.boostAmount;

        float totalTime = campActionData.completeTime - speedIncrease;
        float progress = Mathf.Clamp01(adjustedElapsedTime / totalTime) * 100f;

        Debug.Log($"[{SlotKey}] Debug Progress Info:\n" +
                  $"StartTime: {StartTime} (UTC)\n" +
                  $"CurrentTime: {DateTime.UtcNow} (UTC)\n" +
                  $"Raw Elapsed Time: {elapsedTime:F2} sec\n" +
                  $"DEV Speed Multiplier: {devMultiplier}\n" +
                  $"Adjusted Elapsed Time: {adjustedElapsedTime:F2} sec\n" +
                  $"Base Complete Time: {campActionData.completeTime} sec\n" +
                  $"Speed Boost Reduction: {speedIncrease} sec\n" +
                  $"Effective Complete Time: {totalTime} sec\n" +
                  $"Progress: {progress:F1}%\n");
    }

}

