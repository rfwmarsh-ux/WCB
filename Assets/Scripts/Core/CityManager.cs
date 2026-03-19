using UnityEngine;
using System.Collections.Generic;

public class CityManager : MonoBehaviour
{
    [SerializeField] private List<District> districts = new List<District>();
    [SerializeField] private List<Business> businesses = new List<Business>();
    [SerializeField] private MissionHubManager missionHubManager;
    [SerializeField] private RoadworksManager roadworksManager;
    [SerializeField] private RampManager rampManager;
    [SerializeField] private MotorcycleGang motorcycleGang;
    [SerializeField] private GangMissionSystem gangMissionSystem;
    [SerializeField] private VeterinaryCentreManager veterinaryCentreManager;
    [SerializeField] private BodyArmourManager bodyArmourManager;
    [SerializeField] private GunShopManager gunShopManager;
    [SerializeField] private ScrapyardManager scrapyardManager;

    private Dictionary<string, District> districtMap = new Dictionary<string, District>();
    private Dictionary<string, Business> businessMap = new Dictionary<string, Business>();

    public void InitializeCity()
    {
        InitializeDistricts();
        InitializeBusinesses();
        if (missionHubManager != null)
            missionHubManager.enabled = true;
        if (roadworksManager != null)
            roadworksManager.enabled = true;
        if (motorcycleGang != null)
            motorcycleGang.enabled = true;
        if (gangMissionSystem != null)
            gangMissionSystem.enabled = true;
        if (rampManager != null)
        if (veterinaryCentreManager != null)
            veterinaryCentreManager.enabled = true;
        if (bodyArmourManager != null)
        if (gunShopManager != null)
        if (scrapyardManager != null)
            scrapyardManager.enabled = true;
            gunShopManager.enabled = true;
            bodyArmourManager.enabled = true;
            rampManager.enabled = true;
    }

    private void InitializeDistricts()
    {
        // Wolverhampton-based districts
        districts.Add(new District("City Centre", new Vector2(500, 500), 200f));
        districts.Add(new District("East Side", new Vector2(800, 500), 200f));
        districts.Add(new District("West End", new Vector2(200, 500), 200f));
        districts.Add(new District("Northside", new Vector2(500, 800), 200f));
        districts.Add(new District("Southside", new Vector2(500, 200), 200f));

        foreach (var district in districts)
        {
            districtMap[district.Name] = district;
        }
    }

    private void InitializeBusinesses()
    {
        // City Centre businesses (fictional names)
        businesses.Add(new Business("Chrome Wheels Auto", "City Centre", new Vector2(480, 520), BusinessType.Garage));
        businesses.Add(new Business("Neon Nights Club", "City Centre", new Vector2(520, 480), BusinessType.Club));
        businesses.Add(new Business("Grease Pit Diner", "City Centre", new Vector2(500, 500), BusinessType.Food));
        businesses.Add(new Business("Crimson Cab Station", "City Centre", new Vector2(500, 500), BusinessType.Garage)); // Veronica's taxi rank

        // East Side businesses
        businesses.Add(new Business("Rusty's Chop Shop", "East Side", new Vector2(820, 510), BusinessType.Garage));
        businesses.Add(new Business("The Velvet Underground", "East Side", new Vector2(800, 530), BusinessType.Club));
        businesses.Add(new Business("Iron Fist Pawn", "East Side", new Vector2(780, 490), BusinessType.Shop));

        // West End businesses
        businesses.Add(new Business("Speed Demons Motors", "West End", new Vector2(220, 490), BusinessType.Garage));
        businesses.Add(new Business("The Factory", "West End", new Vector2(150, 450), BusinessType.Club)); // Rusty's dive bar
        businesses.Add(new Business("Lucky's Electronics", "West End", new Vector2(180, 480), BusinessType.Shop));

        // Northside businesses
        businesses.Add(new Business("Midnight Garage", "Northside", new Vector2(490, 820), BusinessType.Garage));
        businesses.Add(new Business("Apex Club", "Northside", new Vector2(520, 800), BusinessType.Club));

        // Southside businesses
        businesses.Add(new Business("Thunder's Garage", "Southside", new Vector2(490, 180), BusinessType.Garage));
        businesses.Add(new Business("The Diamond", "Southside", new Vector2(520, 220), BusinessType.Club));
        businesses.Add(new Business("Black Market", "Southside", new Vector2(480, 200), BusinessType.Shop));

        foreach (var business in businesses)
        {
            businessMap[business.Name] = business;
        }
    }

    public District GetDistrict(string name) => districtMap.ContainsKey(name) ? districtMap[name] : null;
    public RoadworksManager GetRoadworksManager() => roadworksManager;
    public Business GetBusiness(string name) => businessMap.ContainsKey(name) ? businessMap[name] : null;
    public List<Business> GetBusinessesInDistrict(string districtName) => businesses.FindAll(b => b.District == districtName);
    public MissionHubManager GetMissionHubManager() => missionHubManager;
    public RampManager GetRampManager() => rampManager;
    public VeterinaryCentreManager GetVeterinaryCentreManager() => veterinaryCentreManager;
    public MotorcycleGang GetMotorcycleGang() => motorcycleGang;
    public GangMissionSystem GetGangMissionSystem() => gangMissionSystem;
    public ScrapyardManager GetScrapyardManager() => scrapyardManager;
    public BodyArmourManager GetBodyArmourManager() => bodyArmourManager;
    public GunShopManager GetGunShopManager() => gunShopManager;
}

public class District
{
    public string Name { get; set; }
    public Vector2 CenterPosition { get; set; }
    public float Radius { get; set; }

    public District(string name, Vector2 centerPosition, float radius)
    {
        Name = name;
        CenterPosition = centerPosition;
        Radius = radius;
    }
}

public class Business
{
    public string Name { get; set; }
    public string District { get; set; }
    public Vector2 Position { get; set; }
    public BusinessType Type { get; set; }
    public bool IsActive { get; set; } = true;

    public Business(string name, string district, Vector2 position, BusinessType type)
    {
        Name = name;
        District = district;
        Position = position;
        Type = type;
    }
}

public enum BusinessType
{
    Garage,
    Club,
    Food,
    Shop,
    ArmsDealers,
    Safehouse
}
