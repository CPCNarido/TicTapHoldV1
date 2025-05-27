using UnityEngine;

public class SongDescriptionManager : MonoBehaviour
{
    public AudioSource musicSource; // Assign in Inspector
    public AudioClip[] musicClips;  // Assign in Inspector, order matches buttons

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

        // Play the music for the selected index
        if (musicClips != null && selectedButtonIndex >= 0 && selectedButtonIndex < musicClips.Length)
        {
            if (musicSource != null)
            {
                musicSource.clip = musicClips[selectedButtonIndex];
                musicSource.Play();
            }
            else
            {
                Debug.LogWarning("MusicSource not assigned!");
            }
        }
        else
        {
            Debug.LogWarning("No music clip assigned for this index!");
        }
    }
}