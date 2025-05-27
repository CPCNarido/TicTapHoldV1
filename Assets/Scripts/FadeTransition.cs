using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class FadeTransition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("Sprites to hide during fade")]
    public List<SpriteRenderer> spritesToHide = new List<SpriteRenderer>();

    // Call this to fade in (to black), then load the next scene
    public void FadeInAndLoadScene(string sceneName)
    {
        foreach (var sprite in spritesToHide)
        {
            if (sprite != null)
                sprite.gameObject.SetActive(false);
        }
        StartCoroutine(FadeInAndLoadSceneCoroutine(sceneName));
    }

    private IEnumerator FadeInAndLoadSceneCoroutine(string sceneName)
    {
        // Fade in (to black)
        float elapsed = 0f;
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 1f;
        fadeImage.color = c;

        // Now load the next scene (still black)
        SceneManager.LoadScene(sceneName);
    }

    // Call this in the next scene's Start() to fade out (from black to visible)
    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 0f;
        fadeImage.color = c;
    }
}