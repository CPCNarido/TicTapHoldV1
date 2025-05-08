using UnityEngine;

public class Restore : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestoreDefaults()
    {
        Debug.Log("Restoring all settings to default values...");

        // Restore default graphics quality (e.g., High)
        PlayerPrefs.SetInt("GraphicsQuality", 2); // Default to High quality
        PlayerPrefs.Save();
        Debug.Log("Graphics quality restored to High.");

        // Restore default volume settings
        RestoreDefaultSoundSettings();

        // Apply the default graphics quality if a graphics manager exists
        GraphicsQualityManager graphicsManager = GameObject.FindWithTag("GraphicsQualityManagerTag")?.GetComponent<GraphicsQualityManager>();
        if (graphicsManager != null)
        {
            graphicsManager.SetQuality(2); // Default to High quality
        }
        else
        {
            Debug.LogWarning("GraphicsQualityManager not found. Graphics settings may not be applied.");
        }

        Debug.Log("All settings restored to default.");
    }

    public void RestoreDefaultSoundSettings()
    {
        Debug.Log("Restoring sound settings to default values...");

        // Find the VolumeSettings script and reset the sliders
        VolumeSettings volumeSettings = GameObject.FindWithTag("VolumeSettings")?.GetComponent<VolumeSettings>();
        if (volumeSettings != null)
        {
            // Reset the sliders to 100%
            volumeSettings.musicSlider.value = 100;
            volumeSettings.sfxSlider.value = 100;
            volumeSettings.masterSlider.value = 100;

            Debug.Log("Sliders reset to 100%.");
        }
        else
        {
            Debug.LogWarning("VolumeSettings not found. Sliders may not be updated.");
        }
    }
}
