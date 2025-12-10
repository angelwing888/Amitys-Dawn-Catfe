using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrderController : MonoBehaviour
{
    public static OrderController Instance { get; private set; }
    public List<OrderProgress> activateOrders = new();
    private OrderUI orderUI;

    private List<string> handinOrderIDs = new();
    
    // Track fulfilled orders count
    private int fulfilledOrdersCount = 0;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        orderUI = FindFirstObjectByType<OrderUI>();
    }

    private void Start() {
        // start after both singletons are initialized
        if (InventoryController.Instance != null) {
            InventoryController.Instance.OnInventoryChanged += CheckInventoryForOrders;
        }
    }

    public void AcceptOrder(Order order) {
        if (IsOrderActive(order.orderID)) return;
        activateOrders.Add(new OrderProgress(order));
        orderUI.UpdateOrderUI();
    }

    public bool IsOrderActive(string orderID) => activateOrders.Exists(o => o.OrderID == orderID);

    public void CheckInventoryForOrders() {
        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

        foreach (OrderProgress order in activateOrders) {
            foreach (OrderObjective orderObjective in order.objectives) {
                if (orderObjective.type != ObjectiveType.MakeDrink) continue;
                if (!int.TryParse(orderObjective.objectiveID, out int itemID)) continue;

                int newAmount = itemCounts.TryGetValue(itemID, out int count) ? Mathf.Min(count, orderObjective.requiredAmount) : 0;

                if (orderObjective.currentAmount != newAmount) {
                    orderObjective.currentAmount = newAmount;
                }
            }
        }

        orderUI.UpdateOrderUI();
    }

    public bool IsOrderCompleted(string orderID) {
        OrderProgress order = activateOrders.Find(q => q.OrderID == orderID);
        return order != null && order.objectives.TrueForAll(o => o.IsCompleted);
    }

    public void HandInOrder(string orderID) {
        // try remove required item
        if (!RemoveRequiredItemsFromInventory(orderID)) {
            // order not completed - missing items
            return;
        }

        // remove order from order log
        OrderProgress order = activateOrders.Find(q => q.OrderID == orderID);
        if (order != null) {
            ScoreManager.Instance.AddScore(20);
            SoundEffectManager.Play("Cups");
            fulfilledOrdersCount++;
            handinOrderIDs.Add(orderID);
            activateOrders.Remove(order);
            orderUI.UpdateOrderUI();
        }
    }

    public bool IsOrderHandedIn(string orderID) {
        return handinOrderIDs.Contains(orderID);
    }

    public bool RemoveRequiredItemsFromInventory(string orderID) {
        OrderProgress order = activateOrders.Find(q => q.OrderID == orderID);
        if (order == null) return false;

        Dictionary<int, int> requiredItems = new();

        // item requirements from objs
        foreach (OrderObjective objective in order.objectives) {
            if (objective.type == ObjectiveType.MakeDrink && int.TryParse(objective.objectiveID, out int itemID)) {
                requiredItems[itemID] = objective.requiredAmount;
            }
        }

        // verify we have items
        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();
        foreach (var item in requiredItems) {
            if (itemCounts.GetValueOrDefault(item.Key) < item.Value) {
                // not enough items to complete quest
                return false;
            }
        }

        // remove required items from inventory
        foreach (var itemRequirement in requiredItems) {
            // RemoveItemsFromInventory
            InventoryController.Instance.RemoveItemsFromInventory(itemRequirement.Key, itemRequirement.Value);
        }

        return true;
    }

    // for when player fails at the end
    public int FailAllOrdersAndCount()
    {
        int failedCount = activateOrders.Count;
        activateOrders.Clear();
        orderUI.UpdateOrderUI();
        return failedCount;
    }
    
    // Get number of fulfilled orders
    public int GetFulfilledOrdersCount()
    {
        return fulfilledOrdersCount;
    }
    
    // Get number of remaining orders
    public int GetRemainingOrdersCount()
    {
        return activateOrders.Count;
    }

}