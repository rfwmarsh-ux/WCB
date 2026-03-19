using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrenadeProjectile : MonoBehaviour
{
    private float fuseTime;
    private float explosionRadius;
    private float explosionDamage;
    private bool hasExploded = false;

    public void Setup(float fuse, float radius, float damage)
    {
        fuseTime = fuse;
        explosionRadius = radius;
        explosionDamage = damage;
        StartCoroutine(FuseTimer());
    }

    private IEnumerator FuseTimer()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        CreateExplosionEffect();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            float distance = Vector2.Distance(transform.position, hit.transform.position);
            float damageFalloff = 1f - (distance / explosionRadius);
            float finalDamage = explosionDamage * Mathf.Clamp01(damageFalloff);

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(finalDamage);
            }

            NPC npc = hit.GetComponent<NPC>();
            if (npc != null)
            {
                npc.TakeDamage(finalDamage);
            }

            Vehicle vehicle = hit.GetComponent<Vehicle>();
            if (vehicle != null)
            {
                vehicle.TakeDamage(finalDamage * 0.5f);
            }
        }

        Destroy(gameObject);
    }

    private void CreateExplosionEffect()
    {
        StartCoroutine(ExplosionEffectRoutine());
    }

    private IEnumerator ExplosionEffectRoutine()
    {
        GameObject explosion = new GameObject("Explosion");
        explosion.transform.position = transform.position;

        SpriteRenderer sr = explosion.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(1f, 0.5f, 0f, 0.9f);
        sr.sortingOrder = 15;

        explosion.transform.localScale = Vector3.one * explosionRadius * 2;

        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            explosion.transform.localScale = Vector3.one * explosionRadius * 2 * (1f - t * 0.5f);
            sr.color = new Color(1f, 0.5f * (1f - t), 0f, 0.9f * (1f - t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        CreateSmokeEffect(explosion.transform.position);
        Destroy(explosion);

        Debug.Log($"Grenade exploded! Radius: {explosionRadius}, Damage: {explosionDamage}");
    }

    private void CreateSmokeEffect(Vector2 position)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject smoke = new GameObject("Smoke");
            smoke.transform.position = position + Random.insideUnitCircle * explosionRadius * 0.3f;

            SpriteRenderer sr = smoke.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.color = new Color(0.3f, 0.3f, 0.3f, 0.6f);
            sr.sortingOrder = 5;

            float size = Random.Range(0.5f, 1.5f);
            smoke.transform.localScale = Vector3.one * size;

            StartCoroutine(SmokeRiseAndFade(smoke));
        }
    }

    private IEnumerator SmokeRiseAndFade(GameObject smoke)
    {
        float duration = 1.5f;
        float elapsed = 0f;
        Vector3 startPos = smoke.transform.position;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            smoke.transform.position = startPos + Vector3.up * t * 2f;
            smoke.transform.localScale = Vector3.one * (1f - t) * 1.5f;

            SpriteRenderer sr = smoke.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0.3f, 0.3f, 0.3f, 0.6f * (1f - t));
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(smoke);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}