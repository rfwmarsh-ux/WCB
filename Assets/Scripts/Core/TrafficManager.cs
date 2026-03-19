using UnityEngine;
using System.Collections.Generic;

public enum DestinationType
{
    CarPark,
    Residential,
    ShoppingCentre,
    Business,
    BusStation,
    Hospital,
    Random
}

public class TrafficManager : MonoBehaviour
{
    public static TrafficManager Instance { get; private set; }

    private List<TrafficVehicle> activeVehicles = new List<TrafficVehicle>();
    private int maxDayVehicles = 120;
    private int maxNightVehicles = 40;
    private int maxRushHourVehicles = 180;

    private float spawnTimer = 0f;
    private float spawnInterval = 0.8f;

    private List<Vector2> carParks = new List<Vector2>();
    private List<Vector2> residentialAreas = new List<Vector2>();
    private List<Vector2> shoppingCentres = new List<Vector2>();
    private List<Vector2> businessAreas = new List<Vector2>();
    
    private float rushHourMorningStart = 7f;
    private float rushHourMorningEnd = 9f;
    private float rushHourEveningStart = 16f;
    private float rushHourEveningEnd = 18f;

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
        InitializeDestinations();
    }

    private void Update()
    {
        UpdateTrafficSpawning();
        UpdateVehicles();
    }

    private void InitializeDestinations()
    {
        carParks.Add(new Vector2(200, 400));
        carParks.Add(new Vector2(500, 500));
        carParks.Add(new Vector2(700, 400));
        carParks.Add(new Vector2(300, 600));
        carParks.Add(new Vector2(600, 700));

        residentialAreas.Add(new Vector2(440, 620));
        residentialAreas.Add(new Vector2(360, 610));
        residentialAreas.Add(new Vector2(700, 560));
        residentialAreas.Add(new Vector2(640, 750));
        residentialAreas.Add(new Vector2(200, 660));

        shoppingCentres.Add(new Vector2(500, 500));
        shoppingCentres.Add(new Vector2(150, 450));
        shoppingCentres.Add(new Vector2(750, 600));
        shoppingCentres.Add(new Vector2(620, 700));

        businessAreas.Add(new Vector2(480, 520));
        businessAreas.Add(new Vector2(520, 480));
        businessAreas.Add(new Vector2(820, 510));
        businessAreas.Add(new Vector2(300, 350));
    }

    private void UpdateTrafficSpawning()
    {
        int maxVehicles = GetMaxVehiclesForTime();
        float currentInterval = GetSpawnIntervalForTime();

        if (activeVehicles.Count >= maxVehicles) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer < currentInterval) return;
        spawnTimer = 0f;

        TrySpawnTrafficVehicle();
    }

    private int GetMaxVehiclesForTime()
    {
        if (DayNightCycleManager.Instance != null)
        {
            float time = DayNightCycleManager.Instance.GetCurrentTime();
            
            if (time < 6f || time > 22f)
            {
                return maxNightVehicles;
            }
            
            if ((time >= rushHourMorningStart && time <= rushHourMorningEnd) ||
                (time >= rushHourEveningStart && time <= rushHourEveningEnd))
            {
                return maxRushHourVehicles;
            }
            
            if (time >= 10f && time <= 16f)
            {
                return maxDayVehicles;
            }
            
            return Mathf.RoundToInt(maxDayVehicles * 0.7f);
        }
        return maxDayVehicles;
    }
    
    private float GetSpawnIntervalForTime()
    {
        if (DayNightCycleManager.Instance != null)
        {
            float time = DayNightCycleManager.Instance.GetCurrentTime();
            
            if ((time >= rushHourMorningStart && time <= rushHourMorningEnd) ||
                (time >= rushHourEveningStart && time <= rushHourEveningEnd))
            {
                return 0.5f;
            }
            
            if (time >= 10f && time <= 16f)
            {
                return 1.0f;
            }
            
            if (time >= 6f && time <= 22f)
            {
                return 2.0f;
            }
            
            return 4f;
        }
        return spawnInterval;
    }

    private void TrySpawnTrafficVehicle()
    {
        float rand = Random.value;
        VehicleType type;

        if (rand < 0.15f)
        {
            type = VehicleType.Bus;
            SpawnBus();
        }
        else if (rand < 0.3f)
        {
            type = VehicleType.TaxiCab;
            SpawnCar(type);
        }
        else if (rand < 0.45f)
        {
            type = VehicleType.SlowVan;
            SpawnCar(type);
        }
        else
        {
            type = (VehicleType)Random.Range((int)VehicleType.CompactCar, (int)VehicleType.SportsCar + 1);
            SpawnCar(type);
        }
    }

    private void SpawnBus()
    {
        BusManager busManager = FindObjectOfType<BusManager>();
        if (busManager != null) return;

        Vector2 startPos = new Vector2(480, 520);
        TrafficVehicle bus = CreateTrafficVehicle(VehicleType.Bus, startPos);
        bus.SetDestination(new Vector2(200, 650), DestinationType.BusStation);
        activeVehicles.Add(bus);
    }

    private void SpawnCar(VehicleType type)
    {
        Vector2 startPos = GetRandomEdgePosition();
        DestinationType destType = (DestinationType)Random.Range(0, 5);
        Vector2 destination = GetRandomDestination(destType);

        TrafficVehicle car = CreateTrafficVehicle(type, startPos);
        car.SetDestination(destination, destType);
        activeVehicles.Add(car);
    }

    private Vector2 GetRandomEdgePosition()
    {
        int edge = Random.Range(0, 4);
        switch (edge)
        {
            case 0: return new Vector2(150, Random.Range(200f, 800f));
            case 1: return new Vector2(850, Random.Range(200f, 800f));
            case 2: return new Vector2(Random.Range(200f, 800f), 150);
            default: return new Vector2(Random.Range(200f, 800f), 850);
        }
    }
    
    public Vector2 GetRandomRoadPosition()
    {
        if (MapSystem.Instance != null)
        {
            return MapSystem.Instance.GetRandomJunction();
        }
        return GetRandomEdgePosition();
    }
    
    public bool IsOnRoad(Vector2 position)
    {
        if (MapSystem.Instance != null)
        {
            return MapSystem.Instance.IsOnRoad(position);
        }
        return true;
    }

    private Vector2 GetRandomDestination(DestinationType type)
    {
        switch (type)
        {
            case DestinationType.CarPark:
                return carParks.Count > 0 ? carParks[Random.Range(0, carParks.Count)] : GetRandomEdgePosition();
            case DestinationType.Residential:
                return residentialAreas.Count > 0 ? residentialAreas[Random.Range(0, residentialAreas.Count)] : GetRandomEdgePosition();
            case DestinationType.ShoppingCentre:
                return shoppingCentres.Count > 0 ? shoppingCentres[Random.Range(0, shoppingCentres.Count)] : GetRandomEdgePosition();
            case DestinationType.Business:
                return businessAreas.Count > 0 ? businessAreas[Random.Range(0, businessAreas.Count)] : GetRandomEdgePosition();
            default:
                return GetRandomEdgePosition();
        }
    }

    private TrafficVehicle CreateTrafficVehicle(VehicleType type, Vector2 position)
    {
        Vector2 leftLanePos = GetUkLeftLanePosition(position);
        GameObject vehicleGO = new GameObject($"Traffic_{type}");
        vehicleGO.transform.position = (Vector3)leftLanePos;

        SpriteRenderer sr = vehicleGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.sortingOrder = 5;
        
        Vector2 size = GetVehicleSize(type);
        vehicleGO.transform.localScale = new Vector3(size.x, size.y, 1f);

        Color vehicleColor = GetVehicleColor(type);
        sr.color = vehicleColor;

        Rigidbody2D rb = vehicleGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;

        BoxCollider2D collider = vehicleGO.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        collider.isTrigger = false;

        TrafficVehicle trafficVehicle = vehicleGO.AddComponent<TrafficVehicle>();
        trafficVehicle.Initialize(type);

        AddDriverAndSound(vehicleGO, type);

        return trafficVehicle;
    }

    private Vector2 GetUkLeftLanePosition(Vector2 position)
    {
        if (MapSystem.Instance != null)
        {
            return MapSystem.Instance.GetLeftLanePosition(position);
        }
        return position;
    }

    private void AddDriverAndSound(GameObject vehicleGO, VehicleType type)
    {
        DriverBehavior driver = vehicleGO.AddComponent<DriverBehavior>();
        
        EngineSoundProfile profile = type switch
        {
            VehicleType.Motorcycle or VehicleType.SportBike or VehicleType.Scooter => EngineSoundProfile.Motorcycle,
            VehicleType.Bus => EngineSoundProfile.Bus,
            VehicleType.SlowTruck => EngineSoundProfile.Truck,
            VehicleType.SportsCar or VehicleType.SuperCar or VehicleType.MuscleCar or VehicleType.RallyCar or VehicleType.DriftCar => EngineSoundProfile.SportsCar,
            VehicleType.CompactCar or VehicleType.EconomyCar or VehicleType.Hatchback or VehicleType.ClassicCoupe => EngineSoundProfile.CompactCar,
            VehicleType.TaxiCab or VehicleType.SedanCar or VehicleType.LuxurySedan or VehicleType.Convertible or VehicleType.HotRod => EngineSoundProfile.Sedan,
            VehicleType.SUV or VehicleType.PickupTruck or VehicleType.OffroadVehicle => EngineSoundProfile.SUV,
            VehicleType.SlowVan or VehicleType.MediumVan or VehicleType.FastVan or VehicleType.Van => EngineSoundProfile.Van,
            VehicleType.ArmoredCar => EngineSoundProfile.Armored,
            VehicleType.PoliceCruiser or VehicleType.Ambulance => EngineSoundProfile.Emergency,
            _ => EngineSoundProfile.Sedan
        };

        VehicleSoundSystem soundSystem = vehicleGO.AddComponent<VehicleSoundSystem>();
        soundSystem.engineProfile = profile;
    }

    private Color GetVehicleColor(VehicleType type)
    {
        switch (type)
        {
            case VehicleType.Bus: return new Color(0.2f, 0.4f, 0.8f);
            case VehicleType.TaxiCab: return new Color(1f, 1f, 0f);
            case VehicleType.SlowVan: return new Color(0.7f, 0.7f, 0.7f);
            default: return new Color(Random.value, Random.value, Random.value);
        }
    }

    private Vector2 GetVehicleSize(VehicleType type)
    {
        // Real world proportions scaled to fit lane width (3.5f units = 3.65m)
        // Cars: ~4.5m long x 1.8m wide
        // Vans: ~5m long x 2m wide  
        // Buses: ~12m long x 2.5m wide
        // Trucks: ~8m long x 2.5m wide
        // Motorcycles: ~2m long x 0.7m wide
        
        switch (type)
        {
            // Trains/Metro
            case VehicleType.Train:
                return new Vector2(30f, 3f);
            case VehicleType.Metro:
                return new Vector2(18f, 2.8f);
            
            // Buses
            case VehicleType.Bus:
                return new Vector2(14f, 3f);
            
            // Trucks
            case VehicleType.SlowTruck:
                return new Vector2(12f, 3f);
            
            // Vans
            case VehicleType.SlowVan:
            case VehicleType.MediumVan:
            case VehicleType.FastVan:
            case VehicleType.Van:
                return new Vector2(6f, 2.8f);
            
            // Emergency
            case VehicleType.Ambulance:
                return new Vector2(6f, 2.8f);
            
            // Cars
            case VehicleType.CompactCar:
            case VehicleType.EconomyCar:
            case VehicleType.Hatchback:
                return new Vector2(4.5f, 2.2f);
            
            case VehicleType.TaxiCab:
            case VehicleType.SedanCar:
            case VehicleType.LuxurySedan:
                return new Vector2(5f, 2.4f);
            
            case VehicleType.ClassicCoupe:
            case VehicleType.MuscleCar:
            case VehicleType.SportsCar:
            case VehicleType.SuperCar:
            case VehicleType.Convertible:
            case VehicleType.DriftCar:
            case VehicleType.HotRod:
            case VehicleType.RallyCar:
                return new Vector2(4.8f, 2.3f);
            
            case VehicleType.PoliceCruiser:
                return new Vector2(5f, 2.5f);
            
            // SUVs and Trucks
            case VehicleType.SUV:
                return new Vector2(5f, 2.5f);
            
            case VehicleType.PickupTruck:
                return new Vector2(6f, 2.5f);
            
            case VehicleType.OffroadVehicle:
                return new Vector2(4.5f, 2.3f);
            
            // Armored
            case VehicleType.ArmoredCar:
                return new Vector2(6f, 2.8f);
            
            // Motorcycles
            case VehicleType.Motorcycle:
                return new Vector2(2.5f, 1f);
            
            case VehicleType.SportBike:
                return new Vector2(2.2f, 0.9f);
            
            case VehicleType.Scooter:
                return new Vector2(2f, 0.9f);
            
            default:
                return new Vector2(5f, 2.4f);
        }
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
    }
}

