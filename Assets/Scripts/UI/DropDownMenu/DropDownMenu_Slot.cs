using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.Collections;

public class DropDownMenu_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Text SlotAmount;
    public string itemID;
    public Text SlotName;
    public Text SlotType;
    public Image SlotImage;
    public GameObject ParentButton;
    public CampType campType_;
    public DropDownMenu dropDownMenu;
    public Animator animator;

    public float holdTimeRequired = 1f;
    private float holdTimer = 0f;
    private bool isHolding = false;

    public void SetupSlot(ItemData_Struc itemData, int Amount, CampType campType, GameObject parentButton)
    {
        if(DataGameManager.instance.currentActiveCamp == CampType.Blacksmith)
        {
            SlotAmount.text = (Amount * itemData.FuelAmount).ToString();
            SlotType.text = "Fuel";
        }
        else
        {
            SlotAmount.text = Amount.ToString();
            SlotType.text = "Bait";
        }
       
        SlotName.text = itemData.ItemName;
        SlotImage.sprite = itemData.ItemImage;
        campType_ = campType;
        ParentButton = parentButton;
    }

    void Update()
    {
        

        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdTimeRequired)
            {
                isHolding = false;
                holdTimer = 0f;
                OnHeldClick();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (DataGameManager.instance.currentActiveCamp != CampType.Blacksmith) return;

        isHolding = true;
        holdTimer = 0f;

        animator.Play("IdleState", 0, 0f);
        animator.ResetTrigger("HoldDown");
        animator.SetTrigger("HoldDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CancelHold();
    }

    private void CancelHold()
    {
        isHolding = false;
        holdTimer = 0f;
        animator.Play("IdleState", 0, 0f);
    }

    void OnHeldClick()
    {
      
        if (!DataGameManager.instance.TryFindItemData(itemID, out var item))
            return;

        int currentqty = TownStorageManager.GetCurrentQuantity(itemID);
        int fuelPerItem = item.FuelAmount;
        int fuelNeeded = DataGameManager.instance.maxBlacksmithFuel - DataGameManager.instance.currentBlacksmithFuel;

        Debug.Log("fuel needed is" + fuelNeeded);

        if (fuelNeeded <= 0 || currentqty <= 0 || fuelPerItem <= 0)
            return;

        // Calculate max fuel we can add based on currentqty
        int maxFuelFromItems = currentqty * fuelPerItem;
        int fuelToAdd = Mathf.Min(fuelNeeded, maxFuelFromItems);

        // Calculate how many items we need to consume to add that fuel
        int itemsToRemove = Mathf.CeilToInt((float)fuelToAdd / fuelPerItem);

        // Recalculate the actual fuel we'll get from that many items
        int actualFuelAdded = itemsToRemove * fuelPerItem;

        // Clamp to max fuel just in case
        int finalFuel = Mathf.Min(DataGameManager.instance.currentBlacksmithFuel + actualFuelAdded, DataGameManager.instance.maxBlacksmithFuel);

        // Apply
        DataGameManager.instance.currentBlacksmithFuel = finalFuel;
        TownStorageManager.RemoveItem(itemID, itemsToRemove);

        // UI update
        var blacksmithCampUpper_Script = DataGameManager.instance.upperPanelManager.blacksmithCamp_Buttons.GetComponent<UpperPanel_Blacksmith>();
        blacksmithCampUpper_Script.SetupFuelBar();

        DataGameManager.instance.populate_Camp_Slots.UpdateCampSpecific_UI();
        

        dropDownMenu.PopulateSlots();

        Debug.Log($"Held click triggered on {gameObject.name}, removed {itemsToRemove} items, added {actualFuelAdded} fuel");
    }


    public void OnClickedButton()
    {
       
        if (campType_ == CampType.FishingCamp)
        {
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
                StartCoroutine(CloseDropdownAfterClick());

                fishingCampBait_Button.SetButton(true);
            }
            else
            {
                DataGameManager.instance.Game_Text_Alerts.PlayAlert("Cannot change bait whilst fishing!");
            }
        }

        if (campType_ == CampType.Blacksmith)
        {

            DataGameManager.instance.TryFindItemData(itemID, out var item);

            int currentqty = TownStorageManager.GetCurrentQuantity(itemID);
            int fuelPerItem = item.FuelAmount;
            int fuelNeeded = DataGameManager.instance.maxBlacksmithFuel - DataGameManager.instance.currentBlacksmithFuel;

            Debug.Log("fuel needed is" + fuelNeeded);

            if (fuelNeeded <= 0 || currentqty <= 0 || fuelPerItem <= 0)
            {
                DataGameManager.instance.Game_Text_Alerts.PlayAlert("Fuel is full!");
                return;
            }

            DataGameManager.instance.currentBlacksmithFuel = DataGameManager.instance.currentBlacksmithFuel + item.FuelAmount;

            UpperPanel_Blacksmith blacksmithCampUpper_Script = DataGameManager.instance.upperPanelManager.blacksmithCamp_Buttons.GetComponent<UpperPanel_Blacksmith>();
            blacksmithCampUpper_Script.SetupFuelBar();

            TownStorageManager.RemoveItem(itemID, 1);

            dropDownMenu.PopulateSlots();

            
            
            
        }

        DataGameManager.instance.populate_Camp_Slots.UpdateCampSpecific_UI();
    }

    private IEnumerator CloseDropdownAfterClick()
    {
        yield return null; // wait one frame
        dropDownMenu.PlayAnimation_Close();
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        if(DataGameManager.instance.currentActiveCamp == CampType.FishingCamp)
        {
            Bait_ToolTipUI.instance.ShowTooltipBelow_Bait(transform as RectTransform, itemID);
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Bait_ToolTipUI.instance.Hide();
        CancelHold(); // Optional: cancel if pointer leaves
    }
}

