using UnityEngine;
using System.Collections.Generic;

public class TrainCollisionSystem : MonoBehaviour
{
    public static TrainCollisionSystem Instance { get; private set; }
    
    private float trainCriticalSpeedPercent = 0.1f;
    private float metroCriticalSpeedPercent = 0.1f;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void HandleTrainCollision(Vehicle vehicle, Train train)
    {
        if (train == null || !train.IsMoving())
        {
            return;
        }
        
        float trainSpeed = train.GetCurrentSpeed();
        float maxSpeed = train.GetMaxSpeed();
        float speedRatio = trainSpeed / maxSpeed;
        
        if (speedRatio < trainCriticalSpeedPercent)
        {
            return;
        }
        
        bool instantDestroy = IsLightVehicle(vehicle.VehicleType);
        bool heavyVehicle = IsHeavyVehicle(vehicle.VehicleType);
        
        if (instantDestroy)
        {
            vehicle.TakeDamage(vehicle.MaxHealth * 2f);
            CreateExplosionEffect(vehicle.transform.position);
            Debug.Log($"Vehicle {vehicle.DisplayName} destroyed by train collision!");
        }
        else
        {
            float bounceForce = trainSpeed * 3f;
            BounceVehicleOffTrack(vehicle, train.transform.position, bounceForce);
            
            float damage = vehicle.MaxHealth * speedRatio * 0.5f;
            vehicle.TakeDamage(damage);
            Debug.Log($"Vehicle {vehicle.DisplayName} bounced by train. Damage: {damage}");
        }
    }
    
    public void HandleMetroCollision(Vehicle vehicle, MetroTram metro)
    {
        if (metro == null || !metro.IsMoving())
        {
            return;
        }
        
        float metroSpeed = metro.GetCurrentSpeed();
        float maxSpeed = metro.GetMaxSpeed();
        float speedRatio = metroSpeed / maxSpeed;
        
        if (speedRatio < metroCriticalSpeedPercent)
        {
            return;
        }
        
        bool instantDestroy = IsLightVehicle(vehicle.VehicleType);
        
        if (instantDestroy)
        {
            vehicle.TakeDamage(vehicle.MaxHealth * 1.5f);
            CreateExplosionEffect(vehicle.transform.position);
            Debug.Log($"Vehicle {vehicle.DisplayName} destroyed by metro collision!");
        }
        else
        {
            float bounceForce = metroSpeed * 2.5f;
            BounceVehicleOffTrack(vehicle, metro.transform.position, bounceForce);
            
            float damage = vehicle.MaxHealth * speedRatio * 0.3f;
            vehicle.TakeDamage(damage);
            Debug.Log($"Vehicle {vehicle.DisplayName} bounced by metro. Damage: {damage}");
        }
    }
    
    private void BounceVehicleOffTrack(Vehicle vehicle, Vector2 trainPosition, float force)
    {
        Vector2 vehiclePos = vehicle.transform.position;
        Vector2 bounceDirection = (vehiclePos - trainPosition).normalized;
        
        if (Mathf.Abs(bounceDirection.x) < 0.3f)
        {
            bounceDirection = new Vector2(bounceDirection.y > 0 ? 1f : -1f, 0);
        }
        else
        {
            bounceDirection = new Vector2(bounceDirection.x > 0 ? 1f : -1f, bounceDirection.y * 0.3f).normalized;
        }
        
        Rigidbody2D rb = vehicle.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = bounceDirection * force;
            rb.AddForce(bounceDirection * force * 10f, ForceMode2D.Impulse);
        }
        else
        {
            Vector2 currentVel = vehicle.GetVelocity();
            vehicle.transform.position += (Vector3)(bounceDirection * force * 0.1f);
        }
        
        Debug.Log($"Vehicle bounced off track with force {force}");
    }
    
    private bool IsLightVehicle(VehicleType type)
    {
        return type == VehicleType.Motorcycle ||
               type == VehicleType.SportBike ||
               type == VehicleType.Scooter ||
               type == VehicleType.CompactCar ||
               type == VehicleType.Hatchback ||
               type == VehicleType.EconomyCar;
    }
    
    private bool IsHeavyVehicle(VehicleType type)
    {
        return type == VehicleType.SlowTruck ||
               type == VehicleType.Bus ||
               type == VehicleType.Ambulance ||
               type == VehicleType.SUV ||
               type == VehicleType.PickupTruck ||
               type == VehicleType.OffroadVehicle ||
               type == VehicleType.ArmoredCar;
    }
    
    private void CreateExplosionEffect(Vector2 position)
    {
        GameObject explosion = new GameObject("TrainCollisionExplosion");
        explosion.transform.position = (Vector3)position;
        
        SpriteRenderer sr = explosion.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(1f, 0.5f, 0f, 0.8f);
        sr.sortingOrder = 20;
        explosion.transform.localScale = new Vector3(12f, 12f, 1f);
        
        ParticleSystem particles = explosion.AddComponent<ParticleSystem>();
        if (particles != null)
        {
            var main = particles.main;
            main.startColor = new Color(1f, 0.3f, 0f, 1f);
            main.startSize = 2f;
            main.startSpeed = 6f;
            main.startLifetime = 0.8f;
            
            var emission = particles.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20) });
        }
        
        Destroy(explosion, 1.5f);
    }
}

