using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderUI : MonoBehaviour
{
    public Transform orderListContent;
    public GameObject orderEntryPrefab;
    public GameObject objectiveTextPrefab;
    /* public Order testOrder;
    public int testOrderAmount;
    private List<OrderProgress> testOrders = new(); */

    void Start() {
        /* for (int i = 0; i < testOrderAmount; i++) {
            testOrders.Add(new OrderProgress(testOrder));
        } */
        UpdateOrderUI();
    }

    public void UpdateOrderUI() {
        //destroy existing entries
        foreach (Transform child in orderListContent) {
            Destroy(child.gameObject);
        }

        // build order entries
        // foreach(var order in testOrders) {
        foreach(var order in OrderController.Instance.activateOrders) {
            GameObject entry = Instantiate(orderEntryPrefab, orderListContent);
            TMP_Text questNameText = entry.transform.Find("QuestNameText").GetComponent<TMP_Text>();
            Transform objectiveList = entry. transform.Find("ObjectiveList");
            questNameText.text = order.order.orderName; // Use the orderName field instead of name
            foreach(var objective in order.objectives) {
                GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGO.GetComponent<TMP_Text>();
                objText.text = $"{objective.description} ({objective.currentAmount}/{objective.requiredAmount})"; // Brew a Catpuccino
            }
        }
    }
}