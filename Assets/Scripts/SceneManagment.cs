using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagment : MonoBehaviour
{
    public GameObject terminalCanvas;
    public GameObject pauseMenuUI; // The Pause Menu Canvas
    public GameObject controlsMenuUI; // The Controls Menu Canvas
    public GameObject LevelSelectUI; // Level select Canvas
    public GameObject MainMenuUI;
    private bool isPaused = false;

    void Update()
    {
        // Check if the player presses the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Pauses the game
    public void Pause()
    {
        pauseMenuUI.SetActive(true);  // Show pause menu
        Time.timeScale = 0f;  // Freeze the game
        isPaused = true;
    }

    // Resumes the game
    public void Resume()
    {
        pauseMenuUI.SetActive(false);  // Hide pause menu
        controlsMenuUI.SetActive(false);  // Hide controls menu if it's open
        Time.timeScale = 1f;  // Unfreeze the game
        isPaused = false;
    }

    // Opens the Controls Menu
    public void OpenControlsMenu()
    {
        pauseMenuUI.SetActive(false);  // Hide the pause menu
        controlsMenuUI.SetActive(true);  // Show controls menu
    }

    // Opens the Level Menu
    public void OpenLevelSelect()
    {
        MainMenuUI.SetActive(false);  // Hide the pause menu
        LevelSelectUI.SetActive(true);  // Show controls menu
    }

   


    // Goes back to the Pause Menu from Controls Menu
    public void BackToPauseMenu()
    {
        controlsMenuUI.SetActive(false);  // Hide controls menu
        pauseMenuUI.SetActive(true);  // Show pause menu
    }

    // PlayGame function - Ensure the game starts unpaused
    public void PlayGame()
    {
        Time.timeScale = 1f;  // Ensure the game is unpaused when starting
        SceneManager.LoadScene("Scene1"); // Replace with your actual game scene name
    }

    // QuitGame function - unchanged
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Back to main menu - Ensure time scale is reset
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;  // Ensure the game is unpaused when returning to the menu
        SceneManager.LoadScene("Main Menu"); // Replace with your actual main menu scene name
    }

    public void Exit()
    {
        if (terminalCanvas != null)
        {
            terminalCanvas.SetActive(false); // Deactivate the canvas
        }
    }
}
