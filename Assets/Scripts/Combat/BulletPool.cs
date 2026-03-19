using UnityEngine;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int initialPoolSize = 50;

    private Queue<GameObject> availableBullets = new Queue<GameObject>();
    private List<GameObject> activeBullets = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (bulletPrefab == null)
        {
            bulletPrefab = CreateBulletPrefab();
        }

        InitializePool();
    }

    private GameObject CreateBulletPrefab()
    {
        GameObject prefab = new GameObject("BulletPrefab");
        SpriteRenderer sr = prefab.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = Color.yellow;
        sr.sortingOrder = 10;
        
        CircleCollider2D collider = prefab.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        
        prefab.AddComponent<Bullet>();
        prefab.SetActive(false);
        return prefab;
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewBullet();
        }
    }

    private GameObject CreateNewBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform);
        bullet.SetActive(false);
        availableBullets.Enqueue(bullet);
        return bullet;
    }

    public GameObject GetBullet()
    {
        if (availableBullets.Count == 0)
        {
            CreateNewBullet();
        }

        GameObject bullet = availableBullets.Dequeue();
        bullet.SetActive(true);
        activeBullets.Add(bullet);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        activeBullets.Remove(bullet);
        availableBullets.Enqueue(bullet);
    }

    public void ClearActiveBullets()
    {
        foreach (var bullet in activeBullets)
        {
            bullet.SetActive(false);
            availableBullets.Enqueue(bullet);
        }
        activeBullets.Clear();
    }
}