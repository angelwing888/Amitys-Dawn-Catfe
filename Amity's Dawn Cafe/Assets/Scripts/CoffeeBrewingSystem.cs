using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CoffeeBrewingSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI recipeOutputText;
    [SerializeField] private Button brewButton;

    [Header("Inventory & Items")]
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private GameObject failedDrinkPrefab; // generic failed drink prefab

    [Header("Drink Prefabs")]
    [SerializeField] private List<GameObject> drinkPrefabs; // assign all your drink variants here

    private List<string> currentIngredients = new List<string>();
    private const int maxIngredients = 3;

    // Stores all valid recipes
    private Dictionary<string, string> recipes = new Dictionary<string, string>();
    private Dictionary<string, GameObject> drinkPrefabMap = new Dictionary<string, GameObject>();

    private void Start()
    {
        // Define drink recipes
        recipes.Add("ESP", "Espresso");
        recipes.Add("ESP,WT,WT", "Black Catfee");
        recipes.Add("ESP,MF,MF", "Catpuccino");
        recipes.Add("ESP,SM", "Catado");
        recipes.Add("ESP,SM,SM", "Cat White");
        recipes.Add("ESP,SM,MF", "Catte Meowchiatto");

        // Build prefab map from the list
        foreach (GameObject prefab in drinkPrefabs)
        {
            Item item = prefab.GetComponent<Item>();
            if (item != null && !string.IsNullOrEmpty(item.itemName))
            {
                drinkPrefabMap[item.itemName] = prefab;
            }
        }

        if (inventoryController == null)
            inventoryController = FindFirstObjectByType<InventoryController>();

        if (brewButton != null)
            brewButton.onClick.AddListener(BrewDrink);

        ClearIngredients();
    }

    public void AddIngredient(string ingredientCode)
    {
        if (currentIngredients.Count >= maxIngredients)
        {
            recipeOutputText.text = "Too many ingredients!";
            return;
        }

        currentIngredients.Add(ingredientCode);
        recipeOutputText.text = "Current: " + string.Join(", ", currentIngredients);
    }

    public void BrewDrink()
    {
        if (currentIngredients.Count == 0)
        {
            recipeOutputText.text = "Add ingredients first!";
            return;
        }

        string key = string.Join(",", currentIngredients);
        GameObject brewedItem;

        if (recipes.ContainsKey(key))
        {
            string drinkName = recipes[key];
            recipeOutputText.text = $"Brewed: {drinkName} ☕";
            brewedItem = CreateDrinkItem(drinkName, success: true);
        }
        else
        {
            recipeOutputText.text = "Invalid recipe!";
            brewedItem = CreateDrinkItem("Failed Drink", success: false);
        }

        inventoryController.AddItem(brewedItem);
        ClearIngredients();
    }

    private GameObject CreateDrinkItem(string drinkName, bool success)
    {
        GameObject prefabToSpawn = success && drinkPrefabMap.ContainsKey(drinkName)
                                ? drinkPrefabMap[drinkName]
                                : failedDrinkPrefab;

        GameObject itemObject = Instantiate(prefabToSpawn);

        Item item = itemObject.GetComponent<Item>();
        if (item != null)
        {
            item.itemName = drinkName;
            item.uniqueID = GlobalHelper.GenerateUniqueID(itemObject); // assign unique runtime ID
        }

        itemObject.SetActive(false); // don't leave it floating in the scene
        return itemObject;
    }

    public void ClearIngredients()
    {
        currentIngredients.Clear();
        recipeOutputText.text = "Ready to brew!";
    }
}
