using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewUnlocks_Notifcation_Manager : MonoBehaviour
{

    public Text actionName;
    public Image actionImage;
    public Animator animator;
    public List<CampActionData> newActionsList;

    private bool isAnimationPlaying = false;

    private void Awake()
    {
        newActionsList = new List<CampActionData>();
    }

    void Start()
    {
        XPManager.newUnlocksNotifcation = this;
    }

    public void NewUnlocksAnimationSetup()
    {
        animator.Play("IdleState");
        animator.ResetTrigger("PlayAnimation");
        animator.SetTrigger("PlayAnimation");
    }


    public void HandlePlayingAnimations()
    {  
        if (isAnimationPlaying) return;

        if (newActionsList.Count > 0)
        {
            CampActionData firstAction = newActionsList[0];
            actionName.text = firstAction.resourceName;
            actionImage.sprite = firstAction.bgImage;
            NewUnlocksAnimationSetup();
            isAnimationPlaying = true;
        }
        else
        {
            Debug.Log("No more unlocks to show!");
        }
    }

    public void RemoveFromList()
    {
        if (newActionsList.Count > 0)
            newActionsList.RemoveAt(0);

        isAnimationPlaying = false;
        animator.Play("IdleState", 0, 0f);
        HandlePlayingAnimations();
    }

    public void CheckForNewUnlocks(CampType campType, int newLevel, int oldLevel)
    {
        var campDataDict = DataGameManager.instance.GetCampData(campType);

        newActionsList.AddRange(campDataDict.Values.Where(data => data.levelUnlocked > oldLevel && data.levelUnlocked <= newLevel));

        HandlePlayingAnimations();

    }
}
