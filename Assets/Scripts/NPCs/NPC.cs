using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 50f;
    protected float currentHealth;
    protected bool isAlive = true;
    protected int killerPlayerId = 1;
    protected bool isInCombat = false;

    private float deathTime;
    private bool cleanupScheduled = false;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isAlive = false;
        currentHealth = 0;
        deathTime = Time.time;
        Debug.Log($"{gameObject.name} has died");
        
        SpawnLoot();
        ReportMurder();
        ReportDeadBody();
        cleanupScheduled = true;
    }

    private void ReportDeadBody()
    {
        if (AmbulanceManager.Instance != null)
        {
            AmbulanceManager.Instance.ReportDeadBody(transform.position, this);
        }
    }

    private void SpawnLoot()
    {
        Vector2 spawnPos = transform.position;

        if (Random.value < 0.3f)
        {
            float cashAmount = Random.Range(10f, 50f);
            GameObject cash = new GameObject("CashDrop");
            cash.transform.position = spawnPos;
            SpriteRenderer sr = cash.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.color = Color.green;
            sr.sortingOrder = 5;
            CircleCollider2D collider = cash.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            collider.isTrigger = true;
            cash.AddComponent<CashDrop>().Initialize(cashAmount, spawnPos);
        }

        if (Random.value < 0.1f)
        {
            Gun[] possibleGuns = new Gun[]
            {
                new Gun(Gun.GunType.Pistol, "Pistol", 15f, 0.3f, 12, 30f, 0.85f, 200f),
                new Gun(Gun.GunType.SMG, "SMG", 10f, 0.1f, 30, 25f, 0.7f, 400f),
                new Gun(Gun.GunType.Revolver, "Revolver", 25f, 0.5f, 6, 35f, 0.9f, 350f),
                new Gun(Gun.GunType.Shotgun, "Shotgun", 20f, 0.6f, 8, 15f, 0.6f, 500f)
            };

            Gun randomGun = possibleGuns[Random.Range(0, possibleGuns.Length)];
            GameObject weapon = new GameObject("WeaponDrop");
            weapon.transform.position = spawnPos;
            SpriteRenderer ws = weapon.AddComponent<SpriteRenderer>();
            ws.sprite = SpriteHelper.GetDefaultSprite();
            ws.sortingOrder = 5;
            CircleCollider2D wCollider = weapon.AddComponent<CircleCollider2D>();
            wCollider.radius = 1f;
            wCollider.isTrigger = true;
            weapon.AddComponent<WeaponDrop>().Initialize(randomGun, spawnPos);
        }
    }

    private void Update()
    {
        if (cleanupScheduled && isAlive == false)
        {
            if (Time.time - deathTime > 120f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ReportMurder()
    {
        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.ReportCrime(killerPlayerId, WantedLevelManager.CrimeType.Murder);
            Debug.Log($"Murder reported! Player {killerPlayerId} killed a pedestrian");
        }
    }

    public void SetKillerPlayer(int playerId)
    {
        killerPlayerId = playerId;
    }

    public void RequestCleanup()
    {
        if (AmbulanceManager.Instance != null)
        {
            AmbulanceManager.Instance.RemoveDeadPedestrianByNpc(this);
        }
        Destroy(gameObject);
    }

    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsAlive() => isAlive;
    public bool IsInCombat() => isInCombat;
    public void SetInCombat(bool combat) => isInCombat = combat;
}