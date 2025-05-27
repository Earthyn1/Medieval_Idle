using UnityEngine;

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Tutorial/Step")]
public class TutorialStepData : ScriptableObject
{
    public string stepID;
    [TextArea]
    public string instructionText;

    public string focusObjectName; // e.g. "InventoryButton"
    public bool clickFocusedObject = false; // You can add more conditions later
    public float autoAdvanceDelay = 0f; // Set > 0 to auto-advance
}
