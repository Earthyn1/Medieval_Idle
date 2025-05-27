using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RaycastPassthroughArea : MonoBehaviour, ICanvasRaycastFilter
{
    public RectTransform passthroughRect;

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (passthroughRect == null)
        {
            Debug.LogWarning("Passthrough Rect not assigned.");
            return false;
        }

        bool inside = RectTransformUtility.RectangleContainsScreenPoint(passthroughRect, screenPoint, eventCamera);
        Debug.Log("Raycast test at point: " + screenPoint + ", Inside highlight area: " + inside);
        return inside;
    }
}
