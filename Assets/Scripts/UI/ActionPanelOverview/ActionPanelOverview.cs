using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanelOverview : MonoBehaviour
{
    public bool IsOpen;
    public Animator animator;
    public GameObject ObjectivesList;
    public GameObject objectiveSlot;
    public GameObject objectiveTaskSlot;
    public GameObject sideActionSlot;
    public Transform gridParent;
    public Text new_Update_Text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Objective_Manager.objectivesTracker = this;
        DataGameManager.instance.actionPanelOverview = this;

        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }


    }

    public void ToggleObjectiveTracker_State()
    {


        if (DataGameManager.instance.Tutorial_Lists.GetFlag(TutorialFlagID.ObjectivesTrackerUnlock.ToString()))
        {

            if (IsOpen)
            {
                animator.Play("IdleState", 0, 0f);
                animator.ResetTrigger("Close");
                animator.SetTrigger("Close");
                IsOpen = false;
            }
            else
            {
                animator.Play("IdleState", 0, 0f);
                animator.ResetTrigger("Open");
                animator.SetTrigger("Open");
                IsOpen = true;
            }
        }
        else
        {
            Debug.Log("NotYetUnlocked!");
        }
   
    }
    public void ShowUpdateButtonAnimation()
    {
        if (IsOpen == false)
        {
            new_Update_Text.text = "UPDATE";
            animator.Play("IdleState", 0, 0f);
            animator.ResetTrigger("FlashUpdate");
            animator.SetTrigger("FlashUpdate");
        }
    }
    public void CreateNewObjective(ObjectiveInstance data)
    {
        GameObject newSlot = Instantiate(objectiveSlot, ObjectivesList.transform);
        Objective_Slot_UI newslotScript = newSlot.GetComponent<Objective_Slot_UI>();

        newslotScript.SetupObjectiveUI(data);

        if (IsOpen == false)
        {
            new_Update_Text.text = "NEW";
            animator.Play("IdleState", 0, 0f);
            animator.ResetTrigger("FlashUpdate");
            animator.SetTrigger("FlashUpdate");
        }
    }

    public void UpdateObjectivesUI(ObjectiveInstance instance)
    {
        Debug.Log(instance.baseData.objectiveName);

        foreach (Transform child in ObjectivesList.transform)
        {
            Objective_Slot_UI newslotScript = child.GetComponent<Objective_Slot_UI>();
  

            if (newslotScript.objective_Name.text == instance.baseData.objectiveName)
            {
                newslotScript.RefreshUI(instance);
            }   
        }
    }

    public void RemoveObjectivesUI(ObjectiveInstance instance)
    {

        foreach (Transform child in ObjectivesList.transform)
        {
            Objective_Slot_UI newslotScript = child.GetComponent<Objective_Slot_UI>();

            if (newslotScript != null && newslotScript.objective_Name.text == instance.baseData.objectiveName)
            {
                GameObject.Destroy(child.gameObject); // ✅ Destroy the child GameObject
                break; // Optional: stop after finding the first match
            }
        }
    }

    public void CheckForObjectiveComplete()
    {

        foreach (ObjectiveInstance instance in DataGameManager.instance.ActiveObjectives)
        {
            bool allTasksCompleted = true;

            foreach (TaskInstance task in instance.taskInstances)
            {
                if (task.currentQty != task.maxQty)
                {
                    allTasksCompleted = false;
                }
            }

            if (allTasksCompleted)
            {
                new_Update_Text.text = "UPDATE";
                animator.Play("IdleState", 0, 0f);
                animator.ResetTrigger("FlashUpdate");
                animator.SetTrigger("FlashUpdate");
            }
        }
    }

    public SideActionSlot AddSideActionPanel(CampActionData slot, string key)
    {
        GameObject newSlot = Instantiate(sideActionSlot, ObjectivesList.transform);
        SideActionSlot newslotScript = newSlot.GetComponent<SideActionSlot>();

        newslotScript.SetupSlot(slot, key);

        return newslotScript;
    }
}
