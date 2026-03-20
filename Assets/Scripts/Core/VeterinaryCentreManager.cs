using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all veterinary centres and player respawn logic
/// </summary>
public class VeterinaryCentreManager : MonoBehaviour
{
    public static VeterinaryCentreManager Instance { get; private set; }
    [SerializeField] private int numberOfCentres = 4;
    private List<VeterinaryCentre> veterinaryCentres = new List<VeterinaryCentre>();
    
    private VeterinaryCentre player1LastCentre;
    private VeterinaryCentre player2LastCentre;

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
        InitializeVeterinaryCentres();
    }

    private void InitializeVeterinaryCentres()
    {
        // Create 4 veterinary centres at strategic locations across the city
        // City Centre
        SpawnVeterinaryCentre("Midland Veterinary Clinic", new Vector2(500f, 500f));
        
        // East Side
        SpawnVeterinaryCentre("Eastside Animal Hospital", new Vector2(800f, 500f));
        
        // West End
        SpawnVeterinaryCentre("West End Pet Care", new Vector2(200f, 500f));
        
        // North
        SpawnVeterinaryCentre("Northside Emergency Vets", new Vector2(500f, 800f));

        Debug.Log($"Initialized {veterinaryCentres.Count} veterinary centres");
    }

    private void SpawnVeterinaryCentre(string name, Vector2 position)
    {
        GameObject centreGO = new GameObject($"VeterinaryCentre_{name}");
        centreGO.transform.position = (Vector3)position;
        centreGO.tag = "VeterinaryCentre";

        // Visual representation
        SpriteRenderer spriteRenderer = centreGO.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
        spriteRenderer.color = new Color(0f, 0.8f, 0.2f, 1f); // Green
        spriteRenderer.sortingOrder = 1;

        // Collider for detection
        CircleCollider2D collider = centreGO.AddComponent<CircleCollider2D>();
        collider.radius = 5f;
        collider.isTrigger = true;

        VeterinaryCentre centre = centreGO.AddComponent<VeterinaryCentre>();
        centreGO.name = name;

        centreGO.transform.parent = transform;
        veterinaryCentres.Add(centre);
    }

    public VeterinaryCentre GetClosestCentre(Vector2 playerPosition)
    {
        return GetClosestCentre(playerPosition, null);
    }

    public VeterinaryCentre GetClosestCentre(Vector2 playerPosition, VeterinaryCentre excludeCentre)
    {
        if (veterinaryCentres.Count == 0)
        {
            Debug.LogError("No veterinary centres available!");
            return null;
        }

        VeterinaryCentre closest = null;
        float minDistance = float.MaxValue;

        foreach (var centre in veterinaryCentres)
        {
            if (centre == excludeCentre) continue;
            
            float distance = Vector2.Distance(playerPosition, centre.GetRespawnPosition());
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = centre;
            }
        }

        if (closest == null)
        {
            closest = GetClosestCentre(playerPosition);
        }

        return closest;
    }

    public void SetPlayer1LastCentre(VeterinaryCentre centre)
    {
        player1LastCentre = centre;
    }

    public void SetPlayer2LastCentre(VeterinaryCentre centre)
    {
        player2LastCentre = centre;
    }

    public VeterinaryCentre GetPlayer1LastCentre() => player1LastCentre;
    public VeterinaryCentre GetPlayer2LastCentre() => player2LastCentre;

    public void RespawnPlayerAtClosestCentre(PlayerManager playerManager, Vector2 playerLastPosition)
    {
        RespawnPlayerAtClosestCentre(playerManager, playerLastPosition, 1);
    }

    public void RespawnPlayerAtClosestCentre(Player2Manager playerManager, Vector2 playerLastPosition, int playerId)
    {
        VeterinaryCentre excludeCentre = null;
        
        if (playerId == 2 && GameModeManager.Instance != null && GameModeManager.Instance.IsVs())
        {
            excludeCentre = player1LastCentre;
        }
        
        VeterinaryCentre closestCentre = GetClosestCentre(playerLastPosition, excludeCentre);
        
        if (closestCentre != null)
        {
            if (playerId == 1)
                player1LastCentre = closestCentre;
            else if (playerId == 2)
                player2LastCentre = closestCentre;
                
            closestCentre.RespawnPlayer2(playerManager);
        }
        else
        {
            Debug.LogError("Failed to find a veterinary centre for respawn!");
        }
    }

    public void RespawnPlayerAtClosestCentre(PlayerManager playerManager, Vector2 playerLastPosition, int playerId)
    {
        VeterinaryCentre excludeCentre = null;
        
        if (playerId == 2 && GameModeManager.Instance != null && GameModeManager.Instance.IsVs())
        {
            excludeCentre = player1LastCentre;
        }
        
        VeterinaryCentre closestCentre = GetClosestCentre(playerLastPosition, excludeCentre);
        
        if (closestCentre != null)
        {
            if (playerId == 1)
                player1LastCentre = closestCentre;
            else if (playerId == 2)
                player2LastCentre = closestCentre;
                
            closestCentre.RespawnPlayer(playerManager);
        }
        else
        {
            Debug.LogError("Failed to find a veterinary centre for respawn!");
        }
    }

    public List<VeterinaryCentre> GetAllCentres() => veterinaryCentres;
    public int GetCentreCount() => veterinaryCentres.Count;
}
