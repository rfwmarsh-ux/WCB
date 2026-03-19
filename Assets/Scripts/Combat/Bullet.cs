using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 direction;
    private float speed = 20f;
    private float damage;
    private float maxRange;
    private Gun.GunType gunType;
    private Vector2 startPosition;
    private int shooterPlayer = 1;

    public void Setup(Vector2 position, Vector2 dir, float dmg, float range, Gun.GunType type, int shooter = 1)
    {
        transform.position = position;
        direction = dir;
        damage = dmg;
        maxRange = range;
        gunType = type;
        startPosition = position;
        shooterPlayer = shooter;
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (Vector2.Distance(startPosition, transform.position) > maxRange)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Vehicle"))
        {
            return;
        }

        bool isVsMode = GameModeManager.Instance != null && GameModeManager.Instance.IsVs();

        if (isVsMode)
        {
            if (other.name == "Player1" && shooterPlayer != 1)
            {
                VsModeSystem.Instance?.DealDamageToPlayer(1, damage);
                ReturnToPool();
                return;
            }
            if (other.name == "Player2" && shooterPlayer != 2)
            {
                VsModeSystem.Instance?.DealDamageToPlayer(2, damage);
                ReturnToPool();
                return;
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                return;
            }
        }

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            ApplyDamage(enemy);
        }

        NPC npc = other.GetComponent<NPC>();
        if (npc != null)
        {
            npc.SetKillerPlayer(shooterPlayer);
            npc.TakeDamage(damage);
        }

        if (other.CompareTag("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            ReturnToPool();
        }
    }

    private void ApplyDamage(Enemy enemy)
    {
        enemy.TakeDamage(damage);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        BulletPool.Instance.ReturnBullet(gameObject);
    }
}