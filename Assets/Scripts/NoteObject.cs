using System.Collections;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public bool canBePressed;
    public string buttonTag; // Tag for the button corresponding to this note
    public bool isHoldNote; // Indicates if this is a hold note
    public float holdDuration; // Duration for the hold note to shrink
    private bool isShrinking = false;

    // Static flag to ensure only one note is processed at a time
    private static bool isProcessingNote = false;

    void Update()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    // Check if the touch is on the correct button
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                    if (hit.collider != null && hit.collider.CompareTag(buttonTag))
                    {
                        if (canBePressed && !isProcessingNote)
                        {
                            isProcessingNote = true; // Lock processing to this note

                            if (isHoldNote)
                            {
                                StartCoroutine(ShrinkAndRemove());
                            }
                            else
                            {
                                gameObject.SetActive(false);
                                isProcessingNote = false; // Unlock processing immediately for tap notes
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
        {
            canBePressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
        {
            canBePressed = false;
        }
    }

    private IEnumerator ShrinkAndRemove()
    {
        if (isShrinking) yield break;
        isShrinking = true;

        Vector3 originalScale = transform.localScale;
        float elapsedTime = 0f;

        try
        {
            while (elapsedTime < holdDuration)
            {
                float progress = elapsedTime / holdDuration;
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }
        finally
        {
            isProcessingNote = false;
        }
    }

}