using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour
{
    public ScoreManager scoreManager; // Reference to the ScoreManager
    public int scoreForPlate; // Score to be added when a plate is destroyed

    public LayerMask PlateLayer;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object entered sink");
        // Check if the object entering the sink is on the Plate layer
        if (PlateLayer == (PlateLayer | (1 << other.gameObject.layer)))
        {
            Debug.Log("Plate Destroyed in sink");
            StartCoroutine(DestroyPlateAfterDelay(other.gameObject, 1f));
        }
        
    }

    public IEnumerator DestroyPlateAfterDelay(GameObject plate, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Call the AddScore method to increase the player's score
        scoreManager.AddScore(scoreForPlate);
        Debug.Log("Point for putting plate away");
        // Destroy the plate
        Destroy(plate);
    }
}