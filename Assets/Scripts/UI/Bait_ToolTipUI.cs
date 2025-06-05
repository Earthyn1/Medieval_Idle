using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Bait_ToolTipUI : MonoBehaviour
{
    public static Bait_ToolTipUI instance;

    public RectTransform panel;
    private Canvas canvas;

    private List<CampBoost_Slot> campBoostSlots;

  
    void Awake()
    {
        instance = this;
        canvas = GetComponentInParent<Canvas>();
        Hide();
        campBoostSlots = GetComponentsInChildren<CampBoost_Slot>().ToList();
    }

    public void ShowTooltipBelow_Bait(RectTransform targetButton, string baitid)
    {

        if(DataGameManager.instance.fishingBait_Item_List.TryGetValue(baitid, out var baitData))
        {
            var baitBoosts = baitData.boosts;

            // Update each CampBoost_Slot with the bait boosts
            for (int i = 0; i < campBoostSlots.Count && i < baitBoosts.Count; i++)
            {
                campBoostSlots[i].SetBoost(baitBoosts[i]);
            }
        }
        else
        {

        }

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
        anchoredPos.y -= 60f;

        panel.anchoredPosition = anchoredPos;
    }


    public void Hide()
    {
        panel.gameObject.SetActive(false);
    }
}
