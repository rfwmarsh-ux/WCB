using UnityEngine;
using System.Collections.Generic;

public class TowTruckManager : MonoBehaviour
{
    public static TowTruckManager Instance { get; private set; }
    
    private List<TowTruck> activeTowTrucks = new List<TowTruck>();
    private List<TowTruck> availableTowTrucks = new List<TowTruck>();
    private int maxScrapyardTowTrucks = 2;
    private float towTruckSpawnInterval = 30f;
    private float spawnTimer = 0f;
    
    private float playerTowTruckStartHour = 18f;
    private float playerTowTruckEndHour = 8f;
    private GameObject playerTowTruck;
    private bool isPlayerUsingTowTruck = false;
    private DestroyedVehicle vehicleBeingTowed;
    private bool isTowingToScrapyard = false;
    private Vector2 scrapyardDestination;
    
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
        SpawnInitialTowTrucks();
    }
    
    private void Update()
    {
        UpdateScrapyardTowTrucks();
        UpdatePlayerTowTruck();
    }
    
    private void UpdateScrapyardTowTrucks()
    {
        spawnTimer += Time.deltaTime;
        
        int activeFromScrapyard = 0;
        foreach (var truck in activeTowTrucks)
        {
            if (truck != null && truck.IsFromScrapyard())
            {
                activeFromScrapyard++;
            }
        }
        
        if (spawnTimer >= towTruckSpawnInterval && activeFromScrapyard < maxScrapyardTowTrucks)
        {
            SpawnScrapyardTowTruck();
            spawnTimer = 0f;
        }
    }
    
    private void UpdatePlayerTowTruck()
    {
        if (!IsPlayerTowTruckAvailable()) return;
        
        if (isTowingToScrapyard && vehicleBeingTowed != null)
        {
            UpdateTowingToScrapyard();
        }
    }
    
    private void UpdateTowingToScrapyard()
    {
        if (vehicleBeingTowed == null)
        {
            FinishTowing();
            return;
        }
        
        if (ScrapyardManager.Instance != null)
        {
            Scrapyard nearestScrapyard = ScrapyardManager.Instance.GetNearestScrapyard(playerTowTruck.transform.position);
            if (nearestScrapyard != null)
            {
                scrapyardDestination = nearestScrapyard.transform.position;
                
                Vector2 direction = (scrapyardDestination - (Vector2)playerTowTruck.transform.position).normalized;
                float distance = Vector2.Distance(playerTowTruck.transform.position, scrapyardDestination);
                
                playerTowTruck.transform.position += (Vector3)direction * 10f * Time.deltaTime;
                vehicleBeingTowed.transform.position = playerTowTruck.transform.position;
                
                if (distance < 5f)
                {
                    CompleteTowingAtScrapyard(nearestScrapyard);
                }
            }
        }
    }
    
    private void CompleteTowingAtScrapyard(Scrapyard scrapyard)
    {
        if (vehicleBeingTowed != null)
        {
            float value = vehicleBeingTowed.GetScrapValue();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddMoney(value);
                Debug.Log($"Sold destroyed vehicle for ${value}");
            }
            
            Destroy(vehicleBeingTowed.gameObject);
        }
        
        FinishTowing();
    }
    
    private void FinishTowing()
    {
        isTowingToScrapyard = false;
        vehicleBeingTowed = null;
        
        if (playerTowTruck != null)
        {
            playerTowTruck.SetActive(true);
        }
    }
    
    private void SpawnInitialTowTrucks()
    {
        for (int i = 0; i < maxScrapyardTowTrucks; i++)
        {
            SpawnScrapyardTowTruck();
        }
    }
    
    private void SpawnScrapyardTowTruck()
    {
        if (ScrapyardManager.Instance == null) return;
        
        List<Scrapyard> scrapyards = ScrapyardManager.Instance.GetAllScrapyards();
        if (scrapyards.Count == 0) return;
        
        Scrapyard scrapyard = scrapyards[Random.Range(0, scrapyards.Count)];
        
        Vector2 spawnPos = scrapyard.transform.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);
        
        GameObject truckGO = CreateTowTruck("ScrapyardTowTruck", spawnPos);
        TowTruck towTruck = truckGO.AddComponent<TowTruck>();
        towTruck.Initialize(true, scrapyard.transform.position);
        
        activeTowTrucks.Add(towTruck);
    }
    
    private GameObject CreateTowTruck(string name, Vector2 position)
    {
        GameObject truckGO = new GameObject(name);
        truckGO.transform.position = (Vector3)position;
        
        SpriteRenderer sr = truckGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.8f, 0.6f, 0.2f, 1f);
        sr.sortingOrder = 5;
        
        truckGO.transform.localScale = new Vector3(8f, 4f, 1f);
        
        Rigidbody2D rb = truckGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;
        
        BoxCollider2D col = truckGO.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
        
        truckGO.tag = "TowTruck";
        
        return truckGO;
    }
    
    public bool IsPlayerTowTruckAvailable()
    {
        if (DayNightCycleManager.Instance == null) return false;
        
        float currentHour = DayNightCycleManager.Instance.GetCurrentTime();
        
        if (playerTowTruckStartHour > playerTowTruckEndHour)
        {
            return currentHour >= playerTowTruckStartHour || currentHour < playerTowTruckEndHour;
        }
        else
        {
            return currentHour >= playerTowTruckStartHour && currentHour < playerTowTruckEndHour;
        }
    }
    
    public GameObject GetPlayerTowTruck()
    {
        if (!IsPlayerTowTruckAvailable()) return null;
        
        if (playerTowTruck == null)
        {
            Vector2 spawnPos = Vector2.zero;
            if (PlayerManager.Instance != null)
            {
                spawnPos = PlayerManager.Instance.transform.position + new Vector3(5f, 0, 0);
            }
            
            playerTowTruck = CreateTowTruck("PlayerTowTruck", spawnPos);
            playerTowTruck.SetActive(false);
        }
        
        return playerTowTruck;
    }
    
    public void StartTowingVehicle(DestroyedVehicle vehicle)
    {
        if (vehicle == null || playerTowTruck == null) return;
        
        vehicleBeingTowed = vehicle;
        vehicle.SetBeingTowed(true);
        isTowingToScrapyard = true;
        
        playerTowTruck.SetActive(false);
        
        Debug.Log("Started towing vehicle to scrapyard");
    }
    
    public bool IsTowingVehicle()
    {
        return isTowingToScrapyard && vehicleBeingTowed != null;
    }
    
    public void RequestTowTruckFromScrapyard(Vector2 destroyedVehiclePosition)
    {
        DestroyedVehicle nearestVehicle = DestroyedVehicleManager.Instance?.GetNearestDestroyedVehicle(destroyedVehiclePosition);
        
        if (nearestVehicle == null) return;
        
        foreach (var truck in activeTowTrucks)
        {
            if (truck != null && truck.IsAvailable() && truck.IsFromScrapyard())
            {
                truck.AssignPickup(nearestVehicle);
                return;
            }
        }
    }
    
    public void OnTowTruckArrived(TowTruck truck, DestroyedVehicle vehicle)
    {
        if (vehicle != null)
        {
            Destroy(vehicle.gameObject);
        }
        
        if (truck != null)
        {
            truck.ReturnToScrapyard();
        }
    }
    
    public void RegisterTowTruck(TowTruck truck)
    {
        if (!activeTowTrucks.Contains(truck))
        {
            activeTowTrucks.Add(truck);
        }
    }
    
    public void UnregisterTowTruck(TowTruck truck)
    {
        activeTowTrucks.Remove(truck);
    }
}

