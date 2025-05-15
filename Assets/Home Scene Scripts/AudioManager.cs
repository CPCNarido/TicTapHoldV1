using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXsource;

    [Header("Audio Clips")]
    public AudioClip background;
    public AudioClip click;

    public static AudioManager instance;

    [Header("Excluded Scenes (no music)")]
    [SerializeField] private string[] excludedScenes;

    private void Awake()
    {

        // Ensure AudioManager is a singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            Debug.Log("[AudioManager] Awake in scene: " + SceneManager.GetActiveScene().name);
            TryPlayMusic(SceneManager.GetActiveScene().name); // Immediately handle current scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[AudioManager] Scene loaded: " + scene.name);
        TryPlayMusic(scene.name);
    }

    private void TryPlayMusic(string sceneName)
    {
        if (musicSource == null)
        {
            Debug.LogError("[AudioManager] musicSource is not assigned!");
            return;
        }

        if (background == null)
        {
            Debug.LogError("[AudioManager] Background clip not assigned!");
            return;
        }

        // Stop if the scene is in the excluded list
        foreach (string excluded in excludedScenes)
        {
            if (sceneName.Trim() == excluded.Trim())
            {
                Debug.Log("[AudioManager] Scene excluded from music. Stopping.");
                if (musicSource.isPlaying) musicSource.Stop();
                return;
            }
        }

        // Only start if not already playing
        if (!musicSource.isPlaying)
        {
            Debug.Log("[AudioManager] Playing music for scene: " + sceneName);
            musicSource.clip = background;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.Log("[AudioManager] Music already playing.");
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (SFXsource == null)
        {
            Debug.LogError("[AudioManager] SFX source not assigned!");
            return;
        }

        SFXsource.PlayOneShot(clip);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
