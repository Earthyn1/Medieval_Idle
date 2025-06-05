using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Category_Base : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image Category_Image;
    public Image BG_Image;
    public CampCategoryData CampCategoryData;

    public RectTransform rectTransform;  // Reference to the RectTransform
    public float targetY = 50f;
    private Color black25 = new Color(0, 0, 0, 0.25f);
    private Color black50 = new Color(0, 0, 0, 0.8f);
    private Color black100 = new Color(0, 0, 0, 0.9f);
    private Color white50 = new Color(1, 1, 1, 0.8f);
    private Color white100 = new Color(1, 1, 1, 1f);

    public void SetAsSelected()
    {
        if (DataGameManager.instance.CurrentCategoryUISelected == gameObject)
        {

        }
        else
        {

            Category_Image.color = white100;
            BG_Image.color = black100;

            // Set the anchored position directly to the target Y value
            rectTransform.anchoredPosition = new Vector2(
            rectTransform.anchoredPosition.x, targetY);

            DataGameManager.instance.currentCampCategory = CampCategoryData.campCategory;
            var campDataDict = DataGameManager.instance.GetCampData(CampCategoryData.campType);
            DataGameManager.instance.populate_Camp_Slots.PopulateSlots(campDataDict);

            if (DataGameManager.instance.CurrentCategoryUISelected != null)
            {
                Category_Base UIScript = DataGameManager.instance.CurrentCategoryUISelected.GetComponent<Category_Base>();
                UIScript.SetAsUnSelected();
            }
            DataGameManager.instance.CurrentCategoryUISelected = gameObject;
        }


    }

    public void SetAsUnSelected()
    {
        
            Category_Image.color = white50;
            BG_Image.color = black50;
            // Set the anchored position directly to the target Y value
            rectTransform.anchoredPosition = new Vector2(
            rectTransform.anchoredPosition.x, 0);
        
    }

    public void OnHovered()
    {
        Category_Image.color = white100;
        BG_Image.color = black50;

    }

    public void OnUnHovered()
    {
        if (DataGameManager.instance.CurrentCategoryUISelected != gameObject)
        {
            SetAsUnSelected();
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHovered();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnUnHovered();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      //  SetAsSelected();
    }
}
