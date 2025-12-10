using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    private DialogueController dialogueUI;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private enum OrderState { NotStarted, InProgress, Completed }
    private OrderState orderState = OrderState.NotStarted;

    private void Start() {
        dialogueUI = DialogueController.Instance;
    }
    public bool CanInteract() {
        return true;
    }

    public void Interact() {
        // if no dialogue data or game is paused with no dialogue active
        if (dialogueData == null || (PauseController.IsGamePaused && (!isDialogueActive))) {
            return;
        }

        if (isDialogueActive) {
            NextLine();
        }

        else {
            StartDialogue();
        }
    }

    void StartDialogue() {

        // sync with order data
        SyncOrderState();
        // set dialogue based on orderState
        if (orderState == OrderState.NotStarted) {
            dialogueIndex = 0;
        }

        else if (orderState == OrderState.InProgress) {
            dialogueIndex = dialogueData.orderInProgressIndex;
        }

        else if (orderState == OrderState.Completed) {
            dialogueIndex = dialogueData.orderCompletedIndex;
        }


        isDialogueActive = true;

        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);
        PauseController.SetPause(true);
        
        // Tell the manager which NPC is talking
        DialogueManager.StartDialogue(this);
        
        if (dialogueData.voiceSound != null)
        {
            SoundEffectManager.PlayVoice(dialogueData.voiceSound, dialogueData.voicePitch);
        }

        DisplayCurrentLine();
    }

    private void SyncOrderState() {
        if (dialogueData.order == null) {
            return;
        }
        
        string orderID = dialogueData.order.orderID;
        
        // Only check if already handed in, not if completed
        if (OrderController.Instance.IsOrderHandedIn(orderID)) {
            orderState = OrderState.Completed;
            return;
        }

        if (OrderController.Instance.IsOrderActive(orderID)) {
            orderState = OrderState.InProgress;
        }
        else {
            orderState = OrderState.NotStarted;
        }
    }

    void NextLine() {
        //Debug.Log($"NextLine called. Current dialogueIndex: {dialogueIndex}, isTyping: {isTyping}");
        
        if (isTyping) {
            // skip typing animation and show full line
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
            return;
        }
        
        // clear choices
        dialogueUI.ClearChoices();
        
        // check EndDialogue lines
        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex]) {
            //Debug.Log($"Ending dialogue at index {dialogueIndex}");
            EndDialogue();
            return;
        }
        
        // check if choices & display
        foreach(DialogueChoice dialogueChoice in dialogueData.choices) {
            //Debug.Log($"Checking choice at dialogueIndex {dialogueChoice.dialogueIndex} vs current {dialogueIndex}");
            if (dialogueChoice.dialogueIndex == dialogueIndex) {
                //Debug.Log($"Displaying choices at index {dialogueIndex}");
                DisplayChoices(dialogueChoice);
                return;
            }
        }
        
        //Debug.Log($"No choices found, incrementing from {dialogueIndex} to {dialogueIndex + 1}");
        
        if (++dialogueIndex < dialogueData.dialogueLines.Length) {
            // type line if another line
            DisplayCurrentLine();
        }
        else {
            EndDialogue();
        }
    }

    IEnumerator TypeLine() {
        isTyping = true;
        dialogueUI.SetDialogueText("");
        
        foreach (char letter in dialogueData.dialogueLines[dialogueIndex]) {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex]) {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }
    
    void DisplayChoices(DialogueChoice choice) {
        //Debug.Log($"DisplayChoices called with {choice.choices.Length} choices");
        for (int i = 0; i < choice.choices.Length; i++) {
            int nextIndex = choice.nextDialogueIndexes[i];
            bool givesOrder = choice.givesOrder[i];
            //Debug.Log($"Creating button: '{choice.choices[i]}' -> index {nextIndex}");
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex, givesOrder));
        }
    }

    void ChooseOption(int nextIndex, bool givesOrder) {
        //Debug.Log($"ChooseOption called: nextIndex={nextIndex}, givesOrder={givesOrder}");
        
        if (givesOrder) {
            //Debug.Log($"Accepting order: {dialogueData.order.orderName}");

            OrderController.Instance.AcceptOrder(dialogueData.order);
            orderState = OrderState.InProgress;
        }
        
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    void DisplayCurrentLine() {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }
    
    public void EndDialogue() {
        if (orderState == OrderState.InProgress && !OrderController.Instance.IsOrderHandedIn(dialogueData.order.orderID)) {
            // HandleOrderCompletion
            HandleOrderCompletion(dialogueData.order);
        }

        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);
    }

    void HandleOrderCompletion(Order order) {
        OrderController.Instance.HandInOrder(order.orderID);
    }
}
