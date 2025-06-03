using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Tutorial_Lists : MonoBehaviour
{
    public TutorialManager TutorialManager;
    public List<TutorialGroupData> ListOfDialogs;
    public GameObject parentObject;


    void Start()
    {
        TutorialManager = GetComponent<TutorialManager>();
        DataGameManager.instance.Tutorial_Lists = this;
    }

    public void CompleteDialogEvent(TutorialFlagID tutorialFlagID)
    {
        switch (tutorialFlagID)
        {
            case TutorialFlagID.CompleteFirstObjective:
                Debug.Log("Completed first objective!");
                break;

            case TutorialFlagID.SwitchTo_UnlockConstructionCamp_FromOpen:
                TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("ExplainCompletedObjectivesButton_FromOpen");
                DataGameManager.instance.tutorialManager.SetupTutorial(tutorialGroupData);
                SetFlag(tutorialFlagID.ToString(), true);
                break;

            case TutorialFlagID.ObjectivesTrackerUnlock:
                SetFlag(tutorialFlagID.ToString(), true);    
                break;

            case TutorialFlagID.UnlockConstructionCamp:
                DataGameManager.instance.SetCampLockedStatus(CampType.ConstructionCamp, false);
                DataGameManager.instance.campButtonUpdater.UpdateCampButtonAsUnlocked(CampType.ConstructionCamp);
               // XPManager.levelUpNotification.CampUnlocked("Construction Camp");

                SetFlag(tutorialFlagID.ToString(), true);
                break;

            case TutorialFlagID.FirstTimeVisitStorageSellPanel:
                TownStorageManager.AddItem("WolfMeat", 3, CampType.NA);
                SetFlag(tutorialFlagID.ToString(), true);
                break;

            case TutorialFlagID.PurchaseLandDeedUnlocked:
                SetFlag(tutorialFlagID.ToString(), true);
                break;

            case TutorialFlagID.FirstTimeVisitLocalMarket:
                SetFlag(tutorialFlagID.ToString(), true);
                break;

            case TutorialFlagID.GivePlayerCopperNails:
                TownStorageManager.AddItem("CopperNails", 200, CampType.NA);
                SetFlag(tutorialFlagID.ToString(), true);
                break;

            case TutorialFlagID.FirstTimeCedricDialog:
                SetFlag(tutorialFlagID.ToString(), true);
                break;




        }
    }

    public void ClearAllButtonListeners()
    {
        Button[] buttons = parentObject.GetComponentsInChildren<Button>(true); // true includes inactive children

        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveAllListeners();
        }
    }
    public bool GetFlag(string key)
    {
        return DataGameManager.instance.tutorialFlags.TryGetValue(key, out var value) ? value : false;
    }

    public void SetFlag(string key, bool value)
    {
        Debug.Log("Flag set for - " + key +"As - " + value);
        DataGameManager.instance.tutorialFlags[key] = value;
    }
    public TutorialGroupData FindDialog(string id)
    {
        TutorialGroupData foundGroup = ListOfDialogs.FirstOrDefault(g => g.groupId == id);

        // Check if found
        if (foundGroup != null)
        {
            Debug.Log("Found group: " + foundGroup.groupId);
            return foundGroup;
        }
        else
        {
            Debug.Log("Group not found");
            return null;
        }
    }




}