public class TrafficVehicle : MonoBehaviour
{
    private VehicleType vehicleType;
    private DestinationType destinationType;
    private Vector2 destination;
    private Vector2 currentWaypoint;
    private List<Vector2> waypoints = new List<Vector2>();
    private int currentWaypointIndex = 0;

    private float maxSpeed = 35f;
    private float currentSpeed = 0f;
    private float acceleration = 20f;
    private float deceleration = 40f;
    private bool hasReachedDestination = false;
    private float waitTimer = 0f;
    private float waitTime = 3f;
    private float stopTimer = 0f;
    private bool isStoppedForLight = false;
    private float stopDistance = 15f;
    private float collisionDistance = 8f;

    private Vector2 lastPosition;
    private Vector2 velocity;

    public void Initialize(VehicleType type)
    {
        vehicleType = type;
        
        switch (type)
        {
            case VehicleType.Bus:
                maxSpeed = 25f;
                stopDistance = 18f;
                collisionDistance = 12f;
                break;
            case VehicleType.SlowTruck:
                maxSpeed = 22f;
                stopDistance = 16f;
                collisionDistance = 10f;
                break;
            case VehicleType.Motorcycle:
            case VehicleType.SportBike:
            case VehicleType.Scooter:
                maxSpeed = 45f;
                stopDistance = 12f;
                collisionDistance = 6f;
                break;
            default:
                maxSpeed = 35f;
                stopDistance = 15f;
                collisionDistance = 8f;
                break;
        }
        
        currentSpeed = maxSpeed * 0.5f;
    }

