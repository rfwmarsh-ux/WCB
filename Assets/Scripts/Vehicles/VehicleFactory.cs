using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Factory for creating and managing vehicle instances with predefined stats
/// </summary>
public class VehicleFactory : MonoBehaviour
{
    private Dictionary<VehicleType, VehicleStats> vehicleStatsLibrary = new Dictionary<VehicleType, VehicleStats>();

    private void Awake()
    {
        InitializeVehicleStats();
    }

    private void InitializeVehicleStats()
    {
        // TRAINS - Highest health (1400)
        vehicleStatsLibrary[VehicleType.Train] = new VehicleStats(
            type: VehicleType.Train,
            displayName: " freight Train",
            maxSpeed: 6f,
            acceleration: 2f,
            turnSpeed: 20f,
            weight: 20f,
            capacity: 2,
            brakePower: 0.1f,
            color: new Color(0.3f, 0.2f, 0.1f),
            health: 1400f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.Metro] = new VehicleStats(
            type: VehicleType.Metro,
            displayName: "Metro Tram",
            maxSpeed: 8f,
            acceleration: 3f,
            turnSpeed: 40f,
            weight: 15f,
            capacity: 20,
            brakePower: 0.12f,
            color: new Color(0.8f, 0.2f, 0.2f),
            health: 1400f,
            offRoad: true
        );

        // TRUCKS/BUSES - High health (1050)
        vehicleStatsLibrary[VehicleType.SlowTruck] = new VehicleStats(
            type: VehicleType.SlowTruck,
            displayName: "Industrial Hauler",
            maxSpeed: 8f,
            acceleration: 3f,
            turnSpeed: 80f,
            weight: 5f,
            capacity: 2,
            brakePower: 0.15f,
            color: new Color(0.6f, 0.5f, 0.4f),
            health: 1050f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.Bus] = new VehicleStats(
            type: VehicleType.Bus,
            displayName: "City Bus",
            maxSpeed: 7f,
            acceleration: 2.5f,
            turnSpeed: 60f,
            weight: 8f,
            capacity: 30,
            brakePower: 0.12f,
            color: new Color(0.2f, 0.4f, 0.8f),
            health: 1050f,
            offRoad: true
        );

        // VANS/AMBULANCES - Medium health (560)
        vehicleStatsLibrary[VehicleType.SlowVan] = new VehicleStats(
            type: VehicleType.SlowVan,
            displayName: "Economy Van",
            maxSpeed: 10f,
            acceleration: 4f,
            turnSpeed: 120f,
            weight: 2.5f,
            capacity: 6,
            brakePower: 0.18f,
            color: new Color(0.3f, 0.3f, 0.7f),
            health: 560f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.MediumVan] = new VehicleStats(
            type: VehicleType.MediumVan,
            displayName: "Transit Van",
            maxSpeed: 12f,
            acceleration: 5f,
            turnSpeed: 130f,
            weight: 2.2f,
            capacity: 6,
            brakePower: 0.20f,
            color: new Color(0.7f, 0.7f, 0.7f),
            health: 560f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.FastVan] = new VehicleStats(
            type: VehicleType.FastVan,
            displayName: "Delivery Van",
            maxSpeed: 14f,
            acceleration: 6f,
            turnSpeed: 140f,
            weight: 2f,
            capacity: 6,
            brakePower: 0.22f,
            color: new Color(1f, 0.64f, 0f),
            health: 560f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.Ambulance] = new VehicleStats(
            type: VehicleType.Ambulance,
            displayName: "Ambulance",
            maxSpeed: 14f,
            acceleration: 5.5f,
            turnSpeed: 135f,
            weight: 2.5f,
            capacity: 4,
            brakePower: 0.22f,
            color: Color.white,
            health: 560f,
            offRoad: true
        );

        // CARS - Lower health (350)
        vehicleStatsLibrary[VehicleType.CompactCar] = new VehicleStats(
            type: VehicleType.CompactCar,
            displayName: "Compact Runner",
            maxSpeed: 12f,
            acceleration: 5.2f,
            turnSpeed: 170f,
            weight: 1f,
            capacity: 3,
            brakePower: 0.24f,
            color: new Color(1f, 0.85f, 0f),
            health: 350f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.EconomyCar] = new VehicleStats(
            type: VehicleType.EconomyCar,
            displayName: "City Runner",
            maxSpeed: 13f,
            acceleration: 5.5f,
            turnSpeed: 160f,
            weight: 1.2f,
            capacity: 4,
            brakePower: 0.25f,
            color: new Color(1f, 0f, 0f),
            health: 350f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.TaxiCab] = new VehicleStats(
            type: VehicleType.TaxiCab,
            displayName: "Yellow Cab",
            maxSpeed: 13.5f,
            acceleration: 5.6f,
            turnSpeed: 155f,
            weight: 1.3f,
            capacity: 4,
            brakePower: 0.245f,
            color: new Color(1f, 1f, 0f),
            health: 350f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.ClassicCoupe] = new VehicleStats(
            type: VehicleType.ClassicCoupe,
            displayName: "Retro Coupe",
            maxSpeed: 14f,
            acceleration: 5.8f,
            turnSpeed: 145f,
            weight: 1.5f,
            capacity: 2,
            brakePower: 0.27f,
            color: new Color(0.5f, 0.2f, 0.8f),
            health: 350f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.SedanCar] = new VehicleStats(
            type: VehicleType.SedanCar,
            displayName: "Family Sedan",
            maxSpeed: 15f,
            acceleration: 6f,
            turnSpeed: 150f,
            weight: 1.4f,
            capacity: 4,
            brakePower: 0.26f,
            color: new Color(0.2f, 0.2f, 0.2f),
            health: 350f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.LuxurySedan] = new VehicleStats(
            type: VehicleType.LuxurySedan,
            displayName: "Executive",
            maxSpeed: 16f,
            acceleration: 6.5f,
            turnSpeed: 148f,
            weight: 1.6f,
            capacity: 4,
            brakePower: 0.28f,
            color: new Color(0.3f, 0.3f, 0.5f),
            health: 350f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.MuscleCar] = new VehicleStats(
            type: VehicleType.MuscleCar,
            displayName: "Bruiser",
            maxSpeed: 17f,
            acceleration: 7.5f,
            turnSpeed: 130f,
            weight: 1.9f,
            capacity: 2,
            brakePower: 0.29f,
            color: new Color(0.5f, 0f, 0f),
            health: 350f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.SportsCar] = new VehicleStats(
            type: VehicleType.SportsCar,
            displayName: "Thunder Coupe",
            maxSpeed: 18f,
            acceleration: 8f,
            turnSpeed: 180f,
            weight: 1f,
            capacity: 2,
            brakePower: 0.30f,
            color: new Color(1f, 0.2f, 0f),
            health: 350f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.PoliceCruiser] = new VehicleStats(
            type: VehicleType.PoliceCruiser,
            displayName: "Police Cruiser",
            maxSpeed: 19f,
            acceleration: 7.8f,
            turnSpeed: 175f,
            weight: 1.7f,
            capacity: 2,
            brakePower: 0.32f,
            color: new Color(0f, 0f, 0.8f),
            health: 560f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.SuperCar] = new VehicleStats(
            type: VehicleType.SuperCar,
            displayName: "Apex Predator",
            maxSpeed: 22f,
            acceleration: 10f,
            turnSpeed: 200f,
            weight: 0.8f,
            capacity: 2,
            brakePower: 0.35f,
            color: new Color(0.8f, 0.8f, 0f),
            health: 350f,
            offRoad: true
        );

        // NEW CAR TYPES
        vehicleStatsLibrary[VehicleType.Hatchback] = new VehicleStats(
            type: VehicleType.Hatchback,
            displayName: "City Hatch",
            maxSpeed: 12f,
            acceleration: 5f,
            turnSpeed: 160f,
            weight: 1.2f,
            capacity: 4,
            brakePower: 0.25f,
            color: new Color(0.9f, 0.4f, 0.2f),
            health: 280f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.SUV] = new VehicleStats(
            type: VehicleType.SUV,
            displayName: "Urban SUV",
            maxSpeed: 14f,
            acceleration: 4.5f,
            turnSpeed: 120f,
            weight: 2.0f,
            capacity: 6,
            brakePower: 0.22f,
            color: new Color(0.2f, 0.2f, 0.3f),
            health: 420f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.PickupTruck] = new VehicleStats(
            type: VehicleType.PickupTruck,
            displayName: "Work Pickup",
            maxSpeed: 13f,
            acceleration: 4f,
            turnSpeed: 100f,
            weight: 2.2f,
            capacity: 3,
            brakePower: 0.2f,
            color: new Color(0.6f, 0.5f, 0.4f),
            health: 450f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.OffroadVehicle] = new VehicleStats(
            type: VehicleType.OffroadVehicle,
            displayName: "Trail Blazer",
            maxSpeed: 11f,
            acceleration: 3.5f,
            turnSpeed: 90f,
            weight: 2.5f,
            capacity: 4,
            brakePower: 0.18f,
            color: new Color(0.3f, 0.6f, 0.3f),
            health: 480f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.Van] = new VehicleStats(
            type: VehicleType.Van,
            displayName: "Delivery Van",
            maxSpeed: 11f,
            acceleration: 4f,
            turnSpeed: 110f,
            weight: 2.0f,
            capacity: 3,
            brakePower: 0.22f,
            color: new Color(0.9f, 0.9f, 0.9f),
            health: 400f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.Convertible] = new VehicleStats(
            type: VehicleType.Convertible,
            displayName: "Sun Cruiser",
            maxSpeed: 18f,
            acceleration: 7f,
            turnSpeed: 190f,
            weight: 1.3f,
            capacity: 2,
            brakePower: 0.32f,
            color: new Color(1f, 0.3f, 0.5f),
            health: 320f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.ArmoredCar] = new VehicleStats(
            type: VehicleType.ArmoredCar,
            displayName: "Secure Transport",
            maxSpeed: 10f,
            acceleration: 3f,
            turnSpeed: 80f,
            weight: 5.0f,
            capacity: 4,
            brakePower: 0.15f,
            color: new Color(0.4f, 0.4f, 0.45f),
            health: 800f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.RallyCar] = new VehicleStats(
            type: VehicleType.RallyCar,
            displayName: "Stage Rally",
            maxSpeed: 19f,
            acceleration: 8.5f,
            turnSpeed: 210f,
            weight: 1.1f,
            capacity: 2,
            brakePower: 0.33f,
            color: new Color(0.9f, 0.2f, 0.2f),
            health: 340f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.DriftCar] = new VehicleStats(
            type: VehicleType.DriftCar,
            displayName: "Drift King",
            maxSpeed: 17f,
            acceleration: 7.5f,
            turnSpeed: 230f,
            weight: 1.2f,
            capacity: 2,
            brakePower: 0.34f,
            color: new Color(0.2f, 0.8f, 0.9f),
            health: 330f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.HotRod] = new VehicleStats(
            type: VehicleType.HotRod,
            displayName: "Classic Hot Rod",
            maxSpeed: 16f,
            acceleration: 6.5f,
            turnSpeed: 170f,
            weight: 1.5f,
            capacity: 2,
            brakePower: 0.30f,
            color: new Color(0.8f, 0.2f, 0.1f),
            health: 360f,
            offRoad: true
        );

        // MOTORCYCLE - Lowest health (175)
        vehicleStatsLibrary[VehicleType.Motorcycle] = new VehicleStats(
            type: VehicleType.Motorcycle,
            displayName: "Street Bike",
            maxSpeed: 20f,
            acceleration: 9f,
            turnSpeed: 220f,
            weight: 0.5f,
            capacity: 1,
            brakePower: 0.32f,
            color: new Color(0f, 0f, 0f),
            health: 175f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.SportBike] = new VehicleStats(
            type: VehicleType.SportBike,
            displayName: "Racing Spec",
            maxSpeed: 25f,
            acceleration: 12f,
            turnSpeed: 280f,
            weight: 0.4f,
            capacity: 1,
            brakePower: 0.38f,
            color: new Color(1f, 0f, 0.2f),
            health: 150f,
            offRoad: true
        );

        vehicleStatsLibrary[VehicleType.Scooter] = new VehicleStats(
            type: VehicleType.Scooter,
            displayName: "City Scooter",
            maxSpeed: 14f,
            acceleration: 5f,
            turnSpeed: 180f,
            weight: 0.8f,
            capacity: 1,
            brakePower: 0.28f,
            color: new Color(0.9f, 0.9f, 0.9f),
            health: 120f,
            offRoad: true
        );
    }

