using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager I;

    [System.Serializable]
    public struct NamedClip { public string id; public AudioClip clip; }

    [Header("Libraries")]
    [SerializeField] private NamedClip[] musicLib;
    [SerializeField] private NamedClip[] sfxLib;

    private Dictionary<string, AudioClip> musicMap;
    private Dictionary<string, AudioClip> sfxMap;

    [Header("Sources (auto-created if null)")]
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource sfx;

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private bool enableAutoAdvance = true;
    private Coroutine watchCo;

    private bool appFocused = true;
    private bool osPaused = false;

    void Awake() {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        if (!music) music = gameObject.AddComponent<AudioSource>();
        if (!sfx) sfx = gameObject.AddComponent<AudioSource>();
        music.loop = false;
        music.playOnAwake = false;
        sfx.playOnAwake = false;

        musicMap = new Dictionary<string, AudioClip>(musicLib?.Length ?? 0);
        foreach (var nc in musicLib) if (nc.clip && !string.IsNullOrEmpty(nc.id)) musicMap[nc.id] = nc.clip;

        sfxMap = new Dictionary<string, AudioClip>(sfxLib?.Length ?? 0);
        foreach (var nc in sfxLib) if (nc.clip && !string.IsNullOrEmpty(nc.id)) sfxMap[nc.id] = nc.clip;

        ApplyVolumes();
    }

    void Start() {
        if (musicLib == null || musicLib.Length == 0) return;
        int idx = Random.Range(0, musicLib.Length);
        var clip = musicLib[idx].clip;
        if (clip) PlayMusic(clip, 0.35f);
    }

    void OnApplicationFocus(bool focus) { appFocused = focus; }
    void OnApplicationPause(bool paused) { osPaused = paused; }

    public void PlayMusic(AudioClip clip, float fadeSeconds = 0.35f) {
        if (!clip) return;
        enableAutoAdvance = true;

        if (!music.isPlaying) {
            music.clip = clip;
            music.volume = 0f;
            music.Play();
            StartCoroutine(Fade(music, musicVolume, fadeSeconds));
        } else if (music.clip != clip) {
            StartCoroutine(Crossfade(clip, fadeSeconds));
        }

        RestartWatcher();
    }

    public void PlayMusic(string id, float fadeSeconds = 0.35f) {
        if (musicMap != null && musicMap.TryGetValue(id, out var clip)) PlayMusic(clip, fadeSeconds);
    }

    public void PlayMusic(int index, float fadeSeconds = 0.35f) {
        if (musicLib == null || index < 0 || index >= musicLib.Length) return;
        PlayMusic(musicLib[index].clip, fadeSeconds);
    }

    public void StopMusic(float fadeSeconds = 0.25f) {
        if (!music.isPlaying) return;
        enableAutoAdvance = false;
        StartCoroutine(Fade(music, 0f, fadeSeconds, stopAfter: true));
        RestartWatcher();
    }

    public void NextTrack(float fadeSeconds = 0.35f) {
        PlayNextRandom(exclude: music ? music.clip : null, fadeSeconds);
    }


    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f) {
        if (!clip) return;
        sfx.pitch = pitch;
        sfx.PlayOneShot(clip, sfxVolume * Mathf.Clamp01(volume));
    }

    public void PlaySFX(string id, float volume = 1f, float pitch = 1f) {
        if (sfxMap != null && sfxMap.TryGetValue(id, out var clip)) PlaySFX(clip, volume, pitch);
    }

    public void PlaySFX(int index, float volume = 1f, float pitch = 1f) {
        if (sfxLib == null || index < 0 || index >= sfxLib.Length) return;
        PlaySFX(sfxLib[index].clip, volume, pitch);
    }


    public void SetMusicVolume(float v) { musicVolume = Mathf.Clamp01(v); ApplyVolumes(); }
    public void SetSFXVolume(float v) { sfxVolume = Mathf.Clamp01(v); }

    private void ApplyVolumes() {
        if (music) music.volume = musicVolume;
    }


    private void PlayNextRandom(AudioClip exclude, float fadeSeconds = 0.35f) {
        if (musicLib == null || musicLib.Length == 0) return;

        for (int tries = 0; tries < 6; tries++) {
            var pick = musicLib[Random.Range(0, musicLib.Length)].clip;
            if (pick && pick != exclude) { PlayMusic(pick, fadeSeconds); return; }
        }
        var first = musicLib[0].clip;
        if (first) PlayMusic(first, fadeSeconds);
    }

    private void RestartWatcher() {
        if (watchCo != null) StopCoroutine(watchCo);
        watchCo = StartCoroutine(WatchMusicEnd());
    }

    private IEnumerator WatchMusicEnd() {
        while (true) {
            if (!enableAutoAdvance) yield break;
            if (!music || !music.clip) { yield return null; continue; }

            // if app is unfocused or OS-paused, do nothing
            if (!appFocused || osPaused || AudioListener.pause) { yield return null; continue; }

            // consider “ended” only when playback stopped AND cursor reset to start
            bool ended = !music.isPlaying && music.timeSamples == 0 && music.clip;
            if (ended) {
                PlayNextRandom(exclude: music.clip);
                yield break; // next PlayMusic restarts watcher
            }
            yield return null;
        }
    }


    private IEnumerator Crossfade(AudioClip next, float seconds) {
        var from = music;
        float start = from.volume;
        float t = 0f;

        var incoming = gameObject.AddComponent<AudioSource>();
        incoming.loop = false; incoming.playOnAwake = false;
        incoming.clip = next; incoming.volume = 0f; incoming.Play();

        while (t < seconds) {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / seconds);
            from.volume = Mathf.Lerp(start, 0f, k);
            incoming.volume = Mathf.Lerp(0f, musicVolume, k);
            yield return null;
        }

        from.Stop();
        from.clip = next;
        from.volume = musicVolume;

        Destroy(incoming);
        if (!from.isPlaying) from.Play();

        RestartWatcher();
    }

    private IEnumerator Fade(AudioSource src, float target, float seconds, bool stopAfter = false) {
        float start = src.volume, t = 0f;
        while (t < seconds) {
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(start, target, Mathf.Clamp01(t / seconds));
            yield return null;
        }
        src.volume = target;
        if (stopAfter) src.Stop();
    }
}
