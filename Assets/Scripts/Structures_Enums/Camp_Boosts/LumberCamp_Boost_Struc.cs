using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.Rendering.DebugUI;

public class LumberCamp_Boost_Struc
{
    public CampBoost_Class RapidWoodcutting;
    public CampBoost_Class TwinHarvest;
    public CampBoost_Class FullYield;
    public CampBoost_Class WoodcuttersInsight;

    public LumberCamp_Boost_Struc()
    {
        RapidWoodcutting = new CampBoost_Class
        {
            boostName = "Rapid Woodcutting",
            boostDescription = "Reduces the base Action time by # seconds.",
            boostAmount = 0f,
            boostUnit = BoostUnit.Seconds,
            boostSprite = null // load later

        };

        TwinHarvest = new CampBoost_Class
        {
            boostName = "Twin Harvest",
            boostDescription = "Chance to double the resource you acquire.",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };

        FullYield = new CampBoost_Class
        {
            boostName = "Full Yield",
            boostDescription = "Chance to guarantee all tree drops.",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };

        WoodcuttersInsight = new CampBoost_Class
        {
            boostName = "Woodcutters Insight",
            boostDescription = "Increases experience gained",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };   
    }

    public void InitializeSprites()
    {
        RapidWoodcutting.boostSprite = SpriteLoader.LoadBoostSprite("RapidWoodcutting");
        TwinHarvest.boostSprite = SpriteLoader.LoadBoostSprite("TwinHarvest");
        FullYield.boostSprite = SpriteLoader.LoadBoostSprite("FullYield");
        WoodcuttersInsight.boostSprite = SpriteLoader.LoadBoostSprite("WoodcuttersInsight");
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
        RapidWoodcutting,
        TwinHarvest,
        FullYield,
        WoodcuttersInsight
    };
    }

}

