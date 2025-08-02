using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Levels : MonoBehaviour
{
    [Header("Level Buttons (Assign 12 Buttons in Order)")]
    public Button[] levelButtons; // Assign all 12 buttons in the Inspector

    [Header("Level Scene Names (Match Order of Buttons)")]
    public string[] levelSceneNames; // Add all 12 scene names in correct order

    void Start()
    {
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i < unlockedLevels)
            {
                levelButtons[i].interactable = true;

                int index = i; // local copy for lambda
                levelButtons[i].onClick.AddListener(() => LoadLevel(index));
            }
            else
            {
                levelButtons[i].interactable = false;
            }
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelSceneNames.Length)
        {
            SceneManager.LoadScene(levelSceneNames[levelIndex]);
        }
    }

    public void ResetProgress()
    {
        PlayerPrefs.SetInt("UnlockedLevels", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
