using UnityEngine;
using System.Collections.Generic;
using System.Linq;


[System.Serializable]
public class TaskData
{
    public string taskName;
    public string taskId;
    public int taskMaxQty;
    public int taskCurrentQty = 0;
}

[CreateAssetMenu(fileName = "NewObjective", menuName = "Objectives/Objective")]
public class ObjectiveData : ScriptableObject
{
    public string objectiveName;
    public List<TaskData> tasks = new List<TaskData>();
}

public class ObjectiveInstance
{
    public ObjectiveData baseData;
    public List<TaskInstance> taskInstances;

    public ObjectiveInstance(ObjectiveData data)
    {
        baseData = data;
        taskInstances = data.tasks.Select(t => new TaskInstance(t)).ToList();
    }
}

public class TaskInstance
{
    public string taskName;
    public string taskId;
    public int currentQty;
    public int maxQty;

    public TaskInstance(TaskData data)
    {
        taskName = data.taskName;
        taskId = data.taskId;
        maxQty = data.taskMaxQty;
        currentQty = 0;
    }
}
