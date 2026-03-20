using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Type")]
    public EnemyType enemyType = EnemyType.Normal;
    
    public enum EnemyType
    {
        Normal,     // Balanced - medium health, medium speed, medium damage
        Fast,       // Fast but weak - low health, high speed, low damage
        Heavy,      // Slow but tough - high health, low speed, high damage
        Ranged,     // Shoots projectiles - medium health, stays at distance
        Explosive,  // Explodes on death - medium health, explodes when killed
        Boss        // Boss enemy - very high health, high damage, special attacks
    }
    
    [Header("Stats")]
    public float health = 50f;
    public float maxHealth = 50f;
    public float damage = 10f;
    public float attackRange = 1.5f;
    public float speed = 3f;
    public float attackCooldown = 1f;
    
    [Header("Ranged Attack (for Ranged type)")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;
    public float shootRange = 10f;
    public float shootCooldown = 1.5f;
    private float lastShootTime = 0f;
    
    [Header("Explosive Enemy (for Explosive type)")]
    public float explosionRadius = 4f;
    public float explosionDamage = 30f;
    public GameObject explosionEffect;
    
    [Header("Boss Special Abilities")]
    public bool canHeal = false;
    public float healAmount = 20f;
    public float healCooldown = 8f;
    private float lastHealTime = 0f;
    
    public bool canSummonMinions = false;
    public GameObject minionPrefab;
    public int minionCount = 3;
    public float summonCooldown = 15f;
    private float lastSummonTime = 0f;
    
    public bool hasAreaAttack = false;
    public float areaAttackDamage = 25f;
    public float areaAttackRadius = 5f;
    public float areaAttackCooldown = 6f;
    private float lastAreaAttackTime = 0f;
    
    [Header("Visual Effects")]
    public GameObject deathEffect;
    public GameObject hitEffect;
    
    private Transform player;
    private float lastAttackTime = 0f;
    private Animator animator;
    private Renderer enemyRenderer;
    private Material originalMaterial;
    private bool isDead = false;
    private Rigidbody rb;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        enemyRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        
        if (enemyRenderer)
        {
            originalMaterial = enemyRenderer.material;
        }
        
        // Set stats based on enemy type
        SetStatsByType();
        
        Debug.Log($"{enemyType} Enemy spawned! Health: {health}, Speed: {speed}, Damage: {damage}");
    }
    
    void SetStatsByType()
    {
        switch (enemyType)
        {
            case EnemyType.Normal:
                health = 50f;
                maxHealth = 50f;
                damage = 12f;
                speed = 3f;
                attackRange = 1.8f;
                attackCooldown = 1.2f;
                transform.localScale = new Vector3(1f, 1f, 1f);
                break;
                
            case EnemyType.Fast:
                health = 30f;
                maxHealth = 30f;
                damage = 8f;
                speed = 6f;
                attackRange = 1.5f;
                attackCooldown = 0.8f;
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                break;
                
            case EnemyType.Heavy:
                health = 150f;
                maxHealth = 150f;
                damage = 25f;
                speed = 1.5f;
                attackRange = 2.2f;
                attackCooldown = 1.8f;
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                break;
                
            case EnemyType.Ranged:
                health = 40f;
                maxHealth = 40f;
                damage = 10f;
                speed = 2.5f;
                attackRange = 1.5f;
                attackCooldown = 1f;
                shootRange = 12f;
                shootCooldown = 1.2f;
                transform.localScale = new Vector3(1f, 1f, 1f);
                break;
                
            case EnemyType.Explosive:
                health = 35f;
                maxHealth = 35f;
                damage = 15f;
                speed = 3.5f;
                attackRange = 2f;
                attackCooldown = 1f;
                explosionRadius = 4f;
                explosionDamage = 35f;
                transform.localScale = new Vector3(1f, 1f, 1f);
                break;
                
            case EnemyType.Boss:
                health = 500f;
                maxHealth = 500f;
                damage = 35f;
                speed = 2.2f;
                attackRange = 2.5f;
                attackCooldown = 1.2f;
                transform.localScale = new Vector3(2f, 2f, 2f);
                
                // Boss special abilities
                canHeal = true;
                canSummonMinions = true;
                hasAreaAttack = true;
                break;
        }
    }
    
    void Update()
    {
        if (player == null || isDead) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Handle different enemy behaviors
        switch (enemyType)
        {
            case EnemyType.Ranged:
                HandleRangedBehavior(distanceToPlayer);
                break;
                
            case EnemyType.Boss:
                HandleBossBehavior(distanceToPlayer);
                break;
                
            default:
                HandleMeleeBehavior(distanceToPlayer);
                break;
        }
        
        // Visual feedback based on health
        UpdateVisualFeedback();
    }
    
    void HandleMeleeBehavior(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
        else if (distanceToPlayer <= 15f)
        {
            Chase();
        }
    }
    
    void HandleRangedBehavior(float distanceToPlayer)
    {
        if (distanceToPlayer <= shootRange && distanceToPlayer > attackRange)
        {
            // Face player
            Vector3 lookDirection = player.position - transform.position;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f * Time.deltaTime);
            }
            
            // Shoot projectile
            if (Time.time - lastShootTime >= shootCooldown)
            {
                ShootProjectile();
            }
        }
        else if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
        else if (distanceToPlayer > shootRange)
        {
            Chase();
        }
    }
    
    void HandleBossBehavior(float distanceToPlayer)
    {
        // Chase or attack
        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
        else
        {
            Chase();
        }
        
        // Boss special: Heal
        if (canHeal && health < maxHealth * 0.5f && Time.time - lastHealTime >= healCooldown)
        {
            Heal();
        }
        
        // Boss special: Summon minions
        if (canSummonMinions && Time.time - lastSummonTime >= summonCooldown)
        {
            SummonMinions();
        }
        
        // Boss special: Area attack
        if (hasAreaAttack && distanceToPlayer <= areaAttackRadius && Time.time - lastAreaAttackTime >= areaAttackCooldown)
        {
            AreaAttack();
        }
    }
    
    void Chase()
    {
        // Move toward player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        
        // Face player
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f * Time.deltaTime);
        }
        
        if (animator) animator.SetBool("IsChasing", true);
    }
    
    void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        
        lastAttackTime = Time.time;
        
        if (animator) animator.SetTrigger("Attack");
        
        // Deal damage after delay
        Invoke("DealDamage", 0.3f);
        
        Debug.Log($"{enemyType} enemy attacks!");
    }
    
    void DealDamage()
    {
        if (!player) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange + 0.5f)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController)
            {
                playerController.TakeDamage(damage);
                Debug.Log($"{enemyType} enemy dealt {damage} damage!");
            }
        }
    }
    
    void ShootProjectile()
    {
        if (!projectilePrefab || !player) return;
        
        lastShootTime = Time.time;
        
        Vector3 direction = (player.position - transform.position).normalized;
        
        GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward * 1.5f, Quaternion.LookRotation(direction));
        
        // Set projectile direction
        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript)
        {
            projScript.SetDirection(direction);
            projScript.damage = damage;
        }
        
        Debug.Log($"{enemyType} enemy shoots projectile!");
    }
    
    void Heal()
    {
        lastHealTime = Time.time;
        health += healAmount;
        if (health > maxHealth) health = maxHealth;
        
        Debug.Log($"BOSS HEALS! Health: {health}/{maxHealth}");
        
        // Visual effect for healing
        if (enemyRenderer)
        {
            StartCoroutine(FlashColor(Color.green, 0.3f));
        }
    }
    
    void SummonMinions()
    {
        lastSummonTime = Time.time;
        
        for (int i = 0; i < minionCount; i++)
        {
            if (minionPrefab)
            {
                Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
                GameObject minion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
                
                // Set minion as normal type
                Enemy minionEnemy = minion.GetComponent<Enemy>();
                if (minionEnemy)
                {
                    minionEnemy.enemyType = EnemyType.Normal;
                    minionEnemy.SetStatsByType();
                }
            }
        }
        
        Debug.Log($"BOSS SUMMONS {minionCount} minions!");
    }
    
    void AreaAttack()
    {
        lastAreaAttackTime = Time.time;
        
        // Create visual effect
        StartCoroutine(FlashColor(Color.red, 0.2f));
        
        // Damage all players in radius
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, areaAttackRadius);
        foreach (Collider hit in hitPlayers)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerController playerController = hit.GetComponent<PlayerController>();
                if (playerController)
                {
                    playerController.TakeDamage(areaAttackDamage);
                }
            }
        }
        
        // Knockback
        if (player)
        {
            Vector3 knockbackDirection = (player.position - transform.position).normalized;
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb)
            {
                playerRb.AddForce(knockbackDirection * 800f, ForceMode.Impulse);
            }
        }
        
        Debug.Log($"BOSS AREA ATTACK! {areaAttackDamage} damage in radius!");
    }
    
    IEnumerator FlashColor(Color flashColor, float duration)
    {
        if (enemyRenderer)
        {
            enemyRenderer.material.color = flashColor;
            yield return new WaitForSeconds(duration);
            enemyRenderer.material.color = originalMaterial ? originalMaterial.color : Color.white;
        }
    }
    
    void UpdateVisualFeedback()
    {
        // Scale based on health (for visual feedback)
        float healthPercent = health / maxHealth;
        float scale = 0.7f + (healthPercent * 0.6f);
        transform.localScale = Vector3.Lerp(transform.localScale, 
            new Vector3(scale, scale, scale), 
            Time.deltaTime * 5f);
    }
    
    public void TakeDamage(float amount)
    {
        if (isDead) return;
        
        health -= amount;
        Debug.Log($"{enemyType} enemy took {amount} damage! Health: {health}/{maxHealth}");
        
        // Hit effect
        if (hitEffect)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        
        // Flash red on hit
        StartCoroutine(FlashColor(Color.red, 0.1f));
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        isDead = true;
        Debug.Log($"{enemyType} enemy defeated!");
        
        // Explosive enemy explodes on death
        if (enemyType == EnemyType.Explosive)
        {
            Explode();
        }
        
        // Death effect
        if (deathEffect)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        // Add score
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager)
        {
            int scoreValue = GetScoreValue();
            gameManager.AddScore(scoreValue);
            gameManager.EnemyDefeated();
        }
        
        // Notify spawn manager
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager)
        {
            spawnManager.EnemyDied(gameObject);
        }
        
        Destroy(gameObject, 0.5f);
    }
    
    void Explode()
    {
        // Damage all enemies and player in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy") && hit.gameObject != gameObject)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.TakeDamage(explosionDamage);
                }
            }
            else if (hit.CompareTag("Player"))
            {
                PlayerController playerController = hit.GetComponent<PlayerController>();
                if (playerController)
                {
                    playerController.TakeDamage(explosionDamage);
                }
            }
        }
        
        // Explosion effect
        if (explosionEffect)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        
        Debug.Log($"💥 EXPLOSIVE ENEMY EXPLODES! {explosionDamage} damage in radius! 💥");
    }
    
    int GetScoreValue()
    {
        switch (enemyType)
        {
            case EnemyType.Fast: return 50;
            case EnemyType.Normal: return 100;
            case EnemyType.Ranged: return 120;
            case EnemyType.Explosive: return 150;
            case EnemyType.Heavy: return 200;
            case EnemyType.Boss: return 1000;
            default: return 100;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw shoot range for ranged enemies
        if (enemyType == EnemyType.Ranged)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, shootRange);
        }
        
        // Draw explosion radius for explosive enemies
        if (enemyType == EnemyType.Explosive)
        {
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
        
        // Draw area attack radius for boss
        if (enemyType == EnemyType.Boss && hasAreaAttack)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, areaAttackRadius);
        }
    }
}