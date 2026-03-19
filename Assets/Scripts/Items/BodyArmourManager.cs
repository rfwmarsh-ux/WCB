using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages three permanent body armour spawn locations
/// </summary>
public class BodyArmourManager : MonoBehaviour
{
    [SerializeField] private int numberOfArmourLocations = 3;
    private List<BodyArmour> armourLocations = new List<BodyArmour>();

    private void Start()
    {
        InitializeArmourLocations();
    }

    private void InitializeArmourLocations()
    {
        // Three strategic locations across the city
        SpawnArmourLocation("West End Safehouse", new Vector2(150f, 400f));
        SpawnArmourLocation("East Market Armoury", new Vector2(850f, 600f));
        SpawnArmourLocation("Northside Hideout", new Vector2(500f, 850f));

        Debug.Log($"Initialized {armourLocations.Count} body armour locations");
    }

    private void SpawnArmourLocation(string locationName, Vector2 position)
    {
        GameObject armourGO = new GameObject($"BodyArmour_{locationName}");
        armourGO.transform.position = (Vector3)position;
        armourGO.tag = "Item";

        // Visual representation - tactical armor vest shape
        SpriteRenderer spriteRenderer = armourGO.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
        spriteRenderer.color = new Color(0.2f, 0.2f, 0.6f, 1f); // Dark blue
        spriteRenderer.sortingOrder = 2;

        // Pickup collider
        CircleCollider2D collider = armourGO.AddComponent<CircleCollider2D>();
        collider.radius = 0.8f;
        collider.isTrigger = true;

        BodyArmour armour = armourGO.AddComponent<BodyArmour>();
        armourGO.name = locationName;

        armourGO.transform.parent = transform;
        armourLocations.Add(armour);
    }

    public BodyArmour GetNearestArmour(Vector2 playerPosition)
    {
        if (armourLocations.Count == 0) return null;

        BodyArmour nearest = null;
        float minDistance = float.MaxValue;

        foreach (var armour in armourLocations)
        {
            if (!armour.IsActive()) continue;

            float distance = Vector2.Distance(playerPosition, armour.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = armour;
            }
        }

        return nearest;
    }

    public List<BodyArmour> GetArmourLocations() => armourLocations;
    public int GetArmourLocationCount() => armourLocations.Count;
}
