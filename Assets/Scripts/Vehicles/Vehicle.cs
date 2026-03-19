using UnityEngine;

/// <summary>
/// Vehicle configuration with stats for different vehicle types
/// </summary>
[System.Serializable]
public class VehicleStats
{
    public VehicleType type;
    public string displayName;
    public float maxSpeed;
    public float acceleration;
    public float turnSpeed;
    public float weight;
    public int capacity; // Number of passengers
    public float brakePower;
    public Color vehicleColor;
    public float maxHealth;
    public bool canDriveOffRoad;

    public VehicleStats(VehicleType type, string displayName, float maxSpeed, float acceleration, 
        float turnSpeed, float weight, int capacity, float brakePower, Color color, float health, bool offRoad = false)
    {
        this.type = type;
        this.displayName = displayName;
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.turnSpeed = turnSpeed;
        this.weight = weight;
        this.capacity = capacity;
        this.brakePower = brakePower;
        this.vehicleColor = color;
        this.maxHealth = health;
        this.canDriveOffRoad = offRoad;
    }
}

public enum VehicleType
{
    // Trains/Metro (highest health)
    Train,
    Metro,
    
    // Trucks/Buses (second highest)
    SlowTruck,
    Bus,
    
    // Vans/Ambulances (third)
    SlowVan,
    MediumVan,
    FastVan,
    Ambulance,
    
    // Cars (fourth)
    CompactCar,
    EconomyCar,
    TaxiCab,
    ClassicCoupe,
    SedanCar,
    LuxurySedan,
    MuscleCar,
    SportsCar,
    PoliceCruiser,
    SuperCar,
    
    // New car types
    Hatchback,
    SUV,
    PickupTruck,
    OffroadVehicle,
    Van,
    Convertible,
    ArmoredCar,
    RallyCar,
    DriftCar,
    HotRod,
    
    // Motorcycle types (lowest)
    Motorcycle,
    SportBike,
    Scooter
}

/// <summary>
/// enhanced Vehicle class with stats-based system
/// </summary>
public class Vehicle : MonoBehaviour, IStealable
{
    [SerializeField] private VehicleStats vehicleStats;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public string Id { get; private set; }
    public VehicleType VehicleType => vehicleStats.type;
    public string DisplayName => vehicleStats.displayName;
    public bool IsOccupied { get; private set; } = false;
    public bool IsStolen { get; set; } = false;
    public float CurrentSpeed { get; private set; } = 0f;
    public float MaxHealth => vehicleStats.maxHealth;
    public float CurrentHealth { get; private set; }
    public bool CanDriveOffRoad => vehicleStats.canDriveOffRoad;
    public bool IsDestroyed { get; private set; } = false;
    public bool IsAirborne { get; private set; } = false;
    public float AirborneHeight { get; private set; } = 0f;

    private PlayerController driver;
    private Vector2 currentVelocity = Vector2.zero;
    private float currentRotation = 0f;
    private int damageLevel = 0;
    private VehicleDriver vehicleDriver;
    private bool hasDriver = true;
    private CarLights carLights;
    private VehicleSoundSystem soundSystem;
    
    private float collisionCooldown = 0f;
    private SpriteRenderer[] allRenderers;
    private Color originalColor;
    
    private GameObject smokeEffect;
    private GameObject fireEffect;
    private bool isSmoking = false;
    private bool isOnFire = false;
    private float smokeTimer = 0f;
    private float fireTimer = 0f;
    private float maxSmokeDuration = 5f;
    private float maxFireDuration = 8f;

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        allRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalColor = vehicleStats.vehicleColor;

        carLights = GetComponent<CarLights>();
        soundSystem = GetComponent<VehicleSoundSystem>();

        Id = System.Guid.NewGuid().ToString();
        CurrentHealth = vehicleStats.maxHealth;

        if (spriteRenderer != null)
            spriteRenderer.color = vehicleStats.vehicleColor;

