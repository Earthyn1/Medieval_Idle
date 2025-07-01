using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections;

public class SideActionSlot : MonoBehaviour
{
    public Text actionName;
    public Image actionImage;
    public Image progressBar;
    public string Campkey;
    public CampType campType;
    public Camp_Resource_Slot camp_Resource_Slot;
    void Start()
    {
        
    }

    public void SetupSlot(CampActionData slot, string key)
    {
       
        actionName.text = slot.resourceName;
        actionImage.sprite = slot.bgImage;
        Campkey = key;
        campType = slot.campType;   

    }

    public void UpdateProgressBar(float progress)
    {
        progressBar.fillAmount = progress;  // Assuming the slider's range is 0 to 1
    }

    IEnumerator DelayedDestroy()
    {
        yield return null; // waits one frame
        DestroySelf();
    }

    public void RemoveAction()
    {
        if (DataGameManager.instance.currentActiveCamp == campType)
        {
            Debug.Log(camp_Resource_Slot);
            camp_Resource_Slot.DeactivateActionSlot();
        }
        else
        {
            DataGameManager.instance.actionCampHandler.RemoveCampAction(Campkey, campType);
        }

      //  StartCoroutine(DelayedDestroy());
    }

    public void DestroySelf()
    {
        Destroy(gameObject);

    }

}
