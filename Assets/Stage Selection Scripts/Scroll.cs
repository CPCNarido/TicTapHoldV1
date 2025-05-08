using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Scroll : MonoBehaviour
{
    public GameObject scrollbar;
    float scroll_pos = 0;
    float[] pos;
    private GameObject currentlySelectedParent = null;

    void Update()
    {
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1);

        // Populate the pos array
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = i * distance;
        }

        // Update scroll_pos based on the scrollbar value
        scroll_pos = scrollbar.GetComponent<Scrollbar>().value;

        for (int i = 0; i < pos.Length; i++)
        {
            if (scroll_pos > pos[i] - (distance / 2) && scroll_pos < pos[i] + (distance / 2))
            {
                // Scale the selected button
                transform.GetChild(i).localScale = Vector3.Lerp(transform.GetChild(i).localScale, new Vector3(1.2f, 1.2f, 1f), 0.1f);
                for (int a = 0; a < pos.Length; a++)
                {
                    if (a != i)
                    {
                        // Scale the unselected buttons
                        transform.GetChild(a).localScale = Vector3.Lerp(transform.GetChild(a).localScale, new Vector3(0.8f, 0.8f, 0.8f), 0.1f);
                    }
                }
            }
        }

        // Detect touches or mouse clicks outside buttons
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                Debug.Log("Touch began outside UI.");
                ResetActiveButton();
            }
        }
        else if (Input.GetMouseButtonDown(0)) // For PC testing
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Mouse click detected outside UI.");
                ResetActiveButton();
            }
        }
    }

    // Method to reset the currently active button
    private void ResetActiveButton()
    {
        if (currentlySelectedParent != null)
        {
            Debug.Log("Resetting active button: " + currentlySelectedParent.name);
            
            // Make parent button visible again
            Image parentImage = currentlySelectedParent.GetComponent<Image>();
            if (parentImage != null) parentImage.enabled = true;
            
            Text parentText = currentlySelectedParent.GetComponentInChildren<Text>();
            if (parentText != null) parentText.enabled = true;

            // Enable the parent button's Button component
            Button parentButtonComponent = currentlySelectedParent.GetComponent<Button>();
            if (parentButtonComponent != null)
            {
                parentButtonComponent.interactable = true;
            }

            // Disable the child button
            Transform childButton = currentlySelectedParent.transform.GetChild(0);
            if (childButton != null)
            {
                childButton.gameObject.SetActive(false);
            }

            currentlySelectedParent = null;
        }
    }

    // Method to handle parent button click
    public void OnParentButtonClick(GameObject parentButton)
    {
        Debug.Log("OnParentButtonClick called for: " + parentButton.name);

        // If clicking the same button again, toggle it off
        if (currentlySelectedParent == parentButton)
        {
            ResetActiveButton();
            return;
        }

        // Reset any previously selected button
        if (currentlySelectedParent != null)
        {
            ResetActiveButton();
        }

        // Set the new selected button
        currentlySelectedParent = parentButton;

        // Hide the parent button's visual elements
        Image parentImage = parentButton.GetComponent<Image>();
        if (parentImage != null) parentImage.enabled = false;
        
        Text parentText = parentButton.GetComponentInChildren<Text>();
        if (parentText != null) parentText.enabled = false;

        // Show the child button
        Transform childButton = parentButton.transform.GetChild(0);
        if (childButton == null)
        {
            Debug.LogError("Child button not found under: " + parentButton.name);
            return;
        }

        Debug.Log("Activating child button: " + childButton.name);
        childButton.gameObject.SetActive(true);

        // Disable the parent button's Button component
        Button parentButtonComponent = parentButton.GetComponent<Button>();
        if (parentButtonComponent != null)
        {
            parentButtonComponent.interactable = false;
            Debug.Log("Parent button interaction disabled: " + parentButton.name);
        }
    }

    // Method to handle child button click
    public void OnChildButtonClick(string levelName)
    {
        Debug.Log("Loading level: " + levelName);
        SceneManager.LoadScene(levelName);
    }
}