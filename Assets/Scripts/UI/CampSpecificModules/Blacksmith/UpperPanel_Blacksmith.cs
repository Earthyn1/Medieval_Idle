using UnityEngine;
using UnityEngine.UI;

public class UpperPanel_Blacksmith : MonoBehaviour
{
    public Image progressBarFuel;
    public Text progressBarText;
    public DropDownMenu menu;

    public void ResetDropDownMenu()
    {
        menu.SetasDefault();
    }

    public void SetupFuelBar()
    {
        int currentFuel = DataGameManager.instance.currentBlacksmithFuel;
        int maxFuel = DataGameManager.instance.maxBlacksmithFuel;

        // Safety check to avoid divide-by-zero
        float fillAmount = (maxFuel > 0) ? (float)currentFuel / maxFuel : 0f;

        // Set progress bar fill
        progressBarFuel.fillAmount = fillAmount;

        // Set progress text
        progressBarText.text = $"{currentFuel}/{maxFuel}";
    }

    public void UpdateFuelBar()
    {

    }

}
