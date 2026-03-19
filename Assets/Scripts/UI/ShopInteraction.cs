using UnityEngine;

public class ShopInteraction : MonoBehaviour
{
    [SerializeField] private GunShopManager gunShopManager;
    [SerializeField] private BodyArmourManager bodyArmourManager;
    [SerializeField] private ScrapyardManager scrapyardManager;
    [SerializeField] private VehicleGarage garage;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            TryHealPlayer1();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            TryHealPlayer2();
        }

        HandleOutfitSelection();
    }

    private void TryHealPlayer1()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 3f);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<VeterinaryCentre>() != null)
            {
                if (VeterinaryHeal.Instance == null)
                {
                    GameObject vh = new GameObject("VeterinaryHeal");
                    vh.AddComponent<VeterinaryHeal>();
                }
                VeterinaryHeal.Instance.TryHeal();
                return;
            }
        }
    }

    private void TryHealPlayer2()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 3f);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<VeterinaryCentre>() != null)
            {
                if (VeterinaryHeal.Instance == null)
                {
                    GameObject vh = new GameObject("VeterinaryHeal");
                    vh.AddComponent<VeterinaryHeal>();
                }
                VeterinaryHeal.Instance.TryHealPlayer2();
                return;
            }
        }
    }

    private void HandleOutfitSelection()
    {
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (OutfitShop.Instance != null)
                {
                    OutfitShop.Instance.ChangeOutfit(i);
                    return;
                }
            }
        }
    }

    private void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 3f);
        
        foreach (var hit in hits)
        {
            if (hit.GetComponent<VeterinaryCentre>() != null)
            {
                OpenVeterinaryCentre();
                return;
            }

            if (hit.GetComponent<GunShop>() != null)
            {
                GunShop shop = hit.GetComponent<GunShop>();
                if (shop != null)
                {
                    OpenGunShopUI(shop);
                    return;
                }
            }

            if (hit.GetComponent<BodyArmour>() != null)
            {
                BodyArmour armour = hit.GetComponent<BodyArmour>();
                BuyArmour(armour);
                return;
            }

            if (hit.GetComponent<Scrapyard>() != null)
            {
                Scrapyard scrapyard = hit.GetComponent<Scrapyard>();
                SellVehicle(scrapyard);
                return;
            }

            if (hit.GetComponent<VehicleGarage>() != null)
            {
                StoreVehicle();
                return;
            }

            if (hit.GetComponent<CricketBat>() != null)
            {
                CollectCricketBat(hit.gameObject);
                return;
            }

            if (hit.GetComponent<LaserGunPickup>() != null)
            {
                CollectLaserGun(hit.gameObject);
                return;
            }

            if (hit.GetComponent<GrenadePickup>() != null)
            {
                CollectGrenade(hit.gameObject);
                return;
            }

            if (hit.GetComponent<ShopData>() != null)
            {
                ShopData shop = hit.GetComponent<ShopData>();
                if (shop.Type == ShopType.OutfitShop)
                {
                    OpenOutfitShop();
                    return;
                }
                else
                {
                    Debug.Log($"Approached {shop.Name} ({shop.Type})");
                    return;
                }
            }
        }
    }

    private void OpenOutfitShop()
    {
        if (FindObjectOfType<OutfitShop>() == null)
        {
            GameObject os = new GameObject("OutfitShopSystem");
            os.AddComponent<OutfitShop>();
        }

        OutfitShop.Instance.ShowOutfitMenu();
        Debug.Log("Press 1-0 to select outfit (clears wanted level for $200)");
    }

    private void CollectGrenade(GameObject g)
    {
        for (int i = 0; i < GrenadeSpawner.Instance.transform.childCount; i++)
        {
            Transform child = GrenadeSpawner.Instance.transform.GetChild(i);
            if (child.gameObject == g)
            {
                GrenadeSpawner.Instance.CollectGrenade(i);
                Debug.Log("Collected 3 grenades! G to throw, they respawn after 60 seconds.");
                return;
            }
        }
    }

    private void CollectCricketBat(GameObject bat)
    {
        SpecialItemManager.Instance.CollectCricketBat();
        Destroy(bat);
        Debug.Log("Collected Cricket Bat! Q/E to attack - weak but better than fists");
    }

    private void CollectLaserGun(GameObject laser)
    {
        SpecialItemManager.Instance.CollectLaserGun();
        Destroy(laser);
        Debug.Log("COLLECTED LASER CANNON! Click to fire - best weapon in game. L to reload.");
    }

    private void OpenGunShopUI(GunShop shop)
    {
        Debug.Log($"Opened {shop.GetShopName()} - Press 1-7 to buy weapons");
        Debug.Log("1: Pistol ($200) | 2: Revolver ($400) | 3: SMG ($300)");
        Debug.Log("4: Shotgun ($500) | 5: Assault Rifle ($600) | 6: Sniper ($800) | 7: RPG ($2000)");
        
        Gun[] inventory = shop.GetInventory().ToArray();
        for (int i = 0; i < inventory.Length; i++)
        {
            Debug.Log($"{i+1}. {inventory[i].Name} - ${inventory[i].Cost} | DMG: {inventory[i].Damage} | Mag: {inventory[i].MagazineSize}");
        }
    }

    private void BuyArmour(BodyArmour armour)
    {
        GameManager gm = GameManager.Instance;
        PlayerManager pm = PlayerManager.Instance;
        float cost = armour.GetCost();
        
        if (gm.CanAfford(cost))
        {
            gm.AddMoney(-cost);
            pm.GainArmour(armour.GetArmourAmount());
            Debug.Log($"Bought body armour for ${cost}");
        }
    }

    private void SellVehicle(Scrapyard scrapyard)
    {
        Debug.Log("Scrapyard interaction - can sell vehicles here");
    }

    private void StoreVehicle()
    {
        Debug.Log("Garage interaction - press to store/retrieve vehicles");
    }

    private void OpenVeterinaryCentre()
    {
        if (VeterinaryHeal.Instance == null)
        {
            GameObject vh = new GameObject("VeterinaryHeal");
            vh.AddComponent<VeterinaryHeal>();
        }

        PlayerManager pm = PlayerManager.Instance;
        if (pm != null && pm.GetHealth() < pm.GetMaxHealth())
        {
            Debug.Log("=== VETERINARY CENTRE ===");
            Debug.Log($"Full Heal Cost: ${VeterinaryHeal.Instance.GetCost()}");
            Debug.Log("Press H to heal");
        }
        else
        {
            Debug.Log("At full health - no need to visit veterinary centre");
        }

        if (GameModeManager.Instance != null && GameModeManager.Instance.IsPlayer2Active())
        {
            Player2Manager p2m = Player2Manager.Instance;
            if (p2m != null && p2m.GetHealth() < p2m.GetMaxHealth())
            {
                Debug.Log("Player 2 can also heal - press J");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}