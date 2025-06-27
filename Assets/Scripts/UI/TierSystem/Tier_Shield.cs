using UnityEngine;
using UnityEngine.UI;

public class Tier_Shield : MonoBehaviour
{
    public Image campImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        DataGameManager.instance.tierShield = this;
       
    }

    public void OnButtonClicked()
    {
        if (DataGameManager.instance.Tutorial_Lists.GetFlag("CanOpenTierSystem"))
        {
            AnimatorStateInfo stateInfo = DataGameManager.instance.tierSystem.animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.normalizedTime < 1f && stateInfo.IsName("TierSystemOpen"))
            {
                Debug.Log("Animalready playing");
            }
            else
            {
                DataGameManager.instance.tierSystem.OpenTierSystem();
            }
        }
        else
        {
            DataGameManager.instance.Game_Text_Alerts.PlayAlert("Tier system not yet unlocked!");
        }
       
       
    }
   
       
    


}

     
   

