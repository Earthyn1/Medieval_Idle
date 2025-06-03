using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class LandDeed_Manager : MonoBehaviour
{
    public Text purchaseDeedAmountText;
    public GameObject purchaseDeedParentBox;
    public Button purchaseDeedAmountButton;

    private float basePrice = 160f;
    private float growthRate = 1.15f;

    public int GetDeedCost()
    {
        return Mathf.RoundToInt(basePrice * Mathf.Pow(growthRate, DataGameManager.instance.landDeedsbrought));
    }

    public void SetupDeedBox()
    {
        purchaseDeedAmountButton.interactable = true;
        //set purchase amount and red text if not enough black text if enough.
        purchaseDeedAmountText.text = GetDeedCost().ToString();
        if(GetDeedCost() > DataGameManager.instance.PlayerGold)
        {
            purchaseDeedAmountText.color = Color.red;
        }
        else
        {
            purchaseDeedAmountText.color = Color.green;
        }
    }


    public void OnExitWindowPressed()
    {
       
        if (!DataGameManager.instance.tutorialManager.textBoxParent.activeInHierarchy)
        {
            Animator animator = purchaseDeedParentBox.GetComponent<Animator>();
            animator.Play("IdleState", 0, 0f);
            animator.ResetTrigger("Close");
            animator.SetTrigger("Close");
        }
        else
        {
            Debug.Log("Text box is open?");
        }
       
       
    }

    public void CloseWindow() //triggers after animation via event
    {
        Animator animator = purchaseDeedParentBox.GetComponent<Animator>();
        animator.Play("IdleState", 0, 0f);
        purchaseDeedParentBox.SetActive(false);
       
    }

    public void PurchaseDeed()
    {
        if(DataGameManager.instance.PlayerGold < GetDeedCost())
        {
            Debug.Log("Not enough gold!");
        }
        else
        {
            //Update player gold, landdeeds bought
            DataGameManager.instance.PlayerGold = DataGameManager.instance.PlayerGold - GetDeedCost();
            DataGameManager.instance.landDeedsbrought = DataGameManager.instance.landDeedsbrought + 1;
            DataGameManager.instance.CurrentLandDeedsOwned = DataGameManager.instance.CurrentLandDeedsOwned + 1;
            DataGameManager.instance.topPanelManager.UpdateGold();
            int cost = GetDeedCost();
            DataGameManager.instance.topPanelManager.AddedRemovedGoldAnim(false, cost);


            SetupDeedBox();

            //Update the campslots to show new landdeed
            var campDataDict = DataGameManager.instance.GetCampData(CampType.ConstructionCamp);
            DataGameManager.instance.populate_Camp_Slots.PopulateSlots(campDataDict);
            //update upper panels to represent the landdeed
            DataGameManager.instance.upperPanelManager.EnableCampSpecificPanels(CampType.ConstructionCamp);

            Animator animator = purchaseDeedParentBox.GetComponent<Animator>();
            animator.Play("IdleState", 0, 0f);
            animator.ResetTrigger("Close");
            animator.SetTrigger("Close");
        }
       
    }

  
}
