using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 熱重載：遊戲執行中偵測 StreamingAssets/tuning.json 是否被改過，
/// 改了就重讀，並把值推回場景上的 Player / Skeleton / Boss。
/// 不必停 Play、不必重 Build，存檔就生效。
/// 狀態機裡的數值是每幀直接讀 GameTuning，天生就會跟著變；
/// 只有 Inspector 型欄位需要在這裡「推」一次。
/// </summary>
public class TuningRuntime : MonoBehaviour
{
    private static TuningRuntime _instance;

    private long _lastStampTicks;
    private WaitForSeconds _wait;
    private float _toastUntil;
    private string _toastText;
    private GUIStyle _toastStyle;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (_instance != null)
            return;

        GameObject go = new GameObject("~TuningRuntime");
        DontDestroyOnLoad(go);
        _instance = go.AddComponent<TuningRuntime>();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        _lastStampTicks = ReadStampTicks();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplyToScene();

        if (GameTuning.Current.hotReload.enabled)
            StartCoroutine(WatchFile());
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyToScene();
    }

    private static long ReadStampTicks()
    {
        try
        {
            string p = GameTuning.FilePath;
            if (!File.Exists(p))
                return 0L;

            return File.GetLastWriteTimeUtc(p).Ticks;
        }
        catch
        {
            return 0L;
        }
    }

    private IEnumerator WatchFile()
    {
        float poll = GameTuning.Current.hotReload.pollSeconds;
        if (poll < 0.1f)
            poll = 0.1f;

        _wait = new WaitForSeconds(poll);   // 快取，避免每圈 new（零 GC 規範）

        while (true)
        {
            yield return _wait;

            long ticks = ReadStampTicks();
            if (ticks == 0L || ticks == _lastStampTicks)
                continue;

            _lastStampTicks = ticks;
            GameTuning.Reload();
            ApplyToScene();

            if (GameTuning.Current.hotReload.showToast)
            {
                _toastText = "\u27F3  " + GameTuning.LastMessage;
                _toastUntil = Time.unscaledTime + 2f;
            }

            Debug.Log("[GameTuning] 熱重載：" + GameTuning.LastMessage);
        }
    }

    /// <summary>把 JSON 有指定的欄位推到場景元件；沒指定的一律不碰 Inspector 值。</summary>
    public static void ApplyToScene()
    {
        TuningData t = GameTuning.Current;

        Player[] players = Object.FindObjectsOfType<Player>(true);
        for (int i = 0; i < players.Length; i++)
        {
            Player p = players[i];
            if (p == null)
                continue;

            if (TuningData.Has(t.player.jumpForce))
                p.jumpForce = t.player.jumpForce;
            if (TuningData.Has(t.player.attackDamage))
                p.attackDamage = t.player.attackDamage;
            if (TuningData.Has(t.player.maxHealth))
                p.SetMaxHealth(t.player.maxHealth);
        }

        Skeleton[] skeletons = Object.FindObjectsOfType<Skeleton>(true);
        for (int i = 0; i < skeletons.Length; i++)
        {
            Skeleton s = skeletons[i];
            if (s == null)
                continue;

            if (TuningData.Has(t.skeleton.attackDamage))
                s.attackDamage = t.skeleton.attackDamage;
            if (TuningData.Has(t.skeleton.maxHealth))
                s.SetMaxHealth(t.skeleton.maxHealth);
            if (TuningData.Has(t.skeleton.attackDistance))
                s.attackDistance = t.skeleton.attackDistance;
            if (TuningData.Has(t.skeleton.stunDuration))
                s.stunDuration = t.skeleton.stunDuration;
        }

        BossTuning bt = t.boss;
        BossController[] bosses = Object.FindObjectsOfType<BossController>(true);
        for (int i = 0; i < bosses.Length; i++)
        {
            BossController b = bosses[i];
            if (b == null)
                continue;

            if (TuningData.Has(bt.aggroRange))
                b.aggroRange = bt.aggroRange;
            if (TuningData.Has(bt.chaseSpeed))
                b.chaseSpeed = bt.chaseSpeed;
            if (TuningData.Has(bt.meleeRange))
                b.meleeRange = bt.meleeRange;
            if (TuningData.Has(bt.meleeCooldown))
                b.meleeCooldown = bt.meleeCooldown;
            if (TuningData.Has(bt.meleeDamage))
                b.meleeDamage = bt.meleeDamage;
            if (TuningData.Has(bt.counterWindow))
                b.counterWindow = bt.counterWindow;
            if (TuningData.Has(bt.rangedCooldown))
                b.rangedCooldown = bt.rangedCooldown;
            if (TuningData.Has(bt.projectileSpeed))
                b.projectileSpeed = bt.projectileSpeed;
            if (TuningData.Has(bt.projectileDamage))
                b.projectileDamage = bt.projectileDamage;
            if (TuningData.Has(bt.maxMinions))
                b.maxMinions = bt.maxMinions;
            if (TuningData.Has(bt.summonCooldown))
                b.summonCooldown = bt.summonCooldown;
            if (TuningData.Has(bt.enrageHpRatio))
                b.enrageHpRatio = bt.enrageHpRatio;
            if (TuningData.Has(bt.enrageCooldownScale))
                b.enrageCooldownScale = bt.enrageCooldownScale;
            if (TuningData.Has(bt.enrageSpeedScale))
                b.enrageSpeedScale = bt.enrageSpeedScale;

            Enemy bossEntity = b.GetComponent<Enemy>();
            if (bossEntity != null && TuningData.Has(bt.maxHealth))
                bossEntity.SetMaxHealth(bt.maxHealth);
        }
    }

    private void OnGUI()
    {
        if (string.IsNullOrEmpty(_toastText) || Time.unscaledTime > _toastUntil)
            return;   // 沒東西畫就立刻退出，不進 GUI 配置流程

        if (_toastStyle == null)
        {
            _toastStyle = new GUIStyle(GUI.skin.box);
            _toastStyle.fontSize = 18;
            _toastStyle.alignment = TextAnchor.MiddleLeft;
            _toastStyle.padding = new RectOffset(12, 12, 8, 8);
        }

        GUI.color = new Color(1f, 1f, 1f, 0.92f);
        GUI.Box(new Rect(16f, 16f, 460f, 40f), _toastText, _toastStyle);
        GUI.color = Color.white;
    }
}
