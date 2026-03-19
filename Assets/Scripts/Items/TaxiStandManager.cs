using UnityEngine;
using System.Collections.Generic;

public class TaxiStand : MonoBehaviour
{
}

public class TaxiStandManager : MonoBehaviour
{
    public static TaxiStandManager Instance { get; private set; }

    private List<GameObject> taxiStands = new List<GameObject>();

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
        CreateDefaultStands();
    }

    private void CreateDefaultStands()
    {
        SpawnTaxiStand("Central Taxi", new Vector2(500f, 480f));
        SpawnTaxiStand("Station Taxi", new Vector2(400f, 350f));
        SpawnTaxiStand("Shopping Taxi", new Vector2(600f, 600f));

        Debug.Log($"Created {taxiStands.Count} taxi stands");
    }

    private void SpawnTaxiStand(string name, Vector2 position)
    {
        GameObject standGO = new GameObject($"TaxiStand_{name}");
        standGO.transform.position = (Vector3)position;
        standGO.tag = "TaxiStand";

        SpriteRenderer sr = standGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = Color.yellow;
        sr.sortingOrder = 2;
        standGO.transform.localScale = new Vector3(6f, 4f, 1f);

        CircleCollider2D collider = standGO.AddComponent<CircleCollider2D>();
        collider.radius = 3f;
        collider.isTrigger = true;

        standGO.AddComponent<TaxiStand>();
        taxiStands.Add(standGO);
    }

    public List<GameObject> GetAllStands() => taxiStands;
}
