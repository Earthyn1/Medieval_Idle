using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System;

public class Camp_Resource_Slot : MonoBehaviour

{
    public CampActionEntry campActionData;
    public int flipflop;
    public Image progressBar;
    public Image FGImage;
    public Image BGImage;
    public Text popCount;
    public Text actionName;
    public GameObject requiredResource_Parent;



    public void OnClicked()
    {        

        if (!DataGameManager.instance.activeCamps.Contains(campActionData)) //Doesnt contain camp?

        {
            ActivateActionSlot();
        }
        else
        {
            DeactivateActionSlot();
        }

        Debug.Log(campActionData.CampData.campType);
    }

    public void UpdateProgressBar(float progress)
    {
        progressBar.fillAmount = progress;  // Assuming the slider's range is 0 to 1
    }

    public void DeactivateActionSlot()
    {
        Transform parentTransform = progressBar.transform.parent;

        DataGameManager.instance.activeCamps.Remove(campActionData);
        parentTransform.gameObject.SetActive(false);
        requiredResource_Parent.SetActive(true);

        DataGameManager.instance.CurrentVillagerCount = DataGameManager.instance.CurrentVillagerCount + campActionData.CampData.populationCost;
        DataGameManager.instance.topPanelManager.UpdateTownPopulation();
    }

    public void ActivateActionSlot()
    {
 
        if (DataGameManager.instance.CurrentVillagerCount - campActionData.CampData.populationCost >= 0)
        {
            Transform parentTransform = progressBar.transform.parent;

            campActionData.StartTime = DateTime.Now; //set timer to 0
            DataGameManager.instance.activeCamps.Add(campActionData);
            parentTransform.gameObject.SetActive(true);
            requiredResource_Parent.SetActive(false);

            DataGameManager.instance.CurrentVillagerCount = DataGameManager.instance.CurrentVillagerCount - campActionData.CampData.populationCost;
            DataGameManager.instance.topPanelManager.UpdateTownPopulation();

        }
        else
        {
            Debug.Log("Not enough pop");
        }

        
    }

}
