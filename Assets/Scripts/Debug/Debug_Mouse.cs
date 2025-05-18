using UnityEngine;
using UnityEngine.EventSystems;

public class MouseClickDebug : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Left click
        {
            // Check if clicking on a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Get the UI element clicked
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                var results = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (var result in results)
                {
                    Debug.Log("Clicked on UI: " + result.gameObject.name);
                }
            }
            else
            {
                // Raycast for non-UI elements
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Debug.Log("Clicked on: " + hit.collider.name);
                }
            }
        }
    }
}
