using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 玩家死亡介面：Player.Die() 呼叫 Show() → 顯示 YOU DIED 畫面並凍結遊戲。
/// 按鈕（編輯期序列化持久 listener）：RETRY 重載本關、MAIN MENU 回主選單。
/// 單例（每場景一個，不跨場景保留；重載後新實例會重設 Instance）。
/// </summary>
public class DeathScreenController : MonoBehaviour
{
    public static DeathScreenController Instance { get; private set; }

    [SerializeField] private GameObject deathUI;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool shown;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (deathUI != null) deathUI.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Show()
    {
        if (shown) return;
        shown = true;
        if (deathUI != null) deathUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
