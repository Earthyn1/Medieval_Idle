using UnityEngine;
using UnityEngine.UI;

public class TopPanelManager : MonoBehaviour
{
    public Text campName;
    public Text goldText;
    public Text playerPopText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DataGameManager.instance.topPanelManager = this;
    }

    public void UpdateGold()
    {
        goldText.text = DataGameManager.instance.PlayerGold.ToString();

    }

    public void UpdateTownPopulation()
    {
        playerPopText.text = DataGameManager.instance.CurrentVillagerCount.ToString() + "/" + DataGameManager.instance.MaxVillagerCapacity;

    }




}
