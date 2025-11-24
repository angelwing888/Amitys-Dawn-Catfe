using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour
{
    public int ID;
    public string itemName;
    public string uniqueID;   // runtime unique ID
    public Sprite icon;

    private void Awake()
    {
        // Generate a unique ID when the item is instantiated
        uniqueID = GlobalHelper.GenerateUniqueID(gameObject);
    }

}
