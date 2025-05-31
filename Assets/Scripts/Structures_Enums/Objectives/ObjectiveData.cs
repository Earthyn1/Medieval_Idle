using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewObjective", menuName = "Objectives/Objective")]
public class ObjectiveData : ScriptableObject
{
    public string objectiveName;
    public List<TaskData> tasks = new List<TaskData>();
    public TutorialGroupData DialogIDWhenCompleted;
    public List<Objectives_Rewards> Objectives_Rewards = new List<Objectives_Rewards>();
}