using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    public float damage = 80f;
    public float explosionRadius = 5f;
    public float speed = 30f;
    public GameObject explosionEffect;
    public GameObject trailEffect;
    
    private GameObject owner;
    
    void Start()
    {
        // Add trail effect
        if (trailEffect)
        {
            Instantiate(trailEffect, transform);
        }
        
        // Destroy after time
        Destroy(gameObject, 5f);
    }
    
    void Update()
    {
        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    
    public void Initialize(float damageAmount, float radius, GameObject source)
    {
        damage = damageAmount;
        explosionRadius = radius;
        owner = source;
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Don't hit owner
        if (other.gameObject == owner) return;
        
        // Explode
        Explode();
    }
    
    void Explode()
    {
        // Damage enemies in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.TakeDamage(damage);
                    Debug.Log($"Rocket hit enemy for {damage} damage!");
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