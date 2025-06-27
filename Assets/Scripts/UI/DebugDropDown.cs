using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastTester : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Raycast hit: " + gameObject.name);
    }
}