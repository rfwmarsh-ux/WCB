using UnityEngine;
using System.Collections.Generic;

public class PoliceVehicleManager : MonoBehaviour
{
    public static PoliceVehicleManager Instance { get; private set; }

    private const int MIN_PATROL_CARS = 4;
    private const int MAX_PATROL_CARS = 8;
    private const int MAX_PATROL_VANS_WANTED_3 = 2;
    private const int MAX_PATROL_VANS_WANTED_4 = 5;
    private const int MAX_PATROL_VANS_WANTED_5 = 10;

    private List<PoliceVehicle> activeVehicles = new List<PoliceVehicle>();
    private List<PoliceVehicle> activeVans = new List<PoliceVehicle>();

    private float spawnTimer = 0f;
    private float spawnInterval = 3f;

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
        for (int i = 0; i < MIN_PATROL_CARS; i++)
        {
            TrySpawnPatrolCar();
        }
    }

    private void Update()
    {
        UpdateSpawning();
        UpdateVehicles();
        RemoveInactiveVehicles();
    }

    private void RemoveInactiveVehicles()
    {
        activeVehicles.RemoveAll(v => v == null || !v.gameObject.activeSelf);
        activeVans.RemoveAll(v => v == null || !v.gameObject.activeSelf);
    }

    private void UpdateSpawning()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer < spawnInterval) return;
        spawnTimer = 0f;

        int wantedLevel = WantedLevelManager.Instance != null ? WantedLevelManager.Instance.GetMaxWantedLevelInGame() : 0;

        int targetCars = MIN_PATROL_CARS + (wantedLevel * 2);
        targetCars = Mathf.Min(targetCars, MAX_PATROL_CARS);

        while (activeVehicles.Count < MIN_PATROL_CARS)
        {
            TrySpawnPatrolCar();
        }

        if (activeVehicles.Count < targetCars)
        {
            TrySpawnPatrolCar();
        }

        if (wantedLevel >= 3)
        {
            int targetVans = wantedLevel == 3 ? MAX_PATROL_VANS_WANTED_3 :
                           wantedLevel == 4 ? MAX_PATROL_VANS_WANTED_4 :
                           MAX_PATROL_VANS_WANTED_5;

            if (activeVans.Count < targetVans)
            {
                TrySpawnPoliceVan();
            }
        }
    }

    private void TrySpawnPatrolCar()
    {
        PoliceStationManager stationManager = PoliceStationManager.Instance;
        if (stationManager == null) return;

        PoliceStation station = stationManager.GetRandomStation();
        if (station == null) return;

        Vector2 spawnPos = station.Position + new Vector2(Random.Range(-5f, 5f), -15f);
        PoliceVehicle vehicle = CreatePoliceVehicle(spawnPos, false);
        if (vehicle != null)
        {
            activeVehicles.Add(vehicle);
        }
    }

    private void TrySpawnPoliceVan()
    {
        PoliceStationManager stationManager = PoliceStationManager.Instance;
        if (stationManager == null) return;

        PoliceStation station = stationManager.GetRandomStation();
        if (station == null) return;

        Vector2 spawnPos = station.Position + new Vector2(Random.Range(-5f, 5f), -18f);
        PoliceVehicle vehicle = CreatePoliceVehicle(spawnPos, true);
        if (vehicle != null)
        {
            activeVans.Add(vehicle);
        }
    }

    private PoliceVehicle CreatePoliceVehicle(Vector2 position, bool isVan)
    {
        GameObject vehicleGO = new GameObject(isVan ? "PoliceVan" : "PoliceCar");
        vehicleGO.transform.position = (Vector3)position;

        SpriteRenderer sr = vehicleGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = isVan ? new Color(0.1f, 0.1f, 0.15f, 1f) : new Color(0.15f, 0.15f, 0.2f, 1f);
        sr.sortingOrder = 5;

        // Real world proportions (Police car ~5m, Police van ~6m)
        float width = isVan ? 6f : 5f;
        float height = isVan ? 2.8f : 2.5f;
        vehicleGO.transform.localScale = new Vector3(width, height, 1f);

        Rigidbody2D rb = vehicleGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        BoxCollider2D collider = vehicleGO.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(width * 0.9f, height * 0.8f);
        collider.isTrigger = true;

        PoliceVehicle vehicle = vehicleGO.AddComponent<PoliceVehicle>();
        vehicle.Initialize(isVan);

        return vehicle;
    }

    private void UpdateVehicles()
    {
        for (int i = activeVehicles.Count - 1; i >= 0; i--)
        {
            if (activeVehicles[i] == null)
            {
                activeVehicles.RemoveAt(i);
                continue;
            }
            activeVehicles[i].UpdateBehavior();
        }

        for (int i = activeVans.Count - 1; i >= 0; i--)
        {
            if (activeVans[i] == null)
            {
                activeVans.RemoveAt(i);
                continue;
            }
            activeVans[i].UpdateBehavior();
        }
    }

    public void RemoveVehicle(PoliceVehicle vehicle)
    {
        if (vehicle.IsVan)
        {
            activeVans.Remove(vehicle);
        }
        else
        {
            activeVehicles.Remove(vehicle);
        }
    }

    public int GetActiveVehicleCount() => activeVehicles.Count;
    public int GetActiveVanCount() => activeVans.Count;
}

