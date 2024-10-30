using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private List<int> guestScores = new List<int>();
    public TextMeshProUGUI averageScoreText; // Reference to your UI Text
    public static int totalNumberOfGuests; // Static variable to track total guests spawned

    // Singleton implementation for easy access
    public static ScoreManager Instance { get; private set; }
    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure there's only one instance
        }
    }
    public void AddScore(int score)
    {
        guestScores.Add(score);
        UpdateAverageScore();
        Debug.Log($"Score added: {score}. Total scores: {guestScores.Count}");
    }

    public void SubtractScore(int score)
    {
        guestScores.Remove(score);
        UpdateAverageScore();
        Debug.Log($"Score subtracted: {score}. Total scores: {guestScores.Count}");
    }
    public void OnGuestSpawned()
    {
        totalNumberOfGuests++; // Increment total guests when a new guest is instantiated
    }
    private void UpdateAverageScore()
    {
        if (totalNumberOfGuests == 0)
            return;

        float averageScore = guestScores.Sum() / (float)totalNumberOfGuests;
        averageScoreText.text = "Score: " + averageScore.ToString("F1"); // Format to 1 decimal place
    }
}
