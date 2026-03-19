using UnityEngine;
using System.Collections.Generic;

public enum PedestrianAwareness
{
    TrafficAware,
    Distracted,
    NotTrafficAware
}

public enum PedestrianBehavior
{
    Peaceful,
    Angry,
    Violent
}

public class PedestrianTrafficBehavior : MonoBehaviour
{
    public PedestrianAwareness awareness = PedestrianAwareness.TrafficAware;
    public PedestrianBehavior behavior = PedestrianBehavior.Peaceful;
    
    private float accidentProbability;
    private float aggressionLevel = 0f;
    private float lastAccidentTime = -30f;
    private bool isCrossingStreet = false;
    private Vector2 crossDirection;
    private float crossTimer = 0f;
    
    private Pedestrian pedestrian;
    private float wanderRadius = 50f;
    private Vector2 targetPosition;
    private float targetTimer = 0f;

    private void Start()
    {
        pedestrian = GetComponent<Pedestrian>();
        InitializeBehavior();
    }

    private void InitializeBehavior()
    {
        float roll = Random.value;
        
        if (roll < 0.15f)
        {
            awareness = PedestrianAwareness.NotTrafficAware;
            accidentProbability = 0.6f;
        }
        else if (roll < 0.35f)
        {
            awareness = PedestrianAwareness.Distracted;
            accidentProbability = 0.25f;
        }
        else
        {
            awareness = PedestrianAwareness.TrafficAware;
            accidentProbability = 0.05f;
        }
        
        roll = Random.value;
        if (roll < 0.02f)
        {
            behavior = PedestrianBehavior.Violent;
            aggressionLevel = Random.Range(0.7f, 1f);
        }
        else if (roll < 0.1f)
        {
            behavior = PedestrianBehavior.Angry;
            aggressionLevel = Random.Range(0.3f, 0.6f);
        }
    }

    private void Update()
    {
        UpdateMovement();
        
        if (awareness != PedestrianAwareness.TrafficAware)
        {
            UpdateStreetCrossing();
        }
        
        if (behavior == PedestrianBehavior.Violent)
        {
            UpdateViolentBehavior();
        }
    }

    private void UpdateMovement()
    {
        targetTimer -= Time.deltaTime;
        
        if (targetTimer <= 0)
        {
            if (awareness == PedestrianAwareness.NotTrafficAware && Random.value < 0.3f)
            {
                Vector2 roadPos = GetRandomRoadPosition();
                targetPosition = roadPos;
            }
            else
            {
                targetPosition = GetRandomWanderPosition();
            }
            targetTimer = Random.Range(3f, 8f);
        }
    }

    private Vector2 GetRandomWanderPosition()
    {
        Vector2 currentPos = transform.position;
        return currentPos + new Vector2(
            Random.Range(-wanderRadius, wanderRadius),
            Random.Range(-wanderRadius, wanderRadius)
        );
    }

    private Vector2 GetRandomRoadPosition()
    {
        if (MapSystem.Instance != null)
        {
            var roads = MapSystem.Instance.GetRoadSegments();
            if (roads.Count > 0)
            {
                Rect road = roads[Random.Range(0, roads.Count)];
                return new Vector2(
                    Random.Range(road.x, road.x + road.width),
                    Random.Range(road.y, road.y + road.height)
                );
            }
        }
        return GetRandomWanderPosition();
    }

    private void UpdateStreetCrossing()
    {
        if (isCrossingStreet)
        {
            crossTimer -= Time.deltaTime;
            
            if (crossTimer <= 0)
            {
                isCrossingStreet = false;
            }
            
            float accidentRoll = Random.value;
            if (accidentRoll < accidentProbability * Time.deltaTime * 0.5f)
            {
                OnPedestrianAccident();
            }
        }
        else
        {
            if (Random.value < 0.01f && CanCrossStreet())
            {
                StartCrossingStreet();
            }
        }
    }

    private bool CanCrossStreet()
    {
        Vector2 pos = transform.position;
        
        if (MapSystem.Instance != null)
        {
            return MapSystem.Instance.IsOnRoad(pos);
        }
        
        return Random.value < 0.3f;
    }

