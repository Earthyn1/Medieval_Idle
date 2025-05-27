using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dimBackgroundPanel;    // Full-screen semi-transparent dark panel
    public GameObject highlightPanel;        // Panel to highlight UI element
    public Text instructionText;              // UI Text to show instructions
    public Button textboxButton;
    public GameObject textBoxParent;
    public Text clickToContinueText;
    public UnityAction listener;

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

        if (step.focusObjectName != "") 
        {
            highlightPanel.SetActive(true);

            GameObject focusObject = GameObject.Find(step.focusObjectName);
            if (focusObject == null)
            {
                Debug.LogWarning($"Focus object '{step.focusObjectName}' not found in scene.");
                highlightPanel.SetActive(false);
                dimBackgroundPanel.SetActive(true);
                //return;
            }

            RectTransform targetRect = focusObject.GetComponent<RectTransform>();
            if (targetRect == null)
            {
                Debug.LogWarning($"Focus object '{step.focusObjectName}' does not have a RectTransform!");
                highlightPanel.SetActive(false);
                dimBackgroundPanel.SetActive(true);
                // return;
            }

            PositionHighlight(targetRect);

            // ✅ Disable all buttons except the one we’re highlighting
            Button focusButton = focusObject.GetComponent<Button>();
            if (focusButton != null)
            {
                DisableAllButtonsExcept(focusButton);
            }
            else
            {
                Debug.LogWarning($"Focus object '{step.focusObjectName}' does not have a Button component.");
                DisableAllButtonsExcept(null); // disable all buttons if none are allowed
            }

            //Add listener for when button pressed
            listener = () => OnTutorialButtonClicked(focusButton);
            focusButton.onClick.AddListener(listener);
        }
        else
        {
            highlightPanel.SetActive(false);
        }

        if (step.clickFocusedObject == true)
        {
            clickToContinueText.text = "";
            textboxButton.interactable = false;
        }
        else
        {
            clickToContinueText.text = "Click to continue";
            textboxButton.interactable = true;
        }

        dimBackgroundPanel.SetActive(true);
      

       
        // If this step has an auto-advance delay, start a new coroutine
        if (step.autoAdvanceDelay > 0f)
        {
            autoAdvanceCoroutine = StartCoroutine(AutoAdvanceAfterDelay(step.autoAdvanceDelay));
        }
    }


    void OnTutorialButtonClicked(Button button)
    {    
        Debug.Log("Tutorial step completed!");
      
        NextStep();
        button.onClick.RemoveListener(listener);
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
        EnableAllTutorialButtons();
        textBoxParent.SetActive(false);
        dimBackgroundPanel.SetActive(false);
        highlightPanel.SetActive(false);
        instructionText.text = "";
        currentStepIndex = -1;
    }

    public void DisableAllButtonsExcept(Button allowedButton)
    {
        GameObject[] buttonObjects = GameObject.FindGameObjectsWithTag("Active_Buttons");
        List<Button> allButtons = new List<Button>();

        // Convert GameObjects to Buttons and add to list
        foreach (GameObject obj in buttonObjects)
        {
            Button btn = obj.GetComponent<Button>();
            if (btn != null)
                allButtons.Add(btn);
        }

        // Now disable/enable
        foreach (Button btn in allButtons)
        {
            btn.interactable = (btn == allowedButton);
            Debug.Log($"{btn.gameObject.name} interactable set to {btn.interactable}");
        }
    }


    public void EnableAllTutorialButtons()
    {
        GameObject[] tutorialButtons = GameObject.FindGameObjectsWithTag("Active_Buttons");

        foreach (GameObject obj in tutorialButtons)
        {
            Button btn = obj.GetComponent<Button>();
            if (btn != null)
            {
                btn.interactable = true;
            }
        }
    }

    public void SetupTutorial(List<TutorialStepData> TutorialSteps)
    {
        tutorialSteps = TutorialSteps;
        textBoxParent.SetActive(true);
        NextStep();
    }


}
