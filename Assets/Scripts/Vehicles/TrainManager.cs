using UnityEngine;
using System.Collections.Generic;

public class TrainManager : MonoBehaviour
{
    public static TrainManager Instance { get; private set; }

    [Header("Train Settings")]
    [SerializeField] private int numberOfTrains = 4;
    [SerializeField] private float trainSpeed = 25f;
    [SerializeField] private int trainCapacity = 50;

    private List<Train> activeTrains = new List<Train>();
    private List<TrainStation> trainStations = new List<TrainStation>();
    private List<List<Vector2>> railwayLines = new List<List<Vector2>>();

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
        CreateRailwayLines();
        CreateTunnelEntrances();
        CreateTrainStation();
        SpawnTrains();
    }

    private void CreateRailwayLines()
    {
        railwayLines.Add(new List<Vector2>
        {
            new Vector2(460, 550),
            new Vector2(450, 600),
            new Vector2(420, 700),
            new Vector2(380, 800)
        });

        railwayLines.Add(new List<Vector2>
        {
            new Vector2(460, 550),
            new Vector2(400, 520),
            new Vector2(300, 480),
            new Vector2(200, 450)
        });

        railwayLines.Add(new List<Vector2>
        {
            new Vector2(460, 550),
            new Vector2(500, 480),
            new Vector2(580, 400),
            new Vector2(700, 300)
        });

        railwayLines.Add(new List<Vector2>
        {
            new Vector2(460, 550),
            new Vector2(550, 580),
            new Vector2(680, 650),
            new Vector2(800, 750)
        });

        foreach (var line in railwayLines)
        {
            CreateRailwayTrack(line);
        }

        Debug.Log($"Created {railwayLines.Count} railway lines");
    }

    private void CreateRailwayTrack(List<Vector2> points)
    {
        GameObject track = new GameObject("RailwayTrack");
        
        LineRenderer lr = track.AddComponent<LineRenderer>();
        lr.positionCount = points.Count;
        
        for (int i = 0; i < points.Count; i++)
        {
            lr.SetPosition(i, new Vector3(points[i].x, points[i].y, 0.3f));
        }
        
        lr.startWidth = 2f;
        lr.endWidth = 2f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        lr.endColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        track.transform.parent = transform;
    }

    private void CreateTunnelEntrances()
    {
        float mapMin = 50f;
        float mapMax = 950f;
        
        foreach (var line in railwayLines)
        {
            if (line.Count > 0)
            {
                Vector2 startTunnel = new Vector2(mapMin, line[0].y);
                CreateTunnel(startTunnel, true);
                
                Vector2 endTunnel = new Vector2(mapMax, line[line.Count - 1].y);
                CreateTunnel(endTunnel, false);
                
                line[0] = startTunnel;
                line[line.Count - 1] = endTunnel;
            }
        }
        
        CreateAllTunnelVisuals();
    }

    private void CreateTunnel(Vector2 position, bool isEntrance)
    {
        GameObject tunnel = new GameObject(isEntrance ? "TunnelEntrance" : "TunnelExit");
        tunnel.transform.position = (Vector3)position;
        
        SpriteRenderer sr = tunnel.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        sr.sortingOrder = -2;
        tunnel.transform.localScale = new Vector3(12f, 10f, 1f);
        
        CircleCollider2D col = tunnel.AddComponent<CircleCollider2D>();
        col.radius = 5f;
        col.isTrigger = true;
        
        tunnel.AddComponent<TunnelEntrance>();
        
        tunnel.transform.parent = transform;
    }
    
    private void CreateAllTunnelVisuals()
    {
        foreach (var line in railwayLines)
        {
            if (line.Count >= 2)
            {
                CreateTunnelTrackVisuals(line[0], line[1]);
                CreateTunnelTrackVisuals(line[line.Count - 2], line[line.Count - 1]);
            }
        }
    }
    
    private void CreateTunnelTrackVisuals(Vector2 from, Vector2 to)
    {
        GameObject tunnelTrack = new GameObject("TunnelTrack");
        
        LineRenderer lr = tunnelTrack.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, new Vector3(from.x, from.y, 0.2f));
        lr.SetPosition(1, new Vector3(to.x, to.y, 0.2f));
        
        lr.startWidth = 3f;
        lr.endWidth = 3f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        lr.endColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        
        tunnelTrack.transform.parent = transform;
    }

    private void CreateTrainStation()
    {
        GameObject station = new GameObject("Wolverhampton Railway Station");
        station.transform.position = new Vector3(460, 550, 0);
        
        SpriteRenderer sr = station.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.6f, 0.3f, 0.1f, 1f);
        sr.sortingOrder = 3;
        station.transform.localScale = new Vector3(30f, 12f, 1f);
        
        CircleCollider2D collider = station.AddComponent<CircleCollider2D>();
        collider.radius = 8f;
        collider.isTrigger = true;
        
        TrainStation trainStation = station.AddComponent<TrainStation>();
        trainStation.Initialize("Wolverhampton Railway Station", new Vector2(460, 550), true);
        
        station.transform.parent = transform;
        trainStations.Add(trainStation);
        
        Debug.Log("Created Wolverhampton Railway Station");
    }

    private void SpawnTrains()
    {
        for (int i = 0; i < numberOfTrains; i++)
        {
            SpawnTrain(i);
        }
        Debug.Log($"Spawned {activeTrains.Count} trains");
    }

    private void SpawnTrain(int index)
    {
        int lineIndex = index % railwayLines.Count;
        List<Vector2> route = railwayLines[lineIndex];
        
        GameObject trainGO = new GameObject($"Train_{index}");
        
        SpriteRenderer sr = trainGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        sr.sortingOrder = 4;
        trainGO.transform.localScale = new Vector3(14f, 3.5f, 1f);
        
        CircleCollider2D collider = trainGO.AddComponent<CircleCollider2D>();
        collider.radius = 3f;
        
        Rigidbody2D rb = trainGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;
        
        Train train = trainGO.AddComponent<Train>();
        train.Initialize(trainCapacity, trainSpeed, route, index);
        
        trainGO.transform.parent = transform;
        activeTrains.Add(train);
    }

    public List<TrainStation> GetAllStations() => trainStations;
    public List<List<Vector2>> GetRailwayLines() => railwayLines;
}

public class TrainStation : MonoBehaviour
{
    public string StationName { get; private set; }
    public Vector2 Position { get; private set; }
    public bool IsMainStation { get; private set; }

    private List<Pedestrian> waitingPassengers = new List<Pedestrian>();
    private int maxWaiting = 20;

    public void Initialize(string name, Vector2 position, bool isMain)
    {
        StationName = name;
        Position = position;
        IsMainStation = isMain;
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

public class TunnelEntrance : MonoBehaviour
{
}
