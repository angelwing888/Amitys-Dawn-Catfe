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
    
    [Header("Order Spawning")]
    [SerializeField] private Transform drinkSpawnPoint; // NEW: Where drinks spawn (on table in front of NPC)

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
            EndDialogue();
            return;
        }
        
        // check if choices & display
        foreach(DialogueChoice dialogueChoice in dialogueData.choices) {
            if (dialogueChoice.dialogueIndex == dialogueIndex) {
                DisplayChoices(dialogueChoice);
                return;
            }
        }
        
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
        for (int i = 0; i < choice.choices.Length; i++) {
            int nextIndex = choice.nextDialogueIndexes[i];
            bool givesOrder = choice.givesOrder[i];
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex, givesOrder));
        }
    }

    void ChooseOption(int nextIndex, bool givesOrder) {
        if (givesOrder) {
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
            // Try to hand in the order
            bool wasSuccessful = TryHandInOrder(dialogueData.order);
            
            // Only spawn drinks if hand-in was successful
            if (wasSuccessful)
            {
                SpawnOrderDrinks(dialogueData.order);
            }
        }

        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);
    }

    // Modified to return success status
    bool TryHandInOrder(Order order) {
        // Check if order can be completed (has all items)
        if (OrderController.Instance.IsOrderCompleted(order.orderID))
        {
            OrderController.Instance.HandInOrder(order.orderID);
            return true; // Successfully handed in
        }
        
        return false; // Not enough items
    }

    // Remove this method since not using it anymore
    /*
    void HandleOrderCompletion(Order order) {
        OrderController.Instance.HandInOrder(order.orderID);
        SpawnOrderDrinks(order);
    }
    */
    
    // spawn drink prefabs based on order objectives
    void SpawnOrderDrinks(Order order)
    {
        if (drinkSpawnPoint == null)
        {
            Debug.LogWarning($"No drink spawn point set for NPC: {dialogueData.npcName}");
            return;
        }
        
        foreach (OrderObjective objective in order.objectives)
        {
            if (objective.type == ObjectiveType.MakeDrink)
            {
                // Get the item ID from the objective
                if (int.TryParse(objective.objectiveID, out int itemID))
                {
                    // Find the drink prefab with this ID
                    GameObject drinkPrefab = FindDrinkPrefabByID(itemID);
                    
                    if (drinkPrefab != null)
                    {
                        // Spawn the required amount
                        for (int i = 0; i < objective.requiredAmount; i++)
                        {
                            Vector3 spawnOffset = new Vector3(i * 0.3f, 0, 0); // Offset multiple drinks slightly
                            GameObject spawnedDrink = Instantiate(drinkPrefab, drinkSpawnPoint.position + spawnOffset, Quaternion.identity);
                            
                            // Make sure the EmptyCup script is enabled on the spawned drink
                            EmptyCup emptyCup = spawnedDrink.GetComponent<EmptyCup>();
                            if (emptyCup != null)
                            {
                                emptyCup.enabled = true;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find drink prefab for item ID: {itemID}");
                    }
                }
            }
        }
    }
    
    // NEW: Helper to find drink prefab by item ID
    GameObject FindDrinkPrefabByID(int itemID)
    {
        // Search through InventoryController's itemPrefabs
        if (InventoryController.Instance != null)
        {
            foreach (GameObject prefab in InventoryController.Instance.itemPrefabs)
            {
                Item item = prefab.GetComponent<Item>();
                if (item != null && item.ID == itemID)
                {
                    return prefab;
                }
            }
        }
        
        return null;
    }
}