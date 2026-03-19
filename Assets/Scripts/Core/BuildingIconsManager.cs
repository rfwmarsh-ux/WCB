using UnityEngine;
using System.Collections.Generic;

public enum BuildingIconType
{
    GunShop,
    Scrapyard,
    Hospital,
    Police,
    Church,
    Clothes,
    Barber,
    Restaurant,
    Taxi,
    Bus,
    Train,
    Mission,
    Navigation
}

public class BuildingIconData
{
    public GameObject iconObject;
    public BuildingIconType iconType;
    public Vector2 worldPosition;
    public string buildingName;
    public MonoBehaviour buildingComponent;
}

public class BuildingIconsManager : MonoBehaviour
{
    public static BuildingIconsManager Instance { get; private set; }

    private List<BuildingIconData> buildingIcons = new List<BuildingIconData>();
    private float iconHeight = 15f;
    private float iconBobSpeed = 2f;
    private float iconBobHeight = 1f;

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
        CreateAllBuildingIcons();
    }

    private void Update()
    {
        UpdateIconAnimation();
    }

    private void CreateAllBuildingIcons()
    {
        CreateGunShopIcons();
        CreateScrapyardIcons();
        CreateHospitalIcons();
        CreatePoliceIcons();
        CreateChurchIcons();
        CreateClothesIcons();
        CreateBarberIcons();
        CreateRestaurantIcons();
        CreateTaxiIcons();
        CreateBusIcons();
        CreateTrainIcons();

        Debug.Log($"Created {buildingIcons.Count} building icons");
    }

    private void CreateGunShopIcons()
    {
        if (GunShopManager.Instance != null)
        {
            foreach (var shop in GunShopManager.Instance.GetAllGunShops())
            {
                CreateBuildingIcon(shop.transform.position, BuildingIconType.GunShop, "Gun Shop", shop);
            }
        }
    }

    private void CreateScrapyardIcons()
    {
        if (ScrapyardManager.Instance != null)
        {
            foreach (var yard in ScrapyardManager.Instance.GetAllScrapyards())
            {
                CreateBuildingIcon(yard.transform.position, BuildingIconType.Scrapyard, "Scrapyard", yard);
            }
        }
    }

    private void CreateHospitalIcons()
    {
        if (VeterinaryCentreManager.Instance != null)
        {
            foreach (var centre in VeterinaryCentreManager.Instance.GetAllCentres())
            {
                CreateBuildingIcon(centre.transform.position, BuildingIconType.Hospital, "Hospital", centre);
            }
        }

        if (AmbulanceManager.Instance != null)
        {
            foreach (var hospital in AmbulanceManager.Instance.GetHospitals())
            {
                CreateBuildingIcon(hospital.Position, BuildingIconType.Hospital, hospital.HospitalName, hospital);
            }
        }
    }

    private void CreatePoliceIcons()
    {
        if (PoliceStationManager.Instance != null)
        {
            foreach (var station in PoliceStationManager.Instance.GetAllStations())
            {
                CreateBuildingIcon(station.Position, BuildingIconType.Police, station.StationName, station);
            }
        }
    }

    private void CreateChurchIcons()
    {
        if (ChurchManager.Instance != null)
        {
            CreateBuildingIcon(ChurchManager.Instance.GetChurchPosition(), BuildingIconType.Church, "Church", ChurchManager.Instance);
        }
    }

    private void CreateClothesIcons()
    {
        if (OutfitShopManager.Instance != null)
        {
            foreach (var shop in OutfitShopManager.Instance.GetAllShops())
            {
                CreateBuildingIcon(shop.transform.position, BuildingIconType.Clothes, "Clothes Shop", shop);
            }
        }
        else
        {
            GameObject[] clothesShops = GameObject.FindGameObjectsWithTag("ClothesShop");
            foreach (var shop in clothesShops)
            {
                CreateBuildingIcon(shop.transform.position, BuildingIconType.Clothes, "Clothes Shop", shop.GetComponent<OutfitShop>());
            }
        }
    }

    private void CreateBarberIcons()
    {
        GameObject[] barbers = GameObject.FindGameObjectsWithTag("BarberShop");
        foreach (var barber in barbers)
        {
            CreateBuildingIcon(barber.transform.position, BuildingIconType.Barber, "Barber", barber.GetComponent<BarberShop>());
        }
    }

    private void CreateRestaurantIcons()
    {
        if (RestaurantManager.Instance != null)
        {
            foreach (var restaurant in RestaurantManager.Instance.GetAllRestaurants())
            {
                CreateBuildingIcon(restaurant.transform.position, BuildingIconType.Restaurant, "Restaurant", restaurant.GetComponent<MonoBehaviour>());
            }
        }
        else
        {
            GameObject[] restaurants = GameObject.FindGameObjectsWithTag("Restaurant");
            foreach (var restaurant in restaurants)
            {
                CreateBuildingIcon(restaurant.transform.position, BuildingIconType.Restaurant, "Restaurant", restaurant.GetComponent<MonoBehaviour>());
            }
        }
    }

    private void CreateTaxiIcons()
    {
        GameObject[] taxis = GameObject.FindGameObjectsWithTag("TaxiStand");
        foreach (var taxi in taxis)
        {
            CreateBuildingIcon(taxi.transform.position, BuildingIconType.Taxi, "Taxi Stand", taxi.GetComponent<TaxiStand>());
        }
    }

    private void CreateBusIcons()
    {
        if (BusManager.Instance != null)
        {
            foreach (var stop in BusManager.Instance.GetAllStops())
            {
                CreateBuildingIcon(stop.transform.position, BuildingIconType.Bus, "Bus Stop", stop);
            }
        }
    }

    private void CreateTrainIcons()
    {
        if (TrainManager.Instance != null)
        {
            foreach (var station in TrainManager.Instance.GetAllStations())
            {
                CreateBuildingIcon(station.transform.position, BuildingIconType.Train, "Train Station", station);
            }
        }
        if (MetroManager.Instance != null)
        {
            foreach (var station in MetroManager.Instance.GetAllStations())
            {
                CreateBuildingIcon(station.transform.position, BuildingIconType.Train, "Metro Station", station);
            }
        }
    }

    private void CreateBuildingIcon(Vector2 position, BuildingIconType type, string name, MonoBehaviour component)
    {
        GameObject iconObj = new GameObject($"Icon_{name}");
        iconObj.transform.position = (Vector3)position + Vector3.up * iconHeight;

        SpriteRenderer sr = iconObj.AddComponent<SpriteRenderer>();
        sr.sprite = GetIconSprite(type);
        sr.sortingOrder = 15;

        CircleCollider2D collider = iconObj.AddComponent<CircleCollider2D>();
        collider.radius = 3f;
        collider.isTrigger = true;

        BuildingIcon icon = iconObj.AddComponent<BuildingIcon>();
        icon.Initialize(type, name, position, component);

        iconObj.transform.parent = transform;

        buildingIcons.Add(new BuildingIconData
        {
            iconObject = iconObj,
            iconType = type,
            worldPosition = position,
            buildingName = name,
            buildingComponent = component
        });
    }

    private Sprite GetIconSprite(BuildingIconType type)
    {
        string key = type switch
        {
            BuildingIconType.GunShop => "icon_gunshop",
            BuildingIconType.Scrapyard => "icon_scrapyard",
            BuildingIconType.Hospital => "icon_hospital",
            BuildingIconType.Police => "icon_police",
            BuildingIconType.Church => "icon_church",
            BuildingIconType.Clothes => "icon_clothes",
            BuildingIconType.Barber => "icon_barber",
            BuildingIconType.Restaurant => "icon_restaurant",
            BuildingIconType.Taxi => "icon_taxi",
            BuildingIconType.Bus => "icon_bus",
            BuildingIconType.Train => "icon_train",
            BuildingIconType.Mission => "icon_mission",
            BuildingIconType.Navigation => "icon_navigation",
            _ => "icon_gunshop"
        };
        return GTASpriteGenerator.GetSprite(key);
    }

    private void UpdateIconAnimation()
    {
        float bobOffset = Mathf.Sin(Time.time * iconBobSpeed) * iconBobHeight;
        foreach (var icon in buildingIcons)
        {
            if (icon.iconObject != null)
            {
                Vector3 pos = icon.iconObject.transform.position;
                icon.iconObject.transform.position = new Vector3(pos.x, pos.y + bobOffset * Time.deltaTime, pos.z);
            }
        }
    }

    public List<BuildingIconData> GetAllIcons()
    {
        return buildingIcons;
    }

    public List<BuildingIconData> GetIconsOfType(BuildingIconType type)
    {
        return buildingIcons.FindAll(i => i.iconType == type);
    }

    public BuildingIconData GetNearestIcon(Vector2 position, float maxDistance = float.MaxValue)
    {
        BuildingIconData nearest = null;
        float minDist = maxDistance;

        foreach (var icon in buildingIcons)
        {
            float dist = Vector2.Distance(position, icon.worldPosition);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = icon;
            }
        }

        return nearest;
    }
}

