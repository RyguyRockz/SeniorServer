using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GuestAI : MonoBehaviour
{
    public Transform exitPoint; // Keep this private

    public float sittingDuration = 5f;
    public float walkSpeed = 3f;

    private NavMeshAgent agent;
    private Transform targetChair;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(GuestRoutine());
    }

    private IEnumerator GuestRoutine()
    {
        if (ChairManager.Instance == null || ExitManager.Instance == null)
        {
            Debug.LogError("ChairManager.Instance or ExitManager.Instance is null. Ensure both are properly initialized.");
            yield break;
        }

        var chairs = ChairManager.Instance.chairs;
        if (chairs == null || chairs.Count == 0)
        {
            Debug.LogError("Chair list is null or empty. Make sure chairs are assigned in ChairManager.");
            yield break;
        }

        targetChair = ChooseRandomChair();
        if (targetChair != null)
        {
            agent.SetDestination(targetChair.position);
            agent.speed = walkSpeed;
            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance < 0.5f);

            yield return new WaitForSeconds(sittingDuration);

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
    }


    public void SetExitPoint(Transform exit)
    {
        exitPoint = exit;
    }

    private Transform ChooseRandomChair()
    {
        var chairs = ChairManager.Instance.chairs;
        if (chairs.Count == 0)
        {
            Debug.LogError("No available chairs found.");
            return null;
        }

        foreach (Transform chair in chairs)
        {
            if (!chair.GetComponent<Chair>().IsOccupied)
            {
                chair.GetComponent<Chair>().IsOccupied = true;
                return chair;
            }
        }
        return null;
    }
}