    private void StartCrossingStreet()
    {
        isCrossingStreet = true;
        crossTimer = Random.Range(3f, 10f);
        crossDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void OnPedestrianAccident()
    {
        lastAccidentTime = Time.time;
        
        if (behavior == PedestrianBehavior.Peaceful)
        {
            float newRoll = Random.value;
            if (newRoll < 0.3f)
            {
                behavior = PedestrianBehavior.Angry;
                aggressionLevel = Random.Range(0.2f, 0.5f);
            }
        }
        
        Debug.Log($"{name} was in a traffic accident!");
    }

    private void UpdateViolentBehavior()
    {
        if (aggressionLevel <= 0) return;
        
        float violenceRoll = Random.value;
        float violenceThreshold = aggressionLevel * 0.0005f;
        
        if (violenceRoll < violenceThreshold)
        {
            AttemptViolentAct();
        }
    }

    private void AttemptViolentAct()
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 15f);
        
        foreach (var col in nearby)
        {
            if (col.CompareTag("Pedestrian") && col.gameObject != gameObject)
            {
                Pedestrian otherPed = col.GetComponent<Pedestrian>();
                if (otherPed != null && !otherPed.IsDead())
                {
                    AttackPedestrian(otherPed);
                    return;
                }
            }
        }
    }

    private void AttackPedestrian(Pedestrian target)
    {
        Debug.Log($"{name} attacked {target.DisplayName}!");
        
        int damage = Random.Range(20, 50);
        target.TakeDamage(damage);
        
        float deathRoll = Random.value;
        if (deathRoll < 0.3f)
        {
            target.Kill();
            Debug.Log($"{target.DisplayName} was killed by {name}!");
        }
        
        aggressionLevel *= 0.5f;
    }

    public float GetAccidentRisk()
    {
        return awareness switch
        {
            PedestrianAwareness.NotTrafficAware => 0.8f,
            PedestrianAwareness.Distracted => 0.4f,
            PedestrianAwareness.TrafficAware => 0.1f,
            _ => 0.2f
        };
    }

    public bool IsDangerous()
    {
        return behavior == PedestrianBehavior.Violent || behavior == PedestrianBehavior.Angry;
    }
}

public class PedestrianWeaponSystem : MonoBehaviour
{
    public bool hasWeapon = false;
    public string weaponType = "";
    private float attackCooldown = 0f;
    private float attackRange = 3f;
    
    private PedestrianTrafficBehavior trafficBehavior;

    private void Start()
    {
        trafficBehavior = GetComponent<PedestrianTrafficBehavior>();
        InitializeWeapon();
    }

    private void InitializeWeapon()
    {
        float roll = Random.value;
        
        if (roll < 0.05f)
        {
            hasWeapon = true;
            weaponType = GetRandomWeapon();
        }
    }

    private string GetRandomWeapon()
    {
        string[] weapons = { "Knife", "Baseball Bat", "Bottle", "Brass Knuckles" };
        return weapons[Random.Range(0, weapons.Length)];
    }

    private void Update()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public void AttemptAttack()
    {
        if (!hasWeapon || attackCooldown > 0) return;
        
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, attackRange);
        
        foreach (var col in nearby)
        {
            if (col.CompareTag("Pedestrian") && col.gameObject != gameObject)
            {
                Pedestrian target = col.GetComponent<Pedestrian>();
                if (target != null && !target.IsDead())
                {
                    PerformAttack(target);
                    return;
                }
            }
        }
    }

    private void PerformAttack(Pedestrian target)
    {
        attackCooldown = Random.Range(2f, 5f);
        
        int damage = weaponType switch
        {
            "Knife" => Random.Range(30, 60),
            "Baseball Bat" => Random.Range(25, 45),
            "Bottle" => Random.Range(15, 30),
            "Brass Knuckles" => Random.Range(20, 35),
            _ => 20
        };
        
        target.TakeDamage(damage);
        
        float killChance = weaponType switch
        {
            "Knife" => 0.4f,
            "Baseball Bat" => 0.3f,
            "Bottle" => 0.2f,
            "Brass Knuckles" => 0.25f,
            _ => 0.2f
        };
        
        if (Random.value < killChance)
        {
            target.Kill();
        }
        
        Debug.Log($"{name} attacked {target.DisplayName} with {weaponType}!");
    }

    public void OnTrafficAccident()
    {
        if (trafficBehavior != null && trafficBehavior.behavior == PedestrianBehavior.Violent)
        {
            if (Random.value < 0.2f)
            {
                AttemptAttack();
            }
        }
    }
}
