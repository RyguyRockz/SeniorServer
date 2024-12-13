using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;



public class SceneManagment : MonoBehaviour
{
    public static SceneManagment Instance { get; private set; }
    public GameObject InGameCanvas;
    public GameObject terminalCanvas;
    public GameObject pauseMenuUI; // The Pause Menu Canvas
    public GameObject controlsMenuUI; // The Controls Menu Canvas
    public GameObject LevelSelectUI; // Level select Canvas
    public GameObject MainMenuUI;
    public GameObject LevelOverCanvas; // Canvas for the end-of-level UI
    private bool isPaused = false;

    public TextMeshProUGUI levelAccessMessage;

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

    public void ShowLevelOverCanvas()
    {
        InGameCanvas.SetActive(false);
        LevelOverCanvas.SetActive(true); // Show the canvas
        Time.timeScale = 0f; // Pause the game
    }

    public void HideLevelOverCanvas()
    {
        LevelOverCanvas.SetActive(false); // Hide the canvas
        Time.timeScale = 1f; // Unpause the game if resuming
    }

    // PlayGame function - Ensure the game starts unpaused
    public void Level1()
    {
        Time.timeScale = 1f;  // Ensure the game is unpaused when starting
        SceneManager.LoadScene("Level1"); // Replace with your actual game scene name
        ScoreManager.Instance.StartNewLevel();
    }

    public void LoadTutorial()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TutorialLevel");
        ScoreManager.Instance.StartNewLevel();
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

    public void CompleteLevel1()
    {
        ScoreManager.Instance.FinishLevel1(); // Capture Level 1 score
        SceneManager.LoadScene("Main Menu");
    }

    public void CompleteLevel2()
    {
        ScoreManager.Instance.FinishLevel2(); // Capture Level 2 score
        SceneManager.LoadScene("Main Menu");
    }

    public void CompleteLevel3()
    {
        ScoreManager.Instance.FinishLevel3(); // Capture Level 2 score
        SceneManager.LoadScene("Main Menu");
    }
    public void Exit()
    {
        if (terminalCanvas != null)
        {
            terminalCanvas.SetActive(false); // Deactivate the canvas
        }
    }

    private IEnumerator ClearLevelAccessMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        levelAccessMessage.text = ""; // Clear the message
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadLevel2(string levelName)
    {
        // Check if the player has enough score from Level 1 to access Level 2
        if (levelName == "Level2" && ScoreManager.Instance.LevelScore1 < 3)
        {
            levelAccessMessage.text = "You need at least 3 stars on Level 1 to access Level 2!"; // Set feedback message
            StartCoroutine(ClearLevelAccessMessageAfterDelay(3f)); // Clear message after 3 seconds
            return; // Prevent loading the level
        }

        // Load the level
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelName);
        ScoreManager.Instance.StartNewLevel();
    }

    public void LoadLevel3(string levelName)
    {
        // Check if the player has enough score from Level 2 to access Level 3
        if (levelName == "Level3" && ScoreManager.Instance.LevelScore2 < 3)
        {
            levelAccessMessage.text = "You need at least 3 stars on Level 2 to access Level 3!"; // Set feedback message
            StartCoroutine(ClearLevelAccessMessageAfterDelay(3f)); // Clear message after 3 seconds
            return; // Prevent loading the level
        }

        // Load the level
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelName);
        ScoreManager.Instance.StartNewLevel();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the scene is one of the levels (Level1, Level2, Level3)
        if (scene.name == "Level1" || scene.name == "Level2" || scene.name == "Level3")
        {
            ScoreManager.Instance.StartNewLevel(false); // Do not reset score for any level transition
        }
        else if (scene.name == "Main Menu")
        {
            ScoreManager.Instance.StartNewLevel(true); // Reset score when returning to the main menu
        }
    }



}
