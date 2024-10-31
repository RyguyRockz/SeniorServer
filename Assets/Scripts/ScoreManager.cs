using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private List<int> guestScores = new List<int>();
    public TextMeshProUGUI averageScoreText; // Reference to your in-game UI Text
    public TextMeshProUGUI levelOverScoreText; // Reference to your LevelOver UI Text
    public static int totalNumberOfGuests; // Static variable to track total guests spawned

    // Penalty counters
    public int guestsWaitedTooLong = 0;
    public int guestsDidNotOrder = 0;
    public int guestsReceivedWrongOrder = 0;
    public int guestsFoodTookTooLong = 0;
    public int spillsTookTooLong = 0;

    // Singleton implementation for easy access
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure there's only one instance
        }
    }

    public string GetPenaltySummary()
    {
        return 
               $"Guests that waited too long to be seated: {guestsWaitedTooLong}\n" +
               $"Guests that did not order: {guestsDidNotOrder}\n" +
               $"Guests that received the wrong order: {guestsReceivedWrongOrder}\n" +
               $"Guests whose food took too long: {guestsFoodTookTooLong}\n" +
               $"Spills that took too long to clean: {spillsTookTooLong}\n" ;
    }

    public void AddScore(int score)
    {
        guestScores.Add(score);
        UpdateAverageScore();
    }

    public void OnGuestSpawned()
    {
        totalNumberOfGuests++;
    }

    public void SubtractScore(int score)
    {
        guestScores.Remove(score);
        UpdateAverageScore();
        Debug.Log($"Score subtracted: {score}. Total scores: {guestScores.Count}");
    }

    private void UpdateAverageScore()
    {
        if (totalNumberOfGuests == 0) return;

        float averageScore = guestScores.Sum() / (float)totalNumberOfGuests;
        string scoreText = "Score: " + averageScore.ToString("F1");

        // Update both average score texts
        averageScoreText.text = scoreText;
        if (levelOverScoreText != null) // Check if levelOverScoreText is assigned
        {
            levelOverScoreText.text = averageScore.ToString("F1") + " Stars";
        }
    }

    // Penalty increment methods
    public void IncrementGuestsWaitedTooLong() => guestsWaitedTooLong++;
    public void IncrementGuestsDidNotOrder() => guestsDidNotOrder++;
    public void IncrementGuestsReceivedWrongOrder() => guestsReceivedWrongOrder++;
    public void IncrementGuestsFoodTookTooLong() => guestsFoodTookTooLong++;
    public void IncrementSpillsTookTooLong() => spillsTookTooLong++;
}
