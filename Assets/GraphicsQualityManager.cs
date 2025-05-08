using UnityEngine;
using UnityEngine.UI;

public class GraphicsQualityManager : MonoBehaviour
{
    public Button lowButton;
    public Button mediumButton;
    public Button highButton;
    public Button restoreButton; // Add a reference for the Restore button

    private void Start()
    {
        // Find buttons dynamically by tag
        if (lowButton == null) lowButton = GameObject.FindWithTag("LowButtonTag").GetComponent<Button>();
        if (mediumButton == null) mediumButton = GameObject.FindWithTag("MediumButtonTag").GetComponent<Button>();
        if (highButton == null) highButton = GameObject.FindWithTag("HighButtonTag").GetComponent<Button>();
        if (restoreButton == null) restoreButton = GameObject.FindWithTag("RestoreButtonTag").GetComponent<Button>();

        // Apply the saved quality level at the start of the game
        ApplySavedQuality();

        // Set up button listeners
        if (lowButton != null) lowButton.onClick.AddListener(() => SetQuality(0)); // Low
        if (mediumButton != null) mediumButton.onClick.AddListener(() => SetQuality(1)); // Medium
        if (highButton != null) highButton.onClick.AddListener(() => SetQuality(2)); // High
        if (restoreButton != null) restoreButton.onClick.AddListener(RestoreDefaultQuality); // Restore

        // Update button states to reflect the current quality setting
        UpdateButtonStates();
    }

    public void SetQuality(int qualityIndex)
    {
        Debug.Log($"Setting quality to index: {qualityIndex}");
        // Set the quality level based on the index provided
        QualitySettings.SetQualityLevel(qualityIndex, true);
        // Save the selected quality level to PlayerPrefs for persistence
        PlayerPrefs.SetInt("GraphicsQuality", qualityIndex);
        PlayerPrefs.Save();

        // Update button states
        UpdateButtonStates();
    }

    public int GetQuality()
    {
        // Retrieve the saved quality level from PlayerPrefs, defaulting to 2 (High) if not set
        return PlayerPrefs.GetInt("GraphicsQuality", 2);
    }

    private void ApplySavedQuality()
    {
        int savedQuality = GetQuality();
        Debug.Log($"Applying saved quality: {savedQuality}");
        QualitySettings.SetQualityLevel(savedQuality, true);
    }

    private void UpdateButtonStates()
    {
        int currentQuality = GetQuality();
        Debug.Log($"[UpdateButtonStates] Current Quality Level: {currentQuality}");

        // Update button interactability based on the current quality level
        if (lowButton != null)
        {
            lowButton.interactable = currentQuality != 0;
            Debug.Log($"Low Button interactable: {lowButton.interactable}");
        }
        if (mediumButton != null)
        {
            mediumButton.interactable = currentQuality != 1;
            Debug.Log($"Medium Button interactable: {mediumButton.interactable}");
        }
        if (highButton != null)
        {
            highButton.interactable = currentQuality != 2;
            Debug.Log($"High Button interactable: {highButton.interactable}");
        }
    }

    private void RestoreDefaultQuality()
    {
        Debug.Log("Restoring default quality: High");
        // Set the quality level to High (default index 2)
        SetQuality(2);
    }
}
