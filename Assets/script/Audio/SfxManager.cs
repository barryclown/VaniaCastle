using UnityEngine;

/// <summary>
/// 音效（SFX）分軌播放器，與 BGM 分開的音量軌（PlayerPrefs "SfxVolume"，由設置面板的 SFX slider 控制）。
/// 內建以程式生成的簡單音效（打擊/跳躍），之後有正式音效資產可改 PlaySfx(clip)。
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SfxManager : MonoBehaviour
{
    public static SfxManager instance;

    private AudioSource sfxSource;
    private AudioClip hitClip;
    private AudioClip jumpClip;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = GetComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = PlayerPrefs.GetFloat("SfxVolume", 0.8f);

        hitClip = MakeHit();
        jumpClip = MakeBlip(660f, 0.12f);
    }

    public void PlaySfx(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    public void PlayHit() => PlaySfx(hitClip);
    public void PlayJump() => PlaySfx(jumpClip, 0.6f);

    public void SetSfxVolume(float v)
    {
        if (sfxSource != null) sfxSource.volume = v;
        PlayerPrefs.SetFloat("SfxVolume", v);
    }

    public float GetSfxVolume()
    {
        return PlayerPrefs.GetFloat("SfxVolume", 0.8f);
    }

    // ===== 以程式合成簡單音效（免外部資產） =====
    private static AudioClip MakeHit()
    {
        int rate = 44100;
        float dur = 0.14f;
        int n = (int)(rate * dur);
        var data = new float[n];
        var rnd = new System.Random(12345);
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)rate;
            float env = Mathf.Exp(-t * 32f);                 // 快速衰減
            float noise = (float)(rnd.NextDouble() * 2.0 - 1.0);
            float tone = Mathf.Sin(2f * Mathf.PI * 180f * t); // 低頻悶響
            data[i] = (noise * 0.5f + tone * 0.5f) * env * 0.7f;
        }
        var clip = AudioClip.Create("sfx_hit", n, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private static AudioClip MakeBlip(float freq, float dur)
    {
        int rate = 44100;
        int n = (int)(rate * dur);
        var data = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)rate;
            float env = Mathf.Exp(-t * 14f);
            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.6f;
        }
        var clip = AudioClip.Create("sfx_blip", n, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
