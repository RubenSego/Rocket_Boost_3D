using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string firstLevelSceneName = "Level1";

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainButtonsPanel;
    [SerializeField] private GameObject levelsPanel;
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject titlePanel;        

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo de Sparrow's Peak...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OpenLevels()
    {
        mainButtonsPanel.SetActive(false);
        titlePanel.SetActive(false);
        levelsPanel.SetActive(true);
    }

    public void OpenStory()
    {
        mainButtonsPanel.SetActive(false);
        titlePanel.SetActive(false);        
        storyPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        mainButtonsPanel.SetActive(false);
        titlePanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        levelsPanel.SetActive(false);
        storyPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainButtonsPanel.SetActive(true);
        titlePanel.SetActive(true);
    }

    // To load a specific level.
    public void LoadSpecificLevel(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}