public class Train : MonoBehaviour
{
    [SerializeField] private int capacity = 50;
    [SerializeField] private float speed = 25f;
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float stopDuration = 10f;

    private List<Vector2> route;
    private int currentWaypointIndex = 0;
    private int trainNumber;
    private bool isMoving = false;
    private bool isStopped = false;
    private float stopTimer = 0f;
    private Rigidbody2D rb;

    private List<Pedestrian> passengers = new List<Pedestrian>();
    private int maxPassengers;

    private enum TrainDirection { Forward, Reverse }
    private TrainDirection direction = TrainDirection.Forward;
    
    private bool isStolen = false;
    private bool isAtTunnel = false;
    private Vector2 tunnelExitPosition;
    
    private float mapMinX = 50f;
    private float mapMaxX = 950f;
    private float mapMinY = 50f;
    private float mapMaxY = 950f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;
        
        CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
        col.radius = 4f;
        col.isTrigger = true;
    }

    public void Initialize(int trainCapacity, float trainSpeed, List<Vector2> routePath, int trainNum)
    {
        maxPassengers = trainCapacity;
        speed = trainSpeed;
        maxSpeed = trainSpeed;
        trainNumber = trainNum;
        route = new List<Vector2>(routePath);

        if (route.Count > 0)
        {
            transform.position = (Vector3)route[0];
            isMoving = true;
        }
    }

    private void Update()
    {
        CheckForVehicleCollisions();
        
        CheckForPlayerStealing();
        
        if (isStolen && isAtTunnel)
        {
            EjectPlayerAndDestroy();
            return;
        }
        
        if (isStopped)
        {
            stopTimer -= Time.deltaTime;
            if (stopTimer <= 0)
            {
                Depart();
            }
            return;
        }

        if (route.Count == 0) return;

        Vector2 targetWaypoint = route[currentWaypointIndex];
        Vector2 directionVec = (targetWaypoint - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetWaypoint);

        if (distance < 3f)
        {
            if (isAtMapEdge(targetWaypoint))
            {
                EnterTunnel();
                return;
            }

            ArriveAtWaypoint(targetWaypoint);
        }
        else
        {
            rb.linearVelocity = directionVec * speed;
            if (directionVec.x != 0)
            {
                GetComponent<SpriteRenderer>().flipX = directionVec.x < 0;
            }
        }
    }
    
    private void CheckForPlayerStealing()
    {
        if (isStolen) return;
        if (!isStopped) return;
        
        if (PlayerManager.Instance != null && !PlayerManager.Instance.IsInVehicle)
        {
            float dist = Vector2.Distance(PlayerManager.Instance.transform.position, transform.position);
            if (dist < 8f && Random.value < 0.02f)
            {
                StealTrain();
            }
        }
    }
    
    private void StealTrain()
    {
        isStolen = true;
        Debug.Log($"Train {trainNumber} has been stolen!");
        
        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.ReportSuccessfulTheft(1);
        }
    }
    
    private void EjectPlayerAndDestroy()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.IsInVehicle && 
            PlayerManager.Instance.currentVehicle != null)
        {
            Vehicle currentVehicle = PlayerManager.Instance.currentVehicle;
            
            if (currentVehicle is Train)
            {
                Vector2 ejectPos = transform.position + new Vector3(Random.Range(-5f, 5f), Random.Range(5f, 15f), 0);
                ejectPos.x = Mathf.Clamp(ejectPos.x, mapMinX + 10f, mapMaxX - 10f);
                ejectPos.y = Mathf.Clamp(ejectPos.y, mapMinY + 10f, mapMaxY - 10f);
                
                PlayerManager.Instance.ExitVehicle();
                currentVehicle.RemoveDriver();
                
                PlayerManager.Instance.transform.position = ejectPos;
                PlayerManager.Instance.gameObject.SetActive(true);
                PlayerManager.Instance.GetComponent<SpriteRenderer>().enabled = true;
                PlayerManager.Instance.GetComponent<Rigidbody2D>().simulated = true;
                
                Debug.Log($"Player ejected from train at {ejectPos}");
            }
        }
        
        Destroy(gameObject, 0.1f);
    }
    
    private bool isAtMapEdge(Vector2 point)
    {
        return point.x < mapMinX || point.x > mapMaxX || 
               point.y < mapMinY || point.y > mapMaxY;
    }
    
    private void CheckForVehicleCollisions()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 6f);
        
        foreach (var hit in hits)
        {
            Vehicle vehicle = hit.GetComponent<Vehicle>();
            if (vehicle != null && vehicle.IsOccupied)
            {
                TrainCollisionSystem.Instance?.HandleTrainCollision(vehicle, this);
            }
        }
    }

    public bool IsMoving() => isMoving && !isStopped;
    public float GetCurrentSpeed() => speed;
    public float GetMaxSpeed() => maxSpeed;
    public bool IsStolen() => isStolen;

    private void EnterTunnel()
    {
        isAtTunnel = true;
        
        if (isStolen)
        {
            Debug.Log($"Stolen train {trainNumber} entering tunnel - player will be ejected!");
        }
        else
        {
            Debug.Log($"Train {trainNumber} entered tunnel and left the city");
            
            foreach (var passenger in passengers)
            {
                if (passenger != null)
                {
                    Destroy(passenger.gameObject);
                }
            }
            passengers.Clear();

            Destroy(gameObject, 1f);
        }
    }

    private void ArriveAtWaypoint(Vector2 waypoint)
    {
        currentWaypointIndex++;

        if (currentWaypointIndex >= route.Count)
        {
            currentWaypointIndex = 0;
            direction = TrainDirection.Reverse;
            route.Reverse();
        }

        if (IsStation(waypoint))
        {
            StopAtStation();
        }
    }

    private bool IsStation(Vector2 point)
    {
        Vector2 stationPos = new Vector2(460, 550);
        return Vector2.Distance(point, stationPos) < 10f;
    }

    private void StopAtStation()
    {
        isStopped = true;
        stopTimer = stopDuration;
        rb.linearVelocity = Vector2.zero;
        
        Debug.Log($"Train {trainNumber} stopped at station");
    }

    private void Depart()
    {
        isStopped = false;
        isStolen = false;
        Debug.Log($"Train {trainNumber} departed");
    }

    public void AddPassenger(Pedestrian passenger)
    {
        if (passengers.Count < maxPassengers)
        {
            passengers.Add(passenger);
        }
    }

    public void RemovePassenger(Pedestrian passenger)
    {
        passengers.Remove(passenger);
    }

    public int GetPassengerCount() => passengers.Count;
    public int GetTrainNumber() => trainNumber;
    
    public bool IsStopped() => isStopped;
    
    public void SetDriver(PlayerController driver)
    {
        
    }
}

