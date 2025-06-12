using UnityEngine;

public class FishingCamp_UpperPanel_Module : MonoBehaviour
{
    public GameObject FishBaitButton;

   
    public void SetupBaitButton()
    {
        FishingCampBait_Button fishingCampBait_Button = FishBaitButton.GetComponent<FishingCampBait_Button>();

        fishingCampBait_Button.DropDownPanel.SetasDefault();

        if (DataGameManager.instance.currentFishingBaitEquipped.item == "")
        {

            fishingCampBait_Button.SetasEmpty();
        }
        else
        {
            fishingCampBait_Button.SetButton(true);
        }
    }

    public void UpdateBaitButton()
    {
        FishingCampBait_Button fishingCampBait_Button = FishBaitButton.GetComponent<FishingCampBait_Button>();

        if (DataGameManager.instance.currentFishingBaitEquipped.item == "")
        {

            fishingCampBait_Button.SetasEmpty();
        }
        else
        {
            fishingCampBait_Button.SetButton(false);
        }
    }
}
    
    

