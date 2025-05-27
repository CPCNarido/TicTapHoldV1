using UnityEngine;
using System;

public class HoldNote : MonoBehaviour
{
    public float holdDuration;
    public bool isRight;
    public float speed;
    public Vector3 targetPosition;
    public Transform bodyTransform;
    public Transform headTransform;
    public Transform endTransform;

    private float holdStartTime;
    private bool isHolding = false;
    private bool isCompleted = false;
    private bool isAtButton = false;
    private float shrinkTimer = 0f;
    private float initialBodyLength;

    public static event Action<HoldNote> OnHoldComplete;
    public static event Action<HoldNote> OnHoldEarlyRelease;
    public static event Action<HoldNote> OnHoldLateRelease;

    void Start()
    {
        initialBodyLength = Mathf.Clamp(holdDuration, 0.5f, 8f);
    }

    void Update()
    {
        if (!isAtButton)
        {
            // Move the note towards the button
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isAtButton = true;
                shrinkTimer = 0f;
                if (isHolding)
                    holdStartTime = Time.time;
            }
        }
        else
        {
            // Head stays at button, body shrinks toward head, tail moves toward head
            if (isHolding && !isCompleted)
            {
                shrinkTimer += Time.deltaTime;
                float progress = Mathf.Clamp01(shrinkTimer / holdDuration);
                float bodyLength = Mathf.Lerp(initialBodyLength, 0f, progress);

                if (bodyTransform != null)
                    bodyTransform.localScale = new Vector3(bodyLength, bodyTransform.localScale.y, bodyTransform.localScale.z);

                if (endTransform != null)
                {
                    float tip = isRight ? bodyLength : -bodyLength;
                    endTransform.localPosition = new Vector3(tip, 0, 0);
                }

                // When head and tail meet (bodyLength ~ 0)
                if (bodyLength <= 0.01f)
                {
                    isCompleted = true;
                    OnHoldComplete?.Invoke(this);
                }
            }
            else if (!isHolding && !isCompleted)
            {
                // Released early
                isCompleted = true;
                OnHoldEarlyRelease?.Invoke(this);
            }
        }
    }

    public void StartHolding()
    {
        isHolding = true;
        if (isAtButton)
            holdStartTime = Time.time;
    }

    public void StopHolding()
    {
        if (!isCompleted)
        {
            isHolding = false;
            if (isAtButton && shrinkTimer < holdDuration)
            {
                isCompleted = true;
                OnHoldEarlyRelease?.Invoke(this);
            }
        }
    }
}