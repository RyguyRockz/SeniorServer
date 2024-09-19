using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TimerScript : MonoBehaviour
{
    public float timeLimit = 60f; // Time limit for the level
    private float timer;

    public TMP_Text timerText; // Reference to the UI Text component

    void Start()
    {
        timer = timeLimit;
        UpdateTimerText();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            LoadGameOverScene();
        }
        else
        {
            UpdateTimerText();
        }
    }

    void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOverScene"); // Replace with your actual GameOver scene name
    }

    void UpdateTimerText()
    {
        // Calculate minutes and seconds
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        // Format the time as MM:SS
        timerText.text = string.Format("{0}:{1:D2}", minutes, seconds);
    }
}