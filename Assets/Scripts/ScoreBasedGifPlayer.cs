using UnityEngine;

public class ScoreBasedGifPlayer : MonoBehaviour
{
    public Sprite[] highScoreFrames;    // For percentage >= 95
    public Sprite[] midScoreFrames;     // For 85 <= percentage < 95
    public Sprite[] lowScoreFrames;     // For 75 <= percentage < 85
    public Sprite[] defeatFrames;       // For percentage < 75
    public float framesPerSecond = 12f;

    public AudioSource victoryBGM;      // Assign in Inspector (for any non-defeat)
    public AudioSource defeatBGM;       // Assign in Inspector (for defeat only)

    private SpriteRenderer spriteRenderer;
    private Sprite[] currentFrames;
    private int currentFrame;
    private float timer;
    private bool animationFinished = false;

    public void SetScore(int percentage)
    {
        // Stop both BGMs before playing the correct one
        if (victoryBGM != null && victoryBGM.isPlaying) victoryBGM.Stop();
        if (defeatBGM != null && defeatBGM.isPlaying) defeatBGM.Stop();

        if (percentage >= 95)
        {
            currentFrames = highScoreFrames;
            if (victoryBGM != null) victoryBGM.Play();
        }
        else if (percentage >= 85)
        {
            currentFrames = midScoreFrames;
            if (victoryBGM != null) victoryBGM.Play();
        }
        else if (percentage >= 75)
        {
            currentFrames = lowScoreFrames;
            if (victoryBGM != null) victoryBGM.Play();
        }
        else
        {
            currentFrames = defeatFrames;
            if (defeatBGM != null) defeatBGM.Play();
        }

        currentFrame = 0;
        timer = 0f;
        animationFinished = false;
        if (currentFrames != null && currentFrames.Length > 0)
            spriteRenderer.sprite = currentFrames[0];
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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