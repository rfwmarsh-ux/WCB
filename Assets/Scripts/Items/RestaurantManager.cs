using UnityEngine;
using System.Collections.Generic;

public class RestaurantManager : MonoBehaviour
{
    public static RestaurantManager Instance { get; private set; }

    private List<GameObject> restaurants = new List<GameObject>();

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
        CreateDefaultRestaurants();
    }

    private void CreateDefaultRestaurants()
    {
        SpawnRestaurant("Pizza Palace", new Vector2(450f, 450f));
        SpawnRestaurant("Burger Barn", new Vector2(550f, 550f));
        SpawnRestaurant("Chinese Kitchen", new Vector2(300f, 500f));
        SpawnRestaurant("Indian Spice", new Vector2(700f, 450f));

        Debug.Log($"Created {restaurants.Count} restaurants");
    }

    private void SpawnRestaurant(string name, Vector2 position)
    {
        GameObject restGO = new GameObject($"Restaurant_{name}");
        restGO.transform.position = (Vector3)position;
        restGO.tag = "Restaurant";

        SpriteRenderer sr = restGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.6f, 0.4f, 0.2f);
        sr.sortingOrder = 2;
        restGO.transform.localScale = new Vector3(10f, 8f, 1f);

        CircleCollider2D collider = restGO.AddComponent<CircleCollider2D>();
        collider.radius = 4f;
        collider.isTrigger = true;

        restaurants.Add(restGO);
    }

    public List<GameObject> GetAllRestaurants() => restaurants;
}
