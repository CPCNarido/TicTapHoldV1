using System;
using System.Collections.Generic;

[Serializable]
public class NoteConfig
{
    public List<Note> notes; // List of notes in the configuration
    public HighScore highScore; // High score data
}

[System.Serializable]
public class HighScore
{
    public int best;
    public int current;
}