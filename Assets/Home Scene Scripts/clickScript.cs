using UnityEngine;

public class clickScript : MonoBehaviour
{
    private AudioManager audioManager; // Reference to the AudioManager script

    private void Awake()
    {
        // Find the AudioManager in the scene
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
    }

    // Method to be called when the button is clicked
    public void OnButtonClick()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.click); // Play the sound named "click"
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
    }
}
