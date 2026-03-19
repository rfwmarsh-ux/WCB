using UnityEngine;
using System.Collections.Generic;

public class PoliceOfficer : MonoBehaviour
{
    public static List<PoliceOfficer> AllOfficers = new List<PoliceOfficer>();

    private enum OfficerState { Patrolling, Chasing, Attacking, Searching }
    private OfficerState currentState = OfficerState.Patrolling;

    private float moveSpeed = 25f;
    private float chaseSpeed = 35f;
    private float visionRange = 80f;
    private float attackRange = 50f;
    private float shootInterval = 1.5f;

    private Vector2 patrolTarget;
    private Vector2 lastKnownPlayerPosition;
    private float stateTimer = 0f;
    private float shootTimer = 0f;

    private int targetPlayerId = 1;
    private bool isEnabled = true;
    private bool isFromVan = false;
    private bool hasLineOfSight = false;

    private int maxShotsAtWanted1 = 2;
    private int maxShotsAtWanted2 = 4;
    private int maxShotsAtWanted3 = 6;
    private int maxShotsAtWanted4 = 8;
    private int maxShotsAtWanted5 = 12;

    public static PoliceOfficer CreateOfficer(Vector2 position, bool fromVan = false)
    {
        GameObject officerGO = new GameObject("PoliceOfficer");
        officerGO.transform.position = (Vector3)position;

        SpriteRenderer sr = officerGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.2f, 0.3f, 0.5f, 1f);
        sr.sortingOrder = 10;
        officerGO.transform.localScale = new Vector3(2f, 2.5f, 1f);

        Rigidbody2D rb = officerGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        BoxCollider2D collider = officerGO.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;

        PoliceOfficer officer = officerGO.AddComponent<PoliceOfficer>();
        officer.isFromVan = fromVan;
        officer.InitializePatrol();

