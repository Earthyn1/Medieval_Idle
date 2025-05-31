using UnityEngine;
using UnityEngine.UI;

public class Game_Text_Alerts : MonoBehaviour
{
    public Text alertText;
    public Animator alertAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DataGameManager.instance.Game_Text_Alerts = this;
    }

    public void PlayAlert(string AlertText)
    {
        alertText.text = AlertText;
        alertAnimator.Play("IdleState", 0, 0f);
        alertAnimator.ResetTrigger("PlayAlert");
        alertAnimator.SetTrigger("PlayAlert");
    }

 
}
