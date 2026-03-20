using UnityEngine;
using System.Collections.Generic;

public class TrainCollisionSystem : MonoBehaviour
{
    public static TrainCollisionSystem Instance { get; private set; }
    
    private float trainCriticalSpeedPercent = 0.1f;
    private float metroCriticalSpeedPercent = 0.1f;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void HandleTrainCollision(Vehicle vehicle, Train train)
    {
        if (vehicle == null || train == null || !train.IsMoving())
        {
            return;
        }
        
        float trainSpeed = train.GetCurrentSpeed();
        float maxSpeed = train.GetMaxSpeed();
        if (maxSpeed <= 0f) return;
        float speedRatio = trainSpeed / maxSpeed;
        
        if (speedRatio < trainCriticalSpeedPercent)
        {
            return;
        }
        
        bool instantDestroy = IsLightVehicle(vehicle.VehicleType);
        if (instantDestroy)
        {
            vehicle.TakeDamage(vehicle.MaxHealth * 2f);
            CreateExplosionEffect(vehicle.transform.position);
            Debug.Log($"Vehicle {vehicle.DisplayName} destroyed by train collision!");
        }
        else
        {
            float bounceForce = trainSpeed * 3f;
            BounceVehicleOffTrack(vehicle, train.transform.position, bounceForce);
            
            float heavyVehicleMultiplier = IsHeavyVehicle(vehicle.VehicleType) ? 0.65f : 1f;
            float damage = vehicle.MaxHealth * speedRatio * 0.5f * heavyVehicleMultiplier;
            vehicle.TakeDamage(damage);
            Debug.Log($"Vehicle {vehicle.DisplayName} bounced by train. Damage: {damage}");
        }
    }
    
    public void HandleMetroCollision(Vehicle vehicle, MetroTram metro)
    {
        if (vehicle == null || metro == null || !metro.IsMoving())
        {
            return;
        }
        
        float metroSpeed = metro.GetCurrentSpeed();
        float maxSpeed = metro.GetMaxSpeed();
        if (maxSpeed <= 0f) return;
        float speedRatio = metroSpeed / maxSpeed;
        
        if (speedRatio < metroCriticalSpeedPercent)
        {
            return;
        }
        
        bool instantDestroy = IsLightVehicle(vehicle.VehicleType);
        
        if (instantDestroy)
        {
            vehicle.TakeDamage(vehicle.MaxHealth * 1.5f);
            CreateExplosionEffect(vehicle.transform.position);
            Debug.Log($"Vehicle {vehicle.DisplayName} destroyed by metro collision!");
        }
        else
        {
            float bounceForce = metroSpeed * 2.5f;
            BounceVehicleOffTrack(vehicle, metro.transform.position, bounceForce);
            
            float heavyVehicleMultiplier = IsHeavyVehicle(vehicle.VehicleType) ? 0.7f : 1f;
            float damage = vehicle.MaxHealth * speedRatio * 0.3f * heavyVehicleMultiplier;
            vehicle.TakeDamage(damage);
            Debug.Log($"Vehicle {vehicle.DisplayName} bounced by metro. Damage: {damage}");
        }
    }
    
    private void BounceVehicleOffTrack(Vehicle vehicle, Vector2 trainPosition, float force)
    {
        Vector2 vehiclePos = vehicle.transform.position;
        Vector2 bounceDirection = (vehiclePos - trainPosition).normalized;
        
        if (Mathf.Abs(bounceDirection.x) < 0.3f)
        {
            bounceDirection = new Vector2(bounceDirection.y > 0 ? 1f : -1f, 0);
        }
        else
        {
            bounceDirection = new Vector2(bounceDirection.x > 0 ? 1f : -1f, bounceDirection.y * 0.3f).normalized;
        }
        
        Rigidbody2D rb = vehicle.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = bounceDirection * force;
            rb.AddForce(bounceDirection * force * 10f, ForceMode2D.Impulse);
        }
        else
        {
            vehicle.transform.position += (Vector3)(bounceDirection * force * 0.1f);
        }
        
        Debug.Log($"Vehicle bounced off track with force {force}");
    }
    
    private bool IsLightVehicle(VehicleType type)
    {
        return type == VehicleType.Motorcycle ||
               type == VehicleType.SportBike ||
               type == VehicleType.Scooter ||
               type == VehicleType.CompactCar ||
               type == VehicleType.Hatchback ||
               type == VehicleType.EconomyCar;
    }
    
    private bool IsHeavyVehicle(VehicleType type)
    {
        return type == VehicleType.SlowTruck ||
               type == VehicleType.Bus ||
               type == VehicleType.Ambulance ||
               type == VehicleType.SUV ||
               type == VehicleType.PickupTruck ||
               type == VehicleType.OffroadVehicle ||
               type == VehicleType.ArmoredCar;
    }
    
    private void CreateExplosionEffect(Vector2 position)
    {
        GameObject explosion = new GameObject("TrainCollisionExplosion");
        explosion.transform.position = (Vector3)position;
        
        SpriteRenderer sr = explosion.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(1f, 0.5f, 0f, 0.8f);
        sr.sortingOrder = 20;
        explosion.transform.localScale = new Vector3(12f, 12f, 1f);
        
        ParticleSystem particles = explosion.AddComponent<ParticleSystem>();
        if (particles != null)
        {
            var main = particles.main;
            main.startColor = new Color(1f, 0.3f, 0f, 1f);
            main.startSize = 2f;
            main.startSpeed = 6f;
            main.startLifetime = 0.8f;
            
            var emission = particles.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20) });
        }
        
        Destroy(explosion, 1.5f);
    }
}
