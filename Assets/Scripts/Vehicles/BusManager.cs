using UnityEngine;
using System.Collections.Generic;

public class BusManager : MonoBehaviour
{
    public static BusManager Instance { get; private set; }

    [Header("Bus Settings")]
    [SerializeField] private int numberOfBuses = 3;
    [SerializeField] private float busSpeed = 8f;
    [SerializeField] private int busCapacity = 20;

    private List<Bus> activeBuses = new List<Bus>();
    private List<BusStop> busStops = new List<BusStop>();

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
        CreateBusStations();
        SpawnBuses();
    }

    private void CreateBusStations()
    {
        CreateBusStation("Wolverhampton Bus Station", new Vector2(480, 520), true);
        CreateBusStop("City Centre (Stop CV)", new Vector2(520, 480));
        CreateBusStop("Wulfrun Centre", new Vector2(500, 510));
        CreateBusStop("Dudley Road", new Vector2(350, 400));
        CreateBusStop("Weston Street", new Vector2(550, 450));
        CreateBusStop("Bilston", new Vector2(300, 300));
        CreateBusStop("Wednesfield", new Vector2(700, 400));
        CreateBusStop("Tettenhall", new Vector2(600, 700));
        CreateBusStop("Pendeford", new Vector2(750, 600));
        CreateBusStop("Bushbury", new Vector2(200, 650));
        CreateBusStop("Codsall", new Vector2(150, 750));
        CreateBusStop("Compton", new Vector2(700, 750));

        Debug.Log($"Created {busStops.Count} bus stops");
    }

    private void CreateBusStation(string name, Vector2 position, bool isMainStation)
    {
        GameObject station = new GameObject(name);
        station.transform.position = (Vector3)position;

        SpriteRenderer sr = station.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = isMainStation ? new Color(0.2f, 0.4f, 0.8f, 1f) : new Color(0.3f, 0.5f, 0.8f, 1f);
        sr.sortingOrder = 3;

        float size = isMainStation ? 20f : 12f;
        station.transform.localScale = new Vector3(size, size * 0.6f, 1f);

        CircleCollider2D collider = station.AddComponent<CircleCollider2D>();
        collider.radius = 8f;
        collider.isTrigger = true;

        BusStop stop = station.AddComponent<BusStop>();
        stop.Initialize(name, position, isMainStation, true);
        stop.SetColor(sr.color);

        station.transform.parent = transform;
        busStops.Add(stop);
    }

    private void CreateBusStop(string name, Vector2 position)
    {
        GameObject stop = new GameObject(name);
        stop.transform.position = (Vector3)position;

        SpriteRenderer sr = stop.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.4f, 0.6f, 0.9f, 1f);
        sr.sortingOrder = 2;
        stop.transform.localScale = new Vector3(8f, 6f, 1f);

        CircleCollider2D collider = stop.AddComponent<CircleCollider2D>();
        collider.radius = 5f;
        collider.isTrigger = true;

        BusStop busStop = stop.AddComponent<BusStop>();
        busStop.Initialize(name, position, false, false);
        busStop.SetColor(sr.color);

        stop.transform.parent = transform;
        busStops.Add(busStop);
    }

    private void SpawnBuses()
    {
        for (int i = 0; i < numberOfBuses; i++)
        {
            SpawnBus(i);
        }
        Debug.Log($"Spawned {activeBuses.Count} buses");
    }

    private void SpawnBus(int index)
    {
        GameObject busGO = new GameObject($"Bus_{index}");
        
        SpriteRenderer sr = busGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.9f, 0.8f, 0.2f, 1f);
        sr.sortingOrder = 4;
        busGO.transform.localScale = new Vector3(6f, 2.5f, 1f);

        CircleCollider2D collider = busGO.AddComponent<CircleCollider2D>();
        collider.radius = 1.5f;

        Rigidbody2D rb = busGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;

        Bus bus = busGO.AddComponent<Bus>();
        bus.Initialize(busCapacity, busSpeed, busStops, index);

        busGO.transform.parent = transform;
        activeBuses.Add(bus);
    }

    public List<BusStop> GetAllBusStops() => busStops;
    public BusStop GetNearestStop(Vector2 position)
    {
        BusStop nearest = null;
        float minDist = float.MaxValue;

        foreach (var stop in busStops)
        {
            float dist = Vector2.Distance(position, stop.Position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = stop;
            }
        }
        return nearest;
    }

    public List<Bus> GetActiveBuses() => activeBuses;
}

public class BusStop : MonoBehaviour
{
    public string StopName { get; private set; }
    public Vector2 Position { get; private set; }
    public bool IsMainStation { get; private set; }
    public bool HasWaitingArea { get; private set; }

    private List<Pedestrian> waitingPassengers = new List<Pedestrian>();
    private int maxWaiting = 10;
    private Color stopColor;

    public void Initialize(string name, Vector2 position, bool isMainStation, bool hasWaitingArea)
    {
        StopName = name;
        Position = position;
        IsMainStation = isMainStation;
        HasWaitingArea = hasWaitingArea;
    }

    public void SetColor(Color color)
    {
        stopColor = color;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = color;
    }

    public void AddPassenger(Pedestrian passenger)
    {
        if (waitingPassengers.Count < maxWaiting)
        {
            waitingPassengers.Add(passenger);
        }
    }

    public void RemovePassenger(Pedestrian passenger)
    {
        waitingPassengers.Remove(passenger);
    }

    public List<Pedestrian> GetWaitingPassengers() => waitingPassengers;
    public int GetWaitingCount() => waitingPassengers.Count;
    public bool HasPassengers() => waitingPassengers.Count > 0;
}
