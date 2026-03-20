using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 8f;
    
    [Header("Combat")]
    public float punchDamage = 15f;
    public float punchRange = 2f;
    
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
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        
        currentSpeed = walkSpeed;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
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
        
        // Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 move = moveDirection * currentSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
        
        // Run with Left Shift
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = runSpeed;
        else
            currentSpeed = walkSpeed;
        
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
        
        // Punch (Left Mouse Button)
        if (Input.GetMouseButtonDown(0))
        {
            Punch();
        }
    }
    
    void Punch()
    {
        Debug.Log("PUNCH!");
        
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, punchRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("HIT ENEMY!");
            }
        }
    }
    
    public void TakeDamage(float amount)
    {
        if (isDead) return;
        if (isInvincible) return;
        
        currentHealth -= amount;
        Debug.Log($"Player took {amount} damage! Health: {currentHealth}/{maxHealth}");
        
        // Start invincibility frames
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        
        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        isDead = true;
        Debug.Log("GAME OVER! Player has died.");
        
        // Disable player movement
        enabled = false;
    }
    
    void OnGUI()
    {
        if (isDead) return;
        
        // Health bar background
        GUI.Box(new Rect(10, 10, healthBarWidth, healthBarHeight), "");
        
        // Health bar fill
        float healthPercent = currentHealth / maxHealth;
        GUI.backgroundColor = Color.red;
        GUI.Box(new Rect(10, 10, healthBarWidth * healthPercent, healthBarHeight), "");
        
        // Health text
        GUI.Label(new Rect(15, 12, 100, 20), $"{currentHealth}/{maxHealth}");
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