using System.Collections.Generic;
using UnityEngine;

public class XPSystem : MonoBehaviour
{
    private Dictionary<int, int> levelXp = new Dictionary<int, int>();   

    void Start()
    {
        int[] xpTable = {
            0, 83, 174, 276, 388, 512, 650, 801, 969, 1154,
            1358, 1584, 1833, 2107, 2411, 2746, 3115, 3523,
            3973, 4470, 5018, 5624, 6291, 7028, 7842, 8740,
            9730, 10824, 12031, 13363, 14833, 16456, 18247,
            20224, 22406, 24815, 27473, 30408, 33648, 37224,
            41171, 45529, 50339, 55649, 61512, 67983, 75127,
            83014, 91721, 101333, 111945, 123660, 136594,
            150872, 166636, 184040, 203254, 224466, 247886,
            273742, 302288, 333804, 368599, 407015, 449428,
            496254, 547953, 605032, 668051, 737627, 814445,
            899257, 992895, 1096278, 1210421, 1336443,
            1475581, 1629200, 1798808, 1986068, 2192818,
            2421087, 2673114, 2951373, 3258594, 3597792,
            3972294, 4385776, 4842295, 5346332, 5902831,
            6517253, 7195629, 7944614, 8771558, 9684577,
            10692629, 11805606, 13034431
        };

        for (int i = 0; i < xpTable.Length; i++)
        {
            levelXp.Add(i + 1, xpTable[i]);
        }

        DataGameManager.instance.levelXp = levelXp;
    }
}

public static class XPManager
{
    public static CampButtonUpdater campverticalLayout;
    public static CampProgressBar campProgressBar;




    public static int GetXPForLevel(int level)
    {
        return DataGameManager.instance.levelXp.ContainsKey(level) ? DataGameManager.instance.levelXp[level] : 0;
    }

    public static int GetLevelForXP(int xp)
    {
        int level = 1;
        foreach (var kvp in DataGameManager.instance.levelXp)
        {
            if (xp < kvp.Value) break;
            level = kvp.Key;
        }
        return level;
    }

    public static bool CanLevelUp(CampType campType)
    {
        // Get the camp's current XP and level from the campXPDictionaries
        int currentXP = DataGameManager.instance.campXPDictionaries[campType].currentXP;
        int currentLevel = DataGameManager.instance.campXPDictionaries[campType].currentLevel;

        // Check if the camp can level up by comparing current XP with XP needed for the next level
        int nextLevelXP = GetXPForLevel(currentLevel + 1);
        return currentXP >= nextLevelXP;
    }

    public static int XPToNextLevel(CampType campType)
    {
        // Get the camp's current XP and level from the campXPDictionaries
        int currentXP = DataGameManager.instance.campXPDictionaries[campType].currentXP;
        int currentLevel = DataGameManager.instance.campXPDictionaries[campType].currentLevel;

        // Get the XP required for the next level
        int nextLevelXP = GetXPForLevel(currentLevel + 1);

        // Return the XP needed to reach the next level (ensuring it doesn't return a negative value)
        return Mathf.Max(0, nextLevelXP - currentXP);
    }


    public static float GetLevelProgress(CampType campType)
    {
        // Check if the camp type exists in the campXP dictionary
        if (DataGameManager.instance.campXPDictionaries.ContainsKey(campType))
        {
            // Get the camp's XP data
            CampXPData campData = DataGameManager.instance.campXPDictionaries[campType];

            // Get the current level's XP and the next level's XP
            int currentLevelXP = GetXPForLevel(campData.currentLevel);
            int nextLevelXP = GetXPForLevel(campData.currentLevel + 1);

            // Calculate the progress
            int xpForLevel = nextLevelXP - currentLevelXP;
            int xpIntoLevel = campData.currentXP - currentLevelXP;

            // Return the progress as a percentage (between 0 and 1)
            return (float)xpIntoLevel / xpForLevel;
        }
        else
        {
            Debug.LogWarning($"CampType {campType} not found in campXP dictionary.");
            return 0f;  // Return 0 if the campType is not found
        }
    }


    public static bool AddXP(CampType campType, int xpToAdd)
    {
        // Check if the camp type exists in the campXP dictionary
        if (DataGameManager.instance.campXPDictionaries.ContainsKey(campType))
        {
            // Get the current camp's XP data
            CampXPData campData = DataGameManager.instance.campXPDictionaries[campType];

            // Add XP to the current camp's XP
            campData.currentXP += xpToAdd;

            if(campType == DataGameManager.instance.currentActiveCamp)
            {
                campProgressBar.UpdateProgressBar(campType);
            }

           


            // Check if the camp can level up d
            if (CanLevelUp(campType))
            {
                // Get the new level based on the XP
                int newLevel = GetLevelForXP(campData.currentXP);

                // If the new level is higher, update the level
                if (newLevel > campData.currentLevel)
                {
                    campData.currentLevel = newLevel;
                    campverticalLayout.UpdateCampButtonLevel(campType); //update the side button xp aswell!
                    campProgressBar.UpdateProgressBar(campType);

                    Debug.Log($"{campType} leveled up to {campData.currentLevel}!");
                    return true;  // Indicates that the camp leveled up
                }
            }
        }
        else
        {
            Debug.LogWarning($"CampType {campType} not found in campXP dictionary.");
        }

        return false;  // No level up
    }



}
