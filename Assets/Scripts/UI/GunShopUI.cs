using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// UI display for gun shop interface and player weapons
/// </summary>
public class GunShopUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentGunText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI gunListText;
    [SerializeField] private TextMeshProUGUI shopInfoText;
    [SerializeField] private PlayerManager playerManager;

    private GunShop currentShop;
    private bool isInShop = false;

    private void Start()
    {
        if (playerManager == null)
            playerManager = GameManager.Instance?.GetComponent<PlayerManager>();
    }

    private void Update()
    {
        if (playerManager == null) return;

        UpdateWeaponDisplay();
    }

    private void UpdateWeaponDisplay()
    {
        Gun gun = playerManager.GetCurrentGun();
        
        if (gun != null && currentGunText != null)
        {
            currentGunText.text = $"Equipped: {gun.Name}";
            currentGunText.color = Color.cyan;
        }

        if (ammoText != null)
        {
            if (gun != null)
                ammoText.text = $"Ammo: {gun.AmmoInMagazine}/{gun.MagazineSize} | Reserve: {gun.CurrentAmmo}";
            else
                ammoText.text = "Ammo: No weapon equipped";
            
            ammoText.color = gun != null && gun.HasAmmo() ? Color.green : Color.red;
        }
    }

    public void EnterShop(GunShop shop)
    {
        currentShop = shop;
        isInShop = true;

        if (shopInfoText != null)
        {
            shopInfoText.text = $"Welcome to {shop.GetShopName()}\n\n";
            shopInfoText.text += $"Ammo Refill: {shop.GetAmmoRefillCost()}$ for {shop.GetAmmoRefillAmount()} rounds\n";
            shopInfoText.text += $"Body Armour: {shop.GetArmourCost()}$ for {shop.GetArmourAmount()} armour\n\n";
            shopInfoText.text += "AVAILABLE GUNS:\n";

            foreach (var gun in shop.GetInventory())
            {
                shopInfoText.text += $"\n{gun.Name}\n";
                shopInfoText.text += $"  Damage: {gun.Damage} | Fire Rate: {gun.FireRate} | Range: {gun.Range}\n";
                shopInfoText.text += $"  Accuracy: {gun.Accuracy:P0} | Magazine: {gun.MagazineSize}\n";
                shopInfoText.text += $"  Cost: {gun.Cost}$\n";
            }
        }
    }

    public void ExitShop()
    {
        isInShop = false;
        currentShop = null;

        if (shopInfoText != null)
            shopInfoText.text = "";
    }

    public void BuyGun(Gun.GunType gunType)
    {
        if (currentShop == null) return;

        Gun gunToBuy = currentShop.GetInventory().Find(g => g.Type == gunType);
        if (gunToBuy != null)
        {
            if (GameManager.Instance.CanAfford(gunToBuy.Cost))
            {
                GameManager.Instance.AddMoney(-gunToBuy.Cost);
                Gun newGun = new Gun(gunToBuy.Type, gunToBuy.Name, gunToBuy.Damage, gunToBuy.FireRate, 
                                    gunToBuy.MagazineSize, gunToBuy.Range, gunToBuy.Accuracy, gunToBuy.Cost);
                playerManager.EquipGun(newGun);
                Debug.Log($"Purchased {gunToBuy.Name}");
            }
        }
    }

    public void BuyAmmo()
    {
        if (currentShop != null)
        {
            currentShop.RefillAmmo(playerManager, GameManager.Instance);
        }
    }

    public void BuyArmour()
    {
        if (currentShop != null)
        {
            currentShop.PurchaseArmour(playerManager, GameManager.Instance);
        }
    }

    public bool IsInShop() => isInShop;
}
