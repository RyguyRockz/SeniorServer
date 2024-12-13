using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spill : MonoBehaviour
{
    private float spawnTime; // Time when the spill was created

    private void Start()
    {
        spawnTime = Time.time; // Record when the spill is spawned
    }

    public float GetLifetime()
    {
        return Time.time - spawnTime; // Returns how long the spill has existed
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpillManager.Instance.OnSpillCleaned(gameObject);
        }
    }
}