    public void SetDestination(Vector2 dest, DestinationType type)
    {
        destination = dest;
        destinationType = type;
        hasReachedDestination = false;
        GenerateWaypoints();
    }

    private void GenerateWaypoints()
    {
        waypoints.Clear();
        
        Vector2 startPos = transform.position;
        Vector2 targetPos = destination;
        
        float startRoadY = GetNearestHorizontalRoad(startPos.y);
        float startRoadX = GetNearestVerticalRoad(startPos.x);
        
        float endRoadY = GetNearestHorizontalRoad(targetPos.y);
        float endRoadX = GetNearestVerticalRoad(target.x);
        
        Vector2 roadStart = GetUkLeftLanePosition(new Vector2(startPos.x, startRoadY));
        Vector2 roadEnd = GetUkLeftLanePosition(new Vector2(targetPos.x, endRoadY));
        
        waypoints.Add(startPos);
        
        if (Mathf.Abs(startPos.y - startRoadY) > 5f)
        {
            waypoints.Add(roadStart);
        }
        
        AddIntermediateWaypointsAlongRoad(roadStart.x, roadEnd.x, startRoadY);
        
        float verticalRoadX = GetNearestVerticalRoad((roadStart.x + roadEnd.x) / 2f);
        float verticalRoadX2 = GetNearestVerticalRoad((roadStart.x + roadEnd.x) / 4f);
        float verticalRoadX3 = GetNearestVerticalRoad((roadStart.x + roadEnd.x) * 3f / 4f);
        
        waypoints.Add(GetUkLeftLanePosition(new Vector2(verticalRoadX2, startRoadY)));
        waypoints.Add(GetUkLeftLanePosition(new Vector2(verticalRoadX2, endRoadY)));
        waypoints.Add(GetUkLeftLanePosition(new Vector2(verticalRoadX3, endRoadY)));
        waypoints.Add(GetUkLeftLanePosition(new Vector2(verticalRoadX, endRoadY)));
        
        if (roadEnd.x < targetPos.x)
        {
            AddIntermediateWaypointsAlongRoad(roadEnd.x, targetPos.x, endRoadY);
        }
        else
        {
            AddIntermediateWaypointsAlongRoad(roadEnd.x, targetPos.x, endRoadY);
        }
        
        if (Mathf.Abs(targetPos.y - endRoadY) > 5f)
        {
            waypoints.Add(roadEnd);
        }
        
        waypoints.Add(targetPos);
        
        currentWaypointIndex = 0;
        if (waypoints.Count > 0)
        {
            currentWaypoint = waypoints[0];
            SetInitialRotation();
        }
    }

