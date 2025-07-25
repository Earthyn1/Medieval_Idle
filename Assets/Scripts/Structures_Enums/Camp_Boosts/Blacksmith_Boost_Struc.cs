using Mono.Cecil;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.Rendering.DebugUI;

public class Blacksmith_Boost_Struc : ICampBoostData
{
    public CampBoost_Class SwiftBlacksmithing;
    public CampBoost_Class MasteredCreation;
    public CampBoost_Class ResourceEfficiency;
    public CampBoost_Class BlacksmithsInsight;
    public int CurrentTier { get; set; }

    private int resource1Current;
    private int resource2Current;

    public int Resource1_Current => resource1Current;
    public int Resource2_Current => resource2Current;

    public Blacksmith_Boost_Struc()
    {
        SwiftBlacksmithing = new CampBoost_Class
        {
            boostName = "Swift Blacksmithing",
            boostDescription = "Reduces the base Action time by # seconds.",
            boostAmount = 0f,
            boostUnit = BoostUnit.Seconds,
            boostSprite = null // load later

        };

        MasteredCreation = new CampBoost_Class
        {
            boostName = "Mastered Creation",
            boostDescription = "Increased chance of high quality creation",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };

        ResourceEfficiency = new CampBoost_Class
        {
            boostName = "Resource Efficiency",
            boostDescription = "Chance to not use any resources.",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };

        BlacksmithsInsight = new CampBoost_Class
        {
            boostName = "Blacksmiths Insight",
            boostDescription = "Increases experience gained",
            boostAmount = 0f,
            boostUnit = BoostUnit.Percent,
            boostSprite = null // load later
        };   
    }

    public void InitializeSprites()
    {
        SwiftBlacksmithing.boostSprite = SpriteLoader.LoadBoostSprite("SwiftBlacksmithing");
        MasteredCreation.boostSprite = SpriteLoader.LoadBoostSprite("MasteredCreation");
        ResourceEfficiency.boostSprite = SpriteLoader.LoadBoostSprite("ResourceEfficiency");
        BlacksmithsInsight.boostSprite = SpriteLoader.LoadBoostSprite("BlacksmithsInsight");
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
        SwiftBlacksmithing,
        MasteredCreation,
        ResourceEfficiency,
        BlacksmithsInsight
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

