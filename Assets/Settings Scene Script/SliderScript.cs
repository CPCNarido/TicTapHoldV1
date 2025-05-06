using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import the TextMeshPro namespace

public class SliderScript : MonoBehaviour
{
    [SerializeField] private Slider _slider; // Reference to the Slider component
    [SerializeField] private TextMeshProUGUI _sliderText; // Reference to the TextMeshProUGUI component

    // Start is called before the first frame update
    void Start()
    {
        if (_slider == null || _sliderText == null)
        {
            Debug.LogError("Slider or SliderText is not assigned in the Inspector!");
            return;
        }

        // Initialize the text with the slider's current value
        _sliderText.text = $"{_slider.value:0}%";

        // Add listener to update the text when the slider value changes
        _slider.onValueChanged.AddListener((v) => {
            Debug.Log($"Slider value changed: {v}");
            _sliderText.text = $"{v:0}%";
        });
    }
}
