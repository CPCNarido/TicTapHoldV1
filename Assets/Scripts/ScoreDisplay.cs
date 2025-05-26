using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private string jsonFileName = "mistydrive_easy.json";
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public float animationDuration = 1.0f;

    public ScoreBasedGifPlayer gifPlayer; // Reference to your ScoreBasedGifPlayer

    void Start()
    {
        LoadAndDisplayScores();
    }

    int CalculateMaxScore(int noteCount, int streakThreshold)
    {
        int maxScore = 0;
        int multiplier = 1;
        int streak = 0;
        for (int i = 0; i < noteCount; i++)
        {
            maxScore += 100 * multiplier;
            streak++;
            if (streak % streakThreshold == 0)
                multiplier++;
        }
        return maxScore;
    }

    void LoadAndDisplayScores()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"Score file not found at: {filePath}");
            scoreText.text = "Score: 0";
            highScoreText.text = "High Score: 0";
            if (gifPlayer != null) gifPlayer.SetScore(0);
            return;
        }

        string json = File.ReadAllText(filePath);
        NoteConfig config = JsonUtility.FromJson<NoteConfig>(json);

        if (config != null && config.highScore != null)
        {
            int score = config.highScore.current;
            int best = config.highScore.best;
            int noteCount = config.notes.Count;
            int maxScore = CalculateMaxScore(noteCount, 10); // or pass your actual streakThreshold
                int percentage = (maxScore > 0) ? Mathf.RoundToInt((float)score / maxScore * 100f) : 0;

            StartCoroutine(AnimateScore(scoreText, score, animationDuration));

            if (score >= best)
                highScoreText.text = "New Highscore!!";
            else
                highScoreText.text = $"High Score: {best}";

            // Pass the score to the gif player
            if (gifPlayer != null)
                gifPlayer.SetScore(percentage);
        }
        else
        {
            scoreText.text = "Score: 0";
            highScoreText.text = "High Score: 0";
            if (gifPlayer != null) gifPlayer.SetScore(0);
        }
    }

    IEnumerator AnimateScore(TextMeshProUGUI text, int target, float duration)
    {
        float elapsed = 0f;
        int start = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int current = Mathf.RoundToInt(Mathf.Lerp(start, target, elapsed / duration));
            text.text = current.ToString();
            yield return null;
        }
        text.text = target.ToString();
    }
}