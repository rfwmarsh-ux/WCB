using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSetup : MonoBehaviour
{
    public const string MAIN_MENU_SCENE = "MainMenuScene";
    public const string GAME_SCENE = "GameScene";

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }

    public static void LoadGame()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }
}

public class GameSceneSetup : MonoBehaviour
{
    [Header("Scene Configuration")]
    [SerializeField] private bool initializeOnStart = true;

    private void Start()
    {
        if (initializeOnStart)
        {
            InitializeScene();
        }
    }

    private void InitializeScene()
    {
        Debug.Log("=== Initializing Game Scene ===");

        InitializeManagers();
        InitializeMap();
        InitializePlayers();
        InitializeSystems();

        Debug.Log("=== Game Scene Ready ===");
    }

    private void InitializeManagers()
    {
        if (FindObjectOfType<GameModeManager>() == null)
        {
            new GameObject("GameModeManager").AddComponent<GameModeManager>();
        }

        if (FindObjectOfType<GameInputManager>() == null)
        {
            new GameObject("GameInputManager").AddComponent<GameInputManager>();
        }

        if (FindObjectOfType<GameInitializer>() == null)
        {
            new GameObject("GameInitializer").AddComponent<GameInitializer>();
        }

        if (FindObjectOfType<CityManager>() == null)
        {
            new GameObject("CityManager").AddComponent<CityManager>();
        }

        if (FindObjectOfType<VehicleManager>() == null)
        {
            new GameObject("VehicleManager").AddComponent<VehicleManager>();
        }

        if (FindObjectOfType<PedestrianManager>() == null)
        {
            new GameObject("PedestrianManager").AddComponent<PedestrianManager>();
        }

        if (FindObjectOfType<DayNightCycleManager>() == null)
        {
            new GameObject("DayNightCycleManager").AddComponent<DayNightCycleManager>();
        }

        if (FindObjectOfType<WeatherSystem>() == null)
        {
            new GameObject("WeatherSystem").AddComponent<WeatherSystem>();
        }

        if (FindObjectOfType<PoliceSystem>() == null)
        {
            new GameObject("PoliceSystem").AddComponent<PoliceSystem>();
        }

        if (FindObjectOfType<MissionManager>() == null)
        {
            new GameObject("MissionManager").AddComponent<MissionManager>();
        }

        if (FindObjectOfType<WantedLevelManager>() == null)
        {
            new GameObject("WantedLevelManager").AddComponent<WantedLevelManager>();
        }

        if (FindObjectOfType<SaveLoadSystem>() == null)
        {
            new GameObject("SaveLoadSystem").AddComponent<SaveLoadSystem>();
        }

        if (FindObjectOfType<InGameMenuManager>() == null)
        {
            new GameObject("InGameMenuManager").AddComponent<InGameMenuManager>();
        }

        if (FindObjectOfType<SplitScreenManager>() == null)
        {
            new GameObject("SplitScreenManager").AddComponent<SplitScreenManager>();
        }

        if (FindObjectOfType<WeaponSpawnManager>() == null)
        {
            new GameObject("WeaponSpawnManager").AddComponent<WeaponSpawnManager>();
        }

        if (FindObjectOfType<MusicManager>() == null)
        {
            new GameObject("MusicManager").AddComponent<MusicManager>();
        }
    }

