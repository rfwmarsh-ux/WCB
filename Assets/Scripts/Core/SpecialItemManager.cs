using UnityEngine;
using System.Collections.Generic;

public class SpecialItemManager : MonoBehaviour
{
    public static SpecialItemManager Instance { get; private set; }

    [SerializeField] private GameObject cricketBatPrefab;

    private List<Vector2> cricketBatLocations = new List<Vector2>();
    private bool laserGunCollected = false;

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
        InitializeCricketBats();
        SpawnLaserGun();
    }

    private void InitializeCricketBats()
    {
        cricketBatLocations = new List<Vector2>
        {
            new Vector2(150f, 200f),
            new Vector2(300f, 400f),
            new Vector2(500f, 150f),
            new Vector2(700f, 250f),
            new Vector2(850f, 500f),
            new Vector2(200f, 700f),
            new Vector2(450f, 800f),
            new Vector2(650f, 650f)
        };

        SpawnCricketBats();
    }

    private void SpawnCricketBats()
    {
        foreach (Vector2 pos in cricketBatLocations)
        {
            GameObject bat = new GameObject($"CricketBat_{pos}");
            bat.transform.position = (Vector3)pos;
            
            SpriteRenderer sr = bat.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.color = new Color(0.8f, 0.6f, 0.3f);
            sr.sortingOrder = 5;
            
            CircleCollider2D collider = bat.AddComponent<CircleCollider2D>();
            collider.radius = 1.5f;
            collider.isTrigger = true;

            bat.AddComponent<CricketBat>();
            
            Collider2D[] existing = bat.GetComponents<Collider2D>();
            foreach (var c in existing)
            {
                c.enabled = true;
            }
            
            bat.transform.parent = transform;
        }

        Debug.Log($"Spawned 8 cricket bats at easy locations");
    }

    public void CollectCricketBat()
    {
        PlayerManager pm = PlayerManager.Instance;
        if (pm != null)
        {
            pm.EquipMeleeWeapon(CricketBat.Instance);
            Debug.Log("Picked up Cricket Bat - weak but better than fists!");
        }
    }

    public void SpawnLaserGun()
    {
        Vector2 hardLocation = new Vector2(950f, 950f);
        
        GameObject laser = new GameObject("LaserGunPickup");
        laser.transform.position = (Vector3)hardLocation;
        
        SpriteRenderer sr = laser.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = Color.cyan;
        sr.sortingOrder = 5;
        
        CircleCollider2D collider = laser.AddComponent<CircleCollider2D>();
        collider.radius = 2f;
        collider.isTrigger = true;

        laser.AddComponent<LaserGunPickup>();
        laser.transform.parent = transform;
        Debug.Log("Laser Cannon spawned at hard-to-reach location (950, 950)");
    }

    public void CollectLaserGun()
    {
        laserGunCollected = true;
        PlayerManager pm = PlayerManager.Instance;
        if (pm != null)
        {
            pm.EquipLaserGun();
            Debug.Log("Picked up Laser Cannon - ultimate weapon!");
        }
    }

    public bool HasLaserGun() => laserGunCollected;
}