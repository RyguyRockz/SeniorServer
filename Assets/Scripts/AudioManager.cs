using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----- Audio Source -----")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

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
    void Start()
    {
        musicSource.clip = GameMusic;
        musicSource.Play();
    }


    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