    private void InitializeMap()
    {
        if (FindObjectOfType<MapSystem>() == null)
        {
            new GameObject("MapSystem").AddComponent<MapSystem>();
        }

        if (FindObjectOfType<BusinessManager>() == null)
        {
            new GameObject("BusinessManager").AddComponent<BusinessManager>();
        }

        if (FindObjectOfType<BuildingManager>() == null)
        {
            new GameObject("BuildingManager").AddComponent<BuildingManager>();
        }

        if (FindObjectOfType<MapMarkers>() == null)
        {
            new GameObject("MapMarkers").AddComponent<MapMarkers>();
        }

        if (FindObjectOfType<ChurchManager>() == null)
        {
            new GameObject("ChurchManager").AddComponent<ChurchManager>();
        }

        if (FindObjectOfType<BusManager>() == null)
        {
            new GameObject("BusManager").AddComponent<BusManager>();
        }

        if (FindObjectOfType<MetroManager>() == null)
        {
            new GameObject("MetroManager").AddComponent<MetroManager>();
        }

        if (FindObjectOfType<TrainManager>() == null)
        {
            new GameObject("TrainManager").AddComponent<TrainManager>();
        }

        if (FindObjectOfType<BridgeManager>() == null)
        {
            new GameObject("BridgeManager").AddComponent<BridgeManager>();
        }

        if (FindObjectOfType<AreaManager>() == null)
        {
            new GameObject("AreaManager").AddComponent<AreaManager>();
        }

        if (FindObjectOfType<TrafficLightManager>() == null)
        {
            new GameObject("TrafficLightManager").AddComponent<TrafficLightManager>();
        }

        if (FindObjectOfType<TrafficManager>() == null)
        {
            new GameObject("TrafficManager").AddComponent<TrafficManager>();
        }

        if (FindObjectOfType<AmbulanceManager>() == null)
        {
            new GameObject("AmbulanceManager").AddComponent<AmbulanceManager>();
        }

        if (FindObjectOfType<WaterManager>() == null)
        {
            new GameObject("WaterManager").AddComponent<WaterManager>();
        }

        if (FindObjectOfType<WaterDeathHandler>() == null)
        {
            new GameObject("WaterDeathHandler").AddComponent<WaterDeathHandler>();
        }

        if (FindObjectOfType<VeterinaryCentreManager>() == null)
        {
            new GameObject("VeterinaryCentreManager").AddComponent<VeterinaryCentreManager>();
        }

        if (FindObjectOfType<ScrapyardManager>() == null)
        {
            new GameObject("ScrapyardManager").AddComponent<ScrapyardManager>();
        }

        if (FindObjectOfType<DestroyedVehicleManager>() == null)
        {
            new GameObject("DestroyedVehicleManager").AddComponent<DestroyedVehicleManager>();
        }

        if (FindObjectOfType<TowTruckManager>() == null)
        {
            new GameObject("TowTruckManager").AddComponent<TowTruckManager>();
        }

        if (FindObjectOfType<ParkedVehicleManager>() == null)
        {
            new GameObject("ParkedVehicleManager").AddComponent<ParkedVehicleManager>();
        }

        if (FindObjectOfType<VehicleStealingSystem>() == null)
        {
            new GameObject("VehicleStealingSystem").AddComponent<VehicleStealingSystem>();
        }

        if (FindObjectOfType<TrainCollisionSystem>() == null)
        {
            new GameObject("TrainCollisionSystem").AddComponent<TrainCollisionSystem>();
        }

        if (FindObjectOfType<ZebraCrossingManager>() == null)
        {
            new GameObject("ZebraCrossingManager").AddComponent<ZebraCrossingManager>();
        }

        if (FindObjectOfType<FootbridgeSystem>() == null)
        {
            new GameObject("FootbridgeSystem").AddComponent<FootbridgeSystem>();
        }

        if (FindObjectOfType<GunShopManager>() == null)
        {
            new GameObject("GunShopManager").AddComponent<GunShopManager>();
        }

        if (FindObjectOfType<MissionGiverManager>() == null)
        {
            new GameObject("MissionGiverManager").AddComponent<MissionGiverManager>();
        }

        if (FindObjectOfType<MissionDialogueManager>() == null)
        {
            new GameObject("MissionDialogueManager").AddComponent<MissionDialogueManager>();
        }

        if (FindObjectOfType<VehicleHornManager>() == null)
        {
            new GameObject("VehicleHornManager").AddComponent<VehicleHornManager>();
        }

        if (FindObjectOfType<TrafficDriverManager>() == null)
        {
            new GameObject("TrafficDriverManager").AddComponent<TrafficDriverManager>();
        }

        if (FindObjectOfType<CannabisHouseManager>() == null)
        {
            new GameObject("CannabisHouseManager").AddComponent<CannabisHouseManager>();
        }

        if (FindObjectOfType<CannabisPurchaseManager>() == null)
        {
            new GameObject("CannabisPurchaseManager").AddComponent<CannabisPurchaseManager>();
        }

        if (FindObjectOfType<StreetLampManager>() == null)
        {
            new GameObject("StreetLampManager").AddComponent<StreetLampManager>();
        }
    }

    private void InitializePlayers()
    {
        GameMode mode = GameModeManager.Instance.GetGameMode();
        
        switch (mode)
        {
            case GameMode.SinglePlayer:
                CreatePlayer1();
                break;
            case GameMode.Coop:
                CreatePlayer1();
                CreatePlayer2();
                break;
            case GameMode.Vs:
                CreatePlayer1();
                CreatePlayer2();
                break;
            default:
                CreatePlayer1();
                break;
        }
    }

    private void CreatePlayer1()
    {
        GameObject player = GameObject.Find("Player1");
        
        Vector3 spawnPos;
        if (ChurchManager.Instance != null)
        {
            spawnPos = (Vector3)ChurchManager.Instance.GetChurchPosition() + Vector3.up * 12f;
        }
        else
        {
            spawnPos = new Vector3(300, 500, 0);
        }

        if (player == null)
        {
            player = new GameObject("Player1");
        }
        
        player.transform.position = spawnPos;
        player.tag = "Player";

        if (player.GetComponent<SpriteRenderer>() == null)
            player.AddComponent<SpriteRenderer>();
        if (player.GetComponent<CircleCollider2D>() == null)
            player.AddComponent<CircleCollider2D>();
        if (player.GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;
        }
        if (player.GetComponent<PlayerManager>() == null)
            player.AddComponent<PlayerManager>();
        if (player.GetComponent<PlayerController>() == null)
            player.AddComponent<PlayerController>();
        if (player.GetComponent<PlayerHUD>() == null)
        {
            PlayerHUD hud = player.AddComponent<PlayerHUD>();
            hud.playerId = 1;
        }

        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = Color.white;
        sr.sortingOrder = 5;
    }

