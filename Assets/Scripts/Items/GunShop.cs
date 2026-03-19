using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gun shop where player can buy guns and ammo
/// </summary>
public class GunShop : MonoBehaviour
{
    [SerializeField] private string shopName = "Gun Shop";
    [SerializeField] private Vector2 shopLocation;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float ammoRefillCost = 50f;
    [SerializeField] private int ammoRefillAmount = 60;
    [SerializeField] private float armourCost = 150f;
    [SerializeField] private int armourAmount = 50;
    [SerializeField] private float grenadeCost = 100f;
    [SerializeField] private int grenadeAmount = 5;

    private List<Gun> inventory = new List<Gun>();

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        shopLocation = transform.position;
        InitializeInventory();
        SetAppearance();
    }

    private void InitializeInventory()
    {
        // Pistol: balanced, affordable
        inventory.Add(new Gun(Gun.GunType.Pistol, "9mm Pistol", 15f, 4f, 15, 25f, 0.8f, 200f));
        
        // Revolver: powerful, slow
        inventory.Add(new Gun(Gun.GunType.Revolver, "Magnum Revolver", 30f, 1.5f, 6, 30f, 0.85f, 400f));
        
        // SMG: fast, low damage
        inventory.Add(new Gun(Gun.GunType.SMG, "Submachine Gun", 12f, 10f, 30, 20f, 0.7f, 300f));
        
        // Shotgun: high damage, short range
        inventory.Add(new Gun(Gun.GunType.Shotgun, "Combat Shotgun", 40f, 1f, 8, 15f, 0.6f, 500f));
        
        // Assault Rifle: balanced all-rounder
        inventory.Add(new Gun(Gun.GunType.AssaultRifle, "Assault Rifle", 22f, 6f, 30, 40f, 0.75f, 600f));
        
        // Sniper: slow, powerful, long range
        inventory.Add(new Gun(Gun.GunType.SniperRifle, "Sniper Rifle", 50f, 0.5f, 5, 60f, 0.95f, 800f));

        // RPG: devastating, most expensive
        inventory.Add(new Gun(Gun.GunType.RPG, "Rocket Launcher", 100f, 0.2f, 5, 50f, 0.9f, 2000f));
    }

    private void SetAppearance()
    {
        // Gun shop color - red and black (weapons dealer aesthetic)
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0.8f, 0f, 0f, 1f);
    }

    public List<Gun> GetInventory() => inventory;
    public string GetShopName() => shopName;
    public Vector2 GetShopLocation() => shopLocation;
    public float GetAmmoRefillCost() => ammoRefillCost;
    public int GetAmmoRefillAmount() => ammoRefillAmount;
    public float GetArmourCost() => armourCost;
    public int GetArmourAmount() => armourAmount;
    public float GetGrenadeCost() => grenadeCost;
    public int GetGrenadeAmount() => grenadeAmount;

    public void PurchaseGun(Gun gun, PlayerManager player, GameManager gameManager)
    {
        if (gameManager.CanAfford(gun.Cost))
        {
            gameManager.AddMoney(-gun.Cost);
            player.EquipGun(gun);
            Debug.Log($"Purchased and equipped {gun.Name} for {gun.Cost}");
        }
        else
        {
            Debug.Log($"Cannot afford {gun.Name}. Need: {gun.Cost}, Have: {gameManager.Money}");
        }
    }

    public void RefillAmmo(PlayerManager player, GameManager gameManager)
    {
        if (gameManager.CanAfford(ammoRefillCost))
        {
            gameManager.AddMoney(-ammoRefillCost);
            player.GainAmmo(ammoRefillAmount);
            Debug.Log($"Refilled ammo for {ammoRefillCost}");
        }
        else
        {
            Debug.Log($"Cannot afford ammo refill. Need: {ammoRefillCost}, Have: {gameManager.Money}");
        }
    }

    public void PurchaseArmour(PlayerManager player, GameManager gameManager)
    {
        if (gameManager.CanAfford(armourCost))
        {
            gameManager.AddMoney(-armourCost);
            player.GainArmour(armourAmount);
            Debug.Log($"Purchased body armour for {armourCost}");
        }
        else
        {
            Debug.Log($"Cannot afford armour. Need: {armourCost}, Have: {gameManager.Money}");
        }
    }

    public void PurchaseGrenades(GameManager gameManager)
    {
        if (gameManager.CanAfford(grenadeCost))
        {
            gameManager.AddMoney(-grenadeCost);
            Grenade.Instance.AddGrenades(grenadeAmount);
            Debug.Log($"Purchased {grenadeAmount} grenades for {grenadeCost}");
        }
        else
        {
            Debug.Log($"Cannot afford grenades. Need: {grenadeCost}, Have: {gameManager.Money}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw shop zone in editor
        Gizmos.color = new Color(0.8f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}
