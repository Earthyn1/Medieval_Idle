using UnityEditor.UI;
using UnityEngine;

public class Objective_Tracker_UI : MonoBehaviour
{
    public bool IsOpen;
    public Animator animator;
    public GameObject ObjectivesList;
    public GameObject objectiveSlot;
    public GameObject objectiveTaskSlot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Objective_Manager.objectivesTracker = this;
    }

    public void ToggleObjectiveTracker_State()
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

    public void CreateNewObjective(ObjectiveInstance data)
    {
        GameObject newSlot = Instantiate(objectiveSlot, ObjectivesList.transform);
        Objective_Slot_UI newslotScript = newSlot.GetComponent<Objective_Slot_UI>();

        newslotScript.SetupObjectiveUI(data);
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

}