        SetupCollisions();
        InitializeDriver();
    }
    
    private void Update()
    {
        if (collisionCooldown > 0)
        {
            collisionCooldown -= Time.deltaTime;
        }
        
        UpdateDamageEffects();
    }

    private void UpdateDamageEffects()
    {
        float healthPercent = CurrentHealth / MaxHealth;
        
        if (healthPercent <= 0.25f && !isSmoking)
        {
            StartSmokeEffect();
        }
        
        if (healthPercent <= 0.10f && !isOnFire)
        {
            StartFireEffect();
        }
        
        if (isSmoking)
        {
            smokeTimer += Time.deltaTime;
            if (smokeTimer > maxSmokeDuration)
            {
                StopSmokeEffect();
            }
        }
        
        if (isOnFire)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer > maxFireDuration)
            {
                StopFireEffect();
            }
        }
    }
    
    private void StartSmokeEffect()
    {
        if (smokeEffect != null) return;
        
        isSmoking = true;
        smokeTimer = 0f;
        
        smokeEffect = new GameObject("SmokeEffect");
        smokeEffect.transform.position = transform.position + Vector3.up * 2f;
        smokeEffect.transform.parent = transform;
        
        ParticleSystem particles = smokeEffect.AddComponent<ParticleSystem>();
        if (particles != null)
        {
            var main = particles.main;
            main.startColor = new Color(0.4f, 0.4f, 0.4f, 0.6f);
            main.startSize = 1f;
            main.startSpeed = 2f;
            main.startLifetime = 2f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.5f;
            
            var emission = particles.emission;
            emission.rateOverTime = 15f;
            
            var colorOverLifetime = particles.colorOverLifetime;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(0.3f, 0.3f, 0.3f), 0f), new GradientColorKey(new Color(0.2f, 0.2f, 0.2f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0.6f, 0f), new GradientAlphaKey(0.3f, 0.5f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = gradient;
        }
        
        if (soundSystem != null)
        {
            soundSystem.SetDamagedEngine(true);
        }
        
        Debug.Log($"{DisplayName} started smoking!");
    }
    
    private void StopSmokeEffect()
    {
        if (smokeEffect != null)
        {
            ParticleSystem ps = smokeEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var emission = ps.emission;
                emission.rateOverTime = 0;
            }
            
            Destroy(smokeEffect, 1f);
            smokeEffect = null;
        }
        isSmoking = false;
    }
    
    private void StartFireEffect()
    {
        if (fireEffect != null) return;
        
        isOnFire = true;
        fireTimer = 0f;
        
        fireEffect = new GameObject("FireEffect");
        fireEffect.transform.position = transform.position + Vector3.up * 1.5f;
        fireEffect.transform.parent = transform;
        
        ParticleSystem particles = fireEffect.AddComponent<ParticleSystem>();
        if (particles != null)
        {
            var main = particles.main;
            main.startColor = new Color(1f, 0.4f, 0f, 0.9f);
            main.startSize = 0.8f;
            main.startSpeed = 3f;
            main.startLifetime = 1.5f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.3f;
            
            var emission = particles.emission;
            emission.rateOverTime = 20f;
            
            var colorOverLifetime = particles.colorOverLifetime;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(new Color(1f, 0.8f, 0.2f), 0f), 
                    new GradientColorKey(new Color(1f, 0.3f, 0f), 0.4f),
                    new GradientColorKey(new Color(0.2f, 0.2f, 0.2f), 1f) 
                },
                new GradientAlphaKey[] { new GradientAlphaKey(0.9f, 0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = gradient;
        }
        
        Debug.Log($"{DisplayName} is on fire!");
    }
    
    private void StopFireEffect()
    {
        if (fireEffect != null)
        {
            ParticleSystem ps = fireEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var emission = ps.emission;
                emission.rateOverTime = 0;
            }
            
            Destroy(fireEffect, 0.5f);
            fireEffect = null;
        }
        isOnFire = false;
    }

    private void SetupCollisions()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.isTrigger = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsDestroyed) return;

        if (IsBuildingCollision(collision.gameObject))
        {
            HandleBuildingCollision(collision);
            return;
        }

        if (collisionCooldown > 0) return;
        
        float impactSpeed = collision.relativeVelocity.magnitude;
        
        if (impactSpeed < 5f) return;
        
        collisionCooldown = 0.5f;
        
        float damage = CalculateCollisionDamage(impactSpeed);
        
        ApplyDamage(damage);
        
        ApplyVisualDamage();
        
        if (impactSpeed > 20f && driver != null)
        {
            float driverDamage = impactSpeed * 0.5f;
            if (driver is PlayerManager pm)
            {
                pm.TakeDamage(driverDamage * 0.1f);
            }
        }
    }

    private bool IsBuildingCollision(GameObject obj)
    {
        if (obj == null) return false;

        if (obj.layer == LayerMask.NameToLayer("Buildings")) return true;

        if (BuildingCollisionManager.Instance != null && BuildingCollisionManager.Instance.IsBuilding(obj)) return true;

        string objName = obj.name.ToLower();
        if (objName.Contains("building") ||
            objName.Contains("shop") ||
            objName.Contains("house") ||
            objName.Contains("restaurant") ||
            objName.Contains("garage") ||
            objName.Contains("hospital") ||
            objName.Contains("police") ||
            objName.Contains("church") ||
            objName.Contains("station") ||
            objName.Contains("collision"))
        {
            return true;
        }

        return false;
    }

    private void HandleBuildingCollision(Collision2D collision)
    {
        float impactSpeed = collision.relativeVelocity.magnitude;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflectDir = Vector2.Reflect(rb.velocity.normalized, normal);
            rb.velocity = reflectDir * rb.velocity.magnitude * 0.3f;

            if (impactSpeed > 10f)
            {
                rb.velocity = Vector2.zero;
            }
        }

        if (impactSpeed > 15f && collisionCooldown <= 0)
        {
            collisionCooldown = 0.5f;
            float damage = impactSpeed * 0.5f;
            ApplyDamage(damage);
            ApplyVisualDamage();

            if (driver is PlayerManager pm)
            {
                pm.TakeDamage(impactSpeed * 0.05f);
            }
        }
    }
    
    private float CalculateCollisionDamage(float impactSpeed)
    {
        float baseDamage = impactSpeed * 2f;
        float weightFactor = 1f / (vehicleStats.weight / 1f);
        return baseDamage * weightFactor;
    }
    
    private void ApplyDamage(float damage)
    {
        CurrentHealth -= damage;
        Debug.Log($"{DisplayName} took {damage} damage. Health: {CurrentHealth}/{MaxHealth}");

        if (CurrentHealth <= 0)
        {
            DestroyVehicle();
        }
    }
    
    private void ApplyVisualDamage()
    {
        damageLevel++;
        float damagePercent = 1f - (CurrentHealth / MaxHealth);
        
        foreach (var sr in allRenderers)
        {
            if (sr != null)
            {
                Color damagedColor = Color.Lerp(originalColor, Color.gray, damagePercent);
                sr.color = damagedColor;
                
                if (damagePercent > 0.5f)
                {
                    float smokeAlpha = (damagePercent - 0.5f) * 0.5f;
                    sr.color = new Color(damagedColor.r * 0.7f, damagedColor.g * 0.7f, damagedColor.b * 0.7f, 1f - smokeAlpha * 0.3f);
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsDestroyed) return;
        ApplyDamage(damage);
        ApplyVisualDamage();
    }

    private void DestroyVehicle()
    {
        if (IsDestroyed) return;
        IsDestroyed = true;
        
        StopAllEffects();
        
        Debug.Log($"{DisplayName} has been destroyed!");
        
        if (driver != null)
        {
            driver.ExitVehicle();
        }
        
        CreateDestroyedVehicle();
        
        Destroy(gameObject, 0.1f);
    }
    
    private void StopAllEffects()
    {
        StopSmokeEffect();
        StopFireEffect();
        
        if (soundSystem != null)
        {
            soundSystem.SetDamagedEngine(false);
        }
    }
    
    private void CreateDestroyedVehicle()
    {
        GameObject destroyedGO = new GameObject($"Destroyed_{DisplayName}");
        destroyedGO.transform.position = transform.position;
        destroyedGO.transform.rotation = transform.rotation;
        destroyedGO.transform.localScale = transform.localScale;
        
        DestroyedVehicle dv = destroyedGO.AddComponent<DestroyedVehicle>();
        dv.Initialize(transform.position, new Vector2(transform.localScale.x, transform.localScale.y), 
                      transform.rotation, currentVelocity);
        
        DestroyedVehicleManager.Instance?.RegisterDestroyedVehicle(dv);
        
        TowTruckManager.Instance?.RequestTowTruckFromScrapyard(transform.position);
    }

    public bool CanPlayerDriveOffRoad()
    {
        return vehicleStats.canDriveOffRoad || vehicleStats.type == VehicleType.PoliceCruiser || 
               vehicleStats.type == VehicleType.Ambulance;
    }

    private void InitializeDriver()
    {
        if (vehicleStats.type == VehicleType.PoliceCruiser) return;

        GameObject driverGO = new GameObject("VehicleDriver");
        driverGO.transform.parent = transform;
        driverGO.transform.localPosition = Vector3.zero;
        
        vehicleDriver = driverGO.AddComponent<VehicleDriver>();
        vehicleDriver.InitializeAsDriver(gameObject);
    }

    private void FixedUpdate()
    {
        if (IsOccupied && driver != null)
        {
            HandleDriving();
        }
        else
        {
            currentVelocity *= 0.93f;
        }

        rb.velocity = currentVelocity;
        CurrentSpeed = currentVelocity.magnitude;

        UpdateAirborneState();
    }

    private void UpdateAirborneState()
    {
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            AirborneHeight = transform.position.y;
            
            if (!IsAirborne && rb.velocity.y > 2f)
            {
                IsAirborne = true;
            }
            
            if (IsAirborne && AirborneHeight <= 0.1f)
            {
                HandleLanding();
            }
        }
    }

    private void HandleLanding()
    {
        if (!IsAirborne) return;
        
        IsAirborne = false;
        
        Vector2 landPos = FindLandablePosition(transform.position);
        
        float distanceToOriginal = Vector2.Distance(landPos, (Vector2)transform.position);
        if (distanceToOriginal > 0.5f)
        {
            transform.position = landPos;
            currentVelocity = new Vector2(currentVelocity.x * 0.5f, 0);
            rb.velocity = currentVelocity;
        }
        else
        {
            currentVelocity = new Vector2(currentVelocity.x * 0.3f, 0);
            rb.velocity = currentVelocity;
        }
        
        float fallDamage = CalculateFallDamage();
        if (fallDamage > 0)
        {
            TakeDamage(fallDamage, "Fall");
        }
    }

    private float CalculateFallDamage()
    {
        float speed = currentVelocity.magnitude;
        if (speed < 5f) return 0f;
        if (speed < 10f) return 5f;
        if (speed < 15f) return 15f;
        if (speed < 20f) return 30f;
        return 50f;
    }

    private Vector2 FindLandablePosition(Vector2 fromPosition)
    {
        Vector2[] directions = new Vector2[] 
        {
            Vector2.right,
            Vector2.left,
            Vector2.up,
            Vector2.down,
            new Vector2(1, 1).normalized,
            new Vector2(-1, 1).normalized,
            new Vector2(1, -1).normalized,
            new Vector2(-1, -1).normalized
        };

        float checkDistance = 5f;
        float checkStep = 0.5f;

        foreach (Vector2 dir in directions)
        {
            for (float d = 0; d < checkDistance; d += checkStep)
            {
                Vector2 testPos = fromPosition + dir * d;
                if (!Physics2D.OverlapCircle(testPos, 1f, LayerMask.GetMask("Buildings")))
                {
                    return testPos;
                }
            }
        }

        return fromPosition + Vector2.down * 5f;
    }

    public void LaunchFromRamp(Vector2 launchVelocity)
    {
        currentVelocity = launchVelocity;
        rb.velocity = launchVelocity;
        IsAirborne = true;
        AirborneHeight = transform.position.y;
    }

    private void HandleDriving()
    {
        Vector2 input = driver.GetInputDirection();
        
        float healthPercent = CurrentHealth / MaxHealth;
        float speedMultiplier = healthPercent <= 0.10f ? 0.75f : 1f;

        if (input.magnitude > 0)
        {
            float effectiveAcceleration = vehicleStats.acceleration / (vehicleStats.weight / 1f);
            
            Vector2 moveDirection = new Vector2(Mathf.Cos(currentRotation * Mathf.Deg2Rad), 
                                                Mathf.Sin(currentRotation * Mathf.Deg2Rad));
            
            float targetSpeed = vehicleStats.maxSpeed * input.magnitude * speedMultiplier;
            currentVelocity = Vector2.Lerp(currentVelocity, moveDirection * targetSpeed, 
                effectiveAcceleration * Time.fixedDeltaTime);

            float speedFactor = Mathf.Clamp01(CurrentSpeed / vehicleStats.maxSpeed);
            float effectiveTurnSpeed = vehicleStats.turnSpeed / (1f + speedFactor);
            
            float targetRotation = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg - 90f;
            currentRotation = Mathf.LerpAngle(currentRotation, targetRotation, effectiveTurnSpeed * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocity *= (1f - vehicleStats.brakePower * Time.fixedDeltaTime);
        }

        transform.rotation = Quaternion.AngleAxis(currentRotation, Vector3.forward);
    }

    public void SetDriver(PlayerController newDriver)
    {
        driver = newDriver;
        IsOccupied = true;
    }

    public void RemoveDriver()
    {
        driver = null;
        IsOccupied = false;
        currentVelocity *= 0.5f;
    }

    public void HonkHorn()
    {
        if (soundSystem != null)
        {
            soundSystem.HonkHorn(false);
        }
    }

    public void ToggleCarLights()
    {
        if (carLights != null)
        {
            carLights.ToggleLights();
        }
    }

    public bool AreLightsOn()
    {
        return carLights != null && carLights.AreLightsOn();
    }

    public bool Steal()
    {
        if (vehicleDriver != null && vehicleDriver.IsFighting())
        {
            Debug.Log("Cannot steal - driver is fighting back!");
            return false;
        }

        IsStolen = true;
        hasDriver = false;
        
        if (vehicleDriver != null)
        {
            vehicleDriver.OnAttemptedTheft(GameObject.FindGameObjectWithTag("Player"));
            
            if (vehicleDriver.IsFighting())
            {
                GameManager.Instance.UpdateWantedLevel(2);
                Debug.Log($"Theft attempt triggered fight! Wanted level increased.");
                return false;
            }
        }

        GameManager.Instance.UpdateWantedLevel(1);
        Debug.Log($"{DisplayName} has been stolen!");
        return true;
    }

    public string GetStealableInfo()
    {
        return $"{DisplayName} - {VehicleType}";
    }

    public Vector2 GetInputDirection() => driver != null ? driver.GetInputDirection() : Vector2.zero;

    public void SetVehicleStats(VehicleStats stats)
    {
        vehicleStats = stats;
        if (spriteRenderer != null)
            spriteRenderer.color = stats.vehicleColor;
    }

    public Vector2 GetVelocity() => currentVelocity;
    public Vector2 Velocity { get { return currentVelocity; } set { currentVelocity = value; } }
    public Rigidbody2D Rb => rb;
    
    private void OnDestroy()
    {
        StopAllEffects();
    }
}

public interface IStealable
{
        StopAllEffects();
    }
}
    
    private void Update()
    {
        if (collisionCooldown > 0)
        {
            collisionCooldown -= Time.deltaTime;
        }
    }

    private void SetupCollisions()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.isTrigger = false;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsDestroyed || collisionCooldown > 0) return;
        
        float impactSpeed = collision.relativeVelocity.magnitude;
        
        if (impactSpeed < 5f) return;
        
        collisionCooldown = 0.5f;
        
        float damage = CalculateCollisionDamage(impactSpeed);
        
        ApplyDamage(damage);
        
        ApplyVisualDamage();
        
        if (impactSpeed > 20f && driver != null)
        {
            float driverDamage = impactSpeed * 0.5f;
            if (driver is PlayerManager pm)
            {
                pm.TakeDamage(driverDamage * 0.1f);
            }
        }
    }
    
    private float CalculateCollisionDamage(float impactSpeed)
    {
        float baseDamage = impactSpeed * 2f;
        float weightFactor = 1f / (vehicleStats.weight / 1f);
        return baseDamage * weightFactor;
    }
    
    private void ApplyDamage(float damage)
    {
        CurrentHealth -= damage;
        Debug.Log($"{DisplayName} took {damage} damage. Health: {CurrentHealth}/{MaxHealth}");

        if (CurrentHealth <= 0)
        {
            DestroyVehicle();
        }
    }
    
    private void ApplyVisualDamage()
    {
        damageLevel++;
        float damagePercent = 1f - (CurrentHealth / MaxHealth);
        
        foreach (var sr in allRenderers)
        {
            if (sr != null)
            {
                Color damagedColor = Color.Lerp(originalColor, Color.gray, damagePercent);
                sr.color = damagedColor;
                
                if (damagePercent > 0.5f)
                {
                    float smokeAlpha = (damagePercent - 0.5f) * 0.5f;
                    sr.color = new Color(damagedColor.r * 0.7f, damagedColor.g * 0.7f, damagedColor.b * 0.7f, 1f - smokeAlpha * 0.3f);
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsDestroyed) return;
        ApplyDamage(damage);
        ApplyVisualDamage();
    }

    private void DestroyVehicle()
    {
        if (IsDestroyed) return;
        IsDestroyed = true;
        
        Debug.Log($"{DisplayName} has been destroyed!");
        
        if (driver != null)
        {
            driver.ExitVehicle();
        }
        
        CreateDestroyedVehicle();
        
        Destroy(gameObject, 0.1f);
    }
    
    private void CreateDestroyedVehicle()
    {
        GameObject destroyedGO = new GameObject($"Destroyed_{DisplayName}");
        destroyedGO.transform.position = transform.position;
        destroyedGO.transform.rotation = transform.rotation;
        destroyedGO.transform.localScale = transform.localScale;
        
        DestroyedVehicle dv = destroyedGO.AddComponent<DestroyedVehicle>();
        dv.Initialize(transform.position, new Vector2(transform.localScale.x, transform.localScale.y), 
                      transform.rotation, currentVelocity);
        
        DestroyedVehicleManager.Instance?.RegisterDestroyedVehicle(dv);
        
        TowTruckManager.Instance?.RequestTowTruckFromScrapyard(transform.position);
    }

    public bool CanPlayerDriveOffRoad()
    {
        return vehicleStats.canDriveOffRoad || vehicleStats.type == VehicleType.PoliceCruiser || 
               vehicleStats.type == VehicleType.Ambulance;
    }

    private void InitializeDriver()
    {
        if (vehicleStats.type == VehicleType.PoliceCruiser) return;

        GameObject driverGO = new GameObject("VehicleDriver");
        driverGO.transform.parent = transform;
        driverGO.transform.localPosition = Vector3.zero;
        
        vehicleDriver = driverGO.AddComponent<VehicleDriver>();
        vehicleDriver.InitializeAsDriver(gameObject);
    }

    public void SetDriver(PlayerController newDriver)
    {
        driver = newDriver;
        IsOccupied = true;
    }

    public void RemoveDriver()
    {
        driver = null;
        IsOccupied = false;
        currentVelocity *= 0.5f; // Vehicle slows when driver exits
    }

    public void HonkHorn()
    {
        if (soundSystem != null)
        {
            soundSystem.HonkHorn(false);
        }
    }

    public void ToggleCarLights()
    {
        if (carLights != null)
        {
            carLights.ToggleLights();
        }
    }

    public bool AreLightsOn()
    {
        return carLights != null && carLights.AreLightsOn();
    }

    public void TakeDamage(float damage)
    {
        damageLevel++;
        float damagePercentage = (damageLevel / 5f) * 100f;
        
        if (damagePercentage > 50f)
        {
            // Increased wear and tear
            if (spriteRenderer != null)
                spriteRenderer.color = Color.Lerp(vehicleStats.vehicleColor, Color.gray, damagePercentage / 100f);
        }

        if (damageLevel >= 5)
        {
            VehicleDestroyed();
        }
    }

    public bool Steal()
    {
        if (vehicleDriver != null && vehicleDriver.IsFighting())
        {
            Debug.Log("Cannot steal - driver is fighting back!");
            return false;
        }

        IsStolen = true;
        hasDriver = false;
        
        if (vehicleDriver != null)
        {
            vehicleDriver.OnAttemptedTheft(GameObject.FindGameObjectWithTag("Player"));
            
            if (vehicleDriver.IsFighting())
            {
                GameManager.Instance.UpdateWantedLevel(2);
                Debug.Log($"Theft attempt triggered fight! Wanted level increased.");
                return false;
            }
        }

        GameManager.Instance.UpdateWantedLevel(1);
        Debug.Log($"{DisplayName} has been stolen!");
        return true;
    }

    public string GetStealableInfo()
    {
        return $"{DisplayName} - {VehicleType}";
    }

    private void VehicleDestroyed()
    {
        if (IsOccupied)
            RemoveDriver();
        
        Destroy(gameObject);
    }

    public Vector2 GetInputDirection() => driver != null ? driver.GetInputDirection() : Vector2.zero;

    // For testing - set vehicle stats manually
    public void SetVehicleStats(VehicleStats stats)
    {
        vehicleStats = stats;
        if (spriteRenderer != null)
            spriteRenderer.color = stats.vehicleColor;
    }

    public VehicleStats GetStats() => vehicleStats;
}

public interface IStealable
{
    void Steal();
    string GetStealableInfo();
}
