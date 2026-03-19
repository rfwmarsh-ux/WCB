using UnityEngine;
using System.Collections.Generic;

public class VehicleStealingSystem : MonoBehaviour
{
    public static VehicleStealingSystem Instance { get; private set; }
    
    private float stealRange = 5f;
    private float doorOpenDistance = 3f;
    
    private float fleeChance = 0.03f;
    private float passengerGunChance = 0.06f;
    
    private bool isStealingInProgress = false;
    private Vehicle beingStolen = null;
    
    private Dictionary<Vehicle, VehicleOccupants> vehicleOccupants = new Dictionary<Vehicle, VehicleOccupants>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Update()
    {
        CheckForPlayerProximity();
        
        if (isStealingInProgress && beingStolen != null)
        {
            CheckForStealingCancellation();
        }
    }
    
    private void CheckForStealingCancellation()
    {
        if (beingStolen == null) return;
        
        Vector2 playerPos = PlayerManager.Instance != null ? 
            PlayerManager.Instance.transform.position : Vector2.zero;
        
        float dist = Vector2.Distance(playerPos, beingStolen.transform.position);
        
        if (dist > stealRange * 1.5f)
        {
            CancelStealing();
        }
    }
    
    private void CancelStealing()
    {
        if (beingStolen != null)
        {
            if (beingStolen is TrafficVehicle tv)
            {
                tv.SetBeingStolen(false);
            }
            
            RemoveVehicleOccupants(beingStolen);
        }
        
        isStealingInProgress = false;
        beingStolen = null;
        
        Debug.Log("Stealing cancelled - player moved away!");
    }
    
    private void CheckForPlayerProximity()
    {
        if (isStealingInProgress) return;
        
        Vector2 playerPos = PlayerManager.Instance != null ? 
            PlayerManager.Instance.transform.position : Vector2.zero;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(playerPos, stealRange * 2f);
        
        foreach (var hit in hits)
        {
            TrafficVehicle tv = hit.GetComponent<TrafficVehicle>();
            if (tv != null && !tv.IsBeingStolen())
            {
                float dist = Vector2.Distance(playerPos, tv.transform.position);
                if (dist < stealRange)
                {
                    if (Random.value < fleeChance)
                    {
                        tv.StartErraticFlee();
                    }
                    else
                    {
                        tv.SlowDownForPlayer();
                    }
                }
            }
        }
    }
    
    public bool CanStealVehicle(Vehicle vehicle)
    {
        if (vehicle == null) return false;
        if (isStealingInProgress) return false;
        if (vehicle.VehicleType == VehicleType.PoliceCruiser) return false;
        
        if (vehicle is Train train)
        {
            return train.IsStolen() == false && !train.IsMoving();
        }
        
        if (vehicle is MetroTram metro)
        {
            return metro.IsStolen() == false && !metro.IsMoving();
        }
        
        return true;
    }
    
    public void StartStealingVehicle(Vehicle vehicle)
    {
        if (!CanStealVehicle(vehicle)) return;
        
        isStealingInProgress = true;
        beingStolen = vehicle;
        
        if (vehicle is Train train)
        {
            train.gameObject.AddComponent<TrainStealingHandler>();
            train.gameObject.GetComponent<TrainStealingHandler>().Initialize(train, this);
            return;
        }
        
        if (vehicle is MetroTram metro)
        {
            metro.gameObject.AddComponent<MetroStealingHandler>();
            metro.gameObject.GetComponent<MetroStealingHandler>().Initialize(metro, this);
            return;
        }
        
        if (vehicle is TrafficVehicle tv)
        {
            tv.SetBeingStolen(true);
        }
        
        if (IsMotorcycle(vehicle.VehicleType))
        {
            StealMotorcycle(vehicle);
        }
        else
        {
            StealCar(vehicle);
        }
    }
    
    private void StealCar(Vehicle vehicle)
    {
        TrafficVehicle tv = vehicle as TrafficVehicle;
        bool isParked = tv == null;
        
        if (tv != null)
        {
            tv.StopVehicle();
        }
        
        CreateOccupantsForVehicle(vehicle);
        
        StartCoroutine(StealingSequence(vehicle, isParked));
    }
    
