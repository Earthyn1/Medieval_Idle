using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampButtonUpdater : MonoBehaviour
{
    public GameObject campsVerticalLayout;  // Assign the parent object holding all camp buttons
    private Dictionary<CampType, Text> campButtonTexts = new Dictionary<CampType, Text>();

    void Start()
    {
        InitializeCampButtonDictionary();
        XPManager.campverticalLayout = FindFirstObjectByType<CampButtonUpdater>();
        StartCoroutine(UpdateCampButtonLevelsDelayed());
    }
        
    private void InitializeCampButtonDictionary() // Initialize the dictionary by looping through child buttons
    {
        foreach (Transform child in campsVerticalLayout.transform)
        {
            string[] ignoredNames = { "Town_Storage_Button", "Local_Market_Button", "Empty_Button" };
            if (System.Array.Exists(ignoredNames, name => name == child.gameObject.name))
            {
                continue; // Skip this iteration and move to the next child;
            }
                
            // Get the CampSideButtonSetup component directly from the button
            CampButtonSetup buttonSetup = child.GetComponent<CampButtonSetup>();
            Text buttonText = child.Find("Camp_Level")?.GetComponent<Text>(); // Find the Text component specifically named "Camp_Level" within the child                                                                              
            campButtonTexts[buttonSetup.campData.campType] = buttonText;            
        }   
    }
    private IEnumerator UpdateCampButtonLevelsDelayed()
    {
        // Wait for half a second
        yield return new WaitForSeconds(0.5f);

        // Now execute the update logic
        UpdateAllCampButtonLevels();
    }

    public void UpdateCampButtonLevel(CampType campType)
    {
        if (campButtonTexts.ContainsKey(campType))  // Check if the dictionary contains the key
        {
            Text buttonText = campButtonTexts[campType];  // Get the button text            
            int level = DataGameManager.instance.campXPDictionaries[campType].currentLevel;  // Get the level from the XP data
            buttonText.text = $" Lvl: {level}";  // Update the text with the camp type and level
        }
        else
        {
            Debug.LogError($"No button found for camp type: {campType}");  // Log if the key doesn't exist in the dictionary
        }
    }

    
    public void UpdateAllCampButtonLevels() // Update all buttons
    {
        
        foreach (var kvp in campButtonTexts)
        {           
            UpdateCampButtonLevel(kvp.Key);
        }
    }
}