public class PoliceVehicle : MonoBehaviour
{
    public bool IsVan { get; private set; }
    public bool IsChasing { get; private set; }
    public bool IsParked { get; private set; }

    private enum VehicleState { Patrolling, Chasing, Returning, Parked }
    private VehicleState currentState = VehicleState.Patrolling;

    private float speed = 60f;
    private float chaseSpeed = 80f;
    private Vector2 targetPosition;
    private Vector2 patrolTarget;

    private float stateTimer = 0f;
    private float minPatrolTime = 10f;
    private float maxPatrolTime = 20f;

    private List<PoliceOfficer> passengers = new List<PoliceOfficer>();
    private int officersInVan = 5;
    private bool officersSpawned = false;

    private int targetPlayerId = 1;
    private bool sirenRegistered = false;

    private float npcCrimeScanRange = 80f;
    private float lastNpcScanTime = 0f;
    private float npcScanInterval = 0.5f;
    private Transform chaseTargetNPC = null;
    private bool isChasingNPC = false;

    public void Initialize(bool isVan)
    {
        IsVan = isVan;
        speed = isVan ? 50f : 60f;
        chaseSpeed = isVan ? 70f : 80f;
        PickNewPatrolTarget();
        IsParked = false;

        if (SirenManager.Instance != null)
        {
            SirenManager.Instance.CreateSirenLights(gameObject, SirenType.Police);
            SirenManager.Instance.RegisterSiren(gameObject, SirenType.Police, false);
            sirenRegistered = true;
        }
    }

    private void Start()
    {
        if (!sirenRegistered && SirenManager.Instance != null)
        {
            SirenManager.Instance.CreateSirenLights(gameObject, SirenType.Police);
            SirenManager.Instance.RegisterSiren(gameObject, SirenType.Police, false);
            sirenRegistered = true;
        }
    }

