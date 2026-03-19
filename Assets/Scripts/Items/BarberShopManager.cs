using UnityEngine;
using System.Collections.Generic;

public class BarberShop : MonoBehaviour
{
}

public class BarberShopManager : MonoBehaviour
{
    public static BarberShopManager Instance { get; private set; }

    private List<GameObject> barberShops = new List<GameObject>();

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
        SpawnBarberShop("Main Street Barbers", new Vector2(520f, 520f));
        SpawnBarberShop("Gentleman's Cut", new Vector2(350f, 450f));
        SpawnBarberShop("Style Studio", new Vector2(680f, 380f));

        Debug.Log($"Created {barberShops.Count} barber shops");
    }

    private void SpawnBarberShop(string name, Vector2 position)
    {
        GameObject shopGO = new GameObject($"BarberShop_{name}");
        shopGO.transform.position = (Vector3)position;
        shopGO.tag = "BarberShop";

        SpriteRenderer sr = shopGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.6f, 0.5f, 0.3f);
        sr.sortingOrder = 2;
        shopGO.transform.localScale = new Vector3(8f, 6f, 1f);

        CircleCollider2D collider = shopGO.AddComponent<CircleCollider2D>();
        collider.radius = 3f;
        collider.isTrigger = true;

        shopGO.AddComponent<BarberShop>();
        barberShops.Add(shopGO);
    }

    public List<GameObject> GetAllShops() => barberShops;
}
