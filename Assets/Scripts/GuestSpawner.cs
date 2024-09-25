using UnityEngine;
using System.Collections;

public class GuestSpawner : MonoBehaviour
{
    public GameObject guestPrefab;
    public Transform[] spawnPoints;
    public int numberOfGuests;
    public float spawnDelay;

    private void Start()
    {
        // Start the coroutine to spawn guests
        StartCoroutine(SpawnGuests());
    }

    private IEnumerator SpawnGuests()
    {
        for (int i = 0; i < numberOfGuests; i++)
        {
            SpawnGuest();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnGuest()
    {
        if (guestPrefab != null && ExitManager.Instance != null)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject guest = Instantiate(guestPrefab, spawnPoint.position, spawnPoint.rotation);
            GuestAI guestAI = guest.gameObject.GetComponentInChildren<GuestAI>();//problem

            if (guestAI != null)
            {
                Transform exit = ExitManager.Instance.exitPoint;
                Debug.Log(exit != null);
                Debug.Log(ExitManager.Instance != null);
                if (exit != null)
                {
                    guestAI.SetExitPoint(exit); // Set the exit point using the method
                    Debug.Log("Exit point set for guest: " + exit.name);
                }
                else
                {
                    Debug.LogError("Exit point is not available when spawning guest.");
                }
            }
        }
        else
        {
            Debug.LogError("guestPrefab or ExitManager.Instance is not set.");
        }
    }
}
