using UnityEngine;
using System.Collections.Generic;

public enum WaterType
{
    Canal,
    River,
    Pond,
    Lake,
    Reservoir
}

public class WaterManager : MonoBehaviour
{
    public static WaterManager Instance { get; private set; }

    private List<WaterBody> waterBodies = new List<WaterBody>();

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
        CreateAllWaterBodies();
    }

    private void CreateAllWaterBodies()
    {
        CreateCanals();
        CreateRivers();
        CreatePondsAndLakes();

        Debug.Log($"Created {waterBodies.Count} water bodies");
    }

    private void CreateCanals()
    {
        CreateWaterBody("Staffordshire Worcestershire Canal", new Vector2(350, 360), WaterType.Canal, 180f, 15f);
        CreateWaterBody("Birmingham Canal", new Vector2(320, 340), WaterType.Canal, 100f, 12f);
        CreateWaterBody("Wyrley Essington Canal", new Vector2(280, 370), WaterType.Canal, 80f, 10f);
        CreateWaterBody("Bentley Canal", new Vector2(620, 380), WaterType.Canal, 60f, 12f);
    }

    private void CreateRivers()
    {
        CreateWaterBody("River Smestow", new Vector2(380, 350), WaterType.River, 150f, 18f);
        CreateWaterBody("River Boulton", new Vector2(450, 420), WaterType.River, 40f, 10f);
        CreateWaterBody("River Penk", new Vector2(280, 300), WaterType.River, 60f, 12f);
    }

    private void CreatePondsAndLakes()
    {
        CreateWaterBody("West Park Lake", new Vector2(400, 560), WaterType.Lake, 25f, 20f);
        CreateWaterBody("East Park Pond", new Vector2(600, 490), WaterType.Pond, 18f, 15f);
        CreateWaterBody("Bantock Park Pond", new Vector2(350, 610), WaterType.Pond, 20f, 16f);
        CreateWaterBody("St George's Lake", new Vector2(520, 465), WaterType.Lake, 22f, 18f);
        CreateWaterBody("Graiseley Pond", new Vector2(560, 570), WaterType.Pond, 15f, 12f);
        CreateWaterBody("Castleford Lake", new Vector2(260, 590), WaterType.Lake, 18f, 15f);
        CreateWaterBody("Finchfield Pond", new Vector2(310, 540), WaterType.Pond, 14f, 12f);
        CreateWaterBody("Tettenhall Wood Pond", new Vector2(650, 750), WaterType.Pond, 16f, 14f);
        CreateWaterBody("Compton Lake", new Vector2(710, 760), WaterType.Lake, 25f, 20f);
        CreateWaterBody("Wergs Reservoir", new Vector2(690, 730), WaterType.Reservoir, 30f, 25f);
        CreateWaterBody("Ashmore Park Pool", new Vector2(280, 460), WaterType.Pond, 20f, 18f);
        CreateWaterBody("Bradmore Pool", new Vector2(710, 570), WaterType.Pond, 18f, 15f);
        CreateWaterBody("Low Hill Pond", new Vector2(410, 660), WaterType.Pond, 15f, 12f);
        CreateWaterBody("Heath Town Pool", new Vector2(550, 755), WaterType.Pond, 14f, 12f);
        CreateWaterBody("Pendeford Mill Pond", new Vector2(780, 630), WaterType.Pond, 22f, 18f);
        CreateWaterBody("Bushbury Pond", new Vector2(195, 650), WaterType.Pond, 16f, 14f);
        CreateWaterBody("Horseley Fields Pond", new Vector2(310, 390), WaterType.Pond, 18f, 15f);
        CreateWaterBody("Dunstall Hill Pond", new Vector2(400, 400), WaterType.Pond, 14f, 12f);
        CreateWaterBody("Goldthorn Pond", new Vector2(330, 410), WaterType.Pond, 12f, 10f);
        CreateWaterBody("Merridale Pond", new Vector2(480, 440), WaterType.Pond, 16f, 14f);
    }

    private void CreateWaterBody(string name, Vector2 position, WaterType type, float width, float height)
    {
        GameObject waterGO = new GameObject($"Water_{name}");
        waterGO.transform.position = (Vector3)position;

        WaterBody water = waterGO.AddComponent<WaterBody>();
        water.Initialize(name, position, type, width, height);

        waterGO.transform.parent = transform;
        waterBodies.Add(water);
    }

    public bool IsOverWater(Vector2 position, out WaterBody water)
    {
        water = null;
        foreach (var wb in waterBodies)
        {
            if (wb.IsPositionInWater(position))
            {
                water = wb;
                return true;
            }
        }
        return false;
    }

    public bool IsOverCanal(Vector2 position)
    {
        foreach (var wb in waterBodies)
        {
            if (wb.WaterType == WaterType.Canal && wb.IsPositionInWater(position))
            {
                return true;
            }
        }
        return false;
    }

    public List<WaterBody> GetAllWaterBodies() => waterBodies;
}

public class WaterBody : MonoBehaviour
{
    public string WaterName { get; private set; }
    public Vector2 Position { get; private set; }
    public WaterType WaterType { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }

    private SpriteRenderer spriteRenderer;

    public void Initialize(string name, Vector2 position, WaterType type, float width, float height)
    {
        WaterName = name;
        Position = position;
        WaterType = type;
        Width = width;
        Height = height;

        CreateWaterVisuals();
    }

    private void CreateWaterVisuals()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
        spriteRenderer.sortingOrder = -8;

        transform.localScale = new Vector3(Width, Height, 1f);

        Color waterColor = GetWaterColor();
        spriteRenderer.color = waterColor;

        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = Mathf.Max(Width, Height) / 2f;
        collider.isTrigger = true;
    }

    private Color GetWaterColor()
    {
        return WaterType switch
        {
            WaterType.Canal => new Color(0.2f, 0.4f, 0.5f, 0.85f),
            WaterType.River => new Color(0.15f, 0.35f, 0.45f, 0.9f),
            WaterType.Pond => new Color(0.25f, 0.45f, 0.35f, 0.8f),
            WaterType.Lake => new Color(0.2f, 0.4f, 0.4f, 0.85f),
            WaterType.Reservoir => new Color(0.18f, 0.38f, 0.42f, 0.85f),
            _ => new Color(0.2f, 0.4f, 0.4f, 0.8f)
        };
    }

    public bool IsPositionInWater(Vector2 position)
    {
        float halfWidth = Width / 2f;
        float halfHeight = Height / 2f;

        return position.x >= Position.x - halfWidth && position.x <= Position.x + halfWidth &&
               position.y >= Position.y - halfHeight && position.y <= Position.y + halfHeight;
    }

    public bool IsSafeCanal()
    {
        return WaterType == WaterType.Canal;
    }
}
