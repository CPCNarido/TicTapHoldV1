using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToPrevScene : MonoBehaviour
{
    private string previousScene;

    void Start()
    {
        // Store the name of the current scene as the previous scene
        previousScene = PlayerPrefs.GetString("PreviousScene", "");
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
            Debug.LogWarning("No previous scene found!");
        }
    }

    public static void SetPreviousScene(string sceneName)
    {
        // Save the current scene name as the previous scene
        PlayerPrefs.SetString("PreviousScene", sceneName);
        PlayerPrefs.Save();
    }
}
