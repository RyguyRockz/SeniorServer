using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    private float timer = 0f; // Start timer at 0

    public TMP_Text timerText; // Reference to the UI Text component

    void Start()
    {
        UpdateTimerText();
    }

    void Update()
    {
        timer += Time.deltaTime;
        UpdateTimerText();
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