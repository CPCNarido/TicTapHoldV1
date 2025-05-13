using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    public float beatTempo; // Beats per minute
    public GameObject leftNotePrefab;
    public GameObject rightNotePrefab;
    public Transform centerPosition;
    public Transform leftTarget;
    public Transform rightTarget;

    private bool hasStarted = false;
    private NoteConfig noteConfig;
    private int noteIndex = 0;
    private List<GameObject> activeNotes = new List<GameObject>();

    void Start()
    {
        beatTempo = beatTempo / 60f; // Convert BPM to units per second
        Debug.Log($"Beat tempo set to {beatTempo} units per second.");
        LoadNoteConfig();
    }

    void Update()
    {
        if (!hasStarted)
        {
            if (Input.touchCount > 0 || Input.anyKeyDown) // Can auto-start with touch or any key
            {
                hasStarted = true;
                Debug.Log("BeatScroller started.");
            }
        }
        else
        {
            // Ensure noteConfig is not null before accessing it
            if (noteConfig == null || noteConfig.notes == null)
            {
                Debug.LogError("NoteConfig is null or not properly loaded!");
                return;
            }

            while (noteIndex < noteConfig.notes.Count &&
                Time.timeSinceLevelLoad >= noteConfig.notes[noteIndex].time)
            {
                SpawnNote(noteConfig.notes[noteIndex]);
                noteIndex++;
            }

            // Check for user input to remove or hold notes
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                HandleHoldNote("left", Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                HandleHoldNote("right", Time.deltaTime);
            }
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
                return;
            }
        }
        Debug.Log($"No {direction} note to remove.");
    }

    void LoadNoteConfig()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "noteConfig.json");
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

    void SpawnNote(Note note)
    {
        GameObject prefab = note.direction == "left" ? leftNotePrefab : rightNotePrefab;
        if (prefab == null)
        {
            Debug.LogError($"Prefab for {note.direction} notes is not assigned!");
            return;
        }

        Vector3 spawnPosition = new Vector3(centerPosition.position.x, centerPosition.position.y, -5f);
        Vector3 targetPosition = note.direction == "left"
            ? new Vector3(leftTarget.position.x, leftTarget.position.y, -5f)
            : new Vector3(rightTarget.position.x, rightTarget.position.y, -5f);

        GameObject spawnedNote = Instantiate(prefab, spawnPosition, Quaternion.identity);
        NoteMover noteMover = spawnedNote.GetComponent<NoteMover>();

        if (noteMover != null)
        {
            bool isHoldNote = note.holdDuration > 0; // Check if it's a hold note
            noteMover.Initialize(targetPosition, beatTempo, isHoldNote, note.holdDuration);

            if (isHoldNote)
            {
                // Stretch the note horizontally based on the hold duration
                float scaleFactor = note.holdDuration * beatTempo; // Adjust scaling based on tempo
                spawnedNote.transform.localScale = new Vector3(scaleFactor, spawnedNote.transform.localScale.y, spawnedNote.transform.localScale.z);
            }

            activeNotes.Add(spawnedNote);
        }
        else
        {
            Debug.LogError("Spawned note does not have a NoteMover component!");
        }
    }

    IEnumerator SpawnHoldSegments(GameObject parentNote, float holdDuration)
    {
        float segmentInterval = 0.1f; // Time between each segment spawn
        float elapsedTime = 0f;

        while (elapsedTime < holdDuration)
        {
            // Spawn a segment as a child of the parent note
            GameObject segment = Instantiate(parentNote, parentNote.transform.position, Quaternion.identity);
            segment.transform.SetParent(parentNote.transform);

            // Offset the segment's position to create a "trail"
            segment.transform.localPosition += new Vector3(-elapsedTime * beatTempo, 0, 0);

            elapsedTime += segmentInterval;
            yield return new WaitForSeconds(segmentInterval);
        }
    }

    void HandleHoldNote(string direction, float deltaTime)
    {
        foreach (GameObject note in activeNotes)
        {
            NoteMover noteMover = note.GetComponent<NoteMover>();
            if (noteMover != null && noteMover.GetDirection() == direction && noteMover.IsAtTarget())
            {
                if (noteMover.IsHoldNote())
                {
                    noteMover.IncrementHoldProgress(deltaTime);
                    if (noteMover.IsHoldComplete())
                    {
                        Debug.Log($"Hold note {direction} completed!");
                        activeNotes.Remove(note);
                        Destroy(note);
                        return;
                    }
                }
                else
                {
                    Debug.Log($"Tap note {direction} removed!");
                    activeNotes.Remove(note);
                    Destroy(note);
                    return;
                }
            }
        }
    }
}