using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class FishingCamp_Boost_Struc
{
    public CampBoost_Class SwiftFishing;
    public CampBoost_Class CatchChance;
    public CampBoost_Class DoubleCatch;
    public CampBoost_Class AnglersInsight;

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
        SwiftFishing.boostSprite = SpriteLoader.LoadBoostSprite("SwiftFishing");
        CatchChance.boostSprite = SpriteLoader.LoadBoostSprite("CatchChance");
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

}

