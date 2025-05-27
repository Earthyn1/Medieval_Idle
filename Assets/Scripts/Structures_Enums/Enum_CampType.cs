using UnityEngine;

public enum CampType
{
    LumberCamp,
    Blacksmith,
    ConstructionCamp,
    FishingCamp,
    MiningCamp,
    AdventureCamp,
    MerchantsCamp,
    TownStorage,
    LocalMarket,
    TownOverview,
}

[CreateAssetMenu(fileName = "CampTypeData", menuName = "ScriptableObjects/CampTypeData", order = 1)]
public class CampTypeData : ScriptableObject
{
    public CampType campType;
    public Sprite campImage;
    public string campName;
    public Sprite campTierImage;
}

[System.Serializable]
public class CampCsvEntry
{
    public CampType campType;
    public TextAsset csvFile;
}


[System.Serializable]
public class CampModuleEntry
{
    public CampType campType;
    public GameObject campModulePrefab;
}
