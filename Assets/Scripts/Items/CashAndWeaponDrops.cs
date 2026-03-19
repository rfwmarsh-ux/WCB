using UnityEngine;
using System.Collections;

public class CashDrop : MonoBehaviour
{
    private float cashAmount;
    private Vector2 bounceVelocity;
    private float gravity = -9.8f;
    private float lifetime = 10f;
    private bool isGrounded = false;
    private float groundY;

    private void Awake()
    {
        groundY = transform.position.y - 0.5f;
    }

    public void Initialize(float amount, Vector2 spawnPosition)
    {
        cashAmount = amount;
        transform.position = spawnPosition;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = GTASpriteGenerator.GetPickupSprite("cash");
        sr.sortingOrder = 5;

        bounceVelocity = new Vector2(Random.Range(-3f, 3f), Random.Range(5f, 8f));
        
        StartCoroutine(LifetimeTimer());
    }

    private void Update()
    {
        if (!isGrounded)
        {
            bounceVelocity.y += gravity * Time.deltaTime;
            transform.position += (Vector3)(bounceVelocity * Time.deltaTime);

            if (transform.position.y <= groundY)
            {
                transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
                isGrounded = true;
                bounceVelocity = Vector2.zero;
            }
        }

        transform.Rotate(0f, 0f, 90f * Time.deltaTime);
    }

    private IEnumerator LifetimeTimer()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager pm = other.GetComponent<PlayerManager>();
            if (pm != null)
            {
                CollectCash(pm);
            }
        }
        else if (other.CompareTag("Player2"))
        {
            Player2Manager pm2 = other.GetComponent<Player2Manager>();
            if (pm2 != null)
            {
                CollectCash(pm2);
            }
        }
    }

    private void CollectCash(PlayerManager pm)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(cashAmount);
        }
        Debug.Log($"Picked up ${cashAmount}!");
        Destroy(gameObject);
    }

    private void CollectCash(Player2Manager pm)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(cashAmount);
        }
        Debug.Log($"P2 picked up ${cashAmount}!");
        Destroy(gameObject);
    }
}

public class FoodPickup : MonoBehaviour
{
    private float healthRestore;
    private string foodType;

    public void Initialize(string type, float health)
    {
        foodType = type;
        healthRestore = health;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = GTASpriteGenerator.GetPickupSprite(type);
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
                Debug.Log($"Ate {foodType} and restored {healthRestore} health!");
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Player2"))
        {
            Player2Manager pm2 = other.GetComponent<Player2Manager>();
            if (pm2 != null)
            {
                pm2.HealDamage(healthRestore);
                Debug.Log($"P2 ate {foodType} and restored {healthRestore} health!");
                Destroy(gameObject);
            }
        }
    }
}

public class WeaponDrop : MonoBehaviour
{
    private Gun weapon;
    private float lifetime = 30f;

    public void Initialize(Gun gun, Vector2 spawnPosition)
    {
        weapon = gun;
        transform.position = spawnPosition;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = GTASpriteGenerator.GetWeaponSprite(gun.Type);
        sr.sortingOrder = 5;

        StartCoroutine(LifetimeTimer());
    }

    private IEnumerator LifetimeTimer()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager pm = other.GetComponent<PlayerManager>();
            if (pm != null && weapon != null)
            {
                pm.EquipGun(weapon);
                Debug.Log($"Picked up {weapon.Name}!");
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Player2"))
        {
            Player2Manager pm2 = other.GetComponent<Player2Manager>();
            if (pm2 != null && weapon != null)
            {
                pm2.EquipGun(weapon);
                Debug.Log($"P2 picked up {weapon.Name}!");
                Destroy(gameObject);
            }
        }
    }
}