    private void CreatePlayer2()
    {
        GameObject player = GameObject.Find("Player2");
        
        Vector3[] corners = new Vector3[]
        {
            new Vector3(120, 120, 0),
            new Vector3(880, 120, 0),
            new Vector3(120, 880, 0),
            new Vector3(880, 880, 0)
        };
        
        Vector3 spawnPos = corners[Random.Range(0, corners.Length)];
        
        if (player == null)
        {
            player = new GameObject("Player2");
        }
        
        player.transform.position = spawnPos;

        if (player.GetComponent<SpriteRenderer>() == null)
            player.AddComponent<SpriteRenderer>();
        if (player.GetComponent<CircleCollider2D>() == null)
            player.AddComponent<CircleCollider2D>();
        if (player.GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;
        }
        if (player.GetComponent<Player2Manager>() == null)
            player.AddComponent<Player2Manager>();
        if (player.GetComponent<Player2Controller>() == null)
            player.AddComponent<Player2Controller>();
        if (player.GetComponent<PlayerHUD>() == null)
        {
            PlayerHUD hud = player.AddComponent<PlayerHUD>();
            hud.playerId = 2;
        }

        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.3f, 0.3f, 0.9f, 1f);
        sr.sortingOrder = 5;
    }

    private void InitializeSystems()
    {
        if (FindObjectOfType<BulletPool>() == null)
            new GameObject("BulletPool").AddComponent<BulletPool>();

        if (FindObjectOfType<RocketPool>() == null)
            new GameObject("RocketPool").AddComponent<RocketPool>();

        if (FindObjectOfType<GrenadeSpawner>() == null)
            new GameObject("GrenadeSpawner").AddComponent<GrenadeSpawner>();

        if (FindObjectOfType<Grenade>() == null)
            new GameObject("GrenadeManager").AddComponent<Grenade>();

        if (FindObjectOfType<SpecialItemManager>() == null)
            new GameObject("SpecialItemManager").AddComponent<SpecialItemManager>();

        if (FindObjectOfType<SnacksManager>() == null)
            new GameObject("SnacksManager").AddComponent<SnacksManager>();

        if (FindObjectOfType<StreetDetailsManager>() == null)
            new GameObject("StreetDetailsManager").AddComponent<StreetDetailsManager>();

        if (FindObjectOfType<BuildingIconsManager>() == null)
            new GameObject("BuildingIconsManager").AddComponent<BuildingIconsManager>();

        if (FindObjectOfType<NavigationSystem>() == null)
            new GameObject("NavigationSystem").AddComponent<NavigationSystem>();

        if (FindObjectOfType<SirenManager>() == null)
            new GameObject("SirenManager").AddComponent<SirenManager>();

        if (FindObjectOfType<BuildingCollisionManager>() == null)
            new GameObject("BuildingCollisionManager").AddComponent<BuildingCollisionManager>();

        if (FindObjectOfType<TunnelManager>() == null)
            new GameObject("TunnelManager").AddComponent<TunnelManager>();

        if (FindObjectOfType<GameCameraManager>() == null)
        {
            GameObject camObj = new GameObject("GameCamera");
            camObj.AddComponent<Camera>();
            camObj.AddComponent<GameCameraManager>();
        }

        if (FindObjectOfType<OutfitShopManager>() == null)
            new GameObject("OutfitShopManager").AddComponent<OutfitShopManager>();

        if (FindObjectOfType<RestaurantManager>() == null)
            new GameObject("RestaurantManager").AddComponent<RestaurantManager>();

        if (FindObjectOfType<TaxiStandManager>() == null)
            new GameObject("TaxiStandManager").AddComponent<TaxiStandManager>();

        if (FindObjectOfType<BarberShopManager>() == null)
            new GameObject("BarberShopManager").AddComponent<BarberShopManager>();

        GameMode mode = GameModeManager.Instance.GetGameMode();
        if (mode == GameMode.Coop)
        {
            if (FindObjectOfType<CoopModeSystem>() == null)
                gameObject.AddComponent<CoopModeSystem>();
        }
        else if (mode == GameMode.Vs)
        {
            if (FindObjectOfType<VsModeSystem>() == null)
                gameObject.AddComponent<VsModeSystem>();
        }

        Debug.Log($"Game mode: {mode}");
    }
}

public class MainMenuSetup : MonoBehaviour
{
    [Header("Menu Configuration")]
    [SerializeField] private bool showVersion = true;

    private void Start()
    {
        Cursor.visible = false;
        
        if (FindObjectOfType<MainMenu>() == null)
        {
            gameObject.AddComponent<MainMenu>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Press F10 to quit (in editor, stops play mode)");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}