    public void UpdateBehavior()
    {
        if (IsParked)
        {
            UpdateParkedState();
            return;
        }

        int wantedLevel = WantedLevelManager.Instance != null ? WantedLevelManager.Instance.GetMaxWantedLevelInGame() : 0;
        bool shouldChase = wantedLevel > 0;

        if (shouldChase && !IsChasing && !isChasingNPC)
        {
            StartChasing();
        }
        else if (!shouldChase && !isChasingNPC && IsChasing)
        {
            StopChasing();
        }

        if (currentState == VehicleState.Patrolling)
        {
            ScanForNPCCrimes();
        }

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case VehicleState.Patrolling:
                UpdatePatrolling();
                break;
            case VehicleState.Chasing:
                UpdateChasing();
                break;
            case VehicleState.Returning:
                UpdateReturning();
                break;
        }
    }

    private void ScanForNPCCrimes()
    {
        if (Time.time - lastNpcScanTime < npcScanInterval) return;
        lastNpcScanTime = Time.time;

        if (isChasingNPC && chaseTargetNPC != null)
        {
            targetPosition = chaseTargetNPC.position;
            MoveToTarget(targetPosition, chaseSpeed);
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, npcCrimeScanRange);
        foreach (Collider2D hit in hits)
        {
            NPC npc = hit.GetComponent<NPC>();
            if (npc != null && npc.IsInCombat())
            {
                chaseTargetNPC = npc.transform;
                isChasingNPC = true;
                currentState = VehicleState.Chasing;
                Debug.Log($"Police vehicle chasing NPC for assault!");
                return;
            }
        }

        foreach (Collider2D hit in hits)
        {
            DeadBody body = hit.GetComponent<DeadBody>();
            if (body != null)
            {
                chaseTargetNPC = body.transform;
                isChasingNPC = true;
                currentState = VehicleState.Chasing;
                Debug.Log($"Police vehicle investigating dead body!");
                return;
            }
        }
    }

    private void PickNewPatrolTarget()
    {
        float x = Random.Range(150f, 850f);
        float y = Random.Range(150f, 850f);
        patrolTarget = new Vector2(x, y);
    }

    private void UpdatePatrolling()
    {
        if (Vector2.Distance(transform.position, patrolTarget) < 10f || stateTimer > Random.Range(minPatrolTime, maxPatrolTime))
        {
            PickNewPatrolTarget();
            stateTimer = 0f;
        }

        MoveToTarget(patrolTarget, speed);
    }

    private void UpdateChasing()
    {
        if (isChasingNPC && chaseTargetNPC != null)
        {
            if (!chaseTargetNPC.gameObject.activeSelf || chaseTargetNPC.GetComponent<NPC>() == null || !chaseTargetNPC.GetComponent<NPC>().IsAlive())
            {
                Debug.Log($"NPC caught/killed! Police returning to patrol.");
                StopChasingNPC();
                return;
            }

            targetPosition = chaseTargetNPC.position;
            MoveToTarget(targetPosition, chaseSpeed);

            if (Vector2.Distance(transform.position, targetPosition) < 10f)
            {
                Debug.Log($"Police caught NPC! Returning to patrol.");
                StopChasingNPC();
            }
            return;
        }

        int player1Wanted = WantedLevelManager.Instance != null ? WantedLevelManager.Instance.Player1WantedLevel : 0;
        int player2Wanted = WantedLevelManager.Instance != null ? WantedLevelManager.Instance.Player2WantedLevel : 0;

        Vector2 p1Pos = WantedLevelManager.Instance != null ? WantedLevelManager.Instance.GetLastKnownPlayerPosition(1) : Vector2.zero;
        Vector2 p2Pos = WantedLevelManager.Instance != null ? WantedLevelManager.Instance.GetLastKnownPlayerPosition(2) : Vector2.zero;

        if (player1Wanted > 0 && player2Wanted > 0)
        {
            targetPlayerId = player1Wanted >= player2Wanted ? 1 : 2;
        }
        else if (player1Wanted > 0)
        {
            targetPlayerId = 1;
        }
        else if (player2Wanted > 0)
        {
            targetPlayerId = 2;
        }
        else
        {
            StopChasing();
            return;
        }

        targetPosition = targetPlayerId == 1 ? p1Pos : p2Pos;

        MoveToTarget(targetPosition, chaseSpeed);

        if (Vector2.Distance(transform.position, targetPosition) < 15f)
        {
            TryParkAndDeploy();
        }
    }

    private void StopChasingNPC()
    {
        isChasingNPC = false;
        chaseTargetNPC = null;
        currentState = VehicleState.Patrolling;
        PickNewPatrolTarget();
        stateTimer = 0f;
    }

    private void UpdateReturning()
    {
        PoliceStationManager stationManager = PoliceStationManager.Instance;
        if (stationManager == null) return;

        PoliceStation closest = stationManager.GetClosestStation(transform.position);
        if (closest == null) return;

        MoveToTarget(closest.Position, speed * 0.7f);

        if (Vector2.Distance(transform.position, closest.Position) < 20f)
        {
            Destroy(gameObject);
            PoliceVehicleManager.Instance.RemoveVehicle(this);
        }
    }

    private void MoveToTarget(Vector2 target, float moveSpeed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void StartChasing()
    {
        IsChasing = true;
        currentState = VehicleState.Chasing;
        stateTimer = 0f;

        if (SirenManager.Instance != null)
        {
            SirenManager.Instance.SetSirenActive(gameObject, true);
        }
    }

    private void StopChasing()
    {
        IsChasing = false;
        isChasingNPC = false;
        chaseTargetNPC = null;
        currentState = VehicleState.Returning;
        stateTimer = 0f;

        if (SirenManager.Instance != null)
        {
            SirenManager.Instance.SetSirenActive(gameObject, false);
        }
    }

    private void TryParkAndDeploy()
    {
        if (!officersSpawned)
        {
            if (IsVan)
            {
                officersInVan = 5;
            }
            else
            {
                officersInVan = 2;
            }
            
            IsParked = true;
            SpawnOfficers();
        }
    }

    private void UpdateParkedState()
    {
        int wantedLevel = WantedLevelManager.Instance.GetMaxWantedLevelInGame();

        if (wantedLevel == 0)
        {
            IsParked = false;
            officersSpawned = false;
            foreach (var officer in passengers)
            {
                if (officer != null) Destroy(officer.gameObject);
            }
            passengers.Clear();
            StopChasing();
            return;
        }

        Vector2 playerPos = WantedLevelManager.Instance.GetLastKnownPlayerPosition(targetPlayerId);
        float distToPlayer = Vector2.Distance(transform.position, playerPos);

        foreach (var officer in passengers)
        {
            if (officer != null)
            {
                officer.SetTargetPlayer(targetPlayerId);
                officer.SetEnabled(true);
            }
        }

        if (distToPlayer > 100f)
        {
            IsParked = false;
            foreach (var officer in passengers)
            {
                if (officer != null) Destroy(officer.gameObject);
            }
            passengers.Clear();
            officersSpawned = false;
        }
    }

    private void SpawnOfficers()
    {
        for (int i = 0; i < officersInVan; i++)
        {
            Vector2 offset = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            Vector2 spawnPos = (Vector2)transform.position + offset;

            PoliceOfficer officer = PoliceOfficer.CreateOfficer(spawnPos, true);
            officer.SetTargetPlayer(targetPlayerId);
            passengers.Add(officer);
        }
        officersSpawned = true;
    }

    private void OnDestroy()
    {
        foreach (var officer in passengers)
        {
            if (officer != null) Destroy(officer.gameObject);
        }
    }

    private float lastRammingReportTime = 0f;
    private float rammingCooldown = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time - lastRammingReportTime < rammingCooldown) return;

        int playerId = 0;

        if (other.name == "Player1" || other.CompareTag("Player"))
        {
            playerId = 1;
        }
        else if (other.name == "Player2")
        {
            playerId = 2;
        }

        if (playerId > 0)
        {
            lastRammingReportTime = Time.time;
            
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                float playerSpeed = playerRb.velocity.magnitude;
                if (playerSpeed > 5f)
                {
                    WantedLevelManager.Instance.ReportCrime(playerId, WantedLevelManager.CrimeType.PoliceVehicleRamming);
                    Debug.Log($"Player {playerId} rammed a police vehicle! Wanted level increased.");
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (SirenManager.Instance != null)
        {
            SirenManager.Instance.UnregisterSiren(gameObject);
        }
    }
}
