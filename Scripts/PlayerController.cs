using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 8f;
    
    [Header("Combat")]
    public float punchDamage = 15f;
    public float punchRange = 2f;
    public float kickDamage = 25f;
    public float kickRange = 2.5f;
    public float attackCooldown = 0.5f;
    
    [Header("Special Moves")]
    public float powerPunchDamage = 40f;
    public float powerPunchCooldown = 3f;
    private float powerPunchTimer = 0f;
    private bool powerPunchReady = true;
    
    public float roundhouseKickDamage = 50f;
    public float roundhouseKickCooldown = 4f;
    private float roundhouseKickTimer = 0f;
    private bool roundhouseKickReady = true;
    
    public float groundSlamDamage = 60f;
    public float groundSlamRadius = 3f;
    public float groundSlamCooldown = 5f;
    private float groundSlamTimer = 0f;
    private bool groundSlamReady = true;
    
    [Header("Ultimate Move")]
    public float ultimateDamage = 100f;
    public float ultimateCooldown = 10f;
    public float ultimateRange = 5f;
    private float ultimateTimer = 0f;
    private bool ultimateReady = true;
    public int ultimateRequiredCombo = 5;
    private int currentCombo = 0;
    private float lastAttackTime;
    private float comboTimeWindow = 1.5f;
    
    [Header("Dash/Roll Movement")]
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private float dashTimer = 0f;
    private bool dashReady = true;
    private bool isDashing = false;
    private Vector3 dashDirection;
    
    public float rollDistance = 4f;
    public float rollDuration = 0.3f;
    public float rollCooldown = 1.5f;
    private float rollTimer = 0f;
    private bool rollReady = true;
    private bool isRolling = false;
    private Vector3 rollDirection;
    
    public float dashDamage = 25f;
    public bool dashDamagesEnemies = true;
    
    [Header("Effects")]
    public GameObject powerPunchEffect;
    public GameObject roundhouseKickEffect;
    public GameObject groundSlamEffect;
    public GameObject ultimateEffect;
    
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false;
    public float invincibilityDuration = 1f;
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;
    
    [Header("UI")]
    public float healthBarWidth = 200f;
    public float healthBarHeight = 20f;
    
    private Rigidbody rb;
    private float currentSpeed;
    private bool isGrounded;
    private Animator animator;
    private bool canAttack = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        
        currentSpeed = walkSpeed;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        
        // Initialize UI
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
            UIManager.Instance.ShowVehicleUI(false);
        }
        
        // Play gameplay music
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlayMusic("Gameplay");
        }
        
        // Initialize EffectManager if needed
        if (EffectManager.Instance == null)
        {
            GameObject effectManager = new GameObject("EffectManager");
            effectManager.AddComponent<EffectManager>();
        }
    }
    
    void Update()
    {
        // Handle invincibility frames
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }
        
        if (isDead) return;
        
        // Update cooldowns
        UpdateCooldowns();
        
        // Update combo timer
        if (Time.time - lastAttackTime > comboTimeWindow)
        {
            currentCombo = 0;
            // Update UI when combo resets
            if (UIManager.Instance)
            {
                UIManager.Instance.UpdateCombo(currentCombo);
            }
        }
        
        HandleMovement();
        HandleCombat();
        HandleSpecialMoves();
        HandleDashAndRoll();
    }
    
    void UpdateCooldowns()
    {
        if (!powerPunchReady)
        {
            powerPunchTimer -= Time.deltaTime;
            if (powerPunchTimer <= 0f)
            {
                powerPunchReady = true;
                Debug.Log("POWER PUNCH READY!");
            }
            // Update UI cooldown
            if (UIManager.Instance)
            {
                UIManager.Instance.UpdateSpecialMoveCooldown("PowerPunch", powerPunchTimer, powerPunchCooldown);
            }
        }
        
        if (!roundhouseKickReady)
        {
            roundhouseKickTimer -= Time.deltaTime;
            if (roundhouseKickTimer <= 0f)
            {
                roundhouseKickReady = true;
                Debug.Log("ROUNDHOUSE KICK READY!");
            }
            if (UIManager.Instance)
            {
                UIManager.Instance.UpdateSpecialMoveCooldown("RoundhouseKick", roundhouseKickTimer, roundhouseKickCooldown);
            }
        }
        
        if (!groundSlamReady)
        {
            groundSlamTimer -= Time.deltaTime;
            if (groundSlamTimer <= 0f)
            {
                groundSlamReady = true;
                Debug.Log("GROUND SLAM READY!");
            }
            if (UIManager.Instance)
            {
                UIManager.Instance.UpdateSpecialMoveCooldown("GroundSlam", groundSlamTimer, groundSlamCooldown);
            }
        }
        
        if (!ultimateReady)
        {
            ultimateTimer -= Time.deltaTime;
            if (ultimateTimer <= 0f)
            {
                ultimateReady = true;
                Debug.Log("ULTIMATE MOVE READY!");
            }
            if (UIManager.Instance)
            {
                UIManager.Instance.UpdateSpecialMoveCooldown("Ultimate", ultimateTimer, ultimateCooldown);
            }
        }
        
        if (!dashReady)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                dashReady = true;
                Debug.Log("⚡ DASH READY! ⚡");
            }
        }
        
        if (!rollReady)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0f)
            {
                rollReady = true;
                Debug.Log("🌀 ROLL READY! 🌀");
            }
        }
    }
    
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 move = moveDirection * currentSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
        
        // Run with Left Shift
        if (Input.GetKey(KeyCode.LeftShift) && !isDashing && !isRolling)
            currentSpeed = runSpeed;
        else
            currentSpeed = walkSpeed;
        
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDashing && !isRolling)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            
            // Play jump sound
            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySFX("Jump");
            }
        }
    }
    
    void HandleCombat()
    {
        if (!canAttack || isDashing || isRolling) return;
        
        // Light Punch (Left Mouse)
        if (Input.GetMouseButtonDown(0))
        {
            PerformAttack("Punch", punchDamage, punchRange);
            UpdateCombo();
        }
        
        // Heavy Kick (Right Mouse)
        if (Input.GetMouseButtonDown(1))
        {
            PerformAttack("Kick", kickDamage, kickRange);
            UpdateCombo();
        }
    }
    
    void HandleSpecialMoves()
    {
        if (isDashing || isRolling || isDead) return;
        
        // Power Punch (Q key)
        if (Input.GetKeyDown(KeyCode.Q) && powerPunchReady)
        {
            PowerPunch();
        }
        
        // Roundhouse Kick (E key)
        if (Input.GetKeyDown(KeyCode.E) && roundhouseKickReady)
        {
            RoundhouseKick();
        }
        
        // Ground Slam (R key)
        if (Input.GetKeyDown(KeyCode.R) && groundSlamReady)
        {
            GroundSlam();
        }
        
        // Ultimate Move (F key) - requires combo
        if (Input.GetKeyDown(KeyCode.F) && ultimateReady && currentCombo >= ultimateRequiredCombo)
        {
            UltimateMove();
        }
    }
    
    void HandleDashAndRoll()
    {
        // Dash (Left Shift + Direction) - only when not already dashing/rolling
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashReady && !isDashing && !isRolling && !isDead)
        {
            // Get movement direction
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            dashDirection = new Vector3(horizontal, 0, vertical).normalized;
            
            // If no input, dash forward
            if (dashDirection.magnitude < 0.1f)
            {
                dashDirection = transform.forward;
            }
            
            StartDash();
        }
        
        // Roll (Left Control or C)
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C)) && rollReady && !isRolling && !isDashing && !isDead)
        {
            // Get movement direction
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            rollDirection = new Vector3(horizontal, 0, vertical).normalized;
            
            // If no input, roll backward
            if (rollDirection.magnitude < 0.1f)
            {
                rollDirection = -transform.forward;
            }
            
            StartRoll();
        }
        
        // Update dash
        if (isDashing)
        {
            UpdateDash();
        }
        
        // Update roll
        if (isRolling)
        {
            UpdateRoll();
        }
    }
    
    void UpdateCombo()
    {
        if (Time.time - lastAttackTime <= comboTimeWindow)
        {
            currentCombo++;
            Debug.Log($"🔥 COMBO x{currentCombo} 🔥");
        }
        else
        {
            currentCombo = 1;
        }
        lastAttackTime = Time.time;
        
        // Play combo sound on high combos
        if (currentCombo >= 3 && AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("ComboHit");
        }
        
        // Update UI combo display
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateCombo(currentCombo);
        }
    }
    
    void PerformAttack(string attackName, float damage, float range)
    {
        canAttack = false;
        
        if (animator) animator.SetTrigger(attackName);
        
        // Play attack sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX(attackName);
        }
        
        // Combo bonus
        float finalDamage = damage;
        if (currentCombo >= 3)
        {
            finalDamage = damage * 1.5f;
            Debug.Log($"COMBO BONUS! +50% damage!");
        }
        if (currentCombo >= 5)
        {
            finalDamage = damage * 2f;
            Debug.Log($"SUPER COMBO! +100% damage!");
        }
        
        // Raycast for hit
        Ray ray = new Ray(transform.position + Vector3.up * 1f, transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, range))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.TakeDamage(finalDamage);
                    Debug.Log($"{attackName} hit enemy for {finalDamage} damage!");
                    
                    // Spawn hit effect
                    if (EffectManager.Instance)
                    {
                        EffectManager.Instance.SpawnHitImpact(hit.point, hit.normal);
                    }
                }
            }
        }
        
        Invoke("ResetAttack", attackCooldown);
    }
    
    void ResetAttack()
    {
        canAttack = true;
    }
    
    void PowerPunch()
    {
        powerPunchReady = false;
        powerPunchTimer = powerPunchCooldown;
        
        if (animator) animator.SetTrigger("PowerPunch");
        
        // Play special move sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("PowerPunch");
        }
        
        // Spawn effect
        if (powerPunchEffect)
        {
            Instantiate(powerPunchEffect, transform.position + transform.forward * 2f, Quaternion.identity);
        }
        
        // Area damage
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * 2f, 2f);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript)
                {
                    enemyScript.TakeDamage(powerPunchDamage);
                    Debug.Log($"💥 POWER PUNCH! {powerPunchDamage} damage! 💥");
                    
                    // Spawn hit effect
                    if (EffectManager.Instance)
                    {
                        EffectManager.Instance.SpawnHitImpact(enemy.transform.position, transform.forward);
                    }
                }
            }
        }
        
        // Knockback effect
        rb.AddForce(transform.forward * 500f, ForceMode.Impulse);
        
        // Update UI cooldown
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateSpecialMoveCooldown("PowerPunch", powerPunchTimer, powerPunchCooldown);
        }
    }
    
    void RoundhouseKick()
    {
        roundhouseKickReady = false;
        roundhouseKickTimer = roundhouseKickCooldown;
        
        if (animator) animator.SetTrigger("RoundhouseKick");
        
        // Play special move sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("RoundhouseKick");
        }
        
        // Spawn effect
        if (roundhouseKickEffect)
        {
            Instantiate(roundhouseKickEffect, transform.position, Quaternion.identity);
        }
        
        // 360-degree damage
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 2.5f);
        int enemiesHit = 0;
        
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript)
                {
                    enemyScript.TakeDamage(roundhouseKickDamage);
                    enemiesHit++;
                    
                    // Spawn hit effect
                    if (EffectManager.Instance)
                    {
                        EffectManager.Instance.SpawnHitImpact(enemy.transform.position, transform.forward);
                    }
                }
            }
        }
        
        Debug.Log($"🌀 ROUNDHOUSE KICK! Hit {enemiesHit} enemies for {roundhouseKickDamage} damage! 🌀");
        
        // Update UI cooldown
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateSpecialMoveCooldown("RoundhouseKick", roundhouseKickTimer, roundhouseKickCooldown);
        }
    }
    
    void GroundSlam()
    {
        groundSlamReady = false;
        groundSlamTimer = groundSlamCooldown;
        
        if (animator) animator.SetTrigger("GroundSlam");
        
        // Play special move sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("GroundSlam");
        }
        
        // Spawn effect
        if (groundSlamEffect)
        {
            Instantiate(groundSlamEffect, transform.position, Quaternion.identity);
        }
        
        // Jump then slam
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        Invoke("SlamDown", 0.3f);
        
        // Update UI cooldown
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateSpecialMoveCooldown("GroundSlam", groundSlamTimer, groundSlamCooldown);
        }
    }
    
    void SlamDown()
    {
        rb.AddForce(Vector3.down * 20f, ForceMode.Impulse);
        
        // Area damage
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, groundSlamRadius);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript)
                {
                    enemyScript.TakeDamage(groundSlamDamage);
                }
            }
        }
        
        // Spawn ground slam effect
        if (EffectManager.Instance)
        {
            EffectManager.Instance.SpawnGroundSlam(transform.position);
        }
        
        // Camera shake for ground slam
        if (CameraController.Instance)
        {
            CameraController.Instance.ShakeCamera(0.3f, 0.2f);
        }
        
        Debug.Log($"🌍 GROUND SLAM! {groundSlamDamage} damage in radius! 🌍");
    }
    
    void UltimateMove()
    {
        ultimateReady = false;
        ultimateTimer = ultimateCooldown;
        
        if (animator) animator.SetTrigger("Ultimate");
        
        // Play ultimate sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("Ultimate");
        }
        
        // Spawn effect
        if (ultimateEffect)
        {
            Instantiate(ultimateEffect, transform.position, Quaternion.identity);
        }
        
        // Spawn ultimate particle effect
        if (EffectManager.Instance)
        {
            EffectManager.Instance.SpawnUltimate(transform.position);
        }
        
        // Massive area damage
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, ultimateRange);
        int enemiesHit = 0;
        
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript)
                {
                    enemyScript.TakeDamage(ultimateDamage);
                    enemiesHit++;
                    
                    // Spawn explosion effect on each enemy
                    if (EffectManager.Instance)
                    {
                        EffectManager.Instance.SpawnExplosion(enemy.transform.position);
                    }
                }
            }
        }
        
        // Reset combo after ultimate
        currentCombo = 0;
        
        // Camera shake on ultimate
        if (CameraController.Instance)
        {
            CameraController.Instance.ShakeCamera(0.5f, 0.3f);
        }
        
        // Update UI
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateCombo(currentCombo);
            UIManager.Instance.UpdateSpecialMoveCooldown("Ultimate", ultimateTimer, ultimateCooldown);
            UIManager.Instance.ShowNotification($"ULTIMATE! {enemiesHit} ENEMIES DESTROYED!");
        }
        
        Debug.Log($"✨ ULTIMATE MOVE! {enemiesHit} enemies destroyed for {ultimateDamage} damage! ✨");
    }
    
    void StartDash()
    {
        isDashing = true;
        dashReady = false;
        dashTimer = dashCooldown;
        
        // Set invincible during dash
        isInvincible = true;
        invincibilityTimer = dashDuration;
        
        // Play dash sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("Dash");
        }
        
        // Spawn dash trail effect
        if (EffectManager.Instance)
        {
            EffectManager.Instance.SpawnDashTrail(transform, dashDuration);
        }
        
        // Play dash animation
        if (animator) animator.SetTrigger("Dash");
        
        Debug.Log($"⚡ DASH! ⚡");
        
        // Damage enemies in path
        if (dashDamagesEnemies)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1.5f, dashDirection, dashDistance);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy)
                    {
                        enemy.TakeDamage(dashDamage);
                        Debug.Log($"💥 DASH ATTACK! {dashDamage} damage! 💥");
                        
                        // Spawn hit effect
                        if (EffectManager.Instance)
                        {
                            EffectManager.Instance.SpawnHitImpact(hit.point, dashDirection);
                        }
                        
                        // Knockback enemy
                        Rigidbody enemyRb = hit.collider.GetComponent<Rigidbody>();
                        if (enemyRb)
                        {
                            enemyRb.AddForce(dashDirection * 500f, ForceMode.Impulse);
                        }
                    }
                }
            }
        }
        
        // Disable normal movement during dash
        StartCoroutine(PerformDash());
    }
    
    IEnumerator PerformDash()
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + dashDirection * dashDistance;
        
        while (Time.time - startTime < dashDuration)
        {
            float t = (Time.time - startTime) / dashDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            
            // Create trail effect
            CreateDashTrail();
            
            yield return null;
        }
        
        // Final position
        transform.position = targetPosition;
        isDashing = false;
        
        Debug.Log("⚡ Dash complete! ⚡");
    }
    
    void UpdateDash()
    {
        // Additional dash logic if needed
    }
    
    void StartRoll()
    {
        isRolling = true;
        rollReady = false;
        rollTimer = rollCooldown;
        
        // Set invincible during roll
        isInvincible = true;
        invincibilityTimer = rollDuration;
        
        // Play roll sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("Roll");
        }
        
        // Play roll animation
        if (animator) animator.SetTrigger("Roll");
        
        Debug.Log($"🌀 ROLL! 🌀");
        
        // Start roll movement
        StartCoroutine(PerformRoll());
    }
    
    IEnumerator PerformRoll()
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + rollDirection * rollDistance;
        
        // Reduce player scale slightly during roll
        Vector3 originalScale = transform.localScale;
        transform.localScale = new Vector3(originalScale.x, originalScale.y * 0.7f, originalScale.z);
        
        while (Time.time - startTime < rollDuration)
        {
            float t = (Time.time - startTime) / rollDuration;
            
            // Smooth movement with easing
            float easeT = 1 - Mathf.Pow(1 - t, 2);
            transform.position = Vector3.Lerp(startPosition, targetPosition, easeT);
            
            // Rotation effect
            transform.Rotate(0, 360 * Time.deltaTime, 0);
            
            yield return null;
        }
        
        // Reset scale
        transform.localScale = originalScale;
        
        // Reset rotation
        transform.rotation = Quaternion.identity;
        
        isRolling = false;
        
        Debug.Log("🌀 Roll complete! 🌀");
    }
    
    void UpdateRoll()
    {
        // Additional roll logic if needed
    }
    
    void CreateDashTrail()
    {
        // Create simple trail effect
        GameObject trail = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        trail.transform.localScale = Vector3.one * 0.3f;
        trail.transform.position = transform.position - dashDirection * 0.5f;
        trail.GetComponent<Renderer>().material.color = Color.cyan;
        Destroy(trail, 0.1f);
    }
    
    public void TakeDamage(float amount)
    {
        if (isDead) return;
        if (isInvincible) return;
        
        currentHealth -= amount;
        Debug.Log($"Player took {amount} damage! Health: {currentHealth}/{maxHealth}");
        
        // Play damage sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("Damage");
        }
        
        // Spawn blood effect
        if (EffectManager.Instance)
        {
            EffectManager.Instance.SpawnBlood(transform.position);
        }
        
        // Start invincibility frames
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        
        // Reset combo when hit
        currentCombo = 0;
        
        // Camera shake on damage
        if (CameraController.Instance)
        {
            CameraController.Instance.OnPlayerDamage();
        }
        
        // Update UI
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
            UIManager.Instance.ShowDamageFlash();
            UIManager.Instance.UpdateCombo(currentCombo);
        }
        
        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        isDead = true;
        Debug.Log("💀 GAME OVER! 💀");
        
        // Play death sound
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySFX("Death");
            AudioManager.Instance.PlayMusic("GameOver");
        }
        
        // Spawn death explosion
        if (EffectManager.Instance)
        {
            EffectManager.Instance.SpawnExplosion(transform.position);
        }
        
        // Show game over UI
        if (UIManager.Instance)
        {
            int finalScore = GameManager.Instance ? GameManager.Instance.score : 0;
            UIManager.Instance.ShowGameOver(finalScore);
        }
        
        // Save game on death
        if (SaveSystem.Instance)
        {
            SaveSystem.Instance.SaveGame();
        }
        
        enabled = false;
    }
    
    void OnGUI()
    {
        // Fallback UI if UIManager doesn't exist
        if (UIManager.Instance != null) return;
        
        if (isDead) return;
        
        float yPos = 10;
        
        // Health bar
        float healthPercent = currentHealth / maxHealth;
        GUI.Box(new Rect(10, yPos, healthBarWidth, healthBarHeight), "");
        GUI.backgroundColor = Color.red;
        GUI.Box(new Rect(10, yPos, healthBarWidth * healthPercent, healthBarHeight), "");
        GUI.Label(new Rect(15, yPos + 2, 100, 20), $"{currentHealth}/{maxHealth}");
        
        GUI.backgroundColor = Color.white;
        
        // Combo counter
        if (currentCombo > 0)
        {
            GUI.color = Color.yellow;
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - 80, 100, 50), 
                      $"x{currentCombo} COMBO!");
            GUI.color = Color.white;
        }
        
        // Special moves UI
        yPos = 80;
        GUI.color = powerPunchReady ? Color.green : Color.red;
        GUI.Label(new Rect(Screen.width - 150, yPos, 140, 25), 
                  $"💪 POWER [Q]: {(powerPunchReady ? "READY" : Mathf.CeilToInt(powerPunchTimer) + "s")}");
        
        yPos += 30;
        GUI.color = roundhouseKickReady ? Color.green : Color.red;
        GUI.Label(new Rect(Screen.width - 150, yPos, 140, 25), 
                  $"🌀 KICK [E]: {(roundhouseKickReady ? "READY" : Mathf.CeilToInt(roundhouseKickTimer) + "s")}");
        
        yPos += 30;
        GUI.color = groundSlamReady ? Color.green : Color.red;
        GUI.Label(new Rect(Screen.width - 150, yPos, 140, 25), 
                  $"🌍 SLAM [R]: {(groundSlamReady ? "READY" : Mathf.CeilToInt(groundSlamTimer) + "s")}");
        
        yPos += 30;
        GUI.color = (ultimateReady && currentCombo >= ultimateRequiredCombo) ? Color.cyan : Color.gray;
        GUI.Label(new Rect(Screen.width - 150, yPos, 180, 25), 
                  $"✨ ULTIMATE [F]: {(ultimateReady && currentCombo >= ultimateRequiredCombo ? "READY!" : $"Need {ultimateRequiredCombo - currentCombo} combo")}");
        
        // Dash and Roll UI
        yPos += 40;
        GUI.color = dashReady ? Color.cyan : Color.gray;
        GUI.Label(new Rect(Screen.width - 150, yPos, 140, 25), 
                  $"⚡ DASH [Shift]: {(dashReady ? "READY" : Mathf.CeilToInt(dashTimer) + "s")}");
        
        yPos += 30;
        GUI.color = rollReady ? Color.magenta : Color.gray;
        GUI.Label(new Rect(Screen.width - 150, yPos, 140, 25), 
                  $"🌀 ROLL [C]: {(rollReady ? "READY" : Mathf.CeilToInt(rollTimer) + "s")}");
        
        GUI.color = Color.white;
    }
    
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
    
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}