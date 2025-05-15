using UnityEngine;
using UnityEngine.Audio;

public class GlobalVolumeLoader : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer; // Reference to the AudioMixer

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Make this GameObject persistent across scenes
    }   

    private void Start()
    {
        if (myMixer == null)
        {
            Debug.LogError("AudioMixer is not assigned in the Inspector!");
            return;
        }

        // Load saved volume settings from PlayerPrefs
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 100);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 100);
        float savedMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 100);

        // Apply the saved values to the AudioMixer
        SetVolume("music", savedMusicVolume);
        SetVolume("sfx", savedSFXVolume);
        SetVolume("master", savedMasterVolume);

        Debug.Log("Global volume settings applied successfully.");
    }

    private void SetVolume(string parameterName, float sliderValue)
    {
        float normalizedValue = Mathf.Clamp(sliderValue / 100, 0.0001f, 1f); // Avoid log(0)
        float volume = Mathf.Log10(normalizedValue) * 20;
        myMixer.SetFloat(parameterName, volume);
        Debug.Log($"Set {parameterName} volume to {volume} dB (Slider: {sliderValue}%)");
    }
}