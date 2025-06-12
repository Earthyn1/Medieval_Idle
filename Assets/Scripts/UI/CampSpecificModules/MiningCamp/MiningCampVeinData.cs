using System;

[Serializable]
public class VeinData
{
    public string OreID;
    public int remaining;
    public int initial;
    public float SearchDuration;

    public VeinData(string oreID, int veinRemaining, int initialVeinSize, float searchDuration)
    {
        OreID = oreID;
        remaining = veinRemaining;
        initial = initialVeinSize;
        SearchDuration = searchDuration;
    }
}