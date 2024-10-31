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

    // Level scores
    public float LevelScore1 { get; private set; }
    public float LevelScore2 { get; private set; }
    public float LevelScore3 { get; private set; }

    // Current score for the active level
    private float currentLevelScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensure there's only one instance
        }
    }
    
   
    public void InitializeUI(TextMeshProUGUI avgScoreText, TextMeshProUGUI levelScoreText)
    {
        averageScoreText = avgScoreText;
        levelOverScoreText = levelScoreText;

        // initialize them to avoid NullReference
        if (averageScoreText != null) averageScoreText.text = "Score: 0";
        if (levelOverScoreText != null) levelOverScoreText.text = "0 Stars";
    }

    public string GetPenaltySummary()
    {
        return
               $"Guests that waited too long to be seated: {guestsWaitedTooLong}\n" +
               $"Guests that did not order: {guestsDidNotOrder}\n" +
               $"Guests that received the wrong order: {guestsReceivedWrongOrder}\n" +
               $"Guests whose food took too long: {guestsFoodTookTooLong}\n" +
               $"Spills that took too long to clean: {spillsTookTooLong}\n";
    }

    public void AddScore(int score)
    {
        currentLevelScore += score;
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

        if (averageScoreText != null)
        {
            averageScoreText.text = scoreText;
        }
        else
        {
            Debug.LogError(guestScores);

            Debug.LogError("averageScoreText is null!");
        }
        if (levelOverScoreText != null) // Check if levelOverScoreText is assigned
        {
            levelOverScoreText.text = averageScore.ToString("F1") + " Stars";
        }
    }

    // Call this method to start a new level
    public void StartNewLevel()
    {
        
        currentLevelScore = 0; // Reset current score for the new level
        guestScores.Clear(); // Clear guest scores for the new level
        totalNumberOfGuests = 0;// Reset guest count for the new level
    }

   

    // This is for Level Selection Purposes Only
    public float AverageScore()
    {
        if (totalNumberOfGuests == 0) return 0f;
        return guestScores.Sum() / (float)totalNumberOfGuests; // Calculate average score
    }

    public void FinishLevel1()
    {
        // Store the score of Level 1 when it's finished
        LevelScore1 = AverageScore();
    }

    public void FinishLevel2()
    {
        // Store the score of Level 1 when it's finished
        LevelScore2 = AverageScore();
    }

    // Penalty increment methods
    public void IncrementGuestsWaitedTooLong() => guestsWaitedTooLong++;
    public void IncrementGuestsDidNotOrder() => guestsDidNotOrder++;
    public void IncrementGuestsReceivedWrongOrder() => guestsReceivedWrongOrder++;
    public void IncrementGuestsFoodTookTooLong() => guestsFoodTookTooLong++;
    public void IncrementSpillsTookTooLong() => spillsTookTooLong++;


}
