using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game Settings")]
    public int score = 0;
    public int highScore = 0;
    public int enemiesDefeated = 0;
    public int totalEnemies = 0;
    
    [Header("Wave System")]
    public int currentWave = 1;
    public int enemiesPerWave = 5;
    public int enemiesRemainingInWave = 0;
    public float timeBetweenWaves = 5f;
    private bool waveInProgress = false;
    private float waveTimer = 0f;
    
    [Header("Game State")]
    public bool isGameActive = true;
    public bool isGamePaused = false;
    
    void Awake()
    {
        // Singleton pattern
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
        
        // Load saved data
        LoadSavedData();
        
        // Initialize wave
        StartNewWave();
    }
    
    void LoadSavedData()
    {
        if (SaveSystem.Instance)
        {
            // Load high score from save system
            highScore = SaveSystem.Instance.GetHighScore();
            
            // Load other stats for display
            int savedEnemies = SaveSystem.Instance.GetTotalEnemiesDefeated();
            int savedWaves = SaveSystem.Instance.GetTotalWavesCompleted();
            
            Debug.Log($"Loaded save data - High Score: {highScore}, Total Enemies: {savedEnemies}, Total Waves: {savedWaves}");
        }
        else
        {
            // Fallback to PlayerPrefs if SaveSystem not available
            highScore = PlayerPrefs.GetInt("HighScore", 0);
        }
        
        // Update UI with loaded high score
        if (UIManager.Instance && highScoreText)
        {
            UpdateHighScoreDisplay();
        }
    }
    
    void Update()
    {
        // Handle wave delay between waves
        if (!waveInProgress && isGameActive)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0f)
            {
                StartNewWave();
            }
        }
    }
    
    void StartNewWave()
    {
        waveInProgress = true;
        enemiesRemainingInWave = enemiesPerWave + (currentWave - 1) * 2;
        totalEnemies = enemiesRemainingInWave;
        
        Debug.Log($"🌊 WAVE {currentWave} STARTING! {enemiesRemainingInWave} enemies incoming! 🌊");
        
        // Update UI
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowWaveStart(currentWave);
            UIManager.Instance.UpdateWave(currentWave, enemiesRemainingInWave, totalEnemies);
            UIManager.Instance.ShowNotification($"WAVE {currentWave} - FIGHT!");
        }
        
        // Spawn enemies through SpawnManager
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager)
        {
            spawnManager.StartWave(currentWave, enemiesRemainingInWave);
        }
    }
    
    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"Score: {score}");
        
        // Update UI
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateScore(score);
        }
        
        // Update save system
        if (SaveSystem.Instance)
        {
            SaveSystem.Instance.AddScore(score);
            highScore = SaveSystem.Instance.GetHighScore();
            UpdateHighScoreDisplay();
        }
        
        // Check high score
        if (score > highScore)
        {
            highScore = score;
            
            if (UIManager.Instance)
            {
                UIManager.Instance.ShowNotification("NEW HIGH SCORE!");
                UpdateHighScoreDisplay();
            }
            
            // Save high score to PlayerPrefs as backup
            PlayerPrefs.SetInt("HighScore", highScore);
            
            Debug.Log($"🎉 NEW HIGH SCORE: {highScore} 🎉");
        }
    }
    
    void UpdateHighScoreDisplay()
    {
        if (UIManager.Instance && highScoreText)
        {
            // This would need a reference to highScoreText in UIManager
            // UIManager.Instance.UpdateHighScore(highScore);
        }
    }
    
    public void EnemyDefeated()
    {
        enemiesDefeated++;
        enemiesRemainingInWave--;
        
        Debug.Log($"Enemy defeated! {enemiesRemainingInWave} enemies remaining in wave {currentWave}");
        
        // Update save system
        if (SaveSystem.Instance)
        {
            SaveSystem.Instance.AddEnemyDefeated();
        }
        
        // Update UI
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateWave(currentWave, enemiesRemainingInWave, totalEnemies);
            
            // Show wave complete notification if wave is finished
            if (enemiesRemainingInWave <= 0)
            {
                CompleteWave();
            }
        }
    }
    
    void CompleteWave()
    {
        waveInProgress = false;
        waveTimer = timeBetweenWaves;
        
        // Bonus points for completing wave
        int waveBonus = 500 * currentWave;
        AddScore(waveBonus);
        
        // Update save system with wave completion
        if (SaveSystem.Instance)
        {
            SaveSystem.Instance.AddWaveCompleted();
        }
        
        Debug.Log($"🎉 WAVE {currentWave} COMPLETE! +{waveBonus} BONUS! 🎉");
        
        // Update UI
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowWaveComplete();
            UIManager.Instance.ShowNotification($"WAVE {currentWave} COMPLETE! +{waveBonus} SCORE!");
        }
        
        // Increase wave number
        currentWave++;
        
        // Check if game is complete (e.g., wave 20)
        if (currentWave > 20)
        {
            GameComplete();
        }
    }
    
    void GameComplete()
    {
        isGameActive = false;
        Debug.Log("🎮 GAME COMPLETE! YOU BEAT ALL WAVES! 🎮");
        
        // Save final progress
        if (SaveSystem.Instance)
        {
            SaveSystem.Instance.SaveGame();
            
            // Unlock victory achievement
            SaveSystem.Instance.UnlockAchievement("GameComplete");
        }
        
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowNotification("VICTORY! YOU BEAT THE GAME!");
            UIManager.Instance.ShowGameOver(score);
        }
    }
    
    public void EnemySpawned()
    {
        // Track total enemies spawned if needed
    }
    
    public void GameOver()
    {
        isGameActive = false;
        Debug.Log($"💀 GAME OVER! Final Score: {score} 💀");
        
        // Save game on game over
        if (SaveSystem.Instance)
        {
            SaveSystem.Instance.SaveGame();
        }
        
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowGameOver(score);
        }
    }
    
    public void RestartGame()
    {
        // Reset game state
        score = 0;
        enemiesDefeated = 0;
        currentWave = 1;
        enemiesRemainingInWave = 0;
        isGameActive = true;
        
        // Reset UI
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateScore(score);
            UIManager.Instance.UpdateWave(currentWave, 0, 0);
        }
        
        // Reload scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        
        // Resume time
        Time.timeScale = 1f;
    }
    
    public void PauseGame()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0f : 1f;
        
        if (UIManager.Instance)
        {
            if (isGamePaused)
                UIManager.Instance.TogglePause();
        }
    }
    
    public int GetScore()
    {
        return score;
    }
    
    public int GetHighScore()
    {
        return highScore;
    }
    
    public int GetCurrentWave()
    {
        return currentWave;
    }
    
    public int GetEnemiesRemaining()
    {
        return enemiesRemainingInWave;
    }
    
    void OnApplicationQuit()
    {
        // Save game when quitting
        if (SaveSystem.Instance)
        {
            SaveSystem.Instance.SaveGame();
            Debug.Log("Game saved on quit!");
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        // Save game when app is paused (mobile)
        if (pauseStatus && SaveSystem.Instance)
        {
            SaveSystem.Instance.SaveGame();
            Debug.Log("Game saved on pause!");
        }
    }
}