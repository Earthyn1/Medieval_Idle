using UnityEngine;
using UnityEngine.UI;

public class Populate_Local_Market_Slots : MonoBehaviour
{
    public Transform parentContainer;
    public GameObject scrollWindow;
    public GameObject slotPrefab;  // Public reference to your slot prefab

    public void PopulateLocalMarketSlots()
    {
        DataGameManager.instance.populate_Local_Market_Slots.scrollWindow.SetActive(false);
        scrollWindow.SetActive(true);
        ScrollRect scrollRect = scrollWindow.GetComponentInParent<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 1f;  // Set scroll to top

        // Clear existing slots in the UI
        foreach (Transform child in DataGameManager.instance.populate_Camp_Slots.parentContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in parentContainer)
        {
            Destroy(child.gameObject);
        }
        int slotCount = 0;

        // Cycle through the pre-initialized storage list
        foreach (var item in DataGameManager.instance.localMarket_Items_List)
        {
            if (!item.Value.itemLocked)
            {
                GameObject newSlot = Instantiate(slotPrefab, parentContainer);
                Item_Slot_Base slotScript = newSlot.GetComponent<Item_Slot_Base>();

                if (!string.IsNullOrEmpty(item.Value.itemID)) // Check if the slot has a valid item
                {
                    newSlot.name = $"Slot_{item.Value.itemID}";
                    slotScript.LocalMarketItemData = item.Value;

                    slotScript.isLocalMarketSlot = true;
                    slotScript.SetupLocalMarketSlot();
                    slotCount++;
                }
                else
                {
                    newSlot.name = $"Slot_Empty_{slotCount}";
                    slotScript.SetAsEmpty();
                }
            }
        }
            


    }
}
