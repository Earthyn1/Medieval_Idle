using UnityEngine;

public class ItemData_Struc
{
    public string ItemID;
    public string ItemName;
    public string Description;
    public Sprite ItemImage;
    public ItemType ItemType;
    public string ItemCategory;
    public string ItemReplaces;
    public int StorageSpace;
    public int RestoreHealthAmount;
    public int FuelAmount;
    public int ItemSellPrice;
    public int StoredQty;
    public int MaxStack;


    // Parameterless constructor (default values)
    public ItemData_Struc()
    {
        ItemID = "N/A";
        ItemName = "Unknown";
        Description = "No description";
        ItemImage = null;
        ItemType = ItemType.NA;
        ItemCategory = "Misc";
        ItemReplaces = "None";
        StorageSpace = 1;
        RestoreHealthAmount = 0;
        FuelAmount = 0;
        ItemSellPrice = 0;
        StoredQty = 0;
        MaxStack = 1;
    }


    public ItemData_Struc(
        string itemID, string itemName, string description, Sprite itemImage,
        ItemType itemType, string itemCategory, string itemReplaces, int storageSpace,
        int restoreHealthAmount, int fuelAmount, int itemSellPrice, int storedQty, int maxStack)

    {
        ItemID = itemID;
        ItemName = itemName;
        Description = description;
        ItemImage = itemImage;
        ItemType = itemType;
        ItemCategory = itemCategory;
        ItemReplaces = itemReplaces;
        StorageSpace = storageSpace;
        RestoreHealthAmount = restoreHealthAmount;
        FuelAmount = fuelAmount;
        ItemSellPrice = itemSellPrice;
        StoredQty = storedQty;
        MaxStack = maxStack;
    }

}
