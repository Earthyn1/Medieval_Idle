using UnityEngine;
using UnityEngine.UI;

public class ConstructionCamp_UpperPanel_Module : MonoBehaviour
{
    public Text currentLandDeeds;

    public void UpdateText()
    {
        currentLandDeeds.text = DataGameManager.instance.CurrentLandDeedsOwned.ToString();
    }
}
