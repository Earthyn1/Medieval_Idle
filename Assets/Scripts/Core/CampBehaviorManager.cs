using System.Collections.Generic;
using UnityEngine;

public class CampBehaviorManager : MonoBehaviour
{
   // public ConstructionCampBehaviorSO constructionCampBehaviorSO;
   // public FishingCampBehaviorSO fishingCampBehaviorSO;
    // Add other SO references here...

    private void Start()
    {
        DataGameManager.instance.campBehaviorManager = this;    
    }

    public void AssignCampBehaviors(Dictionary<CampType, Dictionary<string, CampActionData>> campDictionaries)
    {
        foreach (var campPair in campDictionaries)
        {
            CampSpecificInterface behavior = GetBehaviorForCampType(campPair.Key);
            if (behavior == null)
            {
           //     Debug.LogWarning($"No behavior assigned for CampType {campPair.Key}");
                continue;
            }

            foreach (var kvp in campPair.Value)
            {
                kvp.Value.SetCampSpecificLogic(behavior);
               // Debug.Log($"Assigned {behavior.GetType().Name} to {kvp.Key}");
            }
        }
    }


    private CampSpecificInterface GetBehaviorForCampType(CampType campType)
    {
        return campType switch
        {
        //    CampType.ConstructionCamp => constructionCampBehaviorSO,
         //   CampType.FishingCamp => fishingCampBehaviorSO,
            // Add other camp types here...
            _ => null,
        };
    }
}