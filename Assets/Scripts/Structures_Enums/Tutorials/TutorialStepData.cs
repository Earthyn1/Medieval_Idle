using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Tutorial/Step")]

[System.Serializable]
public class TutorialStepData
{
    public string stepID;
    [TextArea]
    public string instructionText;
    public string NPCName;
    public Sprite NPCImage;

    public string focusObjectName; // e.g. "InventoryButton"
    public bool clickFocusedObject = false; // You can add more conditions later
    public float autoAdvanceDelay = 0f; // Set > 0 to auto-advance
    public ObjectiveData ObjectiveToGive;
    public TutorialFlagID flagToSetID;
    
}


