using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class CampButtonSetup : MonoBehaviour
{
    public CampTypeData campData;
    public Text CampNameText;
    public Text campXPText;
    public Image CampImage_1;
    public Image CampImage_2;
    public Image Selected_Fade;
   

    private void Start()
    {
        EditorUpdateButton();

        if (campData.campType == CampType.TownStorage)
        {
            TownStorageManager.storageQtyText = campXPText; // set the townstorage inv ref
        }
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
      

        // Switch on camp type to determine setup behavior
        switch (campType)
        {
            case CampType.TownStorage:
                HandleTownStorage();
                break;

            case CampType.LocalMarket:

                HandleLocalMarket();
                break;

            default:
                // Setup for all regular camps
                yield return StartCoroutine(DataGameManager.instance.populate_Camp_Slots.SetupCampCategorys(campType));

                XPManager.campProgressBar.gameObject.SetActive(true);
                DataGameManager.instance.tierShield.SetActive(true);
                DataGameManager.instance.topPanelManager.campNameParent.SetActive(true);
                Tier_Shield tiershieldScript = DataGameManager.instance.tierShield.GetComponent<Tier_Shield>(); //Handle shield icon
                tiershieldScript.campImage.sprite = campData.campTierImage;
                XPManager.campProgressBar.UpdateProgressBar(campType);

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


    public void ButtonHasBeenPressed()
    {
        StartCoroutine(StartLoadCamp());
        
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
    }
    private void HandleTownStorage()
    {
        DataGameManager.instance.topPanelManager.campNameParent.SetActive(true);

        TownStorageManager.storageSellManager.SetupBanner(TownStorageManager.storageSellManager.StorageBGImage);
        DataGameManager.instance.populate_Storage_Slots.PopulateItemSlots();
        XPManager.campProgressBar.gameObject.SetActive(false);
        DataGameManager.instance.tierShield.SetActive(false);
        TownStorageManager.storageSellManager.parentBox.SetActive(false);
        TownStorageManager.storageSellManager.itemName.text = "Nothing Selected";
    }
    public void UpdateButtonCampLevel()
    {


    }

}
