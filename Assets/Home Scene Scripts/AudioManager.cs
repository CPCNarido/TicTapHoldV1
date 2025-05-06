using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
   [SerializeField] AudioSource musicSource; // Reference to the AudioSource component for music
   [SerializeField] AudioSource SFXsource; // Reference to the AudioSource component for sound effects

    public AudioClip background; // Reference to the background music clip
    public AudioClip click; 

    public static AudioManager instance; // Singleton instance of the AudioManager
    private void Awake()
    {

        if (instance == null)
        {
            instance = this; // Assign the current instance to the singleton instance
            DontDestroyOnLoad(gameObject); // Destroy the duplicate instance if one already exists
        } else{
            Destroy(gameObject);
        }
        
    }
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
    }
}

