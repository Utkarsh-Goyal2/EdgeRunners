using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class levelLoader : MonoBehaviour
{
    [Header("Auto-Setup")]
    public Transform buttonContainer; // Parent object containing all level buttons
    public string sceneNamePrefix = "level"; // "level" + text content = "level1"

    void Start()
    {
        SetupLevelButtons();
    }

    void SetupLevelButtons()
    {
        // Find all buttons in the container
        Button[] buttons = buttonContainer.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            // Find the TextMeshPro component on the button or its children
            TMP_Text textComponent = button.GetComponentInChildren<TMP_Text>();

            if (textComponent != null)
            {
                // Get the text content (should be "1", "2", "3", etc.)
                string buttonText = textComponent.text;

                // Try to parse the text as a level number
                if (int.TryParse(buttonText, out int levelNumber))
                {
                    // Check if this level is unlocked
                    bool isUnlocked = IsLevelUnlocked(levelNumber);
                    // Set up the click listener
                    if (isUnlocked)
                    {
                        // Capture the level number for the closure
                        int capturedLevel = levelNumber;
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(() => LoadLevel(capturedLevel));
                    }
                    else
                    {
                        // Disable locked buttons
                        button.interactable = false;
                    }
                }
                else
                {
                    Debug.LogWarning($"Button text '{buttonText}' is not a valid level number!");
                }
            }
            else
            {
                Debug.LogWarning($"Button '{button.name}' doesn't have a TextMeshPro component!");
            }
        }
    }

    bool IsLevelUnlocked(int levelNumber)
    {
        // Level 1 is always unlocked
        if (levelNumber == 1) return true;

        // Other levels unlock based on cleared levels
        return ProgressService.ClearedLevels >= (levelNumber - 1);
    }


    void LoadLevel(int levelNumber)
    {
        Time.timeScale = 1f; // Ensure game is unpaused
        string sceneName = sceneNamePrefix + levelNumber;

        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // Call this method to refresh button states (e.g., after completing a level)
    public void RefreshButtonStates()
    {
        SetupLevelButtons();
    }
    public void loadsurvival()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("survival");
    }
}
