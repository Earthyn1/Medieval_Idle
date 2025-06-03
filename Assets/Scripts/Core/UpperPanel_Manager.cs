using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UpperPanel_Manager : MonoBehaviour
{
    public GameObject campSpecificButtons;

    public GameObject constructionCamp_Buttons;
    public GameObject fishingCamp_Buttons;



    void Start()
    {
        DataGameManager.instance.upperPanelManager = this;
    }

    public void EnableCampSpecificPanels(CampType camptype)
    {
        foreach (Transform child in campSpecificButtons.transform)
        {
            child.gameObject.SetActive(false);
        }

        switch (camptype)
        {
            case CampType.TownStorage:

                break;

            case CampType.ConstructionCamp:
                constructionCamp_Buttons.SetActive(true);
                ConstructionCamp_UpperPanel_Module construcCampUpper_Script = constructionCamp_Buttons.GetComponent<ConstructionCamp_UpperPanel_Module>();
                construcCampUpper_Script.UpdateText();
                break;

            case CampType.FishingCamp:
                fishingCamp_Buttons.SetActive(true);
                FishingCamp_UpperPanel_Module fishingCampUpper_Script = fishingCamp_Buttons.GetComponent<FishingCamp_UpperPanel_Module>();
                fishingCampUpper_Script.SetupBaitButton();

                break;
        }
    }
}


