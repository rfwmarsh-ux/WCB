using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrenadeSpawner : MonoBehaviour
{
    public static GrenadeSpawner Instance { get; private set; }

    [SerializeField] private float respawnTime = 60f;

    private List<Vector2> spawnLocations = new List<Vector2>();
    private List<GameObject> activeGrenades = new List<GameObject>();
    private List<bool> isRespawning = new List<bool>();

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
        InitializeSpawnLocations();
    }

    private void InitializeSpawnLocations()
    {
        spawnLocations = new List<Vector2>
        {
            new Vector2(250f, 300f),
            new Vector2(600f, 500f),
            new Vector2(800f, 200f)
        };

        for (int i = 0; i < spawnLocations.Count; i++)
        {
            isRespawning.Add(false);
        }

        SpawnAllGrenades();
    }

    private void SpawnAllGrenades()
    {
        for (int i = 0; i < spawnLocations.Count; i++)
        {
            SpawnGrenadeAt(i);
        }
    }

    private void SpawnGrenadeAt(int index)
    {
        GameObject grenade = new GameObject($"GrenadePickup_{index}");
        grenade.transform.position = (Vector3)spawnLocations[index];

        SpriteRenderer sr = grenade.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = Color.gray;
        sr.sortingOrder = 5;

        CircleCollider2D collider = grenade.AddComponent<CircleCollider2D>();
        collider.radius = 1f;
        collider.isTrigger = true;

        grenade.AddComponent<GrenadePickup>();
        grenade.transform.parent = transform;

        activeGrenades.Add(grenade);
    }

    public void CollectGrenade(int index)
    {
        if (index < 0 || index >= activeGrenades.Count) return;

        Grenade.Instance.AddGrenades(3);
        Destroy(activeGrenades[index]);
        activeGrenades[index] = null;

        if (!isRespawning[index])
        {
            isRespawning[index] = true;
            StartCoroutine(RespawnGrenade(index));
        }
    }

    private IEnumerator RespawnGrenade(int index)
    {
        yield return new WaitForSeconds(respawnTime);
        
        SpawnGrenadeAt(index);
        isRespawning[index] = false;
        
        Debug.Log($"Grenade respawned at {spawnLocations[index]}");
    }
}