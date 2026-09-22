using System;
using System.IO;
using UnityEngine;

/// <summary>
/// 數值覆寫層的讀取入口。程式碼一律用 GameTuning.Player.xxx 取值，
/// 不必關心 JSON 在不在、有沒有那個欄位。
/// </summary>
public static class GameTuning
{
    public const string FileName = "tuning.json";

    private static TuningData _current = TuningData.CreateDefaults();

    /// <summary>目前生效的數值（永遠不為 null）。</summary>
    public static TuningData Current { get { return _current; } }

    public static PlayerTuning Player { get { return _current.player; } }
    public static SkeletonTuning Skeleton { get { return _current.skeleton; } }
    public static BossTuning Boss { get { return _current.boss; } }

    /// <summary>重新載入完成時觸發（首次載入也會觸發）。</summary>
    public static event Action OnReloaded;

    /// <summary>最近一次載入是否真的讀到檔案。</summary>
    public static bool FileLoaded { get; private set; }

    /// <summary>最近一次載入的結果訊息，給畫面提示用。</summary>
    public static string LastMessage { get; private set; }

    public static string FilePath
    {
        get { return Path.Combine(Application.streamingAssetsPath, FileName); }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void BootLoad()
    {
        Reload();
    }

    /// <summary>
    /// 重讀 tuning.json。讀不到或格式壞掉時退回預設值並回傳 false，
    /// 不會因為一個打錯字的 JSON 就讓遊戲炸掉。
    /// </summary>
    public static bool Reload()
    {
        TuningData fresh = TuningData.CreateDefaults();
        string path = FilePath;

        try
        {
            if (!File.Exists(path))
            {
                _current = fresh;
                FileLoaded = false;
                LastMessage = "找不到 " + FileName + "，使用內建預設值";
                RaiseReloaded();
                return false;
            }

            string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
            JsonUtility.FromJsonOverwrite(json, fresh);

            _current = fresh;
            FileLoaded = true;
            LastMessage = FileName + " 已套用";
            RaiseReloaded();
            return true;
        }
        catch (Exception e)
        {
            _current = fresh;
            FileLoaded = false;
            LastMessage = FileName + " 解析失敗，已退回預設值：" + e.Message;
            Debug.LogWarning("[GameTuning] " + LastMessage);
            RaiseReloaded();
            return false;
        }
    }

    private static void RaiseReloaded()
    {
        Action handler = OnReloaded;
        if (handler != null)
            handler();
    }
}
