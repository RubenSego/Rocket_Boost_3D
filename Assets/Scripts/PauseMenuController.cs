using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [Header("Pause UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "Menu";

    [Header("Panel Block")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject levelCompletedPanel;

    [SerializeField] private PlayerHealth playerHealth;

    public static bool isPaused = false;

    private void Start()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (deathPanel != null && deathPanel.activeInHierarchy)
            {
                return;
            }

            if (levelCompletedPanel != null && levelCompletedPanel.activeInHierarchy)
            {
                return;
            }

            PlayerHealth health = playerHealth != null
                ? playerHealth
                : FindAnyObjectByType<PlayerHealth>(FindObjectsInactive.Include);

            if (health != null && health.IsDead)
            {
                return;
            }

            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SettingsPanelMenu()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}