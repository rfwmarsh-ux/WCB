using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the police system, wanted levels, and law enforcement presence
/// </summary>
public class PoliceSystem : MonoBehaviour
{
    [SerializeField] private int maxWantedLevel = 5;
    [SerializeField] private float wantedDecayRate = 0.1f;
    [SerializeField] private float policeResponseTime = 3f;

    private int currentWantedLevel = 0;
    private float wantedDecayTimer = 0f;
    private bool isPlayerWanted = false;
    private float responseTimer = 0f;

    private List<PoliceOfficer> activePoliceOfficers = new List<PoliceOfficer>();
    [SerializeField] private Transform policeSpawnParent;

    public enum CrimeType
    {
        VehicleTheft,
        Assault,
        Murder,
        Robbery,
        HitAndRun,
        RecklessDriving,
        BreakingAndEntering
    }

    private Dictionary<CrimeType, int> crimeSeverity = new Dictionary<CrimeType, int>()
    {
        { CrimeType.VehicleTheft, 1 },
        { CrimeType.Assault, 2 },
        { CrimeType.Murder, 3 },
        { CrimeType.Robbery, 2 },
        { CrimeType.HitAndRun, 2 },
        { CrimeType.RecklessDriving, 1 },
        { CrimeType.BreakingAndEntering, 1 }
    };

    public int CurrentWantedLevel => currentWantedLevel;
    public bool IsPlayerWanted => isPlayerWanted;

    private void Awake()
    {
        InitializePoliceManagers();
    }

    private void InitializePoliceManagers()
    {
        if (FindObjectOfType<WantedLevelManager>() == null)
        {
            new GameObject("WantedLevelManager").AddComponent<WantedLevelManager>();
        }

        if (FindObjectOfType<PoliceStationManager>() == null)
        {
            new GameObject("PoliceStationManager").AddComponent<PoliceStationManager>();
        }

        if (FindObjectOfType<PoliceVehicleManager>() == null)
        {
            new GameObject("PoliceVehicleManager").AddComponent<PoliceVehicleManager>();
        }
    }

    private void Start()
    {
        currentWantedLevel = 0;
        isPlayerWanted = false;
    }

    private void Update()
    {
        UpdateWantedLevel();
        ManagePolicePresence();
        CleanupDeadOfficers();
    }

    private void UpdateWantedLevel()
    {
        if (currentWantedLevel > 0)
        {
            wantedDecayTimer += Time.deltaTime;
            if (wantedDecayTimer >= 1f)
            {
                // Decay wanted level over time if no new crimes
                currentWantedLevel = Mathf.Max(0, currentWantedLevel - 1);
                wantedDecayTimer = 0f;
                
                if (currentWantedLevel == 0)
                    isPlayerWanted = false;
            }
        }
    }

    private void ManagePolicePresence()
    {
        // Disabled - Police now handled by PoliceVehicleManager and PoliceOfficer
        // The new police system uses vehicles and officers that deploy from vans
        // Keeping this method for compatibility but not spawning foot officers
    }

    private int GetPoliceCountForWantedLevel()
    {
        return currentWantedLevel switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 3,
            4 => 4,
            5 => 5,
            _ => 5
        };
    }

    // DEPRECATED - Police now handled by PoliceVehicleManager and PoliceOfficer in Police folder
    // Keeping for reference but not used
    /*
    private void SpawnPoliceOfficer()
    {
        Vector3 spawnPosition = GetRandomSpawnLocation();
        
        GameObject policeGO = new GameObject("PoliceOfficer");
        policeGO.transform.position = spawnPosition;
        policeGO.tag = "Police";

        // Add visuals
        SpriteRenderer spriteRenderer = policeGO.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
        spriteRenderer.color = new Color(0f, 0.2f, 0.8f); // Dark blue
        spriteRenderer.sortingOrder = 2;

        // Add collider
        CircleCollider2D collider = policeGO.AddComponent<CircleCollider2D>();
        collider.radius = 0.3f;

        // Add rigidbody
        Rigidbody2D rb = policeGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;

        // Add police officer script
        PoliceOfficer officer = policeGO.AddComponent<PoliceOfficer>();
        officer.SetWantedLevel(currentWantedLevel);

        if (policeSpawnParent != null)
            policeGO.transform.parent = policeSpawnParent;

        activePoliceOfficers.Add(officer);
        Debug.Log($"Police officer spawned. Total active: {activePoliceOfficers.Count}");
    }
    */

    private Vector3 GetRandomSpawnLocation()
    {
        // Spawn at edges of map
        float x = Random.value > 0.5f ? Random.Range(100f, 200f) : Random.Range(800f, 900f);
        float y = Random.Range(100f, 900f);
        return new Vector3(x, y, 0f);
    }

    private void CleanupDeadOfficers()
    {
        activePoliceOfficers.RemoveAll(officer => officer == null || !officer.gameObject.activeSelf);
    }

    public void CommitCrime(CrimeType crime)
    {
        int severity = crimeSeverity[crime];
        AddWantedLevel(severity);
        Debug.Log($"Crime committed: {crime}. Wanted level: {currentWantedLevel}");
    }

    public void AddWantedLevel(int amount)
    {
        currentWantedLevel = Mathf.Min(maxWantedLevel, currentWantedLevel + amount);
        isPlayerWanted = currentWantedLevel > 0;
        wantedDecayTimer = 0f;
        GameManager.Instance.UpdateWantedLevel(amount);
        
        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.IncreaseWantedLevel(1, amount);
        }
    }

    public void ReduceWantedLevel(int amount)
    {
        currentWantedLevel = Mathf.Max(0, currentWantedLevel - amount);
        if (currentWantedLevel == 0)
            isPlayerWanted = false;
            
        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.DecreaseWantedLevel(1, amount);
        }
    }

    public List<PoliceOfficer> GetActiveOfficers()
    {
        return activePoliceOfficers;
    }

    public string GetWantedDescription()
    {
        return currentWantedLevel switch
        {
            0 => "Clean",
            1 => "Minor wanted",
            2 => "Wanted",
            3 => "Highly wanted",
            4 => "Extreme wanted",
            5 => "WANTED DEAD OR ALIVE",
            _ => "Unknown"
        };
    }

    public void SetWantedLevel(int level)
    {
        currentWantedLevel = Mathf.Clamp(level, 0, maxWantedLevel);
        isPlayerWanted = currentWantedLevel > 0;
    }
}
