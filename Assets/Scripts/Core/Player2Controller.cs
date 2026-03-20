using UnityEngine;

public class Player2Controller : MonoBehaviour
{
    public static Player2Controller Instance { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isRunning = false;
    private bool canMove = true;

    private bool isAlive = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.3f, 0.3f, 0.9f, 1f);
        sr.sortingOrder = 5;

        if (GetComponent<VehicleTheft>() == null)
        {
            gameObject.AddComponent<VehicleTheft>();
        }
    }

    private void Update()
    {
        if (!canMove || !isAlive) return;
        if (GameInputManager.Instance == null) return;

        moveDirection = GameInputManager.Instance.GetPlayer2Move();
        isRunning = UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightShift);

        if (GameInputManager.Instance.GetPlayer2Fire())
        {
            Player2Manager p2m = GetComponent<Player2Manager>();
            if (p2m != null && p2m.GetCurrentGun() != null)
            {
                Vector2 aimDir = GameInputManager.Instance.GetPlayer2Aim();
                p2m.Fire(aimDir);
            }
        }

        if (GameModeManager.Instance.IsVs())
        {
            HandleVsModeAttacks();
        }

        if (Player2Manager.Instance.IsInVehicle && Player2Manager.Instance.currentVehicle != null)
        {
            HandleVehicleControls();
        }
    }

    private void HandleVehicleControls()
    {
        Vehicle vehicle = Player2Manager.Instance.currentVehicle;
        if (vehicle == null) return;

        if (GameInputManager.Instance.GetPlayer2Horn())
        {
            vehicle.HonkHorn();
        }

        if (GameInputManager.Instance.GetPlayer2CarLights())
        {
            vehicle.ToggleCarLights();
        }
    }

    private void HandleVsModeAttacks()
    {
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            VsModeSystem.Instance?.SabotageMission(2);
            Debug.Log("Player 2 sabotaged Player 1's mission!");
        }

        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            VsModeSystem.Instance?.ExplodePlayerVehicle(1);
            Debug.Log("Player 2 blew up Player 1's vehicle!");
        }
    }

    private void FixedUpdate()
    {
        if (!canMove || moveDirection == Vector2.zero)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float speed = isRunning ? runSpeed : moveSpeed;
        rb.linearVelocity = moveDirection * speed;
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
        if (!canMove) rb.linearVelocity = Vector2.zero;
    }

    public Vector2 GetMoveDirection() => moveDirection;
    public bool IsRunning() => isRunning;
    public bool IsAlive() => isAlive;

    public void SetAlive(bool alive)
    {
        isAlive = alive;
        if (!alive)
        {
            rb.linearVelocity = Vector2.zero;
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }
}

public class Player2Manager : MonoBehaviour
{
    public static Player2Manager Instance { get; private set; }

    public int PlayerId { get; private set; } = 2;

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private float currentArmour = 0f;
    [SerializeField] private float maxArmour = 100f;
    private Gun currentGun;
    private bool isAlive = true;
    private Vector2 lastPosition;

    public bool IsInVehicle { get; set; } = false;
    public Vehicle currentVehicle { get; set; } = null;

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
        lastPosition = transform.position;
    }

    public void TakeDamage(float damageAmount)
    {
        if (!isAlive) return;

        float armourAbsorbed = Mathf.Min(currentArmour, damageAmount);
        float healthDamage = damageAmount - armourAbsorbed;

        currentArmour -= armourAbsorbed;
        currentHealth -= healthDamage;

        Debug.Log($"Player 2 took {damageAmount} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
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
        
        Debug.Log($"Player 2 entered vehicle: {vehicle.DisplayName}");
    }
    
    public void ExitVehicle()
    {
        if (currentVehicle == null) return;
        
        currentVehicle = null;
        IsInVehicle = false;
        
        Debug.Log("Player 2 exited vehicle");
    }

    public void Die()
    {
        isAlive = false;
        currentHealth = 0;
        lastPosition = transform.position;
        Debug.Log("Player 2 died!");

        if (GameModeManager.Instance.IsVs())
        {
            Player2Controller.Instance.SetAlive(false);
            GameObject.FindObjectOfType<VsModeSystem>()?.Player2Died();
        }
        else
        {
            Invoke("TriggerPlayer2Respawn", 1f);
        }
    }

    private void TriggerPlayer2Respawn()
    {
        if (GameManager.Instance != null)
        {
            VeterinaryCentreManager centreManager = GameManager.Instance.GetVeterinaryCentreManager();
            if (centreManager != null)
            {
                centreManager.RespawnPlayerAtClosestCentre(this, lastPosition, PlayerId);
            }
        }
    }

    public void Respawn(Vector2 position)
    {
        currentHealth = maxHealth;
        currentArmour = 0f;
        isAlive = true;
        transform.position = position;
        Player2Controller.Instance.SetAlive(true);

        // Clear wanted level on respawn
        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.ClearWantedLevel(PlayerId);
            Debug.Log($"Player {PlayerId} wanted level cleared on respawn");
        }
    }

    public void EquipGun(Gun gun)
    {
        currentGun = gun;
        Debug.Log($"Player 2 equipped {gun.Name}");
    }

    public void Fire(Vector2 direction)
    {
        if (currentGun == null) return;

        if (currentGun.AmmoInMagazine > 0)
        {
            currentGun.Fire();
            
            if (currentGun.Type == Gun.GunType.RPG)
            {
                if (RocketPool.Instance != null)
                {
                    GameObject rocket = RocketPool.Instance.GetRocket();
                    if (rocket != null)
                    {
                        RocketProjectile rocketScript = rocket.GetComponent<RocketProjectile>();
                        rocketScript.Setup(GetFirePosition(), direction, currentGun.Damage, currentGun.Range, 2);
                    }
                }
            }
            else if (BulletPool.Instance != null)
            {
                GameObject bullet = BulletPool.Instance.GetBullet();
                if (bullet != null)
                {
                    Bullet b = bullet.GetComponent<Bullet>();
                    b.Setup(GetFirePosition(), direction, currentGun.Damage, currentGun.Range, currentGun.Type, 2);
                }
            }
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

    public void Reload()
    {
        if (currentGun != null) currentGun.Reload();
    }

    public void HealDamage(float healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        Debug.Log($"Player 2 healed {healAmount} HP. Health: {currentHealth}/{maxHealth}");
    }

    public float GetHealth() => currentHealth;
    public float Health => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsAlive() => isAlive;
    public Gun GetCurrentGun() => currentGun;
    public Vehicle GetCurrentVehicle() => currentVehicle;
}