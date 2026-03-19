using UnityEngine;

/// <summary>
/// Represents an individual motorcycle gang member
/// </summary>
public class GangMember : Enemy
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    
    public enum GangRole
    {
        Crew,
        Lieutenant,
        Leader
    }

    public GangRole Role { get; set; } = GangRole.Crew;
    public string MemberName { get; set; }
    public int ToughLevel { get; set; } = 1; // 1-5, affects combat ability
    
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float chaseSpeed = 5f;
    private Vector2 currentDirection = Vector2.right;
    private Vector3 targetPosition;
    private bool isInCombat = false;
    private Transform playerTransform;

    private enum GangMemberState
    {
        Patrolling,
        Pursuing,
        Fighting,
        Fleeing
    }

    private GangMemberState currentState = GangMemberState.Patrolling;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        SetAppearance();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        SetNewPatrolTarget();
    }

    private void FixedUpdate()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case GangMemberState.Patrolling:
                Patrol();
                break;
            case GangMemberState.Pursuing:
                Chase();
                break;
            case GangMemberState.Fighting:
                Fight();
                break;
        }
    }

    private void SetAppearance()
    {
        // Gang colors - red and black leather jacket look
        Color memberColor = Role switch
        {
            GangRole.Leader => new Color(1f, 0f, 0f, 1f),      // Bright red
            GangRole.Lieutenant => new Color(0.8f, 0f, 0f, 1f), // Dark red
            GangRole.Crew => new Color(0.6f, 0f, 0f, 1f),       // Darker red
            _ => new Color(0.6f, 0f, 0f, 1f)
        };

        if (spriteRenderer != null)
            spriteRenderer.color = memberColor;
    }

    private void Patrol()
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        rb.velocity = (Vector2)directionToTarget * walkSpeed;
        currentDirection = (Vector2)directionToTarget;

        if (spriteRenderer != null && currentDirection.x != 0)
            spriteRenderer.flipX = currentDirection.x < 0;

        if (Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            SetNewPatrolTarget();
        }
    }

    private void Chase()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        rb.velocity = (Vector2)directionToPlayer * chaseSpeed;
        currentDirection = (Vector2)directionToPlayer;

        if (spriteRenderer != null && currentDirection.x != 0)
            spriteRenderer.flipX = currentDirection.x < 0;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer < 2f)
        {
            currentState = GangMemberState.Fighting;
            isInCombat = true;
        }
    }

    private void Fight()
    {
        rb.velocity = Vector2.zero;
        // Combat would be handled by combat system
    }

    private void SetNewPatrolTarget()
    {
        float x = Random.Range(100f, 900f);
        float y = Random.Range(100f, 900f);
        targetPosition = new Vector3(x, y, 0f);
    }

    public void StartPursue()
    {
        currentState = GangMemberState.Pursuing;
    }

    public void StopPursue()
    {
        currentState = GangMemberState.Patrolling;
        isInCombat = false;
    }

    public bool IsInCombat() => isInCombat;
    public GangMemberState GetCurrentState() => currentState;
}
