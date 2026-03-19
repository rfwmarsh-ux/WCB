using UnityEngine;

/// <summary>
/// Body armour pickup item
/// </summary>
public class BodyArmour : MonoBehaviour
{
    [SerializeField] private int armourValue = 50; // Protection points
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CircleCollider2D pickupCollider;

    private bool isActive = true;
    private float respawnTime = 30f; // Respawn after 30 seconds if taken

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (pickupCollider == null)
            pickupCollider = GetComponent<CircleCollider2D>();

        SetAppearance();
    }

    private void SetAppearance()
    {
        // Dark blue/tactical armor color
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0.2f, 0.2f, 0.6f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isActive)
        {
            PickupArmour(collision.GetComponent<PlayerManager>());
        }
    }

    private void PickupArmour(PlayerManager player)
    {
        if (player == null) return;

        player.GainArmour(armourValue);
        Debug.Log($"Picked up body armour! +{armourValue} armour");

        // Disable visuals and collider
        isActive = false;
        spriteRenderer.enabled = false;
        pickupCollider.enabled = false;

        // Schedule respawn
        Invoke("Respawn", respawnTime);
    }

    private void Respawn()
    {
        isActive = true;
        spriteRenderer.enabled = true;
        pickupCollider.enabled = true;
        Debug.Log("Body armour respawned");
    }

    public int GetArmourValue() => armourValue;
    public bool IsActive() => isActive;

    private void OnDrawGizmosSelected()
    {
        // Draw pickup zone in editor
        Gizmos.color = new Color(0.2f, 0.2f, 0.8f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}
