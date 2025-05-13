using UnityEngine;

public class NoteMover : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private string direction;
    private float holdDuration; // Duration the player needs to hold the button
    private float holdProgress; // How long the player has held the button
    private bool isHoldNote;    // Whether this is a hold note

    void Start()
    {
        var image = GetComponent<UnityEngine.UI.Image>();
        if (image != null) image.raycastTarget = false;

        var text = GetComponent<UnityEngine.UI.Text>();
        if (text != null) text.raycastTarget = false;
    }

    public void Initialize(Vector3 target, float tempo, bool holdNote = false, float holdTime = 0f)
    {
        targetPosition = target;
        speed = tempo;
        direction = target.x < 0 ? "left" : "right"; // Determine direction based on target position
        isHoldNote = holdNote;
        holdDuration = holdTime;
        holdProgress = 0f;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // For hold notes, check if the player is holding the button
        if (isHoldNote && holdProgress >= holdDuration)
        {
            Debug.Log("Hold note completed!");
            Destroy(gameObject); // Destroy the note when the hold is complete
        }
    }

    public bool IsAtTarget(float threshold = 0.5f)
    {
        return Vector3.Distance(transform.position, targetPosition) < threshold;
    }

    public string GetDirection()
    {
        return direction;
    }

    public bool IsHoldNote()
    {
        return isHoldNote;
    }

    public void IncrementHoldProgress(float deltaTime)
    {
        if (isHoldNote)
        {
            holdProgress += deltaTime;
        }
    }

    public bool IsHoldComplete()
    {
        return holdProgress >= holdDuration;
    }
}