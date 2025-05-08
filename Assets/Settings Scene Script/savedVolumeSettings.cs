using UnityEngine;

public class savedVolumeSettings : MonoBehaviour
{
    private VolumeSettings volumeSettings;

    private void Start()
    {
        // Retrieve saved volume settings from PlayerPrefs
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 100);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 100);
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 100);

        // Log the retrieved values (for debugging purposes)
        Debug.Log($"Music Volume: {musicVolume}, SFX Volume: {sfxVolume}, Master Volume: {masterVolume}");

        // Apply the settings to your local logic or UI if needed
        ApplyVolumeSettings(musicVolume, sfxVolume, masterVolume);
    }

    private void ApplyVolumeSettings(float music, float sfx, float master)
    {
        // Example: Apply the settings to your local audio system or UI
        Debug.Log("Volume settings applied successfully.");
    }
}
