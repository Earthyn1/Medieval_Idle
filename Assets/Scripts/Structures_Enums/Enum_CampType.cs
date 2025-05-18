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
}

[CreateAssetMenu(fileName = "CampTypeData", menuName = "ScriptableObjects/CampTypeData", order = 1)]
public class CampTypeData : ScriptableObject
{
    public CampType campType;
    public Sprite campImage;
    public string campName;
}
