using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CannabisHouseManager : MonoBehaviour
{
    public static CannabisHouseManager Instance { get; private set; }

    [Header("House Locations")]
    public Vector2[] houseLocations = new Vector2[5];
    public string[] houseNames = new string[5];

    [Header("Strain Configuration")]
    public string[] availableStrains = new string[]
    {
        "Pink Cert", "Star Dawg", "Cherry Runtz", "Lemon Cherry Gelato", "Kush Cookies",
        "Purple Punch", "GMO", "Rainbow Pie", "Apples and Bananas", "Strawberry Grail",
        "Black Cherry Punch", "Wedding Layer", "Pomegranate Shake", "Hawaiian Rain", "Chem Cookies",
        "Purple Milk", "Platinum Slurricane", "Medellin", "Orange Kush Cake", "Pink Kitty",
        "Blunicorn", "Bananconda", "Blue Pave", "Cookie Pie", "Polar Blaze",
        "Spike Mai Tai", "Lemon Diesel Fritter", "Crystal Caviar", "Grey Goose", "Pave S1",
        "Zour Apples", "Masterpiece", "Alexander The Great", "Gas Cream Cake", "Gastro Pop",
        "Vienna Grapey Grape", "Sticky Cake", "Gushface", "Gush Mitz", "Miracle Alien Cookies",
        "Jet Fuel Gelato", "GLT41", "Apes In Space", "Sage N Sour", "White Hot Guava",
        "Zoom", "Sonis Strawberry", "MJR Runtz", "Cap Junky", "Libras",
        "White Runtz", "Black Runtz", "Dantes Inferno", "Atomic Slurricane", "Watermelon Splash",
        "Pennywise", "QUE", "Chemstar", "Funky Skittlez", "Loud Cake",
        "Karamal Kandy", "Hawaiian Fanta", "High Society", "Tangy", "Tangerine Boost",
        "Gorilla Glue 4", "Pineapple Express", "Pineapple God", "Zookies", "Blackberry Balanced",
        "Lavender Cake", "Bubblegum Skittlez", "Tripoli", "C25", "Haymaker Haze",
        "Amnesia Haze", "Cheese", "Blueberry Cheese", "Triple Cheese", "Mayvar Cheese",
        "Kush Mints", "Fire Cookies", "Cookie Casket", "Death Bubba", "Kings Kush",
        "Skullcap", "Strawberry Sundae", "OMG Burger", "Sun County Kush", "White Haze",
        "Purple Haze", "Candy Kush", "Blueberry Yum Yum", "Blueberry Cookies", "Super Lemon Haze",
        "Crumbled Lime", "Christmas Cake", "Wedding Pop", "Citroli", "Wedding Sunset",
        "Peach Rizz", "Coast Sour Dank", "Sourz", "Khalifa Mintz", "Grape Pie",
        "Moby Dick", "Blackberry Sour", "Blue Dream", "IPST20", "Candy Store",
        "Glitter Bomb", "Pineapple Grape", "Blueberry Fuego", "Berry Cream Puff", "Alien Jelly",
        "First Class Funk", "Long Valley Legend", "SuperBoof", "Red Pop", "Gaschata",
        "Black Candy", "Pink Cookies", "Banana Daddy", "Durban Kush", "Gelatti",
        "Korean BBQ"
    };

    [Header("Pricing")]
    public float[] prices = new float[] { 35f, 60f, 100f, 180f };
    public string[] quantityNames = new string[] { "Henry", "Q", "Halfer", "Ozzy" };
    public float[] durationMinutes = new float[] { 1f, 2f, 5f, 10f };
    public float healthBoostAmount = 50f;

    [Header("Auto-Refresh")]
    public float refreshIntervalMinutes = 18f;
    private float refreshTimer;
    private const float OUT_OF_STOCK_CHANCE = 0.08f;

    [Header("Houses")]
    private List<CannabisHouse> houses = new List<CannabisHouse>();
    private Dictionary<int, string[]> houseStrains = new Dictionary<int, string[]>();
    private Dictionary<int, bool[]> strainAvailable = new Dictionary<int, bool[]>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeHouses();
        refreshTimer = refreshIntervalMinutes * 60f;
    }

    private void Update()
    {
        if (refreshTimer > 0)
        {
            refreshTimer -= Time.deltaTime;
            if (refreshTimer <= 0)
            {
                RefreshAllStrains();
                refreshTimer = refreshIntervalMinutes * 60f;
            }
        }
    }

    private void InitializeHouses()
    {
        houseLocations = new Vector2[]
        {
            new Vector2(350f, 600f),
            new Vector2(450f, 700f),
            new Vector2(550f, 650f),
            new Vector2(650f, 550f),
            new Vector2(750f, 600f)
        };

        houseNames = new string[]
        {
            "The Green House",
            "Purple Palace",
            "Smoke Shack",
            "The Dispensary",
            "Herbal Haven"
        };

        for (int i = 0; i < 5; i++)
        {
            CreateHouse(i, houseNames[i], houseLocations[i]);
            houseStrains[i] = GetRandomStrains(3);
        }

        Debug.Log($"Created {houses.Count} cannabis houses");
    }

    private void CreateHouse(int index, string name, Vector2 position)
    {
        GameObject houseGO = new GameObject($"CannabisHouse_{name}");
        houseGO.transform.position = (Vector3)position;

        SpriteRenderer sr = houseGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.3f, 0.5f, 0.3f, 0.9f);
        sr.sortingOrder = 2;
        houseGO.transform.localScale = new Vector3(25f, 20f, 1f);

        CircleCollider2D collider = houseGO.AddComponent<CircleCollider2D>();
        collider.radius = 12f;
        collider.isTrigger = true;

        CannabisHouse house = houseGO.AddComponent<CannabisHouse>();
        house.Initialize(index, name, position);

        houses.Add(house);
    }

    private void RefreshAllStrains()
    {
        for (int i = 0; i < houses.Count; i++)
        {
            RefreshStrains(i);
        }
        Debug.Log("All cannabis house strains refreshed!");
    }

    private string[] GetRandomStrains(int count)
    {
        return availableStrains.OrderBy(x => Random.value).Take(count).ToArray();
    }

    public List<CannabisHouse> GetAllHouses()
    {
        return houses;
    }

    public CannabisHouse GetNearestHouse(Vector2 position)
    {
        if (houses.Count == 0) return null;

        CannabisHouse nearest = houses[0];
        float minDist = Vector2.Distance(position, nearest.Position);

        foreach (var house in houses)
        {
            float dist = Vector2.Distance(position, house.Position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = house;
            }
        }

        return nearest;
    }

    public string[] GetStrainsForHouse(int houseIndex)
    {
        return houseStrains.ContainsKey(houseIndex) ? houseStrains[houseIndex] : new string[0];
    }

    public void RefreshStrains(int houseIndex)
    {
        string[] newStrains = GetRandomStrains(3);
        bool[] availability = new bool[3];
        
        for (int i = 0; i < 3; i++)
        {
            availability[i] = Random.value > OUT_OF_STOCK_CHANCE;
        }
        
        houseStrains[houseIndex] = newStrains;
        strainAvailable[houseIndex] = availability;
    }

    public bool IsStrainAvailable(int houseIndex, int strainIndex)
    {
        if (!strainAvailable.ContainsKey(houseIndex)) return true;
        if (strainIndex >= strainAvailable[houseIndex].Length) return true;
        return strainAvailable[houseIndex][strainIndex];
    }
}

