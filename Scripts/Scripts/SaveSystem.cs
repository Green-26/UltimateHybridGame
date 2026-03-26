// ============================================
// Ultimate Hybrid Game - Player Controller
// Copyright (c) 2026 Bertin ABIJURU
// All Rights Reserved
// ============================================

using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    // Player Stats
    public int totalScore;
    public int highScore;
    public int totalEnemiesDefeated;
    public int totalWavesCompleted;
    public float totalPlayTime;
    
    // Player Progress
    public int unlockedVehicles;
    public int unlockedWeapons;
    public int unlockedSpecialMoves;
    public List<string> unlockedAchievements;
    
    // Player Settings
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public float ambientVolume;
    public float voiceVolume;
    public float mouseSensitivity;
    public bool invertY;
    public int graphicsQuality;
    public bool fullscreen;
    public int resolutionIndex;
    
    // Unlockable Content
    public bool hasUnlockedNitrous;
    public bool hasUnlockedShield;
    public bool hasUnlockedRockets;
    public bool hasUnlockedMines;
    public List<string> unlockedSkins;
    
    // Last Played
    public string lastPlayedDate;
    public int lastWaveReached;
    public int lastScore;
    
    public GameData()
    {
        // Initialize with default values
        totalScore = 0;
        highScore = 0;
        totalEnemiesDefeated = 0;
        totalWavesCompleted = 0;
        totalPlayTime = 0;
        
        unlockedVehicles = 1;
        unlockedWeapons = 1;
        unlockedSpecialMoves = 1;
        unlockedAchievements = new List<string>();
        
        masterVolume = 1f;
        musicVolume = 1f;
        sfxVolume = 1f;
        ambientVolume = 1f;
        voiceVolume = 1f;
        mouseSensitivity = 1f;
        invertY = false;
        graphicsQuality = 2;
        fullscreen = true;
        resolutionIndex = 0;
        
        hasUnlockedNitrous = false;
        hasUnlockedShield = false;
        hasUnlockedRockets = false;
        hasUnlockedMines = false;
        unlockedSkins = new List<string>();
        
        lastPlayedDate = DateTime.Now.ToString("yyyy-MM-dd");
        lastWaveReached = 0;
        lastScore = 0;
    }
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    
    private GameData gameData;
    private string savePath;
    private string saveFileName = "game_save.dat";
    
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
        
        savePath = Application.persistentDataPath + "/" + saveFileName;
        LoadGame();
    }
    
    void Start()
    {
        // Start tracking play time
        StartCoroutine(TrackPlayTime());
    }
    
    System.Collections.IEnumerator TrackPlayTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (GameManager.Instance != null && GameManager.Instance.isGameActive)
            {
                gameData.totalPlayTime += 1f;
            }
        }
    }
    
    #region Save/Load Methods
    
    public void SaveGame()
    {
        try
        {
            // Update high score from GameManager
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.score > gameData.highScore)
                {
                    gameData.highScore = GameManager.Instance.score;
                }
                gameData.totalScore = GameManager.Instance.score;
                gameData.lastWaveReached = GameManager.Instance.currentWave;
                gameData.lastScore = GameManager.Instance.score;
            }
            
            // Update last played date
            gameData.lastPlayedDate = DateTime.Now.ToString("yyyy-MM-dd");
            
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(savePath, FileMode.Create);
            
            formatter.Serialize(stream, gameData);
            stream.Close();
            
            Debug.Log($"Game saved to: {savePath}");
            Debug.Log($"High Score: {gameData.highScore}, Total Score: {gameData.totalScore}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }
    
    public void LoadGame()
    {
        try
        {
            if (File.Exists(savePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(savePath, FileMode.Open);
                
                gameData = formatter.Deserialize(stream) as GameData;
                stream.Close();
                
                Debug.Log($"Game loaded from: {savePath}");
                ApplyLoadedSettings();
            }
            else
            {
                Debug.Log("No save file found. Creating new game data.");
                gameData = new GameData();
                SaveGame();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            gameData = new GameData();
        }
    }
    
    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            gameData = new GameData();
            SaveGame();
            Debug.Log("Save file deleted!");
        }
    }
    
    #endregion
    
    #region Settings Methods
    
    public void ApplyLoadedSettings()
    {
        // Apply volume settings
        if (AudioManager.Instance)
        {
            AudioManager.Instance.SetMasterVolume(gameData.masterVolume);
            AudioManager.Instance.SetMusicVolume(gameData.musicVolume);
            AudioManager.Instance.SetSFXVolume(gameData.sfxVolume);
            AudioManager.Instance.SetAmbientVolume(gameData.ambientVolume);
            AudioManager.Instance.SetVoiceVolume(gameData.voiceVolume);
        }
        
        // Apply graphics settings
        QualitySettings.SetQualityLevel(gameData.graphicsQuality);
        Screen.fullScreen = gameData.fullscreen;
        
        // Apply mouse settings
        if (CameraController.Instance)
        {
            // Mouse sensitivity can be applied in CameraController
        }
        
        Debug.Log("Loaded settings applied!");
    }
    
    public void SaveSettings()
    {
        SaveGame();
    }
    
    #endregion
    
    #region Progress Tracking
    
    public void AddScore(int score)
    {
        gameData.totalScore += score;
        if (gameData.totalScore > gameData.highScore)
        {
            gameData.highScore = gameData.totalScore;
        }
        SaveGame();
    }
    
    public void AddEnemyDefeated()
    {
        gameData.totalEnemiesDefeated++;
        SaveGame();
        
        // Check achievements based on enemy count
        CheckEnemyAchievements();
    }
    
    public void AddWaveCompleted()
    {
        gameData.totalWavesCompleted++;
        SaveGame();
        
        // Check achievements based on waves
        CheckWaveAchievements();
    }
    
    public void UnlockAchievement(string achievementId)
    {
        if (!gameData.unlockedAchievements.Contains(achievementId))
        {
            gameData.unlockedAchievements.Add(achievementId);
            SaveGame();
            
            // Show notification
            if (UIManager.Instance)
            {
                UIManager.Instance.ShowNotification($"ACHIEVEMENT UNLOCKED: {achievementId}!");
            }
            
            Debug.Log($"Achievement unlocked: {achievementId}");
        }
    }
    
    void CheckEnemyAchievements()
    {
        if (gameData.totalEnemiesDefeated >= 10 && !gameData.unlockedAchievements.Contains("FirstBlood"))
            UnlockAchievement("FirstBlood");
        
        if (gameData.totalEnemiesDefeated >= 100 && !gameData.unlockedAchievements.Contains("EnemySlayer"))
            UnlockAchievement("EnemySlayer");
        
        if (gameData.totalEnemiesDefeated >= 500 && !gameData.unlockedAchievements.Contains("LegendaryWarrior"))
            UnlockAchievement("LegendaryWarrior");
    }
    
    void CheckWaveAchievements()
    {
        if (gameData.totalWavesCompleted >= 5 && !gameData.unlockedAchievements.Contains("WaveSurvivor"))
            UnlockAchievement("WaveSurvivor");
        
        if (gameData.totalWavesCompleted >= 10 && !gameData.unlockedAchievements.Contains("WaveMaster"))
            UnlockAchievement("WaveMaster");
        
        if (gameData.totalWavesCompleted >= 20 && !gameData.unlockedAchievements.Contains("WaveLegend"))
            UnlockAchievement("WaveLegend");
    }
    
    #endregion
    
    #region Unlock Methods
    
    public void UnlockVehicle(string vehicleName)
    {
        gameData.unlockedVehicles++;
        SaveGame();
        
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowNotification($"NEW VEHICLE UNLOCKED: {vehicleName}!");
        }
    }
    
    public void UnlockWeapon(string weaponName)
    {
        gameData.unlockedWeapons++;
        
        switch (weaponName)
        {
            case "Nitrous":
                gameData.hasUnlockedNitrous = true;
                break;
            case "Shield":
                gameData.hasUnlockedShield = true;
                break;
            case "Rockets":
                gameData.hasUnlockedRockets = true;
                break;
            case "Mines":
                gameData.hasUnlockedMines = true;
                break;
        }
        
        SaveGame();
        
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowNotification($"NEW WEAPON UNLOCKED: {weaponName}!");
        }
    }
    
    public bool IsWeaponUnlocked(string weaponName)
    {
        switch (weaponName)
        {
            case "Nitrous": return gameData.hasUnlockedNitrous;
            case "Shield": return gameData.hasUnlockedShield;
            case "Rockets": return gameData.hasUnlockedRockets;
            case "Mines": return gameData.hasUnlockedMines;
            default: return true;
        }
    }
    
    #endregion
    
    #region Getter Methods
    
    public int GetHighScore() => gameData.highScore;
    public int GetTotalScore() => gameData.totalScore;
    public int GetTotalEnemiesDefeated() => gameData.totalEnemiesDefeated;
    public int GetTotalWavesCompleted() => gameData.totalWavesCompleted;
    public float GetTotalPlayTime() => gameData.totalPlayTime;
    public string GetFormattedPlayTime()
    {
        int hours = Mathf.FloorToInt(gameData.totalPlayTime / 3600);
        int minutes = Mathf.FloorToInt((gameData.totalPlayTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(gameData.totalPlayTime % 60);
        
        if (hours > 0)
            return $"{hours}h {minutes}m";
        else if (minutes > 0)
            return $"{minutes}m {seconds}s";
        else
            return $"{seconds}s";
    }
    
    public int GetLastWaveReached() => gameData.lastWaveReached;
    public int GetLastScore() => gameData.lastScore;
    public string GetLastPlayedDate() => gameData.lastPlayedDate;
    
    public List<string> GetUnlockedAchievements() => gameData.unlockedAchievements;
    
    #endregion
    
    #region Settings Getters/Setters
    
    public float GetMasterVolume() => gameData.masterVolume;
    public void SetMasterVolume(float volume)
    {
        gameData.masterVolume = volume;
        SaveGame();
    }
    
    public float GetMusicVolume() => gameData.musicVolume;
    public void SetMusicVolume(float volume)
    {
        gameData.musicVolume = volume;
        SaveGame();
    }
    
    public float GetSFXVolume() => gameData.sfxVolume;
    public void SetSFXVolume(float volume)
    {
        gameData.sfxVolume = volume;
        SaveGame();
    }
    
    public float GetAmbientVolume() => gameData.ambientVolume;
    public void SetAmbientVolume(float volume)
    {
        gameData.ambientVolume = volume;
        SaveGame();
    }
    
    public float GetMouseSensitivity() => gameData.mouseSensitivity;
    public void SetMouseSensitivity(float sensitivity)
    {
        gameData.mouseSensitivity = sensitivity;
        SaveGame();
    }
    
    public bool GetInvertY() => gameData.invertY;
    public void SetInvertY(bool invert)
    {
        gameData.invertY = invert;
        SaveGame();
    }
    
    public int GetGraphicsQuality() => gameData.graphicsQuality;
    public void SetGraphicsQuality(int quality)
    {
        gameData.graphicsQuality = quality;
        QualitySettings.SetQualityLevel(quality);
        SaveGame();
    }
    
    public bool GetFullscreen() => gameData.fullscreen;
    public void SetFullscreen(bool fullscreen)
    {
        gameData.fullscreen = fullscreen;
        Screen.fullScreen = fullscreen;
        SaveGame();
    }
    
    #endregion
}