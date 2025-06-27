using UnityEngine;
using UnityEngine.EventSystems;

public class TierSystem_UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public CanvasGroup buttonCanvas;
    public bool CanUpgrade = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(CanUpgrade )
        {
            buttonCanvas.alpha = 1f;
            gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CanUpgrade)
        {
            buttonCanvas.alpha = 0.8f;
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        } 
    }


}
