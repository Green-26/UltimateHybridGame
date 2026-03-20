using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("Panels")]
    public GameObject hudPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject weaponPanel;
    
    [Header("Health UI")]
    public Slider healthSlider;
    public Text healthText;
    public Image healthFillImage;
    public Gradient healthGradient;
    
    [Header("Shield UI")]
    public Slider shieldSlider;
    public Text shieldText;
    public GameObject shieldPanel;
    
    [Header("Combo UI")]
    public Text comboText;
    public Animator comboAnimator;
    public GameObject comboPanel;
    
    [Header("Score UI")]
    public Text scoreText;
    public Text highScoreText;
    
    [Header("Ammo UI")]
    public Text machineGunAmmoText;
    public Text rocketAmmoText;
    public Text mineAmmoText;
    public GameObject machineGunIcon;
    public GameObject rocketIcon;
    public GameObject mineIcon;
    
    [Header("Special Moves UI")]
    public Image powerPunchIcon;
    public Image roundhouseKickIcon;
    public Image groundSlamIcon;
    public Image ultimateIcon;
    public Text powerPunchCooldownText;
    public Text roundhouseKickCooldownText;
    public Text groundSlamCooldownText;
    public Text ultimateCooldownText;
    
    [Header("Vehicle UI")]
    public Slider vehicleHealthSlider;
    public Text vehicleHealthText;
    public Text speedText;
    public Text currentWeaponText;
    public GameObject vehiclePanel;
    
    [Header("Crosshair")]
    public Image crosshair;
    public Sprite defaultCrosshair;
    public Sprite vehicleCrosshair;
    
    [Header("Damage Effect")]
    public Image damageOverlay;
    public float damageFlashDuration = 0.3f;
    
    [Header("Notifications")]
    public Text notificationText;
    public float notificationDuration = 2f;
    private Coroutine notificationCoroutine;
    
    [Header("Wave Info")]
    public Text waveText;
    public Text enemiesRemainingText;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        // Hide all panels initially
        if (pausePanel) pausePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        
        // Initialize UI
        UpdateHealth(100, 100);
        UpdateScore(0);
        
        if (damageOverlay)
        {
            damageOverlay.color = new Color(1, 0, 0, 0);
        }
        
        // Load high score
        if (highScoreText)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = $"HIGH SCORE: {highScore}";
        }
    }
    
    void Update()
    {
        // Pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    #region Health UI
    
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthSlider)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        
        if (healthText)
        {
            healthText.text = $"{Mathf.Ceil(currentHealth)}/{maxHealth}";
        }
        
        if (healthFillImage && healthGradient)
        {
            healthFillImage.color = healthGradient.Evaluate(currentHealth / maxHealth);
        }
    }
    
    public void ShowDamageFlash()
    {
        if (damageOverlay)
        {
            StartCoroutine(DamageFlashCoroutine());
        }
    }
    
    IEnumerator DamageFlashCoroutine()
    {
        damageOverlay.color = new Color(1, 0, 0, 0.5f);
        
        // Camera shake on damage
        if (CameraController.Instance)
        {
            CameraController.Instance.ShakeCamera(0.2f, 0.15f);
        }
        
        yield return new WaitForSeconds(damageFlashDuration);
        damageOverlay.color = new Color(1, 0, 0, 0);
    }
    
    #endregion
    
    #region Shield UI
    
    public void UpdateShield(float currentShield, float maxShield)
    {
        if (shieldSlider)
        {
            shieldSlider.maxValue = maxShield;
            shieldSlider.value = currentShield;
            shieldPanel.SetActive(currentShield > 0);
        }
        
        if (shieldText)
        {
            shieldText.text = $"{Mathf.Ceil(currentShield)}";
        }
    }
    
    #endregion
    
    #region Combo UI
    
    public void UpdateCombo(int combo)
    {
        if (comboText)
        {
            comboText.text = $"x{combo} COMBO!";
            comboPanel.SetActive(combo > 0);
            
            if (comboAnimator)
            {
                comboAnimator.SetTrigger("Pulse");
            }
            
            // Scale text based on combo
            float scale = 1f + (combo / 10f);
            comboText.transform.localScale = Vector3.one * Mathf.Min(scale, 2f);
        }
    }
    
    #endregion
    
    #region Score UI
    
    public void UpdateScore(int score)
    {
        if (scoreText)
        {
            scoreText.text = $"SCORE: {score}";
        }
        
        // Update high score
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            if (highScoreText)
            {
                highScoreText.text = $"HIGH SCORE: {score}";
            }
            
            // Play high score sound
            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySFX("HighScore");
            }
        }
        
        // Play score increase sound on significant gains
        if (score > 0 && score % 100 == 0 && AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("ScoreIncrease");
        }
    }
    
    #endregion
    
    #region Ammo UI
    
    public void UpdateMachineGunAmmo(int current, int max)
    {
        if (machineGunAmmoText)
        {
            machineGunAmmoText.text = $"{current}/{max}";
            
            // Change color when low on ammo
            if (current < max * 0.2f)
                machineGunAmmoText.color = Color.red;
            else
                machineGunAmmoText.color = Color.white;
        }
    }
    
    public void UpdateRocketAmmo(int current, int max)
    {
        if (rocketAmmoText)
        {
            rocketAmmoText.text = $"{current}/{max}";
            
            if (current == 0)
                rocketAmmoText.color = Color.red;
            else
                rocketAmmoText.color = Color.white;
        }
    }
    
    public void UpdateMineAmmo(int current, int max)
    {
        if (mineAmmoText)
        {
            mineAmmoText.text = $"{current}/{max}";
            
            if (current == 0)
                mineAmmoText.color = Color.red;
            else
                mineAmmoText.color = Color.white;
        }
    }
    
    public void UpdateCurrentWeapon(string weaponName)
    {
        if (currentWeaponText)
        {
            currentWeaponText.text = weaponName;
        }
        
        // Highlight active weapon icon
        if (machineGunIcon) machineGunIcon.GetComponent<Image>().color = weaponName == "Machine Gun" ? Color.yellow : Color.white;
        if (rocketIcon) rocketIcon.GetComponent<Image>().color = weaponName == "Rocket" ? Color.yellow : Color.white;
        if (mineIcon) mineIcon.GetComponent<Image>().color = weaponName == "Mine" ? Color.yellow : Color.white;
        
        // Play weapon switch sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("ButtonClick", 0.5f);
        }
    }
    
    #endregion
    
    #region Vehicle UI
    
    public void ShowVehicleUI(bool show)
    {
        if (vehiclePanel)
        {
            vehiclePanel.SetActive(show);
        }
        
        // Change crosshair
        if (crosshair)
        {
            crosshair.sprite = show ? vehicleCrosshair : defaultCrosshair;
        }
    }
    
    public void UpdateVehicleHealth(float currentHealth, float maxHealth)
    {
        if (vehicleHealthSlider)
        {
            vehicleHealthSlider.maxValue = maxHealth;
            vehicleHealthSlider.value = currentHealth;
        }
        
        if (vehicleHealthText)
        {
            vehicleHealthText.text = $"{Mathf.Ceil(currentHealth)}/{maxHealth}";
        }
    }
    
    public void UpdateSpeed(float speed)
    {
        if (speedText)
        {
            int speedKPH = Mathf.RoundToInt(speed * 3.6f);
            speedText.text = $"{speedKPH} km/h";
        }
    }
    
    #endregion
    
    #region Special Moves UI
    
    public void UpdateSpecialMoveCooldown(string moveName, float remainingTime, float maxTime)
    {
        Image targetIcon = null;
        Text targetText = null;
        
        switch (moveName)
        {
            case "PowerPunch":
                targetIcon = powerPunchIcon;
                targetText = powerPunchCooldownText;
                break;
            case "RoundhouseKick":
                targetIcon = roundhouseKickIcon;
                targetText = roundhouseKickCooldownText;
                break;
            case "GroundSlam":
                targetIcon = groundSlamIcon;
                targetText = groundSlamCooldownText;
                break;
            case "Ultimate":
                targetIcon = ultimateIcon;
                targetText = ultimateCooldownText;
                break;
        }
        
        if (targetIcon)
        {
            float fillAmount = remainingTime > 0 ? 1 - (remainingTime / maxTime) : 1f;
            targetIcon.fillAmount = fillAmount;
            
            if (targetText)
            {
                targetText.text = remainingTime > 0 ? Mathf.CeilToInt(remainingTime).ToString() : "";
                targetText.gameObject.SetActive(remainingTime > 0);
            }
            
            // Change color when ready
            targetIcon.color = remainingTime <= 0 ? Color.green : Color.gray;
            
            // Play ready sound when cooldown completes
            if (remainingTime <= 0 && remainingTime + Time.deltaTime > 0 && AudioManager.Instance)
            {
                AudioManager.Instance.PlaySFX("Notification", 0.6f);
            }
        }
    }
    
    #endregion
    
    #region Wave UI
    
    public void UpdateWave(int wave, int enemiesRemaining, int totalEnemies)
    {
        if (waveText)
        {
            waveText.text = $"WAVE {wave}";
        }
        
        if (enemiesRemainingText)
        {
            enemiesRemainingText.text = $"ENEMIES: {enemiesRemaining}/{totalEnemies}";
        }
    }
    
    public void ShowWaveStart(int wave)
    {
        ShowNotification($"WAVE {wave} START!");
        
        // Play wave start sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("WaveStart");
        }
        
        if (waveText)
        {
            waveText.GetComponent<Animator>()?.SetTrigger("Pulse");
        }
    }
    
    public void ShowWaveComplete()
    {
        ShowNotification("WAVE COMPLETE!");
        
        // Play wave complete sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("WaveComplete");
        }
    }
    
    #endregion
    
    #region Notifications
    
    public void ShowNotification(string message)
    {
        if (notificationText)
        {
            if (notificationCoroutine != null)
                StopCoroutine(notificationCoroutine);
            
            notificationCoroutine = StartCoroutine(ShowNotificationCoroutine(message));
        }
        
        // Play notification sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("Notification", 0.7f);
        }
    }
    
    IEnumerator ShowNotificationCoroutine(string message)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(notificationDuration);
        
        notificationText.gameObject.SetActive(false);
    }
    
    #endregion
    
    #region Pause & Game Over
    
    public void TogglePause()
    {
        if (pausePanel)
        {
            bool isPaused = !pausePanel.activeSelf;
            pausePanel.SetActive(isPaused);
            Time.timeScale = isPaused ? 0 : 1;
            
            // Play pause/unpause sound
            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySFX(isPaused ? "Pause" : "Unpause");
            }
        }
    }
    
    public void ShowGameOver(int finalScore)
    {
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
            Text finalScoreText = gameOverPanel.GetComponentInChildren<Text>();
            if (finalScoreText)
            {
                finalScoreText.text = $"FINAL SCORE: {finalScore}";
            }
        }
        Time.timeScale = 0;
        
        // Play game over sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("GameOver");
        }
    }
    
    public void ResumeGame()
    {
        if (pausePanel)
            pausePanel.SetActive(false);
        Time.timeScale = 1;
        
        // Play unpause sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("Unpause");
        }
    }
    
    public void RestartGame()
    {
        // Play button click sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
        }
        
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        // Play button click sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
        }
        
        Application.Quit();
    }
    
    public void OnButtonHover()
    {
        // Play hover sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("ButtonHover", 0.4f);
        }
    }
    
    #endregion
}