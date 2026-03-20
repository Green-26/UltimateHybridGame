using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;
    public AudioSource voiceSource;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    [Range(0f, 1f)]
    public float ambientVolume = 1f;
    [Range(0f, 1f)]
    public float voiceVolume = 1f;
    
    [Header("Music Tracks")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    public AudioClip combatMusic;
    public AudioClip bossMusic;
    public AudioClip vehicleMusic;
    public AudioClip victoryMusic;
    public AudioClip gameOverMusic;
    
    [Header("Player Sounds")]
    public AudioClip punchSound;
    public AudioClip kickSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip dashSound;
    public AudioClip rollSound;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip healSound;
    
    [Header("Special Move Sounds")]
    public AudioClip powerPunchSound;
    public AudioClip roundhouseKickSound;
    public AudioClip groundSlamSound;
    public AudioClip ultimateSound;
    public AudioClip comboHitSound;
    public AudioClip comboBreakSound;
    
    [Header("Vehicle Sounds")]
    public AudioClip engineStartSound;
    public AudioClip engineIdleSound;
    public AudioClip engineRevSound;
    public AudioClip machineGunSound;
    public AudioClip rocketSound;
    public AudioClip mineDropSound;
    public AudioClip ramSound;
    public AudioClip shieldActivateSound;
    public AudioClip shieldHitSound;
    public AudioClip nitrousSound;
    public AudioClip vehicleDamageSound;
    public AudioClip vehicleExplosionSound;
    public AudioClip vehicleEnterSound;
    public AudioClip vehicleExitSound;
    
    [Header("Enemy Sounds")]
    public AudioClip enemyAttackSound;
    public AudioClip enemyHitSound;
    public AudioClip enemyDeathSound;
    public AudioClip enemySpawnSound;
    public AudioClip bossRoarSound;
    public AudioClip explosiveBeepSound;
    
    [Header("UI Sounds")]
    public AudioClip buttonHoverSound;
    public AudioClip buttonClickSound;
    public AudioClip pauseSound;
    public AudioClip unpauseSound;
    public AudioClip gameOverSound;
    public AudioClip victorySound;
    public AudioClip waveStartSound;
    public AudioClip waveCompleteSound;
    public AudioClip notificationSound;
    public AudioClip scoreIncreaseSound;
    public AudioClip highScoreSound;
    
    [Header("Ambient Sounds")]
    public AudioClip windSound;
    public AudioClip birdsSound;
    public AudioClip cityAmbientSound;
    public AudioClip forestAmbientSound;
    
    [Header("Audio Mixing")]
    public AnimationCurve distanceCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    public float maxSoundDistance = 30f;
    
    private Dictionary<string, AudioClip> soundLibrary = new Dictionary<string, AudioClip>();
    private bool isMusicFading = false;
    private string currentMusicType = "";
    private float originalMusicVolume;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        InitializeAudioSources();
        LoadSoundLibrary();
        LoadVolumeSettings();
    }
    
    void InitializeAudioSources()
    {
        // Create music source if not assigned
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        // Create SFX source if not assigned
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        
        // Create ambient source if not assigned
        if (ambientSource == null)
        {
            ambientSource = gameObject.AddComponent<AudioSource>();
            ambientSource.loop = true;
            ambientSource.playOnAwake = false;
        }
        
        // Create voice source if not assigned
        if (voiceSource == null)
        {
            voiceSource = gameObject.AddComponent<AudioSource>();
            voiceSource.loop = false;
            voiceSource.playOnAwake = false;
        }
    }
    
    void LoadSoundLibrary()
    {
        // Player Sounds
        AddSound("Punch", punchSound);
        AddSound("Kick", kickSound);
        AddSound("Jump", jumpSound);
        AddSound("Land", landSound);
        AddSound("Dash", dashSound);
        AddSound("Roll", rollSound);
        AddSound("Damage", damageSound);
        AddSound("Death", deathSound);
        AddSound("Heal", healSound);
        
        // Special Moves
        AddSound("PowerPunch", powerPunchSound);
        AddSound("RoundhouseKick", roundhouseKickSound);
        AddSound("GroundSlam", groundSlamSound);
        AddSound("Ultimate", ultimateSound);
        AddSound("ComboHit", comboHitSound);
        AddSound("ComboBreak", comboBreakSound);
        
        // Vehicle Sounds
        AddSound("EngineStart", engineStartSound);
        AddSound("EngineIdle", engineIdleSound);
        AddSound("EngineRev", engineRevSound);
        AddSound("MachineGun", machineGunSound);
        AddSound("Rocket", rocketSound);
        AddSound("MineDrop", mineDropSound);
        AddSound("Ram", ramSound);
        AddSound("ShieldActivate", shieldActivateSound);
        AddSound("ShieldHit", shieldHitSound);
        AddSound("Nitrous", nitrousSound);
        AddSound("VehicleDamage", vehicleDamageSound);
        AddSound("VehicleExplosion", vehicleExplosionSound);
        AddSound("VehicleEnter", vehicleEnterSound);
        AddSound("VehicleExit", vehicleExitSound);
        
        // Enemy Sounds
        AddSound("EnemyAttack", enemyAttackSound);
        AddSound("EnemyHit", enemyHitSound);
        AddSound("EnemyDeath", enemyDeathSound);
        AddSound("EnemySpawn", enemySpawnSound);
        AddSound("BossRoar", bossRoarSound);
        AddSound("ExplosiveBeep", explosiveBeepSound);
        
        // UI Sounds
        AddSound("ButtonHover", buttonHoverSound);
        AddSound("ButtonClick", buttonClickSound);
        AddSound("Pause", pauseSound);
        AddSound("Unpause", unpauseSound);
        AddSound("GameOver", gameOverSound);
        AddSound("Victory", victorySound);
        AddSound("WaveStart", waveStartSound);
        AddSound("WaveComplete", waveCompleteSound);
        AddSound("Notification", notificationSound);
        AddSound("ScoreIncrease", scoreIncreaseSound);
        AddSound("HighScore", highScoreSound);
        
        // Ambient Sounds
        AddSound("Wind", windSound);
        AddSound("Birds", birdsSound);
        AddSound("CityAmbient", cityAmbientSound);
        AddSound("ForestAmbient", forestAmbientSound);
    }
    
    void AddSound(string name, AudioClip clip)
    {
        if (clip != null && !soundLibrary.ContainsKey(name))
        {
            soundLibrary.Add(name, clip);
        }
    }
    
    void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ambientVolume = PlayerPrefs.GetFloat("AmbientVolume", 1f);
        voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 1f);
        
        ApplyVolumeSettings();
    }
    
    void ApplyVolumeSettings()
    {
        if (musicSource) musicSource.volume = masterVolume * musicVolume;
        if (sfxSource) sfxSource.volume = masterVolume * sfxVolume;
        if (ambientSource) ambientSource.volume = masterVolume * ambientVolume;
        if (voiceSource) voiceSource.volume = masterVolume * voiceVolume;
    }
    
    #region Music Methods
    
    public void PlayMusic(string musicType)
    {
        AudioClip clip = null;
        
        switch (musicType)
        {
            case "Menu":
                clip = menuMusic;
                break;
            case "Gameplay":
                clip = gameplayMusic;
                break;
            case "Combat":
                clip = combatMusic;
                break;
            case "Boss":
                clip = bossMusic;
                break;
            case "Vehicle":
                clip = vehicleMusic;
                break;
            case "Victory":
                clip = victoryMusic;
                break;
            case "GameOver":
                clip = gameOverMusic;
                break;
        }
        
        if (clip != null && musicSource.clip != clip)
        {
            StartCoroutine(FadeMusic(clip));
            currentMusicType = musicType;
        }
    }
    
    IEnumerator FadeMusic(AudioClip newClip)
    {
        isMusicFading = true;
        originalMusicVolume = musicSource.volume;
        
        // Fade out
        float fadeTime = 1f;
        float elapsed = 0f;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(originalMusicVolume, 0, elapsed / fadeTime);
            yield return null;
        }
        
        // Switch clip
        musicSource.clip = newClip;
        musicSource.Play();
        
        // Fade in
        elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, originalMusicVolume, elapsed / fadeTime);
            yield return null;
        }
        
        isMusicFading = false;
    }
    
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            StartCoroutine(FadeOutMusic());
        }
    }
    
    IEnumerator FadeOutMusic()
    {
        float fadeTime = 1f;
        float elapsed = 0f;
        float startVolume = musicSource.volume;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, elapsed / fadeTime);
            yield return null;
        }
        
        musicSource.Stop();
        musicSource.volume = startVolume;
    }
    
    #endregion
    
    #region SFX Methods
    
    public void PlaySFX(string soundName)
    {
        if (soundLibrary.ContainsKey(soundName))
        {
            sfxSource.PlayOneShot(soundLibrary[soundName], sfxVolume * masterVolume);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found in sound library!");
        }
    }
    
    public void PlaySFX(string soundName, float volumeMultiplier)
    {
        if (soundLibrary.ContainsKey(soundName))
        {
            sfxSource.PlayOneShot(soundLibrary[soundName], sfxVolume * masterVolume * volumeMultiplier);
        }
    }
    
    public void PlaySFXAtPosition(string soundName, Vector3 position, float volumeMultiplier = 1f)
    {
        if (soundLibrary.ContainsKey(soundName))
        {
            AudioSource.PlayClipAtPoint(soundLibrary[soundName], position, sfxVolume * masterVolume * volumeMultiplier);
        }
    }
    
    public void PlaySFXWithPitch(string soundName, float pitch)
    {
        if (soundLibrary.ContainsKey(soundName))
        {
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(soundLibrary[soundName], sfxVolume * masterVolume);
            sfxSource.pitch = 1f;
        }
    }
    
    public void PlayRandomizedSFX(string[] soundNames)
    {
        if (soundNames.Length > 0)
        {
            string randomSound = soundNames[Random.Range(0, soundNames.Length)];
            PlaySFX(randomSound);
        }
    }
    
    #endregion
    
    #region Ambient Methods
    
    public void PlayAmbient(string ambientType)
    {
        AudioClip clip = null;
        
        switch (ambientType)
        {
            case "Wind":
                clip = windSound;
                break;
            case "Birds":
                clip = birdsSound;
                break;
            case "City":
                clip = cityAmbientSound;
                break;
            case "Forest":
                clip = forestAmbientSound;
                break;
        }
        
        if (clip != null)
        {
            ambientSource.clip = clip;
            ambientSource.Play();
        }
    }
    
    public void StopAmbient()
    {
        ambientSource.Stop();
    }
    
    #endregion
    
    #region Voice Methods
    
    public void PlayVoice(string soundName)
    {
        if (soundLibrary.ContainsKey(soundName))
        {
            voiceSource.PlayOneShot(soundLibrary[soundName], voiceVolume * masterVolume);
        }
    }
    
    #endregion
    
    #region Volume Control
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        ApplyVolumeSettings();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        ApplyVolumeSettings();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        ApplyVolumeSettings();
    }
    
    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("AmbientVolume", ambientVolume);
        ApplyVolumeSettings();
    }
    
    public void SetVoiceVolume(float volume)
    {
        voiceVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("VoiceVolume", voiceVolume);
        ApplyVolumeSettings();
    }
    
    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
    public float GetAmbientVolume() => ambientVolume;
    public float GetVoiceVolume() => voiceVolume;
    
    #endregion
}