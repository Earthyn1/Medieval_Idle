using System.Collections.Generic;
using UnityEngine;

public class CampActionData
{
    public string resourceName;
    public string description;
    public int populationCost;
    public int levelUnlocked;
    public int xpGiven;
    public float completeTime;
    public Sprite image2D;
    public Sprite bgImage;
    public CampType campType;
    public List<SimpleItemData> ProducedItems { get; set; }
    public List<SimpleItemData> RequiredItems { get; set; }

    public CampActionData(string resourceName, string description, int populationCost, int levelUnlocked, int xpGiven, float completeTime, Sprite image2D, Sprite bgImage, CampType campType, List<SimpleItemData> producedItems, List<SimpleItemData> requiredItems)
    {
        this.resourceName = resourceName;
        this.description = description;
        this.populationCost = populationCost;
        this.levelUnlocked = levelUnlocked;
        this.xpGiven = xpGiven;
        this.completeTime = completeTime;
        this.image2D = image2D;
        this.bgImage = bgImage;
        this.campType = campType;
        this.ProducedItems = producedItems;
        this.RequiredItems = requiredItems;
    }
}
