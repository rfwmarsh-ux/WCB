using UnityEngine;

/// <summary>
/// Represents a firearm with stats
/// </summary>
[System.Serializable]
public class Gun
{
    public enum GunType
    {
        Pistol,
        SMG,
        Shotgun,
        AssaultRifle,
        SniperRifle,
        Revolver,
        RPG
    }

    public GunType Type { get; set; }
    public string Name { get; set; }
    public float Damage { get; set; }
    public float FireRate { get; set; }           // Shots per second
    public int MagazineSize { get; set; }
    public int CurrentAmmo { get; set; }
    public int AmmoInMagazine { get; set; }
    public float Range { get; set; }
    public float Accuracy { get; set; }            // 0-1, where 1 is perfect accuracy
    public float Cost { get; set; }

    public Gun(GunType type, string name, float damage, float fireRate, int magSize, float range, float accuracy, float cost)
    {
        Type = type;
        Name = name;
        Damage = damage;
        FireRate = fireRate;
        MagazineSize = magSize;
        Range = range;
        Accuracy = accuracy;
        Cost = cost;
        CurrentAmmo = magSize * 3; // Start with 3 magazines worth
        AmmoInMagazine = magSize;
    }

    public void Fire()
    {
        if (AmmoInMagazine > 0)
        {
            AmmoInMagazine--;
            CurrentAmmo--;
        }
    }

    public void Reload()
    {
        int ammoNeeded = MagazineSize - AmmoInMagazine;
        int ammoToReload = Mathf.Min(ammoNeeded, CurrentAmmo);
        
        AmmoInMagazine += ammoToReload;
        CurrentAmmo -= ammoToReload;
    }

    public bool HasAmmo() => AmmoInMagazine > 0 || CurrentAmmo > 0;
    public float GetAmmoPercentage() => (float)AmmoInMagazine / MagazineSize;
}
