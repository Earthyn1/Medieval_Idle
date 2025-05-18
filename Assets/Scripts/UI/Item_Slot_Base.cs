using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;


public class Item_Slot_Base : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public float fadeDuration = 1.0f;
    public CanvasGroup ItemTextCanvasGroup;
    public StorageSlot itemDataBasic;
    public GameObject itemImageBox;
    public Image itemImage;
    public Text itemName;
    public Text itemQty;
    public Image boxOutline;

    private GameObject dragIcon;
    private Canvas canvas; // reference to UI canvas for proper dragging position
    private Transform originalParent;
    public Image itemIcon;               // UI icon image

    public bool isActive;

    private void Start()
    {
        ItemTextCanvasGroup.alpha = 0;  // Start fully transparent
    }

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }
    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;  // Ensure fully opaque at the end
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            yield return null;
        }
        canvasGroup.alpha = 0;  // Ensure fully invisible at the end
    }

    public void OnClick()
    {
        if (itemDataBasic.ItemID != null)
        {
            if (TownStorageManager.currentlySelectedInventorySlot != null) //Already has a ref
            {
                Item_Slot_Base script = TownStorageManager.currentlySelectedInventorySlot.GetComponent<Item_Slot_Base>();
                script.boxOutline.color = new Color(117f / 255f, 229f / 255f, 101f / 255f, 0f / 255f);
            }

            boxOutline.color = new Color(117f / 255f, 229f / 255f, 101f / 255f, 130f / 255f);
            TownStorageManager.currentlySelectedInventorySlot = gameObject; //set self as new ref
            TownStorageManager.storageSellManager.SetupUpUI(itemDataBasic.ItemID);
        }
        else
        {
            if (TownStorageManager.currentlySelectedInventorySlot != null)
            {
                Item_Slot_Base script = TownStorageManager.currentlySelectedInventorySlot.GetComponent<Item_Slot_Base>();
                script.boxOutline.color = new Color(117f / 255f, 229f / 255f, 101f / 255f, 0f / 255f);
                TownStorageManager.storageSellManager.SetasNothingSelected();
            }
        }

        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<UnityEngine.UI.Image>().color = new Color(142f, 142f, 142f, 0.4f);

        if (isActive)
        {
            StartCoroutine(FadeIn(ItemTextCanvasGroup)); //this fades in canvas
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<UnityEngine.UI.Image>().color = new Color(142f, 142f, 142f, 0.05f);

        if (isActive)
        {
            StartCoroutine(FadeOut(ItemTextCanvasGroup)); //this fades out canvas
        }
    }

    public void SetAsEmpty()
    {
        isActive = false;
        itemImageBox.SetActive(false);
        itemDataBasic = new StorageSlot();
    }

    public void SetupAsActive()
    {
        // Check if the ItemID is null or empty
        if (string.IsNullOrEmpty(itemDataBasic.ItemID))
        {
            Debug.LogWarning("SetupAsActive called with an empty or null ItemID.");
            SetAsEmpty();
            return;
        }

        if (DataGameManager.instance.itemData_Array.TryGetValue(itemDataBasic.ItemID, out ItemData_Struc item))
        {
            itemName.text = item.ItemName;
            itemImage.sprite = item.ItemImage;
            itemQty.text = itemDataBasic.Quantity.ToString();
            itemImageBox.SetActive(true);
            isActive = true;
        }
        else
        {
            Debug.LogWarning("Item not found for ID: " + itemDataBasic.ItemID);
            SetAsEmpty();
        }
    }

    public void UpdateQty()
    {
        itemQty.text = itemDataBasic.Quantity.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDataBasic.Quantity <= 0) return; // Nothing to drag

        originalParent = transform.parent;

        // Create drag icon
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform, false);
        dragIcon.transform.SetAsLastSibling();

        var img = dragIcon.AddComponent<Image>();
        img.sprite = itemImage.sprite;
        img.raycastTarget = false;  // so it doesn't block raycasts

        var rect = dragIcon.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(50, 50); // or match your icon size

        // Hide original icon while dragging (optional)
       // itemIcon.enabled = false;
       // itemQty.enabled = false;

        UpdateDragIconPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            UpdateDragIconPosition(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            Destroy(dragIcon);
            dragIcon = null;
        }
    }

    private void UpdateDragIconPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        dragIcon.transform.localPosition = localPoint;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var droppedSlot = eventData.pointerDrag?.GetComponent<Item_Slot_Base>();
        if (droppedSlot != null && droppedSlot != this)
        {
            // Swap or merge items
            Debug.Log($"Dropped {droppedSlot.itemDataBasic.ItemID} onto {itemDataBasic.ItemID}");
            Debug.Log($"Dropped {droppedSlot.itemDataBasic.Quantity} onto {itemDataBasic.Quantity}");

            if (droppedSlot.itemDataBasic.ItemID == itemDataBasic.ItemID)
            {
                // Merge if same item
                MergeItems(droppedSlot);
            }
            else
            {
                // Swap if different items
                SwapItems(droppedSlot);
            }
        }
    }

    private void MergeItems(Item_Slot_Base otherSlot)
    {
        int maxStack = DataGameManager.instance.itemData_Array[otherSlot.itemDataBasic.ItemID].MaxStack;
        int totalQuantity = this.itemDataBasic.Quantity + otherSlot.itemDataBasic.Quantity;

        if (totalQuantity <= maxStack)
        {
            // Combine entirely into one slot
            this.itemDataBasic.Quantity = totalQuantity;
            otherSlot.itemDataBasic.Quantity = 0;
            otherSlot.itemDataBasic.ItemID = null;
        }
        else
        {
            // Fill this slot and put the remainder in the other slot
            this.itemDataBasic.Quantity = maxStack;
            otherSlot.itemDataBasic.Quantity = totalQuantity - maxStack;
        }

        UpdateSlotData(this);
        UpdateSlotData(otherSlot);

        UpdateUI();
        otherSlot.UpdateUI();
    }
    private void SwapItems(Item_Slot_Base otherSlot)
    {
        // Swap item data between slots
        var temp = this.itemDataBasic;
        this.itemDataBasic = otherSlot.itemDataBasic;
        otherSlot.itemDataBasic = temp;

        // Update the storage list to reflect the swap
        int currentIndex = transform.GetSiblingIndex();
        int otherIndex = otherSlot.transform.GetSiblingIndex();

        // Swap items in the TownStorage_List
        var tempSlot = DataGameManager.instance.TownStorage_List[currentIndex];
        DataGameManager.instance.TownStorage_List[currentIndex] = DataGameManager.instance.TownStorage_List[otherIndex];
        DataGameManager.instance.TownStorage_List[otherIndex] = tempSlot;

        // Refresh all UI slots to reflect the changes
        TownStorageManager.RefreshAllSlotsUI();

        // Deselect the current slot
        if (TownStorageManager.currentlySelectedInventorySlot != null)
        {
            Item_Slot_Base script = TownStorageManager.currentlySelectedInventorySlot.GetComponent<Item_Slot_Base>();
            script.boxOutline.color = new Color(117f / 255f, 229f / 255f, 101f / 255f, 0f / 255f);
            TownStorageManager.currentlySelectedInventorySlot = null; //Update this for Set as empty later
            TownStorageManager.storageSellManager.itemName.text = "Nothing Selected";
        }

        // Update UI for both slots
        UpdateUI();
        otherSlot.UpdateUI();
    }

    private void UpdateSlotData(Item_Slot_Base slot)
    {
        int slotIndex = slot.transform.GetSiblingIndex();
        var dataSlot = DataGameManager.instance.TownStorage_List[slotIndex];

        // Update the data in the list to match the current slot
        dataSlot.ItemID = slot.itemDataBasic.ItemID;
        dataSlot.Quantity = slot.itemDataBasic.Quantity;

        DataGameManager.instance.TownStorage_List[slotIndex] = dataSlot;
    }
    public void UpdateUI()
    {
        if (itemDataBasic.Quantity > 0)
        {
            SetupAsActive();
        }
        else
        {
            SetAsEmpty();
        }
    }
}
