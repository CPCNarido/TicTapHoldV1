using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class startGame : MonoBehaviour
{
    // This method will be called when the button is clicked
    public void OnStartButtonClick()
    {
        // Replace "SceneName" with the name of the scene you want to load
        SceneManager.LoadScene("StageSelectionScene"); // Load the scene named "StageSelectionScene"
    }
}