    private System.Collections.IEnumerator StealingSequence(Vehicle vehicle, bool isParked)
    {
        yield return new WaitForSeconds(0.3f);
        
        if (isStealingInProgress && beingStolen == vehicle && vehicleOccupants.ContainsKey(vehicle))
        {
            VehicleOccupants occupants = vehicleOccupants[vehicle];
            
            EjectDriver(vehicle, occupants);
            yield return new WaitForSeconds(0.2f);
            
            if (occupants.passengers != null)
            {
                for (int i = 0; i < occupants.passengers.Count; i++)
                {
                    if (!isStealingInProgress || beingStolen != vehicle) yield break;
                    if (i > 0) yield return new WaitForSeconds(0.15f);
                    EjectPassenger(vehicle, occupants, i);
                }
            }
        }
        
        if (!isStealingInProgress || beingStolen != vehicle) yield break;
        
        yield return new WaitForSeconds(0.3f);
        
        if (!isStealingInProgress || beingStolen != vehicle) yield break;
        
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.EnterVehicle(vehicle);
            vehicle.SetDriver(PlayerManager.Instance.GetComponent<PlayerController>());
            
            GameObject player = PlayerManager.Instance.gameObject;
            player.transform.parent = vehicle.transform;
            player.transform.localPosition = Vector3.zero;
            player.SetActive(false);
        }
        
        yield return new WaitForSeconds(0.2f);
        
