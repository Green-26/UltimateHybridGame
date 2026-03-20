using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 50f;
    public float damage = 10f;
    public float attackRange = 1.5f;
    public float speed = 3f;
    
    private Transform player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Chase player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        
        // Face player
        transform.LookAt(player);
        
        // Attack if in range
        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            Debug.Log("ENEMY ATTACKS!");
        }
    }
    
    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"Enemy took {amount} damage! Health: {health}");
        
        if (health <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Enemy defeated!");
        }
    }
}