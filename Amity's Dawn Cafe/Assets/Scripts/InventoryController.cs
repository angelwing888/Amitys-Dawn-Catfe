using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;
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

                return true;
            }
        }

        Debug.Log("Inventory is full!");
        return false;
    }

}
