
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FeedbackManager : MonoBehaviour
{
    public TextMeshProUGUI leftFeedbackText;
    public TextMeshProUGUI rightFeedbackText;
    public float feedbackDuration = 0.5f;

    public void ShowFeedback(string direction, string result)
    {
        if (direction == "left")
        {
            StartCoroutine(FadeText(leftFeedbackText, result));
            Debug.Log("Left feedback: " + result);
        }
        else if (direction == "right")
        {
            StartCoroutine(FadeText(rightFeedbackText, result));
            Debug.Log("Right feedback: " + result);
        }
    }

    private System.Collections.IEnumerator FadeText(TextMeshProUGUI textObject, string message)
    {
        textObject.text = message;
        textObject.alpha = 1f;

        float timer = 0f;

        while (timer < feedbackDuration)
        {
            timer += Time.deltaTime;
            float fadeAmount = Mathf.Lerp(1f, 0f, timer / feedbackDuration);
            textObject.alpha = fadeAmount;
            yield return null;
        }

        textObject.text = "";
    }
}
