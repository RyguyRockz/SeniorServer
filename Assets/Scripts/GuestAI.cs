using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

public class GuestAI : InteractableObject
{
    public Transform exitPoint;
    public float sittingDuration = 5f;
    public float walkSpeed = 3f;

    private NavMeshAgent agent;
    private Transform targetChair;
    private int order; // Store the guest's random order
    private bool hasOrdered = false; // Track if the guest has ordered yet

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(GuestRoutine());
    }

    public void SetExitPoint(Transform exit)
    {
        exitPoint = exit;
        Debug.Log("Exit point set to: " + exit.name);
    }

    private IEnumerator GuestRoutine()
    {
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

        Debug.Log("Guest seated.");
        yield return new WaitForSeconds(sittingDuration);

        targetChair.GetComponent<Chair>().IsOccupied = false;

        if (exitPoint != null)
        {
            agent.SetDestination(exitPoint.position);
            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance < 0.5f);
        }
        else
        {
            Debug.LogError("Exit point is not set.");
        }

        Destroy(gameObject);
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

        GameObject orderObject = null;

        // Dynamically load the corresponding prefab from the Resources folder
        switch (order)
        {
            case 1:
                orderObject = Resources.Load<GameObject>("FoodBubble1");
                break;
            case 2:
                orderObject = Resources.Load<GameObject>("FoodBubble2");
                break;
            case 3:
                orderObject = Resources.Load<GameObject>("FoodBubble3");
                break;
            case 4:
                orderObject = Resources.Load<GameObject>("DrinkBubble1");
                break;
            case 5:
                orderObject = Resources.Load<GameObject>("DrinkBubble2");
                break;
            case 6:
                orderObject = Resources.Load<GameObject>("DrinkBubble3");
                break;
        }

        if (orderObject != null)
        {
            GameObject instance = Instantiate(orderObject, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            instance.transform.localScale = new Vector3(1f, 1f, 1f); // Adjust scale if needed
            instance.transform.SetParent(transform);

            Destroy(instance, 3f); // Destroy the 3D object after 3 seconds
        }
    }

    private bool IsSeated()
    {
        return targetChair != null && targetChair.GetComponent<Chair>().IsOccupied;
    }

    public int GetRandomOrder()
    {
        return order; // Allow other classes to access the random order if needed
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
}
