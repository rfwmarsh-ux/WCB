using UnityEngine;

/// <summary>
/// Represents a veterinary centre (respawn/hospital location)
/// </summary>
public class VeterinaryCentre : MonoBehaviour
{
    [SerializeField] private string centreName = "Vet Centre";
    [SerializeField] private Vector2 respawnPosition;
    [SerializeField] private float healingCost = 100f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        respawnPosition = transform.position;
        SetAppearance();
    }

    private void SetAppearance()
    {
        // Green medical cross color
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0f, 0.8f, 0.2f, 1f);
    }

    public string GetCentreName() => centreName;
    public Vector2 GetRespawnPosition() => respawnPosition;
    public float GetHealingCost() => healingCost;
    
    public void RespawnPlayer(PlayerManager playerManager)
    {
        if (playerManager != null)
        {
            playerManager.Respawn(respawnPosition);
            GameManager.Instance.AddMoney(-healingCost);
            Debug.Log($"Player respawned at {centreName}. Healing cost: {healingCost}");
        }
    }

    public void RespawnPlayer2(Player2Manager playerManager)
    {
        if (playerManager != null)
        {
            playerManager.Respawn(respawnPosition);
            GameManager.Instance.AddMoney(-healingCost);
            Debug.Log($"Player 2 respawned at {centreName}. Healing cost: {healingCost}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw respawn zone in editor
        Gizmos.color = new Color(0f, 1f, 0.2f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, 2f);
        
        // Draw label
        UnityEditor.Handles.Label(transform.position + Vector3.up * 3f, centreName);
    }
}
