using UnityEngine;

/// <summary>
/// Persistent background-music player. Survives scene loads (DontDestroyOnLoad)
/// so the track keeps playing from the menu into gameplay. Assign a clip to
/// defaultMusic (or call PlayMusic) once an actual BGM asset is available.
/// Volume is stored in PlayerPrefs ("MusicVolume") and driven by the settings slider.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] private AudioClip defaultMusic;

    private AudioSource bgmSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource = GetComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);

        if (defaultMusic != null)
            PlayMusic(defaultMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void SetMusicVolume(float v)
    {
        if (bgmSource != null) bgmSource.volume = v;
        PlayerPrefs.SetFloat("MusicVolume", v);
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", 0.7f);
    }
}
