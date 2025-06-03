using UnityEngine;
using UnityEngine.UI;

public class LandDeed_Button : MonoBehaviour
{

    public GameObject landDeedParent_Box;
    public Text landDeedcurrentAmount;

    public void Updatetext()
    {
        landDeedcurrentAmount.text = DataGameManager.instance.CurrentLandDeedsOwned.ToString();
    }

    public void ToggleLandDeed()
    {
        if (DataGameManager.instance.Tutorial_Lists.GetFlag("PurchaseLandDeedUnlocked"))
        {
            if (landDeedParent_Box.activeInHierarchy)

                landDeedParent_Box.SetActive(false);
            else
            {
                landDeedParent_Box.SetActive(true);
                Animator animator = landDeedParent_Box.GetComponent<Animator>();
                animator.Play("IdleState", 0, 0f);
                animator.ResetTrigger("Open");
                animator.SetTrigger("Open");

                LandDeed_Manager parentscript = landDeedParent_Box.GetComponent<LandDeed_Manager>();
                parentscript.SetupDeedBox();
            }
        }
        else
        {
            DataGameManager.instance.Game_Text_Alerts.PlayAlert("Land Deeds are not yet unlocked!");
            Debug.Log("LandDeed not yet unlocked!");
        }
       
    }




}
