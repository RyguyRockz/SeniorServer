using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour
{
    public ScoreManager scoreManager; // Reference to the ScoreManager
    public int scoreForPlate = 1; // Score to be added when a plate is destroyed

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            StartCoroutine(DestroyPlateAfterDelay(other.gameObject, 1f));
        }
    }

    private IEnumerator DestroyPlateAfterDelay(GameObject plate, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Call the AddScore method to increase the player's score
        scoreManager.AddScore(scoreForPlate);

        // Destroy the plate
        Destroy(plate);
    }
}