public class CannabisHouse : MonoBehaviour
{
    public int houseIndex;
    public string houseName;
    public Vector2 Position { get; private set; }

    public void Initialize(int index, string name, Vector2 position)
    {
        houseIndex = index;
        houseName = name;
        Position = position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Player2"))
        {
            OpenPurchaseMenu();
        }
    }

    private void OpenPurchaseMenu()
    {
        if (InGameMenuManager.Instance != null)
        {
            InGameMenuManager.Instance.OpenCannabisMenu(houseIndex, houseName);
        }
    }
}

public class CannabisEffect : MonoBehaviour
{
    public int playerId;
    public float healthBoost;
    public float duration;
    public float endTime;
    public bool isActive;

    private GameObject smokeCloud;
    private Transform playerTransform;
    private Transform vehicleTransform;
    private bool isInVehicle;

    public void Activate(int playerId, float boost, float durationMinutes)
    {
        this.playerId = playerId;
        healthBoost = boost;
        duration = durationMinutes * 60f;
        endTime = Time.time + duration;
        isActive = true;

        PlayerHUD hud = playerId == 1 ? PlayerHUD.player1HUD : PlayerHUD.player2HUD;
        if (hud != null)
        {
            hud.maxHealth += (int)healthBoost;
        }

        CreateSmokeCloud();
    }