        if (isStealingInProgress && beingStolen == vehicle)
        {
            CompleteStealing(vehicle);
        }
    }
    
    private void StealMotorcycle(Vehicle vehicle)
    {
        if (!isStealingInProgress || beingStolen != vehicle) return;
        
        CreateOccupantsForVehicle(vehicle);
        
        if (vehicleOccupants.ContainsKey(vehicle))
        {
            VehicleOccupants occupants = vehicleOccupants[vehicle];
            
            if (occupants.driver != null)
            {
                ThrowOccupantOffMotorcycle(occupants.driver);
            }
        }
        
        if (!isStealingInProgress || beingStolen != vehicle) return;
        
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.EnterVehicle(vehicle);
            vehicle.SetDriver(PlayerManager.Instance.GetComponent<PlayerController>());
            
            GameObject player = PlayerManager.Instance.gameObject;
            player.transform.parent = vehicle.transform;
            player.transform.localPosition = Vector3.zero;
            player.SetActive(false);
        }
        
        CompleteStealing(vehicle);
    }
    
    private void CreateOccupantsForVehicle(Vehicle vehicle)
    {
        if (vehicleOccupants.ContainsKey(vehicle)) return;
        
        VehicleOccupants occupants = new VehicleOccupants();
        
        bool isTwoDoor = IsTwoDoorVehicle(vehicle.VehicleType);
        int maxPassengers = isTwoDoor ? 1 : 2;
        
        occupants.driver = CreateNPC("Driver", vehicle.transform.position);
        
        if (Random.value < 0.5f)
        {
            int passengerCount = Random.Range(0, maxPassengers + 1);
            for (int i = 0; i < passengerCount; i++)
            {
                GameObject passenger = CreateNPC($"Passenger{i + 1}", vehicle.transform.position);
                if (Random.value < passengerGunChance)
                {
                    GiveGunToNPC(passenger);
                }
                occupants.passengers.Add(passenger);
            }
        }
        
        vehicleOccupants[vehicle] = occupants;
    }
    
    private GameObject CreateNPC(string name, Vector2 position)
    {
        GameObject npc = new GameObject($"NPC_{name}");
        npc.transform.position = (Vector3)position;
        
        SpriteRenderer sr = npc.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.8f, 0.7f, 0.6f);
        sr.sortingOrder = 8;
        npc.transform.localScale = new Vector3(1.5f, 2f, 1f);
        
        Rigidbody2D rb = npc.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        
        CapsuleCollider2D col = npc.AddComponent<CapsuleCollider2D>();
        
        return npc;
    }
    
    private void GiveGunToNPC(GameObject npc)
    {
        NPCGunSystem gunSystem = npc.AddComponent<NPCGunSystem>();
        gunSystem.Initialize();
    }
    
    private void EjectDriver(Vehicle vehicle, VehicleOccupants occupants)
    {
        if (occupants.driver == null) return;
        
        Vector3 ejectDir = Random.value > 0.5f ? Vector3.right : Vector3.left;
        Vector3 ejectPos = vehicle.transform.position + ejectDir * doorOpenDistance;
        
        StartCoroutine(EjectOccupantSequence(occupants.driver, ejectPos, true));
    }
    
    private void EjectPassenger(Vehicle vehicle, VehicleOccupants occupants, int passengerIndex)
    {
        if (occupants.passengers == null || passengerIndex >= occupants.passengers.Count) return;
        
        GameObject passenger = occupants.passengers[passengerIndex];
        if (passenger == null) return;
        
        Vector3 ejectDir = passengerIndex == 0 ? Vector3.right : Vector3.left;
        Vector3 ejectPos = vehicle.transform.position + ejectDir * doorOpenDistance;
        
        StartCoroutine(EjectOccupantSequence(passenger, ejectPos, false));
    }
    
    private System.Collections.IEnumerator EjectOccupantSequence(GameObject npc, Vector3 ejectPos, bool isDriver)
    {
        float duration = 0.3f;
        Vector3 startPos = npc.transform.position;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            npc.transform.position = Vector3.Lerp(startPos, ejectPos, t);
            yield return null;
        }
        
        Rigidbody2D rb = npc.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float randomX = Random.Range(-2f, 2f);
            float randomY = Random.Range(3f, 5f);
            rb.linearVelocity = new Vector2(randomX, randomY);
        }
        
        StartCoroutine(MakeOccupantStandUp(npc));
    }
    
    private System.Collections.IEnumerator MakeOccupantStandUp(GameObject npc)
    {
        yield return new WaitForSeconds(0.5f);
        
        Rigidbody2D rb = npc.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
        
        npc.transform.localScale = new Vector3(1.5f, 2f, 1f);
    }
    
    private void ThrowOccupantOffMotorcycle(GameObject occupant)
    {
        Vector3 throwDir = (Random.value > 0.5f ? Vector3.right : Vector3.left) + Vector3.up * 0.5f;
        
        StartCoroutine(ThrowOccupantSequence(occupant, throwDir));
    }
    
    private System.Collections.IEnumerator ThrowOccupantSequence(GameObject occupant, Vector3 direction)
    {
        float duration = 0.4f;
        Vector3 startPos = occupant.transform.position;
        Vector3 endPos = startPos + direction * 5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            Vector3 arc = Vector3.Lerp(startPos, endPos, t);
            arc.y += Mathf.Sin(t * Mathf.PI) * 3f;
            
            occupant.transform.position = arc;
            occupant.transform.rotation = Quaternion.Euler(0, 0, elapsed * 360f);
            yield return null;
        }
        
        occupant.transform.rotation = Quaternion.identity;
        
        Rigidbody2D rb = occupant.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1f;
        }
    }
    
    private void CompleteStealing(Vehicle vehicle)
    {
        isStealingInProgress = false;
        beingStolen = null;
        
        if (vehicle is TrafficVehicle tv)
        {
            tv.SetBeingStolen(false);
        }
        
        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.ReportSuccessfulTheft(1);
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateWantedLevel(1);
        }
    }
    
    private bool IsMotorcycle(VehicleType type)
    {
        return type == VehicleType.Motorcycle || 
               type == VehicleType.SportBike || 
               type == VehicleType.Scooter;
    }
    
    private bool IsTwoDoorVehicle(VehicleType type)
    {
        return type == VehicleType.ClassicCoupe || 
               type == VehicleType.MuscleCar || 
               type == VehicleType.SportsCar || 
               type == VehicleType.SuperCar || 
               type == VehicleType.Convertible || 
               type == VehicleType.DriftCar || 
               type == VehicleType.HotRod || 
               type == VehicleType.RallyCar;
    }
    
    public void RemoveVehicleOccupants(Vehicle vehicle)
    {
        if (vehicleOccupants.ContainsKey(vehicle))
        {
            VehicleOccupants occupants = vehicleOccupants[vehicle];
            
            if (occupants.driver != null)
            {
                Destroy(occupants.driver);
            }
            
            if (occupants.passengers != null)
            {
                foreach (var passenger in occupants.passengers)
                {
                    if (passenger != null)
                        Destroy(passenger);
                }
            }
            
            vehicleOccupants.Remove(vehicle);
        }
    }
    
    public VehicleOccupants GetOccupants(Vehicle vehicle)
    {
        if (vehicleOccupants.ContainsKey(vehicle))
            return vehicleOccupants[vehicle];
        return null;
    }
    
    public void CompleteStealing()
    {
        isStealingInProgress = false;
        beingStolen = null;
    }
}

