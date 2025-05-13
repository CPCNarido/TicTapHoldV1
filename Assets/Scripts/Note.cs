using System;

[Serializable]
public class Note
{
    public string direction; // "left" or "right"
    public float time;       // Time in seconds when the note should spawn
    public float holdDuration; // Duration for hold notes (optional, 0 for tap notes)
}