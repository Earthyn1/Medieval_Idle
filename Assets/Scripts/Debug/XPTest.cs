using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class XPTest : MonoBehaviour
{
    public ObjectiveData ObjectiveData;

    void OnMouseDown()
    {
        Debug.Log("Slot clicked: " + gameObject.name);
    }
    // Check if the "T" key is pressed

    void Update()
    {

       
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Add 500 XP to the Lumber Camp (you can change the camp type and amount)
           XPManager.AddXP(CampType.ConstructionCamp, 50);
           XPManager.AddXP(CampType.MiningCamp, 500);
            XPManager.AddXP(CampType.Blacksmith, 500);
            DataGameManager.instance.PlayerGold = 2000;


        }

        // Check if the "F" key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            TownStorageManager.AddItem("BirchPlank", 100, CampType.ConstructionCamp);
            TownStorageManager.AddItem("BirchBeam", 100, CampType.ConstructionCamp);
            TownStorageManager.AddItem("MapleLog", 55, CampType.ConstructionCamp);
            TownStorageManager.AddItem("PoorlyCutStone", 55, CampType.ConstructionCamp);
            TownStorageManager.AddItem("TinOre", 100, CampType.ConstructionCamp);




            DataGameManager.instance.CurrentLandDeedsOwned = DataGameManager.instance.CurrentLandDeedsOwned + 1;
            //TownStorageManager.AddItem("WolfMeat", 1, CampType.ConstructionCamp);
            TownStorageManager.AddItem("CopperNails", 200, CampType.NA);

        }

        // Check if the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
          
            TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("GameIntro_V2");
            DataGameManager.instance.tutorialManager.SetupTutorial(tutorialGroupData);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
           

            DataGameManager.instance.SetCampLockedStatus(CampType.LocalMarket, false);

        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            DataGameManager.instance.Tutorial_Lists.CompleteDialogEvent(TutorialFlagID.UnlockConstructionCamp);        
            DataGameManager.instance.Tutorial_Lists.CompleteDialogEvent(TutorialFlagID.ObjectivesTrackerUnlock);

          //  DataGameManager.instance.SetCampLockedStatus(CampType.FishingCamp, false);
          //  DataGameManager.instance.campButtonUpdater.UpdateCampButtonAsUnlocked(CampType.FishingCamp);

            DataGameManager.instance.SetCampLockedStatus(CampType.MiningCamp, false);
            DataGameManager.instance.campButtonUpdater.UpdateCampButtonAsUnlocked(CampType.MiningCamp);

            DataGameManager.instance.SetCampLockedStatus(CampType.FishingCamp, false);
            DataGameManager.instance.campButtonUpdater.UpdateCampButtonAsUnlocked(CampType.FishingCamp);

            DataGameManager.instance.SetCampLockedStatus(CampType.Blacksmith, false);
            DataGameManager.instance.campButtonUpdater.UpdateCampButtonAsUnlocked(CampType.Blacksmith);

            DataGameManager.instance.Tutorial_Lists.SetFlag("FirstTimeCedricDialog", true);

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            var campType = CampType.ConstructionCamp;
            var boostData = DataGameManager.instance.ConstructionCamp_Boost;
            if (boostData == null) return;

            var tierKey = (boostData.CurrentTier + 1).ToString();
            if (!DataGameManager.instance.allCampTiers.TryGetValue(campType, out var campTiers)) return;
            if (!campTiers.TryGetValue(tierKey, out var tierData)) return;

            boostData.AddResources("MapleBeam", 5, tierData);

            if (boostData.IsResourceComplete(tierData))
            {
                // Trigger tier unlock logic here
            }
        }
    }
}
