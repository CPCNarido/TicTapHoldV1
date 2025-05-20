using System.IO;
using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private string jsonFileName = "mistydrive_easy.json"; // Set in Inspector
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public float animationDuration = 1.0f; // Duration of the animation in seconds

    void Start()
    {
        LoadAndDisplayScores();
    }

    void LoadAndDisplayScores()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"Score file not found at: {filePath}");
            scoreText.text = "Score: 0";
            highScoreText.text = "High Score: 0";
            return;
        }

        string json = File.ReadAllText(filePath);
        NoteConfig config = JsonUtility.FromJson<NoteConfig>(json);

        if (config != null && config.highScore != null)
        {
            int score = config.highScore.current;
            int best = config.highScore.best;

            StartCoroutine(AnimateScore(scoreText, score, animationDuration));

            if (score >= best)
                highScoreText.text = "New Highscore!!";
            else
                highScoreText.text = $"High Score: {best}";
        }
        else
        {
            scoreText.text = "Score: 0";
            highScoreText.text = "High Score: 0";
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