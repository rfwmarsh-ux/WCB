using UnityEngine;
using System.Collections.Generic;

public class OutfitShopManager : MonoBehaviour
{
    public static OutfitShopManager Instance { get; private set; }

    private List<OutfitShop> shops = new List<OutfitShop>();

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
        CreateDefaultShops();
    }

    private void CreateDefaultShops()
    {
        SpawnOutfitShop("City Centre Clothes", new Vector2(480f, 480f));
        SpawnOutfitShop("West End Fashions", new Vector2(200f, 480f));
        SpawnOutfitShop("Eastside Boutique", new Vector2(750f, 550f));

        Debug.Log($"Created {shops.Count} outfit shops");
    }

    private void SpawnOutfitShop(string name, Vector2 position)
    {
        GameObject shopGO = new GameObject($"OutfitShop_{name}");
        shopGO.transform.position = (Vector3)position;
        shopGO.tag = "ClothesShop";

        SpriteRenderer sr = shopGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.5f, 0.3f, 0.5f);
        sr.sortingOrder = 2;
        shopGO.transform.localScale = new Vector3(12f, 10f, 1f);

        CircleCollider2D collider = shopGO.AddComponent<CircleCollider2D>();
        collider.radius = 5f;
        collider.isTrigger = true;

        OutfitShop shop = shopGO.AddComponent<OutfitShop>();
        shops.Add(shop);
    }

    public List<OutfitShop> GetAllShops() => shops;
}