public class MetroTram : MonoBehaviour
{
    [SerializeField] private int capacity = 30;
    [SerializeField] private float speed = 15f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float stopDuration = 5f;

    private List<MetroStation> stations;
    private int currentStationIndex = 0;
    private int tramNumber;
    private bool isMoving = false;
    private bool isStopped = false;
    private float stopTimer = 0f;
    private Rigidbody2D rb;

    private List<Pedestrian> passengers = new List<Pedestrian>();
    private int maxPassengers;
    private MetroStation currentStation;
    
    private bool isStolen = false;
    private bool isAtTunnel = false;
    
    private float mapMinX = 50f;
    private float mapMaxX = 950f;
    private float mapMinY = 50f;
    private float mapMaxY = 950f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;
        
        CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
        col.radius = 3f;
        col.isTrigger = true;
    }

    public void Initialize(int tramCapacity, float tramSpeed, List<MetroStation> stationList, int tramNum)
    {
        maxPassengers = tramCapacity;
        speed = tramSpeed;
        maxSpeed = tramSpeed;
        tramNumber = tramNum;
        stations = stationList;

        if (stations.Count > 0)
        {
            currentStation = stations[0];
            transform.position = currentStation.transform.position;
            isMoving = true;
        }
    }

    private void Update()
    {
        CheckForVehicleCollisions();
        
        CheckForPlayerStealing();
        
        if (isStolen && isAtTunnel)
        {
            EjectPlayerAndDestroy();
            return;
        }
        
        if (isStopped)
        {
            stopTimer -= Time.deltaTime;
            if (stopTimer <= 0)
            {
                Depart();
            }
            return;
        }

        if (stations.Count == 0) return;

        Vector2 targetPos = stations[currentStationIndex].transform.position;
        Vector2 directionVec = ((Vector2)targetPos - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetPos);

        if (distance < 2f)
        {
            ArriveAtStation();
        }
        else
        {
            rb.linearVelocity = directionVec * speed;
            if (directionVec.x != 0)
            {
                GetComponent<SpriteRenderer>().flipX = directionVec.x < 0;
            }
        }
    }
    
    private void CheckForPlayerStealing()
    {
        if (isStolen) return;
        if (!isStopped) return;
        
        if (PlayerManager.Instance != null && !PlayerManager.Instance.IsInVehicle)
        {
            float dist = Vector2.Distance(PlayerManager.Instance.transform.position, transform.position);
            if (dist < 6f && Random.value < 0.03f)
            {
                StealMetro();
            }
        }
    }
    
    private void StealMetro()
    {
        isStolen = true;
        Debug.Log($"Metro tram {tramNumber} has been stolen!");
        
        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.ReportSuccessfulTheft(1);
        }
    }
    
    private void EjectPlayerAndDestroy()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.IsInVehicle && 
            PlayerManager.Instance.currentVehicle != null)
        {
            Vehicle currentVehicle = PlayerManager.Instance.currentVehicle;
            
            if (currentVehicle is MetroTram)
            {
                Vector2 ejectPos = transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(5f, 10f), 0);
                ejectPos.x = Mathf.Clamp(ejectPos.x, mapMinX + 10f, mapMaxX - 10f);
                ejectPos.y = Mathf.Clamp(ejectPos.y, mapMinY + 10f, mapMaxY - 10f);
                
                PlayerManager.Instance.ExitVehicle();
                currentVehicle.RemoveDriver();
                
                PlayerManager.Instance.transform.position = ejectPos;
                PlayerManager.Instance.gameObject.SetActive(true);
                PlayerManager.Instance.GetComponent<SpriteRenderer>().enabled = true;
                PlayerManager.Instance.GetComponent<Rigidbody2D>().simulated = true;
                
                Debug.Log($"Player ejected from metro at {ejectPos}");
            }
        }
        
        Destroy(gameObject, 0.1f);
    }
    
    private void CheckForVehicleCollisions()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 5f);
        
        foreach (var hit in hits)
        {
            Vehicle vehicle = hit.GetComponent<Vehicle>();
            if (vehicle != null && vehicle.IsOccupied)
            {
                TrainCollisionSystem.Instance?.HandleMetroCollision(vehicle, this);
            }
        }
    }

    public bool IsMoving() => isMoving && !isStopped;
    public float GetCurrentSpeed() => speed;
    public float GetMaxSpeed() => maxSpeed;
    public bool IsStolen() => isStolen;

    private void ArriveAtStation()
    {
        isStopped = true;
        stopTimer = stopDuration;
        rb.linearVelocity = Vector2.zero;
        currentStation = stations[currentStationIndex];
        
        isAtTunnel = CheckIfAtTunnel();
        
        Debug.Log($"Metro tram {tramNumber} arrived at {currentStation.StationName}" + 
                  (isAtTunnel ? " (at tunnel)" : ""));
    }
    
    private bool CheckIfAtTunnel()
    {
        Vector2 pos = transform.position;
        bool nearLeftEdge = pos.x < mapMinX + 20f;
        bool nearRightEdge = pos.x > mapMaxX - 20f;
        bool nearBottomEdge = pos.y < mapMinY + 20f;
        bool nearTopEdge = pos.y > mapMaxY - 20f;
        
        return nearLeftEdge || nearRightEdge || nearBottomEdge || nearTopEdge;
    }

    private void Depart()
    {
        isStopped = false;
        isStolen = false;
        isAtTunnel = false;
        currentStationIndex = (currentStationIndex + 1) % stations.Count;
        
        if (currentStationIndex == 0)
        {
            stations.Reverse();
        }
        
        Debug.Log($"Metro tram {tramNumber} departed");
    }

    public void AddPassenger(Pedestrian passenger)
    {
        if (passengers.Count < maxPassengers)
        {
            passengers.Add(passenger);
        }
    }

    public void RemovePassenger(Pedestrian passenger)
    {
        passengers.Remove(passenger);
    }

    public int GetPassengerCount() => passengers.Count;
    public int GetTramNumber() => tramNumber;
    public MetroStation GetCurrentStation() => currentStation;
    
    public bool IsStopped() => isStopped;
    
    public void SetDriver(PlayerController driver)
    {
        
    }
}
