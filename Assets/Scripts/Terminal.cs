using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Terminal : InteractableObject
{
    public GameObject terminalCanvas;
    public TextMeshProUGUI orderText; // TextMeshPro for displaying order

    private List<string> currentOrder = new List<string>();

    // Prefab references for food and drinks
    public GameObject foodPrefab1;
    public GameObject foodPrefab2;
    public GameObject foodPrefab3;
    public GameObject drinkPrefab1;
    public GameObject drinkPrefab2;
    public GameObject drinkPrefab3;

    // Target objects where the food/drinks should spawn
    public Transform foodTarget1;
    public Transform foodTarget2;
    public Transform foodTarget3;
    public Transform drinkTarget1;
    public Transform drinkTarget2;
    public Transform drinkTarget3;

    // Delays for each food or drink item
    public float food1Delay = 1f;
    public float food2Delay = 2f;
    public float food3Delay = 3f;
    public float drink1Delay = 1.5f;
    public float drink2Delay = 2.5f;
    public float drink3Delay = 3.5f;

    public override void Interact()
    {
        if (terminalCanvas != null)
        {
            terminalCanvas.SetActive(true); // Show the terminal UI
        }
    }

    // Called when food button is pressed
    public void AddFoodToOrder(string foodName)
    {
        currentOrder.Add(foodName);
        UpdateOrderText();
    }

    // Clear the order
    public void ClearOrder()
    {
        currentOrder.Clear();
        UpdateOrderText();
    }

    // Confirm the order and spawn the corresponding prefabs with unique delays
    public void ConfirmOrder()
    {
        // Create a copy of the current order to avoid modifying the list during iteration
        List<string> orderToProcess = new List<string>(currentOrder);

        StartCoroutine(SpawnOrderWithDelay(orderToProcess));

        // Clear the original order and display "Confirmed!" in the text box
        currentOrder.Clear();
        orderText.text = "Confirmed!";
    }

    // Coroutine to spawn the items with their corresponding delays
    private IEnumerator SpawnOrderWithDelay(List<string> orderToProcess)
    {
        foreach (string food in orderToProcess)
        {
            float delay = GetDelayForFoodOrDrink(food); // Get the unique delay
            yield return new WaitForSeconds(delay); // Wait for the specific delay

            SpawnFoodOrDrink(food); // Spawn the appropriate prefab
        }
    }

    // Get the delay for each item based on its name
    private float GetDelayForFoodOrDrink(string foodName)
    {
        switch (foodName)
        {
            case "Food 1":
                return food1Delay;
            case "Food 2":
                return food2Delay;
            case "Food 3":
                return food3Delay;
            case "Drink 1":
                return drink1Delay;
            case "Drink 2":
                return drink2Delay;
            case "Drink 3":
                return drink3Delay;
            default:
                return 0f; // Default no delay
        }
    }

    // Spawn the correct prefab above the appropriate target
    private void SpawnFoodOrDrink(string foodName)
    {
        GameObject prefabToSpawn = null;
        Transform target = null;

        switch (foodName)
        {
            case "Food 1":
                prefabToSpawn = foodPrefab1;
                target = foodTarget1;
                Debug.Log("SpawningFood1");
                break;
            case "Food 2":
                prefabToSpawn = foodPrefab2;
                target = foodTarget2;
                break;
            case "Food 3":
                prefabToSpawn = foodPrefab3;
                target = foodTarget3;
                break;
            case "Drink 1":
                prefabToSpawn = drinkPrefab1;
                target = drinkTarget1;
                break;
            case "Drink 2":
                prefabToSpawn = drinkPrefab2;
                target = drinkTarget2;
                break;
            case "Drink 3":
                prefabToSpawn = drinkPrefab3;
                target = drinkTarget3;
                break;
        }

        if (prefabToSpawn != null && target != null)
        {
            Instantiate(prefabToSpawn, target.position + Vector3.up * .3f, Quaternion.identity);
        }
    }

    private void UpdateOrderText()
    {
        orderText.text = string.Join(", ", currentOrder);
    }
}
