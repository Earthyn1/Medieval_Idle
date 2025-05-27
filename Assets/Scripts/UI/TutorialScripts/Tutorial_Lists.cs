using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Lists : MonoBehaviour
{
    public TutorialManager TutorialManager;
    public List<TutorialStepData> tutorialSteps_Intro;
    void Start()
    {
        TutorialManager = GetComponent<TutorialManager>();
        DataGameManager.instance.Tutorial_Lists = this;
    }

 
}
