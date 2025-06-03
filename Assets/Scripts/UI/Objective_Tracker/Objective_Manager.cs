using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Objective_Manager
{
    public static Objective_Tracker_UI objectivesTracker;
    public static GameObject objectiveslot;
    public static Objective_Update_Feed_Parent Objective_Update_Feed_Parent;


    public static void CreateNewObjective(ObjectiveData data)
    {
        ObjectiveInstance instance = new ObjectiveInstance(data);
        
        foreach (TaskInstance taskInstance in instance.taskInstances)
        {
            if(taskInstance.isInventoryItem)
            {
                taskInstance.currentQty = Mathf.Min(TownStorageManager.GetCurrentQuantity(taskInstance.taskId),taskInstance.maxQty);
            }
        }

        DataGameManager.instance.ActiveObjectives.Add(instance); 

        objectivesTracker.CreateNewObjective(instance);

        XPManager.levelUpNotification.NewObjective(data.objectiveName);
    }


    public static void UpdateObjectives(string id, int updateAmount)
    {
        // Temporary list to store completed objectives
        List<ObjectiveInstance> completedObjectives = new List<ObjectiveInstance>();

        foreach (ObjectiveInstance instance in DataGameManager.instance.ActiveObjectives)
        {
            bool allTasksCompleted = true;

            foreach (TaskInstance task in instance.taskInstances)
            {
                if (task.taskId == id)
                {
                    bool alreadyCompleted = task.currentQty == task.maxQty;
                    task.currentQty = Mathf.Min(task.currentQty + updateAmount, task.maxQty);
                    objectivesTracker.UpdateObjectivesUI(instance);

                    if (!alreadyCompleted)
                    {
                        Objective_Update_Feed_Parent.AddObjectiveFeed_Slot(task.taskName, $"{task.currentQty}/{task.maxQty}");
                    }
                }

                // Check if the task is completed, regardless of update
                if (task.currentQty < task.maxQty)
                {
                    allTasksCompleted = false;
                }
            }

            if (allTasksCompleted) 
            {
                objectivesTracker.ShowUpdateButtonAnimation();

                if (!DataGameManager.instance.Tutorial_Lists.GetFlag("CompleteObjectiveTutorial")) //This is a one time tutorial step for first time Objective is completed.
                {              
                    if (objectivesTracker.IsOpen)
                    {
                        TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("ExplainCompletedObjectivesButton_FromOpen");
                        DataGameManager.instance.tutorialManager.SetupTutorial(tutorialGroupData);
                    }
                    else
                    {
                        TutorialGroupData tutorialGroupData = DataGameManager.instance.Tutorial_Lists.FindDialog("ExplainCompletedObjectivesButton_FromClosed");
                        DataGameManager.instance.tutorialManager.SetupTutorial(tutorialGroupData);
                    }

                    DataGameManager.instance.Tutorial_Lists.SetFlag("CompleteObjectiveTutorial", true);

                    
                }

              
            }


        }

    }


    public static void RemoveObjective(ObjectiveInstance data)
    {
        DataGameManager.instance.CompletedObjectives.Add(data);
        DataGameManager.instance.ActiveObjectives.Remove(data);

        objectivesTracker.RemoveObjectivesUI(data);
        
    }



}