    /// <summary>
    /// Creates a vehicle prefab with the specified type
    /// </summary>
    public GameObject CreateVehicle(VehicleType type, Vector3 position)
    {
        if (!vehicleStatsLibrary.ContainsKey(type))
        {
            Debug.LogError($"Vehicle type {type} not found in library!");
            return null;
        }

        // Create a new GameObject with vehicle components
        GameObject vehicleGO = new GameObject($"{vehicleStatsLibrary[type].displayName} Instance");
        vehicleGO.transform.position = position;

        // Add required components
        Rigidbody2D rb = vehicleGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;

        SpriteRenderer spriteRenderer = vehicleGO.AddComponent<SpriteRenderer>();
        spriteRenderer.color = vehicleStatsLibrary[type].vehicleColor;

        // Set vehicle scale based on type (real world proportions)
        Vector2 size = GetVehicleDimensions(type);
        vehicleGO.transform.localScale = new Vector3(size.x, size.y, 1f);

        // Add collider
        BoxCollider2D collider = vehicleGO.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(size.x * 0.9f, size.y * 0.8f);

        // Add vehicle script and set stats
        Vehicle vehicle = vehicleGO.AddComponent<Vehicle>();
        vehicle.SetVehicleStats(vehicleStatsLibrary[type]);

        // Add car lights component
        CarLights lights = vehicleGO.AddComponent<CarLights>();

        // Add tag for interactions
        vehicleGO.tag = "Vehicle";

        return vehicleGO;
    }

