using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        Health,
        Ammo,
        Speed,
        Shield,
        Score
    }
    
    public PowerUpType powerUpType;
    public float amount = 20f;
    public float duration = 5f;
    public GameObject pickupEffect;
    public AudioClip pickupSound;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player)
            {
                ApplyPowerUp(player);
            }
            
            // Play sound
            if (AudioManager.Instance && pickupSound)
            {
                AudioManager.Instance.PlaySFXAtPosition("PowerUp", transform.position);
            }
            
            // Spawn effect
            if (EffectManager.Instance && pickupEffect)
            {
                EffectManager.Instance.SpawnEffect("Heal", transform.position);
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
                if (UIManager.Instance)
                    UIManager.Instance.ShowNotification("+HEALTH!");
                break;
                
            case PowerUpType.Ammo:
                VehicleController vehicle = FindObjectOfType<VehicleController>();
                if (vehicle)
                {
                    vehicle.AddAmmo("machinegun", 50);
                    vehicle.AddAmmo("rocket", 5);
                }
                if (UIManager.Instance)
                    UIManager.Instance.ShowNotification("+AMMO!");
                break;
                
            case PowerUpType.Speed:
                player.StartCoroutine(player.TemporarySpeedBoost(duration));
                if (UIManager.Instance)
                    UIManager.Instance.ShowNotification("SPEED BOOST!");
                break;
                
            case PowerUpType.Score:
                GameManager.Instance.AddScore((int)amount);
                if (UIManager.Instance)
                    UIManager.Instance.ShowNotification($"+{amount} SCORE!");
                break;
        }
    }
}