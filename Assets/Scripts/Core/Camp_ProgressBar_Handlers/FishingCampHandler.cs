using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System.Globalization;

public class FishingCampHandler : ICampActionHandler
{
    private float delayTimer;
    private float lerpProgress = 0f; // Tracks lerp progress from 0 to 1
    private float delayAdd;

    public FishingCampHandler()
    {
        delayAdd = UnityEngine.Random.Range(0f, 3f);
    }

    public void RestartTimer(CampActionData entry)
    {
        delayAdd = UnityEngine.Random.Range(0f, 3f);
         lerpProgress = 0f; // Reset lerp on restart
    }

    public void UpdateProgress(CampActionEntry entry)
    {
        if (entry.Slot == null) return;

        CampActionData campActionData = DataGameManager.instance.campDictionaries[entry.CampType][entry.SlotKey];

        delayTimer = campActionData.completeTime + delayAdd;

        Debug.Log(delayTimer);


        float elapsed = (float)(DateTime.Now - entry.StartTime).TotalSeconds;

        // How long before completion to start lerping (last 1 second)
        float lerpStartTime = delayTimer - 1f;

        if (elapsed >= delayTimer)
        {
            // Completed: progress bar at full
            entry.Slot.UpdateProgressBar(1f);
        }
        else if (elapsed >= lerpStartTime)
        {
            // Lerp progress from 0.5 to 1.0 smoothly over last 1 second
            lerpProgress += Time.deltaTime / (delayTimer - lerpStartTime);
            lerpProgress = Mathf.Clamp01(lerpProgress);

            float lerpValue = Mathf.Lerp(0.5f, 1f, lerpProgress);
            entry.Slot.UpdateProgressBar(lerpValue);
        }
        else
        {
            // Before lerp phase: show wobble around 0.5
            lerpProgress = 0f; // Reset lerp progress in case timer restarted early
            float wobble = 0.5f + Mathf.Sin(Time.time * 2f) * 0.1f;
            entry.Slot.UpdateProgressBar(wobble);
        }
    }

    public bool IsCompleted(CampActionEntry entry)
    {
        float elapsed = (float)(DateTime.Now - entry.StartTime).TotalSeconds;
        return elapsed >= delayTimer;
    }
}
