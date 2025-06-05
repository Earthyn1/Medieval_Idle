using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using static FishingCamp_Boost_Struc;
using static UnityEngine.Rendering.DebugUI;

public class FishingBait_CSV_Loader : MonoBehaviour
{
    public TextAsset csvFile;

    private Dictionary<string, FishingBait_Item_Struc> FishingBait_Items_List;

    void Start()
    {
        LoadCSV();
        InitializeFishingBaitBoostSprites();  // <-- Initialize sprites here
        DataGameManager.instance.fishingBait_Item_List = FishingBait_Items_List;

       

       // Print();
    }

    void LoadCSV()
    {
        FishingBait_Items_List = new Dictionary<string, FishingBait_Item_Struc>();
        string[] lines = csvFile.text.Split('\n');

        // Start from the second line to skip the header row
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Split the CSV line by commas, but handle the special case for the Produced Items field
            string[] fields = SplitCsvLine(line);

            if (fields.Length < 3) // Skip malformed rows

            {
                Debug.LogWarning("Invalid or incomplete row: ");
                continue;
            }

            // Skip the first column (index number)
            string itemID = TryGetString(fields, 1);
            float swiftFishing = TryGetFloat(fields, 2);
            int catchChance = TryGetInt(fields, 3);
            int doubleCatch = TryGetInt(fields, 4);

            FishingBait_Item_Struc slot = new FishingBait_Item_Struc(itemID, swiftFishing, catchChance, doubleCatch);



            TryAddItemToBaitList(FishingBait_Items_List, slot);
        }
    }


    void TryAddItemToBaitList(Dictionary<string, FishingBait_Item_Struc> itemDict, FishingBait_Item_Struc item)
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

    void InitializeFishingBaitBoostSprites()
    {
        foreach (var baitItem in FishingBait_Items_List.Values)
        {
            foreach (var boost in baitItem.boosts)
            {
                switch (boost.boostName)
                {
                    
                    case "Swift Fishing":

                        boost.boostSprite = SpriteLoader.LoadBoostSprite("SwiftFishing");
                        break;
                    case "Catch Chance":
                        boost.boostSprite = SpriteLoader.LoadBoostSprite("CatchChance");
                        break;
                    case "Double Catch":
                        boost.boostSprite = SpriteLoader.LoadBoostSprite("DoubleCatch");
                        break;
                    case "Anglers Insight":
                        boost.boostSprite = SpriteLoader.LoadBoostSprite("AnglersInsight");
                        break;
                    default:
                        boost.boostSprite = SpriteLoader.LoadBoostSprite("DefaultBoost"); // Optional fallback
                        Debug.LogWarning($"Unknown boost name: {boost.boostName}");
                        break;
                }
            }
        }
    }




    public Sprite LoadBoostSprite(string spriteName)
    {
        // Assuming all boost sprites are in Resources/Images/Icons/CampBoosts/
        Sprite sprite = Resources.Load<Sprite>($"Images/Icons/CampBoosts/{spriteName}");
        if (sprite == null)
        {
            Debug.LogWarning($"Boost sprite not found: {spriteName}");
        }
        return sprite;  // return the loaded sprite (or null)
    }


    void Print()
    {
        foreach (var kvp in FishingBait_Items_List)
        {
            Debug.Log($"Bait ID: {kvp.Key}");
            foreach (var boost in kvp.Value.boosts)
            {
                Debug.Log($"  - {boost.boostName}: {boost.GetFormattedAmount()}");
            }
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


    private float TryGetFloat(string[] fields, int index)
    {
        if (fields.Length > index && float.TryParse(fields[index], out float result))
            return result;
        return 0f; // Default value if parsing fails
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