public class TowTruck : MonoBehaviour
{
    private bool isFromScrapyard;
    private Vector2 scrapyardPosition;
    private Vector2 targetPosition;
    private DestroyedVehicle pickupTarget;
    
    private enum TowState { Idle, MovingToPickup, PickingUp, MovingToScrapyard, Returning }
    private TowState currentState = TowState.Idle;
    
    private float speed = 15f;
    private bool isAvailable = true;
    
    public void Initialize(bool fromScrapyard, Vector2 scrapyardPos)
    {
        isFromScrapyard = fromScrapyard;
        scrapyardPosition = scrapyardPos;
        currentState = TowState.Idle;
        
        TowTruckManager.Instance?.RegisterTowTruck(this);
    }
    
    private void Update()
    {
        switch (currentState)
        {
            case TowState.MovingToPickup:
                MoveToTarget();
                break;
            case TowState.MovingToScrapyard:
                MoveToScrapyard();
                break;
            case TowState.Returning:
                MoveToScrapyard();
                break;
        }
    }
    
    private void MoveToTarget()
    {
        if (pickupTarget == null)
        {
            currentState = TowState.Returning;
            return;
        }
        
        targetPosition = pickupTarget.transform.position;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetPosition);
        
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        
        if (distance < 2f)
        {
            currentState = TowState.PickingUp;
            PickUpVehicle();
        }
    }
    
    private void MoveToScrapyard()
    {
        Vector2 direction = (scrapyardPosition - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, scrapyardPosition);
        
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        
        if (distance < 5f)
        {
            ArrivedAtScrapyard();
        }
    }
    
    private void PickUpVehicle()
    {
        if (pickupTarget != null)
        {
            pickupTarget.SetBeingTowed(true);
            TowTruckManager.Instance?.OnTowTruckArrived(this, pickupTarget);
            pickupTarget = null;
        }
        
        currentState = TowState.MovingToScrapyard;
        isAvailable = false;
    }
    
    private void ArrivedAtScrapyard()
    {
        currentState = TowState.Idle;
        isAvailable = true;
    }
    
    public void AssignPickup(DestroyedVehicle vehicle)
    {
        pickupTarget = vehicle;
        currentState = TowState.MovingToPickup;
        isAvailable = false;
    }
    
    public void ReturnToScrapyard()
    {
        currentState = TowState.Returning;
        pickupTarget = null;
    }
    
    public bool IsAvailable() => isAvailable && currentState == TowState.Idle;
    public bool IsFromScrapyard() => isFromScrapyard;
    
    private void OnDestroy()
    {
        TowTruckManager.Instance?.UnregisterTowTruck(this);
    }
}
