using UnityEngine;

public class SpriteGifPlayer : MonoBehaviour
{
    public Sprite[] frames;          // Assign your sprite frames in the Inspector
    public float framesPerSecond = 12f;

    private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float timer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentFrame = 0;
        timer = 0f;
    }

    void Update()
    {
        if (frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / framesPerSecond)
        {
            timer -= 1f / framesPerSecond;
            currentFrame = (currentFrame + 1) % frames.Length;
            spriteRenderer.sprite = frames[currentFrame];
        }
    }
}