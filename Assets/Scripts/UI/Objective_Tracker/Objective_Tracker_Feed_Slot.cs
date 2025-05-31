using UnityEngine;
using UnityEngine.UI;

public class Objective_Tracker_Feed_Slot : MonoBehaviour
{

    public Text ObjectiveName_feed_SlotText;
    public Text Objective_Step_Amount_feed_SlotText;
    public Animator item_feed_SlotAnimation;



    public void SetupSlot(string id, string updateAmount)
    {
        ObjectiveName_feed_SlotText.text = id;
        Objective_Step_Amount_feed_SlotText.text = updateAmount;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);

    }
}


