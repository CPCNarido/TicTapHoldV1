using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class RadioButtonSystem : MonoBehaviour
{
    [SerializeField] private ToggleGroup toggleGroup; // Assign in Inspector

    [Header("Scene Names (Order: Easy, Medium, Hard)")]
    [SerializeField] private string[] sceneNames = new string[3];

    public void Submit()
    {
        if (toggleGroup == null)
        {
            Debug.LogError("ToggleGroup is not assigned!");
            return;
        }

        var toggles = toggleGroup.GetComponentsInChildren<Toggle>().ToList();
        int selectedIndex = toggles.FindIndex(t => t.isOn);

        Debug.Log($"Selected Toggle Index: {selectedIndex}");

        if (selectedIndex >= 0 && selectedIndex < sceneNames.Length && !string.IsNullOrEmpty(sceneNames[selectedIndex]))
        {
            SceneManager.LoadScene(sceneNames[selectedIndex]);
        }
        else
        {
            Debug.LogWarning("No toggle selected or scene name not set for this option.");
        }
    }
}
