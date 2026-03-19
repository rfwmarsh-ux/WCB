using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all vehicles in the game world
/// </summary>
public class VehicleManager : MonoBehaviour
{
    [SerializeField] private VehicleFactory vehicleFactory;
    [SerializeField] private int initialVehicleCount = 20;

    private List<Vehicle> allVehicles = new List<Vehicle>();
    private Dictionary<VehicleType, List<Vehicle>> vehiclesByType = new Dictionary<VehicleType, List<Vehicle>>();
    private List<string> ownedVehicles = new List<string>();
    private string currentVehicleId = null;

    private void Start()
    {
        if (vehicleFactory == null)
            vehicleFactory = GetComponent<VehicleFactory>();

        SpawnInitialVehicles();
    }

    private void SpawnInitialVehicles()
    {
        // Distribution: More common vehicles, fewer supercars
        SpawnVehiclesOfType(VehicleType.SlowTruck, 2);
        SpawnVehiclesOfType(VehicleType.SlowVan, 3);
        SpawnVehiclesOfType(VehicleType.MediumVan, 3);
        SpawnVehiclesOfType(VehicleType.FastVan, 2);
        SpawnVehiclesOfType(VehicleType.CompactCar, 2);
        SpawnVehiclesOfType(VehicleType.EconomyCar, 2);
        SpawnVehiclesOfType(VehicleType.TaxiCab, 3);
        SpawnVehiclesOfType(VehicleType.ClassicCoupe, 1);
        SpawnVehiclesOfType(VehicleType.SedanCar, 3);
        SpawnVehiclesOfType(VehicleType.LuxurySedan, 2);
        SpawnVehiclesOfType(VehicleType.MuscleCar, 1);
        SpawnVehiclesOfType(VehicleType.SportsCar, 2);
        SpawnVehiclesOfType(VehicleType.PoliceCruiser, 1);
        SpawnVehiclesOfType(VehicleType.SuperCar, 1);
        SpawnVehiclesOfType(VehicleType.Motorcycle, 1);
        
        // New vehicle types
        SpawnVehiclesOfType(VehicleType.Hatchback, 2);
        SpawnVehiclesOfType(VehicleType.SUV, 2);
        SpawnVehiclesOfType(VehicleType.PickupTruck, 1);
        SpawnVehiclesOfType(VehicleType.OffroadVehicle, 1);
        SpawnVehiclesOfType(VehicleType.Van, 2);
        SpawnVehiclesOfType(VehicleType.Convertible, 1);
        SpawnVehiclesOfType(VehicleType.ArmoredCar, 1);
        SpawnVehiclesOfType(VehicleType.RallyCar, 1);
        SpawnVehiclesOfType(VehicleType.DriftCar, 1);
        SpawnVehiclesOfType(VehicleType.HotRod, 1);
        
        // Motorcycles
        SpawnVehiclesOfType(VehicleType.Motorcycle, 1);
        SpawnVehiclesOfType(VehicleType.SportBike, 1);
        SpawnVehiclesOfType(VehicleType.Scooter, 1);
    }

    private void SpawnVehiclesOfType(VehicleType type, int count)
    {
        if (!vehiclesByType.ContainsKey(type))
            vehiclesByType[type] = new List<Vehicle>();

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = GetRandomSpawnPosition();
            GameObject vehicleGO = vehicleFactory.CreateVehicle(type, randomPosition);
            
            if (vehicleGO != null)
            {
                Vehicle vehicle = vehicleGO.GetComponent<Vehicle>();
                allVehicles.Add(vehicle);
                vehiclesByType[type].Add(vehicle);
                vehicleGO.transform.parent = transform;
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 spawnPos;
        
        if (MapSystem.Instance != null)
        {
            spawnPos = MapSystem.Instance.GetRandomJunction();
            spawnPos += new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));
        }
        else
        {
            spawnPos = new Vector2(Random.Range(100f, 900f), Random.Range(100f, 900f));
        }
        
        return new Vector3(spawnPos.x, spawnPos.y, 0f);
    }

    public Vehicle GetNearestVehicle(Vector3 position, float searchRadius = 10f)
    {
        Vehicle nearest = null;
        float nearestDistance = searchRadius;

        foreach (var vehicle in allVehicles)
        {
            if (vehicle == null) continue;
            
            float distance = Vector3.Distance(vehicle.transform.position, position);
            if (distance < nearestDistance)
            {
                nearest = vehicle;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    public List<Vehicle> GetVehiclesInArea(Vector3 position, float radius)
    {
        List<Vehicle> vehiclesInArea = new List<Vehicle>();

        foreach (var vehicle in allVehicles)
        {
            if (vehicle == null) continue;
            
            float distance = Vector3.Distance(vehicle.transform.position, position);
            if (distance <= radius)
                vehiclesInArea.Add(vehicle);
        }

        return vehiclesInArea;
    }

    public List<Vehicle> GetVehiclesByType(VehicleType type)
    {
        if (vehiclesByType.ContainsKey(type))
            return vehiclesByType[type];
        
        return new List<Vehicle>();
    }

    public Vehicle GetSpecificVehicle(string displayName)
    {
        foreach (var vehicle in allVehicles)
        {
            if (vehicle.DisplayName == displayName)
                return vehicle;
        }

        return null;
    }

    public int GetTotalVehicleCount()
    {
        return allVehicles.Count;
    }

    public Dictionary<VehicleType, int> GetVehicleCountByType()
    {
        Dictionary<VehicleType, int> counts = new Dictionary<VehicleType, int>();

        foreach (var kvp in vehiclesByType)
        {
            counts[kvp.Key] = kvp.Value.Count;
        }

        return counts;
    }

    public void RemoveVehicle(Vehicle vehicle)
    {
        allVehicles.Remove(vehicle);
        if (vehiclesByType.ContainsKey(vehicle.VehicleType))
            vehiclesByType[vehicle.VehicleType].Remove(vehicle);
    }

    public void AddOwnedVehicle(string vehicleId)
    {
        if (!ownedVehicles.Contains(vehicleId))
        {
            ownedVehicles.Add(vehicleId);
        }
    }

    public string[] GetOwnedVehicles()
    {
        return ownedVehicles.ToArray();
    }

    public void SetCurrentVehicle(string vehicleId)
    {
        currentVehicleId = vehicleId;
    }

    public string GetCurrentVehicleId()
    {
        return currentVehicleId;
    }

    // For debugging - list all vehicles
    public void LogAllVehicles()
    {
        Debug.Log($"=== Vehicle List ({allVehicles.Count} total) ===");
        foreach (var kvp in vehiclesByType)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value.Count}");
        }
    }
}
