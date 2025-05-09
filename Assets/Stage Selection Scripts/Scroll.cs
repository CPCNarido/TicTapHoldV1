using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Scroll : MonoBehaviour
{
    public GameObject scrollbar;
    float scroll_pos = 0;
    float[] pos;

    // Static variable to store the selected button index
    public static int SelectedButtonIndex { get; private set; }

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
    }

    public void OnSongButtonClick(int buttonIndex)
    {
         // Save the current scene as the previous scene
        GoToPrevScene.SetPreviousScene(SceneManager.GetActiveScene().name);
        // Store the selected button index
        SelectedButtonIndex = buttonIndex;

        // Load the SongDescriptionScene
        SceneManager.LoadScene("SongDescriptionScene");
    }
}