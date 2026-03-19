using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a point of interest (shops, schools, universities, etc.)
/// </summary>
[System.Serializable]
public class PointOfInterest
{
    public string poiName;
    public Vector2 location;
    public float radius; // Area around POI
    public POIType type;
    public int targetPedestrianCount; // How many pedestrians should be here

    public enum POIType
    {
        Shop,
        ShoppingCentre,
        School,
        University,
        Park,
        Restaurant,
        Bar,
        Library,
        Residential,
        Street
    }

    public PointOfInterest(string name, Vector2 loc, float r, POIType t, int pedestrianCount)
    {
        poiName = name;
        location = loc;
        radius = r;
        type = t;
        targetPedestrianCount = pedestrianCount;
    }
}

/// <summary>
/// Manages spawning and managing pedestrians throughout the city
/// </summary>
public class PedestrianManager : MonoBehaviour
{
    [SerializeField] private int totalPedestrianCount = 200;
    [SerializeField] private float spawnUpdateInterval = 1.5f;

    private List<Pedestrian> activePedestrians = new List<Pedestrian>();
    private List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>();
    private float spawnUpdateTimer = 0f;

    private void Start()
    {
        InitializePointsOfInterest();
        SpawnInitialPedestrians();
    }

    private void Update()
    {
        spawnUpdateTimer += Time.deltaTime;
        if (spawnUpdateTimer >= spawnUpdateInterval)
        {
            UpdatePedestrianDistribution();
            spawnUpdateTimer = 0f;
        }
    }

