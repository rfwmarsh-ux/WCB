using UnityEngine;
using System.Collections.Generic;

public class StreetDetailsManager : MonoBehaviour
{
    public static StreetDetailsManager Instance { get; private set; }

    [SerializeField] private int manholeCount = 50;
    [SerializeField] private int crosswalkCount = 30;
    [SerializeField] private int drainCount = 40;

    private List<GameObject> streetDetails = new List<GameObject>();

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
        GenerateStreetDetails();
    }

    private void GenerateStreetDetails()
    {
        float mapWidth = 1000f;
        float mapHeight = 1000f;

        for (int i = 0; i < manholeCount; i++)
        {
            Vector2 pos = new Vector2(Random.Range(50f, mapWidth - 50f), Random.Range(50f, mapHeight - 50f));
            CreateStreetDetail(pos, "manhole");
        }

        for (int i = 0; i < crosswalkCount; i++)
        {
            Vector2 pos = new Vector2(Random.Range(50f, mapWidth - 50f), Random.Range(50f, mapHeight - 50f));
            CreateStreetDetail(pos, "crosswalk");
        }

        for (int i = 0; i < drainCount; i++)
        {
            Vector2 pos = new Vector2(Random.Range(50f, mapWidth - 50f), Random.Range(50f, mapHeight - 50f));
            CreateStreetDetail(pos, "drain");
        }

        Debug.Log($"Created {streetDetails.Count} street details");
    }

    private void CreateStreetDetail(Vector2 position, string type)
    {
        GameObject detail = new GameObject($"Street_{type}");
        detail.transform.position = (Vector3)position;
        detail.transform.parent = transform;

        SpriteRenderer sr = detail.AddComponent<SpriteRenderer>();
        sr.sprite = GTASpriteGenerator.GetStreetSprite(type);
        sr.sortingOrder = 1;
        sr.color = new Color(0.9f, 0.9f, 0.95f);

        streetDetails.Add(detail);
    }

    public List<GameObject> GetStreetDetails()
    {
        return streetDetails;
    }
}
