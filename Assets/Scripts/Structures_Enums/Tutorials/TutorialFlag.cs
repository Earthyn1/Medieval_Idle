using UnityEngine;

[CreateAssetMenu]
public class TutorialFlag : ScriptableObject
{
    public bool isComplete;
}


public enum TutorialFlagID
{
    NA,
    OpenInventory,
    CompleteFirstObjective,
    VisitBlacksmith,
    ObjectivesTrackerUnlock,
    UnlockConstructionCamp,
    FirstTimeVisitStorageSellPanel,
    SwitchTo_UnlockConstructionCamp_FromOpen,
    FirstTimeCedricDialog,
    GivePlayerCopperNails,
    PurchaseLandDeedUnlocked,
    FirstTimeVisitLocalMarket,
    FirstTimeFishingCamp,
    FirstTimeBlacksmith,
    FirstTimeMiningCamp,
    CanOpenTierSystem,
    GivePlayerBasicBait,
    GivePlayerBasicFuel,



    // etc.
}