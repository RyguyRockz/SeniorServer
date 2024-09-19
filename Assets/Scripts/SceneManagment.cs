using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagment : MonoBehaviour
{
    // Call this method when the Play Game button is pressed
    public void PlayGame()
    {
        // Load the game scene by name or index
        SceneManager.LoadScene("Scene1"); // Replace with your actual game scene name
    }

    // Call this method when the Quit Game button is pressed
    public void QuitGame()
    {
#if UNITY_EDITOR
        // If we're in the editor, stop playing
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If we're in a build, quit the application
        Application.Quit();
#endif
    }

    // Call this method when the Back to Main Menu button is pressed
    public void BackToMainMenu()
    {
        // Load the main menu scene by name or index
        SceneManager.LoadScene("Main Menu"); // Replace with your actual main menu scene name
    }
}

