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
       
        DataGameManager.instance.campButtonUpdater = this;
    }

    public void UpdateCampButtonLevel(CampType campType)
    {
        bool foundMatch = false;

        foreach (Transform child in campsVerticalLayout.transform)
        {
            CampButtonSetup childscript = child.GetComponent<CampButtonSetup>();
            if (childscript.campData.campType == campType)
            {
                foundMatch = true;

                if (DataGameManager.instance.campLockedDict.TryGetValue(campType, out bool isLocked))
                {
                    if (!isLocked)
                    {
                        int level = DataGameManager.instance.campXPDictionaries[campType].currentLevel;
                        childscript.campXPText.text = $" Lvl: {level}";
                    }
                    else
                    {
                        Debug.Log("button locked");
                    }
                }
                break; // No need to keep looping after finding the match
            }
        }

        if (!foundMatch)
        {
            Debug.LogError($"No button found for camp type: {campType}");
        }
    }

    public void UpdateCampButtonAsUnlocked(CampType campType)
    {
        foreach (Transform child in campsVerticalLayout.transform)
        {
            CampButtonSetup childscript = child.GetComponent<CampButtonSetup>();
            if( childscript.campData.campType == campType)
            {
                childscript.SetSlotAsUnlocked();
            }   

        }
    }
}