using UnityEngine;

public static class Objective_Manager
{
    public static Objective_Tracker_UI objectivesTracker;
    public static GameObject objectiveslot;


    public static void CreateNewObjective(ObjectiveData data)
    {
        ObjectiveInstance instance = new ObjectiveInstance(data);

        DataGameManager.instance.ActiveObjectives.Add(instance); 

        objectivesTracker.CreateNewObjective(instance);
    }


    public static void UpdateObjectives(string id, int updateAmount)
    {
        foreach (ObjectiveInstance instance in DataGameManager.instance.ActiveObjectives)
        {
            foreach (TaskInstance task in instance.taskInstances)
            {
                if (task.taskId == id)
                {
                    task.currentQty = Mathf.Min(task.currentQty + updateAmount, task.maxQty);

                    // ✅ Notify UI manager to update this objective
                    objectivesTracker.UpdateObjectivesUI(instance);
                    return; // Exit early once task is updated
                }
            }
        }
    }







}
