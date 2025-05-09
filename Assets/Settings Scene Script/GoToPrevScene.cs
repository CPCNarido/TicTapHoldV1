using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToPrevScene : MonoBehaviour
{
    private string previousScene;

    void Start()
    {
        // Retrieve the name of the previous scene from PlayerPrefs
        previousScene = PlayerPrefs.GetString("PreviousScene", "");

        // Debug log to verify the previous scene
        Debug.Log($"Previous scene loaded: {previousScene}");
    }

    public void OnBackButtonClicked()
    {
        // Load the previous scene if it exists
        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("No previous scene found! Defaulting to Home Scene.");
            SceneManager.LoadScene("HomeScene"); // Default fallback scene
        }
    }

    public static void SetPreviousScene(string sceneName)
    {
        // Save the current scene name as the previous scene
        Debug.Log($"Setting previous scene to: {sceneName}");
        PlayerPrefs.SetString("PreviousScene", sceneName);
        PlayerPrefs.Save();
    }

    public void OnHomeButtonClicked()
    {
        // Load the HomeScene
        Debug.Log("Navigating to HomeScene...");
        SceneManager.LoadScene("New Home Scene");
    }
}
