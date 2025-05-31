using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Objective_Slot_UI : MonoBehaviour
{
    public Text objective_Name;
    public GameObject completeObjectiveButton;
    public GameObject task_List;
    public GameObject task_slot;
    public GameObject Rewards_Parent;
    public GameObject Rewards_Slot;
    public Color defaultColor;
    public Animator animator;
    public ObjectiveInstance ToRemove;


    public void SetupObjectiveUI(ObjectiveInstance data)
    {
        objective_Name.text = data.baseData.objectiveName;

        foreach (Transform child in task_List.transform) // Clear existing slots
        {
            Destroy(child.gameObject);
        }

        foreach (TaskInstance task in data.taskInstances)
        {
            GameObject newSlot = Instantiate(task_slot, task_List.transform);

            Objective_Task_Slot_UI newslotScript = newSlot.GetComponent<Objective_Task_Slot_UI>();
            defaultColor = newslotScript.taskQty.color;
            newslotScript.taskName.text = task.taskName;
            newslotScript.taskQty.text = task.currentQty + "/" + task.maxQty.ToString();
        }

        foreach (Objectives_Rewards objectives_Rewards in data.baseData.Objectives_Rewards)
        {
            GameObject newSlot = Instantiate(Rewards_Slot, Rewards_Parent.transform);

            Objective_Rewards_UI newslotScript = newSlot.GetComponent<Objective_Rewards_UI>();

            if (objectives_Rewards.campType != CampType.NA) //If reward is campXP
            {
                CampTypeData campType = DataGameManager.instance.GetCampTypeDataByType(objectives_Rewards.campType);
                newSlot.name = "RewardSlot_" + campType.name;


                newslotScript.image.sprite = campType.campImage;
                newslotScript.item_XP_text.text = objectives_Rewards.amount.ToString() + "xp";
            }
            else
            {
                if (objectives_Rewards.itemID != null)
                {
                    if (DataGameManager.instance.TryFindItemData(objectives_Rewards.itemID, out var itemdata))
                    {
                        newSlot.name = "RewardSlot_" + itemdata.ItemName;
                        newslotScript.image.sprite = itemdata.ItemImage;
                        newslotScript.item_XP_text.text = objectives_Rewards.amount.ToString();
                    }
                }
            }
        }

        RefreshUI(data);
    }

    public void RefreshUI(ObjectiveInstance instance)
    {
        foreach (Transform child in task_List.transform) // Clear existing slots
        {
            Destroy(child.gameObject);
        }
        bool allTasksCompleted = true;
        string taskname = "";

        foreach (TaskInstance task in instance.taskInstances)
        {
            GameObject newSlot = Instantiate(task_slot, task_List.transform);

            Objective_Task_Slot_UI newslotScript = newSlot.GetComponent<Objective_Task_Slot_UI>();
            newslotScript.taskName.text = task.taskName;
            newslotScript.taskQty.text = task.currentQty + "/" + task.maxQty.ToString();
            taskname = task.taskName;

            if (task.currentQty < task.maxQty)
            {
                allTasksCompleted = false;
                newslotScript.taskQty.color = defaultColor;
            }
            else
            {
                newslotScript.taskQty.color = Color.green;

            }
        }

        if (allTasksCompleted)
        {
            completeObjectiveButton.SetActive(true);

        }

    }



    public void OnCompleteObjectiveButtonPressed()
    {
        ObjectiveInstance toRemove = null;
        CampType campType = CampType.NA;
        foreach (ObjectiveInstance instance in DataGameManager.instance.ActiveObjectives)
        {
            if (instance.baseData.objectiveName == objective_Name.text)
            {
                if (instance.baseData.DialogIDWhenCompleted != null)
                {
                    DataGameManager.instance.tutorialManager.SetupTutorial(instance.baseData.DialogIDWhenCompleted);
                }

                if (instance.baseData.Objectives_Rewards.Count > 0)
                {
                    foreach (Objectives_Rewards objectives_Rewards in instance.baseData.Objectives_Rewards)
                    {
                        if (objectives_Rewards.campType != CampType.NA)
                        {
                            XPManager.AddXP(objectives_Rewards.campType, objectives_Rewards.amount);
                            campType = objectives_Rewards.campType;
                        }
                        else
                        {
                            if (objectives_Rewards.itemID != null)
                            {
                                if (DataGameManager.instance.TryFindItemData(objectives_Rewards.itemID, out var item))
                                {
                                    TownStorageManager.AddItem(item.ItemID, objectives_Rewards.amount, campType);
                                }
                            }
                        }
                    }
                }

                toRemove = instance;
                break; // Assuming only one should match
            }
        }

        if (toRemove != null)
        {
            // Objective_Manager.RemoveObjective(toRemove);

            ToRemove = toRemove;
            animator.Play("IdleState", 0, 0f);
            animator.ResetTrigger("ObjectiveComplete");
            animator.SetTrigger("ObjectiveComplete");
        }
    }

    public void OnCompleteAnimationEnd()
    {
        Objective_Manager.RemoveObjective(ToRemove);
    }

   
       
}
