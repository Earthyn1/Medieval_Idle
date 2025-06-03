using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering.Universal;


public class CampButtonSetup : MonoBehaviour
{
    public CampTypeData campData;
    public Text CampNameText;
    public Text campXPText;
    public Image CampImage_1;
    public Image CampImage_2;
    public Image Selected_Fade;
    public GameObject ImageContainer;
    public GameObject NewCamp_Parent;
    public Animator NewCamp_Loop_Anim;
    public GameObject GreenDot;
    public GameObject CampPopUsage_Parent;
   

    private void Start()
    {
      

        //Here we are manually setting town storage and lumber to not locked.
        switch (campData.campType)
        {
            case CampType.TownStorage:
                DataGameManager.instance.SetCampLockedStatus(campData.campType, false);
                TownStorageManager.storageQtyText = campXPText; // set the townstorage inv ref
                campXPText.text = "0/12";
                break;

            case CampType.LumberCamp:
                DataGameManager.instance.SetCampLockedStatus(campData.campType, false);
                int currentxp = XPManager.GetCampXP(campData.campType)?.currentXP ?? 0;
                campXPText.text = "Lvl: " + XPManager.GetLevelForXP(currentxp).ToString();

                break;

   


            case CampType.TownOverview:

                ImageContainer.SetActive(false);

                break;

            default:
                SetAsLocked();

                break;
        }

        if (campData.campType == CampType.TownStorage) //Select town storage just so everything is loaded
        {
            StartCoroutine(DelayedButtonPress());
        }
    }

    private IEnumerator DelayedButtonPress()
    {
        yield return new WaitForSeconds(0.5f);
        ButtonHasBeenPressed();
    }

    private void OnValidate()
    {
        EditorUpdateButton();
    }

    private void EditorUpdateButton()
    {
        if (campData != null)
        {
           
            if (CampNameText != null)
            {
                CampNameText.text = campData.campName.ToString();
            }
            if (CampImage_1 != null)
            {
                CampImage_1.sprite = campData.campImage;
                CampImage_2.sprite = campData.campImage;
            }

            
            campXPText.text = "Lvl:40"; 
        }
    }

    public void SetSlotAsUnlocked()
    {
        ImageContainer.SetActive(true);
        CampNameText.text = campData.campName.ToString();
        CampImage_1.sprite = campData.campImage;
        CampImage_2.sprite = campData.campImage;

        int currentxp = XPManager.GetCampXP(campData.campType)?.currentXP ?? 0;
        campXPText.text = "Lvl: " + XPManager.GetLevelForXP(currentxp).ToString();

        switch (campData.campType)
        {
            case CampType.LocalMarket:
                campXPText.text = "";

                break;
        }
    }
    public IEnumerator StartLoadCamp()
    {
        CampType campType = campData.campType;

        // Retrieve the camp's data dictionary
        var campDataDict = DataGameManager.instance.GetCampData(campType);
        DataGameManager.instance.currentActiveCamp = campType; // Set current selected camp

        // Set camp name and update progress bar
        DataGameManager.instance.topPanelManager.campName.text = campData.campName.ToString();

        DataGameManager.instance.upperPanelManager.EnableCampSpecificPanels(campType); //Setup the camp specific Panels
      

        // Switch on camp type to determine setup behavior
        switch (campType)
        {
            case CampType.TownStorage:
                HandleTownStorage();
                break;

            case CampType.LocalMarket:

                HandleLocalMarket();
                break;

            case CampType.TownOverview:

                HandleTownOverview();
                break;       

            default:
                // Setup for all regular camps
                yield return StartCoroutine(DataGameManager.instance.populate_Camp_Slots.SetupCampCategorys(campType));

                XPManager.campProgressBar.gameObject.SetActive(true);
                DataGameManager.instance.tierShield.SetActive(true);
                DataGameManager.instance.topPanelManager.campNameParent.SetActive(false);
                Tier_Shield tiershieldScript = DataGameManager.instance.tierShield.GetComponent<Tier_Shield>(); //Handle shield icon
                tiershieldScript.campImage.sprite = campData.campTierImage;
                XPManager.campProgressBar.UpdateProgressBarInstant(campType);

                // Get first camp category, if any
                Transform parent = DataGameManager.instance.populate_Camp_Slots.Camp_Categorys_Parent.transform;
                if (parent.childCount > 0)
                {
                    Transform firstchild = parent.GetChild(0);
                    Category_Base Category_UI_Script = firstchild.GetComponent<Category_Base>();
                    Category_UI_Script.SetAsSelected();
                    DataGameManager.instance.currentCampCategory = Category_UI_Script.CampCategoryData.campCategory;
                    Debug.Log(DataGameManager.instance.currentCampCategory);
          
                }
                else
                {
                    DataGameManager.instance.currentCampCategory = CampCategorys.NA;
                }

                DataGameManager.instance.populate_Camp_Slots.PopulateSlots(campDataDict);
                break;
        }

        CheckforFirstTimeClick(campType);

        // Update button selection UI
        if (DataGameManager.instance.SelectedButton == null)
        {
            DataGameManager.instance.SelectedButton = GetComponentInParent<Button>().gameObject; // If no button selected yet
            Selected_Fade.gameObject.SetActive(true);
        }
        else
        {
            // Unselect previous button
            DataGameManager.instance.SelectedButton.GetComponent<CampButtonSetup>().Selected_Fade.gameObject.SetActive(false);

            // Select new button
            DataGameManager.instance.SelectedButton = GetComponentInParent<Button>().gameObject;
            Selected_Fade.gameObject.SetActive(true);
        }
    }

