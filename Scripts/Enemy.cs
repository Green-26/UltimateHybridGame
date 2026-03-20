using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float health = 50f;
    public float damage = 10f;
    public float attackRange = 1.5f;
    public float speed = 3f;
    public float attackCooldown = 1f;
    
    private Transform player;
    private float lastAttackTime = 0f;
    
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
            Attack();
        }
    }
    
    void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        
        lastAttackTime = Time.time;
        Debug.Log("ENEMY ATTACKS!");
        
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController)
        {
            playerController.TakeDamage(damage);
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