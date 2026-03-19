using UnityEngine;
using System.Collections.Generic;

public class AmbulanceManager : MonoBehaviour
{
    public static AmbulanceManager Instance { get; private set; }

    private List<Ambulance> ambulances = new List<Ambulance>();
    private List<Ambulance> patrolAmbulances = new List<Ambulance>();
    private List<Hospital> hospitals = new List<Hospital>();
    private List<DeadPedestrianInfo> deadPedestrians = new List<DeadPedestrianInfo>();
    private List<PatrolRoute> patrolRoutes = new List<PatrolRoute>();
    
    private int maxDispatchedAmbulances = 3;
    private int patrolAmbulanceCount = 2;
    private float patrolDetectionRange = 150f;
    private float patrolFarFromHospitalThreshold = 200f;

    private float searchTimer = 0f;
    private float searchInterval = 0.5f;
    private float bodyCleanupTime = 30f;

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
        InitializeHospitals();
        InitializePatrolRoutes();
        StartPatrolAmbulances();
    }

    private void Update()
    {
        UpdatePatrolAmbulances();
        SearchForDeadPedestrians();
        TryAssignBodiesToPatrolAmbulances();
        TryDispatchFromHospital();
        CleanupOldBodies();
    }

    private void InitializeHospitals()
    {
        CreateHospital("Wolverhampton General Hospital", new Vector2(500, 550));
        CreateHospital("Bilston Medical Centre", new Vector2(280, 330));
        CreateHospital("Wednesfield Medical", new Vector2(710, 410));
        CreateHospital("Tettenhall Medical Centre", new Vector2(620, 710));

        Debug.Log($"Initialized {hospitals.Count} hospitals");
    }

    private void InitializePatrolRoutes()
    {
        patrolRoutes.Add(new PatrolRoute(new Vector2(400, 200), new Vector2(600, 200), 400));
        patrolRoutes.Add(new PatrolRoute(new Vector2(200, 400), new Vector2(200, 600), 400));
        patrolRoutes.Add(new PatrolRoute(new Vector2(600, 500), new Vector2(800, 500), 600));
        patrolRoutes.Add(new PatrolRoute(new Vector2(300, 700), new Vector2(500, 700), 500));
    }

    private void StartPatrolAmbulances()
    {
        Hospital mainHospital = hospitals.Count > 0 ? hospitals[0] : null;
        if (mainHospital == null) return;

        for (int i = 0; i < patrolAmbulanceCount; i++)
        {
            PatrolRoute route = patrolRoutes[i % patrolRoutes.Count];
            Ambulance ambulance = CreatePatrolAmbulance(mainHospital.Position, route, i);
            patrolAmbulances.Add(ambulance);
            ambulances.Add(ambulance);
        }
        
        Debug.Log($"Started {patrolAmbulanceCount} patrol ambulances");
    }

    private void CreateHospital(string name, Vector2 position)
    {
        GameObject hospitalGO = new GameObject($"Hospital_{name}");
        hospitalGO.transform.position = (Vector3)position;

        SpriteRenderer sr = hospitalGO.AddComponent<SpriteRenderer>();
        sr.sprite = GTASpriteGenerator.GetSprite("building_hospital");
        sr.sortingOrder = 3;
        hospitalGO.transform.localScale = new Vector3(15f, 12f, 1f);

        CircleCollider2D collider = hospitalGO.AddComponent<CircleCollider2D>();
        collider.radius = 10f;
        collider.isTrigger = true;

        Hospital hospital = hospitalGO.AddComponent<Hospital>();
        hospital.Initialize(name, position);

        hospitalGO.transform.parent = transform;
        hospitals.Add(hospital);
    }

    private void UpdatePatrolAmbulances()
    {
        foreach (var ambulance in patrolAmbulances)
        {
            if (ambulance != null && ambulance.IsOnPatrol())
            {
                ambulance.UpdatePatrol();
            }
        }
    }

    private void SearchForDeadPedestrians()
    {
        searchTimer += Time.deltaTime;
        if (searchTimer < searchInterval) return;
        searchTimer = 0f;

        foreach (var npc in FindObjectsOfType<NPC>())
        {
            if (!npc.IsAlive() && !IsDeadPedestrianTracked(npc))
            {
                deadPedestrians.Add(new DeadPedestrianInfo
                {
                    npcReference = npc,
                    position = npc.transform.position,
                    deathTime = Time.time,
                    pickedUp = false,
                    assigned = false
                });
            }
        }
    }

    private void TryAssignBodiesToPatrolAmbulances()
    {
        foreach (var deadInfo in deadPedestrians)
        {
            if (deadInfo.assigned || deadInfo.pickedUp) continue;

            Ambulance closestPatrol = null;
            float closestDist = float.MaxValue;
            float closestToHospital = float.MaxValue;

            foreach (var ambulance in patrolAmbulances)
            {
                if (ambulance == null || !ambulance.IsOnPatrol()) continue;

                float distToBody = Vector2.Distance(ambulance.transform.position, deadInfo.position);
                float distToHospital = ambulance.GetDistanceToNearestHospital();

                if (distToBody > patrolDetectionRange) continue;

                if (distToHospital > patrolFarFromHospitalThreshold) continue;

                if (distToBody < closestDist)
                {
                    closestDist = distToBody;
                    closestPatrol = ambulance;
                    closestToHospital = distToHospital;
                }
            }

            if (closestPatrol != null)
            {
                closestPatrol.SetRespondingToBody(deadInfo);
                deadInfo.assigned = true;
                Debug.Log($"Patrol ambulance responding to body at {deadInfo.position}");
            }
        }
    }

    private void TryDispatchFromHospital()
    {
        int activeFromPatrol = 0;
        foreach (var amb in patrolAmbulances)
        {
            if (amb != null && amb.IsResponding()) activeFromPatrol++;
        }

        int activeDispatched = 0;
        foreach (var amb in ambulances)
        {
            if (amb != null && amb.IsDispatched()) activeDispatched++;
        }

        int totalActive = activeFromPatrol + activeDispatched;
        if (totalActive >= maxDispatchedAmbulances) return;

        foreach (var deadInfo in deadPedestrians)
        {
            if (deadInfo.assigned || deadInfo.pickedUp) continue;
            if (activeFromPatrol >= patrolAmbulanceCount) break;

            bool alreadyAssigned = false;
            foreach (var ambulance in patrolAmbulances)
            {
                if (ambulance != null && ambulance.IsResponding() && ambulance.GetTargetPosition() == deadInfo.position)
                {
                    alreadyAssigned = true;
                    break;
                }
            }
            if (alreadyAssigned) continue;

            float bodyDistToHospital = GetDistanceToNearestHospital(deadInfo.position);
            
            if (bodyDistToHospital <= patrolFarFromHospitalThreshold)
            {
                foreach (var ambulance in patrolAmbulances)
                {
                    if (ambulance == null || !ambulance.IsOnPatrol()) continue;
                    
                    float ambDistToHospital = ambulance.GetDistanceToNearestHospital();
                    if (ambDistToHospital <= patrolFarFromHospitalThreshold)
                    {
                        ambulance.SetRespondingToBody(deadInfo);
                        deadInfo.assigned = true;
                        activeFromPatrol++;
                        Debug.Log($"Near-hospital patrol ambulance responding to body");
                        break;
                    }
                }
            }
            else
            {
                DispatchAmbulanceFromHospital(deadInfo);
                deadInfo.assigned = true;
                activeDispatched++;
            }
        }
    }

    private float GetDistanceToNearestHospital(Vector2 position)
    {
        if (hospitals.Count == 0) return float.MaxValue;
        
        float minDist = float.MaxValue;
        foreach (var hospital in hospitals)
        {
            float dist = Vector2.Distance(position, hospital.Position);
            if (dist < minDist) minDist = dist;
        }
        return minDist;
    }

    private void DispatchAmbulanceFromHospital(DeadPedestrianInfo deadInfo)
    {
        Hospital nearestHospital = GetNearestHospital(deadInfo.position);
        if (nearestHospital == null) return;

        Ambulance ambulance = CreateDispatchedAmbulance(nearestHospital.Position);
        ambulance.SetTarget(deadInfo);
        ambulances.Add(ambulance);

        Debug.Log($"Hospital dispatched ambulance to pickup dead pedestrian at {deadInfo.position}");
    }

    private void CleanupOldBodies()
    {
        for (int i = deadPedestrians.Count - 1; i >= 0; i--)
        {
            var info = deadPedestrians[i];
            
            if (info.npcReference == null)
            {
                deadPedestrians.RemoveAt(i);
                continue;
            }

            if (!info.pickedUp && Time.time - info.deathTime > bodyCleanupTime)
            {
                AmbulanceManager.Instance?.RemoveDeadPedestrianByNpc(info.npcReference);
                if (info.npcReference != null)
                {
                    Destroy(info.npcReference.gameObject);
                }
                deadPedestrians.RemoveAt(i);
            }
        }
    }

    private bool IsDeadPedestrianTracked(NPC npc)
    {
        foreach (var info in deadPedestrians)
        {
            if (info.npcReference == npc)
                return true;
        }
        return false;
    }

    private Hospital GetNearestHospital(Vector2 position)
    {
        if (hospitals.Count == 0) return null;

        Hospital nearest = hospitals[0];
        float minDist = Vector2.Distance(position, nearest.Position);

        foreach (var hospital in hospitals)
        {
            float dist = Vector2.Distance(position, hospital.Position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hospital;
            }
        }

        return nearest;
    }

    private Ambulance CreatePatrolAmbulance(Vector2 position, PatrolRoute route, int patrolIndex)
    {
        GameObject ambGO = new GameObject($"PatrolAmbulance_{patrolIndex}");
        ambGO.transform.position = (Vector3)position;

        SpriteRenderer sr = ambGO.AddComponent<SpriteRenderer>();
        sr.sprite = GTASpriteGenerator.GetSprite("building_hospital");
        sr.color = Color.white;
        sr.sortingOrder = 5;
        ambGO.transform.localScale = new Vector3(6f, 3f, 1f);

        Rigidbody2D rb = ambGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;

        BoxCollider2D collider = ambGO.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one * 6f;
        collider.isTrigger = true;

        Ambulance ambulance = ambGO.AddComponent<Ambulance>();
        ambulance.InitializePatrol(route);
        ambulance.SetNearestHospital(GetNearestHospital(position));
        ambGO.tag = "Vehicle";

        return ambulance;
    }

    private Ambulance CreateDispatchedAmbulance(Vector2 position)
    {
        GameObject ambGO = new GameObject("DispatchedAmbulance");
        ambGO.transform.position = (Vector3)position;

        SpriteRenderer sr = ambGO.AddComponent<SpriteRenderer>();
        sr.sprite = GTASpriteGenerator.GetSprite("building_hospital");
        sr.color = Color.white;
        sr.sortingOrder = 5;
        ambGO.transform.localScale = new Vector3(6f, 3f, 1f);

        Rigidbody2D rb = ambGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;

        BoxCollider2D collider = ambGO.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one * 6f;
        collider.isTrigger = true;

        Ambulance ambulance = ambGO.AddComponent<Ambulance>();
        ambulance.SetNearestHospital(GetNearestHospital(position));
        ambGO.tag = "Vehicle";

        return ambulance;
    }

    public void RemoveDeadPedestrianByNpc(NPC npc)
    {
        deadPedestrians.RemoveAll(info => info.npcReference == npc);
    }

    public void MarkPedestrianPickedUp(NPC npc)
    {
        foreach (var info in deadPedestrians)
        {
            if (info.npcReference == npc)
            {
                info.pickedUp = true;
                return;
            }
        }
    }

    public void ReportDeadBody(Vector2 position, NPC npc)
    {
        if (!IsDeadPedestrianTracked(npc))
        {
            deadPedestrians.Add(new DeadPedestrianInfo
            {
                npcReference = npc,
                position = position,
                deathTime = Time.time,
                pickedUp = false,
                assigned = false
            });
            
            Debug.Log($"Dead body reported at {position}");
        }
    }

    public void RemoveAmbulance(Ambulance ambulance)
    {
        ambulances.Remove(ambulance);
        patrolAmbulances.Remove(ambulance);
    }

    public void OnPatrolAmbulanceFinished(Ambulance ambulance)
    {
        patrolAmbulances.Remove(ambulance);
        ambulances.Remove(ambulance);
    }

    public void RespawnPatrolAmbulance(Ambulance finishedAmbulance, PatrolRoute route)
    {
        Hospital mainHospital = hospitals.Count > 0 ? hospitals[0] : null;
        if (mainHospital == null) return;

        RemoveAmbulance(finishedAmbulance);
        
        Ambulance newAmbulance = CreatePatrolAmbulance(mainHospital.Position, route, patrolAmbulances.Count);
        patrolAmbulances.Add(newAmbulance);
        ambulances.Add(newAmbulance);
        
        Debug.Log($"Respawned patrol ambulance on route {route}");
    }

    public List<Hospital> GetHospitals() => hospitals;
    public List<PatrolRoute> GetPatrolRoutes() => patrolRoutes;
}

