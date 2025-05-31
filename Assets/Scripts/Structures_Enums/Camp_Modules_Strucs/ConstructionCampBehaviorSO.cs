using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Camp/ConstructionBehavior")]
public class ConstructionCampBehaviorSO : ScriptableObject, CampSpecificInterface
{
    [SerializeField] private string behaviorName = "Construction Camp Logic";
    [SerializeField] private Color debugColor = Color.green;

    // 🧰 This helper safely grabs the data for the given slot
    protected ConstructionCampModule GetData(string slotKey)
    {
        return DataGameManager.instance.constructionCampModuleData[slotKey];
    }

    public void OnSlotLoad(string slotKey)
    {
        var data = DataGameManager.instance.constructionCampModuleData[slotKey];

        // Do something with data
    }

    public void OnUpdateSlot(string slotKey)
    {

    }

    public bool HasEnoughCampSpecificResources(string slotKey)
    {
        var data = DataGameManager.instance.constructionCampModuleData[slotKey];
        return DataGameManager.instance.CurrentLandDeedsOwned >= data.landDeed;

    }

    public void RemoveCampSpecificResources(string slotKey)
    {
        var data = DataGameManager.instance.constructionCampModuleData[slotKey];
        DataGameManager.instance.CurrentLandDeedsOwned -= data.landDeed;
    }

    public void OnCompletedCampSpecificAction(string slotKey)
    {
        var data = DataGameManager.instance.constructionCampModuleData[slotKey];
        if (data.BuildingIDUnlocked != null)
        {
            if (Enum.TryParse<CampType>(data.BuildingIDUnlocked, out CampType campType))
            {
                
                DataGameManager.instance.SetCampLockedStatus(campType, false);
                Debug.Log(data.BuildingIDUnlocked + "Has been unlocked!");
                DataGameManager.instance.campButtonUpdater.UpdateCampButtonAsUnlocked(campType);
  
            }

            if (data.SingleUseSlot && DataGameManager.instance.constructionCampModuleData.TryGetValue(slotKey, out var module))
            {
                Debug.Log("We set one to complete!!");
               
                DataGameManager.instance.OneSlotUseActions.Add(slotKey, new OneSlotUseActions_Struc(slotKey)); //Add this slot to the OneSlotUse!

                DataGameManager.instance.actionCampHandler.RemoveCampAction(slotKey, CampType.ConstructionCamp);

                if (DataGameManager.instance.currentActiveCamp == CampType.ConstructionCamp)
                {
                    var campDataDict = DataGameManager.instance.GetCampData(CampType.ConstructionCamp); //update the camps visuals to reflect this camp now gone!
                    DataGameManager.instance.populate_Camp_Slots.PopulateSlots(campDataDict);
                }

            }
          
            
        }
    }
}