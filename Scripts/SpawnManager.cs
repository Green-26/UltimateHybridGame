// ============================================
// Ultimate Hybrid Game - Player Controller
// Copyright (c) 2026 Bertin ABIJURU
// All Rights Reserved
// ============================================

using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject normalEnemyPrefab;
    public GameObject fastEnemyPrefab;
    public GameObject heavyEnemyPrefab;
    public GameObject rangedEnemyPrefab;
    public GameObject explosiveEnemyPrefab;
    public GameObject bossEnemyPrefab;
    
    [Header("Spawn Settings")]
    public float spawnInterval = 5f;
    public int maxEnemies = 10;
    public float spawnRadius = 20f;
    
    [Header("Wave Settings")]
    public int currentWave = 1;
    public int enemiesPerWave = 5;
    public float waveDelay = 3f;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool isSpawning = true;
    private bool waveInProgress = false;
    
    void Start()
    {
        StartCoroutine(StartWave());
    }
    
    System.Collections.IEnumerator StartWave()
    {
        waveInProgress = true;
        Debug.Log($"🌊 WAVE {currentWave} STARTING! 🌊");
        
        int enemiesToSpawn = enemiesPerWave + (currentWave - 1) * 2;
        
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (activeEnemies.Count >= maxEnemies)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }
            
            SpawnRandomEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
        
        waveInProgress = false;
        Debug.Log($"🌊 WAVE {currentWave} COMPLETE! Next wave soon... 🌊");
        
        yield return new WaitForSeconds(waveDelay);
        currentWave++;
        StartCoroutine(StartWave());
    }
    
    void SpawnRandomEnemy()
    {
        // Determine enemy type based on wave number
        GameObject enemyToSpawn = null;
        int random = Random.Range(0, 100);
        
        if (currentWave >= 10 && random < 10)
        {
            // Boss spawns on wave 10+
            enemyToSpawn = bossEnemyPrefab;
            Debug.Log("👑 BOSS SPAWNED! 👑");
        }
        else if (currentWave >= 5 && random < 20)
        {
            // Heavy and Ranged spawn more in later waves
            if (Random.value > 0.5f)
                enemyToSpawn = heavyEnemyPrefab;
            else
                enemyToSpawn = rangedEnemyPrefab;
        }
        else if (currentWave >= 3 && random < 30)
        {
            // Explosive enemies appear from wave 3
            enemyToSpawn = explosiveEnemyPrefab;
        }
        else if (random < 40)
        {
            // Fast enemies
            enemyToSpawn = fastEnemyPrefab;
        }
        else
        {
            // Normal enemies
            enemyToSpawn = normalEnemyPrefab;
        }
        
        if (enemyToSpawn == null)
        {
            enemyToSpawn = normalEnemyPrefab;
        }
        
        // Spawn at random position around player
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        GameObject newEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy);
        
        // Track enemy death
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript)
        {
            // We'll add death tracking later
        }
    }
    
    Vector3 GetRandomSpawnPosition()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return Vector3.zero;
        }
        
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = player.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        
        // Raycast to ground
        RaycastHit hit;
        if (Physics.Raycast(spawnPos + Vector3.up * 10f, Vector3.down, out hit, 20f))
        {
            spawnPos.y = hit.point.y;
        }
        
        return spawnPos;
    }
    
    public void EnemyDied(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}