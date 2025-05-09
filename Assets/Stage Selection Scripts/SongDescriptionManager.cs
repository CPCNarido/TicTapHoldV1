using UnityEngine;

public class SongDescriptionManager : MonoBehaviour
{
    void Start()
    {
        int selectedButtonIndex = Scroll.SelectedButtonIndex;

        // Disable all buttons except the selected one
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i != selectedButtonIndex)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}