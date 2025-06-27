using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dimBackgroundPanel;    // Full-screen semi-transparent dark panel
    public GameObject highlightPanel;        // Panel to highlight UI element
    public Text instructionText;              // UI Text to show instructions
    public Button textboxButton;
    public GameObject textBoxParent;
    public Text clickToContinueText;
    Dictionary<Button, UnityAction> tutorialListeners = new Dictionary<Button, UnityAction>();

    public Image npcImage;
    public Text npcName;
    public Text npcName2;

    [Header("Tutorial Steps")]
    public List<TutorialStepData> tutorialSteps;

    private int currentStepIndex = -1;
    private Coroutine autoAdvanceCoroutine;

    private void Update()
    {
        
    }
    void Start()
    {
        DataGameManager.instance.tutorialManager = this;
       
    }

    public void NextStep()
    {
        DisableAllButtonsExcept(null); // disable all buttons if none are allowed
       // highlightPanel.SetActive(false);


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

        npcImage.sprite = step.NPCImage;
        npcName.text = step.NPCName;
        npcName2.text = npcName.text;

        if (step.focusObjectName != "") 
        {
            GameObject focusObject = null;

            highlightPanel.SetActive(true);

            if (step.focusObjectName == "WolfMeat")
            {
                int index = TownStorageManager.FindItemIndexByID(step.focusObjectName);
                Debug.Log("Location of item --" + index);

                string slotname = DataGameManager.instance.populate_Storage_Slots.parentContainer.GetChild(index).name;
                focusObject = GameObject.Find(slotname);
            }
            else
            {
                focusObject = GameObject.Find(step.focusObjectName);
            }

           
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

            if (focusObject.GetComponent<Button>())
            {
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


                // When assigning:
                UnityAction action = () =>
                {
                  //  Debug.Log($"Tutorial button clicked: {focusButton.name}");
                    OnTutorialButtonClicked(focusButton);
                };

                // Add the listener
                focusButton.onClick.AddListener(action);
               // Debug.Log($"Listener added to button: {focusButton.name}");

                // Store in dictionary
                tutorialListeners[focusButton] = action;
               // Debug.Log($"Action stored in tutorialListeners for button: {focusButton.name}");
            } 
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
      
        if (step.ObjectiveToGive != null)
        {
            Objective_Manager.CreateNewObjective(step.ObjectiveToGive);
        }

        if (step.flagToSetID != TutorialFlagID.NA)
        {
            DataGameManager.instance.Tutorial_Lists.CompleteDialogEvent(step.flagToSetID);
            Debug.Log(DataGameManager.instance.Tutorial_Lists.GetFlag(step.flagToSetID.ToString()));
        }

       
        
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

        if (tutorialListeners.TryGetValue(button, out var action))
        {
            Debug.Log("remove listener from" + button);
            button.onClick.RemoveListener(action);
            tutorialListeners.Remove(button);
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
            {
                Debug.Log($"Found button: {btn.gameObject.name}");
                allButtons.Add(btn);
            }
        }

        // Now disable/enable
        foreach (Button btn in allButtons)
        {
            btn.interactable = (btn == allowedButton);
            Debug.Log($"Button {btn.name} set to interactable = {btn.interactable}");
            //  Debug.Log($"{btn.gameObject.name} interactable set to {btn.interactable}");
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

  

    public void SetupTutorial(TutorialGroupData tutorialGroupData)
    {
        // Clean up existing listeners
        foreach (var pair in tutorialListeners)
        {
            if (pair.Key != null)
            {
                pair.Key.onClick.RemoveListener(pair.Value);
            }
        }
        tutorialListeners.Clear();

        Debug.Log(tutorialGroupData.groupId);
        currentStepIndex = -1;
        tutorialSteps = tutorialGroupData.steps;
        textBoxParent.SetActive(true);
        NextStep();
    }


}