        AllOfficers.Add(officer);
        return officer;
    }

    private void InitializePatrol()
    {
        PickNewPatrolPoint();
    }

    private void OnDestroy()
    {
        AllOfficers.Remove(this);
    }

    private void Update()
    {
        if (!isEnabled) return;

        int wantedLevel = WantedLevelManager.Instance.GetPlayerWantedLevel(targetPlayerId);
        
        Vector2 playerPos = GetPlayerPosition();
        CheckForWitnessedCrimes(playerPos);
        CheckForNearbyDeadBodies(playerPos);

        if (wantedLevel == 0)
        {
            ReturnToStation();
            return;
        }

        CheckLineOfSight();
        UpdateState();
        UpdateShooting(wantedLevel);
    }

    private Vector2 GetPlayerPosition()
    {
        if (targetPlayerId == 1)
        {
            PlayerManager pm = FindObjectOfType<PlayerManager>();
            return pm != null ? pm.transform.position : Vector2.zero;
        }
        else
        {
            Player2Manager p2m = Player2Manager.Instance;
            return p2m != null ? p2m.transform.position : Vector2.zero;
        }
    }

    private void CheckLineOfSight()
    {
        Transform targetTransform = targetPlayerId == 1 ?
            FindObjectOfType<PlayerManager>()?.transform :
            Player2Manager.Instance?.transform;

        if (targetTransform == null)
        {
            hasLineOfSight = false;
            return;
        }

        Vector2 direction = (Vector2)targetTransform.position - (Vector2)transform.position;
        float distance = direction.magnitude;

        if (distance > visionRange)
        {
            hasLineOfSight = false;
            return;
        }

        hasLineOfSight = !IsObstructed((Vector2)transform.position, (Vector2)targetTransform.position);

        if (hasLineOfSight)
        {
            lastKnownPlayerPosition = targetTransform.position;
        }
        
        CheckVehicleRelatedCrimes(targetTransform.position, distance);
    }
    
    private void CheckVehicleRelatedCrimes(Vector2 playerPos, float distance)
    {
        if (distance > visionRange * 0.5f) return;
        
        if (ParkedVehicleManager.Instance != null)
        {
            ParkedVehicle nearbyParked = ParkedVehicleManager.Instance.GetNearestParkedVehicle(playerPos, 10f);
            if (nearbyParked != null)
            {
                bool isTryingToSteal = CheckIfPlayerIsNearVehicle(playerPos, nearbyParked.transform.position, 5f);
                if (isTryingToSteal && WantedLevelManager.Instance != null)
                {
                    WantedLevelManager.Instance.ReportAttemptedTheft(targetPlayerId);
                }
            }
        }
        
        if (TowTruckManager.Instance != null && TowTruckManager.Instance.IsTowingVehicle())
        {
            if (WantedLevelManager.Instance != null)
            {
                WantedLevelManager.Instance.ReportSuccessfulTowing(targetPlayerId);
            }
        }
        
        GameObject playerTowTruck = TowTruckManager.Instance?.GetPlayerTowTruck();
        if (playerTowTruck != null && playerTowTruck.activeSelf)
        {
            if (CheckIfPlayerIsNearVehicle(playerPos, playerTowTruck.transform.position, 8f))
            {
                if (WantedLevelManager.Instance != null)
                {
                    WantedLevelManager.Instance.ReportAttemptedTowing(targetPlayerId);
                }
            }
        }
        
        if (DestroyedVehicleManager.Instance != null)
        {
            var destroyedNear = DestroyedVehicleManager.Instance.GetDestroyedVehiclesNear(playerPos, 10f);
            if (destroyedNear.Count > 0)
            {
                foreach (var dv in destroyedNear)
                {
                    if (dv != null && !dv.IsBeingTowed())
                    {
                        if (CheckIfPlayerIsNearVehicle(playerPos, dv.transform.position, 5f))
                        {
                            if (WantedLevelManager.Instance != null)
                            {
                                WantedLevelManager.Instance.ReportAttemptedTowing(targetPlayerId);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    private float crimeWitnessRange = 60f;
    private float lastWitnessTime = 0f;
    private float witnessCooldown = 2f;

    private void CheckForWitnessedCrimes(Vector2 playerPos)
    {
        if (Time.time - lastWitnessTime < witnessCooldown) return;
        if (WantedLevelManager.Instance != null && WantedLevelManager.Instance.GetPlayerWantedLevel(targetPlayerId) > 0) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(playerPos, crimeWitnessRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Pedestrian") || hit.CompareTag("NPC"))
            {
                NPC npc = hit.GetComponent<NPC>();
                if (npc != null && !npc.IsAlive())
                {
                    WantedLevelManager.Instance.ReportPoliceWitnessedCrime(targetPlayerId, WantedLevelManager.CrimeType.Murder);
                    lastWitnessTime = Time.time;
                    return;
                }

                Pedestrian ped = hit.GetComponent<Pedestrian>();
                if (ped != null && ped.IsInCombat())
                {
                    WantedLevelManager.Instance.ReportPoliceWitnessedCrime(targetPlayerId, WantedLevelManager.CrimeType.Assault);
                    lastWitnessTime = Time.time;
                    return;
                }
            }

            if (hit.CompareTag("PoliceOfficer") || hit.CompareTag("Police"))
            {
                PoliceOfficer otherOfficer = hit.GetComponent<PoliceOfficer>();
                if (otherOfficer != null && otherOfficer != this)
                {
                    PoliceVehicle policeVehicle = hit.GetComponent<PoliceVehicle>();
                    if (policeVehicle != null)
                    {
                        Vehicle playerVehicle = PlayerManager.Instance?.currentVehicle;
                        if (playerVehicle != null)
                        {
                            float dist = Vector2.Distance(playerPos, hit.transform.position);
                            if (dist < crimeWitnessRange * 0.7f)
                            {
                                WantedLevelManager.Instance.ReportPoliceWitnessedCrime(targetPlayerId, WantedLevelManager.CrimeType.PoliceAssault);
                                lastWitnessTime = Time.time;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    private void CheckForNearbyDeadBodies(Vector2 playerPos)
    {
        if (Time.time - lastWitnessTime < witnessCooldown) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(playerPos, crimeWitnessRange);
        foreach (Collider2D hit in hits)
        {
            DeadBody body = hit.GetComponent<DeadBody>();
            if (body != null)
            {
                float dist = Vector2.Distance(playerPos, hit.transform.position);
                if (dist < crimeWitnessRange * 0.8f)
                {
                    WantedLevelManager.Instance.ReportPoliceWitnessedCrime(targetPlayerId, WantedLevelManager.CrimeType.Murder);
                    lastWitnessTime = Time.time;
                    return;
                }
            }
        }
    }
    
    private bool CheckIfPlayerIsNearVehicle(Vector2 playerPos, Vector2 vehiclePos, float threshold)
    {
        return Vector2.Distance(playerPos, vehiclePos) <= threshold;
    }

    private bool IsObstructed(Vector2 start, Vector2 end)
    {
        Vector2 direction = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        int layerMask = LayerMask.GetMask("Buildings", "Obstacles");

        RaycastHit2D hit = Physics2D.Raycast(start, direction, distance, layerMask);
        return hit.collider != null;
    }

    private void UpdateState()
    {
        int wantedLevel = WantedLevelManager.Instance.GetPlayerWantedLevel(targetPlayerId);

        Transform targetTransform = targetPlayerId == 1 ?
            FindObjectOfType<PlayerManager>()?.transform :
            Player2Manager.Instance?.transform;

        if (targetTransform == null)
        {
            currentState = OfficerState.Searching;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, targetTransform.position);

        if (hasLineOfSight && distanceToPlayer <= attackRange)
        {
            currentState = OfficerState.Attacking;
            LookAtTarget(targetTransform.position);
        }
        else if (hasLineOfSight)
        {
            currentState = OfficerState.Chasing;
            LookAtTarget(targetTransform.position);
        }
        else if (Vector2.Distance(transform.position, lastKnownPlayerPosition) > 5f)
        {
            currentState = OfficerState.Searching;
        }
        else
        {
            currentState = OfficerState.Patrolling;
        }

        ExecuteStateBehavior();
    }

    private void ExecuteStateBehavior()
    {
        switch (currentState)
        {
            case OfficerState.Patrolling:
                UpdatePatrolling();
                break;
            case OfficerState.Chasing:
                UpdateChasing();
                break;
            case OfficerState.Attacking:
                break;
            case OfficerState.Searching:
                UpdateSearching();
                break;
        }
    }

    private void UpdatePatrolling()
    {
        if (Vector2.Distance(transform.position, patrolTarget) < 5f || stateTimer > 15f)
        {
            PickNewPatrolPoint();
            stateTimer = 0f;
        }

        MoveTo(patrolTarget, moveSpeed);
        stateTimer += Time.deltaTime;
    }

    private void UpdateChasing()
    {
        MoveTo(lastKnownPlayerPosition, chaseSpeed);
    }

    private void UpdateSearching()
    {
        if (Vector2.Distance(transform.position, lastKnownPlayerPosition) < 5f)
        {
            PickNewPatrolPoint();
            currentState = OfficerState.Patrolling;
        }
        else
        {
            MoveTo(lastKnownPlayerPosition, moveSpeed * 0.5f);
        }
    }

    private void PickNewPatrolPoint()
    {
        PoliceStationManager stationManager = PoliceStationManager.Instance;
        if (stationManager == null)
        {
            patrolTarget = new Vector2(Random.Range(200f, 800f), Random.Range(200f, 800f));
            return;
        }

        PoliceStation nearest = stationManager.GetClosestStation(transform.position);
        if (nearest == null)
        {
            patrolTarget = new Vector2(Random.Range(200f, 800f), Random.Range(200f, 800f));
            return;
        }

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float radius = Random.Range(30f, 100f);
        patrolTarget = nearest.Position + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
    }

    private void MoveTo(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void LookAtTarget(Vector2 targetPos)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void UpdateShooting(int wantedLevel)
    {
        if (currentState != OfficerState.Attacking) return;

        shootTimer += Time.deltaTime;
        if (shootTimer < shootInterval) return;
        shootTimer = 0f;

        int maxShots = GetMaxShotsForWantedLevel(wantedLevel);
        int shotsToFire = Random.Range(1, maxShots + 1);

        for (int i = 0; i < shotsToFire; i++)
        {
            FirePistol();
        }
    }

    private int GetMaxShotsForWantedLevel(int wantedLevel)
    {
        return wantedLevel switch
        {
            1 => maxShotsAtWanted1,
            2 => maxShotsAtWanted2,
            3 => maxShotsAtWanted3,
            4 => maxShotsAtWanted4,
            5 => maxShotsAtWanted5,
            _ => 1
        };
    }

    private void FirePistol()
    {
        Transform targetTransform = targetPlayerId == 1 ?
            FindObjectOfType<PlayerManager>()?.transform :
            Player2Manager.Instance?.transform;

        if (targetTransform == null) return;

        Vector2 shootDirection = ((Vector2)targetTransform.position - (Vector2)transform.position).normalized;
        shootDirection += new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));

        if (BulletPool.Instance != null)
        {
            GameObject bullet = BulletPool.Instance.GetBullet();
            Bullet b = bullet.GetComponent<Bullet>();
            b.Setup(transform.position, shootDirection, 10f, 30f, Bullet.BulletType.Pistol, 0);
        }
    }

    private void ReturnToStation()
    {
        PoliceStationManager stationManager = PoliceStationManager.Instance;
        if (stationManager == null)
        {
            Destroy(gameObject);
            return;
        }

        PoliceStation nearest = stationManager.GetClosestStation(transform.position);
        if (nearest == null || Vector2.Distance(transform.position, nearest.Position) < 20f)
        {
            Destroy(gameObject);
        }
        else
        {
            MoveTo(nearest.Position, moveSpeed * 0.5f);
        }
    }

    public void SetTargetPlayer(int playerId)
    {
        targetPlayerId = playerId;
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
    }

    public void SetFromVan(bool fromVan)
    {
        isFromVan = fromVan;
    }

    public void SetWantedLevel(int level)
    {
        // Compatibility method for PoliceSystem
    }

    public void UpdateBehavior(bool isPlayerWanted)
    {
        // Compatibility method for PoliceSystem - the real behavior is in Update()
    }

    public void TakeDamage(float damage, int attackerPlayerId = 1)
    {
        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.ReportCrime(attackerPlayerId, WantedLevelManager.CrimeType.PoliceAssault);
            Debug.Log($"Player {attackerPlayerId} assaulted a police officer! Wanted level increased.");
        }
    }
}
