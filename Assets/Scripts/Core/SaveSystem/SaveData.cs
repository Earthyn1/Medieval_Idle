using System;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<ItemDataSave> inventory = new List<ItemDataSave>();
    public List<CampDataSave> activeCamps = new List<CampDataSave>();
    public List<TutorialFlagSave> flagIds = new List<TutorialFlagSave>();
    public List<OneSlotUseActions> OneSlotUseActions = new List<OneSlotUseActions>();


    public List<CampLockEntry> campLockedList;
    public BasicVariablesSave basicVariables;
    public CampBoosts_Save boosts;
    public CampTiers_Save tiers;

    public long lastSaveTimeUnix;
    public int saveVersion = 1;
}

[System.Serializable]
public class ItemDataSave
{
    public string itemID;
    public int quantity;
}

[System.Serializable]
public class CampDataSave
{
    public string campID;
    public CampType type;
    public long startedUnixTime;
    
}

[System.Serializable]
public class TutorialFlagSave
{
    public string flagID;
    public bool isCompleted;
}

[System.Serializable]
public class OneSlotUseActions
{
    public string campID;
}

[System.Serializable]
public class BasicVariablesSave
{
    public int CurrentLandDeedsOwned;
    public int landDeedsBought;
    public int MaxInventorySlots;
    public int PlayerGold;

    public int MaxVillagerCapacity;
    public int CurrentVillagerCount;

    public int maxBlacksmithFuel;
    public int currentBlacksmithFuel;

    public ItemDataSave fishingBait;

    public List<CampXPEntry> campXPList = new List<CampXPEntry>();
}

[System.Serializable]
public class CampXPEntry
{
    public CampType campType;
    public CampXPDataSave xpData;

    public override string ToString()
    {
        return $"{campType}: Level {xpData.level}, XP {xpData.currentXP}";
    }
}

[System.Serializable]
public class CampXPDataSave
{
    public float currentXP;
    public int level;
   
}

[System.Serializable]
public class CampBoosts_Save
{
    public CampBoostData_Save FishingCampBoost;
    public CampBoostData_Save LumberCampBoost;
    public CampBoostData_Save BlacksmithBoost;
    public CampBoostData_Save MiningCampBoost;
    public CampBoostData_Save ConstructionCampBoost;
}

[System.Serializable]
public class CampBoostData_Save
{
    public float boost1;
    public float boost2;
    public float boost3;
    public float boost4;
}

[System.Serializable]
public class CampTiers_Save
{
    public TiersData_Save ConstructionCampTier;
    public TiersData_Save LumberCampTier;
    public TiersData_Save BlacksmithTier;
    public TiersData_Save FishingCampTier;
    public TiersData_Save MiningCampTier;
}

[System.Serializable]
public class TiersData_Save
{
    public int CurrentTier;
    public int Resource1;
    public int Resource2;
}

[System.Serializable]
public class CampLockEntry
{
    public CampType campType;
    public bool isLocked;
}



