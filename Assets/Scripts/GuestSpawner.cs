using UnityEngine;
using System.Collections;

public class GuestSpawner : MonoBehaviour
{
    public GameObject guestPrefab;
    public Transform[] spawnPoints;
    public int numberOfGuests;
    public float spawnDelay;

    private int guestsSpawned = 0; // Track guests that have spawned
    private int guestsLeft = 0;     // Track guests that have left
    public int EndLevelTimer;

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
            guestsSpawned++;
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnGuest()
    {
        if (guestPrefab != null && ExitManager.Instance != null)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject guest = Instantiate(guestPrefab, spawnPoint.position, spawnPoint.rotation);

            if (guest != null) // Ensure the guest was successfully instantiated
            {
                GuestAI guestAI = guest.GetComponentInChildren<GuestAI>();

                if (guestAI != null)
                {
                    Transform exit = ExitManager.Instance.exitPoint;
                    if (exit != null)
                    {
                        guestAI.SetExitPoint(exit); // Set the exit point using the method
                        Debug.Log("Exit point set for guest: " + exit.name);
                        guestAI.OnGuestLeft += HandleGuestLeft;
                    }
                    else
                    {
                        Debug.LogError("Exit point is not available when spawning guest.");
                    }

                    // Confirm guest has spawned, so increment the count
                    ScoreManager.Instance.OnGuestSpawned();
                }
                else
                {
                    Debug.LogError("GuestAI component missing on guest.");
                }
            }
            else
            {
                Debug.LogError("Failed to instantiate guest prefab.");
            }
        }
        else
        {
            Debug.LogError("guestPrefab or ExitManager.Instance is not set.");
        }
    }

    private void HandleGuestLeft()
    {
        guestsLeft++; // Increment the count of guests who have left
        if (guestsLeft == guestsSpawned) // Check if all guests have left
        {
            StartCoroutine(EndLevel()); // Start the coroutine to wait before ending the level
        }
    }

    private IEnumerator EndLevel()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(EndLevelTimer);

        // Trigger the LevelOver UI
        SceneManagment.Instance.ShowLevelOverCanvas();
        FindObjectOfType<LevelEndUI>().ShowPenaltyLog();
    }
}
