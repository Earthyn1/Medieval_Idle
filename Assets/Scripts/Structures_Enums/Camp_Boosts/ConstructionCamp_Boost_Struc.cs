using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.Rendering.DebugUI;

public class ConstructionCamp_Boost_Struc : ICampBoostData
{
    public CampBoost_Class SwiftConstruction;
    public CampBoost_Class ResourceConservation;
    public CampBoost_Class DoubleCraft;
    public CampBoost_Class BuildersInsight;
    public int CurrentTier { get; set; }

    private int resource1Current;
    private int resource2Current;

    public int Resource1_Current => resource1Current;
    public int Resource2_Current => resource2Current;


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

