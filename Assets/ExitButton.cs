using UnityEngine;

public class ExitButton : MonoBehaviour
{
    // This method will be called when the button is clicked
    public void OnExitButtonClick()
    {
        // Logs a message in the editor (useful for testing in the Unity Editor)
        Debug.Log("Exit button clicked!");

        // Exits the application (this will only work in a built application, not in the Unity Editor)
        Application.Quit();
    }
}
