using UnityEngine;
using UnityEngine.UI;

public class Objective_Slot_UI : MonoBehaviour
{
    public Text objective_Name;
    public Text objective_NameBG;
    public GameObject task_List;
    public GameObject task_slot;


    public void SetupObjectiveUI(ObjectiveInstance data)
    {
        objective_Name.text = data.baseData.objectiveName;
        objective_NameBG.text = data.baseData.objectiveName;


        foreach (Transform child in task_List.transform) // Clear existing slots
        {
            Destroy(child.gameObject);
        }

        foreach (TaskData task in data.baseData.tasks)
        {
            GameObject newSlot = Instantiate(task_slot, task_List.transform);

            Objective_Task_Slot_UI newslotScript = newSlot.GetComponent<Objective_Task_Slot_UI>();
            newslotScript.taskName.text = task.taskName;
            newslotScript.taskQty.text = task.taskCurrentQty + "/" + task.taskMaxQty.ToString();
        }
    }

    public void RefreshUI(ObjectiveInstance instance)
    {
        foreach (Transform child in task_List.transform) // Clear existing slots
        {
            Destroy(child.gameObject);
        }

        foreach (TaskInstance task in instance.taskInstances)
        {
            GameObject newSlot = Instantiate(task_slot, task_List.transform);

            Objective_Task_Slot_UI newslotScript = newSlot.GetComponent<Objective_Task_Slot_UI>();
            newslotScript.taskName.text = task.taskName;
            newslotScript.taskQty.text = task.currentQty + "/" + task.maxQty.ToString();
        }
    }
}
