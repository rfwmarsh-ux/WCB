using UnityEngine;

public class MeleeCombat : MonoBehaviour
{
    public static MeleeCombat Instance { get; private set; }

    [SerializeField] private float punchDamage = 10f;
    [SerializeField] private float kickDamage = 15f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float punchCooldown = 0.5f;
    [SerializeField] private float kickCooldown = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint;

    private float lastPunchTime;
    private float lastKickTime;
    private bool isAttacking;
    private Animator animator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!PlayerManager.Instance.IsAlive()) return;

        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= lastPunchTime + punchCooldown)
        {
            PerformMeleeAttack("Punch");
        }

        if (Input.GetKeyDown(KeyCode.E) && Time.time >= lastKickTime + kickCooldown)
        {
            PerformMeleeAttack("Kick");
        }
    }

    public void PerformMeleeAttack(string attackType)
    {
        object melee = PlayerManager.Instance.GetCurrentMeleeWeapon();
        float damage;
        float range;
        float cooldown;
        string weaponName;

        if (melee is CricketBat)
        {
            CricketBat bat = (CricketBat)melee;
            damage = bat.Damage;
            range = bat.Range;
            cooldown = bat.AttackCooldown;
            weaponName = "Cricket Bat";
        }
        else
        {
            if (attackType == "Punch")
            {
                damage = punchDamage;
                range = attackRange;
                cooldown = punchCooldown;
                lastPunchTime = Time.time;
            }
            else
            {
                damage = kickDamage;
                range = attackRange;
                cooldown = kickCooldown;
                lastKickTime = Time.time;
            }
            weaponName = attackType;
        }

        PerformAttack(damage, range, weaponName);
    }

    private void Punch()
    {
        lastPunchTime = Time.time;
        PerformAttack(punchDamage, attackRange, "Punch");
    }

    private void Kick()
    {
        lastKickTime = Time.time;
        PerformAttack(kickDamage, attackRange, "Kick");
    }

    private void PerformAttack(float damage, float range, string attackName)
    {
        isAttacking = true;
        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position, 
            range, 
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            NPC npc = enemy.GetComponent<NPC>();
            if (npc != null)
            {
                npc.TakeDamage(damage);
            }
            
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
            }
        }

        Debug.Log($"{attackName} dealt {damage} damage to {hitEnemies.Length} enemies");
        Invoke("ResetAttack", 0.2f);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    public void SetAttackPoint(Transform point)
    {
        attackPoint = point;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}