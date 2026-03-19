using UnityEngine;
using System.Collections.Generic;

public class PoliceStationManager : MonoBehaviour
{
    public static PoliceStationManager Instance { get; private set; }

    private List<PoliceStation> policeStations = new List<PoliceStation>();

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
        InitializePoliceStations();
    }

    private void InitializePoliceStations()
    {
        SpawnPoliceStation("Wolverhampton Central Police Station", new Vector2(480f, 510f), true);
        SpawnPoliceStation("Wednesfield Police Station", new Vector2(720f, 420f));
        SpawnPoliceStation("Bilston Police Station", new Vector2(280f, 320f));
        SpawnPoliceStation("Tettenhall Police Station", new Vector2(620f, 720f));
        SpawnPoliceStation("Heath Town Police Station", new Vector2(550f, 750f));
        SpawnPoliceStation("Low Hill Police Station", new Vector2(400f, 650f));

        Debug.Log($"Initialized {policeStations.Count} police stations");
    }

    private void SpawnPoliceStation(string name, Vector2 position, bool isMainStation = false)
    {
        GameObject stationGO = new GameObject($"PoliceStation_{name}");
        stationGO.transform.position = (Vector3)position;
        stationGO.tag = "PoliceStation";

        SpriteRenderer spriteRenderer = stationGO.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
        spriteRenderer.color = new Color(0.1f, 0.2f, 0.5f, 1f);
        spriteRenderer.sortingOrder = 2;

        BoxCollider2D collider = stationGO.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(15f, 15f);
        collider.isTrigger = true;

        PoliceStation station = stationGO.AddComponent<PoliceStation>();
        station.Initialize(name, position, isMainStation);

        stationGO.transform.parent = transform;
        policeStations.Add(station);
    }

    public List<PoliceStation> GetAllStations() => policeStations;

    public PoliceStation GetClosestStation(Vector2 position)
    {
        if (policeStations.Count == 0) return null;

        PoliceStation closest = policeStations[0];
        float minDist = Vector2.Distance(position, closest.Position);

        foreach (var station in policeStations)
        {
            float dist = Vector2.Distance(position, station.Position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = station;
            }
        }

        return closest;
    }

    public PoliceStation GetRandomStation()
    {
        if (policeStations.Count == 0) return null;
        return policeStations[Random.Range(0, policeStations.Count)];
    }
}

public class PoliceStation : MonoBehaviour
{
    public string StationName { get; private set; }
    public Vector2 Position { get; private set; }
    public bool IsMainStation { get; private set; }
    public List<Vector2> CarParkPositions { get; private set; } = new List<Vector2>();

    public void Initialize(string name, Vector2 position, bool isMainStation = false)
    {
        StationName = name;
        Position = position;
        IsMainStation = isMainStation;

        CreateBuilding();
        CreateCarPark();
    }

    private void CreateBuilding()
    {
        GameObject building = new GameObject("Building");
        building.transform.position = transform.position;
        building.transform.parent = transform;

        SpriteRenderer sr = building.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.15f, 0.25f, 0.55f, 1f);
        sr.sortingOrder = 3;
        building.transform.localScale = new Vector3(12f, 10f, 1f);

        GameObject roof = new GameObject("Roof");
        roof.transform.position = transform.position + Vector3.up * 5f;
        roof.transform.parent = transform;

        SpriteRenderer roofSr = roof.AddComponent<SpriteRenderer>();
        roofSr.sprite = SpriteHelper.GetDefaultSprite();
        roofSr.color = new Color(0.1f, 0.15f, 0.35f, 1f);
        roofSr.sortingOrder = 3;
        roof.transform.localScale = new Vector3(8f, 3f, 1f);

        for (int i = 0; i < 3; i++)
        {
            GameObject light = new GameObject($"Light_{i}");
            light.transform.position = transform.position + new Vector3(-3f + i * 3f, 6f, 0);
            light.transform.parent = transform;

            SpriteRenderer lightSr = light.AddComponent<SpriteRenderer>();
            lightSr.sprite = SpriteHelper.GetDefaultSprite();
            lightSr.color = new Color(1f, 0.9f, 0.3f, 0.8f);
            lightSr.sortingOrder = 4;
            light.transform.localScale = new Vector3(1.5f, 0.5f, 1f);
        }
    }

    private void CreateCarPark()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 offset = new Vector2(-10f + i * 7f, -12f);
            CarParkPositions.Add(Position + offset);

            GameObject parkingSpot = new GameObject($"Parking_{i}");
            parkingSpot.transform.position = (Vector3)(Position + offset);
            parkingSpot.transform.parent = transform;

            SpriteRenderer sr = parkingSpot.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            sr.sortingOrder = 1;
            parkingSpot.transform.localScale = new Vector3(5f, 3f, 1f);
        }
    }
}
