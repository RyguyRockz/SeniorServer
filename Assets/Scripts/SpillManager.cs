using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpillManager : MonoBehaviour
{
    public static SpillManager Instance;
    public Transform[] spillSpawnPoints; // Assign in inspector
    public GameObject spillPrefab;
    public float spillDuration = 15f; // Time before spill affects the score
    public int spillPenalty = 5; // Points to subtract if not cleaned

    private List<Spill> activeSpills = new List<Spill>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TrySpawnSpill(float spillChance)
    {
        if (Random.value < spillChance && spillSpawnPoints.Length > 0)
        {
            Transform randomSpawnPoint = spillSpawnPoints[Random.Range(0, spillSpawnPoints.Length)];
            GameObject spill = Instantiate(spillPrefab, randomSpawnPoint.position, Quaternion.identity);

            Spill newSpill = new Spill(spill, spillDuration);
            activeSpills.Add(newSpill);

            StartCoroutine(SpillTimer(newSpill));
        }
    }

    private IEnumerator SpillTimer(Spill spill)
    {
        yield return new WaitForSeconds(spillDuration);

        if (!spill.IsCleaned)
        {
            ScoreManager.Instance.SubtractScore(spillPenalty);
            Debug.Log("Score decreased due to uncleaned spill.");
        }

        activeSpills.Remove(spill);
    }

    // Call this method from another script when a spill is cleaned up
    public void OnSpillCleaned(GameObject spillObject)
    {
        Spill spill = activeSpills.Find(s => s.SpillObject == spillObject);

        if (spill != null)
        {
            spill.IsCleaned = true;
            activeSpills.Remove(spill);
        }
    }

    private class Spill
    {
        public GameObject SpillObject;
        public bool IsCleaned;

        public Spill(GameObject spillObject, float duration)
        {
            SpillObject = spillObject;
            IsCleaned = false;
        }
    }
}
