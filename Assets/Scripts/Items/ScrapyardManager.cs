using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages two scrapyard locations for vehicle respraying and wanted level reset
/// </summary>
public class ScrapyardManager : MonoBehaviour
{
    private List<Scrapyard> scrapyards = new List<Scrapyard>();

    private void Start()
    {
        InitializeScrapyards();
    }

    private void InitializeScrapyards()
    {
        SpawnScrapyard("Downtown Scrapyard", new Vector2(150f, 250f), new Vector2(40f, 30f));
        SpawnScrapyard("Industrial Scrapyard", new Vector2(850f, 750f), new Vector2(45f, 35f));
        SpawnScrapyard("Northside Scrapyard", new Vector2(750f, 200f), new Vector2(35f, 28f));

        Debug.Log($"Initialized {scrapyards.Count} scrapyards");
    }

    private void SpawnScrapyard(string name, Vector2 position, Vector2 size)
    {
        GameObject scrapyardGO = new GameObject($"Scrapyard_{name}");
        scrapyardGO.transform.position = (Vector3)position;
        scrapyardGO.tag = "Scrapyard";

        SpriteRenderer spriteRenderer = scrapyardGO.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
        spriteRenderer.color = new Color(0.45f, 0.35f, 0.25f, 1f);
        spriteRenderer.sortingOrder = 1;

        scrapyardGO.transform.localScale = new Vector3(size.x, size.y, 1f);

        CircleCollider2D collider = scrapyardGO.AddComponent<CircleCollider2D>();
        collider.radius = Mathf.Max(size.x, size.y) * 0.5f;
        collider.isTrigger = true;

        Scrapyard scrapyard = scrapyardGO.AddComponent<Scrapyard>();
        scrapyardGO.name = name;

        CreateScrapyardDetails(scrapyardGO.transform, position, size);

        scrapyardGO.transform.parent = transform;
        scrapyards.Add(scrapyard);
    }
    
    private void CreateScrapyardDetails(Transform parent, Vector2 center, Vector2 size)
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject car = new GameObject($"ScrapCar_{i}");
            car.transform.position = center + new Vector2(Random.Range(-size.x/3f, size.x/3f), Random.Range(-size.y/3f, size.y/3f));
            car.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-45f, 45f));
            car.transform.parent = parent;
            
            SpriteRenderer sr = car.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.color = new Color(0.3f, 0.25f, 0.2f, 1f);
            sr.sortingOrder = 2;
            
            car.transform.localScale = new Vector3(5f, 2.5f, 1f);
        }
        
        for (int i = 0; i < 3; i++)
        {
            GameObject crane = new GameObject($"Crane_{i}");
            crane.transform.position = center + new Vector2(Random.Range(-size.x/4f, size.x/4f), size.y/3f);
            crane.transform.parent = parent;
            
            SpriteRenderer sr = crane.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.color = new Color(0.6f, 0.5f, 0.4f, 1f);
            sr.sortingOrder = 2;
            
            crane.transform.localScale = new Vector3(3f, 8f, 1f);
        }
    }

    public Scrapyard GetNearestScrapyard(Vector2 playerPosition)
    {
        if (scrapyards.Count == 0) return null;

        Scrapyard nearest = scrapyards[0];
        float minDistance = Vector2.Distance(playerPosition, nearest.GetLocation());

        foreach (var scrapyard in scrapyards)
        {
            float distance = Vector2.Distance(playerPosition, scrapyard.GetLocation());
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = scrapyard;
            }
        }

        return nearest;
    }

    public List<Scrapyard> GetAllScrapyards() => scrapyards;
    public int GetScrapyardCount() => scrapyards.Count;
}
