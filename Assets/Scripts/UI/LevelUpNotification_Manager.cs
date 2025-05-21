using UnityEngine;
using UnityEngine.UI;

public class LevelUpNotification_Manager : MonoBehaviour
{
    public Text camp_Name;
    public Text camp_level;
    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        XPManager.levelUpNotification = this;      
    }

    public void LevelUpNotificationSetup(CampType campType)
    {
        CampTypeData campData = DataGameManager.instance.campTypeDataList.Find(data => data.campType == campType);
        camp_Name.text = campData.campName;

        int currentLevel = DataGameManager.instance.campXPDictionaries.TryGetValue(campType, out var data) ? data.currentLevel : 0;
        camp_level.text = "Level " + currentLevel + "!";
        animator.Play("IdleState");
        animator.SetTrigger("PlayAnim");


    }

}
