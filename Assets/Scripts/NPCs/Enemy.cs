using UnityEngine;

public class Enemy : NPC
{
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected float moveSpeed = 3f;

    protected Transform playerTarget;
    protected float lastAttackTime;
    protected bool isChasing;

    protected override void Start()
    {
        base.Start();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
        }
    }

    protected virtual void Update()
    {
        if (!isAlive || playerTarget == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            StopChasing();
        }
    }

    protected void ChasePlayer()
    {
        isChasing = true;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);
        Vector2 direction = (playerTarget.position - transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        if (direction.x != 0)
        {
            GetComponent<SpriteRenderer>().flipX = direction.x < 0;
        }

        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    protected void StopChasing()
    {
        isChasing = false;
    }

    protected virtual void Attack()
    {
        lastAttackTime = Time.time;
        PlayerManager playerManager = playerTarget.GetComponent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.TakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} attacked player for {attackDamage} damage");
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (isAlive && !isChasing)
        {
            isChasing = true;
        }
    }

    protected override void Die()
    {
        base.Die();
        GameManager.Instance.AddMoney(50f);
    }
}