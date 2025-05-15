using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private string sceneName; // Input the name of the scene in the Inspector

    // This method will be called when the button is pressed
    public void OnButtonPressed()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log($"Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName); // Load the specified scene
        }
        else
        {
            Debug.LogWarning("Scene name is empty! Please input a valid scene name in the Inspector.");
        }
    }
}