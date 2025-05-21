using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections;


public class CampProgressBar : MonoBehaviour
{
    public Image progressbar;
    public Text percent;
    public Text level;
    public Text xpLeft;
    private Coroutine fillCoroutine;


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
        int campnextlevelxp = XPManager.GetXPForLevel(camplevel + 1);
        xpLeft.text = currentxp + "/" + campnextlevelxp;

        // Stop previous fill animation if running
        if (fillCoroutine != null)
            StopCoroutine(fillCoroutine);

        if (gameObject.activeInHierarchy)
        {
            // Start new fill animation
            fillCoroutine = StartCoroutine(AnimateFill(progress));
        }
       
        
    }

    private IEnumerator AnimateFill(float targetFill)
    {
        float startFill = progressbar.fillAmount;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            progressbar.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / duration);
            yield return null;
        }

        progressbar.fillAmount = targetFill;
        fillCoroutine = null;
    }

}
