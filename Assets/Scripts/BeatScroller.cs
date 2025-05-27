using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine;
using TMPro;

public class BeatScroller : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "ResultScene"; // Set this in the Inspector
    public float beatTempo; // BPM
    public GameObject leftNotePrefab;
    public GameObject rightNotePrefab;
    public GameObject leftHoldNotePrefab;
    public GameObject rightHoldNotePrefab;
    public Transform centerPosition;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;
    private bool gameEnded = false;

    public FeedbackManager feedbackManager;

    public AudioSource noteHitSound;
    public AudioSource holdNoteSound;
    public AudioSource backgroundMusic;

    public float musicStartDelay = 2f; // Gap after notes spawn before music starts

    private bool hasStarted = false;
    private bool musicStarted = false;
    private float startTime;

    private NoteConfig noteConfig;
    private int noteIndex = 0;

    private List<GameObject> activeNotes = new List<GameObject>();

    // Scoring system
    private int score = 0;
    private int multiplier = 1;
    private int streak = 0;
    private int perfectStreak = 0;
    private const int streakThreshold = 10; // Increase multiplier every 10 successful hits

    [Header("JSON Configuration")]
    [SerializeField] private string jsonFileName = "mistydrive_easy.json"; // Input JSON file name in the Inspector

    // Button positions and scales
    private Vector2 leftButtonPos = new Vector2(-10.15f, 0.07f);
    private Vector2 rightButtonPos = new Vector2(10.15f, 0.07f);
    private Vector2 buttonScale = new Vector2(1.25f, 2.475f);


    // Hit windows (tweak as needed)
    private float perfectRadius = 0.5f * 1.25f; // Half width of button for perfect
    private float earlyLateRadius = 1.5f * 1.25f; // A bit wider for early/late

    void Start()
    {
        Time.timeScale = 1f;

        if (scoreText != null)
            scoreText.text = "0";

        beatTempo = beatTempo / 60f;

        // Automatically calculate musicStartDelay based on travel time from center to target (9.5 units)
        float noteTravelDistance = 9.5f; // Distance from center (0) to left/right target
        musicStartDelay = noteTravelDistance / beatTempo;

        Debug.Log($"Music will start after {musicStartDelay:F2} seconds (auto-calculated).");

        LoadNoteConfig();
    }

    void Update()
    {
        if (!hasStarted)
        {
            if (Input.touchCount > 0 || Input.anyKeyDown)
            {
                hasStarted = true;
                startTime = Time.timeSinceLevelLoad;
                Debug.Log("Game started: Notes will begin immediately, music after delay.");
            }
        }
        else
        {
            float elapsedTime = Time.timeSinceLevelLoad - startTime;

            // Start background music after delay
            if (!musicStarted && elapsedTime >= musicStartDelay)
            {
                if (backgroundMusic != null && !backgroundMusic.isPlaying)
                {
                    backgroundMusic.Play();
                    musicStarted = true;
                    Debug.Log("Music started.");
                }
            }

            if (noteConfig == null || noteConfig.notes == null)
            {
                Debug.LogError("NoteConfig is null or not properly loaded!");
                return;
            }

            // Continue spawning notes immediately after game start
            while (noteIndex < noteConfig.notes.Count &&
                elapsedTime >= noteConfig.notes[noteIndex].time)
            {
                SpawnNote(noteConfig.notes[noteIndex], elapsedTime);
                noteIndex++;
            }

            // Handle touch input
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                    {
                        if (touch.position.x < Screen.width / 2)
                        {
                            HandleHoldNote("left", Time.deltaTime);
                        }
                        else if (touch.position.x >= Screen.width / 2)
                        {
                            HandleHoldNote("right", Time.deltaTime);
                        }
                    }
                }
            }

            // Remove notes that are destroyed after moving off-screen
            activeNotes.RemoveAll(note => note == null);
        }

        if (!gameEnded && hasStarted && noteIndex >= noteConfig.notes.Count && activeNotes.Count == 0)
        {
            Debug.Log("All notes cleared. Ending game...");
            EndGame();
            hasStarted = false;
            gameEnded = true;
        }
    }

    void LoadNoteConfig()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        Debug.Log($"Loading note configuration from: {filePath}");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            noteConfig = JsonUtility.FromJson<NoteConfig>(json);

            if (noteConfig != null && noteConfig.notes != null)
            {
                Debug.Log($"Loaded {noteConfig.notes.Count} notes from configuration.");
            }
            else
            {
                Debug.LogError("Failed to parse NoteConfig or notes list is null.");
            }
        }
        else
        {
            Debug.LogError($"Note configuration file not found at: {filePath}");
        }
    }





    void SpawnNote(Note note, float elapsedTime)
    {
        GameObject prefab;
        bool isHoldNote = note.holdDuration > 0;

        // Choose prefab based on note type and direction
        if (isHoldNote)
        {
            prefab = note.direction == "left" ? leftHoldNotePrefab : rightHoldNotePrefab;
            Debug.Log($"Spawning HOLD note: direction={note.direction}, time={note.time}, duration={note.holdDuration}");
        }

        else
        {
            prefab = note.direction == "left" ? leftNotePrefab : rightNotePrefab;
            Debug.Log($"Spawning TAP note: direction={note.direction}, time={note.time}");
        }


        if (prefab == null)
        {
            Debug.LogError($"Prefab for {note.direction} {(isHoldNote ? "hold" : "tap")} notes is not assigned!");
            return;
        }

        Vector3 spawnPosition = new Vector3(centerPosition.position.x, centerPosition.position.y, -5f);

        // Calculate the scale factor for hold notes
        float scaleFactor = 1f;
        if (isHoldNote)
            scaleFactor = Mathf.Abs(note.holdDuration * beatTempo);

        // Get the prefab's SpriteRenderer width in world units (after scaling)
        SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
        float prefabWidth = sr != null ? Mathf.Abs(sr.bounds.size.x * scaleFactor) : scaleFactor;

        // Offset logic to avoid overlap for notes spawning at nearly the same time
        Vector3 lastSpawn = note.direction == "left" ? lastLeftNoteSpawn : lastRightNoteSpawn;
        float lastWidth = note.direction == "left"
            ? lastLeftNoteSpawn != Vector3.positiveInfinity ? lastLeftNoteWidth : 0f
            : lastRightNoteSpawn != Vector3.positiveInfinity ? lastRightNoteWidth : 0f;

        float minGap = 0.05f;
        if (lastSpawn != Vector3.positiveInfinity && Mathf.Abs(spawnPosition.x - lastSpawn.x) < (lastWidth + prefabWidth) / 2f + minGap)
        {
            float offset = (lastWidth + prefabWidth) / 2f + minGap;
            spawnPosition.x += note.direction == "left" ? -offset : offset;
        }

        GameObject spawnedNote = Instantiate(prefab, spawnPosition, Quaternion.identity);

        // Update last spawn position and width for this direction
        if (note.direction == "left")
        {
            lastLeftNoteSpawn = spawnPosition;
            lastLeftNoteWidth = prefabWidth;
        }
        else
        {
            lastRightNoteSpawn = spawnPosition;
            lastRightNoteWidth = prefabWidth;
        }

        // Initialize NoteMover if present
        NoteMover noteMover = spawnedNote.GetComponent<NoteMover>();
        if (noteMover != null)
        {
            Vector3 targetPos = note.direction == "left"
                ? new Vector3(leftButtonPos.x, leftButtonPos.y, -5f)
                : new Vector3(rightButtonPos.x, rightButtonPos.y, -5f);

            noteMover.Initialize(targetPos, targetPos, targetPos, beatTempo, note.direction, isHoldNote, note.holdDuration);

            if (isHoldNote)
            {
                spawnedNote.transform.localScale = new Vector3(scaleFactor, spawnedNote.transform.localScale.y, spawnedNote.transform.localScale.z);
            }
        }

        // Initialize HoldNote if present
        // ...inside SpawnNote in BeatScroller.cs...
        HoldNote holdNote = spawnedNote.GetComponent<HoldNote>();
        if (holdNote != null && isHoldNote)
        {
            holdNote.holdDuration = note.holdDuration;
            holdNote.isRight = note.direction == "right";
            holdNote.speed = beatTempo; // Use the same speed as tap notes
            holdNote.targetPosition = note.direction == "right"
                ? new Vector3(rightButtonPos.x, rightButtonPos.y, -5f)
                : new Vector3(leftButtonPos.x, leftButtonPos.y, -5f);
            // Assign bodyTransform, headTransform, endTransform if needed
        }

        activeNotes.Add(spawnedNote);
    }

    // Add these fields at the top of your class:
    private Vector3 lastLeftNoteSpawn = Vector3.positiveInfinity;
    private Vector3 lastRightNoteSpawn = Vector3.positiveInfinity;
    private float lastLeftNoteWidth = 0f;
    private float lastRightNoteWidth = 0f;
    void HandleHoldNote(string direction, float deltaTime)
    {
        Debug.Log($"Checking for {direction} note to hold...");

        Vector2 buttonPos = direction == "left" ? leftButtonPos : rightButtonPos;

        GameObject closestNote = null;
        NoteMover closestNoteMover = null;
        float closestDist = float.MaxValue;
        string accuracy = "";
        int scoreToAdd = 0;

        // Find the closest note in the hit window
        foreach (GameObject note in activeNotes)
        {
            NoteMover noteMover = note.GetComponent<NoteMover>();
            if (noteMover != null && noteMover.GetDirection() == direction)
            {
                Vector2 notePos = note.transform.position;
                float dist = Vector2.Distance(notePos, buttonPos);

                if (dist <= perfectRadius)
                {
                    if (dist < closestDist)
                    {
                        closestNote = note;
                        closestNoteMover = noteMover;
                        closestDist = dist;
                        accuracy = "Perfect";
                        scoreToAdd = 100;
                    }
                }
                else if (dist <= earlyLateRadius)
                {
                    if (dist < closestDist)
                    {
                        closestNote = note;
                        closestNoteMover = noteMover;
                        closestDist = dist;
                        // Determine if it's early or late based on x position
                        if ((direction == "left" && notePos.x > buttonPos.x) ||
                            (direction == "right" && notePos.x < buttonPos.x))
                        {
                            accuracy = "Early";
                        }
                        else
                        {
                            accuracy = "Late";
                        }
                        scoreToAdd = 75;
                    }
                }
            }
        }

        // Only process the closest note
        if (closestNote != null && closestNoteMover != null)
        {
            if (closestNoteMover.IsHoldNote())
            {
                closestNoteMover.IncrementHoldProgress(deltaTime);
                Debug.Log($"Holding {direction} note. Progress: {closestNoteMover.GetHoldProgress():F2}/{closestNoteMover.GetHoldDuration():F2}");

                if (closestNoteMover.IsHoldComplete())
                {
                    Debug.Log($"Hold note {direction} completed!");
                    if (activeNotes.Contains(closestNote))
                        activeNotes.Remove(closestNote);
                    Destroy(closestNote);

                    AddScore(scoreToAdd);
                    feedbackManager.ShowFeedback(direction, accuracy);
                    if (holdNoteSound != null) holdNoteSound.Play();
                }
            }
            else
            {
                Debug.Log($"Tap note {direction} hit!");
                if (activeNotes.Contains(closestNote))
                    activeNotes.Remove(closestNote);
                Destroy(closestNote);

                AddScore(scoreToAdd);
                feedbackManager.ShowFeedback(direction, accuracy);
                if (noteHitSound != null) noteHitSound.Play();
            }
            return; // Only one note handled per press
        }

        Debug.Log($"No valid {direction} note found to hold/tap.");
    }

    private void HandleNoteScored(string direction, string accuracy)
    {
        int points = 0;
        switch (accuracy)
        {
            case "Perfect":
                points = 100;
                perfectStreak++;
                break;
            case "Early":
                points = 75;
                perfectStreak = 0;
                break;
            case "Late":
                points = 50;
                perfectStreak = 0;
                break;
            default:
                perfectStreak = 0;
                break;
        }

        AddScore(points);

        // Show "Perfect xN" if streak is 2 or more, otherwise just show the accuracy
        string feedback = accuracy;
        if (accuracy == "Perfect" && perfectStreak > 1)
            feedback = $"Perfect x{perfectStreak}";

        feedbackManager.ShowFeedback(direction, feedback);
    }

    void AddScore(int baseScore)
    {
        score += baseScore * multiplier;
        streak++;

        if (streak % streakThreshold == 0)
        {
            multiplier++;
            Debug.Log($"Multiplier increased to {multiplier}!");
        }

        Debug.Log($"Score: {score}, Multiplier: {multiplier}, Streak: {streak}");

        // Update the UI
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
            Debug.Log($"[UI] Score updated to {score}");
        }

        if (multiplierText != null)
        {
            multiplierText.text = $"Multiplier: x{multiplier}";
            Debug.Log($"[UI] Multiplier updated to x{multiplier}");
        }
    }
    void OnEnable()
    {
        NoteMover.OnNoteHit += HandleNoteScored;
        NoteMover.OnNoteMissed += HandleNoteMissed;
        HoldNote.OnHoldComplete += HandleHoldNoteComplete;
        HoldNote.OnHoldEarlyRelease += HandleHoldNoteEarlyRelease;
    }

    void OnDisable()
    {
        NoteMover.OnNoteHit += HandleNoteScored;
        NoteMover.OnNoteMissed -= HandleNoteMissed;
        HoldNote.OnHoldComplete -= HandleHoldNoteComplete;
        HoldNote.OnHoldEarlyRelease -= HandleHoldNoteEarlyRelease;
    }

    void HandleNoteMissed(string direction)
    {
        perfectStreak = 0;
        Debug.Log($"Note missed on {direction} side. Resetting multiplier.");
        ResetStreak();
    }

    void ResetStreak()
    {
        streak = 0;
        multiplier = 1;
        Debug.Log("Streak reset. Multiplier reset to 1.");

        // Update the UI
        UpdateScoreUI();
    }

    public void SaveScore()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"Score file not found at: {filePath}");
            return;
        }

        string json = File.ReadAllText(filePath);
        NoteConfig config = JsonUtility.FromJson<NoteConfig>(json);

        if (config != null)
        {
            // Ensure highScore object exists
            if (config.highScore == null)
            {
                config.highScore = new HighScore();
            }

            // Always update currentScore
            config.highScore.current = score;

            // Update best (high score) if needed
            if (config.highScore.best < score)
            {
                config.highScore.best = score;
                Debug.Log("New high score saved!");
            }
            else
            {
                Debug.Log("Score is not higher than the current high score. No update made.");
            }

            // Save back to JSON
            File.WriteAllText(filePath, JsonUtility.ToJson(config, true));
            Debug.Log($"Scores saved! Current: {config.highScore.current}, High: {config.highScore.best}");
        }
        else
        {
            Debug.LogError("Failed to parse JSON or NoteConfig is null.");
        }
    }

    // ...existing code...
    void EndGame()
    {
        SaveScore();
        Debug.Log("Game ended. Score saved.");
    
        // Find the fade transition object and trigger fade
        FadeTransition fade = FindObjectOfType<FadeTransition>();
        if (fade != null)
        {
            fade.FadeInAndLoadScene(nextSceneName);
        }
        else
        {
            // fallback if fade not found
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }
    // ...existing code...

    private IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Use WaitForSecondsRealtime so it works even if Time.timeScale == 0
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }

    IEnumerator SpawnHoldSegments(GameObject parentNote, float holdDuration)
    {
        float segmentInterval = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < holdDuration)
        {
            GameObject segment = Instantiate(parentNote, parentNote.transform.position, Quaternion.identity);
            segment.transform.SetParent(parentNote.transform);
            segment.transform.localPosition += new Vector3(-elapsedTime * beatTempo, 0, 0);

            elapsedTime += segmentInterval;
            yield return new WaitForSeconds(segmentInterval);
        }
    }
    private void HandleHoldNoteComplete(HoldNote note)
    {
        AddScore(100);
        feedbackManager.ShowFeedback(note.isRight ? "right" : "left", "Perfect");
        Destroy(note.gameObject);
    }

    private void HandleHoldNoteEarlyRelease(HoldNote note)
    {
        AddScore(50);
        feedbackManager.ShowFeedback(note.isRight ? "right" : "left", "Early");
        Destroy(note.gameObject);
    }
    
    private void HandleHoldNoteLateRelease(HoldNote note)
    {
        AddScore(25);
        feedbackManager.ShowFeedback(note.isRight ? "right" : "left", "Late");
        Destroy(note.gameObject);
    }
}