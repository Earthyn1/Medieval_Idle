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
            bool leveledUp = XPManager.AddXP(CampType.ConstructionCamp, 500);

            if (leveledUp)
            {
                Debug.Log("Lumber Camp leveled up!");
            }
            else
            {
                Debug.Log("No level up.");
            }

            // Get and display the progress for the Lumber Camp
            float progress = XPManager.GetLevelProgress(CampType.LumberCamp);
            int camplevel = DataGameManager.instance.campXPDictionaries[CampType.LumberCamp].currentLevel;
            int campxp = DataGameManager.instance.campXPDictionaries[CampType.LumberCamp].currentXP;
            Debug.Log($"Lumber Camp Progress: {progress * 100f}%");
            Debug.Log($"Lumber Camp Level: {camplevel} and camp XP: {campxp}!");


        }

        // Check if the "F" key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            TownStorageManager.AddItem("MaplePlank", 100, CampType.ConstructionCamp);
            TownStorageManager.AddItem("MapleBeam", 100, CampType.ConstructionCamp);
            TownStorageManager.AddItem("CopperNails", 100, CampType.ConstructionCamp);
            TownStorageManager.AddItem("MapleLog", 100, CampType.ConstructionCamp);
        }

        // Check if the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            DataGameManager.instance.tutorialManager.SetupTutorial(DataGameManager.instance.Tutorial_Lists.tutorialSteps_Intro);


        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Objective_Manager.CreateNewObjective(ObjectiveData);
       
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Objective_Manager.UpdateObjectives("MapleLog", 1);
        }
    }
}
