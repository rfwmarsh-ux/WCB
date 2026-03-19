using UnityEngine;
using System.Collections.Generic;

public class ParkedVehicleManager : MonoBehaviour
{
    public static ParkedVehicleManager Instance { get; private set; }
    
    private List<ParkedVehicle> parkedVehicles = new List<ParkedVehicle>();
    private List<Transform> carParkLocations = new List<Transform>();
    private List<Transform> residentialParkingSpots = new List<Transform>();
    
    [SerializeField] private int maxCarParkVehicles = 15;
    [SerializeField] private int maxResidentialVehicles = 20;
    
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
        InitializeCarParks();
        InitializeResidentialSpots();
        SpawnParkedVehicles();
    }
    
    private void InitializeCarParks()
    {
        Vector2[] carParkPositions = new Vector2[]
        {
            new Vector2(200, 400),
            new Vector2(500, 500),
            new Vector2(700, 400),
            new Vector2(300, 600),
            new Vector2(600, 700),
            new Vector2(400, 450),
            new Vector2(550, 380),
            new Vector2(250, 500),
            new Vector2(650, 550),
            new Vector2(450, 650)
        };
        
        foreach (Vector2 pos in carParkPositions)
        {
            GameObject carPark = new GameObject($"CarPark_{carParkLocations.Count}");
            carPark.transform.position = (Vector3)pos;
            carParkLocations.Add(carPark.transform);
        }
    }
    
    private void InitializeResidentialSpots()
    {
        Vector2[] residentialPositions = new Vector2[]
        {
            new Vector2(120, 200), new Vector2(150, 200), new Vector2(180, 200),
            new Vector2(120, 300), new Vector2(150, 300),
            new Vector2(420, 400), new Vector2(450, 400), new Vector2(480, 400),
            new Vector2(420, 500), new Vector2(450, 500),
            new Vector2(620, 600), new Vector2(650, 600), new Vector2(680, 600),
            new Vector2(620, 700), new Vector2(650, 700),
            new Vector2(120, 700), new Vector2(150, 700), new Vector2(180, 700),
            new Vector2(320, 250), new Vector2(350, 250),
            new Vector2(720, 300), new Vector2(750, 300),
            new Vector2(820, 450), new Vector2(850, 450),
            new Vector2(280, 550), new Vector2(310, 550),
            new Vector2(550, 280), new Vector2(580, 280),
            new Vector2(380, 750), new Vector2(410, 750)
        };
        
        foreach (Vector2 pos in residentialPositions)
        {
            GameObject spot = new GameObject($"ResidentialSpot_{residentialParkingSpots.Count}");
            spot.transform.position = (Vector3)pos;
            residentialParkingSpots.Add(spot.transform);
        }
    }
    
    private void SpawnParkedVehicles()
    {
        SpawnVehiclesAtCarParks();
        SpawnVehiclesAtResidentialAreas();
        
        Debug.Log($"Spawned {parkedVehicles.Count} parked vehicles");
    }
    
    private void SpawnVehiclesAtCarParks()
    {
        VehicleType[] carParkVehicleTypes = new VehicleType[]
        {
            VehicleType.CompactCar, VehicleType.Hatchback, VehicleType.EconomyCar,
            VehicleType.SedanCar, VehicleType.SUV, VehicleType.MuscleCar,
            VehicleType.SportsCar, VehicleType.Convertible, VehicleType.TaxiCab
        };
        
        foreach (Transform carPark in carParkLocations)
        {
            int vehiclesInThisLot = Random.Range(1, 4);
            
            for (int i = 0; i < vehiclesInThisLot; i++)
            {
                if (parkedVehicles.Count >= maxCarParkVehicles) return;
                
                Vector2 offset = new Vector2(Random.Range(-15f, 15f), Random.Range(-10f, 10f));
                Vector2 spawnPos = carPark.position + (Vector3)offset;
                
                VehicleType vType = carParkVehicleTypes[Random.Range(0, carParkVehicleTypes.Length)];
                ParkedVehicle pv = CreateParkedVehicle(spawnPos, vType, true);
                
                if (pv != null)
                {
                    pv.SetRotation(Random.Range(0f, 360f));
                    pv.SetColor(GetRandomVehicleColor());
                }
            }
        }
    }
    
    private void SpawnVehiclesAtResidentialAreas()
    {
        VehicleType[] residentialTypes = new VehicleType[]
        {
            VehicleType.CompactCar, VehicleType.Hatchback, VehicleType.EconomyCar,
            VehicleType.SedanCar, VehicleType.ClassicCoupe, VehicleType.LuxurySedan
        };
        
        foreach (Transform spot in residentialParkingSpots)
        {
            if (parkedVehicles.Count >= maxCarParkVehicles + maxResidentialVehicles) return;
            
            if (Random.value > 0.6f)
            {
                VehicleType vType = residentialTypes[Random.Range(0, residentialTypes.Length)];
                ParkedVehicle pv = CreateParkedVehicle(spot.position, vType, false);
                
                if (pv != null)
                {
                    float baseRotation = Random.Range(0f, 2f) > 1f ? 90f : -90f;
                    pv.SetRotation(baseRotation + Random.Range(-10f, 10f));
                    pv.SetColor(GetRandomVehicleColor());
                }
            }
        }
    }
    
    private ParkedVehicle CreateParkedVehicle(Vector2 position, VehicleType type, bool inCarPark)
    {
        Vector2 size = GetVehicleDimensions(type);
        
        GameObject vehicleGO = new GameObject($"Parked_{type}");
        vehicleGO.transform.position = (Vector3)position;
        
        SpriteRenderer sr = vehicleGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.sortingOrder = 4;
        
        vehicleGO.transform.localScale = new Vector3(size.x, size.y, 1f);
        
        BoxCollider2D col = vehicleGO.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
        col.isTrigger = true;
        
        ParkedVehicle pv = vehicleGO.AddComponent<ParkedVehicle>();
        pv.Initialize(type, inCarPark);
        
        parkedVehicles.Add(pv);
        
        return pv;
    }
    
    private Vector2 GetVehicleDimensions(VehicleType type)
    {
        switch (type)
        {
            case VehicleType.CompactCar:
            case VehicleType.Hatchback:
            case VehicleType.EconomyCar:
                return new Vector2(4.5f, 2.2f);
            case VehicleType.SedanCar:
            case VehicleType.LuxurySedan:
            case VehicleType.ClassicCoupe:
                return new Vector2(5f, 2.4f);
            case VehicleType.MuscleCar:
            case VehicleType.SportsCar:
            case VehicleType.Convertible:
                return new Vector2(4.8f, 2.3f);
            case VehicleType.SUV:
            case VehicleType.PickupTruck:
                return new Vector2(5f, 2.5f);
            case VehicleType.TaxiCab:
                return new Vector2(5f, 2.4f);
            default:
                return new Vector2(5f, 2.4f);
        }
    }
    
    private Color GetRandomVehicleColor()
    {
        Color[] colors = new Color[]
        {
            new Color(0.2f, 0.2f, 0.8f),
            new Color(0.8f, 0.2f, 0.2f),
            new Color(0.2f, 0.8f, 0.2f),
            new Color(0.9f, 0.9f, 0.9f),
            new Color(0.1f, 0.1f, 0.1f),
            new Color(0.8f, 0.8f, 0.2f),
            new Color(0.8f, 0.2f, 0.8f),
            new Color(0.2f, 0.8f, 0.8f),
            new Color(0.6f, 0.3f, 0.2f),
            new Color(0.3f, 0.3f, 0.3f)
        };
        
        return colors[Random.Range(0, colors.Length)];
    }
    
    public List<ParkedVehicle> GetAllParkedVehicles() => parkedVehicles;
    
    public ParkedVehicle GetNearestParkedVehicle(Vector2 position, float maxDistance = 10f)
    {
        ParkedVehicle nearest = null;
        float minDist = maxDistance;
        
        foreach (var pv in parkedVehicles)
        {
            if (pv == null) continue;
            float dist = Vector2.Distance(position, pv.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = pv;
            }
        }
        
        return nearest;
    }
    
    public void RemoveParkedVehicle(ParkedVehicle vehicle)
    {
        parkedVehicles.Remove(vehicle);
    }
}

