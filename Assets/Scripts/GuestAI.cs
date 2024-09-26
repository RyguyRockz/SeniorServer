using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

public class GuestAI : InteractableObject
{
    public Transform exitPoint; // Assign this via SetExitPoint method
    public GameObject chatBubblePrefab;
    public float sittingDuration = 5f;
    public float walkSpeed = 3f;

    private NavMeshAgent agent;
    private Transform targetChair;
    private int order; // To store the order (1, 2, or 3)
    private bool hasOrdered = false; // To track if the guest has ordered yet

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
        if (ChairManager.Instance == null || ExitManager.Instance == null)
        {
            Debug.LogError("ChairManager.Instance or ExitManager.Instance is null.");
            yield break;
        }

        while (true)
        {
            targetChair = ChooseRandomChair();
            if (targetChair != null) break;
            Debug.Log("No available chairs. Guest is waiting...");
            yield return new WaitForSeconds(1f); // Wait 1 second before checking again
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
        order = Random.Range(1, 4);

        Debug.Log("Guest ordered item: " + order);

        GameObject chatBubble = Instantiate(chatBubblePrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
        chatBubble.GetComponentInChildren<TextMesh>().text = order.ToString();

        Destroy(chatBubble, 3f);
    }

    private bool IsSeated()
    {
        return targetChair != null && targetChair.GetComponent<Chair>().IsOccupied;
    }

    public bool CheckOrder(int deliveredItem)
    {
        return deliveredItem == order;
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