    private void InitializePointsOfInterest()
    {
        // City Centre - Shopping district (busiest area)
        pointsOfInterest.Add(new PointOfInterest("Main Shopping Centre", new Vector2(500, 500), 80f, 
            PointOfInterest.POIType.ShoppingCentre, 15));
        pointsOfInterest.Add(new PointOfInterest("High Street", new Vector2(480, 520), 50f, 
            PointOfInterest.POIType.Shop, 8));
        pointsOfInterest.Add(new PointOfInterest("City Centre Park", new Vector2(520, 450), 60f, 
            PointOfInterest.POIType.Park, 8));

        // East Side - University district
        pointsOfInterest.Add(new PointOfInterest("Wolverhampton University", new Vector2(800, 500), 100f, 
            PointOfInterest.POIType.University, 20));
        pointsOfInterest.Add(new PointOfInterest("Campus Shops", new Vector2(820, 480), 50f, 
            PointOfInterest.POIType.Shop, 10));
        pointsOfInterest.Add(new PointOfInterest("Library", new Vector2(780, 520), 45f, 
            PointOfInterest.POIType.Library, 10));
        pointsOfInterest.Add(new PointOfInterest("Student Union", new Vector2(800, 530), 40f, 
            PointOfInterest.POIType.Bar, 6));

        // West End - School/shopping area
        pointsOfInterest.Add(new PointOfInterest("West End High School", new Vector2(200, 500), 90f, 
            PointOfInterest.POIType.School, 15));
        pointsOfInterest.Add(new PointOfInterest("West End Shopping", new Vector2(150, 480), 55f, 
            PointOfInterest.POIType.Shop, 8));
        pointsOfInterest.Add(new PointOfInterest("West End Park", new Vector2(220, 520), 60f, 
            PointOfInterest.POIType.Park, 6));
        pointsOfInterest.Add(new PointOfInterest("West End Pubs", new Vector2(180, 500), 40f, 
            PointOfInterest.POIType.Bar, 5));

        // Northside - Shopping/schools
        pointsOfInterest.Add(new PointOfInterest("Northside Shopping Complex", new Vector2(500, 800), 85f, 
            PointOfInterest.POIType.ShoppingCentre, 12));
        pointsOfInterest.Add(new PointOfInterest("Primary School", new Vector2(480, 820), 60f, 
            PointOfInterest.POIType.School, 10));
        pointsOfInterest.Add(new PointOfInterest("Shopping District", new Vector2(520, 790), 55f, 
            PointOfInterest.POIType.Shop, 7));

        // Southside - Shopping/restaurant area
        pointsOfInterest.Add(new PointOfInterest("South Mall", new Vector2(500, 200), 90f, 
            PointOfInterest.POIType.ShoppingCentre, 14));
        pointsOfInterest.Add(new PointOfInterest("Restaurant District", new Vector2(480, 180), 50f, 
            PointOfInterest.POIType.Restaurant, 8));
        pointsOfInterest.Add(new PointOfInterest("Retail Park", new Vector2(520, 220), 60f, 
            PointOfInterest.POIType.Shop, 8));
        pointsOfInterest.Add(new PointOfInterest("Southside Pubs", new Vector2(500, 160), 40f, 
            PointOfInterest.POIType.Bar, 5));

        // Additional areas for more population
        pointsOfInterest.Add(new PointOfInterest("Bilston Town Centre", new Vector2(300, 300), 70f, 
            PointOfInterest.POIType.ShoppingCentre, 10));
        pointsOfInterest.Add(new PointOfInterest("Wednesfield Centre", new Vector2(700, 400), 60f, 
            PointOfInterest.POIType.Shop, 7));
        pointsOfInterest.Add(new PointOfInterest("Tettenhall Green", new Vector2(600, 700), 50f, 
            PointOfInterest.POIType.Park, 5));

        // Residential areas (lower density)
        pointsOfInterest.Add(new PointOfInterest("Pendeford Estate", new Vector2(750, 600), 70f, 
            PointOfInterest.POIType.Residential, 8));
        pointsOfInterest.Add(new PointOfInterest("Bushbury Estate", new Vector2(200, 650), 65f, 
            PointOfInterest.POIType.Residential, 7));
        pointsOfInterest.Add(new PointOfInterest("Oxley Estate", new Vector2(300, 750), 60f, 
            PointOfInterest.POIType.Residential, 6));
        pointsOfInterest.Add(new PointOfInterest("Goldthorn Park Estate", new Vector2(400, 200), 55f, 
            PointOfInterest.POIType.Residential, 6));
        pointsOfInterest.Add(new PointOfInterest("Ettingshall Estate", new Vector2(350, 350), 55f, 
            PointOfInterest.POIType.Residential, 6));
        pointsOfInterest.Add(new PointOfInterest("Merry Hill Estate", new Vector2(600, 250), 50f, 
            PointOfInterest.POIType.Residential, 5));

        // Street areas (pedestrians walking on roads/sidewalks)
        pointsOfInterest.Add(new PointOfInterest("Ring Road Pedestrians", new Vector2(500, 100), 80f, 
            PointOfInterest.POIType.Street, 4));
        pointsOfInterest.Add(new PointOfInterest("Main Street Walkers", new Vector2(500, 500), 40f, 
            PointOfInterest.POIType.Street, 5));
        pointsOfInterest.Add(new PointOfInterest("Commercial District Walkers", new Vector2(600, 500), 50f, 
            PointOfInterest.POIType.Street, 4));
        pointsOfInterest.Add(new PointOfInterest("Industrial Area Workers", new Vector2(800, 200), 60f, 
            PointOfInterest.POIType.Street, 3));

        Debug.Log($"Initialized {pointsOfInterest.Count} points of interest");
    }

    private void SpawnInitialPedestrians()
    {
        for (int i = 0; i < totalPedestrianCount; i++)
        {
            SpawnPedestrian();
        }

        Debug.Log($"Spawned {activePedestrians.Count} pedestrians");
    }

    private void SpawnPedestrian()
    {
        // Choose a random POI to spawn near
        PointOfInterest poi = pointsOfInterest[Random.Range(0, pointsOfInterest.Count)];
        
        Vector3 spawnPosition = (Vector3)poi.location + (Vector3)Random.insideUnitCircle * poi.radius;
        spawnPosition.z = 0f;

        GameObject pedestrianGO = new GameObject($"Pedestrian_{activePedestrians.Count}");
        pedestrianGO.transform.position = spawnPosition;
        pedestrianGO.tag = "Pedestrian";

        // Add visual representation
        SpriteRenderer spriteRenderer = pedestrianGO.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
        spriteRenderer.color = Color.white;
        spriteRenderer.sortingOrder = 2;

        // Add collider
        CircleCollider2D collider = pedestrianGO.AddComponent<CircleCollider2D>();
        collider.radius = 0.3f;

        // Add rigidbody
        Rigidbody2D rb = pedestrianGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;

        // Add pedestrian script
        Pedestrian ped = pedestrianGO.AddComponent<Pedestrian>();
        ped.Type = (Pedestrian.PedestrianType)Random.Range(0, 5);
        ped.DisplayName = $"Pedestrian_{activePedestrians.Count}";

        // Add traffic behavior
        PedestrianTrafficBehavior trafficBehavior = pedestrianGO.AddComponent<PedestrianTrafficBehavior>();
        
        // Add weapon system (rare)
        PedestrianWeaponSystem weaponSystem = pedestrianGO.AddComponent<PedestrianWeaponSystem>();

        pedestrianGO.transform.parent = transform;
        activePedestrians.Add(ped);
    }

