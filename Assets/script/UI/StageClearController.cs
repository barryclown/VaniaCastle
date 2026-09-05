using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 關卡通關控制：監看 boss 是否被消滅。boss 是 Enemy，死亡時 Die() 會 Destroy 自己，
/// 因此這裡偵測 boss 參照變 null 即視為通關 → 顯示 STAGE CLEAR 畫面並凍結遊戲，
/// 按鈕（runtime 綁定，免 serialized listener）回主選單。
/// </summary>
public class StageClearController : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject stageClearUI;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool bossWasAlive = false;
    private bool cleared = false;

    private void Start()
    {
        bossWasAlive = boss != null;
        if (stageClearUI != null) stageClearUI.SetActive(false);
        // MAIN MENU 按鈕的 onClick 用序列化的持久 listener 接（編輯期 UnityEventTools 指派，同既有選單做法）

        if (!bossWasAlive)
            Debug.LogWarning("StageClearController: boss 參照未指派，通關判定不會生效。");
    }

    private void Update()
    {
        if (cleared || !bossWasAlive) return;

        // boss 死亡會 Destroy 自己 → 參照變 null 即通關
        if (boss == null)
        {
            cleared = true;
            if (stageClearUI != null) stageClearUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
