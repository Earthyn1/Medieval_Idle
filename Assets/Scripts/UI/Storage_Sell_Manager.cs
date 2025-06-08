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
    public Text singleCostText;
    public Text buySellText;
    public Text buySellText_2;
    public CanvasGroup SellBuyButtonCanvasGroup;
    public Texture2D StorageBGImage;
    public Texture2D MarketBGImage;
    public Image BG_Banner;
    public GameObject sellPanel;



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

            Item_Slot_Base script = TownStorageManager.currentlySelectedInventorySlot.GetComponent<Item_Slot_Base>();

            if (!script.isLocalMarketSlot)
            {
                TownStorageManager.RemoveItemFromSlot(slotIndex, (int)slider.value); //If selling from storage code
                Debug.Log("Removed item");
                DataGameManager.instance.topPanelManager.AddedRemovedGoldAnim(true, int.Parse(finalPrice.text));
                DataGameManager.instance.PlayerGold += int.Parse(finalPrice.text);
                DataGameManager.instance.topPanelManager.UpdateGold();

                Item_Slot_Base baseslotscript = TownStorageManager.currentlySelectedInventorySlot.GetComponent<Item_Slot_Base>();
                baseslotscript.StorageSlotClicked(); //here we update the slot to update the info/unselect the slot if its empty
            }
            else
            {
                if(DataGameManager.instance.PlayerGold >= int.Parse(finalPrice.text)) //Buying stuff from market
                {
                    Item_Slot_Base itemscript = TownStorageManager.currentlySelectedInventorySlot.GetComponent<Item_Slot_Base>();
                    bool added = TownStorageManager.AddItem(itemscript.LocalMarketItemData.itemID, int.Parse(currentQtySelected.text), CampType.LocalMarket); //If selling from storage code
                    if (!added)
                    {
                        Debug.LogWarning("Failed to add equipped bait due to full inventory.");
                        // Optionally: trigger a UI message, fallback storage, etc.
                    }
                    DataGameManager.instance.PlayerGold -= int.Parse(finalPrice.text);
                    DataGameManager.instance.topPanelManager.UpdateGold();
                    DataGameManager.instance.topPanelManager.AddedRemovedGoldAnim(false, int.Parse(finalPrice.text));

                    UpdateUI();
                    Debug.Log("updatedGold");
                }
                else
                {

                }
            }
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
            itemType.text = data.ItemType.ToString();
            itemMaxQty.text = script.itemDataBasic.Quantity.ToString();
            singleCostText.text = "Single sell cost";
            buySellText.text = "Sell";
            buySellText_2.text = "Sell";
            SellBuyButtonCanvasGroup.alpha = 1f;
            slider.maxValue = script.itemDataBasic.Quantity;
            slider.minValue = 1f;
            slider.value = (1 + float.Parse(itemMaxQty.text)) / 2;
        }

    public void SetupBanner(Sprite texture)
    {
        //Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
     //   BG_Banner.sprite = texture;
       // BG_Banner.gameObject.SetActive(true);
    }

    public void SetupUpUI_Market(string itemID)
    {
       
        parentBox.SetActive(true);
        DataGameManager.instance.itemData_Array.TryGetValue(itemID, out var data);
        Item_Slot_Base script = TownStorageManager.currentlySelectedInventorySlot.GetComponent<Item_Slot_Base>();
        currentItemSelected = data;
        itemName.text = data.ItemName;
        itemDescription.text = data.Description;
        itemImage.sprite = data.ItemImage;
        itemType.text = data.ItemType.ToString();
        singleCostText.text = "Single buy cost";
        buySellText.text = "Buy";
        buySellText_2.text = "Buy";
        DataGameManager.instance.localMarket_Items_List.TryGetValue(itemID, out var localmarketItemData);
        itemSinglePrice.text = localmarketItemData.itemSellPrice.ToString();

        int itemMaxpurchaseqty = DataGameManager.instance.PlayerGold / localmarketItemData.itemSellPrice;

        if (DataGameManager.instance.PlayerGold >= int.Parse(finalPrice.text))
        {
            SellBuyButtonCanvasGroup.alpha = 1f;
        }
        else
        {

            SellBuyButtonCanvasGroup.alpha = 0.5f;

        }

        itemMaxpurchaseqty = Mathf.Max(1, itemMaxpurchaseqty);
        itemMaxQty.text = itemMaxpurchaseqty.ToString();
        slider.maxValue = itemMaxpurchaseqty;

        slider.minValue = 1f;
        slider.value = (1 + itemMaxpurchaseqty) / 2f;
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

            if (!script.isLocalMarketSlot) 
            {
                if (script.itemDataBasic.Quantity == 0)
                {
                    SetasNothingSelected();
                }
                else //First area for inventory
                {
                    itemMaxQty.text = script.itemDataBasic.Quantity.ToString();
                    slider.maxValue = script.itemDataBasic.Quantity; //setup slider and the values.
                    slider.minValue = 1f;
                    slider.value = Mathf.Clamp(slider.value, 1f, slider.maxValue = script.itemDataBasic.Quantity);
                    currentQtySelected.text = slider.value.ToString();
                    finalPrice.text = ((int)slider.value * int.Parse(itemSinglePrice.text)).ToString();

                }
                SellBuyButtonCanvasGroup.alpha = 1f;

            }
            else //This is for local market
            {
                // Calculate max stacks the player can afford
                int maxStacks = DataGameManager.instance.PlayerGold / script.LocalMarketItemData.itemSellPrice;
                maxStacks = Mathf.Max(1, maxStacks); // Ensure it's at least 1

                slider.minValue = 1f;
                slider.maxValue = maxStacks;

                slider.value = Mathf.Clamp(slider.value, slider.minValue, slider.maxValue);

                int totalQuantity = (int)slider.value * script.LocalMarketItemData.itemStackSizeSold;

                itemMaxQty.text = (maxStacks * script.LocalMarketItemData.itemStackSizeSold).ToString();
                currentQtySelected.text = totalQuantity.ToString();
                finalPrice.text = ((int)slider.value * script.LocalMarketItemData.itemSellPrice).ToString();

                if (DataGameManager.instance.PlayerGold >= int.Parse(finalPrice.text))
                {
                    SellBuyButtonCanvasGroup.alpha = 1f;
                }
                else
                {
                    
                    SellBuyButtonCanvasGroup.alpha = 0.5f;
                    
                }



            }
        } 
    }
}
