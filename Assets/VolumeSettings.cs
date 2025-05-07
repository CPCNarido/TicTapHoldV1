using UnityEngine;
using UnityEngine.UI; // Import the Unity UI namespace
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer; // Reference to the AudioMixer
    [SerializeField] private Slider musicSlider; // Reference to the Music Slider component
    [SerializeField] private Slider sfxSlider; // Reference to the SFX Slider component
    [SerializeField] private Slider masterSlider; // Reference to the Master Slider component

    private void Start()
    {
        if (musicSlider == null || sfxSlider == null || masterSlider == null || myMixer == null)
        {
            Debug.LogError("MusicSlider, SFXSlider, MasterSlider, or AudioMixer is not assigned in the Inspector!");
            return;
        }

        // Load saved slider values from PlayerPrefs
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 100); // Default to 100 if not set
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 100); // Default to 100 if not set
        float savedMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 100); // Default to 100 if not set

        // Set slider values to the saved values
        musicSlider.value = savedMusicVolume;
        sfxSlider.value = savedSFXVolume;
        masterSlider.value = savedMasterVolume;

        // Apply the saved values to the AudioMixer
        SetMusicVolume(savedMusicVolume);
        SetSFXVolume(savedSFXVolume);
        SetMasterVolume(savedMasterVolume);

        // Add listeners to update the AudioMixer volume and save the values when sliders change
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    public void SetMusicVolume(float sliderValue)
    {
        // Convert the slider value (0 to 100) to a logarithmic AudioMixer volume (-80 to 0 dB)
        float normalizedValue = Mathf.Clamp(sliderValue / 100, 0.0001f, 1f); // Avoid log(0)
        float volume = Mathf.Log10(normalizedValue) * 20;
        myMixer.SetFloat("music", volume); // Set the volume in the AudioMixer
        PlayerPrefs.SetFloat("MusicVolume", sliderValue); // Save the slider value
        PlayerPrefs.Save(); // Ensure the value is saved immediately
        Debug.Log($"Music volume set to: {volume} dB (Slider: {sliderValue}%)");
    }

    public void SetSFXVolume(float sliderValue)
    {
        // Convert the slider value (0 to 100) to a logarithmic AudioMixer volume (-80 to 0 dB)
        float normalizedValue = Mathf.Clamp(sliderValue / 100, 0.0001f, 1f); // Avoid log(0)
        float volume = Mathf.Log10(normalizedValue) * 20;
        myMixer.SetFloat("sfx", volume); // Set the volume in the AudioMixer
        PlayerPrefs.SetFloat("SFXVolume", sliderValue); // Save the slider value
        PlayerPrefs.Save(); // Ensure the value is saved immediately
        Debug.Log($"SFX volume set to: {volume} dB (Slider: {sliderValue}%)");
    }

    public void SetMasterVolume(float sliderValue)
    {
        // Convert the slider value (0 to 100) to a logarithmic AudioMixer volume (-80 to 0 dB)
        float normalizedValue = Mathf.Clamp(sliderValue / 100, 0.0001f, 1f); // Avoid log(0)
        float volume = Mathf.Log10(normalizedValue) * 20;
        myMixer.SetFloat("master", volume); // Set the volume in the AudioMixer
        PlayerPrefs.SetFloat("MasterVolume", sliderValue); // Save the slider value
        PlayerPrefs.Save(); // Ensure the value is saved immediately
        Debug.Log($"Master volume set to: {volume} dB (Slider: {sliderValue}%)");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
