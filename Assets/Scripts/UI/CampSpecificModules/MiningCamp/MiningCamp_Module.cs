using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class MiningCamp_Module : MonoBehaviour , CampUISlotInterface, IPointerEnterHandler, IPointerExitHandler
{
    public string campid;
    public GameObject veinProgressBar_Parent;
    public Image veinProgressBar;
    private Camp_Resource_Slot parentSlot;
    private Image mainProgressBar = null;
    public GameObject veinTrackerParent;
    public GameObject searchingParent;
    public Text searchingText;
    public Text veinAmountText;
   

    private void Awake()
    {
        GameObject parentObject = transform.parent.parent.gameObject;
        parentSlot = parentObject.GetComponent<Camp_Resource_Slot>();
        campid = parentSlot.slotkey;
        mainProgressBar = parentSlot.progressBar;
        
    }

    private void Start()
    {
       
    }

    public void OnUISlotLoad(string campid)
    {
    }

    void Update() //Updates any UI stuff!
    {
       
        if (!parentSlot.isLocked)
        {
            var miningEntry = FindMiningEntryForSlot(campid);
            if (miningEntry == null || parentSlot == null) return;

            // 🚨 New check: Immediately disable all visuals if inactive
            if (!miningEntry.IsActive)
            {
                veinProgressBar_Parent.SetActive(false);
                mainProgressBar.transform.parent.gameObject.SetActive(false);
                veinTrackerParent.SetActive(false);
                searchingParent.SetActive(false);
                return;
            }

            Transform mainProgressParent = mainProgressBar.transform.parent;

            if (miningEntry.IsSearching)
            {
                float searchProgress = miningEntry.GetSearchProgress();

                searchingParent.SetActive(true);
                veinTrackerParent.SetActive(false);

                // Hide main progress bar, show vein progress bar filling from 0 → 1
                mainProgressParent.gameObject.SetActive(false);
                veinProgressBar_Parent.SetActive(true);
                veinProgressBar.fillAmount = Mathf.Clamp01(searchProgress);
                float elapsedTime = (float)(DateTime.Now - miningEntry.SearchStartTime).TotalSeconds;
                float timeRemaining = Mathf.Max(0f, miningEntry.SearchDuration - elapsedTime);
                searchingText.text = $"Finding Veins...  {timeRemaining:F1}s";

                if (searchProgress >= 1f)
                {
                   
                    // Optionally handle when search completes here
                }
            }
            else
            {
                // Not searching: show main progress bar and vein progress bar
                searchingParent.SetActive(false);
                veinTrackerParent.SetActive(true);
                mainProgressParent.gameObject.SetActive(true);

                veinProgressBar_Parent.SetActive(miningEntry.VeinRemaining > 0);
                veinAmountText.text = miningEntry.VeinRemaining.ToString() + "/" + miningEntry.InitialVeinSize.ToString();
                float veinProgress = (float)miningEntry.VeinRemaining / miningEntry.InitialVeinSize;
                veinProgressBar.fillAmount = veinProgress;

                float progress = miningEntry.GetProgress();
                mainProgressBar.fillAmount = progress;
            }
        }
    }


    MiningActionEntry FindMiningEntryForSlot(string slotKey)
    {
  
        foreach (var entry in DataGameManager.instance.activeCamps)
        {
            if (entry is MiningActionEntry miningEntry && miningEntry.Slot != null)
            {
                if (miningEntry.Slot.slotkey == slotKey)
                    return miningEntry;
            }
        }
        return null;
    }

    public void OnUISlotUpdate(string campid)
    {
     
    }
  

       
    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.instance.Hide();
    }

}
