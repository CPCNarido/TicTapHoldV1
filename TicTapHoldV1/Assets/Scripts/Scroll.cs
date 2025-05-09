using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public void LoadLevelDescription(int levelIndex)
    {
        PlayerPrefs.SetInt("SelectedLevel", levelIndex);
        SceneManager.LoadScene("LevelDescription");
    }
}