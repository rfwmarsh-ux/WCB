using UnityEngine;
using System.Collections.Generic;

public class DestroyedVehicleManager : MonoBehaviour
{
    public static DestroyedVehicleManager Instance { get; private set; }
    
    private List<DestroyedVehicle> destroyedVehicles = new List<DestroyedVehicle>();
    private int maxStoredDestroyedVehicles = 10;
    private float cleanupInterval = 180f;
    private float cleanupTimer = 0f;
    
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
        cleanupTimer += Time.deltaTime;
        if (cleanupTimer >= cleanupInterval)
        {
            cleanupTimer = 0f;
            CleanupExcessDestroyedVehicles();
        }
    }
    
    public void RegisterDestroyedVehicle(DestroyedVehicle vehicle)
    {
        destroyedVehicles.Add(vehicle);
    }
    
    public void UnregisterDestroyedVehicle(DestroyedVehicle vehicle)
    {
        destroyedVehicles.Remove(vehicle);
    }
    
    public DestroyedVehicle GetNearestDestroyedVehicle(Vector2 position)
    {
        DestroyedVehicle nearest = null;
        float minDist = float.MaxValue;
        
        foreach (var vehicle in destroyedVehicles)
        {
            if (vehicle == null) continue;
            float dist = Vector2.Distance(position, vehicle.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = vehicle;
            }
        }
        
        return nearest;
    }
    
    public List<DestroyedVehicle> GetDestroyedVehiclesNear(Vector2 position, float radius)
    {
        List<DestroyedVehicle> result = new List<DestroyedVehicle>();
        foreach (var vehicle in destroyedVehicles)
        {
            if (vehicle == null) continue;
            if (Vector2.Distance(position, vehicle.transform.position) <= radius)
            {
                result.Add(vehicle);
            }
        }
        return result;
    }
    
    private void CleanupExcessDestroyedVehicles()
    {
        if (destroyedVehicles.Count <= maxStoredDestroyedVehicles) return;
        
        int toRemove = destroyedVehicles.Count - maxStoredDestroyedVehicles;
        int removed = 0;
        
        for (int i = destroyedVehicles.Count - 1; i >= 0 && removed < toRemove; i--)
        {
            if (destroyedVehicles[i] != null && !destroyedVehicles[i].IsBeingTowed())
            {
                Destroy(destroyedVehicles[i].gameObject);
                destroyedVehicles.RemoveAt(i);
                removed++;
            }
        }
        
        Debug.Log($"Cleaned up {removed} excess destroyed vehicles");
    }
    
    public int GetDestroyedVehicleCount() => destroyedVehicles.Count;
    
    public void ClearAllDestroyedVehicles()
    {
        foreach (var vehicle in destroyedVehicles)
        {
            if (vehicle != null)
            {
                Destroy(vehicle.gameObject);
            }
        }
        destroyedVehicles.Clear();
    }
}