    private void SetInitialRotation()
    {
        if (waypoints.Count == 0) return;
        
        Vector2 target = waypoints[0];
        Vector2 direction = target - (Vector2)transform.position;
        if (direction.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    
    private void AddIntermediateWaypointsAlongRoad(float startX, float endX, float y)
    {
        float step = 100f;
        float direction = endX > startX ? 1f : -1f;
        float currentX = startX + direction * 80f;
        
        while ((direction > 0 && currentX < endX - 50f) || (direction < 0 && currentX > endX + 50f))
        {
            waypoints.Add(new Vector2(currentX, y));
            currentX += direction * step;
        }
    }

    private Vector2 GetUkLeftLaneWaypoint(float x, float y)
    {
        if (MapSystem.Instance != null)
        {
            return MapSystem.Instance.GetLeftLanePosition(new Vector2(x, y));
        }
        return new Vector2(x, y);
    }

    private float GetNearestHorizontalRoad(float y)
    {
        if (MapSystem.Instance != null)
        {
            return MapSystem.Instance.GetNearestHorizontalRoad(y);
        }
        
        float[] roadYs = { 200f, 300f, 400f, 500f, 600f, 700f, 800f };
        
        float nearest = roadYs[0];
        float minDist = Mathf.Abs(y - nearest);
        
        foreach (float roadY in roadYs)
        {
            float dist = Mathf.Abs(y - roadY);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = roadY;
            }
        }
        
        return nearest;
    }

    private float GetNearestVerticalRoad(float x)
    {
        if (MapSystem.Instance != null)
        {
            return MapSystem.Instance.GetNearestVerticalRoad(x);
        }
        
        float[] roadXs = { 100f, 200f, 300f, 400f, 500f, 600f, 700f, 800f };
        
        float nearest = roadXs[0];
        float minDist = Mathf.Abs(x - nearest);
        
        foreach (float roadX in roadXs)
        {
            float dist = Mathf.Abs(x - roadX);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = roadX;
            }
        }
        
        return nearest;
    }

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (DayNightCycleManager.Instance != null)
        {
            float time = DayNightCycleManager.Instance.GetCurrentTime();
            if (time < 6f || time > 20f)
            {
                if (Random.value < 0.95f) return;
            }
        }

