using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class Item_XP_FeedManager : MonoBehaviour
{
    public GameObject itemXpFeedSlotPrefab;  // Public reference to your slot prefab
    public GameObject itemFeedSlotPrefab;  // Public reference to your slot prefab

    private void Start()
    {
        DataGameManager.instance.item_XP_FeedManager = this;
    }

    public void AddXPFeedSlot(string xpAmount,Sprite CampImage, CampType campType)
    {
        GameObject newSlot = Instantiate(itemXpFeedSlotPrefab, gameObject.transform);
        Item_XP_Feed_Slot newSlot_Script = newSlot.GetComponent<Item_XP_Feed_Slot>();
        newSlot_Script.CampType = campType; //for now we just add, but later we should check to see if camptypexp already in feed
        newSlot_Script.SetupXP(xpAmount, CampImage);
    }

    public void AddItemFeedSlot(string itemID, int amount, CampType campType) 
    {
        if(gameObject.transform.childCount == 0)
        {
            GameObject newSlot = Instantiate(itemFeedSlotPrefab, gameObject.transform);
            Item_Feed_Slot newSlotScript = newSlot.gameObject.GetComponent<Item_Feed_Slot>();

            ItemData_Struc foundItem = DataGameManager.instance.itemData_Array.TryGetValue(itemID, out var tempItem) ? tempItem : null;
            newSlotScript.item_feed_SlotText.text = foundItem.ItemName;
            newSlotScript.itemAmount_feed_SlotText.text = amount.ToString();
            newSlotScript.item_feed_SlotImage.sprite = foundItem.ItemImage;

            int currentQty = TownStorageManager.GetCurrentQuantity(itemID);
            newSlotScript.itemAmountInStorageText.text = "(" + currentQty.ToString() + ")";
        }

        bool foundmatch = false;

        foreach (Transform child in gameObject.transform)
        {
            Item_XP_Feed_Slot childScript = child.GetComponent<Item_XP_Feed_Slot>();

            if(childScript != null)
            {
                if (childScript.CampType == campType)
                {
                    foundmatch = true;
                    GameObject newSlot = Instantiate(itemFeedSlotPrefab, childScript.item_Panel_Parent.transform);
                    Item_Feed_Slot newSlotScript = newSlot.gameObject.GetComponent<Item_Feed_Slot>();

                    ItemData_Struc foundItem = DataGameManager.instance.itemData_Array.TryGetValue(itemID, out var tempItem) ? tempItem : null;
                    newSlotScript.item_feed_SlotText.text = foundItem.ItemName;
                    newSlotScript.itemAmount_feed_SlotText.text = amount.ToString();
                    newSlotScript.item_feed_SlotImage.sprite = foundItem.ItemImage;

                    int currentQty = TownStorageManager.GetCurrentQuantity(itemID);
                    newSlotScript.itemAmountInStorageText.text = "(" + currentQty.ToString() + ")";
                }
            }
            
        }

        if (!foundmatch)
        {
            GameObject newSlot = Instantiate(itemFeedSlotPrefab, gameObject.transform);
            Item_Feed_Slot newSlotScript = newSlot.gameObject.GetComponent<Item_Feed_Slot>();

            ItemData_Struc foundItem = DataGameManager.instance.itemData_Array.TryGetValue(itemID, out var tempItem) ? tempItem : null;
            newSlotScript.item_feed_SlotText.text = foundItem.ItemName;
            newSlotScript.itemAmount_feed_SlotText.text = amount.ToString();
            newSlotScript.item_feed_SlotImage.sprite = foundItem.ItemImage;

            int currentQty = TownStorageManager.GetCurrentQuantity(itemID);
            newSlotScript.itemAmountInStorageText.text = "(" + currentQty.ToString() + ")";
        }
    }

  

}