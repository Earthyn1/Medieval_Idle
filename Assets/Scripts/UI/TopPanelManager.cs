using UnityEngine;
using UnityEngine.UI;

public class TopPanelManager : MonoBehaviour
{
    public Text campName;
    public Text goldText;
    public Text playerPopText;
    public GameObject campNameParent;

    public Animator AddedGoldAnimator;
    public Text AddedRemovedGold;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DataGameManager.instance.topPanelManager = this;
        UpdateGold();
        UpdateTownPopulation();
    }

    public void UpdateGold()
    {
        goldText.text = DataGameManager.instance.PlayerGold.ToString();

    }

    public void UpdateTownPopulation()
    {
        playerPopText.text = DataGameManager.instance.CurrentVillagerCount.ToString() + "/" + DataGameManager.instance.MaxVillagerCapacity;
        DataGameManager.instance.populate_Camp_Slots.UpdateRequiredVillager_Colors();

    }

    public void AddedRemovedGoldAnim(bool adding, int amount)
    {
        if(adding)
        {
            AddedRemovedGold.text = "+" + amount;
            AddedRemovedGold.color = Color.green;
        }
        else
        {
            AddedRemovedGold.text = "-" + amount;
            AddedRemovedGold.color = Color.red;
        }

        AddedGoldAnimator.Play("IdleState");
        AddedGoldAnimator.ResetTrigger("PlayAnimation");
        AddedGoldAnimator.SetTrigger("PlayAnimation");
    }


}
