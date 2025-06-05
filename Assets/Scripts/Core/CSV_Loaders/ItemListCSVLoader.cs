using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;



public class ItemListCSVLoader : MonoBehaviour
{
    public TextAsset csvFile;

    private Dictionary<string, ItemData_Struc> itemData_Array;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadCSV();

     //   PrintTest();

        DataGameManager.instance.itemData_Array = itemData_Array;
    }

    void LoadCSV()
    {
        itemData_Array = new Dictionary<string, ItemData_Struc>();
        string[] lines = csvFile.text.Split('\n');

        // Start from the second line to skip the header row
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Split the CSV line by commas, but handle the special case for the Produced Items field
            string[] fields = SplitCsvLine(line);

            if (fields.Length < 13) // Skip malformed rows

            {
                Debug.LogWarning("Invalid or incomplete row: ");
                continue;
            }

            // Skip the first column (index number)
            string itemID = TryGetString(fields, 1);
            string itemName = TryGetString(fields, 2);
            string description = TryGetString(fields, 3);
            string itemImage = TryGetString(fields, 4);
            ItemType itemType = TryGetEnum<ItemType>(fields, 6);
            string itemCategory = TryGetString(fields, 5);
            string itemReplaces = TryGetString(fields, 7);
            int storageSpace = TryGetInt(fields, 8);
            int restoreHealthAmount = TryGetInt(fields, 9);
            int fuelAmount = TryGetInt(fields, 10);
            int itemSellPrice = TryGetInt(fields, 11);
            int storedQty = TryGetInt(fields, 12);
            int maxstack = TryGetInt(fields, 13);

            // Load images from the entire Resources folder
            Sprite image2D = LoadImageFromResources(itemImage);

            // Create IdleSlot with the complete data
            ItemData_Struc slot = new ItemData_Struc(itemID, itemName, description, image2D, itemType, itemCategory, itemReplaces, storageSpace, restoreHealthAmount, fuelAmount, itemSellPrice, storedQty, maxstack);

            if (SafeAddItem(itemData_Array, slot))
            {       
                itemData_Array.Add(itemID, slot);
            }
            else
            {
          
            }
  
        }
    }

    public TEnum TryGetEnum<TEnum>(string[] fields, int index, TEnum fallback = default) where TEnum : struct
    {
        if (index >= 0 && index < fields.Length)
        {
           
            string value = fields[index].Trim();
           
            if (Enum.TryParse<TEnum>(value, out var result))
            {
                return result;
            }
        }

        return fallback;
    }
    string[] SplitCsvLine(string line) // A method to split the CSV line while keeping the Produced Items field intact
    {
        List<string> fields = new List<string>();
        bool insideQuotes = false;
        StringBuilder currentField = new StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"' && (i == 0 || line[i - 1] != '\\'))
            {
                insideQuotes = !insideQuotes; // Toggle insideQuotes when encountering quotes
                continue;
            }

            if (c == ',' && !insideQuotes)
            {
                fields.Add(currentField.ToString().Trim());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }

        if (currentField.Length > 0)
        {
            fields.Add(currentField.ToString().Trim());
        }

        return fields.ToArray();
    }

    private string TryGetString(string[] fields, int index)
    {
        return (fields.Length > index) ? fields[index].Trim('"') : "";
    }

    private int TryGetInt(string[] fields, int index)
    {
        if (fields.Length > index && int.TryParse(fields[index], out int result))
            return result;
        return 0; // Default value if parsing fails
    }

    Sprite LoadImageFromResources(string imageName)
    {
        // Load all sprites from the entire Resources folder
        Sprite[] allSprites = Resources.LoadAll<Sprite>("");

        foreach (Sprite sprite in allSprites)
        {
            if (sprite.name == imageName)
            {
                return sprite; // Return the matching image
            }
        }
        return null; // Return null if no match is found
    }

    void PrintTest()
    {       
        
            foreach (var slot in itemData_Array)
            {
                Debug.Log($"--- Item: {slot.Key} ---");
                Debug.Log($"Name: {slot.Value.ItemName}, Description: {slot.Value.Description}, Item Image: {slot.Value.ItemImage}, " +
                    $"Category: {slot.Value.ItemCategory}, Replaces: {slot.Value.ItemReplaces}, Storage Space: {slot.Value.StorageSpace}, " +
                    $"Restore Health: {slot.Value.RestoreHealthAmount}, Fuel Amount: {slot.Value.FuelAmount}, Sell Price: {slot.Value.ItemSellPrice}, " +
                    $"Stored Qty: {slot.Value.StoredQty}, Max Stack: {slot.Value.MaxStack}, Image: {(slot.Value.ItemImage != null ? slot.Value.ItemImage.name : "None")}");
                Debug.Log("-------------------------");
            }
        

    }

    public static bool SafeAddItem(Dictionary<string, ItemData_Struc> itemDataDict, ItemData_Struc item)
    {
        if (string.IsNullOrEmpty(item.ItemID))
        {
            return false;
        }

        if (itemDataDict.ContainsKey(item.ItemID))
        { 
            return false;
        }

    
        return true;
    }
}