public class DeadPedestrianInfo
{
    public NPC npcReference;
    public Vector2 position;
    public float deathTime;
    public bool pickedUp;
    public bool assigned;
}

public class Hospital : MonoBehaviour
{
    public string HospitalName { get; private set; }
    public Vector2 Position { get; private set; }

    public void Initialize(string name, Vector2 position)
    {
        HospitalName = name;
        Position = position;
    }
}

public class PatrolRoute
{
    public Vector2 PointA { get; }
    public Vector2 PointB { get; }
    public float PatrolRadius { get; }

    public PatrolRoute(Vector2 pointA, Vector2 pointB, float radius)
    {
        PointA = pointA;
        PointB = pointB;
        PatrolRadius = radius;
    }
}

public class Ambulance : MonoBehaviour
{
    public enum AmbulanceState { Patrol, OnCall, Responding, AtScene, Transporting, Returning }
    private AmbulanceState currentState = AmbulanceState.Patrol;

    private float patrolSpeed = 35f;
    private float respondSpeed = 55f;
    private Vector2 targetPosition;
    private Hospital targetHospital;
    private Hospital nearestHospital;
    private NPC targetNpc;
    private bool hasPatient = false;
    private bool isActive = true;
    private bool sirenRegistered = false;
    private bool isPatrolAmbulance = false;
    private PatrolRoute patrolRoute;
    private Vector2 patrolTarget;
    private bool movingToPointA = true;

