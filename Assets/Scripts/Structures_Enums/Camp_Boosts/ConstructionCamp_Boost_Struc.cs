using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.Rendering.DebugUI;

public class ConstructionCamp_Boost_Struc
{
    public CampBoost_Class SwiftConstruction;
    public CampBoost_Class ResourceConservation;
    public CampBoost_Class DoubleCraft;
    public CampBoost_Class BuildersInsight;

    public ConstructionCamp_Boost_Struc()
    {
        SwiftConstruction = new CampBoost_Class
        {
            boostName = "Swift Construction",
            boostDescription = "Reduces the base Action time by # seconds.",
            boostAmount = 0f,
            boostUnit = BoostUnit.Seconds,
            boostSprite = null // load later

        };

        ResourceConservation = new CampBoost_Class
        {
            boostName = "Resource Conservation",
            boostDescription = "Chance to not use any resources for this Action.",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };

        DoubleCraft = new CampBoost_Class
        {
            boostName = "Double Craft",
            boostDescription = "Chance to double the resource you acquire.",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };

        BuildersInsight = new CampBoost_Class
        {
            boostName = "Builder’s Insight",
            boostDescription = "Increases experience gained",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };   
    }

    public void InitializeSprites()
    {
        SwiftConstruction.boostSprite = SpriteLoader.LoadBoostSprite("SwiftConstruction");
        ResourceConservation.boostSprite = SpriteLoader.LoadBoostSprite("ResourceConservation");
        DoubleCraft.boostSprite = SpriteLoader.LoadBoostSprite("DoubleCraft");
        BuildersInsight.boostSprite = SpriteLoader.LoadBoostSprite("BuildersInsight");
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
        SwiftConstruction,
        ResourceConservation,
        DoubleCraft,
        BuildersInsight
    };
    }

}

