using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;
using TMPro;

public class GuestAI : InteractableObject
{
    public Transform exitPoint;
    public float sitTimeLimit = 10f; // Time before guest leaves if no order is taken
    public float foodWaitTimeLimit = 20f; // Time before guest leaves if no food is received
    public float eatingDuration = 5f; // Time spent eating food
    public float walkSpeed = 3f;

    private NavMeshAgent agent;
    private Transform targetChair;
    private int order; // Guest's order number
    private bool hasOrdered = false; // Tracks if the guest has ordered
    private bool hasReceivedFood = false; // Tracks if the guest has received food
    private bool isEating = false; // Tracks if guest is currently eating
    private float foodWaitElapsedTime = 0f; // To keep track of elapsed time for food waiting

    public GameObject platePrefab;
    public GameObject currentFoodObject;

    private UIManager uiManager;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        uiManager = FindObjectOfType<UIManager>(); // Find the UIManager in the scene

        // Get the layer indices
        int playerLayer = LayerMask.NameToLayer("playerLayer");
        int guestLayer = gameObject.layer; // This should be set to the appropriate layer in the inspector

        // Check if layers are valid
        if (playerLayer < 0 || playerLayer > 31)
        {
            Debug.LogError("Invalid player layer index: " + playerLayer);
        }

        if (guestLayer < 0 || guestLayer > 31)
        {
            Debug.LogError("Invalid guest layer index: " + guestLayer);
        }

        // Ignore collisions between PlayerLayer and GuestAI layer if valid
        if (playerLayer >= 0 && playerLayer <= 31 && guestLayer >= 0 && guestLayer <= 31)
        {
            Physics.IgnoreLayerCollision(playerLayer, guestLayer);
        }

