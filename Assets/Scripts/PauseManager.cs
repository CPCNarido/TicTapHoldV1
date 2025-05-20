using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject blurBackground;
    public AudioSource[] audioSources; // Assign all AudioSources you want to pause (e.g., backgroundMusic, SFX)

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        blurBackground.SetActive(true);

        foreach (var audio in audioSources)
        {
            if (audio != null && audio.isPlaying)
                audio.Pause();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        blurBackground.SetActive(false);

        foreach (var audio in audioSources)
        {
            if (audio != null)
                audio.UnPause();
        }
    }
}