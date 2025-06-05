using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FishingBait_Item_Struc
{
    public string itemID;
    public List<CampBoost_Class> boosts;

    public FishingBait_Item_Struc(string itemID, float swiftFishing, int catchChance, int doubleCatch)
    {
        this.itemID = itemID;
        boosts = new List<CampBoost_Class>
        {
            new CampBoost_Class
            {
                boostName = "Swift Fishing",
                boostDescription = "Reduces fishing time",
                boostAmount = swiftFishing,
                boostUnit = BoostUnit.Seconds,
                boostSprite = null // load later
},
            new CampBoost_Class
            {
                boostName = "Catch Chance",
                boostDescription = "Increases chance to catch fish",
                boostAmount = catchChance / 100f, // If CSV is percent like 25, convert to 0.25
                boostUnit = BoostUnit.Percent,
                boostSprite = null // load later
            },
            new CampBoost_Class
            {
                boostName = "Double Catch",
                boostDescription = "Chance to catch two fish",
                boostAmount = doubleCatch / 100f,
                boostUnit = BoostUnit.Percent,
                boostSprite = null // load later
            }
        };
    }
}