    private void UpdatePedestrianDistribution()
    {
        // Ensure pedestrians are distributed correctly among POIs
        foreach (var poi in pointsOfInterest)
        {
            int pedestriansNearPOI = CountPedestriansNearPOI(poi);
            
            // Spawn more if needed
            while (pedestriansNearPOI < poi.targetPedestrianCount && activePedestrians.Count < totalPedestrianCount)
            {
                SpawnPedestrianAtPOI(poi);
                pedestriansNearPOI++;
            }

            // Move away extras (handled through natural wandering)
        }
    }

    private int CountPedestriansNearPOI(PointOfInterest poi)
    {
        int count = 0;
        foreach (var ped in activePedestrians)
        {
            if (Vector2.Distance(ped.transform.position, poi.location) <= poi.radius)
                count++;
        }
        return count;
    }

    private void SpawnPedestrianAtPOI(PointOfInterest poi)
    {
        Vector3 spawnPosition = (Vector3)poi.location + (Vector3)Random.insideUnitCircle * (poi.radius * 0.8f);
        spawnPosition.z = 0f;

        GameObject pedestrianGO = new GameObject($"Pedestrian_{activePedestrians.Count}");
        pedestrianGO.transform.position = spawnPosition;
        pedestrianGO.tag = "Pedestrian";

        SpriteRenderer spriteRenderer = pedestrianGO.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
        spriteRenderer.color = Color.white;
        spriteRenderer.sortingOrder = 2;

        CircleCollider2D collider = pedestrianGO.AddComponent<CircleCollider2D>();
        collider.radius = 0.3f;

        Rigidbody2D rb = pedestrianGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;

        Pedestrian ped = pedestrianGO.AddComponent<Pedestrian>();
        
        // Choose type based on POI
        switch (poi.type)
        {
            case PointOfInterest.POIType.University:
            case PointOfInterest.POIType.School:
                ped.Type = Pedestrian.PedestrianType.Student;
                break;
            case PointOfInterest.POIType.ShoppingCentre:
            case PointOfInterest.POIType.Shop:
                ped.Type = Pedestrian.PedestrianType.Shopper;
                break;
            case PointOfInterest.POIType.Library:
            case PointOfInterest.POIType.Restaurant:
                ped.Type = Pedestrian.PedestrianType.Worker;
                break;
            default:
                ped.Type = (Pedestrian.PedestrianType)Random.Range(0, 5);
                break;
        }

        ped.DisplayName = $"Pedestrian_{activePedestrians.Count}";
        pedestrianGO.transform.parent = transform;
        activePedestrians.Add(ped);
    }

    public List<Pedestrian> GetAllPedestrians()
    {
        return activePedestrians;
    }

    public List<Pedestrian> GetPedestriansNearPosition(Vector3 position, float radius)
    {
        List<Pedestrian> result = new List<Pedestrian>();
        foreach (var ped in activePedestrians)
        {
            if (Vector3.Distance(ped.transform.position, position) <= radius)
                result.Add(ped);
        }
        return result;
    }

    public List<PointOfInterest> GetPointsOfInterest()
    {
        return pointsOfInterest;
    }

    public PointOfInterest GetNearestPOI(Vector3 position)
    {
        PointOfInterest nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (var poi in pointsOfInterest)
        {
            float distance = Vector2.Distance(position, poi.location);
            if (distance < nearestDistance)
            {
                nearest = poi;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    public int GetPedestrianCount()
    {
        return activePedestrians.Count;
    }

    public void LogPedestrianStats()
    {
        Debug.Log($"=== Pedestrian Statistics ===");
        Debug.Log($"Total Pedestrians: {activePedestrians.Count}");
        
        foreach (var poi in pointsOfInterest)
        {
            int count = CountPedestriansNearPOI(poi);
            Debug.Log($"{poi.poiName}: {count}/{poi.targetPedestrianCount}");
        }
    }
}
