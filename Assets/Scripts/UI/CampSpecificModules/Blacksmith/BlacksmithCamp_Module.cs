using NUnit.Framework.Internal;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using System;

public class BlacksmithCamp_Module : MonoBehaviour , CampUISlotInterface
{

    private BlacksmithCampFuelData fuelData;
    public string campid;
    public Text fuelAmount;
    private Camp_Resource_Slot parentSlot;
    public Animator animator;




    private void Awake()
    {
        GameObject parentObject = transform.parent.parent.gameObject;
        parentSlot = parentObject.GetComponent<Camp_Resource_Slot>();
        campid = parentSlot.slotkey;

        if (DataGameManager.instance.blacksmithCampModuleData.TryGetValue(campid, out BlacksmithCampFuelData blacksmithData))
        {
            fuelData = blacksmithData;
            fuelAmount.text = fuelData.fuelRequired.ToString();
        }
    }

    public void OnUISlotLoad(string campid)
    {

    }

    void Update() //Updates any UI stuff!
    {
    
    }

    public void OnUISlotUpdate(string campid)
    {
        if (parentSlot == null)
        {
            return;
        }

        if (!parentSlot.isLocked)
        {
            if (DataGameManager.instance.currentBlacksmithFuel < fuelData.fuelRequired)
            {
                fuelAmount.color = Color.red;
            }
            else
            {
                fuelAmount.color = Color.green;
            }
        }     
    }

    public void OnUISlotSingleCall()
    {
        Debug.Log(Environment.StackTrace); // or System.Diagnostics.StackTrace for fine control
       
        animator.Play("IdleState", 0, 0f);
        animator.ResetTrigger("PlayAnimation");
        animator.SetTrigger("PlayAnimation");

    }
}


