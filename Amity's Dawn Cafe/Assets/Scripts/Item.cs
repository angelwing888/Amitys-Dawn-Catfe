using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour
{
    public int ID;
    public string itemName;
    public string uniqueID;   // runtime unique ID
    public Sprite icon;
    public int quantity = 1;

    private void Awake()
    {
        // Generate a unique ID when the item is instantiated
        uniqueID = GlobalHelper.GenerateUniqueID(gameObject);
    }

    public void RemoveFromStack(int amount)
    {
        quantity -= amount;
        if (quantity < 0) quantity = 0;
    }

}