public class DestroyedVehicle : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private GameObject smokeEffect;
    private GameObject explosionEffect;
    private float rollDistance = 0f;
    private float rollSpeed = 0f;
    private float rotationSpeed = 0f;
    private Vector2 rollDirection;
    private bool isRolling = true;
    private bool isBeingTowed = false;
    private float lifetime = 0f;
    private float maxLifetime = 300f;
    
    private Color blackenedColor = new Color(0.15f, 0.15f, 0.15f, 1f);
    private Vector2 originalSize;
    
    public void Initialize(Vector2 position, Vector2 size, Quaternion rotation, Vector2 velocity)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = new Vector3(size.x, size.y, 1f);
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
            spriteRenderer.sortingOrder = 4;
        }
        
        originalSize = size;
        
        rollDirection = velocity.normalized;
        rollSpeed = velocity.magnitude * 0.3f;
        rotationSpeed = (Random.value > 0.5f ? 1f : -1f) * Random.Range(30f, 90f);
        
        CreateExplosionEffect();
        
        StartCoroutine(RollAndStop());
    }
    
    private System.Collections.IEnumerator RollAndStop()
    {
        float maxRollDistance = Random.Range(5f, 15f);
        float elapsed = 0f;
        float rollDuration = Random.Range(0.5f, 1.5f);
        
        while (elapsed < rollDuration && rollDistance < maxRollDistance)
        {
            elapsed += Time.deltaTime;
            
            float progress = elapsed / rollDuration;
            float currentRollSpeed = rollSpeed * (1f - progress);
            float currentRotationSpeed = rotationSpeed * (1f - progress);
            
            Vector2 movement = rollDirection * currentRollSpeed * Time.deltaTime;
            transform.position += (Vector3)movement;
            transform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);
            
            rollDistance += movement.magnitude;
            
            yield return null;
        }
        
        isRolling = false;
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
        
        CreateSmokeEffect();
    }
    
    private void CreateExplosionEffect()
    {
        explosionEffect = new GameObject("ExplosionEffect");
        explosionEffect.transform.position = transform.position;
        explosionEffect.transform.parent = transform;
        
        SpriteRenderer sr = explosionEffect.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(1f, 0.5f, 0f, 0.8f);
        sr.sortingOrder = 15;
        explosionEffect.transform.localScale = new Vector3(8f, 8f, 1f);
        
        ParticleSystem particles = explosionEffect.AddComponent<ParticleSystem>();
        if (particles != null)
        {
            var main = particles.main;
            main.startColor = new Color(1f, 0.6f, 0.2f, 1f);
            main.startSize = 0.5f;
            main.startSpeed = 5f;
            main.startLifetime = 0.5f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            
            var emission = particles.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20) });
        }
        
        Destroy(explosionEffect, 0.8f);
    }
    
    private void CreateSmokeEffect()
    {
        spriteRenderer.color = blackenedColor;
        
        smokeEffect = new GameObject("SmokeEffect");
        smokeEffect.transform.position = transform.position + Vector3.up * 2f;
        smokeEffect.transform.parent = transform;
        
        ParticleSystem particles = smokeEffect.AddComponent<ParticleSystem>();
        if (particles != null)
        {
            var main = particles.main;
            main.startColor = new Color(0.3f, 0.3f, 0.3f, 0.6f);
            main.startSize = 1f;
            main.startSpeed = 1f;
            main.startLifetime = 3f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 1f;
            
            var emission = particles.emission;
            emission.rateOverTime = 5f;
        }
    }
    
    private void Update()
    {
        lifetime += Time.deltaTime;
        
        if (smokeEffect != null && !isRolling)
        {
            float healthPercent = 1f - (lifetime / maxLifetime);
            
            if (healthPercent < 0.5f)
            {
                ParticleSystem ps = smokeEffect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var emission = ps.emission;
                    emission.rateOverTime = Mathf.Max(0, emission.rateOverTime.constant - 0.5f);
                }
            }
            
            if (healthPercent < 0.2f || lifetime > maxLifetime * 0.7f)
            {
                ParticleSystem ps = smokeEffect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var emission = ps.emission;
                    emission.rateOverTime = 0;
                }
            }
            
            if (healthPercent < 0.1f)
            {
                Destroy(smokeEffect);
                smokeEffect = null;
            }
        }
    }
    
    public bool IsBeingTowed()
    {
        return isBeingTowed;
    }
    
    public void SetBeingTowed(bool value)
    {
        isBeingTowed = value;
    }
    
    public float GetScrapValue()
    {
        return Random.Range(50f, 200f);
    }
    
    public Vector2 GetSize()
    {
        return originalSize;
    }
    
    private void OnDestroy()
    {
        if (DestroyedVehicleManager.Instance != null)
        {
            DestroyedVehicleManager.Instance.UnregisterDestroyedVehicle(this);
        }
        
        if (smokeEffect != null)
        {
            Destroy(smokeEffect);
        }
        if (explosionEffect != null)
        {
            Destroy(explosionEffect);
        }
    }
}
