using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
   [SerializeField] AudioSource musicSource; // Reference to the AudioSource component for music
   [SerializeField] AudioSource SFXsource; // Reference to the AudioSource component for sound effects

    public AudioClip background; // Reference to the background music clip
    public AudioClip click; 

    public void Start()
    {
        // Play the background music at the start
        musicSource.clip = background;
        musicSource.Play();
    }
    
    internal void PlaySFX(AudioClip clip)
    {
        // Play the specified sound effect
        SFXsource.clip = clip;
        SFXsource.Play();
        throw new NotImplementedException();
    }
}

