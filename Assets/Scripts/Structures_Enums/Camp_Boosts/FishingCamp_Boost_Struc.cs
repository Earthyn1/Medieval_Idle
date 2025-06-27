using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class FishingCamp_Boost_Struc : ICampBoostData
{
    public CampBoost_Class SwiftFishing;
    public CampBoost_Class CatchChance;
    public CampBoost_Class DoubleCatch;
    public CampBoost_Class AnglersInsight;
    public int CurrentTier { get; set; }

    private int resource1Current;
    private int resource2Current;

    public int Resource1_Current => resource1Current;
    public int Resource2_Current => resource2Current;

    public FishingCamp_Boost_Struc()
    {
        SwiftFishing = new CampBoost_Class
        {
            boostName = "Swift Fishing",
            boostDescription = "Reduces fishing time",
            boostAmount = 0f,
            boostUnit = BoostUnit.Seconds,
            boostSprite = null // load later

        };

        CatchChance = new CampBoost_Class
        {
            boostName = "Catch Chance",
            boostDescription = "Increases chance to catch fish",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };

        DoubleCatch = new CampBoost_Class
        {
            boostName = "Double Catch",
            boostDescription = "Chance to catch two fish instead of one",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };

        AnglersInsight = new CampBoost_Class
        {
            boostName = "Angler's Insight",
            boostDescription = "Increases experience gained",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };   
    }

    public void InitializeSprites()
    {
        SwiftFishing.boostSprite = SpriteLoader.LoadBoostSprite("CatchChance");
        CatchChance.boostSprite = SpriteLoader.LoadBoostSprite("SwiftFishing");
        DoubleCatch.boostSprite = SpriteLoader.LoadBoostSprite("DoubleCatch");
        AnglersInsight.boostSprite = SpriteLoader.LoadBoostSprite("AnglersInsight");   
    }

    public static class SpriteLoader
    {
        public static Sprite LoadBoostSprite(string spriteName)
        {
            // Assuming all boost sprites are in Resources/Sprites/Boosts/
            Sprite sprite = Resources.Load<Sprite>($"Images/Icons/CampBoosts/{spriteName}");
            if (sprite == null)
            {
                Debug.LogWarning($"Boost sprite not found: {spriteName}");
            }
            return sprite;
        }
    }

    public List<CampBoost_Class> GetAllBoosts()
    {
        return new List<CampBoost_Class>
    {
        SwiftFishing,
        CatchChance,
        DoubleCatch,
        AnglersInsight
    };
    }
    public void AddResources(string itemId, int amount, CampTiersArray tierData)
    {
        if (tierData.resource1.item == itemId)
            resource1Current = Mathf.Min(resource1Current + amount, tierData.resource1.qty);
        if (tierData.resource2.item == itemId)
            resource2Current = Mathf.Min(resource2Current + amount, tierData.resource2.qty);
    }

    public bool IsResourceComplete(CampTiersArray tierData)
    {
        return resource1Current >= tierData.resource1.qty && resource2Current >= tierData.resource2.qty;
    }

    public void ResetResources()
    {
        resource1Current = 0;
        resource2Current = 0;
    }
}

