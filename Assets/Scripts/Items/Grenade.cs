using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour
{
    public static Grenade Instance { get; private set; }

    [SerializeField] private float fuseTime = 2f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDamage = 60f;
    [SerializeField] private GameObject explosionEffect;

    public string Name => "Hand Grenade";
    public int Quantity { get; private set; } = 1;
    public int MaxCarried => 10;
    public float Cost => 0;

    private bool isActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddGrenades(int amount)
    {
        Quantity = Mathf.Min(Quantity + amount, MaxCarried);
        Debug.Log($"Grenades: {Quantity}/{MaxCarried}");
    }

    public void UseGrenade()
    {
        if (Quantity > 0)
        {
            Quantity--;
        }
    }

    public int GetQuantity() => Quantity;

    public GameObject Throw(Vector2 position, Vector2 direction, float throwForce)
    {
        if (Quantity <= 0)
        {
            Debug.Log("No grenades left!");
            return null;
        }

        UseGrenade();

        GameObject grenade = new GameObject("GrenadeProjectile");
        grenade.transform.position = (Vector3)position;
        
        SpriteRenderer sr = grenade.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = Color.gray;
        sr.sortingOrder = 5;

        Rigidbody2D rb = grenade.AddComponent<Rigidbody2D>();
        rb.linearVelocity = direction * throwForce;
        rb.gravityScale = 0.5f;

        CircleCollider2D collider = grenade.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;

        GrenadeProjectile gp = grenade.AddComponent<GrenadeProjectile>();
        gp.Setup(fuseTime, explosionRadius, explosionDamage);
        
        return grenade;
    }
}