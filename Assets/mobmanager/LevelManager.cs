using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadLevel(int levelNumber)
    {
        Time.timeScale = 1f; // FIXED: Ensure game is unpaused
        string sceneName = "level" + levelNumber;
        SceneManager.LoadScene(sceneName);
    }

    public void Home()  
    {
        Time.timeScale = 1f; // FIXED: Ensure game is unpaused
        SceneManager.LoadScene("Home");
    }

    public void ReloadCurrentScene()
    {
        Time.timeScale = 1f; // FIXED: Ensure game is unpaused
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f; // FIXED: Ensure game is unpaused
        int currentLevel = GetCurrentLevelNumber();
        int nextLevel = currentLevel + 1;
        LoadLevel(nextLevel);
    }

    private int GetCurrentLevelNumber()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene.StartsWith("level"))
        {
            string numberPart = currentScene.Substring(5);
            if (int.TryParse(numberPart, out int levelNum))
                return levelNum;
        }
        return 1;
    }
}
