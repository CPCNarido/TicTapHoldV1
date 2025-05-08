using UnityEngine;
using UnityEngine.UI;
public class TabsManager : MonoBehaviour
{
    public GameObject[] Tabs; // Array of tab GameObjects
    public Image[] TabButtons; // Array of tab images for visual feedback
    public Sprite InactiveTabBG,ActiveTabBG; // Sprites for inactive and active tab backgrounds
    public Vector2 InactiveTabButtonsSize, ActiveTabButtonsSize; // Sizes for inactive and active tab buttons
    
    
    public void SwitchToTab(int TabID){
        foreach (GameObject go in Tabs){
            go.SetActive(false); // Deactivate all tabs
        }

        Tabs[TabID].SetActive(true); // Activate the selected tab

           foreach (Image img in TabButtons){
        //     img.rectTransform.sizeDelta = InactiveTabButtonsSize; // Set all tab buttons to inactive size
               img.sprite = InactiveTabBG; // Set all tab buttons to inactive background
           }

        TabButtons[TabID].rectTransform.sizeDelta = ActiveTabButtonsSize; // Set the selected tab button to active size
        TabButtons[TabID].sprite = ActiveTabBG; // Set the selected tab button to active background
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
