using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.Rendering.DebugUI;

public class MiningCamp_Boost_Struc : ICampBoostData
{
    public CampBoost_Class RapidMining;
    public CampBoost_Class ExpandedVeins;
    public CampBoost_Class RuneweaversEssence;
    public CampBoost_Class ProspectorsInsight;
    public int CurrentTier { get; set; }

    private int resource1Current;
    private int resource2Current;

    public int Resource1_Current => resource1Current;
    public int Resource2_Current => resource2Current;
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

