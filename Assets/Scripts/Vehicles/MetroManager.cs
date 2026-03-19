using UnityEngine;
using System.Collections.Generic;

public class MetroManager : MonoBehaviour
{
    public static MetroManager Instance { get; private set; }

    [Header("Metro Settings")]
    [SerializeField] private int numberOfTrams = 2;
    [SerializeField] private float metroSpeed = 15f;
    [SerializeField] private int tramCapacity = 30;

    private List<MetroTram> activeTrams = new List<MetroTram>();
    private List<MetroStation> metroStations = new List<MetroStation>();

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
        CreateMetroTrack();
        CreateMetroStations();
        SpawnTrams();
    }

    private void CreateMetroTrack()
    {
        float mapMin = 50f;
        float mapMax = 950f;
        
        GameObject track = new GameObject("MetroTrack");
        
        LineRenderer lr = track.AddComponent<LineRenderer>();
        lr.positionCount = 5;
        lr.SetPosition(0, new Vector3(mapMin, 510, 0.5f));
        lr.SetPosition(1, new Vector3(470, 540, 0.5f));
        lr.SetPosition(2, new Vector3(490, 510, 0.5f));
        lr.SetPosition(3, new Vector3(510, 480, 0.5f));
        lr.SetPosition(4, new Vector3(mapMax, 450, 0.5f));
        
        lr.startWidth = 2f;
        lr.endWidth = 2f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(0.2f, 0.6f, 0.9f, 0.6f);
        lr.endColor = new Color(0.2f, 0.6f, 0.9f, 0.6f);
        
        track.transform.parent = transform;
        
        CreateMetroTunnels(mapMin, mapMax);
        Debug.Log("Created Metro track line with tunnels");
    }
    
    private void CreateMetroTunnels(float mapMin, float mapMax)
    {
        CreateMetroTunnel(new Vector2(mapMin, 510), true);
        CreateMetroTunnel(new Vector2(mapMax, 450), false);
        
        GameObject tunnel1 = new GameObject("MetroTunnelLeft");
        tunnel1.transform.position = new Vector3(mapMin - 5f, 510, 0);
        SpriteRenderer sr1 = tunnel1.AddComponent<SpriteRenderer>();
        sr1.sprite = SpriteHelper.GetDefaultSprite();
        sr1.color = new Color(0.1f, 0.1f, 0.12f, 1f);
        sr1.sortingOrder = -2;
        tunnel1.transform.localScale = new Vector3(10f, 6f, 1f);
        tunnel1.transform.parent = transform;
        
        GameObject tunnel2 = new GameObject("MetroTunnelRight");
        tunnel2.transform.position = new Vector3(mapMax + 5f, 450, 0);
        SpriteRenderer sr2 = tunnel2.AddComponent<SpriteRenderer>();
        sr2.sprite = SpriteHelper.GetDefaultSprite();
        sr2.color = new Color(0.1f, 0.1f, 0.12f, 1f);
        sr2.sortingOrder = -2;
        tunnel2.transform.localScale = new Vector3(10f, 6f, 1f);
        tunnel2.transform.parent = transform;
    }
    
    private void CreateMetroTunnel(Vector2 position, bool isEntrance)
    {
        GameObject tunnel = new GameObject(isEntrance ? "MetroTunnelEntrance" : "MetroTunnelExit");
        tunnel.transform.position = (Vector3)position;
        
        SpriteRenderer sr = tunnel.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.1f, 0.1f, 0.12f, 1f);
        sr.sortingOrder = -2;
        tunnel.transform.localScale = new Vector3(8f, 6f, 1f);
        
        CircleCollider2D col = tunnel.AddComponent<CircleCollider2D>();
        col.radius = 4f;
        col.isTrigger = true;
        
        tunnel.transform.parent = transform;
    }

    private void CreateMetroStations()
    {
        CreateMetroStation("Wolverhampton Station", new Vector2(470, 540), true, false);
        CreateMetroStation("Pipers Row", new Vector2(490, 510), false, false);
        CreateMetroStation("Wolverhampton St George's", new Vector2(510, 480), false, true);

        Debug.Log($"Created {metroStations.Count} Metro stations");
    }

    private void CreateMetroStation(string name, Vector2 position, bool isTerminus = false, bool isInterchange = false)
    {
        GameObject station = new GameObject(name);
        station.transform.position = (Vector3)position;

        SpriteRenderer sr = station.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = isTerminus ? new Color(0.8f, 0.2f, 0.2f, 1f) : new Color(0.2f, 0.8f, 0.4f, 1f);
        sr.sortingOrder = 3;

        float width = isTerminus ? 18f : 14f;
        station.transform.localScale = new Vector3(width, 8f, 1f);

        CircleCollider2D collider = station.AddComponent<CircleCollider2D>();
        collider.radius = 6f;
        collider.isTrigger = true;

        MetroStation metroStation = station.AddComponent<MetroStation>();
        metroStation.Initialize(name, position, isTerminus, isInterchange);

        station.transform.parent = transform;
        metroStations.Add(metroStation);
    }

    private void SpawnTrams()
    {
        for (int i = 0; i < numberOfTrams; i++)
        {
            SpawnTram(i);
        }
        Debug.Log($"Spawned {activeTrams.Count} Metro trams");
    }

    private void SpawnTram(int index)
    {
        GameObject tramGO = new GameObject($"MetroTram_{index}");
        
        SpriteRenderer sr = tramGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.2f, 0.6f, 0.9f, 1f);
        sr.sortingOrder = 4;
        tramGO.transform.localScale = new Vector3(10f, 3f, 1f);

        CircleCollider2D collider = tramGO.AddComponent<CircleCollider2D>();
        collider.radius = 2f;

        Rigidbody2D rb = tramGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;

        MetroTram tram = tramGO.AddComponent<MetroTram>();
        tram.Initialize(tramCapacity, metroSpeed, metroStations, index);

        tramGO.transform.parent = transform;
        activeTrams.Add(tram);
    }

    public List<MetroStation> GetAllStations() => metroStations;
    public MetroStation GetNearestStation(Vector2 position)
    {
        MetroStation nearest = null;
        float minDist = float.MaxValue;

        foreach (var station in metroStations)
        {
            float dist = Vector2.Distance(position, station.Position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = station;
            }
        }
        return nearest;
    }

    public List<MetroTram> GetActiveTrams() => activeTrams;
}

public class MetroStation : MonoBehaviour
{
    public string StationName { get; private set; }
    public Vector2 Position { get; private set; }
    public bool IsTerminus { get; private set; }
    public bool IsInterchange { get; private set; }

    private List<Pedestrian> waitingPassengers = new List<Pedestrian>();
    private int maxWaiting = 15;

    public void Initialize(string name, Vector2 position, bool isTerminus, bool isInterchange)
    {
        StationName = name;
        Position = position;
        IsTerminus = isTerminus;
        IsInterchange = isInterchange;
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
