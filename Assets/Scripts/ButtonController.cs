using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer theSR;
    public Sprite defaultImage;
    public Sprite pressedImage;

    private bool isPressed = false;

    [Header("Touch Settings")]
    public LayerMask buttonLayer;

    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();

        if (theSR == null)
        {
            Debug.LogError("SpriteRenderer not found!");
        }

        if (defaultImage == null || pressedImage == null)
        {
            Debug.LogError("Please assign both defaultImage and pressedImage in the Inspector.");
        }
    }

    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, buttonLayer);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    OnButtonPressed();
                }
            }
            else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isPressed)
            {
                OnButtonReleased();
            }
        }
    }

    public void OnButtonPressed()
    {
        if (!isPressed)
        {
            isPressed = true;
            theSR.sprite = pressedImage;

            // Check for overlapping notes
            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, theSR.bounds.size, 0f);
            foreach (var hit in hits)
            {
                NoteMover note = hit.GetComponent<NoteMover>();
                if (note != null && note.IsAtTarget())
                {
                    // Calculate accuracy and trigger scoring
                    string accuracy = note.GetAccuracyZone();
                    note.TriggerNoteHit(accuracy);
                    note.StartMovingOffScreen();
                    break; // Only process one note per press
                }
            }
        }
    }

    public void OnButtonReleased()
    {
        if (isPressed)
        {
            isPressed = false;

            theSR.sprite = defaultImage;
        }
    }
}
