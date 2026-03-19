using UnityEngine;
using System.Collections.Generic;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD player1HUD { get; private set; }
    public static PlayerHUD player2HUD { get; private set; }

    [Header("Player Settings")]
    public int playerId = 1;
    public float maxHealth = 100f;
    public float startingHealth = 100f;
    public float maxArmour = 100f;
    public float currentArmour = 0f;

    [Header("Weapon Settings")]
    public List<Weapon> weapons = new List<Weapon>();
    public int currentWeaponIndex = 0;

    [Header("HUD Positions - Player 1")]
    public Vector2 p1HealthBarPosition = new Vector2(20, 40);
    public Vector2 p1WeaponDisplayPosition = new Vector2(20, 100);
    public Vector2 p1MiniMapPosition = new Vector2(-130, 130);
    public Vector2 p1WantedPosition = new Vector2(20, 85);

    [Header("HUD Positions - Player 2")]
    public Vector2 p2HealthBarPosition = new Vector2(20, 40);
    public Vector2 p2WeaponDisplayPosition = new Vector2(20, 100);
    public Vector2 p2MiniMapPosition = new Vector2(-130, 130);
    public Vector2 p2WantedPosition = new Vector2(20, 85);

    public float barWidth = 180f;
    public float barHeight = 16f;

    [Header("Mini-Map Settings")]
    public float miniMapSize = 120f;
    public float miniMapScale = 0.15f;
    
    [Header("Speed-Based Zoom Settings")]
    public float minZoomSpeed = 5f;
    public float maxZoomSpeed = 30f;
    public float minZoomSize = 120f;
    public float maxZoomSize = 200f;
    public float zoomSmoothSpeed = 3f;
    
    [Header("Wanted Sign Settings")]
    public float wantedSignSize = 30f;
    public float wantedTickSize = 8f;
    public float wantedSignOffset = 35f;

    private PlayerManager player;
    private Player2Manager player2;
    private bool isSplitScreen;
    
    private int lastWantedLevel = 0;
    private float wantedSignGlow = 0f;
    private float wantedSignGlowSpeed = 2f;
    private bool wantedSignLit = false;

    private float currentMinimapSize;
    private float targetMinimapSize;
    private float playerSpeed;

    private void Awake()
    {
        if (playerId == 1)
        {
            if (player1HUD != null && player1HUD != this) Destroy(player1HUD);
            player1HUD = this;
        }
        else
        {
            if (player2HUD != null && player2HUD != this) Destroy(player2HUD);
            player2HUD = this;
        }
    }

    private void Start()
    {
        if (playerId == 1)
            player = PlayerManager.Instance;
        else
            player2 = Player2Manager.Instance;

        currentArmour = 0f;
        currentMinimapSize = miniMapSize;
        targetMinimapSize = miniMapSize;
        isSplitScreen = SplitScreenManager.Instance != null && SplitScreenManager.Instance.isSplitScreenActive;
        
        AdjustPositionsForSplitScreen();
        InitializeWeapons();
        
        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.OnWantedLevelChanged += OnWantedLevelChanged;
            lastWantedLevel = WantedLevelManager.Instance.GetPlayerWantedLevel(playerId);
        }
    }
    
    private void OnDestroy()
    {
        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.OnWantedLevelChanged -= OnWantedLevelChanged;
        }
    }
    
    private void OnWantedLevelChanged(int changedPlayerId, int newLevel)
    {
        if (changedPlayerId == playerId)
        {
            lastWantedLevel = newLevel;
        }
    }
    
    private void Update()
    {
        UpdateWantedSignGlow();
        UpdateMinimapZoom();
    }

    private void UpdateMinimapZoom()
    {
        Vector2 playerPos = GetPlayerPosition();
        float speed = 0f;

        Rigidbody2D rb = null;
        if (playerId == 1 && player != null)
            rb = player.GetComponent<Rigidbody2D>();
        else if (player2 != null)
            rb = player2.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            speed = rb.velocity.magnitude;
        }
        else
        {
            Vector2 currentPos = playerPos;
            playerSpeed = Vector2.Distance(currentPos, playerPos) / Time.deltaTime;
            speed = playerSpeed;
        }

        float speedFactor = Mathf.InverseLerp(minZoomSpeed, maxZoomSpeed, speed);
        targetMinimapSize = Mathf.Lerp(minZoomSize, maxZoomSize, speedFactor);

        currentMinimapSize = Mathf.Lerp(currentMinimapSize, targetMinimapSize, Time.deltaTime * zoomSmoothSpeed);
    }
    
    private void UpdateWantedSignGlow()
    {
        int currentWanted = WantedLevelManager.Instance?.GetPlayerWantedLevel(playerId) ?? 0;
        
        if (currentWanted > 0 && !wantedSignLit)
        {
            wantedSignLit = true;
        }
        else if (currentWanted == 0 && wantedSignLit)
        {
            wantedSignLit = false;
            wantedSignGlow = 0f;
        }
        
        if (wantedSignLit)
        {
            wantedSignGlow = 0.5f + 0.5f * Mathf.Sin(Time.time * wantedSignGlowSpeed);
        }
    }

    private void AdjustPositionsForSplitScreen()
    {
        if (playerId == 2 && isSplitScreen)
        {
            p1HealthBarPosition = new Vector2(Screen.width / 2 + 20, 40);
            p1WeaponDisplayPosition = new Vector2(Screen.width / 2 + 20, 100);
            p1MiniMapPosition = new Vector2(Screen.width / 2 - 170, 170);
        }
    }

    private void InitializeWeapons()
    {
        weapons = new List<Weapon>
        {
            new Weapon { name = "Pistol", damage = 10, maxAmmo = 120, currentAmmo = 60, ammoType = AmmoType.Pistol, fireRate = 0.2f, isMelee = false },
            new Weapon { name = "Fists", damage = 5, maxAmmo = 0, currentAmmo = 0, ammoType = AmmoType.None, fireRate = 0.5f, isMelee = true },
            new Weapon { name = "Knife", damage = 15, maxAmmo = 0, currentAmmo = 0, ammoType = AmmoType.None, fireRate = 0.6f, isMelee = true }
        };
        currentWeaponIndex = 0;
    }

    private void Update()
    {
        if (InGameMenuManager.Instance != null && (InGameMenuManager.Instance.isMenuOpen || InGameMenuManager.Instance.isMapOpen))
            return;

        HandleWeaponInput();
        HandleMenuInput();
    }

    private void HandleWeaponInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        string cycleUp = playerId == 1 ? "q" : "u";
        string cycleDown = playerId == 1 ? "e" : "i";
        
        if (Input.GetKeyDown(KeyCode.Q) && playerId == 1) CycleWeapon(-1);
        if (Input.GetKeyDown(KeyCode.E) && playerId == 1) CycleWeapon(1);
        if (Input.GetKeyDown(KeyCode.U) && playerId == 2) CycleWeapon(-1);
        if (Input.GetKeyDown(KeyCode.I) && playerId == 2) CycleWeapon(1);

        if (Input.GetKeyDown(KeyCode.Alpha1)) CycleWeapon(-1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) CycleWeapon(1);
        
        if (scroll > 0) CycleWeapon(1);
        else if (scroll < 0) CycleWeapon(-1);
    }

    private void HandleMenuInput()
    {
        string menuKey = playerId == 1 ? "escape" : "backspace";
        
        if (Input.GetKeyDown(KeyCode.Escape) && playerId == 1)
        {
            if (InGameMenuManager.Instance != null)
            {
                if (InGameMenuManager.Instance.isMenuOpen || InGameMenuManager.Instance.isMapOpen)
                    InGameMenuManager.Instance.CloseMenu();
                else
                    InGameMenuManager.Instance.OpenMenu();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.M) && playerId == 1)
        {
            if (InGameMenuManager.Instance != null)
            {
                if (InGameMenuManager.Instance.isMapOpen)
                    InGameMenuManager.Instance.CloseMap();
                else
                    InGameMenuManager.Instance.OpenMap();
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && playerId == 2)
        {
            if (InGameMenuManager.Instance != null)
            {
                if (InGameMenuManager.Instance.isMenuOpen || InGameMenuManager.Instance.isMapOpen)
                    InGameMenuManager.Instance.CloseMenu();
                else
                    InGameMenuManager.Instance.OpenMenu();
            }
        }
    }

    public void CycleWeapon(int direction)
    {
        if (weapons.Count <= 1) return;
        currentWeaponIndex = (currentWeaponIndex + direction + weapons.Count) % weapons.Count;
    }

    public Weapon GetCurrentWeapon()
    {
        if (currentWeaponIndex >= 0 && currentWeaponIndex < weapons.Count)
            return weapons[currentWeaponIndex];
        return null;
    }

    public void AddWeapon(Weapon weapon)
    {
        var existing = weapons.Find(w => w.name == weapon.name);
        if (existing != null)
        {
            existing.currentAmmo = Mathf.Min(existing.currentAmmo + weapon.currentAmmo, existing.maxAmmo);
        }
        else
        {
            weapons.Add(weapon);
        }
    }

    public void AddAmmo(AmmoType type, int amount)
    {
        foreach (var weapon in weapons)
        {
            if (weapon.ammoType == type)
            {
                weapon.currentAmmo = Mathf.Min(weapon.currentAmmo + amount, weapon.maxAmmo);
            }
        }
    }

    public bool HasAmmoForWeapon()
    {
        var weapon = GetCurrentWeapon();
        if (weapon == null || weapon.isMelee) return true;
        return weapon.currentAmmo > 0;
    }

    public void UseAmmo()
    {
        var weapon = GetCurrentWeapon();
        if (weapon != null && !weapon.isMelee && weapon.currentAmmo > 0)
        {
            weapon.currentAmmo--;
        }
    }

    public void AddArmour(float amount)
    {
        currentArmour = Mathf.Min(currentArmour + amount, maxArmour);
    }

    public float TakeDamage(float damage)
    {
        float remainingDamage = damage;
        
        if (currentArmour > 0)
        {
            float armourAbsorption = 0.5f;
            float armourDamage = damage * armourAbsorption;
            
            if (currentArmour >= armourDamage)
            {
                currentArmour -= armourDamage;
                remainingDamage = damage * (1 - armourAbsorption);
            }
            else
            {
                remainingDamage = damage - currentArmour;
                currentArmour = 0;
            }
        }

        return remainingDamage;
    }

    public Vector2 GetPlayerPosition()
    {
        if (playerId == 1 && player != null)
            return player.transform.position;
        if (player2 != null)
            return player2.transform.position;
        return Vector2.zero;
    }

    private void OnGUI()
    {
        if (InGameMenuManager.Instance != null && InGameMenuManager.Instance.isMapOpen)
            return;

        if (InGameMenuManager.Instance != null && !InGameMenuManager.Instance.ShouldShowMinimap())
            return;

        Vector2 healthPos = playerId == 1 ? p1HealthBarPosition : p2HealthBarPosition;
        Vector2 weaponPos = playerId == 1 ? p1WeaponDisplayPosition : p2WeaponDisplayPosition;
        Vector2 miniMapPos = playerId == 1 ? p1MiniMapPosition : p2MiniMapPosition;
        Vector2 wantedPos = playerId == 1 ? p1WantedPosition : p2WantedPosition;

        DrawHealthBar(healthPos);
        DrawArmourBar(healthPos);
        DrawWeaponDisplay(weaponPos);
        DrawMiniMap(miniMapPos);
        DrawWantedSign(wantedPos);
        DrawEdgeCompass();
    }

    private void DrawEdgeCompass()
    {
        float compassWidth = 150f;
        float compassHeight = 40f;
        float compassX = (Screen.width - compassWidth) / 2;
        float compassY = Screen.height - compassHeight - 10f;

        if (isSplitScreen && playerId == 2)
        {
            compassX = Screen.width / 2 + (Screen.width / 2 - compassWidth) / 2;
        }

        GUI.color = new Color(0, 0, 0, 0.7f);
        GUI.Box(new Rect(compassX - 3, compassY - 3, compassWidth + 6, compassHeight + 6), "");
        GUI.color = Color.white;

        float playerDirection = 0f;
        if (PlayerManager.Instance != null)
        {
            Rigidbody2D rb = PlayerManager.Instance.GetComponent<Rigidbody2D>();
            if (rb != null && rb.velocity.magnitude > 0.5f)
            {
                playerDirection = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            }
            else
            {
                Transform t = PlayerManager.Instance.transform;
                playerDirection = t.eulerAngles.z;
            }
        }

        float lookDirection = 90f - playerDirection;

        string[] directions = { "W", "NW", "N", "NE", "E", "SE", "S", "SW", "W" };
        float[] angles = { 270f, 315f, 0f, 45f, 90f, 135f, 180f, 225f, 270f };

        float visibleAngleRange = 120f;
        float startAngle = lookDirection - visibleAngleRange / 2;
        float endAngle = lookDirection + visibleAngleRange / 2;

        GUI.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        GUI.DrawLine(
            new Vector2(compassX + compassWidth / 2, compassY + compassHeight / 2),
            new Vector2(compassX, compassY + compassHeight / 2)
        );
        GUI.DrawLine(
            new Vector2(compassX + compassWidth / 2, compassY + compassHeight / 2),
            new Vector2(compassX + compassWidth, compassY + compassHeight / 2)
        );

        for (int i = 0; i < directions.Length; i++)
        {
            float angle = angles[i];
            float relativeAngle = NormalizeAngle(angle - lookDirection);

            if (Mathf.Abs(relativeAngle) <= visibleAngleRange / 2 + 15f)
            {
                float xPos = compassX + compassWidth / 2 + (relativeAngle / (visibleAngleRange / 2)) * (compassWidth / 2);

                if (xPos >= compassX && xPos <= compassX + compassWidth)
                {
                    Color dirColor = directions[i] == "N" ? Color.red : Color.white;
                    GUI.color = dirColor;

                    int fontSize = GUI.skin.label.fontSize;
                    GUI.skin.label.fontSize = 14;
                    GUI.skin.label.alignment = TextAnchor.MiddleCenter;

                    if (directions[i] == "N")
                    {
                        GUI.color = Color.red;
                        GUI.Label(new Rect(xPos - 15, compassY - 5, 30, 20), "▲ " + directions[i]);
                    }
                    else
                    {
                        GUI.Label(new Rect(xPos - 15, compassY + compassHeight / 2, 30, 20), directions[i]);
                    }

                    GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    GUI.DrawLine(
                        new Vector2(compassX + compassWidth / 2, compassY + compassHeight),
                        new Vector2(xPos, compassY + compassHeight + 5)
                    );

                    GUI.skin.label.fontSize = fontSize;
                    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                }
            }
        }

        GUI.color = Color.yellow;
        GUI.DrawLine(
            new Vector2(compassX + compassWidth / 2 - 2, compassY + compassHeight),
            new Vector2(compassX + compassWidth / 2 + 2, compassY + compassHeight)
        );

        GUI.color = Color.white;
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

    private void DrawHealthBar(Vector2 pos)
    {
        float health = playerId == 1 ? (player?.Health ?? startingHealth) : (player2?.Health ?? startingHealth);
        float healthPercent = health / maxHealth;

        GUI.Box(new Rect(pos.x, pos.y, barWidth + 60, barHeight + 25), "");
        
        string playerLabel = playerId == 1 ? "PLAYER 1" : "PLAYER 2";
        Color labelColor = playerId == 1 ? Color.white : Color.cyan;
        
        GUI.color = labelColor;
        GUI.Label(new Rect(pos.x + 5, pos.y + 2, 100, 20), playerLabel);
        GUI.color = Color.white;

        Color originalColor = GUI.color;
        Color healthColor = healthPercent > 0.5f ? Color.green : (healthPercent > 0.25f ? Color.yellow : Color.red);
        GUI.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        GUI.DrawTexture(new Rect(pos.x + 5, pos.y + 22, barWidth, barHeight), Texture2D.whiteTexture);
        
        GUI.color = healthColor;
        GUI.DrawTexture(new Rect(pos.x + 5, pos.y + 22, barWidth * healthPercent, barHeight), Texture2D.whiteTexture);
        
        GUI.color = originalColor;
        GUI.Label(new Rect(pos.x + barWidth + 10, pos.y + 22, 50, 20), $"{health:F0}/{maxHealth}");
    }

    private void DrawArmourBar(Vector2 pos)
    {
        float armourPercent = currentArmour / maxArmour;
        float barY = pos.y + barHeight + 30;

        GUI.Box(new Rect(pos.x, barY, barWidth + 60, barHeight + 25), "");
        
        GUI.Label(new Rect(pos.x + 5, barY + 2, 100, 20), "ARMOUR");

        Color originalColor = GUI.color;
        GUI.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        GUI.DrawTexture(new Rect(pos.x + 5, barY + 22, barWidth, barHeight), Texture2D.whiteTexture);
        
        GUI.color = Color.cyan;
        GUI.DrawTexture(new Rect(pos.x + 5, barY + 22, barWidth * armourPercent, barHeight), Texture2D.whiteTexture);
        
        GUI.color = originalColor;
        GUI.Label(new Rect(pos.x + barWidth + 10, barY + 22, 50, 20), $"{currentArmour:F0}/{maxArmour}");
    }

    private void DrawWeaponDisplay(Vector2 pos)
    {
        var weapon = GetCurrentWeapon();
        if (weapon == null) return;

        GUI.Box(new Rect(pos.x, pos.y, 280, 50), "");

        Sprite weaponSprite = GTASpriteGenerator.GetWeaponSprite(weapon.ammoType);
        if (weaponSprite != null)
        {
            Rect spriteRect = new Rect(pos.x + 5, pos.y + 5, 40, 40);
            GUI.DrawTexture(spriteRect, weaponSprite.texture);
        }

        string weaponInfo = weapon.name;
        if (!weapon.isMelee)
        {
            weaponInfo += $" | {weapon.currentAmmo}/{weapon.maxAmmo}";
        }

        GUI.Label(new Rect(pos.x + 55, pos.y + 15, 220, 20), weaponInfo);
    }

    private void DrawMiniMap(Vector2 pos)
    {
        Vector2 playerPos = GetPlayerPosition();
        
        float mapX = playerId == 1 ? pos.x : pos.x;
        float mapY = pos.y;

        float dynamicMapSize = currentMinimapSize;

        GUI.color = new Color(0, 0, 0, 0.7f);
        GUI.Box(new Rect(mapX - 5, mapY - 5, dynamicMapSize + 10, dynamicMapSize + 10), "");
        GUI.color = Color.white;

        DrawMiniMapContent(mapX, mapY, playerPos, dynamicMapSize);
        DrawCompass(mapX, mapY, dynamicMapSize);
    }

    private void DrawCompass(float mapX, float mapY, float mapSize)
    {
        float compassSize = 24f;
        float compassX = mapX + mapSize - compassSize - 2;
        float compassY = mapY + mapSize - compassSize - 2;

        GUI.color = new Color(0, 0, 0, 0.85f);
        GUI.Box(new Rect(compassX - 2, compassY - 2, compassSize + 4, compassSize + 4), "");
        GUI.color = Color.white;

        float playerDirection = 0f;
        if (GameCameraManager.Instance != null)
        {
            playerDirection = GameCameraManager.Instance.GetPlayerDirection();
        }
        else if (PlayerManager.Instance != null)
        {
            Rigidbody2D rb = PlayerManager.Instance.GetComponent<Rigidbody2D>();
            if (rb != null && rb.velocity.magnitude > 0.5f)
            {
                playerDirection = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            }
        }

        float northAngle = 90f - playerDirection;
        float eastAngle = northAngle - 90f;
        float southAngle = northAngle - 180f;
        float westAngle = northAngle - 270f;

        DrawCompassDirection(compassX, compassY, compassSize, northAngle, "N", Color.red);
        DrawCompassDirection(compassX, compassY, compassSize, southAngle, "S", Color.white);
        DrawCompassDirection(compassX, compassY, compassSize, eastAngle, "E", Color.yellow);
        DrawCompassDirection(compassX, compassY, compassSize, westAngle, "W", Color.white);

        GUI.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        GUI.DrawLine(
            new Vector2(compassX + compassSize / 2 - 2, compassY + compassSize / 2),
            new Vector2(compassX + compassSize / 2 + 2, compassY + compassSize / 2)
        );
        GUI.DrawLine(
            new Vector2(compassX + compassSize / 2, compassY + compassSize / 2 - 2),
            new Vector2(compassX + compassSize / 2, compassY + compassSize / 2 + 2)
        );

        GUI.color = Color.white;
    }

    private void DrawCompassDirection(float centerX, float centerY, float size, float worldAngle, string label, Color color)
    {
        float rad = worldAngle * Mathf.Deg2Rad;
        float radius = size / 2 - 6f;
        
        float x = centerX + size / 2 + Mathf.Cos(rad) * radius - 4f;
        float y = centerY + size / 2 + Mathf.Sin(rad) * radius - 6f;

        if (x >= centerX && x <= centerX + size - 8 && y >= centerY && y <= centerY + size - 12)
        {
            GUI.color = color;
            GUI.Label(new Rect(x, y, 8, 12), label);
        }
    }

    private void DrawMiniMapContent(float mapX, float mapY, Vector2 playerPos, float mapSize)
    {
        float scale = mapSize / 1000f;
        Vector2 center = new Vector2(mapX + mapSize / 2, mapY + mapSize / 2);

        if (AreaManager.Instance != null)
        {
            foreach (var area in AreaManager.Instance.GetAreasOfType(AreaType.Park))
            {
                DrawMiniMarker(area.Position, playerPos, center, scale, Color.green, 5f, mapX, mapY, mapSize);
            }
        }

        DrawMiniUsableBuildings(playerPos, center, scale, mapX, mapY, mapSize);
        DrawMiniPlayers(playerPos, center, scale, mapX, mapY, mapSize);
    }

    private void DrawMiniUsableBuildings(Vector2 playerPos, Vector2 center, float scale, float mapX, float mapY, float mapSize)
    {
        if (BuildingIconsManager.Instance != null)
        {
            foreach (var icon in BuildingIconsManager.Instance.GetAllIcons())
            {
                Color color = GetIconColor(icon.iconType);
                DrawMiniMarker(icon.worldPosition, playerPos, center, scale, color, 7f, mapX, mapY, mapSize);
            }
        }
        else
        {
            if (VeterinaryCentreManager.Instance != null)
            {
                foreach (var centre in VeterinaryCentreManager.Instance.GetAllCentres())
                {
                    DrawMiniMarker(centre.transform.position, playerPos, center, scale, Color.cyan, 8f, mapX, mapY, mapSize);
                }
            }

            if (ScrapyardManager.Instance != null)
            {
                foreach (var yard in ScrapyardManager.Instance.GetAllScrapyards())
                {
                    DrawMiniMarker(yard.transform.position, playerPos, center, scale, Color.magenta, 8f, mapX, mapY, mapSize);
                }
            }

            if (GunShopManager.Instance != null)
            {
                foreach (var shop in GunShopManager.Instance.GetAllGunShops())
                {
                    DrawMiniMarker(shop.transform.position, playerPos, center, scale, Color.red, 8f, mapX, mapY, mapSize);
                }
            }
        }

        DrawMiniNavigationRoute(playerPos, center, scale, mapX, mapY, mapSize);
    }

    private Color GetIconColor(BuildingIconType type)
    {
        return type switch
        {
            BuildingIconType.GunShop => Color.red,
            BuildingIconType.Scrapyard => new Color(0.6f, 0.45f, 0.25f),
            BuildingIconType.Hospital => Color.cyan,
            BuildingIconType.Police => Color.blue,
            BuildingIconType.Church => new Color(0.7f, 0.68f, 0.6f),
            BuildingIconType.Clothes => new Color(0.6f, 0.35f, 0.6f),
            BuildingIconType.Barber => new Color(0.7f, 0.55f, 0.3f),
            BuildingIconType.Restaurant => new Color(0.8f, 0.5f, 0.2f),
            BuildingIconType.Taxi => Color.yellow,
            BuildingIconType.Bus => new Color(0.2f, 0.5f, 0.8f),
            BuildingIconType.Train => new Color(0.4f, 0.35f, 0.3f),
            BuildingIconType.Mission => Color.yellow,
            BuildingIconType.Navigation => new Color(0.2f, 0.6f, 1f),
            _ => Color.white
        };
    }

    private void DrawMiniNavigationRoute(Vector2 playerPos, Vector2 center, float scale, float mapX, float mapY, float mapSize)
    {
        if (NavigationSystem.Instance == null || !NavigationSystem.Instance.IsNavigating()) return;

        var route = NavigationSystem.Instance.GetCurrentRoute();
        if (route == null || route.Count < 2) return;

        GUI.color = new Color(0.2f, 0.6f, 1f, 0.6f);

        for (int i = 0; i < route.Count - 1; i++)
        {
            Vector2 start = route[i];
            Vector2 end = route[i + 1];

            Vector2 startOffset = (start - playerPos) * scale;
            Vector2 endOffset = (end - playerPos) * scale;

            Vector2 startScreen = center + startOffset;
            Vector2 endScreen = center + endOffset;

            if (startScreen.x >= mapX && startScreen.x <= mapX + mapSize &&
                startScreen.y >= mapY && startScreen.y <= mapY + mapSize)
            {
                GUI.DrawLine(
                    new Vector2(startScreen.x, startScreen.y),
                    new Vector2(endScreen.x, endScreen.y)
                );
            }
        }

        Vector2 target = NavigationSystem.Instance.GetCurrentTarget().Value;
        Vector2 targetOffset = (target - playerPos) * scale;
        Vector2 targetScreen = center + targetOffset;

        if (targetScreen.x >= mapX && targetScreen.x <= mapX + mapSize &&
            targetScreen.y >= mapY && targetScreen.y <= mapY + mapSize)
        {
            GUI.color = Color.yellow;
            GUI.DrawTexture(new Rect(targetScreen.x - 4, targetScreen.y - 4, 8, 8), Texture2D.whiteTexture);
        }

        GUI.color = Color.white;
    }

    private void DrawMiniMarker(Vector2 worldPos, Vector2 playerPos, Vector2 center, float scale, Color color, float size = 5f, float mapX = 0, float mapY = 0, float mapSize = 120f)
    {
        if (Vector2.Distance(worldPos, playerPos) > 500f) return;

        Vector2 offset = (worldPos - playerPos) * scale;
        Vector2 screenPos = center + offset;
        
        if (screenPos.x < mapX || screenPos.x > mapX + mapSize ||
            screenPos.y < mapY || screenPos.y > mapY + mapSize)
            return;

        GUI.color = color;
        GUI.DrawTexture(new Rect(screenPos.x - size / 2, screenPos.y - size / 2, size, size), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void DrawMiniPlayers(Vector2 playerPos, Vector2 center, float scale, float mapX, float mapY, float mapSize)
    {
        Vector2 p1Screen = center;
        
        Color p1Color = playerId == 1 ? Color.white : Color.gray;
        GUI.color = p1Color;
        GUI.DrawTexture(new Rect(p1Screen.x - 6, p1Screen.y - 6, 12, 12), Texture2D.whiteTexture);

        if (GameModeManager.Instance != null && GameModeManager.Instance.IsCoop())
        {
            Vector2 otherPlayerPos = playerId == 1 
                ? (Player2Manager.Instance?.transform.position ?? Vector2.zero)
                : (PlayerManager.Instance?.transform.position ?? Vector2.zero);
            
            Vector2 otherOffset = (otherPlayerPos - playerPos) * scale;
            Vector2 otherScreen = center + otherOffset;
            
            if (otherScreen.x >= mapX && otherScreen.x <= mapX + mapSize &&
                otherScreen.y >= mapY && otherScreen.y <= mapY + mapSize)
            {
                Color otherColor = playerId == 1 ? Color.blue : Color.yellow;
                GUI.color = otherColor;
                GUI.DrawTexture(new Rect(otherScreen.x - 5, otherScreen.y - 5, 10, 10), Texture2D.whiteTexture);
            }
        }

        GUI.color = Color.white;
    }
    
    private void DrawWantedSign(Vector2 pos)
    {
        int wantedLevel = WantedLevelManager.Instance?.GetPlayerWantedLevel(playerId) ?? 0;
        
        float baseX = pos.x;
        float baseY = pos.y;
        
        Color signColor;
        if (wantedLevel == 0)
        {
            signColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        }
        else
        {
            float glow = wantedSignGlow;
            float intensity = 0.3f + 0.7f * glow;
            signColor = new Color(intensity, 0f, 0f, 0.8f + 0.2f * glow);
        }
        
        GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);
        GUI.DrawTexture(new Rect(baseX - 2, baseY - 2, wantedSignSize + 4, wantedSignSize + 4), Texture2D.whiteTexture);
        
        GUI.color = signColor;
        GUI.DrawTexture(new Rect(baseX, baseY, wantedSignSize, wantedSignSize), Texture2D.whiteTexture);
        
        string wantedText = "W";
        GUI.color = wantedLevel > 0 ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        GUIStyle textStyle = new GUIStyle(GUI.skin.label);
        textStyle.fontSize = 16;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(baseX, baseY, wantedSignSize, wantedSignSize), wantedText, textStyle);
        
        if (wantedLevel > 0)
        {
            float tickSpacing = wantedTickSize + 2f;
            float totalTicksWidth = 5 * tickSpacing - 2f;
            float tickStartX = baseX + (wantedSignSize - totalTicksWidth) / 2f;
            float tickY = baseY + wantedSignSize + 3f;
            
            for (int i = 0; i < 5; i++)
            {
                float tickX = tickStartX + i * tickSpacing;
                bool isActive = i < wantedLevel;
                
                GUI.color = isActive ? Color.red : new Color(0.2f, 0.2f, 0.2f, 0.5f);
                GUI.DrawTexture(new Rect(tickX, tickY, wantedTickSize, wantedTickSize), Texture2D.whiteTexture);
            }
        }
        
        GUI.color = Color.white;
    }
}

public class Weapon
{
    public string name;
    public int damage;
    public int maxAmmo;
    public int currentAmmo;
    public AmmoType ammoType;
    public float fireRate;
    public bool isMelee;
}

public enum AmmoType
{
    None,
    Pistol,
    Rifle,
    Shotgun,
    Sniper,
    Explosive,
    Laser
}
