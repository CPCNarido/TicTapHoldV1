using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer theSR;
    public Sprite defaultImage;
    public Sprite pressedImage;

    private bool isPressed = false;

    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Check for touch input
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                // Check if the touch is on this object
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    OnButtonPressed();
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (isPressed)
                {
                    OnButtonReleased();
                }
            }
        }
    }

    public void OnButtonPressed()
    {
        isPressed = true;
        theSR.sprite = pressedImage;
    }

    public void OnButtonReleased()
    {
        isPressed = false;
        theSR.sprite = defaultImage;
    }
}