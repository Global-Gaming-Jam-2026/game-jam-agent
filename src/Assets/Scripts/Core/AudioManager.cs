using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Centralized audio management for the game.
/// Handles SFX, music, and volume control.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private int sfxPoolSize = 10;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip battleMusic;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip defeatMusic;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip menuClick;
    [SerializeField] private AudioClip menuHover;

    private List<AudioSource> sfxPool;
    private int currentPoolIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSources();
        LoadVolumeSettings();
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        sfxPool = new List<AudioSource>();
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject poolObj = new GameObject($"SFXPool_{i}");
            poolObj.transform.SetParent(transform);
            var source = poolObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxPool.Add(source);
        }
    }

    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ApplyVolumeSettings();
    }

    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void ApplyVolumeSettings()
    {
        if (musicSource != null)
        {
            musicSource.volume = masterVolume * musicVolume;
        }
    }

    public void PlayMusic(MusicType type)
    {
        AudioClip clip = type switch
        {
            MusicType.Menu => menuMusic,
            MusicType.Battle => battleMusic,
            MusicType.Victory => victoryMusic,
            MusicType.Defeat => defeatMusic,
            _ => null
        };

        if (clip != null && musicSource != null)
        {
            if (musicSource.clip == clip && musicSource.isPlaying)
                return;

            musicSource.clip = clip;
            musicSource.volume = masterVolume * musicVolume;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null) return;

        AudioSource source = sfxPool[currentPoolIndex];
        currentPoolIndex = (currentPoolIndex + 1) % sfxPool.Count;

        source.clip = clip;
        source.volume = masterVolume * sfxVolume * volumeMultiplier;
        source.pitch = 1f;
        source.Play();
    }

    public void PlaySFXWithPitch(AudioClip clip, float pitchMin, float pitchMax, float volumeMultiplier = 1f)
    {
        if (clip == null) return;

        AudioSource source = sfxPool[currentPoolIndex];
        currentPoolIndex = (currentPoolIndex + 1) % sfxPool.Count;

        source.clip = clip;
        source.volume = masterVolume * sfxVolume * volumeMultiplier;
        source.pitch = Random.Range(pitchMin, pitchMax);
        source.Play();
    }

    public void PlayUISound(UISoundType type)
    {
        AudioClip clip = type switch
        {
            UISoundType.Click => menuClick,
            UISoundType.Hover => menuHover,
            _ => null
        };

        if (clip != null)
        {
            PlaySFX(clip, 0.8f);
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        SaveVolumeSettings();
    }
}

public enum MusicType
{
    Menu,
    Battle,
    Victory,
    Defeat
}

public enum UISoundType
{
    Click,
    Hover
}
