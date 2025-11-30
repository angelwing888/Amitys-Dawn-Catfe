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
    [SerializeField] private GameObject failedDrinkPrefab;
    
    [Header("Drink Prefabs")]
    [SerializeField] private List<GameObject> drinkPrefabs;
    
    private List<string> currentIngredients = new List<string>();
    private const int maxIngredients = 3;
    private Dictionary<string, string> recipes = new Dictionary<string, string>();
    private Dictionary<string, GameObject> drinkPrefabMap = new Dictionary<string, GameObject>();
    
    private void Start()
    {
        // Define drink recipes
        recipes.Add("ESP", "Catspresso");
        recipes.Add("ESP,WT,WT", "Black Catfee");
        recipes.Add("ESP,MF,MF", "Catpuccino");
        recipes.Add("ESP,SM", "Catado");
        recipes.Add("ESP,SM,SM", "Cat White");
        recipes.Add("ESP,SM,MF", "Catte Meowcchiato");
        
        // Build prefab map by itemName
        Debug.Log($"=== BUILDING PREFAB MAP === Total prefabs: {drinkPrefabs.Count}");
        
        foreach (GameObject prefab in drinkPrefabs)
        {
            if (prefab == null)
            {
                Debug.LogError("NULL prefab found in drinkPrefabs list!");
                continue;
            }
            
            Debug.Log($"Checking prefab: {prefab.name}");
            
            Item item = prefab.GetComponent<Item>();
            if (item == null)
            {
                Debug.LogError($"Prefab '{prefab.name}' has NO Item component!");
                continue;
            }
            
            Debug.Log($"  - Item component found");
            Debug.Log($"  - itemName = '{item.itemName}'");
            Debug.Log($"  - itemName is null or empty? {string.IsNullOrEmpty(item.itemName)}");
            
            if (!string.IsNullOrEmpty(item.itemName))
            {
                drinkPrefabMap[item.itemName] = prefab;
                Debug.Log($"  ✓ MAPPED: '{item.itemName}' to prefab '{prefab.name}'");
            }
            else
            {
                Debug.LogWarning($"  ✗ SKIPPED: itemName is empty for prefab '{prefab.name}'");
            }
        }
        
        Debug.Log($"=== MAP COMPLETE === Total mapped: {drinkPrefabMap.Count}");
        foreach (var kvp in drinkPrefabMap)
        {
            Debug.Log($"  '{kvp.Key}' → {kvp.Value.name}");
        }
        
        ClearIngredients();
    }
    
    public void AddIngredient(string ingredientCode)
    {
        if (currentIngredients.Count >= maxIngredients)
        {
            recipeOutputText.text = "Too  many  ingredients!";
            return;
        }
        
        currentIngredients.Add(ingredientCode.Trim().ToUpper());
        recipeOutputText.text = "Current: " + string.Join(", ", currentIngredients);
    }
    
    public void BrewDrink()
    {
        if (currentIngredients.Count == 0)
        {
            recipeOutputText.text = "Add  ingredients  first!";
            return;
        }
        
        string key = string.Join(",", currentIngredients);
        Debug.Log($"Brewing with key: {key}");
        
        GameObject drinkPrefab;
        
        if (recipes.ContainsKey(key))
        {
            string drinkName = recipes[key];
            recipeOutputText.text = $"Brewed: {drinkName} ☕";
            
            // Get the prefab for this drink
            if (drinkPrefabMap.ContainsKey(drinkName))
            {
                drinkPrefab = drinkPrefabMap[drinkName];
                Debug.Log($"Found prefab for: {drinkName}");
            }
            else
            {
                Debug.LogWarning($"No prefab found for drink: {drinkName}. Using failed drink.");
                drinkPrefab = failedDrinkPrefab;
            }
        }
        else
        {
            recipeOutputText.text = "Invalid  recipe!";
            drinkPrefab = failedDrinkPrefab;
            Debug.Log($"No recipe found for: {key}");
        }
        
        // Pass the PREFAB to inventory (not an instance)
        bool added = inventoryController.AddItem(drinkPrefab);
        
        if (added)
        {
            Debug.Log("Drink added to inventory successfully!");
        }
        else
        {
            Debug.Log("Failed to add drink - inventory full?");
        }
        
        ClearIngredients();
    }
    
    public void ClearIngredients()
    {
        currentIngredients.Clear();
        recipeOutputText.text = "Time  to  brew!";
    }
}