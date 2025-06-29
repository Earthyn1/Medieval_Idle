using NUnit.Framework.Interfaces;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class FishingCampBait_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public DropDownMenu DropDownPanel;
    public Sprite plusImage;
    public Image buttonImage;
    public GameObject AmountTextParent;
    public Text AmountText;
    public GameObject RemoveButton;

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
            Button myButton = DropDownPanel.BGDimmer.GetComponentInChildren<Button>();
            if (myButton != null) //Making sure the bgdimmer is interable after tutorial!
            {
                myButton.interactable = true;
            }
        }
        else
        {
            DataGameManager.instance.Game_Text_Alerts.PlayAlert("No Bait to equip!");
        }
    }

    public void OnRemoveClicked()
    {

        if (!DataGameManager.instance.activeCamps.Any(entry => entry.CampType == CampType.FishingCamp && entry.IsActive))
        {
            SimpleItemData currentEquippedbait = DataGameManager.instance.currentFishingBaitEquipped;

            bool added = TownStorageManager.AddItem(currentEquippedbait.item, currentEquippedbait.qty, CampType.NA);

            if (!added)
            {
                DataGameManager.instance.Game_Text_Alerts.PlayInvFull();
                return;
                // Optionally: trigger a UI message, fallback storage, etc.
            }
            DataGameManager.instance.currentFishingBaitEquipped.item = "";
            SetasEmpty();

            var campDataDict = DataGameManager.instance.GetCampData(CampType.FishingCamp);

            if (DataGameManager.instance.currentActiveCamp == CampType.FishingCamp)
            {
                DataGameManager.instance.populate_Camp_Slots.PopulateSlots(campDataDict);
                DataGameManager.instance.boostsManager.SetupCampBoosts(CampType.FishingCamp);
            }
        }
        else
        {
            DataGameManager.instance.Game_Text_Alerts.PlayAlert("Cannot change bait whilst fishing!");
        }


    }

    public void SetasEmpty()
    {
        AmountTextParent.SetActive(false);  
        buttonImage.sprite = plusImage;
        RemoveButton.SetActive(false);
    }

    public void SetButton(bool PopulateSlots)
    {
        ItemData_Struc itemdata = new ItemData_Struc();
        if (DataGameManager.instance.TryFindItemData(DataGameManager.instance.currentFishingBaitEquipped.item, out itemdata))
        {  
            AmountTextParent.SetActive(true);
            RemoveButton.SetActive(true);
            AmountText.text = DataGameManager.instance.currentFishingBaitEquipped.qty.ToString();
            buttonImage.sprite = itemdata.ItemImage;
            DataGameManager.instance.boostsManager.SetupCampBoosts(CampType.FishingCamp);

            var campDataDict = DataGameManager.instance.GetCampData(CampType.FishingCamp);

            if(DataGameManager.instance.currentActiveCamp == CampType.FishingCamp && PopulateSlots)
            {
                DataGameManager.instance.populate_Camp_Slots.PopulateSlots(campDataDict);

            }
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DataGameManager.instance.currentFishingBaitEquipped.item != "")
        {
            Bait_ToolTipUI.instance.ShowTooltipBelow_Bait(transform as RectTransform, DataGameManager.instance.currentFishingBaitEquipped.item);

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Bait_ToolTipUI.instance.Hide();
    }
}
