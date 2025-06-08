using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Camp/FishingBehavior")]
public class FishingCampBehaviorSO : ScriptableObject, CampSpecificInterface
{
    [SerializeField] private string behaviorName = "Construction Camp Logic";
    [SerializeField] private Color debugColor = Color.green;

    // 🧰 This helper safely grabs the data for the given slot
   

    public void OnSlotLoad(string slotKey)
    {
     

        // Do something with data
    }

    public void OnUpdateSlot(string slotKey)
    {

    }

    public bool HasEnoughCampSpecificResources(string slotKey)
    {
       
        return true;

    }

    public void RemoveCampSpecificResources(string slotKey)
    {
       
       // if(DataGameManager.instance.currentFishingBaitEquipped.item != "")
       // {
       //     DataGameManager.instance.currentFishingBaitEquipped.qty = DataGameManager.instance.currentFishingBaitEquipped.qty - 1;
       //     if (DataGameManager.instance.currentFishingBaitEquipped.qty <= 0)
       //     {
       //         DataGameManager.instance.currentFishingBaitEquipped.item = "";
          //  }
//
        //    FishingCamp_UpperPanel_Module upperpanelfishingCamp = DataGameManager.instance.upperPanelManager.fishingCamp_Buttons.GetComponent<FishingCamp_UpperPanel_Module>();
        //    upperpanelfishingCamp.UpdateBaitButton();
       // }
 
    }

    public void ReturnCampSpecificResources(string slotKey)
    {
       
       
    }

    public void OnCompletedCampSpecificAction(string slotKey)
    {
    }

    
}