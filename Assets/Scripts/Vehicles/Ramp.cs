using UnityEngine;

/// <summary>
/// A jump ramp that launches vehicles into the air
/// </summary>
public class Ramp : MonoBehaviour
{
    [SerializeField] private float rampLength = 3f;
    [SerializeField] private float rampHeight = 1.5f;
    [SerializeField] private float launchForce = 15f;
    [SerializeField] private float upwardBoost = 10f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D rampCollider;

    private int vehiclesLaunched = 0;
    private bool isActive = true;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (rampCollider == null)
            rampCollider = GetComponent<BoxCollider2D>();

        SetupRamp();
    }

    private void SetupRamp()
    {
        // Visual setup - ramp color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.8f, 0.6f, 0.2f, 0.9f); // Tan/brown
        }

        // Physics setup
        if (rampCollider != null)
        {
            rampCollider.size = new Vector2(rampLength, 0.5f);
            rampCollider.isTrigger = true;
        }

        // Set rotation to create ramp angle
        float angle = Mathf.Atan(rampHeight / rampLength) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        if (collision.CompareTag("Vehicle"))
        {
            Vehicle vehicle = collision.GetComponent<Vehicle>();
            if (vehicle != null)
            {
                LaunchVehicle(vehicle, collision.GetComponent<Rigidbody2D>());
            }
        }
    }

    private void LaunchVehicle(Vehicle vehicle, Rigidbody2D vehicleRb)
    {
        if (vehicleRb == null) return;

        Vector3 rampForward = transform.right;
        Vector3 launchDirection = (rampForward + transform.up).normalized;

        Vector2 launchVelocity = launchDirection * launchForce;
        launchVelocity.y += upwardBoost;

        vehicle.LaunchFromRamp(launchVelocity);

        vehiclesLaunched++;
        Debug.Log($"{vehicle.DisplayName} launched from ramp! Distance: {launchForce}m");

        SpawnJumpEffect(vehicle.transform.position);
    }

    private void SpawnJumpEffect(Vector3 position)
    {
        // Create a dust effect at launch point
        GameObject effectGO = new GameObject("JumpDust");
        effectGO.transform.position = position;

        ParticleSystem particles = effectGO.AddComponent<ParticleSystem>();
        
        var main = particles.main;
        main.duration = 0.5f;
        main.startLifetime = 1f;
        main.startSpeed = 3f;

        var emission = particles.emission;
        emission.rateOverTime = 20f;

        particles.Play();
        Destroy(effectGO, 2f);
    }

    public void DeactivateRamp()
    {
        isActive = false;
        spriteRenderer.color = Color.gray;
    }

    public void ActivateRamp()
    {
        isActive = true;
        spriteRenderer.color = new Color(0.8f, 0.6f, 0.2f, 0.9f);
    }

    public int GetVehiclesLaunched() => vehiclesLaunched;
    public bool IsActive() => isActive;
    public float GetRampLength() => rampLength;
    public float GetRampHeight() => rampHeight;
}
