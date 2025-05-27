using UnityEngine;
using UnityEngine.Video;

public class LoopingVideoBackground : MonoBehaviour
{
    [Header("Assign your VideoPlayer component here")]
    public VideoPlayer videoPlayer;

    [Header("Optional: Assign a RenderTexture for UI/Canvas backgrounds")]
    public RenderTexture renderTexture;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.isLooping = true;

            if (renderTexture != null)
            {
                videoPlayer.targetTexture = renderTexture;
            }

            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("No VideoPlayer assigned to LoopingVideoBackground.");
        }
    }
}