    private Vector2 GetVehicleDimensions(VehicleType type)
    {
        switch (type)
        {
            case VehicleType.Train:
                return new Vector2(30f, 3f);
            case VehicleType.Metro:
                return new Vector2(18f, 2.8f);
            case VehicleType.Bus:
                return new Vector2(14f, 3f);
            case VehicleType.SlowTruck:
                return new Vector2(12f, 3f);
            case VehicleType.SlowVan:
            case VehicleType.MediumVan:
            case VehicleType.FastVan:
            case VehicleType.Van:
                return new Vector2(6f, 2.8f);
            case VehicleType.Ambulance:
                return new Vector2(6f, 2.8f);
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
            case VehicleType.SUV:
                return new Vector2(5f, 2.5f);
            case VehicleType.PickupTruck:
                return new Vector2(6f, 2.5f);
            case VehicleType.OffroadVehicle:
                return new Vector2(4.5f, 2.3f);
            case VehicleType.ArmoredCar:
                return new Vector2(6f, 2.8f);
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

    /// <summary>
    /// Get vehicle stats without creating an instance
    /// </summary>
    public VehicleStats GetVehicleStats(VehicleType type)
    {
        if (vehicleStatsLibrary.ContainsKey(type))
            return vehicleStatsLibrary[type];
        
        Debug.LogWarning($"Vehicle type {type} not found!");
        return null;
    }

    /// <summary>
    /// Get all available vehicle types
    /// </summary>
    public List<VehicleType> GetAllVehicleTypes()
    {
        return new List<VehicleType>(vehicleStatsLibrary.Keys);
    }

    /// <summary>
    /// Get vehicles of a specific category
    /// </summary>
    public List<VehicleType> GetVehiclesByCategory(string category)
    {
        List<VehicleType> result = new List<VehicleType>();

        switch (category.ToLower())
        {
            case "truck":
                result.Add(VehicleType.SlowTruck);
                break;
            case "van":
                result.Add(VehicleType.SlowVan);
                result.Add(VehicleType.MediumVan);
                result.Add(VehicleType.FastVan);
                break;
            case "car":
                result.Add(VehicleType.EconomyCar);
                result.Add(VehicleType.SedanCar);
                result.Add(VehicleType.SportsCar);
                result.Add(VehicleType.SuperCar);
                result.Add(VehicleType.Motorcycle);
                break;
        }

        return result;
    }
}
