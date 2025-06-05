using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Camp_Boosts_Manager : MonoBehaviour
{
    public CampBoost_Slot CampBoost_Slot_1;
    public CampBoost_Slot CampBoost_Slot_2;
    public CampBoost_Slot CampBoost_Slot_3;
    public CampBoost_Slot CampBoost_Slot_4;
    private List<CampBoost_Slot> campBoostSlots;

    void Awake()
    {
        campBoostSlots = GetComponentsInChildren<CampBoost_Slot>().ToList();
    }

    private void Start()
    {
        DataGameManager.instance.boostsManager = this;
    }


    public void SetupCampBoosts(CampType campType)
    {
        var mergedBoosts = GetMergedBoosts(campType);

        for (int i = 0; i < campBoostSlots.Count && i < mergedBoosts.Count; i++)
        {
            campBoostSlots[i].SetBoost(mergedBoosts[i]);
        }
    }

    List<CampBoost_Class> GetMergedBoosts(CampType campType)
    {
        var baseBoosts = DataGameManager.instance.GetBaseBoostsForCamp(campType);
        var tempBoosts = CloneBoosts(baseBoosts);

        if (campType == CampType.FishingCamp)
            ApplyFishingBaitBoosts(tempBoosts);

        // You can add more boost sources here for other camp types...

        return tempBoosts;
    }

    void ApplyFishingBaitBoosts(List<CampBoost_Class> boosts)
    {
        string equippedID = DataGameManager.instance.currentFishingBaitEquipped?.item;
        if (!string.IsNullOrEmpty(equippedID) && DataGameManager.instance.fishingBait_Item_List.TryGetValue(equippedID, out var baitData))
        {
            foreach (var baitBoost in baitData.boosts)
            {

                var match = boosts.FirstOrDefault(b => b.boostName == baitBoost.boostName);
                if (match != null)
                {
                    match.boostAmount += baitBoost.boostAmount;    
                }
                else
                {
                  
                }
            }
        }
        else
        {
          
        }
    }


    private List<CampBoost_Class> CloneBoosts(List<CampBoost_Class> originalList)
    {
        return originalList.Select(b => new CampBoost_Class
        {
            boostName = b.boostName,
            boostDescription = b.boostDescription,
            boostAmount = b.boostAmount,
            boostUnit = b.boostUnit,
            boostSprite = b.boostSprite
        }).ToList();
    }





}
