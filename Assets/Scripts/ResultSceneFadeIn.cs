using UnityEngine;

public class ResultSceneFadeIn : MonoBehaviour
{
    void Start()
    {
        // This will trigger the fade out (from black to visible) when the result scene loads
        FadeTransition fade = FindObjectOfType<FadeTransition>();
        if (fade != null)
        {
            fade.FadeOut();
        }
    }
}