using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string saveFileName = "savefile.json";
    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveGame()
    {
        try
        {
            SaveData data = BuildSaveData();
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);

            // ✅ Also write a human-readable debug version
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "save_debug.json"), json);

            Debug.Log($"Game saved to {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game: {e}");
        }
    }

    public void DelayedLoadGame()
    {
        DataGameManager.instance.GameCanvas.SetActive(true);
       DataGameManager.instance.MainMenuCanvas.SetActive(false);
        StartCoroutine(DelayTheload());
    }

    public void DelayedNewGame()
    {
        DataGameManager.instance.GameCanvas.SetActive(true);
        DataGameManager.instance.MainMenuCanvas.SetActive(false);
        StartCoroutine(DelayTheNewGame());
    }

    public IEnumerator DelayTheload() //Delay load by 2 seconds to allow DataManager to run
    {
        yield return new WaitForSeconds(2f);
        LoadGame();
    }

    public IEnumerator DelayTheNewGame() //Delay newgame by 2 seconds to allow DataManager to run
    {
        yield return new WaitForSeconds(1f);
        TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("GameIntro_V2");
        DataGameManager.instance.tutorialManager.StartTutorialImmediately(tutorialGroupData);

        if (!DataGameManager.instance.TurnOffAutoSave)
        {
            StartAutoSaveLoop();
        }


    }
    public void LoadGame()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("No save file found. Starting new game.");
                return;
            }

            string json = File.ReadAllText(SavePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            ApplySaveData(data);
            Debug.Log("Game loaded.");

            
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e}");
        }


        foreach (var entry in DataGameManager.instance.campLockedDict) //Update Camp Left Buttons
        {
            if (entry.Key == CampType.NA || entry.Key == CampType.TownOverview || entry.Key == CampType.TownStorage)
                continue;

            if (!entry.Value)  // If not locked (i.e., unlocked)
            {

                DataGameManager.instance.campButtonUpdater.LoadCampsFromSave(entry.Key);
            }
        }

        DataGameManager.instance.topPanelManager.UpdateGold();
        DataGameManager.instance.topPanelManager.UpdateTownPopulation();

        if (!DataGameManager.instance.TurnOffAutoSave)
        {
            StartAutoSaveLoop();
        }

          


    }

    private SaveData BuildSaveData()
    {
        SaveData data = new SaveData
        {
            inventory = new List<ItemDataSave>(),
            activeCamps = new List<CampDataSave>(),
            flagIds = new List<TutorialFlagSave>(),
            campLockedList = new List<CampLockEntry>(),
            basicVariables = new BasicVariablesSave(),
            boosts = new CampBoosts_Save(),
            tiers = new CampTiers_Save(),
        };

        data.campLockedList = new List<CampLockEntry>();

        foreach (var townItem in DataGameManager.instance.TownStorage_List) //TownStorage
        {
            ItemDataSave simpleItem = new ItemDataSave
            {
                itemID = townItem.ItemID,
                quantity = townItem.Quantity
            };

            data.inventory.Add(simpleItem);
        }

        foreach (var activeCamps in DataGameManager.instance.activeCamps) //Add active camps
        {
            if (activeCamps.IsActive)
            {
                CampDataSave campDataSave = new CampDataSave
                {
                    campID = activeCamps.SlotKey,
                    type = activeCamps.CampType,
                    startedUnixTime = ((DateTimeOffset)activeCamps.StartTime).ToUnixTimeSeconds(),
                };

                data.activeCamps.Add(campDataSave);
            }
 
        }

        foreach (var allFlags in DataGameManager.instance.tutorialFlags) //Add flags
        {
            TutorialFlagSave flagsSave = new TutorialFlagSave
            {
                flagID = allFlags.Key,
                isCompleted = allFlags.Value
            };

            data.flagIds.Add(flagsSave);
        }

        foreach (var lockedcampslist in DataGameManager.instance.campLockedDict) //Add locked Camps
        {
            CampLockEntry lockedCampSave = new CampLockEntry
            {
                campType = lockedcampslist.Key,
                isLocked = lockedcampslist.Value,
            };

            data.campLockedList.Add(lockedCampSave);
        }

        data.OneSlotUseActions = DataGameManager.instance.OneSlotUseActions
    .Select(pair => new OneSlotUseActions { campID = pair.Key })
    .ToList();

        data.basicVariables.CurrentLandDeedsOwned = DataGameManager.instance.CurrentLandDeedsOwned; //Basic Vars!
        data.basicVariables.landDeedsBought = DataGameManager.instance.landDeedsbrought;
        data.basicVariables.MaxInventorySlots = DataGameManager.instance.MaxInventorySlots;
        data.basicVariables.PlayerGold = DataGameManager.instance.PlayerGold;
        data.basicVariables.MaxVillagerCapacity = DataGameManager.instance.MaxVillagerCapacity;
        data.basicVariables.CurrentVillagerCount = DataGameManager.instance.CurrentVillagerCount;
        data.basicVariables.maxBlacksmithFuel = DataGameManager.instance.maxBlacksmithFuel;
        data.basicVariables.currentBlacksmithFuel = DataGameManager.instance.currentBlacksmithFuel;


        if (DataGameManager.instance.currentFishingBaitEquipped != null) //Add fishing bait
        {
            data.basicVariables.fishingBait = new ItemDataSave
            {
                itemID = DataGameManager.instance.currentFishingBaitEquipped.item,
                quantity = DataGameManager.instance.currentFishingBaitEquipped.qty
            };
        }
        else
        {
            data.basicVariables.fishingBait = null;
        }

        foreach (var campXPData in DataGameManager.instance.campXPDictionaries) // Adds camp XP
        {
            CampXPEntry activeCampsSave = new CampXPEntry
            {
                campType = campXPData.Key,
                xpData = new CampXPDataSave
                {
                    currentXP = campXPData.Value.currentXP,
                    level = campXPData.Value.currentLevel
                }
            };
            data.basicVariables.campXPList.Add(activeCampsSave);
        }


        CampBoosts_Save campBoostsSave = new CampBoosts_Save(); //Get all the boost data!
        campBoostsSave.FishingCampBoost = ExtractBoosts(CampType.FishingCamp);
        campBoostsSave.LumberCampBoost = ExtractBoosts(CampType.LumberCamp);
        campBoostsSave.BlacksmithBoost = ExtractBoosts(CampType.Blacksmith);
        campBoostsSave.MiningCampBoost = ExtractBoosts(CampType.MiningCamp);
        campBoostsSave.ConstructionCampBoost = ExtractBoosts(CampType.ConstructionCamp);
        data.boosts = campBoostsSave;


        CampTiers_Save campTiersSaves = new CampTiers_Save
        {
            ConstructionCampTier = new TiersData_Save(),
            LumberCampTier = new TiersData_Save(),
            BlacksmithTier = new TiersData_Save(),
            FishingCampTier = new TiersData_Save(),
            MiningCampTier = new TiersData_Save(),
        };
        // Then overwrite:
        campTiersSaves.LumberCampTier = ExtractTierData(CampType.LumberCamp);
        campTiersSaves.BlacksmithTier = ExtractTierData(CampType.Blacksmith);
        campTiersSaves.ConstructionCampTier = ExtractTierData(CampType.ConstructionCamp);
        campTiersSaves.FishingCampTier = ExtractTierData(CampType.FishingCamp);
        campTiersSaves.MiningCampTier = ExtractTierData(CampType.MiningCamp);
        data.tiers = campTiersSaves;

        data.lastSaveTimeUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        return data;
    }


    private TiersData_Save ExtractTierData(CampType campType)
    {
        var boostData = DataGameManager.instance.GetBoostData(campType);
        if (boostData == null)
        {
            return new TiersData_Save { CurrentTier = -1, Resource1 = 0, Resource2 = 0 };
        }

        return new TiersData_Save
        {
            CurrentTier = boostData.CurrentTier,
            Resource1 = boostData.Resource1_Current,
            Resource2 = boostData.Resource2_Current
        };
    }


    private CampBoostData_Save ExtractBoosts(CampType campType)
    {
        var boostList = DataGameManager.instance.GetBaseBoostsForCamp(campType);

        return new CampBoostData_Save
        {
            boost1 = boostList.Count > 0 ? boostList[0].boostAmount : 0f,
            boost2 = boostList.Count > 1 ? boostList[1].boostAmount : 0f,
            boost3 = boostList.Count > 2 ? boostList[2].boostAmount : 0f,
            boost4 = boostList.Count > 3 ? boostList[3].boostAmount : 0f
        };
    }

    private void ApplyBoosts(CampBoostData_Save save, List<CampBoost_Class> targetBoosts)
    {
        if (targetBoosts.Count > 0) targetBoosts[0].boostAmount = save.boost1;
        if (targetBoosts.Count > 1) targetBoosts[1].boostAmount = save.boost2;
        if (targetBoosts.Count > 2) targetBoosts[2].boostAmount = save.boost3;
        if (targetBoosts.Count > 3) targetBoosts[3].boostAmount = save.boost4;


    }
    private void ApplySaveData(SaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("Save data is null. Cannot apply save.");
            return;
        }

        // 1. Load Inventory
        DataGameManager.instance.TownStorage_List.Clear();
        foreach (var item in data.inventory)
        {
            DataGameManager.instance.TownStorage_List.Add(new StorageSlot
            {
                ItemID = item.itemID,
                Quantity = item.quantity,
                IsTutorialSlot = false,

            });
        }

        // 2. Load Active Camps
        DataGameManager.instance.activeCamps.Clear();
        foreach (var camp in data.activeCamps)
        {
            Dictionary<string, CampActionData> campDataDict = DataGameManager.instance.GetCampData(camp.type);
            if (campDataDict != null && campDataDict.TryGetValue(camp.campID, out var campActionData))
            {
                DateTime restoredStartTime = DateTimeOffset.FromUnixTimeSeconds(camp.startedUnixTime).UtcDateTime;

                DataGameManager.instance.actionCampHandler.CreateEntryFromSave(camp.campID, camp.type, campActionData, null, DateTime.UtcNow);

            }        
        }

        // 3. Load Tutorial Flags
        DataGameManager.instance.tutorialFlags.Clear();
        foreach (var flag in data.flagIds)
        {
            DataGameManager.instance.tutorialFlags[flag.flagID] = flag.isCompleted;
        }

        // 4. Load Locked Camps
        DataGameManager.instance.campLockedDict.Clear();
        foreach (var entry in data.campLockedList)
        {
            DataGameManager.instance.campLockedDict[entry.campType] = entry.isLocked;
        }

        //4.5 adding the oneslotusecamps!
        DataGameManager.instance.OneSlotUseActions = data.OneSlotUseActions
      .ToDictionary(entry => entry.campID, entry => new OneSlotUseActions_Struc(entry.campID));

        // 5. Load Basic Variables
        var vars = data.basicVariables;
        DataGameManager.instance.CurrentLandDeedsOwned = vars.CurrentLandDeedsOwned;
        DataGameManager.instance.landDeedsbrought = vars.landDeedsBought;
        DataGameManager.instance.MaxInventorySlots = vars.MaxInventorySlots;
        DataGameManager.instance.PlayerGold = vars.PlayerGold;
        DataGameManager.instance.MaxVillagerCapacity = vars.MaxVillagerCapacity;
        DataGameManager.instance.CurrentVillagerCount = vars.CurrentVillagerCount;
        DataGameManager.instance.maxBlacksmithFuel = vars.maxBlacksmithFuel;
        DataGameManager.instance.currentBlacksmithFuel = vars.currentBlacksmithFuel;

        // Fishing Bait
        if (vars.fishingBait != null)
        {
            DataGameManager.instance.currentFishingBaitEquipped = new SimpleItemData(
                vars.fishingBait.itemID,
                vars.fishingBait.quantity,
                100f
            );
        }
        else
        {
            DataGameManager.instance.currentFishingBaitEquipped = null;
        }

        // 6. Load Camp XP
        DataGameManager.instance.campXPDictionaries.Clear();
        foreach (var entry in vars.campXPList)
        {
            DataGameManager.instance.campXPDictionaries[entry.campType] = new CampXPData
            {
                currentXP = (int)entry.xpData.currentXP, // truncate
                currentLevel = entry.xpData.level

            };
        }

        //This applies the boosts back hopefully
        ApplyBoosts(data.boosts.FishingCampBoost, DataGameManager.instance.FishingCamp_Boost.GetAllBoosts());
        ApplyBoosts(data.boosts.LumberCampBoost, DataGameManager.instance.LumberCamp_Boost.GetAllBoosts());
        ApplyBoosts(data.boosts.BlacksmithBoost, DataGameManager.instance.Blacksmith_Boost.GetAllBoosts());
        ApplyBoosts(data.boosts.MiningCampBoost, DataGameManager.instance.MiningCamp_Boost.GetAllBoosts());
        ApplyBoosts(data.boosts.ConstructionCampBoost, DataGameManager.instance.ConstructionCamp_Boost.GetAllBoosts());

        // 8. Load Tiers
        ApplyTiers(data.tiers.ConstructionCampTier, CampType.ConstructionCamp);
        ApplyTiers(data.tiers.LumberCampTier, CampType.LumberCamp);
        ApplyTiers(data.tiers.BlacksmithTier, CampType.Blacksmith);
        ApplyTiers(data.tiers.FishingCampTier, CampType.FishingCamp);
        ApplyTiers(data.tiers.MiningCampTier, CampType.MiningCamp);

    }

    private void ApplyTiers(TiersData_Save save, CampType type)
    {
        var boostData = DataGameManager.instance.GetBoostData(type);
        if (boostData == null) return;

        boostData.CurrentTier = save.CurrentTier;
    }

    private Coroutine autoSaveCoroutine;

    public void StartAutoSaveLoop()
    {
        if (autoSaveCoroutine == null)
            autoSaveCoroutine = StartCoroutine(AutoSaveLoop());
    }

    public void StopAutoSaveLoop()
    {
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
            autoSaveCoroutine = null;
        }
    }

    private IEnumerator AutoSaveLoop()
    {
        while (true)
        {
            if (!DataGameManager.instance.tutorialManager.isTutorialActive)
            {
                yield return new WaitForSeconds(5f);
                SaveGame();
                Debug.Log("Auto-saved game at " + System.DateTime.Now);
            }
            else
            {
                Debug.Log("In dialog, skipping autosave");
                yield return new WaitForSeconds(1f); // Small delay before checking again
            }
        }
    }

}
