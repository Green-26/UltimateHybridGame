using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public float enginePower = 1000f;
    public float maxSpeed = 40f;
    public float turnSpeed = 100f;
    public float ramDamage = 40f;
    
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        
        rb.mass = 1000f;
    }
    
    void Update()
    {
        float accelerate = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");
        
        // Drive
        Vector3 force = transform.forward * accelerate * enginePower * Time.deltaTime;
        rb.AddForce(force);
        
        // Speed limit
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        
        // Turn
        float turnAmount = turn * turnSpeed * Time.deltaTime;
        transform.Rotate(0, turnAmount, 0);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && rb.linearVelocity.magnitude > 15f)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.TakeDamage(ramDamage);
                Debug.Log($"RAMMED ENEMY for {ramDamage} damage!");
            }
        }
    }
}