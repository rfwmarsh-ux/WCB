using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages two gun shop locations in the city
/// </summary>
public class GunShopManager : MonoBehaviour
{
    public static GunShopManager Instance { get; private set; }
    private List<GunShop> gunShops = new List<GunShop>();

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
        InitializeGunShops();
    }

    private void InitializeGunShops()
    {
        // Two strategically placed gun shops
        SpawnGunShop("Gunz Galore", new Vector2(250f, 600f));
        SpawnGunShop("The Arsenal", new Vector2(750f, 400f));

        Debug.Log($"Initialized {gunShops.Count} gun shops");
    }

    private void SpawnGunShop(string name, Vector2 position)
    {
        GameObject shopGO = new GameObject($"GunShop_{name}");
        shopGO.transform.position = (Vector3)position;
        shopGO.tag = "GunShop";

        // Visual representation - red marker
        SpriteRenderer spriteRenderer = shopGO.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
        spriteRenderer.color = new Color(0.8f, 0f, 0f, 1f); // Red
        spriteRenderer.sortingOrder = 1;

        // Interaction collider
        CircleCollider2D collider = shopGO.AddComponent<CircleCollider2D>();
        collider.radius = 3f;
        collider.isTrigger = true;

        GunShop shop = shopGO.AddComponent<GunShop>();
        shopGO.name = name;

        shopGO.transform.parent = transform;
        gunShops.Add(shop);
    }

    public GunShop GetNearestGunShop(Vector2 playerPosition)
    {
        if (gunShops.Count == 0) return null;

        GunShop nearest = gunShops[0];
        float minDistance = Vector2.Distance(playerPosition, nearest.GetShopLocation());

        foreach (var shop in gunShops)
        {
            float distance = Vector2.Distance(playerPosition, shop.GetShopLocation());
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = shop;
            }
        }

        return nearest;
    }

    public List<GunShop> GetAllGunShops() => gunShops;
    public int GetGunShopCount() => gunShops.Count;
}
