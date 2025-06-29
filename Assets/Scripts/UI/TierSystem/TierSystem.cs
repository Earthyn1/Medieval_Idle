using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class TierSystem : MonoBehaviour
{
    public Animator animator;
    public Text campName;
    public Text campTier;
    public Text boostRowNext_1;
    public Text boostRowNext_1_1;
    public Text boostRowNext_2;
    public Text boostRowNext_2_2;
    public Text boostRowNext_3;
    public Text boostRowNext_3_3;
    public Text boostRowNext_4;
    public Text boostRowNext_4_4;
    public Text resource1Amount;
    public Text resource2Amount;
    public Image resource1Image;
    public Image resource2Image;
    public Text goldAmount;
    public Text goldAmount2;
    public CanvasGroup upgradeButton_Canvas;
    public TierSystem_UpgradeButton upgradeButton;
    public bool hasEnoughGold;

    public Text[] boostRowNameTexts;     // for example, 4 UI text elements for boost names
    public Text[] boostRowCurrentTexts;  // 4 UI text elements for current boost values

    private void Awake()
    {
        DataGameManager.instance.tierSystem = this;
        gameObject.SetActive(false);
    }


    public void OpenTierSystem()
    {
        gameObject.SetActive(true);
        animator.Play("IdleState", 0, 0f);
        animator.ResetTrigger("Open");
        animator.SetTrigger("Open");
        DataGameManager.instance.IsTierSystemOpen = true;
        SetupTierPanel();
       
    }

    public void HideTierSystem()
    {
        AnimatorStateInfo stateInfo = DataGameManager.instance.tierSystem.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime < 1f)
        {
 
        }
        else
        {
            animator.Play("IdleState", 0, 0f);
            animator.ResetTrigger("Close");
            animator.SetTrigger("Close");
            DataGameManager.instance.IsTierSystemOpen = false;

        }
    }

    public void SetAsInactive()
    {
        gameObject.SetActive(false);
    }

    public void SetupTierPanel()
    {
        CampType campType = DataGameManager.instance.currentActiveCamp;

        var boost_data = DataGameManager.instance.GetBoostData(campType);

        if (boost_data == null)
        {
            Debug.LogWarning($"No boost data found for {campType}");
            return;
        }

        string formattedCampName = System.Text.RegularExpressions.Regex.Replace(
            campType.ToString(),
            "(\\B[A-Z])",
            " $1"
        );
        campName.text = formattedCampName;
        campTier.text = $"Tier {boost_data.CurrentTier}";

        if (DataGameManager.instance.allCampTiers.TryGetValue(campType, out var campTierDict))
        {
            PopulateBoostRows(boost_data.GetAllBoosts());

            string tierKey = (boost_data.CurrentTier + 1).ToString();

            PopulateTierUpgradePanel(campTierDict, tierKey, boost_data);

            if (campTierDict.TryGetValue(tierKey, out var tierData))
            {
                bool canUpgrade = boost_data.IsResourceComplete(tierData);
                if (canUpgrade)
                {
                    upgradeButton_Canvas.alpha = 1f;
                    upgradeButton.CanUpgrade = true;
                }
                else 
                {
                    upgradeButton_Canvas.alpha = 0.4f;
                    upgradeButton.CanUpgrade = false;
                }
            }
        }
        else
        {
            Debug.LogWarning($"No tier data found for camp type: {campType}");
        }




    }

    void PopulateBoostRows(List<CampBoost_Class> boosts)
    {
        for (int i = 0; i < boosts.Count && i < boostRowNameTexts.Length; i++)
        {
            boostRowNameTexts[i].text = boosts[i].boostName;
            boostRowCurrentTexts[i].text = boosts[i].GetFormattedAmount();
        }
    }
    void PopulateTierUpgradePanel(Dictionary<string, CampTiersArray> campTierDict, string tierKey, ICampBoostData boostData)
    {
        if (campTierDict.TryGetValue(tierKey, out CampTiersArray campTiers))
        {
            // Boost 1
            string formatted = campTiers.boost_1.GetFormattedAmount();
            boostRowNext_1.text = formatted;
            boostRowNext_1_1.text = formatted;

            // Boost 2
            formatted = campTiers.boost_2.GetFormattedAmount();
            boostRowNext_2.text = formatted;
            boostRowNext_2_2.text = formatted;

            // Boost 3
            formatted = campTiers.boost_3.GetFormattedAmount();
            boostRowNext_3.text = formatted;
            boostRowNext_3_3.text = formatted;

            // Boost 4
            formatted = campTiers.boost_4.GetFormattedAmount();
            boostRowNext_4.text = formatted;
            boostRowNext_4_4.text = formatted;

            // Gold cost
            goldAmount.text = campTiers.goldCost.ToString();
            goldAmount2.text = campTiers.goldCost.ToString();

            if(DataGameManager.instance.PlayerGold < campTiers.goldCost)
            {
                goldAmount2.color = Color.red;
                hasEnoughGold = false;
            }
            else
            {
                goldAmount2.color= Color.green;
                hasEnoughGold = true;
            }

            // Resource 1
            resource1Amount.text = $"{boostData.Resource1_Current}/{campTiers.resource1.qty}";
            if (DataGameManager.instance.TryFindItemData(campTiers.resource1.item, out var resource1))
            {
                resource1Image.sprite = resource1.ItemImage;
            }

            // Resource 2
            resource2Amount.text = $"{boostData.Resource2_Current}/{campTiers.resource2.qty}";
            if (DataGameManager.instance.TryFindItemData(campTiers.resource2.item, out var resource2))
            {
                resource2Image.sprite = resource2.ItemImage;
            }
        }
        else
        {
            Debug.LogWarning($"Tier key '{tierKey}' not found in this camp's tier dictionary.");
        }
    }

    void UpgradeBoostsToNextTier(List<CampBoost_Class> boosts, CampTiersArray nextTier)
    {

        List<CampBoost_Class> nextBoosts = new List<CampBoost_Class>
    {
        nextTier.boost_1,
        nextTier.boost_2,
        nextTier.boost_3,
        nextTier.boost_4
    };


        for (int i = 0; i < boosts.Count; i++)
        {
            boosts[i].boostAmount = nextBoosts[i].boostAmount;
        }
    }
    public void OnUpgradeClicked_2()
    {
      //  upgradeButton.CanUpgrade = true;
      //  hasEnoughGold = true;

        if (upgradeButton.CanUpgrade & hasEnoughGold)
        {
            AnimatorStateInfo stateInfo = DataGameManager.instance.tierSystem.animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.normalizedTime < 1f)
            {

            }
            else
            {
                animator.Play("IdleState", 0, 0f);
                animator.ResetTrigger("Upgrade");
                animator.SetTrigger("Upgrade");
            }
        }
    }

    public void UpgradeCampTier()
    {
       
          
        if (upgradeButton.CanUpgrade & hasEnoughGold)
        {
            CampType campType = DataGameManager.instance.currentActiveCamp;

            var boost_data = DataGameManager.instance.GetBoostData(campType);

            if (boost_data == null)
            {
                Debug.LogWarning($"No boost data found for {campType}");
                return;
            }

            string tierKey = (boost_data.CurrentTier + 1).ToString();

            if (DataGameManager.instance.allCampTiers.TryGetValue(campType, out var campTierDict))
            {
                if (campTierDict.TryGetValue(tierKey, out var tierData))
                {

                    UpgradeBoostsToNextTier(boost_data.GetAllBoosts(), tierData);
                }

                boost_data.CurrentTier++;
               
                DataGameManager.instance.PlayerGold = DataGameManager.instance.PlayerGold - tierData.goldCost;
                DataGameManager.instance.topPanelManager.UpdateGold();

                boost_data.ResetResources();

                SetupTierPanel();

                DataGameManager.instance.boostsManager.SetupCampBoosts(campType);
            }
        }
        else
        {
            DataGameManager.instance.Game_Text_Alerts.PlayAlert("Missing upgrade requirements!");
        }
       

    }

   
}
