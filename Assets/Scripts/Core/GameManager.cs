using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private CityManager cityManager;
    [SerializeField] private MissionManager missionManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private VehicleManager vehicleManager;
    [SerializeField] private VehicleFactory vehicleFactory;
    [SerializeField] private EnvironmentManager environmentManager;
    [SerializeField] private DayNightCycleManager dayNightManager;
    [SerializeField] private WeatherSystem weatherSystem;
    [SerializeField] private RoadworksManager roadworksManager;
    [SerializeField] private PedestrianManager pedestrianManager;
    [SerializeField] private PoliceSystem policeSystem;
    [SerializeField] private CrimeDetector crimeDetector;
    [SerializeField] private MotorcycleGang motorcycleGang;
    [SerializeField] private GangMissionSystem gangMissionSystem;
    [SerializeField] private VeterinaryCentreManager veterinaryCentreManager;
    [SerializeField] private BodyArmourManager bodyArmourManager;
    [SerializeField] private GunShopManager gunShopManager;
    [SerializeField] private ScrapyardManager scrapyardManager;
    [SerializeField] private MapSystem mapSystem;

    public float Money { get; set; } = 1000f;
    public int WantedLevel { get; set; } = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (cityManager != null)
            cityManager.InitializeCity();
        if (motorcycleGang != null)
            motorcycleGang.enabled = true;
        if (missionManager != null)
            missionManager.InitializeMissions();
    }

    public void UpdateWantedLevel(int amount)
    {
        WantedLevel = Mathf.Max(0, WantedLevel + amount);
    }

    public void AddMoney(float amount)
    {
        Money += amount;
    }

    public bool CanAfford(float cost)
    {
        return Money >= cost;
    }

    public VehicleManager GetVehicleManager() => vehicleManager;
    public VehicleFactory GetVehicleFactory() => vehicleFactory;
    public EnvironmentManager GetEnvironmentManager() => environmentManager;
    public DayNightCycleManager GetDayNightManager() => dayNightManager;
    public WeatherSystem GetWeatherSystem() => weatherSystem;
    public RoadworksManager GetRoadworksManager() => roadworksManager;
    public PedestrianManager GetPedestrianManager() => pedestrianManager;
    public PoliceSystem GetPoliceSystem() => policeSystem;
    public CrimeDetector GetCrimeDetector() => crimeDetector;
    public MotorcycleGang GetMotorcycleGang() => motorcycleGang;
    public GangMissionSystem GetGangMissionSystem() => gangMissionSystem;
    public VeterinaryCentreManager GetVeterinaryCentreManager() => veterinaryCentreManager;
    public BodyArmourManager GetBodyArmourManager() => bodyArmourManager;
    public ScrapyardManager GetScrapyardManager() => scrapyardManager;
    public GunShopManager GetGunShopManager() => gunShopManager;
}
