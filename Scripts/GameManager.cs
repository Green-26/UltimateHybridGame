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
        
        // Load high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        // Initialize wave
        StartNewWave();
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
        
        // Check high score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            
            if (UIManager.Instance)
            {
                UIManager.Instance.ShowNotification("NEW HIGH SCORE!");
            }
            Debug.Log($"🎉 NEW HIGH SCORE: {highScore} 🎉");
        }
    }
    
    public void EnemyDefeated()
    {
        enemiesDefeated++;
        enemiesRemainingInWave--;
        
        Debug.Log($"Enemy defeated! {enemiesRemainingInWave} enemies remaining in wave {currentWave}");
        
        // Add score based on enemy type (handled in Enemy.cs)
        // AddScore is called from Enemy.cs with specific score values
        
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
}