using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.Rendering.DebugUI;

public class MiningCamp_Boost_Struc
{
    public CampBoost_Class RapidMining;
    public CampBoost_Class ExpandedVeins;
    public CampBoost_Class RuneweaversEssence;
    public CampBoost_Class ProspectorsInsight;

    public MiningCamp_Boost_Struc()
    {
        RapidMining = new CampBoost_Class
        {
            boostName = "Rapid Mining",
            boostDescription = "Reduces the base Action time by # seconds.",
            boostAmount = 0f,
            boostUnit = BoostUnit.Seconds,
            boostSprite = null // load later

        };

        ExpandedVeins = new CampBoost_Class
        {
            boostName = "Expanded Veins",
            boostDescription = "Increases vein ore size.",
            boostAmount = 0f,
            boostUnit = BoostUnit.Flat,
            boostSprite = null // load later
        };

        RuneweaversEssence = new CampBoost_Class
        {
            boostName = "Runeweaver's Essence",
            boostDescription = "Chance to acquire magical essence when mining",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };

        ProspectorsInsight = new CampBoost_Class
        {
            boostName = "Prospector's Insight",
            boostDescription = "Increases experience gained",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };   
    }

    public void InitializeSprites()
    {
        RapidMining.boostSprite = SpriteLoader.LoadBoostSprite("RapidMining");
        ExpandedVeins.boostSprite = SpriteLoader.LoadBoostSprite("ExpandedVeins");
        RuneweaversEssence.boostSprite = SpriteLoader.LoadBoostSprite("RuneweaversEssence");
        ProspectorsInsight.boostSprite = SpriteLoader.LoadBoostSprite("ProspectorsInsight");
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
        RapidMining,
        ExpandedVeins,
        RuneweaversEssence,
        ProspectorsInsight
    };
    }

}

