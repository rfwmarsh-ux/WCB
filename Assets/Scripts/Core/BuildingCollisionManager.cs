using UnityEngine;
using System.Collections.Generic;

public class BuildingCollisionManager : MonoBehaviour
{
    public static BuildingCollisionManager Instance { get; private set; }

    private List<GameObject> solidBuildings = new List<GameObject>();
    private bool initialized = false;

    [Header("Collision Settings")]
    public LayerMask buildingLayer;
    public LayerMask vehicleLayer;
    public bool debugMode = false;

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
        SetupCollisionLayers();
        Invoke("InitializeBuildingCollisions", 1f);
    }

    private void SetupCollisionLayers()
    {
        int buildingLayerIndex = LayerMask.NameToLayer("Buildings");
        if (buildingLayerIndex == -1)
        {
            Debug.LogWarning("Buildings layer not found! Using Default layer.");
        }

        buildingLayer = LayerMask.GetMask("Buildings");
        vehicleLayer = LayerMask.GetMask("Vehicles");

        if (buildingLayer == 0)
        {
            buildingLayer = 1 << LayerMask.NameToLayer("Default");
        }
    }

    private void InitializeBuildingCollisions()
    {
        if (initialized) return;
        initialized = true;

        AddCollidersToExistingBuildings();
        SetupBusinessBuildingCollisions();
        SetupSpecialBuildingCollisions();

        Debug.Log($"BuildingCollisionManager initialized with {solidBuildings.Count} solid buildings");
    }

    private void AddCollidersToExistingBuildings()
    {
        MonoBehaviour[] allObjects = FindObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour obj in allObjects)
        {
            if (obj == null) continue;

            string objName = obj.gameObject.name.ToLower();

            if (objName.Contains("building") ||
                objName.Contains("shop") ||
                objName.Contains("house") ||
                objName.Contains("restaurant") ||
                objName.Contains("garage") ||
                objName.Contains("club") ||
                objName.Contains("bar") ||
                objName.Contains("supermarket") ||
                objName.Contains("police") && !objName.Contains("policeofficer") ||
                objName.Contains("hospital") && !objName.Contains("hospitalmanager") ||
                objName.Contains("station") && !objName.Contains("manager"))
            {
                AddSolidColliderToBuilding(obj.gameObject);
            }
        }
    }

    private void AddSolidColliderToBuilding(GameObject building)
    {
        if (building == null) return;
        if (solidBuildings.Contains(building)) return;

        BoxCollider2D existingCollider = building.GetComponent<BoxCollider2D>();
        CircleCollider2D existingCircleCollider = building.GetComponent<CircleCollider2D>();
        PolygonCollider2D existingPolygonCollider = building.GetComponent<PolygonCollider2D>();

        if (existingPolygonCollider != null) return;

        Vector2 buildingSize = Vector2.one * 10f;
        SpriteRenderer sr = building.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            buildingSize = new Vector2(
                building.transform.lossyScale.x * 0.8f,
                building.transform.lossyScale.y * 0.8f
            );
        }

        GameObject collisionObject = building;
        bool createdNew = false;

        if (existingCollider == null && existingCircleCollider == null)
        {
            collisionObject = new GameObject($"{building.name}_Collision");
            collisionObject.transform.position = building.transform.position;
            collisionObject.transform.rotation = building.transform.rotation;
            collisionObject.transform.parent = building.transform;
            collisionObject.layer = LayerMask.NameToLayer("Buildings");

            SpriteRenderer newSr = collisionObject.AddComponent<SpriteRenderer>();
            newSr.sprite = SpriteHelper.GetDefaultSprite();
            newSr.color = new Color(0, 0, 0, 0);

            BoxCollider2D collider = collisionObject.AddComponent<BoxCollider2D>();
            collider.size = buildingSize;
            collider.isTrigger = false;

            Rigidbody2D rb = collisionObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            createdNew = true;
        }
        else if (existingCollider != null)
        {
            building.layer = LayerMask.NameToLayer("Buildings");

            Rigidbody2D rb = building.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = building.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Static;
            }

            existingCollider.isTrigger = false;
            existingCollider.size = buildingSize;
        }
        else if (existingCircleCollider != null)
        {
            building.layer = LayerMask.NameToLayer("Buildings");

            Rigidbody2D rb = building.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = building.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Static;
            }

            existingCircleCollider.isTrigger = false;
        }

        solidBuildings.Add(building);

        if (debugMode)
        {
            Debug.Log($"Added solid collision to: {building.name}");
        }
    }

    private void SetupBusinessBuildingCollisions()
    {
        if (BusinessManager.Instance == null) return;

        var businesses = BusinessManager.Instance.GetAllBusinessObjects();
        foreach (var business in businesses)
        {
            if (business != null)
            {
                AddSolidColliderToBuilding(business);
            }
        }
    }

    private void SetupSpecialBuildingCollisions()
    {
        if (PoliceStationManager.Instance != null)
        {
            foreach (var station in PoliceStationManager.Instance.GetAllStations())
            {
                if (station != null && station.gameObject != null)
                {
                    AddSolidColliderToBuilding(station.gameObject);
                }
            }
        }

        if (AmbulanceManager.Instance != null)
        {
            foreach (var hospital in AmbulanceManager.Instance.GetHospitals())
            {
                if (hospital != null && hospital.gameObject != null)
                {
                    AddSolidColliderToBuilding(hospital.gameObject);
                }
            }
        }

        if (GunShopManager.Instance != null)
        {
            foreach (var shop in GunShopManager.Instance.GetAllGunShops())
            {
                if (shop != null)
                {
                    AddSolidColliderToBuilding(shop);
                }
            }
        }

        if (ScrapyardManager.Instance != null)
        {
            foreach (var yard in ScrapyardManager.Instance.GetAllScrapyards())
            {
                if (yard != null)
                {
                    AddSolidColliderToBuilding(yard);
                }
            }
        }

        if (VeterinaryCentreManager.Instance != null)
        {
            foreach (var centre in VeterinaryCentreManager.Instance.GetAllCentres())
            {
                if (centre != null)
                {
                    AddSolidColliderToBuilding(centre);
                }
            }
        }
    }

    public void AddBuilding(GameObject building)
    {
        if (building != null)
        {
            AddSolidColliderToBuilding(building);
        }
    }

    public List<GameObject> GetSolidBuildings()
    {
        return solidBuildings;
    }

    public bool IsBuilding(GameObject obj)
    {
        return solidBuildings.Contains(obj) || obj.layer == LayerMask.NameToLayer("Buildings");
    }

    private void OnDrawGizmos()
    {
        if (!debugMode) return;

        Gizmos.color = Color.red;
        foreach (var building in solidBuildings)
        {
            if (building != null)
            {
                Gizmos.DrawWireCube(building.transform.position,
                    new Vector3(
                        building.transform.lossyScale.x,
                        building.transform.lossyScale.y,
                        1f
                    ));
            }
        }
    }
}

public static class BusinessManagerExtensions
{
    public static List<GameObject> GetAllBusinessObjects(this BusinessManager bm)
    {
        var objects = new List<GameObject>();

        MonoBehaviour[] allObjects = FindObjectsOfType<MonoBehaviour>();
        foreach (var obj in allObjects)
        {
            if (obj == null) continue;

            string name = obj.gameObject.name.ToLower();
            if (name.Contains("shopdata") ||
                name.Contains("business"))
            {
                objects.Add(obj.gameObject);
            }
        }

        return objects;
    }
}
