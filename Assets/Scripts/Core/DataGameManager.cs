using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataGameManager : MonoBehaviour
{
    private BaseCVSLoader BaseCSVLoader;

    [HideInInspector]
    public TopPanelManager topPanelManager;
    [HideInInspector]
    public Populate_Camp_Slots populate_Camp_Slots;
    [HideInInspector]
    public Populate_Storage_Slots populate_Storage_Slots;
    [HideInInspector]
    public Populate_Local_Market_Slots populate_Local_Market_Slots;
    [HideInInspector]
    public GameObject SelectedButton;
    [HideInInspector]
    public CampType currentActiveCamp;
    [HideInInspector]
    public Game_Text_Alerts Game_Text_Alerts;
    [HideInInspector]
    public Camp_Boosts_Manager boostsManager;
   
    

    public float DEVspeedMultiplier = 10f; // or whatever speed boost you want


    public int CurrentLandDeedsOwned = 0;
    public int landDeedsbrought = 0;

    public int MaxInventorySlots = 12;  // Default capacity
    public int PlayerGold = 0;
    public int MaxVillagerCapacity =5;
    public int CurrentVillagerCount = 5;
    public Image BG_Banner;
    [HideInInspector]
    public GameObject tierShield;
    public int CurrentContentLevelAvailable = 30;
    [HideInInspector]
    public CampCategorys currentCampCategory;
    [HideInInspector]
    public GameObject CurrentCategoryUISelected;
    [HideInInspector]
    public Item_XP_FeedManager item_XP_FeedManager;
    [HideInInspector]
    public TutorialManager tutorialManager;
    [HideInInspector]
    public CampSpecific_CSV_Loaders CampSpecific_CSV_Loaders;
    [HideInInspector]
    public UpperPanel_Manager upperPanelManager;
    [HideInInspector]
    public CampBehaviorManager campBehaviorManager;
    [HideInInspector]
    public ActionCampHandler actionCampHandler;
    [HideInInspector]
    public CampButtonUpdater campButtonUpdater;
    [HideInInspector]
    public Tutorial_Lists Tutorial_Lists;
    [HideInInspector]
    public DropDownMenu DropDownMenu;
    [HideInInspector]
    public SimpleItemData currentFishingBaitEquipped;
         


    public Dictionary<int, int> levelXp; //levelXp is the 1-99 xp amounts
    public Dictionary<CampType, CampXPData> campXPDictionaries = new Dictionary<CampType, CampXPData>();

    //Dictionary of the camps and if they are locked or not.
    public Dictionary<CampType, bool> campLockedDict = new Dictionary<CampType, bool>(); 


    //list of camp CSV
    public List<CampCsvEntry> campCsvFiles;

    // Separate dictionaries for each camp
    public Dictionary<string, CampActionData> lumberCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> miningCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> fishingCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> blacksmithCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> merchantCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> constructionCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> hunterCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> farmsteadActions = new Dictionary<string, CampActionData>();

    //Seperate dictionaries for each camp specific module

    public Dictionary<string, ConstructionCampModule> constructionCampModuleData;
    public Dictionary<string, VeinData> miningCampModuleData;

    public Dictionary<string, OneSlotUseActions_Struc> OneSlotUseActions = new();


    //combined into 1 dictionary by Camp Type
    public Dictionary<CampType, Dictionary<string, CampActionData>> campDictionaries = new Dictionary<CampType, Dictionary<string, CampActionData>>();

    //Active Camps
    public HashSet<CampActionEntry> activeCamps = new HashSet<CampActionEntry>();

    //Item list dictionary
    public  Dictionary<string, ItemData_Struc> itemData_Array;
    //Current Town Storage dictionary
    public List<StorageSlot> TownStorage_List = new List<StorageSlot>();
    //Local Market dictionary
    public Dictionary<string, LocalMarket_Items> localMarket_Items_List;

    //FishingBait Dictionary
    public Dictionary<string, FishingBait_Item_Struc> fishingBait_Item_List;
   
    //List of the CampTypeData Struc
    public List<CampTypeData> campTypeDataList;

    // CUrrent objectives
    public List<ObjectiveInstance> ActiveObjectives = new List<ObjectiveInstance>();

    // Completed objectives
    public List<ObjectiveInstance> CompletedObjectives = new List<ObjectiveInstance>();

    //Dictionry of flags for the game like first time opening x thing
    public Dictionary<string, bool> tutorialFlags = new Dictionary<string, bool>();

    //The FishingCamp Boost Data.
    public FishingCamp_Boost_Struc FishingCamp_Boost = new FishingCamp_Boost_Struc();
    //The LumberCamp Boost Data.
    public LumberCamp_Boost_Struc LumberCamp_Boost = new LumberCamp_Boost_Struc();
    //The ConstructionCamp Boost Data.
    public ConstructionCamp_Boost_Struc ConstructionCamp_Boost = new ConstructionCamp_Boost_Struc();
    //The MiningCamp Boost Data.
    public MiningCamp_Boost_Struc MiningCamp_Boost = new MiningCamp_Boost_Struc();


    //MiningCamp Vein Data
    public Dictionary<string, VeinData> miningVeinStates = new(); // slotKey -> saved vein










    public static DataGameManager instance;

    public void Awake()
    {
        FishingCamp_Boost.InitializeSprites();
        LumberCamp_Boost.InitializeSprites();
        ConstructionCamp_Boost.InitializeSprites();
        MiningCamp_Boost.InitializeSprites();

        // Check if an instance already exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Makes the GameManager persist between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate
            return;
            
        }

        BaseCSVLoader = GetComponent<BaseCVSLoader>();
        populate_Camp_Slots = GetComponent<Populate_Camp_Slots>();
        populate_Storage_Slots = GetComponent<Populate_Storage_Slots>();
        populate_Local_Market_Slots = GetComponent<Populate_Local_Market_Slots>();  
       // campBehaviorManager = GetComponent<CampBehaviorManager>(); //behaviours replaced

        foreach (CampType type in Enum.GetValues(typeof(CampType)))
        {
            campLockedDict[type] = true; //setting all camps to locked on setup
        }

        for (int i = 0; i < MaxInventorySlots; i++)
        {
            TownStorage_List.Add(new StorageSlot { ItemID = null, Quantity = 0 }); //initialize the slots
        }


        // Load all camp dictionaries and add them to the campDictionaries map
        campDictionaries = new Dictionary<CampType, Dictionary<string, CampActionData>>();

        foreach (var entry in campCsvFiles)
        {
            if (entry.csvFile == null)
            {
                Debug.LogWarning($"CSV for {entry.campType} is null – make sure you assigned it in the inspector!");
                continue;
            }

            var data = BaseCSVLoader.LoadCSV(entry.csvFile); // This must return Dictionary<string, CampActionData>
            campDictionaries[entry.campType] = data;
        }

      //  campBehaviorManager.AssignCampBehaviors(campDictionaries); //Assign the Camp Behaviours! // Behaviours replaced!

        PlayerXPManager();  // Initialize the dictionary with all camps and default XP data

    }

    public List<CampBoost_Class> GetBaseBoostsForCamp(CampType campType)
    {
        return campType switch
        {
            CampType.FishingCamp => FishingCamp_Boost.GetAllBoosts(),
            CampType.LumberCamp => LumberCamp_Boost.GetAllBoosts(),
            CampType.ConstructionCamp => ConstructionCamp_Boost.GetAllBoosts(),
            CampType.MiningCamp => MiningCamp_Boost.GetAllBoosts(),
            _ => new List<CampBoost_Class>()
        };
    }
    public CampTypeData GetCampTypeDataByType(CampType campType)
    {
        return campTypeDataList.Find(data => data.campType == campType);
    }

    public Dictionary<string, CampActionData> GetCampData(CampType campType) // Function to retrieve the dictionary for a specific camp type
    {
        if (campDictionaries.TryGetValue(campType, out var campData))
        {
            return campData;
        }
        Debug.LogWarning($"No data found for camp type: {campType}");
        return null;
    }

    public CampActionData GetCampActionData(CampType campType, string resourceName)
    {
        if (campDictionaries.TryGetValue(campType, out var campDict))
        {
            if (campDict.TryGetValue(resourceName, out var campActionData))
            {
                return campActionData;
            }
        }
        return null; // or throw or handle missing case as needed
    }

    public void PlayerXPManager()  // Initialize the dictionary with all camps and default XP data
    {       
        foreach (CampType campType in Enum.GetValues(typeof(CampType)))
        {
            campXPDictionaries[campType] = new CampXPData { currentXP = 0, currentLevel = 1 };
        }
    }

    public bool TryFindItemData(string itemID, out ItemData_Struc itemData)
    {
        return itemData_Array.TryGetValue(itemID, out itemData);
    }

    public void SetCampLockedStatus(CampType campType, bool isLocked)
    {
        if (campLockedDict.ContainsKey(campType))
        {
            campLockedDict[campType] = isLocked;
        }
        else
        {
            campLockedDict.Add(campType, isLocked);
        }
    }





}







