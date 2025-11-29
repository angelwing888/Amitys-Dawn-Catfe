using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EmptyCup : MonoBehaviour, IInteractable
{
    private bool isInteracted = false;
    public int worth = 20;

    [Header("Cup Prefabs")]
    public GameObject emptyCupPrefab; // Assign your empty cup prefab here

    [Header("Cup Settings")]
    public float timeToEmpty = 10f; // seconds before it turns empty

    private bool isEmpty = false;

    void Start()
    {
        // Only start timer if this is in the world, not in inventory UI
        if (!IsInInventory())
        {
            StartCoroutine(ChangeToEmptyAfterDelay());
        }
    }

    private bool IsInInventory()
    {
        // Check if this object is a child of the inventory panel
        Transform current = transform;
        while (current != null)
        {
            if (current.GetComponent<InventoryController>() != null)
                return true;
            current = current.parent;
        }
        return false;
    }

    private IEnumerator ChangeToEmptyAfterDelay()
    {
        yield return new WaitForSeconds(timeToEmpty);
        ChangeToEmpty();
    }

    private void ChangeToEmpty()
    {
        if (isEmpty) return; // already changed
        isEmpty = true;

        // Spawn empty cup prefab at the same position and rotation
        Instantiate(emptyCupPrefab, transform.position, transform.rotation);

        // Destroy this full cup
        Destroy(gameObject);
    }

    public bool CanInteract()
    {
        // Only allow interaction if not already collected or empty
        return !isEmpty && !isInteracted;
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        isInteracted = true;

        SoundEffectManager.Play("Cups");
        // Option 1: Pick up cup and add to inventory (if you have collector logic)
        var inventory = FindFirstObjectByType<InventoryController>();
        if (inventory != null)
        {
            bool added = inventory.AddItem(emptyCupPrefab);
            if (added)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                Debug.Log("Inventory full!");
                isInteracted = false;
                return;
            }
        }
    }
}
