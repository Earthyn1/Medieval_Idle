using System;

[Serializable]
public class BlacksmithCampFuelData
{
    public string itemID;
    public int fuelRequired;


    public BlacksmithCampFuelData(string itemID, int fuelRequired)
    {
        this.itemID = itemID;
        this.fuelRequired = fuelRequired;
    }
}