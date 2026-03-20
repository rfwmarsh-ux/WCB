using UnityEngine;
using System.Collections.Generic;

public class MetroTram : MonoBehaviour
{
    [SerializeField] private int capacity = 30;
    [SerializeField] private float speed = 15f;
    [SerializeField] private float stopDuration = 8f;

    private List<MetroStation> route;
    private int currentRouteIndex = 0;
    private int tramNumber;
    private bool isMovingToStop = false;
    private bool isStopped = false;
    private float stopTimer = 0f;
    private Rigidbody2D rb;

    private List<Pedestrian> passengers = new List<Pedestrian>();
    private int maxPassengers;

    private enum TramDirection
    {
        Forward,
        Backward
    }

    private TramDirection direction = TramDirection.Forward;
    private bool isStolen = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(int tramCapacity, float tramSpeed, List<MetroStation> stations, int tramNum)
    {
        maxPassengers = tramCapacity;
        speed = tramSpeed;
        tramNumber = tramNum;
        route = new List<MetroStation>(stations);
        route.Sort((a, b) => Vector2.Distance(a.Position, Vector2.zero).CompareTo(Vector2.Distance(b.Position, Vector2.zero)));

        if (route.Count > 0)
        {
            transform.position = (Vector3)route[0].Position + Vector3.right * 5f;
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

        MetroStation targetStation = route[currentRouteIndex];
        Vector2 directionVec = ((Vector2)targetStation.Position - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetStation.Position);

        if (distance < 2f)
        {
            ArriveAtStation(targetStation);
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

    private void ArriveAtStation(MetroStation station)
    {
        isMovingToStop = false;
        isStopped = true;
        rb.linearVelocity = Vector2.zero;

        Debug.Log($"Metro tram {tramNumber} arrived at {station.StationName}");

        BoardPassengers(station);
    }

    private void BoardPassengers(MetroStation station)
    {
        List<Pedestrian> waiting = station.GetWaitingPassengers();

        while (passengers.Count < maxPassengers && waiting.Count > 0)
        {
            Pedestrian p = waiting[0];
            station.RemovePassenger(p);
            passengers.Add(p);
            p.transform.parent = transform;
            p.transform.localPosition = new Vector3(Random.Range(-4f, 4f), Random.Range(-0.5f, 0.5f), 0);
            waiting.RemoveAt(0);
            Debug.Log($"Passenger boarded Metro {tramNumber}");
        }

        stopTimer = stopDuration;
    }

    private void Depart()
    {
        isStopped = false;

        if (direction == TramDirection.Forward)
        {
            currentRouteIndex++;
            if (currentRouteIndex >= route.Count)
            {
                currentRouteIndex = route.Count - 1;
                direction = TramDirection.Backward;
            }
        }
        else
        {
            currentRouteIndex--;
            if (currentRouteIndex < 0)
            {
                currentRouteIndex = 0;
                direction = TramDirection.Forward;
            }
        }

        isMovingToStop = true;
        Debug.Log($"Metro {tramNumber} departed going {direction}");
    }

    public bool HasFreeSeats()
    {
        return passengers.Count < maxPassengers;
    }

    public int GetPassengerCount() => passengers.Count;
    public int GetCapacity() => maxPassengers;
    public int GetTramNumber() => tramNumber;
    public bool IsAtStation() => isStopped;
    public bool IsStopped() => isStopped;
    public bool IsMoving() => isMovingToStop && !isStopped;
    public float GetCurrentSpeed() => speed;
    public float GetMaxSpeed() => speed;
    public bool IsStolen() => isStolen;
    public void SetDriver(PlayerController driver) { }
    public MetroStation GetCurrentStation()
    {
        if (currentRouteIndex < route.Count)
            return route[currentRouteIndex];
        return null;
    }
}

public class MetroInteraction : MonoBehaviour
{
    [SerializeField] private float interactionRange = 5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            TryBoardMetro();
        }
    }

    private void TryBoardMetro()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange);
        
        foreach (var hit in hits)
        {
            MetroTram tram = hit.GetComponent<MetroTram>();
            if (tram != null)
            {
                if (tram.HasFreeSeats())
                {
                    BoardMetro(tram);
                    return;
                }
                else
                {
                    Debug.Log("Metro is full!");
                    return;
                }
            }

            MetroStation station = hit.GetComponent<MetroStation>();
            if (station != null)
            {
                BoardAtStation(station);
                return;
            }
        }

        Debug.Log("No Metro nearby");
    }

    private void BoardMetro(MetroTram tram)
    {
        Debug.Log($"Boarded Metro #{tram.GetTramNumber()} - Passengers: {tram.GetPassengerCount()}/{tram.GetCapacity()}");
        
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.SetCanMove(false);
            
            MetroRide ride = pc.GetComponent<MetroRide>();
            if (ride == null)
            {
                ride = pc.gameObject.AddComponent<MetroRide>();
                ride.StartRide(tram);
            }
        }
    }

    private void BoardAtStation(MetroStation station)
    {
        Debug.Log($"At {station.StationName} Metro station");
        MetroStation nearest = MetroManager.Instance.GetNearestStation(transform.position);
        if (nearest != null)
        {
            Debug.Log($"Nearest station: {nearest.StationName}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}

public class MetroRide : MonoBehaviour
{
    private MetroTram currentTram;
    private float rideTimer = 0f;
    private bool riding = false;

    public void StartRide(MetroTram tram)
    {
        currentTram = tram;
        riding = true;
        Debug.Log("Enjoy your Metro ride!");

        Invoke("EndRide", 15f);
    }

    private void EndRide()
    {
        riding = false;
        
        if (currentTram != null)
        {
            transform.parent = null;
            transform.position = currentTram.transform.position + Vector3.up * 5f;
            
            PlayerController pc = GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.SetCanMove(true);
            }
            
            Destroy(GetComponent<MetroRide>());
            
            MetroStation station = MetroManager.Instance.GetNearestStation(transform.position);
            if (station != null)
            {
                Debug.Log($"Metro ride ended at {station.StationName}");
            }
        }
    }
}
