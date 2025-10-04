using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Levels : MonoBehaviour
{
    [Header("Level Buttons")]
    public Button[] levelButtons;

    [Header("Level Scene Names")]
    public string[] levelSceneNames;

    void Start()
    {
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i < unlockedLevels)
            {
                levelButtons[i].interactable = true;

                int index = i;
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
