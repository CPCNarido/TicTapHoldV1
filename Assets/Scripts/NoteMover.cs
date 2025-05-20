using System;
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

    public delegate void NoteMissedHandler(string direction);
    public static event NoteMissedHandler OnNoteMissed;
    public static event Action<string, string> OnNoteHit;
    public static event Action<string, string> NoteScored;

    private Vector3 earlyTarget, perfectTarget, lateTarget;

    public void Initialize(Vector3 early, Vector3 perfect, Vector3 late, float tempo, string direction, bool holdNote = false, float holdTime = 0f)
    {
        earlyTarget = early;
        perfectTarget = perfect;
        lateTarget = late;
        targetPosition = perfect;
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
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (IsAtTarget() && !isHoldNote)
            {
                StartMovingOffScreen();
            }
        }
        else
        {
            Vector3 offScreenPosition = direction == "left" ? new Vector3(-15, 0, transform.position.z) : new Vector3(15, 0, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, offScreenPosition, speed * Time.deltaTime);

            if ((direction == "left" && transform.position.x <= -11) ||
                (direction == "right" && transform.position.x >= 11))
            {
                OnNoteMissed?.Invoke(direction);
            }

            if (Vector3.Distance(transform.position, offScreenPosition) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }

    public string GetAccuracyZone()
    {
        float distEarly = Vector3.Distance(transform.position, earlyTarget);
        float distPerfect = Vector3.Distance(transform.position, perfectTarget);
        float distLate = Vector3.Distance(transform.position, lateTarget);

        float minDist = Mathf.Min(distEarly, distPerfect, distLate);

        if (minDist == distPerfect) return "Perfect";
        if (minDist == distEarly) return "Early";
        return "Late";
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

    public void TriggerNoteHit(string accuracy)
    {
        OnNoteHit?.Invoke(direction, accuracy);
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