        StartCoroutine(GuestRoutine());
    }



    private void Update()
    {
        RotateTowardsMovementDirection(); // Update rotation every frame
    }

    private void RotateTowardsMovementDirection()
    {
        // Check if the guest is moving
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            // Find the direction the agent is moving in
            Vector3 direction = agent.velocity.normalized;

            // Calculate the target rotation based on the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate the guest towards the target direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
        }
    }

    public void SetExitPoint(Transform exit)
    {
        exitPoint = exit;
        Debug.Log("Exit point set to: " + exit.name);
    }

    private IEnumerator GuestRoutine()
    {
        // Find a random chair and sit down
        while (true)
        {
            targetChair = ChooseRandomChair();
            if (targetChair != null) break;
            Debug.Log("No available chairs. Guest is waiting...");
            yield return new WaitForSeconds(1f);
        }

        agent.SetDestination(targetChair.position);
        agent.speed = walkSpeed;
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance < 0.5f);

        // Guest has reached the chair, stop the agent
        agent.isStopped = true; // Stop the agent from moving
        agent.updateRotation = false; // Prevent the NavMeshAgent from controlling rotation

        Debug.Log("Guest seated.");

        // Rotate guest to face the table instantly
        Transform guestFacingPoint = targetChair.GetComponent<Chair>().GuestFacingPoint;
        if (guestFacingPoint != null)
        {
            RotateTowardsInstantly(guestFacingPoint.position);
        }
        else
        {
            Debug.LogWarning("Chair does not have a GuestFacingPoint assigned.");
        }

        // Start the timer for order-taking
        yield return StartCoroutine(WaitForOrder());

        // After ordering, wait for food to arrive
        if (hasOrdered)
        {
            yield return StartCoroutine(WaitForFood());

        }

        //yield return new WaitForSeconds(eatingDuration);
        // Leave the restaurant after eating or failing to receive food
        yield return StartCoroutine(LeaveRestaurant());
    }

    private IEnumerator LeaveRestaurant()
    {
        yield return new WaitForSeconds(eatingDuration);
       

        targetChair.GetComponent<Chair>().IsOccupied = false; // Mark the chair as unoccupied
        if (exitPoint != null)
        {
            agent.isStopped = false; // Re-enable the agent to move again
            agent.updateRotation = true; // Allow the agent to control rotation

            agent.SetDestination(exitPoint.position);
            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance < 0.5f);
        }
        else
        {
            Debug.LogError("Exit point is not set.");
        }

        Debug.Log("Guest is leaving!");
        Destroy(gameObject);
    }

    private IEnumerator WaitForOrder()
    {
        float elapsedTime = 0f;
        while (!hasOrdered && elapsedTime < sitTimeLimit)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!hasOrdered)
        {
            Debug.Log("Order not taken fast enough. Guest is leaving.");
            uiManager.ShowText(uiManager.guestLeaveText, "I waited too long! I'm leaving!", 3f);
            yield break; // Stop execution
        }
    }

    private IEnumerator WaitForFood()
    {
        Debug.Log("Waiting for food");
        foodWaitElapsedTime = 0f; // Reset elapsed time
        while (!hasReceivedFood && foodWaitElapsedTime < foodWaitTimeLimit)
        {
            foodWaitElapsedTime += Time.deltaTime; // Update elapsed time
            yield return null;
        }

        if (!hasReceivedFood)
        {
            uiManager.ShowText(uiManager.guestIncorrectFoodText, "I Haven't Gotten My Food! I'm Leaving!", 3f);
            yield break;
        }
    }

    public override void Interact()
    {
        TakeOrder();
    }

    public void TakeOrder()
    {
        if (!IsSeated() || hasOrdered) return;

        hasOrdered = true;
        order = Random.Range(1, 7); // Randomly generates number
        Debug.Log("Guest ordered item: " + order);

        // Spawn the 3D order bubble
        GameObject orderObject = LoadOrderPrefab(order);
        if (orderObject != null)
        {
            GameObject instance = Instantiate(orderObject, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            instance.transform.localScale = new Vector3(1f, 1f, 1f);
            instance.transform.SetParent(transform);

            Destroy(instance, 3f);
        }
    }

    public void ReceiveFood(int deliveredOrder)
    {
        if (!hasOrdered || isEating) return;

        if (deliveredOrder == order)
        {
            Debug.Log("Correct food delivered.");
            hasReceivedFood = true; // Mark as received food

            // Start the eating process after receiving food
            StartCoroutine(EatFood(deliveredOrder == order));
        }
        else
        {
            Debug.Log("Wrong food delivered.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is a food item
        if (other.CompareTag("Pickup"))
        {
            // Check if the food item corresponds to the guest's order
            int deliveredOrder = GetOrderFromFood(other.gameObject);
            Debug.Log("Food detected: " + deliveredOrder);

            if (deliveredOrder != -1)
            {
                Debug.Log("Food detected. Checking if it's correct...");
                currentFoodObject = other.gameObject; // Assign the food object to currentFoodObject

                ReceiveFood(deliveredOrder); // Call ReceiveFood to check if the delivered food matches the order
            }
        }
    }

    // Helper function to map food GameObjects to order numbers
    private int GetOrderFromFood(GameObject food)
    {
        FoodItem foodItem = food.GetComponent<FoodItem>();
        if (foodItem != null)
        {
            return foodItem.foodId;
        }

        return -1; // Return -1 if no FoodItem component is found
    }

    private IEnumerator EatFood(bool correctFood)
    {
        isEating = true;
        Debug.Log("Guest is eating.");
        yield return new WaitForSeconds(eatingDuration); // Wait for the guest to finish eating

        if (correctFood)
        {
            Debug.Log("Guest ate the correct food.");
            uiManager.ShowText(uiManager.guestEatText, "Delicious! I'm satisfied!", 3f);

            // Destroy the food object after eating
            if (currentFoodObject != null)
            {
                Destroy(currentFoodObject);
                Debug.Log("Food object destroyed.");
            }

            // Calculate the position in front of the guest (adjust the forward offset as necessary)
            Vector3 plateSpawnPosition = transform.position + transform.forward * .6f + new Vector3(0 , .1f, 0); // Adjust 0.5f to control the distance from the guest


            // Spawn the plate prefab at the guest's location
            Instantiate(platePrefab, plateSpawnPosition, Quaternion.identity);
            Debug.Log("Plate spawned in front of guest.");
        }
        else
        {
            Debug.Log("Guest ate the wrong food.");
            uiManager.ShowText(uiManager.guestIncorrectFoodText, "This isn't what I ordered!", 3f);
        }

        hasReceivedFood = true;
        isEating = false;
    }

    private GameObject LoadOrderPrefab(int orderNumber)
    {
        switch (orderNumber)
        {
            case 1: return Resources.Load<GameObject>("FoodBubble1");
            case 2: return Resources.Load<GameObject>("FoodBubble2");
            case 3: return Resources.Load<GameObject>("FoodBubble3");
            case 4: return Resources.Load<GameObject>("DrinkBubble1");
            case 5: return Resources.Load<GameObject>("DrinkBubble2");
            case 6: return Resources.Load<GameObject>("DrinkBubble3");
            default: return null;
        }
    }

    private bool IsSeated()
    {
        return targetChair != null && targetChair.GetComponent<Chair>().IsOccupied;
    }

    private Transform ChooseRandomChair()
    {
        var availableChairs = ChairManager.Instance.chairs
            .Where(chair => !chair.GetComponent<Chair>().IsOccupied)
            .ToList();

        if (availableChairs.Count == 0)
        {
            Debug.LogError("No available chairs found.");
            return null;
        }

        Transform selectedChair = availableChairs[Random.Range(0, availableChairs.Count)];
        selectedChair.GetComponent<Chair>().IsOccupied = true;
        return selectedChair;
    }

    private void RotateTowardsInstantly(Vector3 targetPosition)
    {
        // Calculate the direction to the target position
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Directly set the rotation to look at the target position
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;
    }
}
