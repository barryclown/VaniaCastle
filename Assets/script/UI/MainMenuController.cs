using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Drives the title-screen main menu: start, settings panel and quit.
/// Wire the public methods to the UI Button onClick events.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SampleScene";
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        Time.timeScale = 1f;
        float v = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        AudioListener.volume = v;
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(v);
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
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