    private float sirenTimer = 0f;
    private bool sirenOn = true;
    private SpriteRenderer spriteRenderer;

    public void InitializePatrol(PatrolRoute route)
    {
        patrolRoute = route;
        isPatrolAmbulance = true;
        patrolTarget = route.PointA;
        currentState = AmbulanceState.Patrol;
        
        if (SirenManager.Instance != null)
        {
            SirenManager.Instance.CreateSirenLights(gameObject, SirenType.Ambulance);
            SirenManager.Instance.SetSirenActive(gameObject, false);
        }
    }

    public void SetNearestHospital(Hospital hospital)
    {
        nearestHospital = hospital;
    }

    public void SetTarget(DeadPedestrianInfo deadInfo)
    {
        isPatrolAmbulance = false;
        targetNpc = deadInfo.npcReference;
        targetPosition = deadInfo.position;
        targetHospital = AmbulanceManager.Instance != null ? 
            AmbulanceManager.Instance.GetHospitals()[Random.Range(0, AmbulanceManager.Instance.GetHospitals().Count)] : null;
        currentState = AmbulanceState.Responding;

        RegisterSiren();
        SirenManager.Instance?.SetSirenActive(gameObject, true);
    }

    public void SetRespondingToBody(DeadPedestrianInfo deadInfo)
    {
        targetNpc = deadInfo.npcReference;
        targetPosition = deadInfo.position;
        targetHospital = nearestHospital;
        currentState = AmbulanceState.Responding;
        
        isPatrolAmbulance = false;
        patrolAmbulances.Remove(this);
        
        RegisterSiren();
        SirenManager.Instance?.SetSirenActive(gameObject, true);
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
            spriteRenderer.color = Color.white;
        }
    }

    private void RegisterSiren()
    {
        if (SirenManager.Instance != null && !sirenRegistered)
        {
            SirenManager.Instance.CreateSirenLights(gameObject, SirenType.Ambulance);
            SirenManager.Instance.RegisterSiren(gameObject, SirenType.Ambulance, true);
            sirenRegistered = true;
        }
    }

    private void Update()
    {
        if (!isActive) return;

        if (DayNightCycleManager.Instance != null)
        {
            float time = DayNightCycleManager.Instance.GetCurrentTime();
            if (time < 6f || time > 20f)
            {
                return;
            }
        }

        switch (currentState)
        {
            case AmbulanceState.Patrol:
                break;
            case AmbulanceState.Responding:
                MoveToTarget();
                if (targetNpc == null || targetNpc.IsAlive())
                {
                    ReturnToPatrol();
                    return;
                }
                if (Vector2.Distance(transform.position, targetPosition) < 4f)
                {
                    currentState = AmbulanceState.AtScene;
                    StartPickup();
                }
                break;
            case AmbulanceState.AtScene:
                break;
            case AmbulanceState.Transporting:
                MoveToHospital();
                break;
            case AmbulanceState.Returning:
                MoveToHospital();
                break;
        }

        UpdateSiren();
    }

    public void UpdatePatrol()
    {
        if (currentState != AmbulanceState.Patrol || patrolRoute == null) return;

        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * patrolSpeed * Time.deltaTime);
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Vector2.Distance(transform.position, patrolTarget) < 5f)
        {
            movingToPointA = !movingToPointA;
            patrolTarget = movingToPointA ? patrolRoute.PointA : patrolRoute.PointB;
        }
    }

    private void MoveToTarget()
    {
        if (targetNpc != null)
        {
            targetPosition = targetNpc.transform.position;
        }

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * respondSpeed * Time.deltaTime);
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void MoveToHospital()
    {
        if (targetHospital == null)
        {
            isActive = false;
            AmbulanceManager.Instance?.RemoveAmbulance(this);
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (targetHospital.Position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * respondSpeed * Time.deltaTime);
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Vector2.Distance(transform.position, targetHospital.Position) < 10f)
        {
            if (currentState == AmbulanceState.Transporting)
            {
                Debug.Log("Patient delivered to hospital! +$50 bonus");
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddMoney(50f);
                }
            }
            
            if (isPatrolAmbulance)
            {
                ReturnToPatrol();
            }
            else
            {
                isActive = false;
                AmbulanceManager.Instance?.RemoveAmbulance(this);
                Destroy(gameObject);
            }
        }
    }

    private void ReturnToPatrol()
    {
        currentState = AmbulanceState.Patrol;
        isPatrolAmbulance = true;
        targetNpc = null;
        hasPatient = false;
        
        SirenManager.Instance?.SetSirenActive(gameObject, false);
        
        if (patrolRoute != null)
        {
            patrolTarget = patrolRoute.PointA;
            movingToPointA = true;
            AmbulanceManager.Instance?.RespawnPatrolAmbulance(this, patrolRoute);
        }
        else
        {
            isActive = false;
            AmbulanceManager.Instance?.RemoveAmbulance(this);
            Destroy(gameObject);
        }
    }

    private void StartPickup()
    {
        if (targetNpc != null)
        {
            hasPatient = true;
            currentState = AmbulanceState.Transporting;
            AmbulanceManager.Instance?.MarkPedestrianPickedUp(targetNpc);
            
            Destroy(targetNpc.gameObject);
            Debug.Log("Ambulance picked up dead body!");
        }
        else
        {
            currentState = AmbulanceState.Returning;
        }
    }

    private void UpdateSiren()
    {
        sirenTimer += Time.deltaTime;
        if (sirenTimer > 0.5f)
        {
            sirenTimer = 0f;
            sirenOn = !sirenOn;
            
            if (spriteRenderer != null && SirenManager.Instance == null)
            {
                spriteRenderer.color = sirenOn ? Color.red : Color.blue;
            }
        }
    }

    private void OnDestroy()
    {
        if (SirenManager.Instance != null)
        {
            SirenManager.Instance.UnregisterSiren(gameObject);
        }
        AmbulanceManager.Instance?.RemoveAmbulance(this);
    }

    public bool IsActive() => isActive;
    public bool IsOnPatrol() => currentState == AmbulanceState.Patrol && isPatrolAmbulance;
    public bool IsResponding() => currentState == AmbulanceState.Responding || currentState == AmbulanceState.AtScene || currentState == AmbulanceState.Transporting;
    public bool IsDispatched() => !isPatrolAmbulance && (currentState == AmbulanceState.Responding || currentState == AmbulanceState.AtScene || currentState == AmbulanceState.Transporting);
    public Vector2 GetTargetPosition() => targetPosition;
    public float GetDistanceToNearestHospital()
    {
        if (nearestHospital == null) return float.MaxValue;
        return Vector2.Distance(transform.position, nearestHospital.Position);
    }
}
