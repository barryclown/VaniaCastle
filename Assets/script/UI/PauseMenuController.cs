using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// In-game pause menu. Toggles on Escape, freezes time while paused,
/// and offers resume / return to main menu / quit.
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool paused;

    private void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Toggle();
    }

    public void Toggle()
    {
        if (paused) Resume();
        else Pause();
    }

    public void Pause()
    {
        paused = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        paused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
