using UnityEngine;

public class Objective_Update_Feed_Parent : MonoBehaviour
{
    public GameObject Objective_Update_Feed_Prefab;  // Public reference to your slot prefab

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Objective_Manager.Objective_Update_Feed_Parent = this;
    }

    public void AddObjectiveFeed_Slot(string id, string updateAmount)
    {
        GameObject newSlot = Instantiate(Objective_Update_Feed_Prefab, gameObject.transform);
        Objective_Tracker_Feed_Slot newSlot_Script = newSlot.GetComponent<Objective_Tracker_Feed_Slot>();
        newSlot_Script.SetupSlot(id, updateAmount);
    }
}
