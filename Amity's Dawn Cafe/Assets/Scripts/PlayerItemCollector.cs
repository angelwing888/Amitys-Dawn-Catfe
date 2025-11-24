using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerItemCollector : MonoBehaviour, IInteractable
{
    /*
    private InventoryController inventoryController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                // add item inventory
                bool itemAdded = inventoryController.AddItem(collision.gameObject);
                if (itemAdded)
                {
                    Destroy(collision.gameObject);
                }
            }
        }
    }
    */
    private InventoryController inventoryController;
    private Item item;

    private bool isCollected = false;

    void Start()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
        item = GetComponent<Item>();
    }

    public bool CanInteract()
    {
        return !isCollected;
    }

    public void Interact()
    {
        if (isCollected || item == null || inventoryController == null)
            return;

        bool itemAdded = inventoryController.AddItem(gameObject);
        if (itemAdded)
        {
            isCollected = true;
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory full! Could not pick up item.");
        }
    }
}
