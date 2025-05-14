using UnityEngine;

public class NoteMover : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private string direction;
    private float holdDuration;
    private float holdProgress;
    private bool isHoldNote;
    private bool isMovingOffScreen = false;

    public void Initialize(Vector3 target, float tempo, string direction, bool holdNote = false, float holdTime = 0f)
    {
        targetPosition = target;
        speed = tempo;
        this.direction = direction.ToLower();
        isHoldNote = holdNote;
        holdDuration = holdTime;
        holdProgress = 0f;
    }

    void Update()
    {
        if (!isMovingOffScreen)
        {
            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Check if the note has reached the target
            if (IsAtTarget() && !isHoldNote)
            {
                // Start moving off-screen if it's not a hold note
                StartMovingOffScreen();
            }
        }
        else
        {
            // Move off-screen
            Vector3 offScreenPosition = direction == "left" ? new Vector3(-60, 0, transform.position.z) : new Vector3(60, 0, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, offScreenPosition, speed * Time.deltaTime);

            // Destroy the note once it reaches the off-screen position
            if (Vector3.Distance(transform.position, offScreenPosition) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }

    public bool IsAtTarget(float threshold = 0.5f)
    {
        return Vector3.Distance(transform.position, targetPosition) < threshold;
    }

    public void StartMovingOffScreen()
    {
        isMovingOffScreen = true;
    }

    public string GetDirection()
    {
        return direction;
    }

    public float GetHoldProgress()
    {
        return holdProgress;
    }

    public float GetHoldDuration()
    {
        return holdDuration;
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