        if (isStoppedForLight)
        {
            stopTimer -= Time.deltaTime;
            if (stopTimer <= 0)
            {
                isStoppedForLight = false;
            }
            return;
        }

        if (hasReachedDestination)
        {
            waitTimer += Time.deltaTime;
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
            ApplyMovement();
            
            if (waitTimer >= waitTime)
            {
                waitTimer = 0f;
                Destroy(gameObject);
            }
            return;
        }

        UpdateBehavior();
    }

    private void UpdateBehavior()
    {
        if (isBeingStolen)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * 3f * Time.deltaTime);
            ApplyMovement();
            lastPosition = transform.position;
            return;
        }
        
        UpdateErraticBehavior();
        
        if (waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count)
        {
            hasReachedDestination = true;
            return;
        }

        if (IsOnOneWayStreetWrongWay())
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
            ApplyMovement();
            lastPosition = transform.position;
            return;
        }

        if (currentWaypointIndex < 0 || currentWaypointIndex >= waypoints.Count)
        {
            hasReachedDestination = true;
            return;
        }

        Vector2 targetWaypoint = waypoints[currentWaypointIndex];
        if (targetWaypoint.x < 100 || targetWaypoint.x > 800 || targetWaypoint.y < 150 || targetWaypoint.y > 850)
        {
            currentWaypoint = targetWaypoint;
        }
        else
        {
            currentWaypoint = GetUkLeftLaneWaypoint(targetWaypoint.x, targetWaypoint.y);
        }
        
        Vector2 toWaypoint = currentWaypoint - (Vector2)transform.position;
        float distToWaypoint = toWaypoint.magnitude;
        
        if (distToWaypoint < 10f)
        {
            currentWaypointIndex++;
            
            if (currentWaypointIndex >= waypoints.Count)
            {
                hasReachedDestination = true;
                return;
            }
            
            targetWaypoint = waypoints[currentWaypointIndex];
            if (targetWaypoint.x < 100 || targetWaypoint.x > 800 || targetWaypoint.y < 150 || targetWaypoint.y > 850)
            {
                currentWaypoint = targetWaypoint;
            }
            else
            {
                currentWaypoint = GetUkLeftLaneWaypoint(targetWaypoint.x, targetWaypoint.y);
            }
            toWaypoint = currentWaypoint - (Vector2)transform.position;
            distToWaypoint = toWaypoint.magnitude;
        }
        
        CheckTrafficLights();
        
        bool shouldStop = ShouldStopForTraffic() || ShouldStopForZebraCrossing();
        
        if (shouldStop)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }
        else if (ShouldSlowForJunction())
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed * 0.5f, deceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        }
        
        ApplyMovement();
        
        lastPosition = transform.position;
        isAtZebraCrossing = false;
    }

    private bool IsOnOneWayStreetWrongWay()
    {
        if (MapSystem.Instance == null) return false;
        
        Vector2 currentPos = transform.position;
        Vector2 direction = transform.up;
        
        RoadSegment segment = MapSystem.Instance.GetRoadSegmentAtPosition(currentPos);
        if (segment == null || segment.direction == RoadDirection.TwoWay) return false;
        
        bool goingWrongWay = false;
        
        if (segment.IsHorizontal())
        {
            if (segment.direction == RoadDirection.OneWayEast && direction.x < -0.3f)
                goingWrongWay = true;
            if (segment.direction == RoadDirection.OneWayWest && direction.x > 0.3f)
                goingWrongWay = true;
        }
        else
        {
            if (segment.direction == RoadDirection.OneWayNorth && direction.y < -0.3f)
                goingWrongWay = true;
            if (segment.direction == RoadDirection.OneWaySouth && direction.y > 0.3f)
                goingWrongWay = true;
        }
        
        if (goingWrongWay)
        {
            Debug.Log($"[TrafficVehicle] {gameObject.name} going wrong way on one-way street!");
        }
        
        return goingWrongWay;
    }

    private bool ShouldStopForTraffic()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, stopDistance);
        
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.gameObject == gameObject) continue;
            if (hit.CompareTag("Player") || hit.CompareTag("Player2")) continue;
            
            TrafficVehicle otherVehicle = hit.GetComponent<TrafficVehicle>();
            if (otherVehicle != null)
            {
                Vector2 toOther = hit.transform.position - transform.position;
                Vector2 forward = transform.up;
                
                if (Vector2.Dot(toOther.normalized, forward) > 0.5f)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    private bool ShouldStopForZebraCrossing()
    {
        if (ZebraCrossingManager.Instance == null) return false;
        if (isFleeingErratically) return false;
        
        ZebraCrossing crossing = ZebraCrossingManager.Instance.GetNearestCrossing(transform.position, 15f);
        
        if (crossing == null) return false;
        
        nearestCrossing = crossing;
        isAtZebraCrossing = true;
        
        return ZebraCrossingProtocol.ShouldVehicleStop(crossing, false);
    }

    private bool ShouldSlowForJunction()
    {
        if (currentWaypointIndex < waypoints.Count - 1) return false;
        
        float distToDestination = Vector2.Distance(transform.position, destination);
        return distToDestination < 30f;
    }

    private void CheckTrafficLights()
    {
        if (isStoppedForLight) return;
        
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, stopDistance * 1.5f);
        
        foreach (Collider2D hit in hitColliders)
        {
            TrafficLight tl = hit.GetComponent<TrafficLight>();
            if (tl != null)
            {
                Vector2 toLight = hit.transform.position - transform.position;
                Vector2 forward = transform.up;
                float dotProduct = Vector2.Dot(toLight.normalized, forward);
                
                if (dotProduct > 0.3f && !tl.CanPass())
                {
                    float distToLight = toLight.magnitude;
                    if (distToLight < stopDistance)
                    {
                        isStoppedForLight = true;
                        stopTimer = 1f + Random.Range(0.5f, 2f);
                        currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime * 2f);
                        return;
                    }
                }
            }
        }
    }

    private void ApplyMovement()
    {
        Vector2 direction;
        
        if (isFleeingErratically)
        {
            direction = erraticDirection;
        }
        else
        {
            direction = ((Vector2)transform.position - lastPosition).normalized;
            if (direction.magnitude < 0.1f)
            {
                direction = ((currentWaypoint - (Vector2)transform.position).normalized);
            }
        }
        
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = transform.rotation.eulerAngles.z;
        
        float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
        float turnSpeed = 5f * (currentSpeed / maxSpeed);
        
        float newAngle = currentAngle + Mathf.Clamp(angleDiff, -turnSpeed * Time.deltaTime * 60f, turnSpeed * Time.deltaTime * 60f);
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
        
        direction = transform.up;
        velocity = direction * currentSpeed * Time.deltaTime;
        transform.position += (Vector3)velocity;
    }

    public float GetCurrentSpeed() => currentSpeed;
    public bool IsMoving() => currentSpeed > 1f;
    public Vector2 GetVelocity() => velocity;
    public float GetMaxHealth() => 100f;
    public float GetCurrentHealth() => currentHealth;
    public bool IsDestroyed() => hasReachedDestination && waitTimer > 0f;
    public bool IsErraticDriver() => isFleeingErratically;
    
    private bool isBeingStolen = false;
    private bool isFleeingErratically = false;
    private float erraticTimer = 0f;
    private Vector2 erraticDirection;
    private float originalMaxSpeed;
    private ZebraCrossing nearestCrossing;
    private bool isAtZebraCrossing = false;
    
    private float currentHealth = 100f;
    private float maxHealth = 100f;
    private GameObject smokeEffect;
    private GameObject fireEffect;
    private bool isSmoking = false;
    private bool isOnFire = false;
    private float smokeTimer = 0f;
    private float maxSmokeDuration = 5f;
    private float maxFireDuration = 8f;
    
    public void SetBeingStolen(bool value)
    {
        isBeingStolen = value;
        if (value)
        {
            currentSpeed = 0f;
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateDamageEffects();
        
        if (currentHealth <= 0)
        {
            DestroyVehicle();
        }
    }
    
    private void UpdateDamageEffects()
    {
        float healthPercent = currentHealth / maxHealth;
        
        if (healthPercent <= 0.25f && !isSmoking)
        {
            StartSmokeEffect();
        }
        
        if (healthPercent <= 0.10f && !isOnFire)
        {
            StartFireEffect();
        }
        
        if (healthPercent <= 0.75f && healthPercent > 0.10f && currentSpeed > 0)
        {
            currentSpeed *= 0.95f;
        }
        
        if (healthPercent <= 0.10f && currentSpeed > 0)
        {
            currentSpeed *= 0.75f;
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
            main.startSize = 0.8f;
            main.startSpeed = 1.5f;
            main.startLifetime = 2f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.4f;
            
            var emission = particles.emission;
            emission.rateOverTime = 10f;
            
            var colorOverLifetime = particles.colorOverLifetime;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(0.3f, 0.3f, 0.3f), 0f), new GradientColorKey(new Color(0.2f, 0.2f, 0.2f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0f), new GradientAlphaKey(0.2f, 0.5f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = gradient;
        }
    }
    
    private void StartFireEffect()
    {
        if (fireEffect != null) return;
        
        isOnFire = true;
        
        fireEffect = new GameObject("FireEffect");
        fireEffect.transform.position = transform.position + Vector3.up * 1.5f;
        fireEffect.transform.parent = transform;
        
        ParticleSystem particles = fireEffect.AddComponent<ParticleSystem>();
        if (particles != null)
        {
            var main = particles.main;
            main.startColor = new Color(1f, 0.4f, 0f, 0.8f);
            main.startSize = 0.6f;
            main.startSpeed = 2f;
            main.startLifetime = 1f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.3f;
            
            var emission = particles.emission;
            emission.rateOverTime = 15f;
        }
    }
    
    private void StopAllEffects()
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
        
        isSmoking = false;
        isOnFire = false;
    }
    
    private void DestroyVehicle()
    {
        StopAllEffects();
        Destroy(gameObject, 0.1f);
    }
    
    public bool IsBeingStolen()
    {
        return isBeingStolen;
    }
    
    public void SlowDownForPlayer()
    {
        if (isFleeingErratically || isBeingStolen) return;
        
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed * 0.3f, deceleration * Time.deltaTime * 2f);
    }
    
    public void StartErraticFlee()
    {
        if (isFleeingErratically || isBeingStolen) return;
        
        isFleeingErratically = true;
        erraticTimer = Random.Range(3f, 6f);
        erraticDirection = GetRandomErraticDirection();
        originalMaxSpeed = maxSpeed;
        maxSpeed *= 1.5f;
        
        Debug.Log("Driver is fleeing erratically!");
    }
    
    private Vector2 GetRandomErraticDirection()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
    
    public void StopVehicle()
    {
        currentSpeed = 0f;
    }
    
    private void UpdateErraticBehavior()
    {
        if (!isFleeingErratically) return;
        
        erraticTimer -= Time.deltaTime;
        
        if (erraticTimer <= 0)
        {
            StopErraticFlee();
            return;
        }
        
        erraticDirection = GetRandomErraticDirection();
        
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * 2f * Time.deltaTime);
    }
    
    private void StopErraticFlee()
    {
        isFleeingErratically = false;
        maxSpeed = originalMaxSpeed;
    }
}

public class TrafficVehicleCollision : MonoBehaviour
{
    private TrafficVehicle vehicle;
    private float collisionCooldown = 0f;
    
    private void Start()
    {
        vehicle = GetComponent<TrafficVehicle>();
    }
    
    private void Update()
    {
        if (collisionCooldown > 0)
        {
            collisionCooldown -= Time.deltaTime;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionCooldown > 0) return;
        
        float impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed < 5f) return;
        
        collisionCooldown = 0.5f;
        
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Player2"))
        {
            ApplyDamageToPlayer(collision.gameObject, impactSpeed);
        }
        
        if (vehicle != null && impactSpeed > 10f)
        {
            float vehicleDamage = impactSpeed * 0.5f;
            vehicle.TakeDamage(vehicleDamage);
        }
    }
    
    private void ApplyDamageToPlayer(GameObject player, float impactSpeed)
    {
        float damage = impactSpeed * 2f;
        
        if (player.CompareTag("Player") && PlayerManager.Instance != null)
        {
            PlayerManager.Instance.TakeDamage(damage);
        }
        else if (player.CompareTag("Player2") && Player2Manager.Instance != null)
        {
            Player2Manager.Instance.TakeDamage(damage);
        }
    }
}
