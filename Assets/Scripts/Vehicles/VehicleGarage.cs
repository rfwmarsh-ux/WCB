using UnityEngine;

/// <summary>
/// Vehicle garage/shop system for buying and customizing vehicles
/// </summary>
public class VehicleGarage : MonoBehaviour
{
    [SerializeField] private string garageName;
    [SerializeField] private Vector2 garageLocation;
    [SerializeField] private VehicleFactory vehicleFactory;
    
    [SerializeField] private int maxCapacity = 10;
    private Vehicle[] garageVehicles;
    private int currentCount = 0;

    private void Start()
    {
        garageVehicles = new Vehicle[maxCapacity];
    }

    public bool AddVehicleToGarage(Vehicle vehicle)
    {
        if (currentCount >= maxCapacity)
        {
            Debug.LogWarning($"{garageName} is at full capacity!");
            return false;
        }

        garageVehicles[currentCount] = vehicle;
        vehicle.transform.position = garageLocation + (Vector2.right * currentCount * 2f);
        currentCount++;
        
        Debug.Log($"{vehicle.DisplayName} added to {garageName}");
        return true;
    }

    public Vehicle RetrieveVehicle(int index)
    {
        if (index < 0 || index >= currentCount)
        {
            Debug.LogWarning("Invalid vehicle index!");
            return null;
        }

        Vehicle vehicle = garageVehicles[index];
        // Shift remaining vehicles
        for (int i = index; i < currentCount - 1; i++)
        {
            garageVehicles[i] = garageVehicles[i + 1];
        }
        garageVehicles[currentCount - 1] = null;
        currentCount--;

        return vehicle;
    }

    public Vehicle GetVehicleInGarage(int index)
    {
        if (index >= 0 && index < currentCount)
            return garageVehicles[index];
        
        return null;
    }

    public int GetGarageCount()
    {
        return currentCount;
    }

    public string GetGarageName()
    {
        return garageName;
    }

    public void ListGarageVehicles()
    {
        Debug.Log($"=== {garageName} Garage ({currentCount}/{maxCapacity}) ===");
        for (int i = 0; i < currentCount; i++)
        {
            if (garageVehicles[i] != null)
                Debug.Log($"[{i}] {garageVehicles[i].DisplayName}");
        }
    }
}
