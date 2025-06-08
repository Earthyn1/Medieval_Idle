using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CampSpecific_CSV_Loaders : MonoBehaviour
{
    
    private Dictionary<string, ConstructionCampModule> constructionCampModuleData;
    private Dictionary<string, VeinData> veinData;
    public TextAsset constructionmoduleCSV;
    public TextAsset miningmoduleCSV;



    void Start()
    {
        DataGameManager.instance.CampSpecific_CSV_Loaders = this;

        var constructionData = LoadConstructionModuleCSV(constructionmoduleCSV);
        DataGameManager.instance.constructionCampModuleData = constructionData;

        var miningData = LoadMiningModuleCSV(miningmoduleCSV);
        DataGameManager.instance.miningCampModuleData = miningData;


    }
    public Dictionary<string, ConstructionCampModule> LoadConstructionModuleCSV(TextAsset CsvLoaded)
    {
        constructionCampModuleData = new Dictionary<string, ConstructionCampModule>();
        string[] lines = CsvLoaded.text.Split('\n');

        // Start from the second line to skip the header row
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Split the CSV line by commas, but handle the special case for the Produced Items field
            string[] fields = SplitCsvLine(line);

            if (fields.Length < 16) // Skip malformed rows

            {
                Debug.LogWarning("Invalid or incomplete row: ");
                continue;
            }

            // Skip the first column (index number)
            string resourceName = TryGetString(fields, 1);
            int landDeed = TryGetInt(fields, 13);
            bool SingleUseSlot = TryGetBool(fields, 14);
            string BuildingIDUnlocked = TryGetString(fields, 15);
            string PreviousUpgradeRequired = TryGetString(fields, 16);

            ConstructionCampModule slot = new ConstructionCampModule(landDeed,SingleUseSlot,BuildingIDUnlocked,PreviousUpgradeRequired);

            constructionCampModuleData.Add(resourceName, slot);
          
        }
        return constructionCampModuleData;
    }

    public Dictionary<string, VeinData> LoadMiningModuleCSV(TextAsset CsvLoaded)
    {
        veinData = new Dictionary<string, VeinData>();
        string[] lines = CsvLoaded.text.Split('\n');

        // Start from the second line to skip the header row
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Split the CSV line by commas, but handle the special case for the Produced Items field
            string[] fields = SplitCsvLine(line);

            if (fields.Length < 16) // Skip malformed rows

            {
                Debug.LogWarning("Invalid or incomplete row: ");
                continue;
            }

            // Skip the first column (index number)
            string OreID = TryGetString(fields, 1);
            int remaining = TryGetInt(fields, 15);
            int initial = TryGetInt(fields, 13);
            float SearchDuration = TryGetFloat(fields, 14);
    
            VeinData slot = new VeinData(OreID, remaining, initial, SearchDuration);

            veinData.Add(OreID, slot);
        }

        return veinData;
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


    private bool TryGetBool(string[] fields, int index)
    {
        if (fields.Length > index && bool.TryParse(fields[index], out bool result))
            return result;
        return false; // Default value if parsing fails
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
}




