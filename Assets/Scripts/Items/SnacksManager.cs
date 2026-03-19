using UnityEngine;
using System.Collections.Generic;

public class SnacksManager : MonoBehaviour
{
    public static SnacksManager Instance { get; private set; }

    [SerializeField] private float respawnTime = 120f;

    private List<SnackPickup> activeSnacks = new List<SnackPickup>();
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
        InitializeSnackLocations();
    }

    private void InitializeSnackLocations()
    {
        List<Vector2> locations = new List<Vector2>
        {
            new Vector2(150f, 450f),
            new Vector2(350f, 300f),
            new Vector2(500f, 500f),
            new Vector2(650f, 350f),
            new Vector2(800f, 550f),
            new Vector2(300f, 650f),
            new Vector2(550f, 750f),
            new Vector2(750f, 200f),
            new Vector2(400f, 150f),
            new Vector2(200f, 600f)
        };

        string[] snackNames = { "Burger", "Pizza", "HotDog", "Sandwich", "Crisps", "Chocolate", "Soda", "Coffee", "IceCream", "Fries" };

        for (int i = 0; i < locations.Count; i++)
        {
            isRespawning.Add(false);
            SpawnSnack(locations[i], snackNames[i % snackNames.Length], i);
        }

        Debug.Log($"Spawned {locations.Count} snacks across the city");
    }

    private void SpawnSnack(Vector2 position, string snackName, int index)
    {
        GameObject snack = new GameObject($"Snack_{snackName}_{index}");
        snack.transform.position = (Vector3)position;

        SpriteRenderer sr = snack.AddComponent<SpriteRenderer>();
        sr.sprite = GTASpriteGenerator.GetPickupSprite(snackName.ToLower());
        sr.sortingOrder = 5;

        CircleCollider2D collider = snack.AddComponent<CircleCollider2D>();
        collider.radius = 1f;
        collider.isTrigger = true;

        SnackPickup pickup = snack.AddComponent<SnackPickup>();
        pickup.Initialize(snackName);
        activeSnacks.Add(pickup);
        snack.transform.parent = transform;
    }

    public void CollectSnack(SnackPickup snack)
    {
        int index = activeSnacks.IndexOf(snack);
        if (index < 0 || index >= activeSnacks.Count) return;

        if (!isRespawning[index])
        {
            isRespawning[index] = true;
            StartCoroutine(RespawnSnack(index));
        }
    }

    private System.Collections.IEnumerator RespawnSnack(int index)
    {
        yield return new WaitForSeconds(respawnTime);

        Vector2[] locations = new Vector2[]
        {
            new Vector2(150f, 450f),
            new Vector2(350f, 300f),
            new Vector2(500f, 500f),
            new Vector2(650f, 350f),
            new Vector2(800f, 550f),
            new Vector2(300f, 650f),
            new Vector2(550f, 750f),
            new Vector2(750f, 200f),
            new Vector2(400f, 150f),
            new Vector2(200f, 600f)
        };

        string[] snackNames = { "Burger", "Pizza", "HotDog", "Sandwich", "Crisps", "Chocolate", "Soda", "Coffee", "IceCream", "Fries" };

        SpawnSnack(locations[index], snackNames[index % snackNames.Length], index);
        isRespawning[index] = false;

        Debug.Log($"Snack respawned at {locations[index]}");
    }
}

public class SnackPickup : MonoBehaviour
{
    private string snackName;
    private float healthRestore;

    public void Initialize(string name)
    {
        snackName = name;
        healthRestore = Random.Range(10f, 25f);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = GTASpriteGenerator.GetPickupSprite(name.ToLower().Replace(" ", ""));
        sr.sortingOrder = 5;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager pm = other.GetComponent<PlayerManager>();
            if (pm != null)
            {
                pm.HealDamage(healthRestore);
                Debug.Log($"Ate {snackName} and restored {healthRestore} health!");
                SnacksManager.Instance?.CollectSnack(this);
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Player2"))
        {
            Player2Manager pm2 = other.GetComponent<Player2Manager>();
            if (pm2 != null)
            {
                pm2.HealDamage(healthRestore);
                Debug.Log($"P2 ate {snackName} and restored {healthRestore} health!");
                SnacksManager.Instance?.CollectSnack(this);
                Destroy(gameObject);
            }
        }
    }
}