    public void CheckforFirstTimeClick(CampType campType)
    {
        // Switch on camp type to determine setup behavior
        switch (campType)
        {
            case CampType.FishingCamp:

                if(!DataGameManager.instance.Tutorial_Lists.GetFlag("FirstTimeFishingCamp"))
                {
                    TutorialGroupData tutorialdialog = DataGameManager.instance.Tutorial_Lists.FindDialog("FirstTimeFishingCamp");
                    DataGameManager.instance.tutorialManager.SetupTutorial(tutorialdialog);
                    DataGameManager.instance.Tutorial_Lists.SetFlag("FirstTimeFishingCamp", true);


                }
                break;
        }
    }

    public void ButtonHasBeenPressed()
    {
        if (DataGameManager.instance.campLockedDict.TryGetValue(campData.campType, out bool isLocked))
        {
            if (!isLocked)
            {
                NewCamp_Loop_Anim.Play("IdleState", 0, 0f);
                NewCamp_Parent.SetActive(false);
                
                StartCoroutine(StartLoadCamp());
            }
            else
            {
                Debug.Log("button locked");
            }
        }


    }

        private void HandleLocalMarket()
    {
        DataGameManager.instance.topPanelManager.campNameParent.SetActive(true);

        TownStorageManager.storageSellManager.SetupBanner(TownStorageManager.storageSellManager.MarketBGImage);
        DataGameManager.instance.populate_Local_Market_Slots.PopulateLocalMarketSlots();
        XPManager.campProgressBar.gameObject.SetActive(false);
        DataGameManager.instance.tierShield.SetActive(false);
        TownStorageManager.storageSellManager.parentBox.SetActive(false);
        TownStorageManager.storageSellManager.itemName.text = "Nothing Selected";
        TownStorageManager.storageSellManager.sellPanel.SetActive(true);


        if (!DataGameManager.instance.Tutorial_Lists.GetFlag("FirstTimeVisitLocalMarket")) //This handles first time local market tutorial
        {
            TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("FirstTimeLocalMarket");
            DataGameManager.instance.tutorialManager.SetupTutorial(tutorialGroupData);

            if(Objective_Manager.objectivesTracker.IsOpen)
            {
                Objective_Manager.objectivesTracker.ToggleObjectiveTracker_State();
            }
        }
    }

    private void HandleTownOverview()
    {
        DataGameManager.instance.topPanelManager.campNameParent.SetActive(true);
        TownStorageManager.storageSellManager.SetupBanner(TownStorageManager.storageSellManager.MarketBGImage);
        XPManager.campProgressBar.gameObject.SetActive(false);
        DataGameManager.instance.tierShield.SetActive(false);
        TownStorageManager.storageSellManager.parentBox.SetActive(false);
    }

    private void HandleTownStorage()
    {
        DataGameManager.instance.topPanelManager.campNameParent.SetActive(true);

        // Ensure the object is active
        if (!TownStorageManager.storageSellManager.gameObject.activeInHierarchy)
        {
            TownStorageManager.storageSellManager.gameObject.SetActive(true);
        }

        TownStorageManager.storageSellManager.SetupBanner(TownStorageManager.storageSellManager.StorageBGImage);
        DataGameManager.instance.populate_Storage_Slots.PopulateItemSlots();
        XPManager.campProgressBar.gameObject.SetActive(false);
        DataGameManager.instance.tierShield.SetActive(false);
        TownStorageManager.storageSellManager.parentBox.SetActive(false);
        TownStorageManager.storageSellManager.itemName.text = "Nothing Selected";

        bool isLocked;

        if (DataGameManager.instance.campLockedDict.TryGetValue(CampType.LocalMarket, out isLocked))  //This for tutorial early if Localmarket not unlocked then we cannot sell things
        {
            if (isLocked)
            { 
                TownStorageManager.storageSellManager.sellPanel.SetActive(false);
            }
            else
            {
                TownStorageManager.storageSellManager.sellPanel.SetActive(true);

                if (!DataGameManager.instance.Tutorial_Lists.GetFlag("FirstTimeVisitStorageSellPanel"))
                {
                    TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("FirstTimeStorageSellMenu");
                    DataGameManager.instance.tutorialManager.SetupTutorial(tutorialGroupData);

                    if (Objective_Manager.objectivesTracker.IsOpen)
                    {
                        Objective_Manager.objectivesTracker.ToggleObjectiveTracker_State();
                    }

                }
            }
        }
    }

    public void SetAsLocked()
    {
        ImageContainer.SetActive(false);
        campXPText.text = "";
        CampNameText.text = "???";
    }
    public void UpdateButtonCampLevel()
    {


    }

}
