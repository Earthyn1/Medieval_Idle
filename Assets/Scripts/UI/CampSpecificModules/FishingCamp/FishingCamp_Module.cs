using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class FishingCamp_Module : MonoBehaviour , CampUISlotInterface, IPointerEnterHandler, IPointerExitHandler
{
    public string campid;
    public Image hookImage;



    private void Awake()
    {
        GameObject parentObject = transform.parent.parent.gameObject;
        Camp_Resource_Slot parentscript = parentObject.GetComponent<Camp_Resource_Slot>();
        campid = parentscript.slotkey;
    }


    public void OnUISlotLoad(string campid)
    {
    }

    public void OnUISlotUpdate(string campid)
    {
        CampActionData campData = DataGameManager.instance.GetCampActionData(CampType.FishingCamp, campid);


        if (campData == null)
        {
           // Debug.LogError($"CampActionData was null for campid: {campid} and campType: {CampType.FishingCamp}");
            return;
        }

        SimpleItemData item = campData.ProducedItems.First();

        float dropChanceBoost = 0f;
        var boosts = DataGameManager.instance.boostsManager.GetMergedBoosts(CampType.FishingCamp);
        var dropBoost = boosts.FirstOrDefault(b => b.boostName == "Catch Chance");
        if (dropBoost != null)
            dropChanceBoost = dropBoost.boostAmount; // we get the merged boosts for fishing camp

       

        if(item.dropChance + dropChanceBoost < 20)
        {
            hookImage.color = Color.red;
        }

        if (item.dropChance + dropChanceBoost >= 20 && item.dropChance + dropChanceBoost < 40)
        {
            hookImage.color = Color.yellow;
        }

        if (item.dropChance + dropChanceBoost >= 40 && item.dropChance + dropChanceBoost < 55)
        {
            hookImage.color = Color.white;
        }

        if (item.dropChance + dropChanceBoost >= 55)
        {
            hookImage.color = Color.green;
        }

    }

       
    public void OnPointerEnter(PointerEventData eventData)
    {
        CampActionData campData = DataGameManager.instance.GetCampActionData(CampType.FishingCamp, campid);

        SimpleItemData item = campData.ProducedItems.First();

        float dropChanceBoost = 0f;
        var boosts = DataGameManager.instance.boostsManager.GetMergedBoosts(CampType.FishingCamp);
        var dropBoost = boosts.FirstOrDefault(b => b.boostName == "Catch Chance");
        if (dropBoost != null)
            dropChanceBoost = dropBoost.boostAmount; // we get the merged boosts for fishing camp

        string text = "Drop chance = " + (item.dropChance + dropChanceBoost) + "%";

        TooltipUI.instance.ShowTooltipBelow_Name(hookImage.transform as RectTransform, text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.instance.Hide();
    }

}
