using UnityEngine;

/// <summary>
/// Represents a pedestrian NPC in the city
/// </summary>
public class Pedestrian : NPC
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    
    public enum PedestrianType
    {
        Shopper,
        Student,
        Worker,
        Tourist,
        LocalResident
    }

    public PedestrianType Type { get; set; }
    public string DisplayName { get; set; }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        SetInCombat(true);
    }
    
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float pauseDuration = 2f;
    [SerializeField] private float directionChangeDistance = 5f;

    private Vector2 currentDirection = Vector2.right;
    private Vector3 targetPosition;
    private bool isWalking = true;
    private float pauseTimer = 0f;
    private float distanceTraveled = 0f;
    
    private float pavementOffset = 9f;
    private Vector2 homePosition;
    private float homeRadius = 50f;
    private float minDistanceFromHome = 15f;

    private Color[] pedestrianColors = new Color[]
    {
        new Color(1f, 0.3f, 0.3f, 1f),    // Red
        new Color(0.3f, 0.5f, 1f, 1f),    // Blue
        new Color(0.3f, 1f, 0.3f, 1f),    // Green
        new Color(1f, 1f, 0.3f, 1f),      // Yellow
        new Color(1f, 0.5f, 1f, 1f),      // Magenta
        new Color(0.5f, 1f, 1f, 1f),      // Cyan
    };

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        SetAppearance();
        
        homePosition = transform.position;
        targetPosition = GetPavementTarget();
    }
    
    private Vector3 GetPavementTarget()
    {
        float roadWidth = 16f;
        float grassStrip = 5f;
        float shoulder = 2f;
        float pavement = 4f;
        
        float offset = roadWidth / 2f + pavement + grassStrip + shoulder + 3f;
        
        bool onHorizontalRoad = Mathf.Abs(currentDirection.x) > Mathf.Abs(currentDirection.y);
        Vector3 roadDirection;
        
        if (onHorizontalRoad)
        {
            float side = Random.value > 0.5f ? 1f : -1f;
            roadDirection = new Vector3(0f, side * offset, 0f);
        }
        else
        {
            float side = Random.value > 0.5f ? 1f : -1f;
            roadDirection = new Vector3(side * offset, 0f, 0f);
        }
        
        Vector3 newTarget = homePosition + roadDirection + (Vector3)Random.insideUnitCircle * 30f;
        
        newTarget.x = Mathf.Clamp(newTarget.x, homePosition.x - homeRadius, homePosition.x + homeRadius);
        newTarget.y = Mathf.Clamp(newTarget.y, homePosition.y - homeRadius, homePosition.y + homeRadius);
        
        return newTarget;
    }
    
    private Vector3 GetNewTargetNearHome()
    {
        Vector3 newTarget;
        int attempts = 0;
        
        do
        {
            newTarget = homePosition + (Vector3)Random.insideUnitCircle * homeRadius;
            attempts++;
        } while (Vector3.Distance(newTarget, transform.position) < minDistanceFromHome && attempts < 10);
        
        return newTarget;
    }

    private void Update()
    {
        if (!isWalking)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                ResumeWalking();
            }
            return;
        }

        WalkTowardTarget();
        CheckPauseConditions();
    }

    private void SetAppearance()
    {
        Color color = pedestrianColors[(int)Type % pedestrianColors.Length];
        if (spriteRenderer != null)
            spriteRenderer.color = color;
    }

    private void WalkTowardTarget()
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        rb.velocity = (Vector2)directionToTarget * walkSpeed;
        currentDirection = (Vector2)directionToTarget;

        // Update sprite facing direction
        if (spriteRenderer != null && currentDirection.x != 0)
        {
            spriteRenderer.flipX = currentDirection.x < 0;
        }

        distanceTraveled += Vector3.Distance(transform.position, targetPosition);
    }

    private void CheckPauseConditions()
    {
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            Pause();
            
            float distFromHome = Vector3.Distance(transform.position, homePosition);
            if (distFromHome > homeRadius * 0.8f)
            {
                targetPosition = GetNewTargetNearHome();
            }
            else
            {
                targetPosition = GetPavementTarget();
            }
        }
        
        float distFromHomeNow = Vector3.Distance(transform.position, homePosition);
        if (distFromHomeNow > homeRadius)
        {
            targetPosition = GetNewTargetNearHome();
        }
    }

    private void Pause()
    {
        isWalking = false;
        pauseTimer = pauseDuration + Random.Range(-0.5f, 1f);
        rb.velocity = Vector2.zero;
    }

    private void ResumeWalking()
    {
        isWalking = true;
    }

    public void SetTargetPosition(Vector3 target)
    {
        targetPosition = target;
    }

    public void SetWalkSpeed(float speed)
    {
        walkSpeed = speed;
    }

    public PedestrianType GetType() => Type;
    public Vector2 GetCurrentDirection() => currentDirection;
    public bool IsWalking() => isWalking;
}
