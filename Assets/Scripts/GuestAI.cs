using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

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

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
            Debug.Log("Guest facing table. Target point: " + guestFacingPoint.position);
            RotateTowardsInstantly(guestFacingPoint.position);
            Debug.Log("Guest rotation after applying: " + transform.rotation.eulerAngles);
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

        // Leave the restaurant after eating or failing to receive food
        targetChair.GetComponent<Chair>().IsOccupied = false;
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

        Debug.Log("Guest is stuffed and leaving!");
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
            yield break;
        }
    }

    private IEnumerator WaitForFood()
    {
        float elapsedTime = 0f;
        while (!hasReceivedFood && elapsedTime < foodWaitTimeLimit)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!hasReceivedFood)
        {
            Debug.Log("Did not get food in time. Guest is leaving.");
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
        }
        else
        {
            Debug.Log("Wrong food delivered.");
        }

        StartCoroutine(EatFood(deliveredOrder == order));
    }

    private IEnumerator EatFood(bool correctFood)
    {
        isEating = true;
        Debug.Log("Guest is eating.");
        yield return new WaitForSeconds(eatingDuration);

        if (correctFood)
        {
            Debug.Log("Guest ate the correct food.");
        }
        else
        {
            Debug.Log("Guest ate the wrong food.");
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
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;

        // Optional: Log the desired rotation
        Debug.Log("Rotated towards table at: " + targetPosition);
    }


}
