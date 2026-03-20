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
    
    private Rigidbody rb;
    private float currentSpeed;
    private bool isGrounded;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        
        currentSpeed = walkSpeed;
    }
    
    void Update()
    {
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