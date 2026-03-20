using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;
    
    [Header("Particle Effects")]
    public GameObject hitParticle;
    public GameObject explosionParticle;
    public GameObject bloodParticle;
    public GameObject sparkParticle;
    public GameObject smokeParticle;
    public GameObject fireParticle;
    public GameObject healParticle;
    public GameObject shieldParticle;
    public GameObject dashTrailParticle;
    public GameObject groundSlamParticle;
    public GameObject ultimateParticle;
    
    [Header("Trail Effects")]
    public GameObject dashTrail;
    public GameObject speedTrail;
    public GameObject exhaustTrail;
    
    [Header("Impact Effects")]
    public GameObject groundImpact;
    public GameObject wallImpact;
    public GameObject waterSplash;
    
    [Header("Post Processing Settings")]
    public bool enablePostProcessing = true;
    public float bloomIntensity = 0.5f;
    public float motionBlurAmount = 0.3f;
    public float vignetteIntensity = 0.3f;
    public Color vignetteColor = Color.black;
    
    [Header("Performance Settings")]
    public int maxParticles = 100;
    public float particleLifetime = 2f;
    public bool enablePooling = true;
    
    private Queue<GameObject> particlePool = new Queue<GameObject>();
    private Dictionary<string, Queue<GameObject>> effectPools = new Dictionary<string, Queue<GameObject>>();
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        InitializePools();
        ApplyPerformanceSettings();
    }
    
    void InitializePools()
    {
        if (!enablePooling) return;
        
        // Create pools for each effect type
        CreatePool("Hit", hitParticle, 20);
        CreatePool("Explosion", explosionParticle, 10);
        CreatePool("Blood", bloodParticle, 30);
        CreatePool("Spark", sparkParticle, 50);
        CreatePool("Smoke", smokeParticle, 20);
        CreatePool("Fire", fireParticle, 15);
        CreatePool("Heal", healParticle, 10);
        CreatePool("Shield", shieldParticle, 10);
        CreatePool("DashTrail", dashTrailParticle, 30);
        CreatePool("GroundSlam", groundSlamParticle, 5);
        CreatePool("Ultimate", ultimateParticle, 3);
    }
    
    void CreatePool(string name, GameObject prefab, int size)
    {
        if (prefab == null) return;
        
        Queue<GameObject> pool = new Queue<GameObject>();
        
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
        
        effectPools[name] = pool;
    }
    
    #region Particle Effects
    
    public void SpawnEffect(string effectName, Vector3 position, float duration = 1f)
    {
        if (!effectPools.ContainsKey(effectName)) return;
        
        GameObject effect = GetFromPool(effectName);
        if (effect == null) return;
        
        effect.transform.position = position;
        effect.SetActive(true);
        
        StartCoroutine(ReturnToPool(effect, effectName, duration));
    }
    
    public void SpawnEffect(string effectName, Vector3 position, Quaternion rotation, float duration = 1f)
    {
        if (!effectPools.ContainsKey(effectName)) return;
        
        GameObject effect = GetFromPool(effectName);
        if (effect == null) return;
        
        effect.transform.position = position;
        effect.transform.rotation = rotation;
        effect.SetActive(true);
        
        StartCoroutine(ReturnToPool(effect, effectName, duration));
    }
    
    GameObject GetFromPool(string poolName)
    {
        if (!effectPools.ContainsKey(poolName)) return null;
        
        Queue<GameObject> pool = effectPools[poolName];
        
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        else
        {
            // Create new if pool empty
            GameObject prefab = GetPrefabByName(poolName);
            if (prefab != null)
            {
                return Instantiate(prefab);
            }
        }
        
        return null;
    }
    
    GameObject GetPrefabByName(string name)
    {
        switch (name)
        {
            case "Hit": return hitParticle;
            case "Explosion": return explosionParticle;
            case "Blood": return bloodParticle;
            case "Spark": return sparkParticle;
            case "Smoke": return smokeParticle;
            case "Fire": return fireParticle;
            case "Heal": return healParticle;
            case "Shield": return shieldParticle;
            case "DashTrail": return dashTrailParticle;
            case "GroundSlam": return groundSlamParticle;
            case "Ultimate": return ultimateParticle;
            default: return null;
        }
    }
    
    IEnumerator ReturnToPool(GameObject obj, string poolName, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (obj != null)
        {
            obj.SetActive(false);
            effectPools[poolName].Enqueue(obj);
        }
    }
    
    #endregion
    
    #region Impact Effects
    
    public void SpawnHitImpact(Vector3 position, Vector3 normal)
    {
        SpawnEffect("Hit", position, Quaternion.LookRotation(normal), 0.5f);
        SpawnEffect("Spark", position, 0.3f);
    }
    
    public void SpawnExplosion(Vector3 position)
    {
        SpawnEffect("Explosion", position, 1f);
        SpawnEffect("Smoke", position, 2f);
        SpawnEffect("Fire", position, 1.5f);
        
        // Camera shake
        if (CameraController.Instance)
        {
            CameraController.Instance.OnExplosion(position);
        }
    }
    
    public void SpawnBlood(Vector3 position)
    {
        SpawnEffect("Blood", position, 0.5f);
    }
    
    public void SpawnGroundSlam(Vector3 position)
    {
        SpawnEffect("GroundSlam", position, 0.8f);
        SpawnEffect("Smoke", position, 1f);
        
        // Ground impact effect
        if (groundImpact)
        {
            GameObject impact = Instantiate(groundImpact, position, Quaternion.identity);
            Destroy(impact, 0.5f);
        }
    }
    
    public void SpawnUltimate(Vector3 position)
    {
        SpawnEffect("Ultimate", position, 1.5f);
        SpawnEffect("Explosion", position, 1f);
        
        // Camera shake
        if (CameraController.Instance)
        {
            CameraController.Instance.ShakeCamera(0.5f, 0.4f);
        }
    }
    
    public void SpawnDashTrail(Transform target, float duration)
    {
        if (dashTrail)
        {
            GameObject trail = Instantiate(dashTrail, target.position, target.rotation);
            trail.transform.parent = target;
            Destroy(trail, duration);
        }
    }
    
    #endregion
    
    #region Performance Optimization
    
    void ApplyPerformanceSettings()
    {
        // Limit particle count
        QualitySettings.particleRaycastBudget = maxParticles;
        
        // Apply LOD settings
        QualitySettings.lodBias = 1f;
        QualitySettings.maximumLODLevel = 0;
        
        // Texture quality
        QualitySettings.masterTextureLimit = 0;
        
        // Shadow settings
        QualitySettings.shadows = ShadowQuality.HardOnly;
        QualitySettings.shadowDistance = 50f;
        
        // VSync
        QualitySettings.vSyncCount = 1;
        
        // Frame rate
        Application.targetFrameRate = 60;
    }
    
    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
        
        switch (level)
        {
            case 0: // Low
                QualitySettings.shadows = ShadowQuality.Disable;
                QualitySettings.masterTextureLimit = 2;
                Application.targetFrameRate = 30;
                break;
                
            case 1: // Medium
                QualitySettings.shadows = ShadowQuality.HardOnly;
                QualitySettings.masterTextureLimit = 1;
                Application.targetFrameRate = 60;
                break;
                
            case 2: // High
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.masterTextureLimit = 0;
                Application.targetFrameRate = 60;
                break;
                
            case 3: // Ultra
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.masterTextureLimit = 0;
                Application.targetFrameRate = -1;
                break;
        }
        
        Debug.Log($"Quality set to level: {level}");
    }
    
    #endregion
}