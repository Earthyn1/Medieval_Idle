using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI instance;

    public RectTransform panel;
    public Text tooltipText; // or UnityEngine.UI.Text
    public Text ItemQty;
    private Canvas canvas;

    void Awake()
    {
        instance = this;
        canvas = GetComponentInParent<Canvas>();
        Hide();
    }

    public void Show(string message, Vector2 position)
    {
        tooltipText.text = message;
        panel.gameObject.SetActive(true);

        // Convert world position to canvas space if needed
        Vector2 anchoredPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            position,
            canvas.worldCamera,
            out anchoredPos
        );
        panel.anchoredPosition = anchoredPos;
    }

    public void ShowTooltipBelow(RectTransform targetButton, string itemID)
    {
        ItemData_Struc itemData;
        if (DataGameManager.instance.itemData_Array.TryGetValue("SomeItemID", out itemData))
        {
            tooltipText.text = itemData.ItemName;
        }

        ItemQty.text = " (" + TownStorageManager.GetCurrentQuantity(itemID) + ")"; 

        panel.gameObject.SetActive(true);

        Vector3[] worldCorners = new Vector3[4];
        targetButton.GetWorldCorners(worldCorners);

        // Get bottom center of the button
        Vector3 bottomCenter = (worldCorners[0] + worldCorners[3]) / 2f;

        // Convert world space to canvas local position
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panel.parent as RectTransform,
            Camera.main.WorldToScreenPoint(bottomCenter),
            Camera.main,
            out anchoredPos
        );

        // Offset down
        anchoredPos.y -= 30f;

        panel.anchoredPosition = anchoredPos;
    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
    }
}
