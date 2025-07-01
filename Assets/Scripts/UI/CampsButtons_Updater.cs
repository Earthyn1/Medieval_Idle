using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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

    public void UpdateCampUsageGreenDots(CampType campType, int Amount)
    {
        foreach (Transform child in campsVerticalLayout.transform)
        {
            CampButtonSetup childscript = child.GetComponent<CampButtonSetup>();
            if (childscript.campData.campType == campType)
            {
                if (Amount > 0)
                {
                    // Add green dots
                    for (int i = 0; i < Amount; i++)
                    {
                        GameObject newDot = Instantiate(childscript.GreenDot, childscript.CampPopUsage_Parent.transform);
                    }
                }
                else if (Amount < 0)
                {
                    // Remove green dots safely by caching first
                    int toRemoveCount = Mathf.Min(Mathf.Abs(Amount), childscript.CampPopUsage_Parent.transform.childCount);
                    List<Transform> toRemove = new List<Transform>();

                    for (int i = 0; i < toRemoveCount; i++)
                    {
                        toRemove.Add(childscript.CampPopUsage_Parent.transform.GetChild(i));
                    }

                    foreach (var t in toRemove)
                    {
                        GameObject.Destroy(t.gameObject);
                    }
                }
                // If Amount == 0, do nothing
            }
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
                XPManager.levelUpNotification.CampUnlocked(campType.ToString());

                childscript.NewCamp_Parent.SetActive(true);
                childscript.NewCamp_Loop_Anim.Play("IdleState", 0, 0f);
                childscript.NewCamp_Loop_Anim.ResetTrigger("PlayNewCampLoop");
                childscript.NewCamp_Loop_Anim.SetTrigger("PlayNewCampLoop");

            }   

        }
    }

    public void LoadCampsFromSave(CampType campType)
    {
        foreach (Transform child in campsVerticalLayout.transform)
        {
            CampButtonSetup childscript = child.GetComponent<CampButtonSetup>();
            if (childscript.campData.campType == campType)
            {
                childscript.SetSlotAsUnlocked();
            }
        }
    }
}