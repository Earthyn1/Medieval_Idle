using NUnit.Framework.Interfaces;
using UnityEngine;

public class XPTest : MonoBehaviour
{
    public ObjectiveData ObjectiveData;

    void Update()
    {
        // Check if the "T" key is pressed
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Add 500 XP to the Lumber Camp (you can change the camp type and amount)
           XPManager.AddXP(CampType.ConstructionCamp, 500);
           XPManager.AddXP(CampType.MiningCamp, 500);


        }

        // Check if the "F" key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            TownStorageManager.AddItem("MaplePlank", 100, CampType.ConstructionCamp);
            TownStorageManager.AddItem("MapleBeam", 100, CampType.ConstructionCamp);
            TownStorageManager.AddItem("MapleLog", 200, CampType.ConstructionCamp);
            TownStorageManager.AddItem("CopperNails", 100, CampType.ConstructionCamp);
          //  TownStorageManager.AddItem("CopperbackTrout", 100, CampType.ConstructionCamp);


            DataGameManager.instance.CurrentLandDeedsOwned = DataGameManager.instance.CurrentLandDeedsOwned + 1;
            //TownStorageManager.AddItem("WolfMeat", 1, CampType.ConstructionCamp);
            TownStorageManager.AddItem("CopperNails", 200, CampType.NA);

        }

        // Check if the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
          
            TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("GameIntro");
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

            DataGameManager.instance.SetCampLockedStatus(CampType.Blacksmith, false);
            DataGameManager.instance.campButtonUpdater.UpdateCampButtonAsUnlocked(CampType.Blacksmith);

            DataGameManager.instance.Tutorial_Lists.SetFlag("FirstTimeCedricDialog", true);

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Objective_Manager.CreateNewObjective(ObjectiveData);
           DataGameManager.instance.boostsManager.AddToBaseBoost(CampType.MiningCamp, "Rapid Mining", 1.5f);
            DataGameManager.instance.boostsManager.AddToBaseBoost(CampType.MiningCamp, "Expanded Veins", 10);
            DataGameManager.instance.boostsManager.AddToBaseBoost(CampType.MiningCamp, "Prospector's Insight", 25);
          //  DataGameManager.instance.boostsManager.SetupCampBoosts(DataGameManager.instance.currentActiveCamp);






        }
    }
}
