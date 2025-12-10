using UnityEngine;

public class Sink : MonoBehaviour, IInteractable
{
    [Header("Sink Settings")]
    [SerializeField] private int pointsPerCup = 10;
    
    public bool CanInteract()
    {
        // Can interact if there are empty cups in inventory
        return HasEmptyCupsInInventory();
    }
    
    public void Interact()
    {
        if (InventoryController.Instance == null)
        {
            Debug.LogWarning("No InventoryController found!");
            return;
        }
        
        int cupsWashed = WashEmptyCups();
        
        if (cupsWashed > 0)
        {
            // Add score
            int totalPoints = cupsWashed * pointsPerCup;
            ScoreManager.Instance.AddScore(totalPoints);
            
            // Play sound effect
            SoundEffectManager.Play("Cups");
            
            Debug.Log($"Washed {cupsWashed} cups! Earned {totalPoints} points.");
        }
        else
        {
            Debug.Log("No empty cups to wash!");
        }
    }
    
    private bool HasEmptyCupsInInventory()
    {
        if (InventoryController.Instance == null) return false;
        
        // Check all inventory slots for empty cups
        foreach (Transform slotTransform in InventoryController.Instance.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            
            if (slot != null && slot.currentItem != null)
            {
                EmptyCup emptyCup = slot.currentItem.GetComponent<EmptyCup>();
                
                if (emptyCup != null && emptyCup.startsEmpty)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    private int WashEmptyCups()
    {
        int cupsWashed = 0;
        
        // Go through all inventory slots
        foreach (Transform slotTransform in InventoryController.Instance.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            
            if (slot != null && slot.currentItem != null)
            {
                EmptyCup emptyCup = slot.currentItem.GetComponent<EmptyCup>();
                
                // Check if it's an empty cup
                if (emptyCup != null && emptyCup.startsEmpty)
                {
                    // Clear the slot image
                    UnityEngine.UI.Image slotImage = slot.GetComponent<UnityEngine.UI.Image>();
                    if (slotImage != null)
                    {
                        slotImage.sprite = null;
                    }
                    
                    // Destroy the item
                    Destroy(slot.currentItem);
                    slot.currentItem = null;
                    
                    cupsWashed++;
                }
            }
        }
        
        // Rebuild item counts after removing cups
        if (cupsWashed > 0)
        {
            InventoryController.Instance.RebuildItemCounts();
        }
        
        return cupsWashed;
    }
}