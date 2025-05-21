using UnityEngine;

public class ScoreBasedGifPlayer : MonoBehaviour
{
    public Sprite[] highScoreFrames;    // For score >= 7000
    public Sprite[] midScoreFrames;     // For 4999 <= score < 7000
    public Sprite[] lowScoreFrames;     // For score <= 4998
    public float framesPerSecond = 12f;

    private SpriteRenderer spriteRenderer;
    private Sprite[] currentFrames;
    private int currentFrame;
    private float timer;
    private bool animationFinished = false;

    // Call this with the final score to set the animation
    public void SetScore(int score)
    {
        if (score >= 7000)
            currentFrames = highScoreFrames;
        else if (score >= 4999)
            currentFrames = midScoreFrames;
        else
            currentFrames = lowScoreFrames;

        currentFrame = 0;
        timer = 0f;
        animationFinished = false;
        if (currentFrames != null && currentFrames.Length > 0)
            spriteRenderer.sprite = currentFrames[0];
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Optionally, set a default animation
        SetScore(0);
    }

    void Update()
    {
        if (animationFinished || currentFrames == null || currentFrames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / framesPerSecond)
        {
            timer -= 1f / framesPerSecond;
            currentFrame++;
            if (currentFrame >= currentFrames.Length)
            {
                currentFrame = currentFrames.Length - 1;
                animationFinished = true;
            }
            spriteRenderer.sprite = currentFrames[currentFrame];
        }
    }
}