using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TutorialGroup", menuName = "Tutorial/Group")]
public class TutorialGroupData : ScriptableObject
{
    public string groupId;
    public List<TutorialStepData> steps;
}