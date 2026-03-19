using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all roadworks/construction zones in the city
/// </summary>
public class RoadworksManager : MonoBehaviour
{
    [SerializeField] private int numberOfRoadworks = 8;
    [SerializeField] private float minDistance = 50f;
    [SerializeField] private Vector2 cityBoundsMin = Vector2.zero;
    [SerializeField] private Vector2 cityBoundsMax = new Vector2(1000f, 1000f);

    private List<Roadworks> activeRoadworks = new List<Roadworks>();

    private void Start()
    {
        SpawnRoadworks();
    }

    private void SpawnRoadworks()
    {
        for (int i = 0; i < numberOfRoadworks; i++)
        {
            SpawnSingleRoadwork();
        }

        Debug.Log($"Spawned {activeRoadworks.Count} roadwork zones");
    }

    private void SpawnSingleRoadwork()
    {
        Vector3 spawnPosition = GetRandomValidPosition();
        
        GameObject roadworksGO = new GameObject($"Roadworks_{activeRoadworks.Count + 1}");
        roadworksGO.transform.position = spawnPosition;
        roadworksGO.tag = "Obstacle";

        // Add sprite renderer
        SpriteRenderer spriteRenderer = roadworksGO.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
        spriteRenderer.color = new Color(1f, 0.5f, 0f, 0.8f); // Orange
        spriteRenderer.sortingOrder = 1;

        // Add collider
        BoxCollider2D collider = roadworksGO.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(3f, 3f);
        collider.isTrigger = false; // Solid obstacle

        // Add roadworks script
        Roadworks roadworks = roadworksGO.AddComponent<Roadworks>();
        
        roadworksGO.transform.parent = transform;
        activeRoadworks.Add(roadworks);
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

            // Check if position is far enough from other roadworks
            validPosition = true;
            foreach (var roadwork in activeRoadworks)
            {
                if (Vector3.Distance(position, roadwork.transform.position) < minDistance)
                {
                    validPosition = false;
                    break;
                }
            }
        } while (!validPosition);

        return position;
    }

    public List<Roadworks> GetAllRoadworks()
    {
        return activeRoadworks;
    }

    public Roadworks GetNearestRoadwork(Vector3 position, float searchRadius = 20f)
    {
        Roadworks nearest = null;
        float nearestDistance = searchRadius;

        foreach (var roadwork in activeRoadworks)
        {
            if (!roadwork.IsActive()) continue;
            
            float distance = Vector3.Distance(roadwork.transform.position, position);
            if (distance < nearestDistance)
            {
                nearest = roadwork;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    public List<Roadworks> GetRoadworksInArea(Vector3 position, float radius)
    {
        List<Roadworks> roadworksInArea = new List<Roadworks>();

        foreach (var roadwork in activeRoadworks)
        {
            if (!roadwork.IsActive()) continue;
            
            float distance = Vector3.Distance(roadwork.transform.position, position);
            if (distance <= radius)
                roadworksInArea.Add(roadwork);
        }

        return roadworksInArea;
    }

    public void TemporarilyCloseRoadwork(Roadworks roadwork, float duration)
    {
        roadwork.DeactivateRoadworks();
        StartCoroutine(ReopenRoadworkAfterDelay(roadwork, duration));
    }

    private System.Collections.IEnumerator ReopenRoadworkAfterDelay(Roadworks roadwork, float delay)
    {
        yield return new WaitForSeconds(delay);
        roadwork.ActivateRoadworks();
    }

    public int GetActiveRoadworkCount()
    {
        return activeRoadworks.FindAll(r => r.IsActive()).Count;
    }

    public void LogAllRoadworks()
    {
        Debug.Log($"=== Roadworks List ({activeRoadworks.Count} total) ===");
        for (int i = 0; i < activeRoadworks.Count; i++)
        {
            Roadworks rw = activeRoadworks[i];
            Debug.Log($"[{i}] {rw.GetZoneName()} at {rw.transform.position} - Active: {rw.IsActive()}");
        }
    }
}
