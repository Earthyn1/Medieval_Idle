using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MiningActionEntry : CampActionEntry
{
    public int VeinRemaining = -1;       // -1 means no active vein yet
    public bool IsSearching = false;
    public DateTime SearchStartTime;
    public int InitialVeinSize { get; set; }
    public float SearchDuration { get; set; }
    public VeinData Data { get; private set; }
    public int BaseInitialVeinSize { get; set; }



    // Constructor — calls base constructor
    public MiningActionEntry(string slotKey, CampType campType, DateTime startTime, float progress, VeinData veinData)
        : base(slotKey, campType, startTime, progress)
    {
        Data = veinData;
        InitialVeinSize = veinData.initial;
        VeinRemaining = veinData.remaining;
        SearchDuration = veinData.SearchDuration;
        BaseInitialVeinSize = veinData.initial;
    }

    

    public float GetSearchProgress()
    {
        if (!IsSearching) return 0f;
        return Mathf.Clamp01((float)(DateTime.Now - SearchStartTime).TotalSeconds / SearchDuration);
    }

    public void GenerateNewVein()
    {
        var boosts = DataGameManager.instance.boostsManager.GetMergedBoosts(CampType.MiningCamp);
        var boostNames = new List<string> { "Expanded Veins" };
        var veinBoost = boosts.FirstOrDefault(b => boostNames.Contains(b.boostName));

        // Determine final vein size with boost (if any)
        int finalVeinSize = veinBoost != null
            ? Mathf.RoundToInt(veinBoost.boostAmount + BaseInitialVeinSize)
            : BaseInitialVeinSize;

        // Apply to entry
        int min = BaseInitialVeinSize;
        int max = finalVeinSize + 1; // +1 because Random.Range is exclusive on the upper bound

        VeinRemaining = UnityEngine.Random.Range(min, max) + 1;
        InitialVeinSize = VeinRemaining - 1;
    }


}