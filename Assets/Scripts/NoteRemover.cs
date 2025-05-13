using UnityEngine;

public class NoteRemover : MonoBehaviour
{
    public BeatScroller beatScroller;
    public string direction; // Set this to "left" or "right" in the Inspector

    public void RemoveNote()
    {
        if (beatScroller != null)
        {
            beatScroller.RemoveNoteByDirection(direction);
        }
        else
        {
            Debug.LogWarning("BeatScroller not assigned in NoteRemover.");
        }
    }
}
