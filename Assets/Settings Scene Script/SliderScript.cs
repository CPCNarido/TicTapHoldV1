using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import the TextMeshPro namespace

public class SliderScript : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _sliderText; // Reference to the TextMeshProUGUI component

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the text with the slider's current value
        _sliderText.text = $"{_slider.value:0.00}%";

        // Add a listener to update the text when the slider value changes
        _slider.onValueChanged.AddListener((v) =>
        {
            _sliderText.text = $"{v:0.00}%"; // Format the slider value as a percentage
        });
    }

    // Update is called once per frame
    void Update()
    {
        // Optional: Add any additional logic here if needed
    }
}