public class TrainStealingHandler : MonoBehaviour
{
    private Train train;
    private VehicleStealingSystem stealingSystem;
    private bool hasEnteredTrain = false;
    
    public void Initialize(Train t, VehicleStealingSystem system)
    {
        train = t;
        stealingSystem = system;
        
        StartCoroutine(StealingSequence());
    }
    
    private System.Collections.IEnumerator StealingSequence()
    {
        yield return new WaitForSeconds(0.5f);
        
        yield return new WaitUntil(() => train != null && train.IsStopped());
        
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.EnterVehicle(train);
            train.SetDriver(PlayerManager.Instance.GetComponent<PlayerController>());
            
            GameObject player = PlayerManager.Instance.gameObject;
            player.transform.parent = train.transform;
            player.transform.localPosition = Vector3.zero;
            player.SetActive(false);
            
            hasEnteredTrain = true;
        }
        
        yield return new WaitForSeconds(0.3f);
        
        if (hasEnteredTrain && stealingSystem != null)
        {
            stealingSystem.CompleteStealing();
        }
        
        Destroy(this);
    }
}

public class MetroStealingHandler : MonoBehaviour
{
    private MetroTram metro;
    private VehicleStealingSystem stealingSystem;
    private bool hasEnteredMetro = false;
    
    public void Initialize(MetroTram m, VehicleStealingSystem system)
    {
        metro = m;
        stealingSystem = system;
        
        StartCoroutine(StealingSequence());
    }
    
    private System.Collections.IEnumerator StealingSequence()
    {
        yield return new WaitForSeconds(0.4f);
        
        yield return new WaitUntil(() => metro != null && metro.IsStopped());
        
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.EnterVehicle(metro);
            metro.SetDriver(PlayerManager.Instance.GetComponent<PlayerController>());
            
            GameObject player = PlayerManager.Instance.gameObject;
            player.transform.parent = metro.transform;
            player.transform.localPosition = Vector3.zero;
            player.SetActive(false);
            
            hasEnteredMetro = true;
        }
        
        yield return new WaitForSeconds(0.25f);
        
        if (hasEnteredMetro && stealingSystem != null)
        {
            stealingSystem.CompleteStealing();
        }
        
        Destroy(this);
    }
}

public class VehicleOccupants
{
    public GameObject driver;
    public List<GameObject> passengers = new List<GameObject>();
}

public class NPCGunSystem : MonoBehaviour
{
    private GameObject target;
    private float fireRate = 1f;
    private float lastFireTime = 0f;
    private float range = 30f;
    private float damage = 10f;
    private bool isAttacking = false;
    
    public void Initialize()
    {
        target = FindTarget();
        
        if (GameInputManager.Instance != null)
        {
            InvokeRepeating("CheckAndAttack", 0.5f, fireRate);
        }
    }
    
    private GameObject FindTarget()
    {
        if (PlayerManager.Instance != null)
            return PlayerManager.Instance.gameObject;
        return null;
    }
    
    private void CheckAndAttack()
    {
        if (target == null || !isAttacking) return;
        
        float dist = Vector2.Distance(transform.position, target.transform.position);
        if (dist > range) return;
        
        FireAtTarget();
    }
    
    private void FireAtTarget()
    {
        if (Time.time - lastFireTime < fireRate) return;
        lastFireTime = Time.time;
        
        Vector2 direction = (target.transform.position - transform.position).normalized;
        
        if (BulletPool.Instance != null)
        {
            GameObject bullet = BulletPool.Instance.GetBullet();
            if (bullet != null)
            {
                Bullet b = bullet.GetComponent<Bullet>();
                b.Setup(transform.position, direction, damage, range, Gun.GunType.Pistol, 0);
            }
        }
        
        Debug.Log("NPC shot at player!");
    }
    
    public void StartAttacking()
    {
        isAttacking = true;
    }
    
    public void StopAttacking()
    {
        isAttacking = false;
    }
}
