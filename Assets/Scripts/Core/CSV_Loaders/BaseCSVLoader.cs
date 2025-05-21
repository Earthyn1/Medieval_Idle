using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;

public class BaseCVSLoader : MonoBehaviour
{   
    public TextAsset csvFile;
    // Dictionary to hold idle slots
    private Dictionary<string, CampActionData> campActionData;   
     
    public Dictionary<string, CampActionData> LoadCSV(TextAsset CsvLoaded)

    {
        campActionData = new Dictionary<string, CampActionData>();
        string[] lines = CsvLoaded.text.Split('\n');

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
            string resourceName = TryGetString(fields, 1);
            string description = TryGetString(fields, 2);
            int populationCost = TryGetInt(fields, 3);
            int levelUnlocked = TryGetInt(fields, 4);
            int xpGiven = TryGetInt(fields, 5);
            float completeTime = TryGetFloat(fields, 6);
            string image2DName = TryGetString(fields, 7);
            string bgImageName = TryGetString(fields, 8);
            CampCategorys campCategory = Enum.TryParse(TryGetString(fields, 9), out CampCategorys categoryResult) ? categoryResult : default;
            CampType campType = Enum.TryParse(TryGetString(fields, 12), out CampType typeResult) ? typeResult : default;

            // Extract optional fields
            string producedItemsString = fields.Length > 10 ? fields[10].Trim('"') : "";
            string requiredItemsString = fields.Length > 11 ? fields[11].Trim('"') : "";

            // Load images from the entire Resources folder

            Sprite image2D = LoadImageFromResources(image2DName);
            Sprite bgImage = LoadImageFromResources(bgImageName);
            

            // Parse the produced items field
            List<SimpleItemData> producedItems = ParseItemList(producedItemsString);

            // Parse the required items field
            List<SimpleItemData> requiredItems = ParseItemList(requiredItemsString);

            // Create IdleSlot with the complete data
            CampActionData slot = new CampActionData(resourceName, description, populationCost, levelUnlocked, xpGiven, completeTime, image2D, bgImage, campType, campCategory,  producedItems, requiredItems);

            // Add the slot to the dictionary
            campActionData.Add(resourceName, slot);
        }
        return campActionData;          
    }


    List<SimpleItemData> ParseItemList(string data)
    {        

        List<SimpleItemData> items = new List<SimpleItemData>();
        if (string.IsNullOrWhiteSpace(data)) return items;

        // Remove outer parentheses and split by '),('
        data = data.Trim('(', ')');
        string[] itemEntries = data.Split(new string[] { "),(" }, StringSplitOptions.None);

        foreach (string entry in itemEntries)
        {
            string[] parts = entry.Split(',');
            string item = GetValue(parts[0]);
            int quantity = TryParseInt(GetValue(parts[1]));
            float dropChance = TryParseFloat(GetValue(parts[2]));

            if (!string.IsNullOrEmpty(item) && item != "N/A")
            {
                items.Add(new SimpleItemData(item, quantity, dropChance));
            }
        }

        return items;
    }

    string GetValue(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";
        int index = input.IndexOf('=');
        if (index >= 0 && index < input.Length - 1)
        {
            return input.Substring(index + 1).Trim();
        }
        return "";
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

    private float TryGetFloat(string[] fields, int index)
    {
        if (fields.Length > index && float.TryParse(fields[index], out float result))
            return result;
        return 0.0f; // Default value if parsing fails
    }

    void PrintIdleSlots()
    {
        // Check if the dictionary has any slots
        if (campActionData == null || campActionData.Count == 0)
        {
            Debug.Log("No idle slots to display.");
            return;
        }

        // Loop through all IdleSlot entries and print them to the console
        foreach (var slot in campActionData)
        {
            Debug.Log($"--- Idle Slot: {slot.Key} ---");
            Debug.Log($"Resource Name: {slot.Value.resourceName}, Description: {slot.Value.description}, Population Cost: {slot.Value.populationCost}, " +
                 $"Level Unlocked: {slot.Value.levelUnlocked}, XP Given: {slot.Value.xpGiven}, Complete Time: {slot.Value.completeTime}, " +
                 $"2D Image: {(slot.Value.image2D != null ? slot.Value.image2D.name : "None")}, " +
                 $"BG Image: {(slot.Value.bgImage != null ? slot.Value.bgImage.name : "None")}");

            // Print produced items, if any
            if (slot.Value.ProducedItems != null && slot.Value.ProducedItems.Count > 0)
            {
                Debug.Log("  Produced Items:");
                foreach (var item in slot.Value.ProducedItems)
                {
                    string producedItemInfo = $"    - Item: {item.item}, Quantity: {item.qty}, Drop Chance: {item.dropChance}%";
                    Debug.Log(producedItemInfo);
                }
            }
            else
            {
                Debug.Log("  - No produced items.");
            }

            // Print required items, if any
            if (slot.Value.RequiredItems != null && slot.Value.RequiredItems.Count > 0)
            {
                Debug.Log("  Required Items:");
                foreach (var item in slot.Value.RequiredItems)
                {
                    string requiredItemInfo = $"    - Item: {item.item}, Quantity: {item.qty}, Drop Chance: {item.dropChance}%";
                    Debug.Log(requiredItemInfo);
                }
            }
            else
            {
                Debug.Log("  - No required items.");
            }

            Debug.Log("-------------------------");
        }
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

    int TryParseInt(string value)
    {
        int result = 0;
        if (!int.TryParse(value.Trim('"'), out result)) // Remove any quotes and parse
        {
          //  Debug.LogWarning($"Failed to parse '{value}' as int. Defaulting to 0.");
        }

        
        return result;
    }

    float TryParseFloat(string value)
    {
        float result = 0;
        if (!float.TryParse(value.Trim('"'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result))
        {
          //  Debug.LogWarning($"Failed to parse '{value}' as float. Defaulting to 0.");
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

