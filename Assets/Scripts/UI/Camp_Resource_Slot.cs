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
        Transform parentTransform = progressBar.transform.parent;

        if (!DataGameManager.instance.activeCamps.Contains(campActionData)) //Doesnt contain camp?

        {
            campActionData.StartTime = DateTime.Now; //set timer to 0
            DataGameManager.instance.activeCamps.Add(campActionData);
            parentTransform.gameObject.SetActive(true);         
            requiredResource_Parent.SetActive(false);
        }
        else
        {
            DataGameManager.instance.activeCamps.Remove(campActionData);
            parentTransform.gameObject.SetActive(false);
            requiredResource_Parent.SetActive(true);
        }

        Debug.Log(campActionData.CampData.campType);
    }

    public void UpdateProgressBar(float progress)
    {
        progressBar.fillAmount = progress;  // Assuming the slider's range is 0 to 1
    }

}
