using UnityEngine;
using System.Collections.Generic;

public class Bus : MonoBehaviour
{
    [SerializeField] private int capacity = 20;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float stopDuration = 5f;

    private List<BusStop> route;
    private int currentRouteIndex = 0;
    private int busNumber;
    private bool isMovingToStop = false;
    private bool isStopped = false;
    private float stopTimer = 0f;
    private Rigidbody2D rb;

    private List<Pedestrian> passengers = new List<Pedestrian>();
    private int maxPassengers;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(int busCapacity, float busSpeed, List<BusStop> allStops, int busNum)
    {
        maxPassengers = busCapacity;
        speed = busSpeed;
        busNumber = busNum;
        route = new List<BusStop>();

        foreach (var stop in allStops)
        {
            route.Add(stop);
        }

        if (route.Count > 0)
        {
            Vector2 startPos = route[0].Position;
            if (MapSystem.Instance != null)
            {
                startPos = MapSystem.Instance.GetLeftLanePosition(startPos);
            }
            transform.position = (Vector3)startPos;
            isMovingToStop = true;
        }
    }

    private void Update()
    {
        if (isStopped)
        {
            stopTimer -= Time.deltaTime;
            if (stopTimer <= 0)
            {
                Depart();
            }
            return;
        }

        if (route.Count == 0) return;

        BusStop targetStop = route[currentRouteIndex];
        Vector2 targetPos = targetStop.Position;
        if (MapSystem.Instance != null)
        {
            targetPos = MapSystem.Instance.GetLeftLanePosition(targetPos);
        }
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetPos);

        if (distance < 2f)
        {
            ArriveAtStop(targetStop);
        }
        else
        {
            rb.linearVelocity = direction * speed;
            if (direction.x != 0)
            {
                GetComponent<SpriteRenderer>().flipX = direction.x < 0;
            }
        }
    }

    private void ArriveAtStop(BusStop stop)
    {
        isMovingToStop = false;
        isStopped = true;
        rb.linearVelocity = Vector2.zero;

        Debug.Log($"Bus {busNumber} arrived at {stop.StopName}");

        BoardPassengers(stop);
    }

    private void BoardPassengers(BusStop stop)
    {
        List<Pedestrian> waiting = stop.GetWaitingPassengers();

        while (passengers.Count < maxPassengers && waiting.Count > 0)
        {
            Pedestrian p = waiting[0];
            stop.RemovePassenger(p);
            passengers.Add(p);
            p.transform.parent = transform;
            p.transform.localPosition = new Vector3(Random.Range(-2f, 2f), Random.Range(-0.5f, 0.5f), 0);
            
            waiting.RemoveAt(0);
            Debug.Log($"Passenger boarded Bus {busNumber}");
        }

        stopTimer = stopDuration;
    }

    private void Depart()
    {
        isStopped = false;
        
        currentRouteIndex = (currentRouteIndex + 1) % route.Count;
        isMovingToStop = true;

        Debug.Log($"Bus {busNumber} departed");
    }

    public bool HasFreeSeats()
    {
        return passengers.Count < maxPassengers;
    }

    public int GetPassengerCount() => passengers.Count;
    public int GetCapacity() => maxPassengers;
    public int GetBusNumber() => busNumber;
    public bool IsAtStop() => isStopped;
    public BusStop GetCurrentStop()
    {
        if (currentRouteIndex < route.Count)
            return route[currentRouteIndex];
        return null;
    }
}

public class BusInteraction : MonoBehaviour
{
    [SerializeField] private float interactionRange = 4f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            TryBoardBus();
        }
    }

    private void TryBoardBus()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange);
        
        foreach (var hit in hits)
        {
            Bus bus = hit.GetComponent<Bus>();
            if (bus != null)
            {
                if (bus.HasFreeSeats())
                {
                    BoardBus(bus);
                    return;
                }
                else
                {
                    Debug.Log("Bus is full!");
                    return;
                }
            }
        }

        Debug.Log("No bus nearby to board");
    }

    private void BoardBus(Bus bus)
    {
        Debug.Log($"Boarded Bus #{bus.GetBusNumber()} - Passengers: {bus.GetPassengerCount()}/{bus.GetCapacity()}");
        
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.SetCanMove(false);
            
            Bus currentBus = pc.GetComponent<Bus>();
            if (currentBus == null)
            {
                pc.gameObject.AddComponent<BusRide>();
                pc.GetComponent<BusRide>().StartRide(bus);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}

public class BusRide : MonoBehaviour
{
    private Bus currentBus;
    private float rideTimer = 0f;
    private bool riding = false;

    public void StartRide(Bus bus)
    {
        currentBus = bus;
        riding = true;
        Debug.Log("Enjoy your bus ride!");

        Invoke("EndRide", 10f);
    }

    private void EndRide()
    {
        riding = false;
        
        if (currentBus != null)
        {
            transform.parent = null;
            transform.position = currentBus.transform.position + Vector3.up * 3f;
            
            PlayerController pc = GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.SetCanMove(true);
            }
            
            Destroy(GetComponent<BusRide>());
            
            Debug.Log($"Bus ride ended at {BusManager.Instance.GetNearestStop(transform.position).StopName}");
        }
    }
}

public class BusVehicle : MonoBehaviour
{
    public static void CreateBusType()
    {
    }
}
