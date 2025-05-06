using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    private string previousScene;

    //prev scene
    public static void SetPreviousScene(string sceneName)
    {
        Debug.Log($"Saving previous scene: {sceneName}");
        PlayerPrefs.SetString("PreviousScene", sceneName);
        PlayerPrefs.Save();
    }

    void Start()
    {
        previousScene = PlayerPrefs.GetString("PreviousScene", "");
        Debug.Log($"Retrieved previous scene: {previousScene}");
    }

    public void OnSettingsButtonClicked()
    {
        // Save the current scene as the previous scene
        SetPreviousScene(SceneManager.GetActiveScene().name);

        // Load the settings scene
        SceneManager.LoadScene("SettingsScene");
    }

    public void OnBackButtonClicked()
    {
        Debug.Log("Back button clicked!");
        if (!string.IsNullOrEmpty(previousScene))
        {
            Debug.Log($"Loading previous scene: {previousScene}");
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("No previous scene found!");
        }
    }
}
