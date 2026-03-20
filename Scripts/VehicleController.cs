using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VehicleController : MonoBehaviour
{
    [Header("Engine")]
    public float enginePower = 1200f;
    public float maxSpeed = 40f;
    public float brakeForce = 800f;
    public float turnSpeed = 100f;
    
    [Header("Health")]
    public float maxHealth = 200f;
    public float currentHealth;
    public bool isDestroyed = false;
    
    [Header("Machine Gun")]
    public bool hasMachineGun = true;
    public GameObject machineGunPoint;
    public GameObject bulletPrefab;
    public float machineGunDamage = 15f;
    public float machineGunFireRate = 0.15f;
    public float machineGunRange = 30f;
    public int machineGunAmmo = 300;
    public int maxMachineGunAmmo = 300;
    private float lastMachineGunTime = 0f;
    
    [Header("Rocket Launcher")]
    public bool hasRocketLauncher = true;
    public GameObject rocketPoint;
    public GameObject rocketPrefab;
    public float rocketDamage = 80f;
    public float rocketFireRate = 1.5f;
    public float rocketExplosionRadius = 5f;
    public int rocketAmmo = 10;
    public int maxRocketAmmo = 10;
    private float lastRocketTime = 0f;
    
    [Header("Mine Dropper")]
    public bool hasMines = true;
    public GameObject minePrefab;
    public float mineDamage = 50f;
    public int mineAmmo = 5;
    public int maxMineAmmo = 5;
    public float mineDropCooldown = 2f;
    private float lastMineTime = 0f;
    
    [Header("Ram Attack")]
    public float ramDamage = 40f;
    public float ramForce = 800f;
    public float ramCooldown = 1f;
    private float lastRamTime = 0f;
    private bool isRamming = false;
    
    [Header("Shield")]
    public bool hasShield = false;
    public GameObject shieldVisual;
    public float shieldHealth = 50f;
    public float maxShieldHealth = 50f;
    public float shieldRegenRate = 5f;
    private float currentShieldHealth;
    private bool shieldActive = false;
    
    [Header("Nitrous Boost")]
    public bool hasNitrous = true;
    public float nitrousBoost = 2f;
    public float nitrousDuration = 3f;
    public float nitrousCooldown = 8f;
    private float nitrousTimer = 0f;
    private bool nitrousReady = true;
    private bool nitrousActive = false;
    private float originalMaxSpeed;
    
    [Header("Visual Effects")]
    public GameObject muzzleFlash;
    public GameObject explosionEffect;
    public GameObject hitEffect;
    public GameObject shieldHitEffect;
    
    [Header("Audio")]
    public AudioClip machineGunSound;
    public AudioClip rocketSound;
    public AudioClip mineSound;
    public AudioClip nitrousSound;
    public AudioClip explosionSound;
    public AudioClip hitSound;
    
    private Rigidbody rb;
    private float currentSpeed;
    private bool isBraking = false;
    private Camera playerCamera;
    private AudioSource audioSource;
    private bool isPlayerInside = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        
        rb.mass = 1000f;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        currentHealth = maxHealth;
        originalMaxSpeed = maxSpeed;
        
        currentShieldHealth = shieldHealth;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        playerCamera = Camera.main;
        
        // Find player for camera reference
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            playerCamera = player.GetComponentInChildren<Camera>();
        }
        
        // Deactivate shield visual initially
        if (shieldVisual)
            shieldVisual.SetActive(false);
        
        // Initialize UI
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateVehicleHealth(currentHealth, maxHealth);
            UIManager.Instance.UpdateMachineGunAmmo(machineGunAmmo, maxMachineGunAmmo);
            UIManager.Instance.UpdateRocketAmmo(rocketAmmo, maxRocketAmmo);
            UIManager.Instance.UpdateMineAmmo(mineAmmo, maxMineAmmo);
            UIManager.Instance.UpdateCurrentWeapon("Machine Gun");
        }
        
        Debug.Log("Vehicle armed and ready!");
    }
    
    void Update()
    {
        if (isDestroyed) return;
        
        HandleDriving();
        HandleWeapons();
        HandleNitrous();
        UpdateShield();
        UpdateUI();
        HandleEnterExit();
    }
    
    void HandleDriving()
    {
        if (!isPlayerInside) return;
        
        float accelerate = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");
        
        // Brake with Space
        isBraking = Input.GetKey(KeyCode.Space);
        
        // Apply engine force
        if (!isBraking && !nitrousActive)
        {
            Vector3 force = transform.forward * accelerate * enginePower * Time.deltaTime;
            rb.AddForce(force);
        }
        else if (nitrousActive)
        {
            Vector3 force = transform.forward * accelerate * enginePower * nitrousBoost * Time.deltaTime;
            rb.AddForce(force);
        }
        
        // Apply brake
        if (isBraking || accelerate < 0)
        {
            Vector3 brakeForceVector = -rb.linearVelocity.normalized * brakeForce * Time.deltaTime;
            rb.AddForce(brakeForceVector);
        }
        
        // Limit speed
        float currentMaxSpeed = nitrousActive ? maxSpeed * nitrousBoost : maxSpeed;
        if (rb.linearVelocity.magnitude > currentMaxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentMaxSpeed;
        }
        
        // Turning
        float currentTurnSpeed = turnSpeed;
        if (rb.linearVelocity.magnitude < 5f)
            currentTurnSpeed *= 0.5f;
        
        float turnAmount = turn * currentTurnSpeed * Time.deltaTime;
        transform.Rotate(0, turnAmount, 0);
        
        currentSpeed = rb.linearVelocity.magnitude;
    }
    
    void HandleWeapons()
    {
        if (!isPlayerInside) return;
        
        // Machine Gun (Left Mouse)
        if (hasMachineGun && Input.GetMouseButton(0) && machineGunAmmo > 0)
        {
            if (Time.time - lastMachineGunTime >= machineGunFireRate)
            {
                ShootMachineGun();
            }
        }
        
        // Rocket Launcher (Right Mouse)
        if (hasRocketLauncher && Input.GetMouseButtonDown(1) && rocketAmmo > 0)
        {
            if (Time.time - lastRocketTime >= rocketFireRate)
            {
                ShootRocket();
            }
        }
        
        // Drop Mine (Q key)
        if (hasMines && Input.GetKeyDown(KeyCode.Q) && mineAmmo > 0)
        {
            if (Time.time - lastMineTime >= mineDropCooldown)
            {
                DropMine();
            }
        }
        
        // Ram Attack (F key)
        if (Input.GetKeyDown(KeyCode.F) && Time.time - lastRamTime >= ramCooldown)
        {
            StartCoroutine(RamAttack());
        }
        
        // Shield (E key)
        if (hasShield && Input.GetKeyDown(KeyCode.E) && !shieldActive && currentShieldHealth > 0)
        {
            ActivateShield();
        }
        
        // Weapon Switch (1, 2, 3 keys)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (UIManager.Instance) UIManager.Instance.UpdateCurrentWeapon("Machine Gun");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (UIManager.Instance) UIManager.Instance.UpdateCurrentWeapon("Rocket");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (UIManager.Instance) UIManager.Instance.UpdateCurrentWeapon("Mine");
        }
    }
    
    void HandleEnterExit()
    {
        // Enter vehicle (E key when near)
        if (Input.GetKeyDown(KeyCode.E) && !isPlayerInside)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player && Vector3.Distance(transform.position, player.transform.position) < 3f)
            {
                EnterVehicle(player);
            }
        }
        
        // Exit vehicle (F key when inside)
        if (Input.GetKeyDown(KeyCode.F) && isPlayerInside)
        {
            ExitVehicle();
        }
    }
    
    void EnterVehicle(GameObject player)
    {
        isPlayerInside = true;
        player.SetActive(false);
        transform.parent = null;
        
        // Show vehicle UI
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowVehicleUI(true);
            UIManager.Instance.ShowNotification("Entered Vehicle - E to exit");
        }
        
        Debug.Log("Player entered vehicle!");
    }
    
    void ExitVehicle()
    {
        isPlayerInside = false;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            player.transform.position = transform.position + transform.right * 2f;
            player.SetActive(true);
        }
        
        // Hide vehicle UI
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowVehicleUI(false);
            UIManager.Instance.ShowNotification("Exited Vehicle");
        }
        
        Debug.Log("Player exited vehicle!");
    }
    
    void ShootMachineGun()
    {
        lastMachineGunTime = Time.time;
        machineGunAmmo--;
        
        // Muzzle flash effect
        if (muzzleFlash)
        {
            GameObject flash = Instantiate(muzzleFlash, machineGunPoint.transform.position, machineGunPoint.transform.rotation);
            Destroy(flash, 0.1f);
        }
        
        // Play sound
        if (machineGunSound && audioSource)
        {
            audioSource.PlayOneShot(machineGunSound, 0.5f);
        }
        
        // Raycast for hit
        Ray ray = new Ray(machineGunPoint.transform.position, machineGunPoint.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, machineGunRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.TakeDamage(machineGunDamage);
                    Debug.Log($"Machine Gun hit enemy for {machineGunDamage} damage!");
                }
            }
            
            // Hit effect
            if (hitEffect)
            {
                GameObject effect = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(effect, 0.2f);
            }
        }
        
        Debug.Log($"Machine Gun fired! Ammo: {machineGunAmmo}/{maxMachineGunAmmo}");
        
        // Update UI
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateMachineGunAmmo(machineGunAmmo, maxMachineGunAmmo);
        }
    }
    
    void ShootRocket()
    {
        lastRocketTime = Time.time;
        rocketAmmo--;
        
        // Play sound
        if (rocketSound && audioSource)
        {
            audioSource.PlayOneShot(rocketSound, 0.8f);
        }
        
        // Spawn rocket
        if (rocketPrefab && rocketPoint)
        {
            GameObject rocket = Instantiate(rocketPrefab, rocketPoint.transform.position, rocketPoint.transform.rotation);
            
            RocketProjectile rocketScript = rocket.GetComponent<RocketProjectile>();
            if (rocketScript)
            {
                rocketScript.Initialize(rocketDamage, rocketExplosionRadius, gameObject);
            }
            else
            {
                // Simple rocket with Rigidbody
                Rigidbody rocketRb = rocket.GetComponent<Rigidbody>();
                if (rocketRb)
                {
                    rocketRb.linearVelocity = rocketPoint.transform.forward * 30f;
                }
                
                // Auto destroy after time
                Destroy(rocket, 5f);
            }
        }
        
        Debug.Log($"Rocket fired! Ammo: {rocketAmmo}/{maxRocketAmmo}");
        
        // Update UI
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateRocketAmmo(rocketAmmo, maxRocketAmmo);
        }
    }
    
    void DropMine()
    {
        lastMineTime = Time.time;
        mineAmmo--;
        
        // Play sound
        if (mineSound && audioSource)
        {
            audioSource.PlayOneShot(mineSound, 0.6f);
        }
        
        // Spawn mine behind vehicle
        if (minePrefab)
        {
            Vector3 minePosition = transform.position - transform.forward * 2f;
            GameObject mine = Instantiate(minePrefab, minePosition, Quaternion.identity);
            
            Mine mineScript = mine.GetComponent<Mine>();
            if (mineScript)
            {
                mineScript.Initialize(mineDamage, gameObject);
            }
        }
        
        Debug.Log($"Mine dropped! Ammo: {mineAmmo}/{maxMineAmmo}");
        
        // Update UI
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateMineAmmo(mineAmmo, maxMineAmmo);
        }
    }
    
    IEnumerator RamAttack()
    {
        isRamming = true;
        lastRamTime = Time.time;
        
        // Boost forward
        Vector3 ramForceVector = transform.forward * ramForce;
        rb.AddForce(ramForceVector, ForceMode.Impulse);
        
        // Damage enemies in front
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * 3f, 2f);
        foreach (Collider hit in hitEnemies)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.TakeDamage(ramDamage);
                    Debug.Log($"Vehicle rammed enemy for {ramDamage} damage!");
                    
                    // Knockback enemy
                    Rigidbody enemyRb = hit.GetComponent<Rigidbody>();
                    if (enemyRb)
                    {
                        enemyRb.AddForce(transform.forward * 500f, ForceMode.Impulse);
                    }
                }
            }
        }
        
        // Visual effect
        StartCoroutine(CameraShake(0.2f, 0.2f));
        
        // Show notification
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowNotification("RAM ATTACK!");
        }
        
        yield return new WaitForSeconds(0.3f);
        isRamming = false;
    }
    
    void ActivateShield()
    {
        shieldActive = true;
        if (shieldVisual)
            shieldVisual.SetActive(true);
        
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowNotification("SHIELD ACTIVATED!");
        }
        
        Debug.Log("Shield activated!");
    }
    
    void UpdateShield()
    {
        if (shieldActive)
        {
            // Regenerate shield when not taking damage
            if (currentShieldHealth < maxShieldHealth)
            {
                currentShieldHealth += shieldRegenRate * Time.deltaTime;
                if (currentShieldHealth > maxShieldHealth)
                    currentShieldHealth = maxShieldHealth;
            }
            
            // Update UI shield
            if (UIManager.Instance)
            {
                UIManager.Instance.UpdateShield(currentShieldHealth, maxShieldHealth);
            }
            
            // Deactivate if depleted
            if (currentShieldHealth <= 0)
            {
                shieldActive = false;
                if (shieldVisual)
                    shieldVisual.SetActive(false);
                
                if (UIManager.Instance)
                {
                    UIManager.Instance.ShowNotification("SHIELD DEPLETED!");
                }
                
                Debug.Log("Shield depleted!");
            }
        }
    }
    
    void HandleNitrous()
    {
        // Nitrous Boost (Left Shift)
        if (hasNitrous && Input.GetKeyDown(KeyCode.LeftShift) && nitrousReady && !nitrousActive && isPlayerInside)
        {
            StartCoroutine(ActivateNitrous());
        }
        
        // Update nitrous cooldown
        if (!nitrousReady)
        {
            nitrousTimer -= Time.deltaTime;
            if (nitrousTimer <= 0f)
            {
                nitrousReady = true;
                if (UIManager.Instance)
                {
                    UIManager.Instance.ShowNotification("NITROUS READY!");
                }
                Debug.Log("Nitrous ready!");
            }
        }
    }
    
    IEnumerator ActivateNitrous()
    {
        nitrousActive = true;
        nitrousReady = false;
        nitrousTimer = nitrousCooldown;
        
        // Play sound
        if (nitrousSound && audioSource)
        {
            audioSource.PlayOneShot(nitrousSound);
        }
        
        // Visual effect
        StartCoroutine(CameraShake(0.1f, 0.15f));
        
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowNotification("NITROUS BOOST!");
        }
        
        Debug.Log("NITROUS BOOST ACTIVATED!");
        
        yield return new WaitForSeconds(nitrousDuration);
        
        nitrousActive = false;
        
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowNotification("NITROUS DEPLETED");
        }
        
        Debug.Log("Nitrous boost ended");
    }
    
    IEnumerator CameraShake(float duration, float magnitude)
    {
        if (!playerCamera) yield break;
        
        Vector3 originalPos = playerCamera.transform.localPosition;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-magnitude, magnitude);
            float y = Random.Range(-magnitude, magnitude);
            playerCamera.transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        playerCamera.transform.localPosition = originalPos;
    }
    
    void UpdateUI()
    {
        if (UIManager.Instance && isPlayerInside)
        {
            UIManager.Instance.UpdateVehicleHealth(currentHealth, maxHealth);
            UIManager.Instance.UpdateSpeed(currentSpeed);
            UIManager.Instance.UpdateShield(currentShieldHealth, maxShieldHealth);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (isDestroyed) return;
        
        // Ram damage on collision
        if (collision.gameObject.CompareTag("Enemy") && currentSpeed > 15f)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.TakeDamage(ramDamage);
                Debug.Log($"Vehicle rammed enemy for {ramDamage} damage!");
            }
            
            // Impact force on vehicle
            Vector3 impactForce = -collision.contacts[0].normal * ramForce * 0.5f;
            rb.AddForce(impactForce);
            
            // Show collision notification
            if (UIManager.Instance)
            {
                UIManager.Instance.ShowNotification("CRASH!");
            }
        }
    }
    
    public void TakeDamage(float amount, GameObject source = null)
    {
        if (isDestroyed) return;
        
        // Shield absorbs damage first
        if (shieldActive && currentShieldHealth > 0)
        {
            float damageToShield = Mathf.Min(amount, currentShieldHealth);
            currentShieldHealth -= damageToShield;
            amount -= damageToShield;
            
            // Shield hit effect
            if (shieldHitEffect)
            {
                Instantiate(shieldHitEffect, transform.position, Quaternion.identity);
            }
            
            Debug.Log($"Shield took {damageToShield} damage! Shield: {currentShieldHealth}/{maxShieldHealth}");
        }
        
        // Remaining damage goes to vehicle health
        if (amount > 0)
        {
            currentHealth -= amount;
            Debug.Log($"Vehicle took {amount} damage! Health: {currentHealth}/{maxHealth}");
            
            // Hit effect
            if (hitEffect)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            
            // Play hit sound
            if (hitSound && audioSource)
            {
                audioSource.PlayOneShot(hitSound);
            }
            
            // Update UI
            if (UIManager.Instance)
            {
                UIManager.Instance.UpdateVehicleHealth(currentHealth, maxHealth);
                UIManager.Instance.ShowDamageFlash();
            }
        }
        
        if (currentHealth <= 0)
        {
            DestroyVehicle();
        }
    }
    
    void DestroyVehicle()
    {
        isDestroyed = true;
        Debug.Log("Vehicle destroyed!");
        
        // Explosion effect
        if (explosionEffect)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        
        // Play explosion sound
        if (explosionSound && audioSource)
        {
            audioSource.PlayOneShot(explosionSound);
        }
        
        // Camera shake
        StartCoroutine(CameraShake(0.5f, 0.5f));
        
        // Show notification
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowNotification("VEHICLE DESTROYED!");
            UIManager.Instance.ShowVehicleUI(false);
        }
        
        // Eject player
        if (isPlayerInside)
        {
            ExitVehicle();
        }
        
        Destroy(gameObject, 0.5f);
    }
    
    public void AddAmmo(string ammoType, int amount)
    {
        switch (ammoType.ToLower())
        {
            case "machinegun":
                machineGunAmmo = Mathf.Min(machineGunAmmo + amount, maxMachineGunAmmo);
                if (UIManager.Instance) UIManager.Instance.UpdateMachineGunAmmo(machineGunAmmo, maxMachineGunAmmo);
                break;
            case "rocket":
                rocketAmmo = Mathf.Min(rocketAmmo + amount, maxRocketAmmo);
                if (UIManager.Instance) UIManager.Instance.UpdateRocketAmmo(rocketAmmo, maxRocketAmmo);
                break;
            case "mine":
                mineAmmo = Mathf.Min(mineAmmo + amount, maxMineAmmo);
                if (UIManager.Instance) UIManager.Instance.UpdateMineAmmo(mineAmmo, maxMineAmmo);
                break;
        }
        
        if (UIManager.Instance)
        {
            UIManager.Instance.ShowNotification($"+{amount} {ammoType} Ammo!");
        }
        
        Debug.Log($"Added {amount} {ammoType} ammo!");
    }
    
    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
    
    public float GetShieldPercent()
    {
        if (!hasShield) return 0f;
        return currentShieldHealth / maxShieldHealth;
    }
    
    public bool IsPlayerInside()
    {
        return isPlayerInside;
    }
}