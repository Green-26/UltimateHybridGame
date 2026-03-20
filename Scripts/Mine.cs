using UnityEngine;

public class Mine : MonoBehaviour
{
    public float damage = 50f;
    public float explosionRadius = 3f;
    public float activationDelay = 1f;
    public GameObject explosionEffect;
    
    private bool isActive = false;
    private GameObject owner;
    
    void Start()
    {
        // Activate after delay
        Invoke("Activate", activationDelay);
        
        // Auto destroy after time
        Destroy(gameObject, 30f);
    }
    
    public void Initialize(float damageAmount, GameObject source)
    {
        damage = damageAmount;
        owner = source;
    }
    
    void Activate()
    {
        isActive = true;
        
        // Visual indicator
        GetComponent<Renderer>().material.color = Color.red;
        Debug.Log("Mine activated!");
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (other.gameObject == owner) return;
        
        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            Explode();
        }
    }
    
    void Explode()
    {
        // Damage all in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.TakeDamage(damage);
                }
            }
            else if (hit.CompareTag("Player") && hit.gameObject != owner)
            {
                PlayerController player = hit.GetComponent<PlayerController>();
                if (player)
                {
                    player.TakeDamage(damage);
                }
            }
        }
        
        // Explosion effect
        if (explosionEffect)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}