using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isRunning = false;
    private bool canMove = true;

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

        if (FindObjectOfType<BulletPool>() == null)
        {
            GameObject poolGO = new GameObject("BulletPool");
            poolGO.AddComponent<BulletPool>();
        }

        if (FindObjectOfType<RocketPool>() == null)
        {
            GameObject rocketPoolGO = new GameObject("RocketPool");
            rocketPoolGO.AddComponent<RocketPool>();
        }

        if (FindObjectOfType<GrenadeSpawner>() == null)
        {
            GameObject gs = new GameObject("GrenadeSpawner");
            gs.AddComponent<GrenadeSpawner>();
        }

        if (FindObjectOfType<SnacksManager>() == null)
        {
            GameObject snacksGO = new GameObject("SnacksManager");
            snacksGO.AddComponent<SnacksManager>();
        }

        if (FindObjectOfType<Grenade>() == null)
        {
            GameObject g = new GameObject("GrenadeManager");
            g.AddComponent<Grenade>();
        }

        if (FindObjectOfType<MapSystem>() == null)
        {
            GameObject m = new GameObject("MapSystem");
            m.AddComponent<MapSystem>();
        }

        if (FindObjectOfType<BusinessManager>() == null)
        {
            GameObject b = new GameObject("BusinessManager");
            b.AddComponent<BusinessManager>();
        }

        if (FindObjectOfType<MapMarkers>() == null)
        {
            GameObject mm = new GameObject("MapMarkers");
            mm.AddComponent<MapMarkers>();
        }

        if (FindObjectOfType<GameInputManager>() == null)
        {
            GameObject im = new GameObject("GameInputManager");
            im.AddComponent<GameInputManager>();
        }

        if (FindObjectOfType<GameModeManager>() == null)
        {
            GameObject gmm = new GameObject("GameModeManager");
            gmm.AddComponent<GameModeManager>();
        }

        if (GetComponent<VehicleTheft>() == null)
        {
            gameObject.AddComponent<VehicleTheft>();
        }

        if (GetComponent<BusInteraction>() == null)
        {
            gameObject.AddComponent<BusInteraction>();
        }

        if (GetComponent<MetroInteraction>() == null)
        {
            gameObject.AddComponent<MetroInteraction>();
        }

        if (GetComponent<TrainInteraction>() == null)
        {
            gameObject.AddComponent<TrainInteraction>();
        }
    }

    private void Update()
    {
        if (!canMove || !PlayerManager.Instance.IsAlive()) return;

        if (GameInputManager.Instance == null)
        {
            float moveX = UnityEngine.Input.GetAxisRaw("Horizontal");
            float moveY = UnityEngine.Input.GetAxisRaw("Vertical");
            moveDirection = new Vector2(moveX, moveY).normalized;
        }
        else
        {
            moveDirection = GameInputManager.Instance.GetPlayer1Move();
        }

        isRunning = GameInputManager.Instance.GetPlayer1Run();

        if (GameInputManager.Instance.GetPlayer1Fire())
        {
            Vector2 shootDir = GameInputManager.Instance.UsingGamepad 
                ? GameInputManager.Instance.GetPlayer1Aim() 
                : GetShootingDirection();
            
            if (PlayerManager.Instance.HasLaserGun())
            {
                LaserGun laser = PlayerManager.Instance.GetLaserGun();
                laser.Fire(shootDir, transform);
            }
            else
            {
                PlayerManager.Instance.Fire(shootDir);
            }
        }

        if (GameInputManager.Instance.GetPlayer1Melee())
        {
            MeleeCombat.Instance?.PerformMeleeAttack("Punch");
        }

        if (GameInputManager.Instance.GetPlayer1Kick())
        {
            MeleeCombat.Instance?.PerformMeleeAttack("Kick");
        }

        if (GameInputManager.Instance.GetPlayer1Grenade())
        {
            ThrowGrenade();
        }

        if (GameInputManager.Instance.GetPlayer1Reload())
        {
            if (PlayerManager.Instance.HasLaserGun())
            {
                LaserGun laser = PlayerManager.Instance.GetLaserGun();
                laser.Reload();
            }
            else
            {
                PlayerManager.Instance.Reload();
            }
        }

        if (GameModeManager.Instance.IsVs())
        {
            HandleVsModeAttacks();
        }

        if (PlayerManager.Instance.IsInVehicle && PlayerManager.Instance.currentVehicle != null)
        {
            HandleVehicleControls();
        }
    }

    private void HandleVehicleControls()
    {
        Vehicle vehicle = PlayerManager.Instance.currentVehicle;
        if (vehicle == null) return;

        if (GameInputManager.Instance.GetPlayer1Horn())
        {
            vehicle.HonkHorn();
        }

        if (GameInputManager.Instance.GetPlayer1CarLights())
        {
            vehicle.ToggleCarLights();
        }
    }

    private void HandleVsModeAttacks()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            VsModeSystem.Instance?.SabotageMission(1);
            Debug.Log("Player 1 sabotaged Player 2's mission!");
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            VsModeSystem.Instance?.ExplodePlayerVehicle(2);
            Debug.Log("Player 1 blew up Player 2's vehicle!");
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
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public Vector2 GetMoveDirection() => moveDirection;
    public bool IsRunning() => isRunning;

    private Vector2 GetShootingDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        return direction;
    }

    private void ThrowGrenade()
    {
        if (Grenade.Instance == null)
        {
            Debug.Log("Grenade system not initialized!");
            return;
        }

        if (Grenade.Instance.GetQuantity() <= 0)
        {
            Debug.Log("No grenades! Buy or find more.");
            return;
        }

        Vector2 direction = GetShootingDirection();
        float throwForce = 10f;
        
        Grenade.Instance.Throw(transform.position, direction, throwForce);
        Debug.Log($"Threw grenade! Remaining: {Grenade.Instance.GetQuantity()}");
    }

    public void RespawnAtSafeLocation()
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
                transform.position = pos;
                rb.velocity = Vector2.zero;
                Debug.Log($"Player respawned at safe location: {pos}");
                return;
            }
        }

        transform.position = new Vector2(500, 500);
        rb.velocity = Vector2.zero;
        Debug.Log("Player respawned at city center (500, 500)");
    }
}