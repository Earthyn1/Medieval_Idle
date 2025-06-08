using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropDownMenu_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text SlotAmount;
    public string itemID;
    public Text SlotName;
    public Image SlotImage;
    public GameObject ParentButton;
    public CampType campType_;
    public DropDownMenu dropDownMenu;
    // Sta
    // rt is called once before the first execution of Update after the MonoBehaviour is created
    public void SetupSlot(ItemData_Struc itemData, int Amount, CampType campType, GameObject parentButton)
    {
        SlotAmount.text = Amount.ToString();
        SlotName.text = itemData.ItemName;
        SlotImage.sprite = itemData.ItemImage;
        campType_ = campType;
        ParentButton = parentButton;
    }

    public void OnClicked()
    {
        if (campType_ != CampType.FishingCamp) return;

        if (!DataGameManager.instance.activeCamps.Any(entry => entry.CampType == CampType.FishingCamp && entry.IsActive))
        {
            FishingCampBait_Button fishingCampBait_Button = ParentButton.GetComponent<FishingCampBait_Button>();

            if (!int.TryParse(SlotAmount.text, out int parsedAmount))
                return;

            var equipped = DataGameManager.instance.currentFishingBaitEquipped;

            if (equipped != null)
            {
                if (equipped.item == itemID)
                {
                    equipped.qty += parsedAmount;
                }
                else
                {
                    bool success = TownStorageManager.AddItem(equipped.item, equipped.qty, CampType.NA);
                    if (!success)
                    {
                        Debug.LogWarning($"Failed to add {equipped.item} x{equipped.qty} to storage. Inventory may be full.");
                        // Optional: Trigger UI warning or fallback logic here
                    }

                    DataGameManager.instance.currentFishingBaitEquipped = new(itemID, parsedAmount, 0);
                }
            }
            else
            {
                DataGameManager.instance.currentFishingBaitEquipped = new(itemID, parsedAmount, 0);
            }

            TownStorageManager.RemoveItem(itemID, parsedAmount);
            dropDownMenu.PlayAnimation_Close();
            fishingCampBait_Button.SetButton(true);
        }
        else
        {
            DataGameManager.instance.Game_Text_Alerts.PlayAlert("Cannot change bait whilst fishing!");
        }

       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Bait_ToolTipUI.instance.ShowTooltipBelow_Bait(transform as RectTransform, itemID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Bait_ToolTipUI.instance.Hide();
    }
}

