using System;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject SelectedButton;
    [HideInInspector]
    public CampType currentActiveCamp;

    public int MaxInventorySlots = 12;  // Default capacity
    public int PlayerGold = 0;
    public int MaxVillagerCapacity =5;
    public int CurrentVillagerCount = 5;
    public GameObject tierShield;


    public Dictionary<int, int> levelXp; //levelXp is the 1-99 xp amounts
    public Dictionary<CampType, CampXPData> campXPDictionaries = new Dictionary<CampType, CampXPData>();

    public TextAsset FishingCampCsv;
    public TextAsset BlacksmithCsv;
    public TextAsset LumberCampCsv;
    public TextAsset ConstructionCampCsv;
    public TextAsset MiningCampCsv;   


    // Separate dictionaries for each camp
    public Dictionary<string, CampActionData> lumberCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> miningCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> fishingCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> blacksmithCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> merchantCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> constructionCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> hunterCampActions = new Dictionary<string, CampActionData>();
    public Dictionary<string, CampActionData> farmsteadActions = new Dictionary<string, CampActionData>();

    //combined into 1 dictionary by Camp Type
    public Dictionary<CampType, Dictionary<string, CampActionData>> campDictionaries = new Dictionary<CampType, Dictionary<string, CampActionData>>();

    //Active Camps
    public HashSet<CampActionEntry> activeCamps = new HashSet<CampActionEntry>();

    //Item list dictionary
    public  Dictionary<string, ItemData_Struc> itemData_Array;
    //Current Town Storage dictionary
    public List<StorageSlot> TownStorage_List = new List<StorageSlot>();


    public static DataGameManager instance;

    public void Start()
    {
        BaseCSVLoader = GetComponent<BaseCVSLoader>();
        populate_Camp_Slots = GetComponent<Populate_Camp_Slots>();
        populate_Storage_Slots = GetComponent<Populate_Storage_Slots>();

        for (int i = 0; i < MaxInventorySlots; i++)
        {
            TownStorage_List.Add(new StorageSlot { ItemID = null, Quantity = 0 }); //initialize the slots
        }


        // Load all camp dictionaries and add them to the campDictionaries map
        campDictionaries[CampType.FishingCamp] = BaseCSVLoader.LoadCSV(FishingCampCsv);
        campDictionaries[CampType.LumberCamp] = BaseCSVLoader.LoadCSV(LumberCampCsv);
        campDictionaries[CampType.MiningCamp] = BaseCSVLoader.LoadCSV(MiningCampCsv);
        campDictionaries[CampType.Blacksmith] = BaseCSVLoader.LoadCSV(BlacksmithCsv);
        campDictionaries[CampType.ConstructionCamp] = BaseCSVLoader.LoadCSV(ConstructionCampCsv);   
        
        PlayerXPManager();  // Initialize the dictionary with all camps and default XP data

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

    public void PlayerXPManager()  // Initialize the dictionary with all camps and default XP data
    {       
        foreach (CampType campType in Enum.GetValues(typeof(CampType)))
        {
            campXPDictionaries[campType] = new CampXPData { currentXP = 0, currentLevel = 1 };
        }
    }


    private void Awake()
    {
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
    }
}







