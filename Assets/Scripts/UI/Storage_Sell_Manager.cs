using UnityEngine;
using UnityEngine.UI;

public class StorageSellManager : MonoBehaviour
{
    public Text itemName;
    public Text itemSinglePrice;
    public Text itemType;
    public Text itemDescription;
    public Text itemMaxQty;
    public Image itemImage;
    public Text currentQtySelected;    
    public Text finalPrice;
    public Slider slider;
    public GameObject parentBox;
    
    

    private ItemData_Struc currentItemSelected;

    private void Start()
    {
        TownStorageManager.storageSellManager = this;
    }

    public void OnClickSell()
    {
        if (TownStorageManager.currentlySelectedInventorySlot != null)
        {
            var slotTransform = TownStorageManager.currentlySelectedInventorySlot.transform;
            int slotIndex = slotTransform.GetSiblingIndex();

            TownStorageManager.RemoveItemFromSlot(slotIndex, (int)slider.value);
            Debug.Log("Removed item");
            DataGameManager.instance.PlayerGold += int.Parse(finalPrice.text);
            DataGameManager.instance.topPanelManager.UpdateGold();
        }
        else
        {
            SetasNothingSelected();         
        }       
    }

    public void SetupUpUI(string itemID)
    {
        
            parentBox.SetActive(true);
            DataGameManager.instance.itemData_Array.TryGetValue(itemID, out var data);
            Item_Slot_Base script = TownStorageManager.currentlySelectedInventorySlot.GetComponent<Item_Slot_Base>();
            currentItemSelected = data;
            itemName.text = data.ItemName;
            itemDescription.text = data.Description;
            itemImage.sprite = data.ItemImage;
            itemSinglePrice.text = data.ItemSellPrice.ToString();
            itemType.text = data.ItemType;
            itemMaxQty.text = script.itemDataBasic.Quantity.ToString();

            slider.maxValue = script.itemDataBasic.Quantity;
            slider.minValue = 1f;
            slider.value = (1 + float.Parse(itemMaxQty.text)) / 2;


    }

    public void SetasNothingSelected()
    {
        Item_Slot_Base script = TownStorageManager.currentlySelectedInventorySlot.GetComponent<Item_Slot_Base>();
        script.boxOutline.color = new Color(117f / 255f, 229f / 255f, 101f / 255f, 0f / 255f);

        parentBox.SetActive(false);
        TownStorageManager.currentlySelectedInventorySlot = null;
        itemName.text = "Nothing Selected";

    }

    public void UpdateUI()
    {
  
        if (TownStorageManager.currentlySelectedInventorySlot != null)
        {
            Item_Slot_Base script = TownStorageManager.currentlySelectedInventorySlot.GetComponent<Item_Slot_Base>(); //get ref to itemslot
            if (script.itemDataBasic.Quantity == 0) 
            {
                SetasNothingSelected();
            }
            else
            {
                itemMaxQty.text = script.itemDataBasic.Quantity.ToString();
                slider.maxValue = script.itemDataBasic.Quantity; //setup slider and the values.
                slider.minValue = 1f;
                slider.value = Mathf.Clamp(slider.value, 1f, slider.maxValue = script.itemDataBasic.Quantity);
                currentQtySelected.text = slider.value.ToString();
                finalPrice.text = ((int)slider.value * int.Parse(itemSinglePrice.text)).ToString();
            }

        }

     
    }
}
