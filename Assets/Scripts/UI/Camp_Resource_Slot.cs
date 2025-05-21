using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System;
using System.Linq;
using static UnityEngine.EventSystems.EventTrigger;

public class Camp_Resource_Slot : MonoBehaviour

{
    public CampActionEntry campActionData;
    public int flipflop;
    public Image progressBar;
    public Image FGImage;
    public Image BGImage;
    public Text lvlUnlocked;
    public Text popCount;
    public Text actionName;
    public GameObject requiredResource_Parent;
    public GameObject Unlocked_Panel;
    public GameObject Locked_Panel;
    public bool isActive = false;
    public bool isLocked = true;
 




    public void OnClicked()
    {
        if (isActive)
        {
            // If already active, always allow deactivation regardless of resources/villagers
            DeactivateActionSlot();
            Debug.Log("Deactivated");
        }
        else
        {
            if (isLocked) 
            {
                Debug.Log("Not yet unlocked");
                return; // Early exit as its locked
            }

            // If not active, try to activate
            if (DataGameManager.instance.CurrentVillagerCount - campActionData.CampData.populationCost >= 0)
            {
                if (HasEnoughResources())
                {
                    ActivateActionSlot();
                    return; // Early exit after successful activation
                }
                else
                {
                    Debug.Log("Not enough resources");
                }
            }
            else
            {
                Debug.Log("Not enough villagers available");
            }

            // Activation failed, but slot is not active so no need to deactivate
        }
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

        isActive = false;
    }

    public void ActivateActionSlot()
    {
            Transform parentTransform = progressBar.transform.parent;

            CheckforRequiredResourceRemoval(campActionData);

            campActionData.StartTime = DateTime.Now; //set timer to 0
            DataGameManager.instance.activeCamps.Add(campActionData);
            parentTransform.gameObject.SetActive(true);
            requiredResource_Parent.SetActive(false);

            DataGameManager.instance.CurrentVillagerCount = DataGameManager.instance.CurrentVillagerCount - campActionData.CampData.populationCost;
            DataGameManager.instance.topPanelManager.UpdateTownPopulation();
            isActive = true;
    }

    public bool HasEnoughResources()
    {
        return campActionData.CampData.RequiredItems.All(item =>
            DataGameManager.instance.TownStorage_List.Any(slot =>
                slot.ItemID == item.item && slot.Quantity >= item.qty));
    }


    public void CheckforRequiredResourceRemoval(CampActionEntry entry)
    {
        if (entry.CampData.RequiredItems.Count > 0)
        {
            foreach (SimpleItemData item in entry.CampData.RequiredItems)
            {
                TownStorageManager.RemoveItem(item.item, item.qty);
            }
        }
    }


}