    private void CreateSmokeCloud()
    {
        smokeCloud = new GameObject($"SmokeCloud_P{playerId}");
        smokeCloud.transform.parent = transform;
        
        for (int i = 0; i < 8; i++)
        {
            GameObject puff = new GameObject($"Puff_{i}");
            puff.transform.parent = smokeCloud.transform;
            
            SpriteRenderer sr = puff.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.sortingOrder = 50;
            sr.color = new Color(0.8f, 0.8f, 0.8f, 0.25f);
            
            float angle = i * 45f * Mathf.Deg2Rad;
            float radius = Random.Range(2f, 4f);
            puff.transform.localPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            puff.transform.localScale = new Vector3(Random.Range(3f, 6f), Random.Range(3f, 6f), 1f);
        }

        smokeCloud.SetActive(false);
    }

    private void Update()
    {
        if (!isActive) return;

        if (Time.time >= endTime)
        {
            Deactivate();
            return;
        }

        UpdateSmokePosition();
    }

    private void UpdateSmokePosition()
    {
        if (smokeCloud == null) return;

        PlayerManager p1 = PlayerManager.Instance;
        Player2Manager p2 = Player2Manager.Instance;
        
        Transform target = null;
        isInVehicle = false;

        if (playerId == 1 && p1 != null)
        {
            if (p1.IsInVehicle)
            {
                isInVehicle = true;
                target = p1.CurrentVehicle?.transform;
            }
            else
            {
                target = p1.transform;
            }
        }
        else if (playerId == 2 && p2 != null)
        {
            if (p2.IsInVehicle)
            {
                isInVehicle = true;
                target = p2.CurrentVehicle?.transform;
            }
            else
            {
                target = p2.transform;
            }
        }

        if (target != null)
        {
            smokeCloud.SetActive(true);

            if (isInVehicle)
            {
                smokeCloud.transform.position = target.position + Vector3.back * 2f;
                
                float[] windowOffsets = new float[] { -3f, 3f, -2f };
                for (int i = 0; i < smokeCloud.transform.childCount && i < windowOffsets.Length; i++)
                {
                    Transform puff = smokeCloud.transform.GetChild(i);
                    puff.localPosition = new Vector3(
                        windowOffsets[i] + Random.Range(-0.5f, 0.5f),
                        Random.Range(-1f, 1f),
                        0
                    );
                }
            }
            else
            {
                smokeCloud.transform.position = target.position + Vector3.back * 2f + Vector3.up * 2f;
                
                for (int i = 0; i < smokeCloud.transform.childCount; i++)
                {
                    Transform puff = smokeCloud.transform.GetChild(i);
                    float t = Time.time * 0.5f + i * 0.5f;
                    puff.localPosition += new Vector3(
                        Mathf.Sin(t) * 0.02f,
                        0.02f,
                        0
                    );
                }
            }
        }
    }

    private void Deactivate()
    {
        isActive = false;

        PlayerHUD hud = playerId == 1 ? PlayerHUD.player1HUD : PlayerHUD.player2HUD;
        if (hud != null)
        {
            hud.maxHealth = 100f;
        }

        if (smokeCloud != null)
        {
            Destroy(smokeCloud);
        }

        Destroy(this);
    }
}

public class CannabisPurchaseManager : MonoBehaviour
{
    public static CannabisPurchaseManager Instance { get; private set; }

    private int currentHouseIndex;
    private string currentHouseName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OpenMenu(int houseIndex, string houseName)
    {
        currentHouseIndex = houseIndex;
        currentHouseName = houseName;
    }

    public void Purchase(int strainIndex, int quantityIndex)
    {
        string[] strains = CannabisHouseManager.Instance.GetStrainsForHouse(currentHouseIndex);
        if (strainIndex >= strains.Length) return;

        string strain = strains[strainIndex];
        float price = CannabisHouseManager.Instance.prices[quantityIndex];
        string quantity = CannabisHouseManager.Instance.quantityNames[quantityIndex];
        float duration = CannabisHouseManager.Instance.durationMinutes[quantityIndex];
        float boost = CannabisHouseManager.Instance.healthBoostAmount;

        GameManager gm = GameManager.Instance;
        if (gm.Money >= price)
        {
            gm.Money -= price;

            int playerId = 1;
            if (PlayerManager.Instance != null)
            {
                PlayerManager pm = PlayerManager.Instance;
                if (pm.IsInVehicle)
                {
                    pm = Player2Manager.Instance;
                    if (pm != null) playerId = 2;
                }
            }

            CannabisEffect effect = new GameObject("CannabisEffect").AddComponent<CannabisEffect>();
            if (playerId == 1)
                effect.transform.parent = PlayerManager.Instance?.transform;
            else
                effect.transform.parent = Player2Manager.Instance?.transform;

            effect.Activate(playerId, boost, duration);

            Debug.Log($"Purchased {quantity} of {strain} for ${price}! Health boosted by {boost} for {duration} minutes.");
        }
    }
}