public class BuildingIcon : MonoBehaviour
{
    private BuildingIconType iconType;
    private string buildingName;
    private Vector2 worldPosition;
    private MonoBehaviour buildingComponent;
    private bool isSelected = false;
    private Color originalColor;
    private Color selectedColor = new Color(1f, 1f, 0.2f, 1f);

    public void Initialize(BuildingIconType type, string name, Vector2 position, MonoBehaviour component)
    {
        iconType = type;
        buildingName = name;
        worldPosition = position;
        buildingComponent = component;
        originalColor = GetComponent<SpriteRenderer>().color;
    }

    private void OnMouseDown()
    {
        SelectBuilding();
    }

    private void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().color = selectedColor;
    }

    private void OnMouseExit()
    {
        if (!isSelected)
        {
            GetComponent<SpriteRenderer>().color = originalColor;
        }
    }

    private void SelectBuilding()
    {
        isSelected = true;
        GetComponent<SpriteRenderer>().color = selectedColor;

        Debug.Log($"Selected building: {buildingName} at {worldPosition}");

        NavigationSystem.Instance?.SetNavigationTarget(worldPosition, buildingName, iconType);

        GetComponent<CircleCollider2D>().radius = 5f;
        Invoke("ResetSelection", 0.5f);
    }

    private void ResetSelection()
    {
        isSelected = false;
        GetComponent<SpriteRenderer>().color = originalColor;
        GetComponent<CircleCollider2D>().radius = 3f;
    }

    public BuildingIconType GetIconType() => iconType;
    public string GetBuildingName() => buildingName;
    public Vector2 GetWorldPosition() => worldPosition;
}
