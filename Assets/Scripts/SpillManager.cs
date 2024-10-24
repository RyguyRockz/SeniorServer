using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpillManager : MonoBehaviour
{
    public Transform[] spillSpots; // Assign spill spots in the Inspector

    // Method to get a random spill spot
    public Transform GetRandomSpillSpot()
    {
        if (spillSpots.Length == 0)
        {
            Debug.LogWarning("No spill spots assigned!");
            return null;
        }
        return spillSpots[Random.Range(0, spillSpots.Length)];
    }
}
