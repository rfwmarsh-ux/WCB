using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RocketPool : MonoBehaviour
{
    public static RocketPool Instance { get; private set; }

    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private int initialPoolSize = 10;

    private Queue<GameObject> availableRockets = new Queue<GameObject>();
    private List<GameObject> activeRockets = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (rocketPrefab == null)
        {
            rocketPrefab = CreateRocketPrefab();
        }

        InitializePool();
    }

    private GameObject CreateRocketPrefab()
    {
        GameObject prefab = new GameObject("RocketPrefab");
        SpriteRenderer sr = prefab.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = Color.red;
        sr.sortingOrder = 10;

        CircleCollider2D collider = prefab.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        collider.isTrigger = true;

        Rigidbody2D rb = prefab.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        prefab.AddComponent<RocketProjectile>();
        prefab.SetActive(false);
        return prefab;
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewRocket();
        }
    }

    private GameObject CreateNewRocket()
    {
        GameObject rocket = Instantiate(rocketPrefab, transform);
        rocket.SetActive(false);
        availableRockets.Enqueue(rocket);
        return rocket;
    }

    public GameObject GetRocket()
    {
        if (availableRockets.Count == 0)
        {
            CreateNewRocket();
        }

        GameObject rocket = availableRockets.Dequeue();
        rocket.SetActive(true);
        activeRockets.Add(rocket);
        return rocket;
    }

    public void ReturnRocket(GameObject rocket)
    {
        rocket.SetActive(false);
        activeRockets.Remove(rocket);
        availableRockets.Enqueue(rocket);
    }

    public void ClearActiveRockets()
    {
        foreach (var rocket in activeRockets)
        {
            rocket.SetActive(false);
            availableRockets.Enqueue(rocket);
        }
        activeRockets.Clear();
    }
}

public class RocketProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed = 15f;
    private float damage;
    private float explosionRadius = 5f;
    private Vector2 startPosition;
    private float maxRange = 50f;
    private int shooterPlayer = 1;

    public void Setup(Vector2 position, Vector2 dir, float dmg, float range, int shooter = 1)
    {
        transform.position = position;
        direction = dir;
        damage = dmg;
        maxRange = range;
        startPosition = position;
        shooterPlayer = shooter;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (Vector2.Distance(startPosition, transform.position) > maxRange)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Vehicle"))
        {
            return;
        }

        bool isVsMode = GameModeManager.Instance != null && GameModeManager.Instance.IsVs();

        if (isVsMode)
        {
            if (other.name == "Player1" && shooterPlayer != 1)
            {
                VsModeSystem.Instance?.DealDamageToPlayer(1, damage);
                Explode();
                return;
            }
            if (other.name == "Player2" && shooterPlayer != 2)
            {
                VsModeSystem.Instance?.DealDamageToPlayer(2, damage);
                Explode();
                return;
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                return;
            }
        }

        Explode();
    }

    private void Explode()
    {
        CreateExplosionEffect();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            float distance = Vector2.Distance(transform.position, hit.transform.position);
            float damageFalloff = 1f - (distance / explosionRadius);
            float finalDamage = damage * Mathf.Clamp01(damageFalloff);

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(finalDamage);
            }

            NPC npc = hit.GetComponent<NPC>();
            if (npc != null)
            {
                npc.SetKillerPlayer(shooterPlayer);
                npc.TakeDamage(finalDamage);
            }

            Vehicle vehicle = hit.GetComponent<Vehicle>();
            if (vehicle != null)
            {
                vehicle.TakeDamage(finalDamage);
            }
        }

        ReturnToPool();
    }

    private void CreateExplosionEffect()
    {
        StartCoroutine(ExplosionEffectRoutine());
    }

    private IEnumerator ExplosionEffectRoutine()
    {
        GameObject explosion = new GameObject("RocketExplosion");
        explosion.transform.position = transform.position;

        SpriteRenderer sr = explosion.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(1f, 0.5f, 0f, 0.9f);
        sr.sortingOrder = 15;

        explosion.transform.localScale = Vector3.one * explosionRadius * 2;

        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            explosion.transform.localScale = Vector3.one * explosionRadius * 2 * (1f - t * 0.5f);
            sr.color = new Color(1f, 0.5f * (1f - t), 0f, 0.9f * (1f - t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        CreateSmokeEffect(explosion.transform.position);
        Destroy(explosion);

        Debug.Log($"Rocket exploded! Radius: {explosionRadius}, Damage: {damage}");
    }

    private void CreateSmokeEffect(Vector2 position)
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject smoke = new GameObject("Smoke");
            smoke.transform.position = position + Random.insideUnitCircle * explosionRadius * 0.5f;

            SpriteRenderer sr = smoke.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);
            sr.sortingOrder = 5;

            float size = Random.Range(1f, 3f);
            smoke.transform.localScale = Vector3.one * size;

            StartCoroutine(SmokeRiseAndFade(smoke));
        }
    }

    private IEnumerator SmokeRiseAndFade(GameObject smoke)
    {
        float duration = 2f;
        float elapsed = 0f;
        Vector3 startPos = smoke.transform.position;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            smoke.transform.position = startPos + Vector3.up * t * 3f;
            smoke.transform.localScale = Vector3.one * (1f - t) * 2f;

            SpriteRenderer sr = smoke.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0.3f, 0.3f, 0.3f, 0.7f * (1f - t));
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(smoke);
    }

    private void ReturnToPool()
    {
        RocketPool.Instance?.ReturnRocket(gameObject);
    }
}
