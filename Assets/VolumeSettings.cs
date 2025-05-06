using UnityEngine;
using UnityEngine.UI; // Import the Unity UI namespace
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer; // Reference to the AudioMixer
    [SerializeField] private Slider musicSlider; // Reference to the Music Slider component
    [SerializeField] private Slider sfxSlider; // Reference to the SFX Slider component

    private void Start()
    {
        if (musicSlider == null || myMixer == null)
        {
            Debug.LogError("MusicSlider or AudioMixer is not assigned in the Inspector!");
            return;
        }

        // Initialize the slider value based on the current AudioMixer volume
        float currentVolume;
        if (myMixer.GetFloat("music", out currentVolume))
        {
            // Convert the AudioMixer volume (-80 to 0 dB) to a percentage (0 to 100)
            musicSlider.value = Mathf.Clamp01(Mathf.Pow(10, currentVolume / 20)) * 100;
        }
        else
        {
            // Default slider value to 100% if no volume is set
            musicSlider.value = 100;
        }

        // Add listener to update the AudioMixer volume when the slider value changes
        musicSlider.onValueChanged.AddListener(SetMusicVolume);

        // Initialize the SFX slider value based on the current AudioMixer volume
        float currentSFXVolume;
        if (myMixer.GetFloat("sfx", out currentSFXVolume))
        {
            // Convert the AudioMixer volume (-80 to 0 dB) to a percentage (0 to 100)
            sfxSlider.value = Mathf.Clamp01(Mathf.Pow(10, currentSFXVolume / 20)) * 100;
        }
        else
        {
            // Default slider value to 100% if no volume is set
            sfxSlider.value = 100;
        }

        // Add listener to update the AudioMixer volume when the SFX slider value changes
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float sliderValue)
    {
        // Convert the slider value (0 to 100) to a logarithmic AudioMixer volume (-80 to 0 dB)
        float normalizedValue = Mathf.Clamp(sliderValue / 100, 0.0001f, 1f); // Avoid log(0)
        float volume = Mathf.Log10(normalizedValue) * 20;
        myMixer.SetFloat("music", volume); // Set the volume in the AudioMixer
        Debug.Log($"Music volume set to: {volume} dB (Slider: {sliderValue}%)");
    }

    public void SetSFXVolume(float sliderValue)
    {
        // Convert the slider value (0 to 100) to a logarithmic AudioMixer volume (-80 to 0 dB)
        float normalizedValue = Mathf.Clamp(sliderValue / 100, 0.0001f, 1f); // Avoid log(0)
        float volume = Mathf.Log10(normalizedValue) * 20;
        myMixer.SetFloat("sfx", volume); // Set the volume in the AudioMixer
        Debug.Log($"SFX volume set to: {volume} dB (Slider: {sliderValue}%)");
    }
}
