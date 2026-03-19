using UnityEngine;

/// <summary>
/// Scrapyard location where player can respray vehicle and lose wanted level
/// </summary>
public class Scrapyard : MonoBehaviour
{
    [SerializeField] private string scrapyardName = "Scrapyard";
    [SerializeField] private Vector2 location;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float resprayBaseCost = 500f;
    
    [SerializeField] private float scrapYardRadius = 15f;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        location = transform.position;
        SetAppearance();
        
        gameObject.tag = "Scrapyard";
    }

    private void SetAppearance()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0.5f, 0.4f, 0.3f, 1f);
    }

    public void ResprayVehicle(Vehicle vehicle, PlayerManager player, GameManager gameManager)
    {
        if (vehicle == null)
        {
            Debug.Log("No vehicle to respray!");
            return;
        }

        float resprayCost = resprayBaseCost;

        if (!gameManager.CanAfford(resprayCost))
        {
            Debug.Log($"Cannot afford respray. Need: {resprayCost}, Have: {gameManager.Money}");
            return;
        }

        Color randomColor = new Color(Random.value, Random.value, Random.value, 1f);
        vehicle.GetComponent<SpriteRenderer>().color = randomColor;

        gameManager.AddMoney(-resprayCost);

        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.ClearWantedLevel(1);
            Debug.Log($"Player 1 wanted level cleared at {scrapyardName}!");
        }

        Debug.Log($"Vehicle resprayed at {scrapyardName}! Lost wanted level. Cost: {resprayCost}");
    }

    public void ResprayVehicleForPlayer2(Vehicle vehicle, Player2Manager player, GameManager gameManager)
    {
        if (vehicle == null)
        {
            Debug.Log("No vehicle to respray!");
            return;
        }

        float resprayCost = resprayBaseCost;

        if (!gameManager.CanAfford(resprayCost))
        {
            Debug.Log($"Cannot afford respray. Need: {resprayCost}, Have: {gameManager.Money}");
            return;
        }

        Color randomColor = new Color(Random.value, Random.value, Random.value, 1f);
        vehicle.GetComponent<SpriteRenderer>().color = randomColor;

        gameManager.AddMoney(-resprayCost);

        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.ClearWantedLevel(2);
            Debug.Log($"Player 2 wanted level cleared at {scrapyardName}!");
        }

        Debug.Log($"Player 2 vehicle resprayed at {scrapyardName}! Lost wanted level. Cost: {resprayCost}");
    }
    
    public bool IsPlayerInScrapyard(Vector2 playerPosition)
    {
        return Vector2.Distance(playerPosition, location) <= scrapYardRadius;
    }
    
    public float SellParkedVehicle(ParkedVehicle vehicle)
    {
        if (vehicle == null) return 0f;
        
        float value = vehicle.GetSellValue();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(value);
        }
        
        vehicle.OnVehicleTaken();
        Destroy(vehicle.gameObject);
        
        Debug.Log($"Sold {vehicle.GetVehicleType()} for ${value}");
        
        return value;
    }
    
    public float SellDestroyedVehicle(DestroyedVehicle vehicle)
    {
        if (vehicle == null) return 0f;
        
        float value = vehicle.GetScrapValue();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(value);
        }
        
        Destroy(vehicle.gameObject);
        
        Debug.Log($"Sold destroyed vehicle for ${value}");
        
        return value;
    }

    public string GetScrapyardName() => scrapyardName;
    public Vector2 GetLocation() => location;
    public float GetResprayBaseCost() => resprayBaseCost;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.4f, 0.3f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, scrapYardRadius);
    }
}
