using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine;
using TMPro;

public class BeatScroller : MonoBehaviour
{
    public float beatTempo; // BPM
    public GameObject leftNotePrefab;
    public GameObject rightNotePrefab;
    public Transform centerPosition;
    public Transform leftTarget;
    public Transform rightTarget;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;


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
    private const int streakThreshold = 10; // Increase multiplier every 10 successful hits


    void Start()
    {
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
        }
    }

    void LoadNoteConfig()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "mistydrive_easy.json");
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
            Debug.LogError("Note configuration file not found!");
        }
    }

void SpawnNote(Note note, float elapsedTime)
{
    GameObject prefab = note.direction == "left" ? leftNotePrefab : rightNotePrefab;
    if (prefab == null)
    {
        Debug.LogError($"Prefab for {note.direction} notes is not assigned!");
        return;
    }

    // Start position (center of the screen)
    Vector3 spawnPosition = new Vector3(centerPosition.position.x, centerPosition.position.y, -5f);

    // Target position based on the note direction
    Vector3 targetPosition = note.direction == "left"
        ? new Vector3(leftTarget.position.x, leftTarget.position.y, -5f)
        : new Vector3(rightTarget.position.x, rightTarget.position.y, -5f);

    // Distance and travel time for the note (based on the beatTempo)
    float distanceToTarget = Mathf.Abs(spawnPosition.x - targetPosition.x);
    float travelTime = distanceToTarget / beatTempo;
    float adjustedSpawnTime = note.time - travelTime;

    // Only spawn the note when it's time
    if (elapsedTime < adjustedSpawnTime)
        return;

    // Instantiate the note prefab
    GameObject spawnedNote = Instantiate(prefab, spawnPosition, Quaternion.identity);

    // Access the NoteMover component to initialize the note's movement
    NoteMover noteMover = spawnedNote.GetComponent<NoteMover>();

    if (noteMover != null)
    {
        bool isHoldNote = note.holdDuration > 0;
        noteMover.Initialize(targetPosition, beatTempo, note.direction, isHoldNote, note.holdDuration);

        // If it's a hold note, we scale it based on the duration
        if (isHoldNote)
        {
            float scaleFactor = note.holdDuration * beatTempo;
            spawnedNote.transform.localScale = new Vector3(scaleFactor, spawnedNote.transform.localScale.y, spawnedNote.transform.localScale.z);
        }

        // Add to the list of active notes
        activeNotes.Add(spawnedNote);
    }
    else
    {
        Debug.LogError("Spawned note does not have a NoteMover component!");
    }
}

    public void RemoveNoteByDirection(string direction)
    {
        for (int i = 0; i < activeNotes.Count; i++)
        {
            GameObject note = activeNotes[i];
            NoteMover noteMover = note.GetComponent<NoteMover>();
            if (noteMover != null && noteMover.GetDirection() == direction)
            {
                Debug.Log($"Removing {direction} note.");
                activeNotes.RemoveAt(i);
                Destroy(note);
                AddScore(50); // Add score for tap note
                Debug.Log($"Note removed. Current score: {score}");

                if (noteHitSound != null)
                {
                    noteHitSound.Play();
                }

                return;
            }
        }
        Debug.Log($"No {direction} note to remove.");
    }

    void HandleHoldNote(string direction, float deltaTime)
    {
        Debug.Log($"Checking for {direction} note to hold...");

        for (int i = 0; i < activeNotes.Count; i++)
        {
            GameObject note = activeNotes[i];
            NoteMover noteMover = note.GetComponent<NoteMover>();

            if (noteMover != null && noteMover.GetDirection() == direction)
            {
                Debug.Log($"Found {direction} note. IsAtTarget: {noteMover.IsAtTarget()}, IsHoldNote: {noteMover.IsHoldNote()}");

                if (noteMover.IsAtTarget())
                {
                    if (noteMover.IsHoldNote())
                    {
                        noteMover.IncrementHoldProgress(deltaTime);
                        Debug.Log($"Holding {direction} note. Progress: {noteMover.GetHoldProgress():F2}/{noteMover.GetHoldDuration():F2}");

                        if (noteMover.IsHoldComplete())
                        {
                            Debug.Log($"Hold note {direction} completed!");
                            activeNotes.RemoveAt(i);
                            Destroy(note);

                            AddScore(100);
                            if (holdNoteSound != null) holdNoteSound.Play();

                            return;
                        }
                    }
                    else
                    {
                        Debug.Log($"Tap note {direction} hit!");
                        activeNotes.RemoveAt(i);
                        Destroy(note);

                        AddScore(50);
                        if (noteHitSound != null) noteHitSound.Play();

                        return;
                    }
                }
            }
        }

        Debug.Log($"No valid {direction} note found to hold/tap.");
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
        else
        {
            Debug.LogWarning("ScoreText is not assigned!");
        }

        if (multiplierText != null)
        {
            multiplierText.text = $"Multiplier: x{multiplier}";
            Debug.Log($"[UI] Multiplier updated to x{multiplier}");
        }
        else
        {
            Debug.LogWarning("MultiplierText is not assigned!");
        }
    }


    void ResetStreak()
    {
        streak = 0;
        multiplier = 1;
        Debug.Log("Streak reset. Multiplier reset to 1.");
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
}