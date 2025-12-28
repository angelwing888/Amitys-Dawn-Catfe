using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;


public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;

    public static InventoryController Instance { get; private set; }
    Dictionary<int, int> itemsCountCache = new();
    public event Action OnInventoryChanged;
    
    void Awake() // for singleton implementation
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();
        
        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
            /*
            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
            }
            */
        }
        
    }

    public void RebuildItemCounts() {
        itemsCountCache.Clear();
        
        foreach (Transform slotTransform in inventoryPanel.transform) {
            Slot slot = slotTransform.GetComponent<Slot>();
            
            if (slot != null && slot.currentItem != null) {
                Item item = slot.currentItem.GetComponent<Item>();
                
                if (item != null) {
                    itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + item.quantity;
                }
            }
        }
        
        OnInventoryChanged?.Invoke(); // FIX: Use ?. to avoid null reference if no listeners
    }

    public Dictionary<int, int> GetItemCounts() => itemsCountCache;

    public bool AddItem(GameObject itemPrefab)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                
                // Disable the EmptyCup script so it doesn't run in inventory
                EmptyCup emptyCupScript = newItem.GetComponent<EmptyCup>();
                if (emptyCupScript != null)
                {
                    emptyCupScript.enabled = false;
                }
                
                // assign sprite to the slot's Image
                Item itemComp = newItem.GetComponent<Item>();
                Image slotImage = slot.GetComponent<Image>();
                if (itemComp != null && slotImage != null)
                    slotImage.sprite = itemComp.icon;
                
                RebuildItemCounts();
                return true;
            }
        }
        
        Debug.Log("Inventory is full!");
        return false;
    }

    public void RemoveItemsFromInventory(int itemID, int amountToRemove) {
    foreach(Transform slotTransform in inventoryPanel.transform) {
        if (amountToRemove <= 0) break;
        
        Slot slot = slotTransform.GetComponent<Slot>();
        if (slot?.currentItem?.GetComponent<Item>() is Item item && item.ID == itemID) {
            int removed = Mathf.Min(amountToRemove, item.quantity);
            item.RemoveFromStack(removed);
            amountToRemove -= removed;

            if (item.quantity == 0) {
                Destroy(slot.currentItem);
                slot.currentItem = null;
                
                // Clear the slot image
                Image slotImage = slot.GetComponent<Image>();
                if (slotImage != null) {
                    slotImage.sprite = null;
                }
            }
        }
    }

    RebuildItemCounts();
}

}
