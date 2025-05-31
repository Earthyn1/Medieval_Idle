using NUnit.Framework.Internal;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ConstructionCamp_Module : MonoBehaviour , CampUISlotInterface
{

    public GameObject landDeedParent;
    public Text deedRequiredAmount;
    private ConstructionCampModule constructionCampModule;
    public string campid;


    
    private void Awake()
    {
        GameObject parentObject = transform.parent.parent.gameObject;
        Camp_Resource_Slot parentscript = parentObject.GetComponent<Camp_Resource_Slot>();
        campid = parentscript.slotkey;

        if (DataGameManager.instance.constructionCampModuleData.TryGetValue(campid, out ConstructionCampModule module))
        {
            constructionCampModule = module;
          if(module.landDeed == 0)
            {
                landDeedParent.SetActive(false);
            }
            else
            {
                deedRequiredAmount.text = module.landDeed.ToString();
            }
        }
    }

  public void OnUISlotLoad(string campid)
    {

    }

    public void OnUISlotUpdate(string campid)
    {
        if (constructionCampModule != null)
        {
            if (DataGameManager.instance.CurrentLandDeedsOwned < constructionCampModule.landDeed)
            {
                deedRequiredAmount.color = Color.red;
            }
            else
            {
                deedRequiredAmount.color = Color.green;
            }
        }
    }


    public bool HasEnoughCampSpecificResources(string campid)
    {
            return DataGameManager.instance.CurrentLandDeedsOwned >= constructionCampModule.landDeed; 
    }

    public void RemoveCampSpecificResources(string campid)
    {


    }



}
