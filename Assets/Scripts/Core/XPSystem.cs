using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using static UnityEngine.EventSystems.EventTrigger;
using System;
using UnityEngine.InputSystem;


public class XPSystem : MonoBehaviour
{
    private Dictionary<int, int> levelXp = new Dictionary<int, int>();   

    void Awake()
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

    public static CampProgressBar campProgressBar;
    public static LevelUpNotification_Manager levelUpNotification;
    public static NewUnlocks_Notifcation_Manager newUnlocksNotifcation;




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

    public static CampXPData GetCampXP(CampType campType)
    {
        if (DataGameManager.instance.campXPDictionaries.TryGetValue(campType, out CampXPData xpData))
        {
            return xpData;
        }

        Debug.LogWarning($"XP data not found for camp type: {campType}");
        return null;
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

            var boosts = DataGameManager.instance.boostsManager.GetMergedBoosts(campType);
            float xpboost = GetBoostAmount(boosts, "Builder’s Insight", "Woodcutters Insight", "Angler's Insight", "Prospector's Insight", "Blacksmiths Insight");

            // Calculate total XP to add including boost percentage

            int bonusAdded = (int)(xpToAdd * (1f + xpboost / 100f));
            int bonusAmount = bonusAdded - xpToAdd;

            // Add the calculated XP to the current camp's XP
            campData.currentXP += bonusAdded;

            // Add the XP as a feed
            CampTypeData match = DataGameManager.instance.campTypeDataList.FirstOrDefault(c => c.campType == campType);



            DataGameManager.instance.item_XP_FeedManager.AddXPFeedSlot(xpToAdd.ToString(), bonusAmount.ToString(), match.campImage, match.campType);

            if (campType == DataGameManager.instance.currentActiveCamp)
            {
                campProgressBar.UpdateProgressBar(campType);
            }

            // Check if the camp can level up
            if (CanLevelUp(campType))
            {
                // Get the new level based on the XP
                int newLevel = GetLevelForXP(campData.currentXP);

                // If the new level is higher, update the level
                if (newLevel > campData.currentLevel)
                {
                    int oldLevel = campData.currentLevel;
                    campData.currentLevel = newLevel;
                    newUnlocksNotifcation.CheckForNewUnlocks(campType, newLevel, oldLevel);
                    DataGameManager.instance.campButtonUpdater.UpdateCampButtonLevel(campType); //update the side button xp aswell!
                    if (!DataGameManager.instance.TurnOffDialog)
                    {
                        CheckForNewQuests(campType, newLevel);
                    }
                   

                    levelUpNotification.LevelUpNotificationSetup(campType);

                    if (campType == DataGameManager.instance.currentActiveCamp) //only refresh if we are on that camp panel.
                    {
                        var campDataDict = DataGameManager.instance.GetCampData(campType); //update the camps visuals to reflect unlocked camps!
                        DataGameManager.instance.populate_Camp_Slots.PopulateSlots(campDataDict);
                        campProgressBar.UpdateProgressBar(campType);
                    }

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


    static float GetBoostAmount(List<CampBoost_Class> boosts, params string[] boostNames)
    {
        return boosts
            .Where(b => boostNames.Contains(b.boostName))
            .Sum(b => b.boostAmount);
    }

    public static void CheckForNewQuests(CampType campType, int level)
    {
        switch (campType)
        {
            case CampType.LumberCamp:
                if (level == 3)
                {
                    TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("ExplainCompletedObjectivesButton_V2");
                    DataGameManager.instance.tutorialManager.StartTutorialImmediately(tutorialGroupData);
                }

                if (level == 5)
                {
                    TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("TierSystemTutorial");
                    DataGameManager.instance.tutorialManager.StartTutorialImmediately(tutorialGroupData);
                }
                break;

            case CampType.ConstructionCamp:
                if (level == 2)
                {
                    TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("JustBuiltSawMill");
                    DataGameManager.instance.tutorialManager.StartTutorialImmediately(tutorialGroupData);
                }

                if (level == 6)
                {
                    TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("JustCollectedMaplePlanksAndBeams");
                    DataGameManager.instance.tutorialManager.StartTutorialImmediately(tutorialGroupData);
                }
                

                if (level == 19)
                {
                    TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("SimpleCabinGuide");
                    DataGameManager.instance.tutorialManager.StartTutorialImmediately(tutorialGroupData);
                }
                break;
        }

    }
}
