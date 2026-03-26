// ============================================
// Ultimate Hybrid Game - Player Controller
// Copyright (c) 2026 Bertin ABIJURU
// All Rights Reserved
// ============================================

using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 15f;
    public float lifetime = 3f;
    
    private Vector3 direction;
    
    void Start()
    {
        // Destroy projectile after lifetime
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        // Move projectile
        transform.Translate(direction * speed * Time.deltaTime);
    }
    
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player)
            {
                player.TakeDamage(damage);
                Debug.Log($"Projectile hit player for {damage} damage!");
            }
            Destroy(gameObject);
        }
        
        // Destroy on hitting anything else (except enemies)
        if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}