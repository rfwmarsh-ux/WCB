using UnityEngine;
using System.Collections;

public class LaserGun : MonoBehaviour
{
    public static LaserGun Instance { get; private set; }

    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private Light muzzleFlash;
    [SerializeField] private float laserDuration = 0.1f;
    [SerializeField] private float chargeTime = 1.5f;
    [SerializeField] private float overheatCooldown = 3f;
    
    private float currentCharge = 0f;
    private bool isOverheated = false;
    private bool isFiring = false;

    public string Name => "Laser Cannon";
    public float Damage => 80f;
    public float FireRate => 2f;
    public float Range => 50f;
    public int MagazineSize => 20;
    public int AmmoInMagazine { get; private set; } = 20;
    public int CurrentAmmo { get; private set; } = 40;
    public float Cost => 0;
    public Gun.GunType Type => Gun.GunType.RPG;
    public bool IsLaser => true;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        if (laserLine == null)
        {
            laserLine = gameObject.AddComponent<LineRenderer>();
            laserLine.startWidth = 0.1f;
            laserLine.endWidth = 0.3f;
            laserLine.material = new Material(Shader.Find("Sprites/Default"));
            laserLine.startColor = Color.cyan;
            laserLine.endColor = Color.blue;
            laserLine.enabled = false;
        }

        if (muzzleFlash == null)
        {
            GameObject flash = new GameObject("MuzzleFlash");
            flash.transform.parent = transform;
            muzzleFlash = flash.AddComponent<Light>();
            muzzleFlash.color = Color.cyan;
            muzzleFlash.intensity = 0;
            muzzleFlash.range = 5f;
        }
    }

    public void Fire(Vector2 direction, Transform firePoint)
    {
        if (isOverheated || AmmoInMagazine <= 0) return;

        AmmoInMagazine--;
        currentCharge += 0.15f;

        if (currentCharge >= 1f)
        {
            StartCoroutine(OverheatRoutine());
        }

        StartCoroutine(FireLaserRoutine(direction, firePoint));
    }

    private IEnumerator FireLaserRoutine(Vector2 direction, Transform firePoint)
    {
        laserLine.enabled = true;
        laserLine.SetPosition(0, firePoint.position);
        
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, Range);
        Vector2 endPoint = hit.point == Vector2.zero ? (Vector2)firePoint.position + direction * Range : hit.point;
        
        laserLine.SetPosition(1, endPoint);
        muzzleFlash.intensity = 5f;

        if (hit.collider != null)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(Damage);
            }

            NPC npc = hit.collider.GetComponent<NPC>();
            if (npc != null)
            {
                npc.TakeDamage(Damage);
            }

            if (hit.collider.CompareTag("Vehicle"))
            {
                Vehicle vehicle = hit.collider.GetComponent<Vehicle>();
                if (vehicle != null)
                {
                    vehicle.TakeDamage(Damage * 0.5f);
                }
            }
        }

        yield return new WaitForSeconds(laserDuration);
        
        laserLine.enabled = false;
        muzzleFlash.intensity = 0;
    }

    private IEnumerator OverheatRoutine()
    {
        isOverheated = true;
        Debug.Log("Laser Cannon overheated! Cooling down...");
        
        yield return new WaitForSeconds(overheatCooldown);
        
        isOverheated = false;
        currentCharge = 0f;
        Debug.Log("Laser Cannon ready!");
    }

    public void Reload()
    {
        int ammoNeeded = MagazineSize - AmmoInMagazine;
        int ammoToReload = Mathf.Min(ammoNeeded, CurrentAmmo);
        AmmoInMagazine += ammoToReload;
        CurrentAmmo -= ammoToReload;
    }

    public bool HasAmmo() => AmmoInMagazine > 0 || CurrentAmmo > 0;
    public bool IsOverheated() => isOverheated;
    public float GetChargeLevel() => currentCharge;
}