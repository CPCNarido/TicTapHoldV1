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
    private Vector2 childButtonSize = new Vector2(400f, 390f); // Set your desired child button size here

    void Update()
    {
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);

        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = i * distance;
        }

        scroll_pos = scrollbar.GetComponent<Scrollbar>().value;

        // Handle button scaling based on scroll position
        for (int i = 0; i < pos.Length; i++)
        {
            if (scroll_pos > pos[i] - (distance / 2) && scroll_pos < pos[i] + (distance / 2))
            {
                transform.GetChild(i).localScale = Vector2.Lerp(
                    transform.GetChild(i).localScale, 
                    new Vector2(1.2f, 1.2f), 
                    0.1f
                );
                
                for (int a = 0; a < pos.Length; a++)
                {
                    if (a != i)
                    {
                        transform.GetChild(a).localScale = Vector2.Lerp(
                            transform.GetChild(a).localScale, 
                            new Vector2(0.8f, 0.8f), 
                            0.1f
                        );
                    }
                }
            }
        }

        // Handle click outside UI
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                ResetActiveButton();
            }
        }
    }

    private void ResetActiveButton()
    {
        if (currentlySelectedParent != null)
        {
            // Find the child button in the hierarchy
            Transform childButton = currentlySelectedParent.transform.parent.Find(currentlySelectedParent.name + "_Child");
            
            if (childButton != null)
            {
                Destroy(childButton.gameObject); // Remove the child button
            }

            // Reactivate the parent
            currentlySelectedParent.SetActive(true);
            currentlySelectedParent = null;
        }
    }

    public void OnParentButtonClick(GameObject parentButton)
    {
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

        // Create a new child button instance (or use a pooled object)
        GameObject childButton = Instantiate(
            parentButton.transform.GetChild(0).gameObject, // Original child prefab
            parentButton.transform.parent
        );

        // Position and size the child button
        RectTransform parentRect = parentButton.GetComponent<RectTransform>();
        RectTransform childRect = childButton.GetComponent<RectTransform>();

        // Set anchors and pivot to center
        childRect.anchorMin = new Vector2(0.5f, 0.5f);
        childRect.anchorMax = new Vector2(0.5f, 0.5f);
        childRect.pivot = new Vector2(0.5f, 0.5f);

        // Set position to match parent (centered)
        childRect.anchoredPosition = parentRect.anchoredPosition;

        // Apply custom size (400x390)
        childRect.sizeDelta = childButtonSize;

        // Set hierarchy order and name
        childButton.transform.SetSiblingIndex(parentButton.transform.GetSiblingIndex());
        childButton.name = parentButton.name + "_Child";

        // Activate the child and deactivate the parent
        childButton.SetActive(true);
        parentButton.SetActive(false);

        // Add click handler to the child button
        Button childBtnComponent = childButton.GetComponent<Button>();
        if (childBtnComponent != null)
        {
            childBtnComponent.onClick.RemoveAllListeners();
            childBtnComponent.onClick.AddListener(() => OnChildButtonClick("YourLevelNameHere"));
        }
    }

    public void OnChildButtonClick(string levelName)
    {
        Debug.Log("Loading level: " + levelName);
        SceneManager.LoadScene(levelName);
    }
}