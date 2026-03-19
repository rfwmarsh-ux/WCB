using UnityEngine;

public class VehicleDriver : MonoBehaviour
{
    public static VehicleDriver Instance { get; private set; }

    [Header("Fighting Settings")]
    [SerializeField] private float fightBackChance = 0.04f;
    [SerializeField] private float armedChance = 0.30f;

    [Header("Driver Stats")]
    [SerializeField] private float health = 50f;
    [SerializeField] private float combatSkill = 1f;
    [SerializeField] private bool hasWeapon = false;
    [SerializeField] private bool isFighting = false;

    private Gun driverGun;
    private Transform playerTarget;
    private float lastAttackTime;
    private float attackCooldown = 1.5f;

    private enum DriverState
    {
        Normal,
        Alert,
        Fighting,
        Fleeing
    }

    private DriverState currentState = DriverState.Normal;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void InitializeAsDriver(GameObject vehicleGO)
    {
        float roll = Random.value;
        
        if (roll < fightBackChance)
        {
            currentState = DriverState.Alert;
            Debug.Log("Driver is alert and may fight back!");
            
            if (Random.value < armedChance)
            {
                EquipDriverWeapon();
                Debug.Log("Driver is armed!");
            }
        }
        else
        {
            Debug.Log("Driver is non-confrontational");
        }
    }

    private void EquipDriverWeapon()
    {
        hasWeapon = true;
        
        int weaponRoll = Random.Range(0, 4);
        switch (weaponRoll)
        {
            case 0:
                driverGun = new Gun(Gun.GunType.Pistol, "Driver Pistol", 15f, 3f, 12, 20f, 0.7f, 0);
                break;
            case 1:
                driverGun = new Gun(Gun.GunType.Revolver, "Driver Revolver", 25f, 1.5f, 6, 25f, 0.8f, 0);
                break;
            case 2:
                driverGun = new Gun(Gun.GunType.SMG, "Driver SMG", 10f, 8f, 25, 15f, 0.6f, 0);
                break;
            case 3:
                driverGun = new Gun(Gun.GunType.AssaultRifle, "Driver Rifle", 20f, 5f, 25, 35f, 0.7f, 0);
                break;
        }
    }

    private void Update()
    {
        if (!isFighting || playerTarget == null) return;

        float distance = Vector2.Distance(transform.position, playerTarget.position);

        switch (currentState)
        {
            case DriverState.Alert:
                if (distance < 15f)
                {
                    StartFighting();
                }
                break;
            case DriverState.Fighting:
                HandleCombat(distance);
                break;
            case DriverState.Fleeing:
                FleeFromPlayer();
                break;
        }
    }

    public void OnAttemptedTheft(GameObject player)
    {
        playerTarget = player.transform;

        if (currentState == DriverState.Alert)
        {
            StartFighting();
            GameManager.Instance.UpdateWantedLevel(1);
        }
    }

    private void StartFighting()
    {
        currentState = DriverState.Fighting;
        isFighting = true;
        Debug.Log("Driver is fighting back!");
        
        PlayerController pc = playerTarget.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.SetCanMove(false);
            Invoke("EnablePlayerMovement", 2f);
        }
    }

    private void HandleCombat(float distance)
    {
        if (distance > 30f)
        {
            currentState = DriverState.Fleeing;
            return;
        }

        if (hasWeapon && Time.time >= lastAttackTime + attackCooldown)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        if (driverGun == null || !driverGun.HasAmmo())
        {
            FleeFromPlayer();
            return;
        }

        lastAttackTime = Time.time;
        driverGun.Fire();

        Vector2 direction = (playerTarget.position - transform.position).normalized;
        
        if (BulletPool.Instance != null)
        {
            GameObject bullet = BulletPool.Instance.GetBullet();
            Bullet b = bullet.GetComponent<Bullet>();
            b.Setup(transform.position, direction, driverGun.Damage, driverGun.Range, driverGun.Type, 0);
        }

        Debug.Log($"Driver shot at player for {driverGun.Damage} damage!");
    }

    private void FleeFromPlayer()
    {
        isFighting = false;
        Debug.Log("Driver fleeing!");
        
        if (playerTarget != null)
        {
            Vector2 fleeDir = ((Vector2)transform.position - (Vector2)playerTarget.position).normalized;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = fleeDir * 8f;
            }
        }
    }

    private void EnablePlayerMovement()
    {
        if (playerTarget != null)
        {
            PlayerController pc = playerTarget.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.SetCanMove(true);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isFighting = false;
        
        if (hasWeapon && Random.value < 0.5f)
        {
            DropWeapon();
        }

        Debug.Log("Driver defeated!");
        Destroy(gameObject, 0.5f);
    }

    private void DropWeapon()
    {
        if (driverGun == null) return;

        GameObject weaponPickup = new GameObject("DroppedWeapon");
        weaponPickup.transform.position = transform.position;
        
        SpriteRenderer sr = weaponPickup.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = Color.red;
        sr.sortingOrder = 5;

        CircleCollider2D collider = weaponPickup.AddComponent<CircleCollider2D>();
        collider.radius = 1.5f;
        collider.isTrigger = true;

        weaponPickup.AddComponent<DroppedWeapon>().Initialize(driverGun);
    }

    public bool IsFighting() => isFighting;
    public bool HasWeapon() => hasWeapon;
}

public class DroppedWeapon : MonoBehaviour
{
    private Gun weapon;

    public void Initialize(Gun gun)
    {
        weapon = gun;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager pm = other.GetComponent<PlayerManager>();
            if (pm != null && weapon != null)
            {
                pm.EquipGun(weapon);
                Debug.Log($"Picked up {weapon.Name} from driver!");
                Destroy(gameObject);
            }
        }
    }
}