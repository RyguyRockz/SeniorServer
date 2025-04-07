using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required to work with scenes

public class AudioManager : MonoBehaviour
{
    [Header("----- Audio Source -----")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("----- Audio Clip -----")]
    public AudioClip MenuMusic;
    public AudioClip GameMusic;
    public AudioClip DoorOpenSFX;
    public AudioClip PickUpFoodSFX;
    public AudioClip DropFoodSFX;
    public AudioClip TerminalOpenSFX;
    public AudioClip TerminalButtonSFX;
    public AudioClip TerminalConfirmButtonSFX;
    public AudioClip PlateDestoryedSFX;
    public AudioClip CleanSpillSFX;
    public AudioClip GuestLeavingMadSFX;
    public AudioClip MenuBackButtonSFX;
    public AudioClip MenuStartButtonSFX;

    private string currentScene; // Track the current scene to avoid redundant updates

    void Start()
    {
        // Ensure the music plays when the scene starts
        PlayMusicForCurrentScene();

        // Subscribe to scene change events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForCurrentScene();
    }

    private void PlayMusicForCurrentScene()
    {
        currentScene = SceneManager.GetActiveScene().name;
        AudioListener.volume = 0.1f; //Manually lowers all sounds
        if (currentScene == "Main Menu")
        {
            // Play menu music
            if (musicSource.clip != MenuMusic)
            {
                musicSource.clip = MenuMusic;
                musicSource.Play();
            }
        }
        else
        {
            // Play game music for levels
            if (musicSource.clip != GameMusic)
            {
                musicSource.clip = GameMusic;
                musicSource.Play();
            }
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed to prevent errors
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
