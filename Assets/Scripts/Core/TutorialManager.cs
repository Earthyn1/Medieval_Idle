using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dimBackgroundPanel;    // Full-screen semi-transparent dark panel
    public GameObject highlightPanel;        // Panel to highlight UI element
    public Text instructionText;              // UI Text to show instructions

    [Header("Tutorial Steps")]
    public List<TutorialStepData> tutorialSteps;

    private int currentStepIndex = -1;
    private Coroutine autoAdvanceCoroutine;


    void Start()
    {
        DataGameManager.instance.tutorialManager = this;
    }

    public void NextStep()
    {
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        currentStepIndex++;
        if (currentStepIndex >= tutorialSteps.Count)
        {
            EndTutorial();
            return;
        }

        ShowStep(tutorialSteps[currentStepIndex]);
    }

    public void ShowStep(TutorialStepData step)
    {
        // Stop any previous auto-advance coroutine
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        instructionText.text = step.instructionText;

        GameObject focusObject = GameObject.Find(step.focusObjectName);

        if (focusObject == null)
        {
            Debug.LogWarning($"Focus object '{step.focusObjectName}' not found in scene.");
            highlightPanel.SetActive(false);
            dimBackgroundPanel.SetActive(true);
            return;
        }

        RectTransform targetRect = focusObject.GetComponent<RectTransform>();
        if (targetRect == null)
        {
            Debug.LogWarning($"Focus object '{step.focusObjectName}' does not have a RectTransform!");
            highlightPanel.SetActive(false);
            dimBackgroundPanel.SetActive(true);
            return;
        }

        PositionHighlight(targetRect);

        dimBackgroundPanel.SetActive(true);
        highlightPanel.SetActive(true);

        // If this step has an auto-advance delay, start a new coroutine
        if (step.autoAdvanceDelay > 0f)
        {
            autoAdvanceCoroutine = StartCoroutine(AutoAdvanceAfterDelay(step.autoAdvanceDelay));
        }
    }


    private void PositionHighlight(RectTransform target)
    {
        RectTransform highlightRect = highlightPanel.GetComponent<RectTransform>();

        // Set size to match target UI element size (you can add padding here if you want)
        highlightRect.sizeDelta = target.sizeDelta;

        // Set position to match target UI element
        // This assumes both are under the same Canvas/coordinate space
        highlightRect.position = target.position;
    }

    private IEnumerator AutoAdvanceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NextStep();
    }

    public void EndTutorial()
    {
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        dimBackgroundPanel.SetActive(false);
        highlightPanel.SetActive(false);
        instructionText.text = "";
    }

}
