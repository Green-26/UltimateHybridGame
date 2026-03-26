using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("PowerUp Type")]
    public PowerUpType powerUpType = PowerUpType.Health;
    
    public enum PowerUpType
    {
        Health,     // Restore health
        Ammo,       // Add ammo for all weapons
        Speed,      // Temporary speed boost
        Shield,     // Temporary shield
        Score,      // Bonus points
        Invincible  // Temporary invincibility
    }
    
    [Header("Settings")]
    public float amount = 25f;           // Amount of health/score/ammo
    public float duration = 5f;          // Duration for temporary effects
    public float rotationSpeed = 100f;   // How fast it spins
    public float bobSpeed = 2f;          // How fast it bobs up and down
    public float bobHeight = 0.3f;        // How high it bobs
    
    [Header("Visual Effects")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;
    public Color powerUpColor = Color.yellow;
    
    private Vector3 startPosition;
    private float bobTimer = 0f;
    
    void Start()
    {
        startPosition = transform.position;
        
        // Change color based on power-up type
        Renderer renderer = GetComponent<Renderer>();
        if (renderer)
        {
            switch (powerUpType)
            {
                case PowerUpType.Health:
                    powerUpColor = Color.red;
                    break;
                case PowerUpType.Ammo:
                    powerUpColor = Color.blue;
                    break;
                case PowerUpType.Speed:
                    powerUpColor = Color.cyan;
                    break;
                case PowerUpType.Shield:
                    powerUpColor = Color.magenta;
                    break;
                case PowerUpType.Score:
                    powerUpColor = Color.yellow;
                    break;
                case PowerUpType.Invincible:
                    powerUpColor = Color.white;
                    break;
            }
            renderer.material.color = powerUpColor;
        }
    }
    
    void Update()
    {
        // Rotate the power-up
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Bob up and down
        bobTimer += Time.deltaTime * bobSpeed;
        float newY = startPosition.y + Mathf.Sin(bobTimer) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player)
            {
                ApplyPowerUp(player);
            }
            
            // Play pickup sound
            if (AudioManager.Instance && pickupSound)
            {
                AudioManager.Instance.PlaySFXAtPosition("PowerUp", transform.position);
            }
            else if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySFX("Notification", 0.8f);
            }
            
            // Spawn pickup effect
            if (EffectManager.Instance && pickupEffect)
            {
                EffectManager.Instance.SpawnEffect("Heal", transform.position, 0.5f);
            }
            
            // Show notification
            if (UIManager.Instance)
            {
                string message = GetPowerUpMessage();
                UIManager.Instance.ShowNotification(message);
            }
            
            Destroy(gameObject);
        }
    }
    
    void ApplyPowerUp(PlayerController player)
    {
        switch (powerUpType)
        {
            case PowerUpType.Health:
                player.Heal(amount);
                Debug.Log($"PowerUp: Healed {amount} health!");
                break;
                
            case PowerUpType.Ammo:
                // Find vehicle or player weapons
                VehicleController vehicle = FindObjectOfType<VehicleController>();
                if (vehicle)
                {
                    vehicle.AddAmmo("machinegun", 50);
                    vehicle.AddAmmo("rocket", 5);
                    vehicle.AddAmmo("mine", 3);
                }
                Debug.Log($"PowerUp: Added ammo!");
                break;
                
            case PowerUpType.Speed:
                player.StartCoroutine(player.TemporarySpeedBoost(duration));
                Debug.Log($"PowerUp: Speed boost for {duration} seconds!");
                break;
                
            case PowerUpType.Shield:
                // Find vehicle or add shield to player
                VehicleController vehicleShield = FindObjectOfType<VehicleController>();
                if (vehicleShield && vehicleShield.IsPlayerInside())
                {
                    // Add shield to vehicle
                    vehicleShield.ActivateShield();
                }
                else
                {
                    // Player shield - could be added later
                    Debug.Log("Player shield - would activate here");
                }
                break;
                
            case PowerUpType.Score:
                if (GameManager.Instance)
                {
                    GameManager.Instance.AddScore((int)amount);
                }
                Debug.Log($"PowerUp: Added {amount} score!");
                break;
                
            case PowerUpType.Invincible:
                // Start invincibility effect
                player.StartCoroutine(TemporaryInvincibility(player, duration));
                Debug.Log($"PowerUp: Invincible for {duration} seconds!");
                break;
        }
    }
    
    IEnumerator TemporaryInvincibility(PlayerController player, float duration)
    {
        // Store original invincibility settings
        bool originalInvincible = player.isInvincible;
        
        // Make player invincible
        player.isInvincible = true;
        
        // Visual feedback - flash effect
        float elapsed = 0f;
        Renderer renderer = player.GetComponent<Renderer>();
        Color originalColor = renderer ? renderer.material.color : Color.white;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            // Flash effect
            if (renderer)
            {
                float flash = Mathf.PingPong(elapsed * 10f, 1f);
                renderer.material.color = Color.Lerp(originalColor, Color.white, flash);
            }
            
            yield return null;
        }
        
        // Reset invincibility
        if (!originalInvincible)
        {
            player.isInvincible = false;
        }
        
        // Reset color
        if (renderer)
        {
            renderer.material.color = originalColor;
        }
        
        Debug.Log("Invincibility ended!");
    }
    
    string GetPowerUpMessage()
    {
        switch (powerUpType)
        {
            case PowerUpType.Health:
                return $"+{amount} HEALTH!";
            case PowerUpType.Ammo:
                return "+AMMO! (Machine Gun, Rockets, Mines)";
            case PowerUpType.Speed:
                return $"SPEED BOOST! ({duration}s)";
            case PowerUpType.Shield:
                return "SHIELD ACTIVATED!";
            case PowerUpType.Score:
                return $"+{amount} SCORE!";
            case PowerUpType.Invincible:
                return $"INVINCIBLE! ({duration}s)";
            default:
                return "POWER UP!";
        }
    }
    
    // For debugging - show power-up range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}