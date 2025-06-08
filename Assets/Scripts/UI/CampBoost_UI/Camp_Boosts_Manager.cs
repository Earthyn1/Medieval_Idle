using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Camp_Boosts_Manager : MonoBehaviour
{
    public CampBoost_Slot CampBoost_Slot_1;
    public CampBoost_Slot CampBoost_Slot_2;
    public CampBoost_Slot CampBoost_Slot_3;
    public CampBoost_Slot CampBoost_Slot_4;
    public List<CampBoost_Slot> campBoostSlots;

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

    public List<CampBoost_Class> GetMergedBoosts(CampType campType)
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
                   // Debug.Log("Bait boost add" + baitBoost.boostAmount);
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

    public void AddToBaseBoost(CampType campType, string boostName, float amountToAdd)
    {
        // Get reference to the actual base boost list (not a copy)
        List<CampBoost_Class> baseBoosts = campType switch
        {
            CampType.FishingCamp => DataGameManager.instance.FishingCamp_Boost.GetAllBoosts(),
            CampType.LumberCamp => DataGameManager.instance.LumberCamp_Boost.GetAllBoosts(),
            CampType.ConstructionCamp => DataGameManager.instance.ConstructionCamp_Boost.GetAllBoosts(),
            CampType.MiningCamp => DataGameManager.instance.MiningCamp_Boost.GetAllBoosts(),
            _ => null
        };

        if (baseBoosts == null)
        {
            Debug.LogWarning($"[Boosts] No base boost list found for camp type: {campType}");
            return;
        }

        // Find the boost by name
        var boost = baseBoosts.FirstOrDefault(b => b.boostName == boostName);
        if (boost != null)
        {
            boost.boostAmount += amountToAdd;
            Debug.Log($"[Boosts] Updated '{boostName}' boost for {campType}: +{amountToAdd}, New Value: {boost.boostAmount}");
        }
        else
        {
            Debug.LogWarning($"[Boosts] Boost named '{boostName}' not found in base boosts for {campType}");
        }
    }






}
