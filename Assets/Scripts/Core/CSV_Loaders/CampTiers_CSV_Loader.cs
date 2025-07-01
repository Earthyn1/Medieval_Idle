using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using static FishingCamp_Boost_Struc;
using static UnityEngine.Rendering.DebugUI;

public class CampTiers_CSV_Loader : MonoBehaviour
{

    public TextAsset ConstrucCampTier;
    public TextAsset LumberCampTier;
    public TextAsset BlacksmithTier;
    public TextAsset FishingCampTier;
    public TextAsset MiningCampTier;


    public Dictionary<string, CampTiersArray> constructionCamp_Tiers;
    public Dictionary<string, CampTiersArray> lumberCamp_Tiers;
    public Dictionary<string, CampTiersArray> blacksmith_Tiers;
    public Dictionary<string, CampTiersArray> miningCamp_Tiers;
    public Dictionary<string, CampTiersArray> fishingCamp_Tiers;



    void Start()
    {

        constructionCamp_Tiers = LoadCSV(ConstrucCampTier);  
        DataGameManager.instance.constructionCamp_Tiers = constructionCamp_Tiers;

        lumberCamp_Tiers = LoadCSV(LumberCampTier);
        DataGameManager.instance.lumberCamp_Tiers = lumberCamp_Tiers;

        miningCamp_Tiers = LoadCSV(MiningCampTier);
        DataGameManager.instance.miningCamp_Tiers = miningCamp_Tiers;

        fishingCamp_Tiers = LoadCSV(FishingCampTier);
        DataGameManager.instance.fishingCamp_Tiers = fishingCamp_Tiers;

        blacksmith_Tiers = LoadCSV(BlacksmithTier);
        DataGameManager.instance.blacksmith_Tiers = blacksmith_Tiers;

        DataGameManager.instance.InitializeCampTiers();


       


        foreach (var kvp in constructionCamp_Tiers)
        {
            string tierKey = kvp.Key;
            CampTiersArray tier = kvp.Value;

            string res1 = tier.resource1 != null ? $"{tier.resource1.item} // {tier.resource1.qty}" : "None";
            string res2 = tier.resource2 != null ? $"{tier.resource2.item} // {tier.resource2.qty}" : "None";

           // Debug.Log($"Tier Key: {tierKey}, Boost1: {tier.boost_1.boostAmount}, Boost1Type: {tier.boost_1.boostUnit} GoldCost: {tier.goldCost}, " +
           //           $"Required Resource 1: {res1}, Required Resource 2: {res2}");
        }

    }


    Dictionary<string, CampTiersArray> LoadCSV(TextAsset csv)
    {
        Dictionary<string, CampTiersArray> campTiersList = new Dictionary<string, CampTiersArray>();

        string[] lines = csv.text.Split('\n');

        // Start from the second line to skip the header row
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] fields = SplitCsvLine(line);

            if (fields.Length < 8)
            {
                Debug.LogWarning("Invalid or incomplete row: ");
                continue;
            }

            int tierLevel = i - 1;

            string unitString = TryGetString(fields,3);
            BoostUnit parsedUnit = BoostUnit.Seconds; // default fallback
            if (!Enum.TryParse(unitString, true, out parsedUnit))
            {
                Debug.LogWarning($"Invalid BoostUnit in CSV: '{unitString}', defaulting to Seconds.");
            }


            CampBoost_Class boost1 = new CampBoost_Class
            {
                boostName = "1",
                boostDescription = "",
                boostAmount = TryGetFloat(fields, 2),             
                boostUnit = parsedUnit,
                boostSprite = null 
            };

            unitString = TryGetString(fields, 5);
            parsedUnit = BoostUnit.Seconds; // default fallback
            if (!Enum.TryParse(unitString, true, out parsedUnit))
            {
                Debug.LogWarning($"Invalid BoostUnit in CSV: '{unitString}', defaulting to Seconds.");
            }

            CampBoost_Class boost2 = new CampBoost_Class
            {
                boostName = "2",
                boostDescription = "",
                boostAmount = TryGetFloat(fields, 4),
                boostUnit = parsedUnit,
                boostSprite = null

            };

            unitString = TryGetString(fields, 7);
            parsedUnit = BoostUnit.Seconds; // default fallback
            if (!Enum.TryParse(unitString, true, out parsedUnit))
            {
                Debug.LogWarning($"Invalid BoostUnit in CSV: '{unitString}', defaulting to Seconds.");
            }
            CampBoost_Class boost3 = new CampBoost_Class
            {
                boostName = "3",
                boostDescription = "",
                boostAmount = TryGetFloat(fields, 6),
                boostUnit = parsedUnit,
                boostSprite = null

            };

            unitString = TryGetString(fields, 9);
            parsedUnit = BoostUnit.Seconds; // default fallback
            if (!Enum.TryParse(unitString, true, out parsedUnit))
            {
                Debug.LogWarning($"Invalid BoostUnit in CSV: '{unitString}', defaulting to Seconds.");
            }

            CampBoost_Class boost4 = new CampBoost_Class
            {
                boostName = "4",
                boostDescription = "",
                boostAmount = TryGetFloat(fields, 8),
                boostUnit = parsedUnit,
                boostSprite = null

            };

            int goldCost = TryGetInt(fields,10);

            // Extract optional fields
            string producedItemsString = fields.Length > 11 ? fields[11].Trim('"') : "";

            // Parse the produced items field
            List<SimpleItemData> requiredResources = ParseItemList(producedItemsString);

            SimpleItemData resource1 = requiredResources.Count > 0 ? requiredResources[0] : null;
            SimpleItemData resource2 = requiredResources.Count > 1 ? requiredResources[1] : null;


            CampTiersArray slot = new CampTiersArray(boost1, boost2, boost3, boost4, tierLevel, goldCost, resource1, resource2);

            TryAddItemToList(campTiersList, slot);
        }

        return campTiersList;
    }



    void TryAddItemToList(Dictionary<string, CampTiersArray> itemDict, CampTiersArray item)
    {
   

        itemDict.Add(item.tierLevel.ToString(), item);

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

    List<SimpleItemData> ParseItemList(string data)
    {
        List<SimpleItemData> items = new List<SimpleItemData>();

        if (string.IsNullOrWhiteSpace(data)) return items;

        // Strip outer double parentheses if they exist
        data = data.Trim();
        if (data.StartsWith("((") && data.EndsWith("))"))
        {
            data = data.Substring(2, data.Length - 4); // remove outer (( and ))
        }

        // Split by "),(" to separate resources
        string[] entries = data.Split(new string[] { "),(" }, System.StringSplitOptions.None);

        foreach (string entry in entries)
        {
            string cleaned = entry.Replace("(", "").Replace(")", "").Trim();
            string[] parts = cleaned.Split(',');

            string itemID = null;
            int amount = 0;

            foreach (string part in parts)
            {
                string trimmed = part.Trim();

                if (trimmed.StartsWith("Resource Name="))
                    itemID = trimmed.Replace("Resource Name=", "").Trim();

                if (trimmed.StartsWith("Amount Required=") && int.TryParse(trimmed.Replace("Amount Required=", "").Trim(), out int amt))
                    amount = amt;
            }

            if (!string.IsNullOrEmpty(itemID) && amount > 0)
            {
                // Assuming default 100% drop chance since it's not in this format
                items.Add(new SimpleItemData(itemID, amount, 1f));
            }
        }

        return items;
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

}
