using UnityEngine;
using UnityEngine.UI;

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
            TownStorageManager.storageQtyText = campXPText;
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

    public void ButtonHasBeenPressed()
    {
        
        CampType campType = campData.campType;

        var campDataDict = DataGameManager.instance.GetCampData(campType);
        DataGameManager.instance.currentActiveCamp = campType; //set the current camp selected

        DataGameManager.instance.topPanelManager.campName.text = campData.campName.ToString(); //set camp name and update progress bar
        XPManager.campProgressBar.UpdateProgressBar(campType);

        if (campData.campType != CampType.TownStorage && campData.campType != CampType.LocalMarket) //Skip if townstorage or Localmarket
        {          
            DataGameManager.instance.populate_Camp_Slots.PopulateSlots(campDataDict);
            XPManager.campProgressBar.gameObject.SetActive(true);
            DataGameManager.instance.tierShield.SetActive(true);


        }

        if (campData.campType == CampType.TownStorage)
        {
            DataGameManager.instance.populate_Storage_Slots.PopulateItemSlots();
            XPManager.campProgressBar.gameObject.SetActive(false);
            DataGameManager.instance.tierShield.SetActive(false);

            TownStorageManager.storageSellManager.parentBox.SetActive(false); //setup the sell box as empty
            TownStorageManager.storageSellManager.itemName.text = "Nothing Selected";


            //code here for townstorage 
        }

        if (campData.campType == CampType.LocalMarket)
        {
            LocalMarketManager.SetupLocalMarket();
            //code here for Local Market 
        }

        if (DataGameManager.instance.SelectedButton == null)
        {
            DataGameManager.instance.SelectedButton = GetComponentInParent<Button>().gameObject; //if no side button selected
            Selected_Fade.gameObject.SetActive(true);
        }
        else
        {
            DataGameManager.instance.SelectedButton.GetComponent<CampButtonSetup>().Selected_Fade.gameObject.SetActive(false); //first update previous selected button an replace ref.
            DataGameManager.instance.SelectedButton = GetComponentInParent<Button>().gameObject;
            Selected_Fade.gameObject.SetActive(true);
        }
    }

    public void UpdateButtonCampLevel()
    {


    }

}
