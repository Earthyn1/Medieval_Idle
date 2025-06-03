using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class FishingCampBait_Button : MonoBehaviour
{
    public DropDownMenu DropDownPanel;
    public Sprite plusImage;
    public Image buttonImage;
    public GameObject AmountTextParent;
    public Text AmountText;

    public void OnClicked()
    {
        bool havebait = false;
        foreach (StorageSlot storageslot in DataGameManager.instance.TownStorage_List)
        {
            int CurrentAmount = storageslot.Quantity;

            ItemData_Struc itemdata;
            if (storageslot.ItemID != null)
            {
                if (DataGameManager.instance.itemData_Array.TryGetValue(storageslot.ItemID, out itemdata))
                {
                    if (itemdata.ItemType == ItemType.Bait)
                    {


                        havebait = true;
                    }
                }
            }
        }

        if (havebait)
        {
            DropDownPanel.PlayAnimation_Open();
            DropDownPanel.PopulateSlots();
        }
        else
        {
            DataGameManager.instance.Game_Text_Alerts.PlayAlert("No Bait to equip!");
        }
    }
       
        
            
        
        
       

    

    public void SetasEmpty()
    {
        AmountTextParent.SetActive(false);  

    }

    public void SetButton()
    {
        ItemData_Struc itemdata = new ItemData_Struc();
        if (DataGameManager.instance.TryFindItemData(DataGameManager.instance.currentFishingBaitEquipped.item, out itemdata))
        {  
            AmountTextParent.SetActive(true);
            AmountText.text = DataGameManager.instance.currentFishingBaitEquipped.qty.ToString();
            buttonImage.sprite = itemdata.ItemImage;
        }

    }
}
