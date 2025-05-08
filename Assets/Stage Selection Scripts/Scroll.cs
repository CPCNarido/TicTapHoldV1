using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject scrollbar;
    float scroll_pos = 0;
    float[] pos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector3(1.2f, 1.2f, 1f), 0.1f);
                for (int a = 0; a < pos.Length; a++)
                {
                    if (a != i)
                    {
                        // Scale the unselected buttons
                        transform.GetChild(a).localScale = Vector2.Lerp(transform.GetChild(a).localScale, new Vector3(0.8f, 0.8f, 0.8f), 0.1f);
                    }
                }
            }
        }
    }
}