public class ParkedVehicle : MonoBehaviour
{
    private VehicleType vehicleType;
    private bool isInCarPark;
    private bool hasBeenTaken = false;
    private SpriteRenderer spriteRenderer;
    
    public void Initialize(VehicleType type, bool inCarPark)
    {
        vehicleType = type;
        isInCarPark = inCarPark;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        gameObject.tag = "ParkedVehicle";
    }
    
    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
    
    public void SetRotation(float rotation)
    {
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
    
    public VehicleType GetVehicleType() => vehicleType;
    public bool IsInCarPark() => isInCarPark;
    public bool HasBeenTaken() => hasBeenTaken;
    
    public void OnVehicleTaken()
    {
        hasBeenTaken = true;
        ParkedVehicleManager.Instance?.RemoveParkedVehicle(this);
    }
    
    public float GetSellValue()
    {
        switch (vehicleType)
        {
            case VehicleType.CompactCar:
            case VehicleType.Hatchback:
            case VehicleType.EconomyCar:
                return Random.Range(200f, 500f);
            case VehicleType.SedanCar:
            case VehicleType.ClassicCoupe:
                return Random.Range(400f, 800f);
            case VehicleType.LuxurySedan:
                return Random.Range(800f, 1500f);
            case VehicleType.MuscleCar:
            case VehicleType.SportsCar:
                return Random.Range(1000f, 3000f);
            case VehicleType.SUV:
            case VehicleType.PickupTruck:
                return Random.Range(500f, 1000f);
            case VehicleType.Convertible:
                return Random.Range(1500f, 4000f);
            case VehicleType.TaxiCab:
                return Random.Range(300f, 600f);
            default:
                return Random.Range(300f, 700f);
        }
    }
}
