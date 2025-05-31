using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System;
using System.Linq;
using static UnityEngine.EventSystems.EventTrigger;

public class Camp_Resource_Slot : MonoBehaviour

{
   
    public string slotkey;
    public CampType campType;    
    public int flipflop;
    public Image progressBar;
    public Image FGImage;
    public Image BGImage;
    public Text lvlUnlocked;
    public Text popCount;
    public Text actionName;
    public GameObject requiredResource_Parent;
    public GameObject Unlocked_Panel;
    public GameObject Locked_Panel;
    public GameObject CampSpecificPrefab;
    public bool isActive = false;
    public bool isLocked = true;
 
    public void OnClicked()
    {
        if (isActive)
        {
            // If already active, always allow deactivation regardless of resources/villagers
            DeactivateActionSlot();
            Debug.Log("Deactivated");
        }
        else
        {
            if (isLocked)
            {
                Debug.Log("Not yet unlocked");
                return; // Early exit as its locked
            }

            if (DataGameManager.instance.actionCampHandler.TryToAddCampSlot(slotkey,campType, this))
            {
                ActivateActionSlot();
            }
            else
            {
                Debug.Log("didnt add slot");
            }
        }     
    }

    public void NotEnoughResourceFlash()
    {
        foreach (Transform child in requiredResource_Parent.transform)
        {
            Required_Resource_Slot childscript = child.GetComponent<Required_Resource_Slot>();

          //  Debug.Log(int.Parse(childscript.itemqty.text) + "////" + TownStorageManager.GetCurrentQuantity(childscript.itemID));
            if (int.Parse(childscript.itemqty.text) > TownStorageManager.GetCurrentQuantity(childscript.itemID))
            {
                childscript.FlashRedAnimation();
                DataGameManager.instance.Game_Text_Alerts.PlayAlert("Not enough resources");
            }
        }
    }
    public void UpdateProgressBar(float progress)
    {
        progressBar.fillAmount = progress;  // Assuming the slider's range is 0 to 1
    }

    public void DeactivateActionSlot()
    {
        Transform parentTransform = progressBar.transform.parent;
        parentTransform.gameObject.SetActive(false);
        requiredResource_Parent.SetActive(true);
        DataGameManager.instance.actionCampHandler.RemoveCampAction(slotkey,campType);
        isActive = false;
    }

    public void ActivateActionSlot()
    {
      Transform parentTransform = progressBar.transform.parent;
      parentTransform.gameObject.SetActive(true);
      requiredResource_Parent.SetActive(false);
      isActive = true;
    }

    public void CheckForDialogs()
    {
        if (slotkey == "Maple Plank")
        {
            if (!DataGameManager.instance.Tutorial_Lists.GetFlag("FirstTimeCedricDialog"))
            {
                TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("JustBuiltSawMill");
                DataGameManager.instance.tutorialManager.SetupTutorial(tutorialGroupData);
                DataGameManager.instance.Tutorial_Lists.SetFlag("FirstTimeCedricDialog", true);
            }
        }


    }
}
