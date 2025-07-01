using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class LevelUpNotification_Manager : MonoBehaviour
{
    public Text camp_Name;
    public Text camp_Name_2;
    public Text camp_level_2;
    public Text camp_level;
    public Text eventType;
    public Animator animator;
    public GameObject levelUp_Parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        XPManager.levelUpNotification = this;      
    }

    public void LevelUpNotificationSetup(CampType campType)
    {
        CampTypeData campData = DataGameManager.instance.campTypeDataList.Find(data => data.campType == campType);
        camp_Name.text = campData.campName;
        camp_Name_2.text = camp_Name.text;
        int currentLevel = DataGameManager.instance.campXPDictionaries.TryGetValue(campType, out var data) ? data.currentLevel : 0;
        camp_level.text = "Level: " + currentLevel.ToString();
        camp_level_2.text = camp_level.text;
        eventType.text = "Level Up!";

        levelUp_Parent.SetActive(true);
        animator.Play("IdleState");
        animator.SetTrigger("PlayAnim");
    }

    public void CampUnlocked(string unlockname)
    {
        eventType.text = "New Unlock!";
        camp_Name.text = unlockname;
        camp_Name_2.text = camp_Name.text;

        camp_level.text = "Unlocked!";
        camp_level_2.text = camp_level.text;

        levelUp_Parent.SetActive(true);
        animator.Play("IdleState");
        animator.SetTrigger("PlayAnim");
    }

    public void NewObjective(string objectiveName)
    {
        eventType.text = "New Objective!";
        camp_Name.text = objectiveName;
        camp_Name_2.text = camp_Name.text;

        camp_level.text = "Added!";
        camp_level_2.text = camp_level.text;

        levelUp_Parent.SetActive(true);
        animator.Play("IdleState");
        animator.SetTrigger("PlayAnim");
    }

    public void IncreasedPop(string objectiveName)
    {
        eventType.text = "Populated Increased!";
        camp_Name.text = objectiveName;
        camp_Name_2.text = camp_Name.text;

        camp_level.text = "Population!";
        camp_level_2.text = camp_level.text;

        levelUp_Parent.SetActive(true);
        animator.Play("IdleState");
        animator.SetTrigger("PlayAnim");
    }

    public void IncreasedStorage(string objectiveName)
    {
        eventType.text = "Storage Increased!";
        camp_Name.text = objectiveName;
        camp_Name_2.text = camp_Name.text;

        camp_level.text = "";
        camp_level_2.text = camp_level.text;

        levelUp_Parent.SetActive(true);
        animator.Play("IdleState");
        animator.SetTrigger("PlayAnim");
    }

}
