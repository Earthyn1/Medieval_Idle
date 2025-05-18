using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class CampProgressBar : MonoBehaviour
{
    public Image progressbar;
    public Text percent;
    public Text level;
    public Text xpLeft;

    private void Start()
    {
        XPManager.campProgressBar = this;
    }

    public void UpdateProgressBar(CampType campType)
    {
        float progress = XPManager.GetLevelProgress(campType);
        int camplevel = DataGameManager.instance.campXPDictionaries[campType].currentLevel; 

        level.text = "Level: " + camplevel;
        percent.text = (progress * 100).ToString("F1") + "%";

        int currentxp = DataGameManager.instance.campXPDictionaries[campType].currentXP;
        int campnextlevelxp = XPManager.GetXPForLevel(camplevel+1);
        xpLeft.text = currentxp + "/" + campnextlevelxp;

        progressbar.fillAmount = progress;

     }

}
