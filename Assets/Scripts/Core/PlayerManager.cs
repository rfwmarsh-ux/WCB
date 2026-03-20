using UnityEngine;

/// <summary>
/// Manages the player character
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public int PlayerId { get; private set; } = 1;

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private float currentArmour = 0f;
    [SerializeField] private float maxArmour = 100f;
    private Gun currentGun;
    private LaserGun laserGun;
    private object currentMeleeWeapon;
    private int currentAmmo = 0;
    private bool isAlive = true;
    private Vector2 lastPosition;

    public bool IsInVehicle { get; set; } = false;
    public Vehicle currentVehicle { get; set; } = null;

    [Header("Money Settings")]
    [SerializeField] private float startingMoney = 1000f;
    private float currentMoney;

    public float CurrentMoney => currentMoney;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentMoney = startingMoney;
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (isAlive)
        {
            lastPosition = transform.position;
            CheckWaterDeath();
        }
    }

    private void CheckWaterDeath()
    {
        if (WaterDeathHandler.Instance != null)
        {
            WaterDeathHandler.Instance.CheckWaterDeath(transform.position, PlayerId);
        }
    }
    
    public void EnterVehicle(Vehicle vehicle)
    {
        if (vehicle == null) return;
        
        currentVehicle = vehicle;
        IsInVehicle = true;
        
        Debug.Log($"Entered vehicle: {vehicle.DisplayName}");
    }
    
    public void ExitVehicle()
    {
        if (currentVehicle == null) return;
        
        currentVehicle = null;
        IsInVehicle = false;
        
        Debug.Log("Exited vehicle");
    }

    public void TakeDamage(float damageAmount)
    {
        if (!isAlive) return;

        // Armour absorbs full damage first, health only takes damage after armour is depleted
        float armourAbsorbed = Mathf.Min(currentArmour, damageAmount);
        float healthDamage = damageAmount - armourAbsorbed;

        currentArmour -= armourAbsorbed;
        currentHealth -= healthDamage;

        Debug.Log($"Player took {damageAmount} damage. Armour absorbed: {armourAbsorbed}. Health: {currentHealth}/{maxHealth}, Armour: {currentArmour}/{maxArmour}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (!isAlive) return;

        isAlive = false;
        currentHealth = 0;
        Debug.Log("Player died!");

        // Disable player movement/control
        GetComponent<Collider2D>().enabled = false;
        if (GetComponent<Rigidbody2D>() != null)
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // Trigger respawn after short delay
        Invoke("TriggerRespawn", 1f);
    }

    private void TriggerRespawn()
    {
        VeterinaryCentreManager centreManager = GameManager.Instance.GetVeterinaryCentreManager();
        if (centreManager != null)
        {
            centreManager.RespawnPlayerAtClosestCentre(this, lastPosition, PlayerId);
        }
        else
        {
            Debug.LogError("VeterinaryCentreManager not found!");
        }
    }

    public void Respawn(Vector2 respawnPosition)
    {
        // Reset health and armour
        currentHealth = maxHealth;
        currentArmour = 0f;
        isAlive = true;

        // Move to respawn location
        transform.position = respawnPosition;
        lastPosition = respawnPosition;

        // Clear wanted level on respawn
        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.ClearWantedLevel(PlayerId);
            Debug.Log($"Player {PlayerId} wanted level cleared on respawn");
        }

        // Re-enable controls
        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = true;

        Debug.Log($"Player respawned at {respawnPosition}");
    }

    public void RespawnPlayer()
    {
        Vector2 safePosition = FindSafeRespawnPosition();
        Respawn(safePosition);
        
        if (currentVehicle != null)
        {
            Vector2 vehicleSafePos = FindSafeRespawnPosition();
            currentVehicle.transform.position = vehicleSafePos;
            currentVehicle.Velocity = Vector2.zero;
            if (currentVehicle.Rb != null)
            {
                currentVehicle.Rb.velocity = Vector2.zero;
            }
        }
    }

    private Vector2 FindSafeRespawnPosition()
    {
        Vector2[] safeLocations = new Vector2[]
        {
            new Vector2(100, 200),
            new Vector2(200, 200),
            new Vector2(300, 300),
            new Vector2(400, 400),
            new Vector2(500, 500),
            new Vector2(600, 600),
            new Vector2(700, 700),
            new Vector2(100, 800),
            new Vector2(200, 800),
            new Vector2(300, 700),
            new Vector2(400, 600),
            new Vector2(500, 500),
            new Vector2(600, 400),
            new Vector2(700, 300),
            new Vector2(800, 200)
        };

        foreach (Vector2 pos in safeLocations)
        {
            if (!Physics2D.OverlapCircle(pos, 3f, LayerMask.GetMask("Buildings")))
            {
                return pos;
            }
        }

        return new Vector2(500, 500);
    }

    public void GainArmour(float armourAmount)
    {
        currentArmour = Mathf.Min(currentArmour + armourAmount, maxArmour);
        Debug.Log($"Gained {armourAmount} armour. Total: {currentArmour}/{maxArmour}");
    }

    public float GetHealth() => currentHealth;
    public float Health => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetArmour() => currentArmour;
    public float GetMaxArmour() => maxArmour;
    public bool IsAlive() => isAlive;
    public Vector2 GetLastPosition() => lastPosition;

    public void HealDamage(float healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        Debug.Log($"Healed {healAmount} HP. Health: {currentHealth}/{maxHealth}");
    }

    public void GainAmmo(int ammoAmount)
    {
        currentAmmo += ammoAmount;
        Debug.Log($"Gained {ammoAmount} ammo. Total: {currentAmmo}");
    }

    public void Fire(Vector2 direction)
    {
        if (currentGun == null)
        {
            Debug.Log("No gun equipped!");
            return;
        }

        if (currentGun.AmmoInMagazine > 0)
        {
            currentGun.Fire();
            CreateBullet(direction);
            Debug.Log($"Fired {currentGun.Name}. Ammo in mag: {currentGun.AmmoInMagazine}/{currentGun.MagazineSize}, Total: {currentGun.CurrentAmmo}");
        }
        else if (currentGun.CurrentAmmo > 0)
        {
            Reload();
            currentGun.Fire();
            CreateBullet(direction);
        }
        else
        {
            Debug.Log("Out of ammo!");
        }
    }

    public Vector2 GetFirePosition()
    {
        if (IsInVehicle && currentVehicle != null)
        {
            Vector2 vehiclePos = currentVehicle.transform.position;
            float vehicleRotation = currentVehicle.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            
            Vector2 forward = new Vector2(Mathf.Cos(vehicleRotation), Mathf.Sin(vehicleRotation));
            Vector2 right = new Vector2(-forward.y, forward.x);
            
            float offset = 3f;
            return vehiclePos + forward * offset;
        }
        return transform.position;
    }

    private void CreateBullet(Vector2 direction)
    {
        if (currentGun == null) return;

        if (currentGun.Type == Gun.GunType.RPG)
        {
            if (RocketPool.Instance != null)
            {
                GameObject rocket = RocketPool.Instance.GetRocket();
                if (rocket != null)
                {
                    RocketProjectile rocketScript = rocket.GetComponent<RocketProjectile>();
                    rocketScript.Setup(GetFirePosition(), direction, currentGun.Damage, currentGun.Range, 1);
                }
            }
        }
        else
        {
            if (BulletPool.Instance != null)
            {
                GameObject bullet = BulletPool.Instance.GetBullet();
                if (bullet != null)
                {
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    bulletScript.Setup(GetFirePosition(), direction, currentGun.Damage, currentGun.Range, currentGun.Type, 1);
                }
            }
        }
    }

    public void EquipGun(Gun gun)
    {
        currentGun = gun;
        Debug.Log($"Equipped {gun.Name}");
    }

    public void DropGun()
    {
        if (currentGun != null)
        {
            Debug.Log($"Dropped {currentGun.Name}");
            currentGun = null;
        }
    }

    public void Reload()
    {
        if (currentGun == null) return;

        currentGun.Reload();
        Debug.Log($"Reloaded {currentGun.Name}. Magazine: {currentGun.AmmoInMagazine}/{currentGun.MagazineSize}");
    }

    public Gun GetCurrentGun() => currentGun;
    public Vehicle GetCurrentVehicle() => currentVehicle;
    public int GetCurrentAmmo() => currentAmmo;

    public void EquipMeleeWeapon(object meleeWeapon)
    {
        currentMeleeWeapon = meleeWeapon;
        if (meleeWeapon is CricketBat)
        {
            Debug.Log("Equipped Cricket Bat - weak but better than fists!");
        }
    }

    public void EquipLaserGun()
    {
        if (laserGun == null)
        {
            laserGun = LaserGun.Instance;
        }
        Debug.Log("Equipped Laser Cannon - ultimate weapon!");
    }

    public object GetCurrentMeleeWeapon() => currentMeleeWeapon;
    public LaserGun GetLaserGun() => laserGun;
    public bool HasLaserGun() => laserGun != null;

    public float GetMoney() => currentMoney;
    
    public void AddMoney(float amount)
    {
        currentMoney += amount;
        Debug.Log($"Added ${amount}. Current balance: ${currentMoney}");
    }
    
    public bool RemoveMoney(float amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            Debug.Log($"Removed ${amount}. Current balance: ${currentMoney}");
            return true;
        }
        Debug.Log($"Cannot remove ${amount}. Insufficient funds. Current balance: ${currentMoney}");
        return false;
    }
    
    public void SetMoney(float amount)
    {
        currentMoney = Mathf.Max(0, amount);
        Debug.Log($"Money set to ${currentMoney}");
    }
}
