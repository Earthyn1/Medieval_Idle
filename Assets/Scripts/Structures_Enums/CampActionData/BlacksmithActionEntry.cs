using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BlacksmithActionEntry : CampActionEntry
{

    public int fuelAmount { get; set; }

    public BlacksmithCampFuelData Data { get; private set; }
    



    // Constructor — calls base constructor
    public BlacksmithActionEntry(string slotKey, CampType campType, DateTime startTime, float progress, BlacksmithCampFuelData fuelData)
        : base(slotKey, campType, startTime, progress)
    {
        Data = fuelData;
        fuelAmount = fuelData.fuelRequired;
        
    }

}