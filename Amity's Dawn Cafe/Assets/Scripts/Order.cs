using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "Orders/Order")]
public class Order : ScriptableObject
{
    public string orderID;
    public string orderName;
    public string description;
    public List<OrderObjective> objectives;

    // called when scr obj is created
    private void OnValidate() {
        if (string.IsNullOrEmpty(orderID)) {
            orderID = orderName + Guid.NewGuid().ToString();
        }
    }

    [System.Serializable]
    public class OrderObjective {
        public string objectiveID;
        public string description;
        public ObjectiveType type;

        public int requiredAmount;
        public int currentAmount;

        public bool IsCompleted => currentAmount >= requiredAmount;
    }

    public enum ObjectiveType { MakeDrink, WashDishes, PetCat }

    [System.Serializable]

    public class OrderProgress {
        public Order order;
        public List<OrderObjective> objectives;

        public OrderProgress(Order order) {
            this.order = order;
            objectives = new List<OrderObjective>();

            // deep copy order
            foreach (var obj in order.objectives) {
                objectives.Add(new OrderObjective {
                    objectiveID = obj.objectiveID,
                    description = obj.description,
                    type = obj.type,
                    requiredAmount = obj.requiredAmount,
                    currentAmount = 0
                });
            }
        }

    public bool IsCompleted => objectives.TrueForAll(o => o.IsCompleted);
    public string OrderID => order.orderID;

    }

}
