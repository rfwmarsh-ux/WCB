using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all jump ramps throughout the city
/// </summary>
public class RampManager : MonoBehaviour
{
    [SerializeField] private int numberOfRamps = 6;
    [SerializeField] private float minDistance = 100f;
    [SerializeField] private Vector2 cityBoundsMin = Vector2.zero;
    [SerializeField] private Vector2 cityBoundsMax = new Vector2(1000f, 1000f);

    private List<Ramp> allRamps = new List<Ramp>();

    private void Start()
    {
        SpawnRamps();
    }

    private void SpawnRamps()
    {
        for (int i = 0; i < numberOfRamps; i++)
        {
            SpawnSingleRamp();
        }

        Debug.Log($"Spawned {allRamps.Count} jump ramps");
    }

    private void SpawnSingleRamp()
    {
        Vector3 spawnPosition = GetRandomValidPosition();
        float randomAngle = Random.Range(0f, 360f);

        GameObject rampGO = new GameObject($"Ramp_{allRamps.Count + 1}");
        rampGO.transform.position = spawnPosition;
        rampGO.transform.rotation = Quaternion.AngleAxis(randomAngle, Vector3.forward);
        rampGO.tag = "Obstacle";

        // Add sprite renderer
        SpriteRenderer spriteRenderer = rampGO.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
        spriteRenderer.color = new Color(0.8f, 0.6f, 0.2f, 0.9f); // Tan/brown
        spriteRenderer.sortingOrder = 1;

        // Add collider
        BoxCollider2D collider = rampGO.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(3f, 0.5f);
        collider.isTrigger = true;

        // Add ramp script
        Ramp ramp = rampGO.AddComponent<Ramp>();

        rampGO.transform.parent = transform;
        allRamps.Add(ramp);
    }

    private Vector3 GetRandomValidPosition()
    {
        Vector3 position;
        bool validPosition = false;

        do
        {
            float x = Random.Range(cityBoundsMin.x, cityBoundsMax.x);
            float y = Random.Range(cityBoundsMin.y, cityBoundsMax.y);
            position = new Vector3(x, y, 0f);

            // Check if position is far enough from other ramps
            validPosition = true;
            foreach (var ramp in allRamps)
            {
                if (Vector3.Distance(position, ramp.transform.position) < minDistance)
                {
                    validPosition = false;
                    break;
                }
            }
        } while (!validPosition);

        return position;
    }

    public List<Ramp> GetAllRamps()
    {
        return allRamps;
    }

    public Ramp GetNearestRamp(Vector3 position, float searchRadius = 30f)
    {
        Ramp nearest = null;
        float nearestDistance = searchRadius;

        foreach (var ramp in allRamps)
        {
            if (!ramp.IsActive()) continue;

            float distance = Vector3.Distance(ramp.transform.position, position);
            if (distance < nearestDistance)
            {
                nearest = ramp;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    public List<Ramp> GetRampsInArea(Vector3 position, float radius)
    {
        List<Ramp> rampsInArea = new List<Ramp>();

        foreach (var ramp in allRamps)
        {
            if (!ramp.IsActive()) continue;

            float distance = Vector3.Distance(ramp.transform.position, position);
            if (distance <= radius)
                rampsInArea.Add(ramp);
        }

        return rampsInArea;
    }

    public int GetTotalRampCount()
    {
        return allRamps.Count;
    }

    public int GetTotalVehiclesLaunched()
    {
        int total = 0;
        foreach (var ramp in allRamps)
        {
            total += ramp.GetVehiclesLaunched();
        }
        return total;
    }

    public void LogAllRamps()
    {
        Debug.Log($"=== Ramp Statistics ({allRamps.Count} total) ===");
        for (int i = 0; i < allRamps.Count; i++)
        {
            Ramp ramp = allRamps[i];
            Debug.Log($"[{i}] Ramp at {ramp.transform.position} - Active: {ramp.IsActive()} - Launches: {ramp.GetVehiclesLaunched()}");
        }
        Debug.Log($"Total vehicles launched: {GetTotalVehiclesLaunched()}");
    }
}
