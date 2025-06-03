using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class LocalMarket_CSVLoader : MonoBehaviour
{
    public TextAsset csvFile;

    private Dictionary<string, LocalMarket_Items> localMarket_Items_List;

    void Start()
    {
        LoadCSV();

        DataGameManager.instance.localMarket_Items_List = localMarket_Items_List;

      
    }

    void LoadCSV()
    {
        localMarket_Items_List = new Dictionary<string, LocalMarket_Items>();
        string[] lines = csvFile.text.Split('\n');

        // Start from the second line to skip the header row
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Split the CSV line by commas, but handle the special case for the Produced Items field
            string[] fields = SplitCsvLine(line);

            if (fields.Length < 8) // Skip malformed rows

            {
                Debug.LogWarning("Invalid or incomplete row: ");
                continue;
            }

            // Skip the first column (index number)
            string itemID = TryGetString(fields, 1);
            int itemStackSizeSold = TryGetInt(fields, 6);
            int itemSellPrice = TryGetInt(fields, 5);
            bool itemLocked = TryGetBool(fields, 7);

            LocalMarket_Items slot = new LocalMarket_Items(itemID, itemSellPrice, itemStackSizeSold, itemLocked);

            TryAddItemToMarketList(localMarket_Items_List, slot);
        }
    }


    void TryAddItemToMarketList(Dictionary<string, LocalMarket_Items> itemDict, LocalMarket_Items item)
    {
        if (string.IsNullOrEmpty(item.itemID))
        {
            Debug.LogWarning("Item ID is null or empty. Skipping.");
            return;
        }

        if (itemDict.ContainsKey(item.itemID))
        {
            Debug.LogWarning($"Item {item.itemID} already exists in dictionary!");
            return;
        }

        itemDict.Add(item.itemID, item);
       
    }


    public void Print()
    {
        DataGameManager.instance.localMarket_Items_List = localMarket_Items_List;

        // Print each item in the dictionary
        foreach (var item in DataGameManager.instance.localMarket_Items_List)
        {
            Debug.Log($"[Market Item] ID: {item.Value.itemID}, Stack Size: {item.Value.itemStackSizeSold}, Sell Price: {item.Value.itemSellPrice}, Locked: {item.Value.itemLocked}");
        }

    }


    public static bool SafeAddItem(Dictionary<string, LocalMarket_Items> itemDataDict, LocalMarket_Items item)
    {
        if (string.IsNullOrEmpty(item.itemID))
        {
            return false;
        }

        if (itemDataDict.ContainsKey(item.itemID))
        {
            return false;
        }
        return true;
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

    private bool TryGetBool(string[] fields, int index)
    {
        bool result = false;
        if (fields.Length > index)
        {
            bool.TryParse(fields[index], out result);
        }
        return result;
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
}
