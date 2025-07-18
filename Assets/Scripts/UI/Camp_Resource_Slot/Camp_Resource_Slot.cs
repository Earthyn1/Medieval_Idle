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
    public Animator animator;
    public GameObject populationBox;
 
    public void OnClicked()
    {
        if (isActive)
        {
            // If already active, always allow deactivation regardless of resources/villagers
            DeactivateActionSlot();
          //  Debug.Log("Deactivated");
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
             //  Debug.Log("Activate Slot!");
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

    public void NotEnoughVillagerFlash()
    {
        animator.Play("IdleState", 0, 0f);
        animator.ResetTrigger("PlayAnimation");
        animator.SetTrigger("PlayAnimation");

    }
    public void UpdateProgressBar(float progress)
    {
        progressBar.fillAmount = progress;  // Assuming the slider's range is 0 to 1
    }

    public void DeactivateActionSlot()
    {
        Debug.Log("Deactivate slot" + slotkey);
        if (slotkey == "Repair Sawmill")
        {
            DataGameManager.instance.Game_Text_Alerts.PlayAlert("Cannot deativate this action!");
            return;
        }
        if (progressBar != null)
        {
            Transform parentTransform = progressBar.transform.parent;
            parentTransform.gameObject.SetActive(false);
            // Continue using it safely...
        }
        else
        {
            Debug.LogWarning("progressBar was destroyed or is null");
        }

        if (requiredResource_Parent != null)
        {
            requiredResource_Parent.SetActive(true);
        }
        else
        {
            Debug.LogWarning("requiredResource_Parent was destroyed or is missing.");
        }
        if (populationBox!= null)
        {
            populationBox.SetActive(true);
        }
        else
        {
            Debug.LogWarning("populationBox was destroyed or is missing.");

        }

        DataGameManager.instance.actionCampHandler.ReturnResources(slotkey, campType);
        DataGameManager.instance.actionCampHandler.RemoveCampAction(slotkey,campType);
        isActive = false;
    }

    public void DeactivateActionSlot_WithoutReturningResources()
    {
        Transform parentTransform = progressBar.transform.parent;
        parentTransform.gameObject.SetActive(false);
        requiredResource_Parent.SetActive(true);
        isActive = false;
        populationBox.SetActive(true);
    }


    public void ActivateActionSlot()
    {
      Transform parentTransform = progressBar.transform.parent;
        populationBox.SetActive(false);

        parentTransform.gameObject.SetActive(true);
      requiredResource_Parent.SetActive(false);
      isActive = true;
    }

    public void CheckForDialogs()
    {
        
    }

    public void CheckForDialogs_Old()
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
