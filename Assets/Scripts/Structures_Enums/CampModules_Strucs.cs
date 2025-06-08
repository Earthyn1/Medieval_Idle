using UnityEngine;

public class ConstructionCampModule
{
    public int landDeed;
    public bool SingleUseSlot;
    public string BuildingIDUnlocked;
    public string PreviousUpgradeRequired;
    public bool isCompleted = false;

    public ConstructionCampModule(int landDeed, bool SingleUseSlot, string BuildingIDUnlocked, string PreviousUpgradeRequired)
    {
        this.landDeed = landDeed;
        this.SingleUseSlot = SingleUseSlot;
        this.BuildingIDUnlocked = BuildingIDUnlocked;
        this.PreviousUpgradeRequired = PreviousUpgradeRequired;
       
    }
}

public class FishingCampModule
{
    public FishingCampModule ()
    {

    }
}

public interface CampSpecificInterface
{
    void OnSlotLoad(string slotKey);
    void OnUpdateSlot(string slotKey);
    bool HasEnoughCampSpecificResources(string slotKey);
    void RemoveCampSpecificResources(string slotKey);

    void ReturnCampSpecificResources(string slotKey);
    void OnCompletedCampSpecificAction(string slotKey);
}

public interface CampUISlotInterface
{
    void OnUISlotUpdate(string slotKey);
    void OnUISlotLoad(string slotKey);
}
