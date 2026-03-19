using UnityEngine;
using System.Collections.Generic;

public class Train : MonoBehaviour
{
    [SerializeField] private int capacity = 50;
    [SerializeField] private float speed = 25f;
    [SerializeField] private float stopDuration = 10f;

    private List<Vector2> route;
    private int currentWaypointIndex = 0;
    private int trainNumber;
    private bool isMoving = false;
    private bool isStopped = false;
    private float stopTimer = 0f;
    private Rigidbody2D rb;

    private List<Pedestrian> passengers = new List<Pedestrian>();
    private int maxPassengers;

    private enum TrainDirection
    {
        Forward,
        Reverse
    }

    private TrainDirection direction = TrainDirection.Forward;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(int trainCapacity, float trainSpeed, List<Vector2> routePath, int trainNum)
    {
        maxPassengers = trainCapacity;
        speed = trainSpeed;
        trainNumber = trainNum;
        route = new List<Vector2>(routePath);

        if (route.Count > 0)
        {
            transform.position = (Vector3)route[0];
            isMoving = true;
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

        Vector2 targetWaypoint = route[currentWaypointIndex];
        Vector2 directionVec = (targetWaypoint - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetWaypoint);

        if (distance < 3f)
        {
            if (IsAtTunnelPoint(targetWaypoint))
            {
                EnterTunnel();
                return;
            }

            ArriveAtWaypoint(targetWaypoint);
        }
        else
        {
            rb.linearVelocity = directionVec * speed;
            if (directionVec.x != 0)
            {
                GetComponent<SpriteRenderer>().flipX = directionVec.x < 0;
            }
        }
    }

    private bool IsAtTunnelPoint(Vector2 point)
    {
        float edgeX = 100f;
        float edgeY = 100f;
        float edgeXMax = 900f;
        float edgeYMax = 900f;

        return point.x < edgeX || point.x > edgeXMax || point.y < edgeY || point.y > edgeYMax;
    }

    private void EnterTunnel()
    {
        Debug.Log($"Train {trainNumber} entered tunnel and left the city");
        
        foreach (var passenger in passengers)
        {
            if (passenger != null)
            {
                Destroy(passenger.gameObject);
            }
        }
        passengers.Clear();
        
        Destroy(gameObject, 0.1f);
    }

    private void ArriveAtWaypoint(Vector2 waypoint)
    {
        isMoving = false;
        isStopped = true;
        rb.linearVelocity = Vector2.zero;

        Debug.Log($"Train {trainNumber} stopped at waypoint");

        TrainStation nearbyStation = GetNearbyStation(waypoint);
        if (nearbyStation != null)
        {
            Debug.Log($"Stopped near {nearbyStation.StationName}");
            BoardPassengers(nearbyStation);
        }

        stopTimer = stopDuration;
    }

    private TrainStation GetNearbyStation(Vector2 position)
    {
        TrainStation[] stations = FindObjectsOfType<TrainStation>();
        
        foreach (var station in stations)
        {
            if (Vector2.Distance(position, station.Position) < 10f)
            {
                return station;
            }
        }
        return null;
    }

    private void BoardPassengers(TrainStation station)
    {
        List<Pedestrian> waiting = station.GetWaitingPassengers();

        while (passengers.Count < maxPassengers && waiting.Count > 0)
        {
            Pedestrian p = waiting[0];
            station.RemovePassenger(p);
            passengers.Add(p);
            p.transform.parent = transform;
            p.transform.localPosition = new Vector3(Random.Range(-6f, 6f), Random.Range(-0.5f, 0.5f), 0);
            waiting.RemoveAt(0);
            Debug.Log($"Passenger boarded Train {trainNumber}");
        }
    }

    private void Depart()
    {
        isStopped = false;

        if (direction == TrainDirection.Forward)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= route.Count)
            {
                route.Reverse();
                currentWaypointIndex = 1;
                direction = TrainDirection.Reverse;
            }
        }
        else
        {
            currentWaypointIndex--;
            if (currentWaypointIndex < 0)
            {
                route.Reverse();
                currentWaypointIndex = 1;
                direction = TrainDirection.Forward;
            }
        }

        isMoving = true;
        Debug.Log($"Train {trainNumber} departed going {direction}");
    }

    public bool HasFreeSeats()
    {
        return passengers.Count < maxPassengers;
    }

    public int GetPassengerCount() => passengers.Count;
    public int GetCapacity() => maxPassengers;
    public int GetTrainNumber() => trainNumber;
    public bool IsAtStation() => isStopped;
}

public class TrainInteraction : MonoBehaviour
{
    [SerializeField] private float interactionRange = 6f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TryBoardTrain();
        }
    }

    private void TryBoardTrain()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange);
        
        foreach (var hit in hits)
        {
            Train train = hit.GetComponent<Train>();
            if (train != null)
            {
                if (train.HasFreeSeats())
                {
                    BoardTrain(train);
                    return;
                }
                else
                {
                    Debug.Log("Train is full!");
                    return;
                }
            }

            TrainStation station = hit.GetComponent<TrainStation>();
            if (station != null)
            {
                Debug.Log($"At {station.StationName} railway station");
                return;
            }
        }

        Debug.Log("No train nearby");
    }

    private void BoardTrain(Train train)
    {
        Debug.Log($"Boarded Train #{train.GetTrainNumber()} - Passengers: {train.GetPassengerCount()}/{train.GetCapacity()}");
        
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.SetCanMove(false);
            
            TrainRide ride = pc.GetComponent<TrainRide>();
            if (ride == null)
            {
                ride = pc.gameObject.AddComponent<TrainRide>();
                ride.StartRide(train);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}

public class TrainRide : MonoBehaviour
{
    private Train currentTrain;
    private bool riding = false;

    public void StartRide(Train train)
    {
        currentTrain = train;
        riding = true;
        Debug.Log("Enjoy your train journey!");

        Invoke("EndRide", 20f);
    }

    private void EndRide()
    {
        riding = false;
        
        if (currentTrain != null)
        {
            transform.parent = null;
            transform.position = currentTrain.transform.position + Vector3.up * 5f;
            
            PlayerController pc = GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.SetCanMove(true);
            }
            
            Destroy(GetComponent<TrainRide>());
            
            TrainStation station = FindObjectOfType<TrainStation>();
            if (station != null)
            {
                Debug.Log($"Train journey ended at {station.StationName}");
            }
        }
    }
}
