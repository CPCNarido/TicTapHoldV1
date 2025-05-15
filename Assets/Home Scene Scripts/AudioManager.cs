using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXsource;

    public AudioClip background;
    public AudioClip click;

    public static AudioManager instance;

    [SerializeField] private string[] excludedScenes; // Scene names to exclude music from

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene change
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        HandleMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleMusicForScene(scene.name);
    }

    private void HandleMusicForScene(string sceneName)
    {
        foreach (string excluded in excludedScenes)
        {
            if (sceneName == excluded)
            {
                if (musicSource.isPlaying)
                {
                    musicSource.Stop();
                }
                return;
            }
        }

        // Only play if not already playing
        if (!musicSource.isPlaying)
        {
            musicSource.clip = background;
            musicSource.Play();
        }
    }

    internal void PlaySFX(AudioClip clip)
    {
        SFXsource.clip = clip;
        SFXsource.Play();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Clean up listener
    }
}
