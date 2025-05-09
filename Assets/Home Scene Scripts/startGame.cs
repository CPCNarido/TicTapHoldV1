using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class startGame : MonoBehaviour
{
    // This method will be called when the button is clicked
    public void OnStartButtonClick()
    {
         // Save the current scene as the previous scene
        GoToPrevScene.SetPreviousScene(SceneManager.GetActiveScene().name);
        // Replace "SceneName" with the name of the scene you want to load
        SceneManager.LoadScene("StageSelectionScene"); // Load the scene named "StageSelectionScene"
    }
}
