using UnityEngine;
using System.Collections.Generic;

public enum BridgeType
{
    RailwayViaduct,
    RoadBridge,
    Footbridge,
    CanalBridge
}

public class BridgeManager : MonoBehaviour
{
    public static BridgeManager Instance { get; private set; }

    private List<Bridge> bridges = new List<Bridge>();

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
        CreateAllBridges();
    }

    private void CreateAllBridges()
    {
        CreateRailwayViaducts();
        CreateRoadBridges();
        CreateFootbridges();
        CreateCanalBridges();

        Debug.Log($"Created {bridges.Count} bridges");
    }

    private void CreateRailwayViaducts()
    {
        CreateBridge("Oxley Viaduct", new Vector2(380f, 420f), BridgeType.RailwayViaduct, 120f, true, 15f);
        CreateBridge("Dunstall Hill Viaduct", new Vector2(420f, 380f), BridgeType.RailwayViaduct, 80f, true, 12f);
        CreateBridge("Goldthorn Hill Viaduct", new Vector2(300f, 380f), BridgeType.RailwayViaduct, 60f, true, 10f);
        CreateBridge("Aldersley Railway Bridge", new Vector2(180f, 420f), BridgeType.RailwayViaduct, 50f, true, 8f);
    }

    private void CreateRoadBridges()
    {
        CreateBridge("Ring Road St David's Bridge", new Vector2(480f, 540f), BridgeType.RoadBridge, 40f, false, 0f);
        CreateBridge("Railway Drive Bridge", new Vector2(490f, 530f), BridgeType.RoadBridge, 35f, false, 0f);
        CreateBridge("Wolverhampton Road West Bridge", new Vector2(350f, 320f), BridgeType.RoadBridge, 45f, false, 0f);
        CreateBridge("Bilston Road Bridge", new Vector2(300f, 400f), BridgeType.RoadBridge, 40f, false, 0f);
        CreateBridge("Dudley Road Bridge", new Vector2(350f, 350f), BridgeType.RoadBridge, 35f, false, 0f);
        CreateBridge("Wednesfield Road Bridge", new Vector2(680f, 380f), BridgeType.RoadBridge, 40f, false, 0f);
        CreateBridge("Tettenhall Road Bridge", new Vector2(600f, 680f), BridgeType.RoadBridge, 35f, false, 0f);
    }

    private void CreateFootbridges()
    {
        CreateBridge("Wolverhampton Station Footbridge", new Vector2(470f, 545f), BridgeType.Footbridge, 30f, true, 10f);
        CreateBridge("City Centre Footbridge", new Vector2(490f, 510f), BridgeType.Footbridge, 25f, true, 8f);
        CreateBridge("Ring Road Footbridge", new Vector2(520f, 500f), BridgeType.Footbridge, 35f, true, 9f);
        CreateBridge("Bus Station Footbridge", new Vector2(485f, 515f), BridgeType.Footbridge, 28f, true, 8f);
    }

    private void CreateCanalBridges()
    {
        CreateBridge("Bentley Canal Footbridge", new Vector2(650f, 380f), BridgeType.CanalBridge, 20f, true, 6f);
        CreateBridge("Bentley Canal Road Bridge", new Vector2(620f, 360f), BridgeType.CanalBridge, 25f, false, 0f);
        CreateBridge("Wyrley Canal Bridge", new Vector2(280f, 380f), BridgeType.CanalBridge, 22f, true, 5f);
        CreateBridge("Shropshire Canal Bridge", new Vector2(200f, 350f), BridgeType.CanalBridge, 20f, true, 5f);
    }

    private void CreateBridge(string name, Vector2 position, BridgeType type, float length, bool isElevated, float clearanceHeight)
    {
        GameObject bridgeGO = new GameObject($"Bridge_{name}");
        bridgeGO.transform.position = (Vector3)position;
        bridgeGO.tag = "Bridge";

        Bridge bridge = bridgeGO.AddComponent<Bridge>();
        bridge.Initialize(name, position, type, length, isElevated, clearanceHeight);

        bridgeGO.transform.parent = transform;
        bridges.Add(bridge);
    }

    public List<Bridge> GetAllBridges() => bridges;
    public List<Bridge> GetBridgesOfType(BridgeType type)
    {
        List<Bridge> result = new List<Bridge>();
        foreach (var bridge in bridges)
        {
            if (bridge.BridgeType == type)
                result.Add(bridge);
        }
        return result;
    }

    public Bridge GetClosestBridge(Vector2 position)
    {
        if (bridges.Count == 0) return null;

        Bridge closest = bridges[0];
        float minDist = Vector2.Distance(position, closest.Position);

        foreach (var bridge in bridges)
        {
            float dist = Vector2.Distance(position, bridge.Position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = bridge;
            }
        }
        return closest;
    }
}

public class BridgeManager : MonoBehaviour
{
    public static BridgeManager Instance { get; private set; }

    private List<Bridge> bridges = new List<Bridge>();

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
        CreateAllBridges();
    }

    private void CreateAllBridges()
    {
        CreateRailwayViaducts();
        CreateRoadBridges();
        CreateFootbridges();
        CreateCanalBridges();

        Debug.Log($"Created {bridges.Count} bridges");
    }

    private void CreateRailwayViaducts()
    {
        CreateBridge("Oxley Viaduct", new Vector2(380f, 420f), BridgeType.RailwayViaduct, 120f, true);
        CreateBridge("Dunstall Hill Viaduct", new Vector2(420f, 380f), BridgeType.RailwayViaduct, 80f, true);
        CreateBridge("Goldthorn Hill Viaduct", new Vector2(300f, 380f), BridgeType.RailwayViaduct, 60f, true);
        CreateBridge("Aldersley Railway Bridge", new Vector2(180f, 420f), BridgeType.RailwayViaduct, 50f, true);
    }

    private void CreateRoadBridges()
    {
        CreateBridge("Ring Road St David's Bridge", new Vector2(480f, 540f), BridgeType.RoadBridge, 40f, false);
        CreateBridge("Railway Drive Bridge", new Vector2(490f, 530f), BridgeType.RoadBridge, 35f, false);
        CreateBridge("Wolverhampton Road West Bridge", new Vector2(350f, 320f), BridgeType.RoadBridge, 45f, false);
        CreateBridge("Bilston Road Bridge", new Vector2(300f, 400f), BridgeType.RoadBridge, 40f, false);
        CreateBridge("Dudley Road Bridge", new Vector2(350f, 350f), BridgeType.RoadBridge, 35f, false);
        CreateBridge("Wednesfield Road Bridge", new Vector2(680f, 380f), BridgeType.RoadBridge, 40f, false);
        CreateBridge("Tettenhall Road Bridge", new Vector2(600f, 680f), BridgeType.RoadBridge, 35f, false);
    }

    private void CreateFootbridges()
    {
        CreateBridge("Wolverhampton Station Footbridge", new Vector2(470f, 545f), BridgeType.Footbridge, 30f, false);
        CreateBridge("City Centre Footbridge", new Vector2(490f, 510f), BridgeType.Footbridge, 25f, false);
        CreateBridge("Ring Road Footbridge", new Vector2(520f, 500f), BridgeType.Footbridge, 35f, false);
        CreateBridge("Bus Station Footbridge", new Vector2(485f, 515f), BridgeType.Footbridge, 28f, false);
    }

    private void CreateCanalBridges()
    {
        CreateBridge("Bentley Canal Footbridge", new Vector2(650f, 380f), BridgeType.CanalBridge, 20f, false);
        CreateBridge("Bentley Canal Road Bridge", new Vector2(620f, 360f), BridgeType.CanalBridge, 25f, false);
        CreateBridge("Wyrley Canal Bridge", new Vector2(280f, 380f), BridgeType.CanalBridge, 22f, false);
        CreateBridge("Shropshire Canal Bridge", new Vector2(200f, 350f), BridgeType.CanalBridge, 20f, false);
    }

    private void CreateBridge(string name, Vector2 position, BridgeType type, float length, bool isElevated)
    {
        GameObject bridgeGO = new GameObject($"Bridge_{name}");
        bridgeGO.transform.position = (Vector3)position;
        bridgeGO.tag = "Bridge";

        Bridge bridge = bridgeGO.AddComponent<Bridge>();
        bridge.Initialize(name, position, type, length, isElevated);

        bridgeGO.transform.parent = transform;
        bridges.Add(bridge);
    }

    public List<Bridge> GetAllBridges() => bridges;
    public List<Bridge> GetBridgesOfType(BridgeType type)
    {
        List<Bridge> result = new List<Bridge>();
        foreach (var bridge in bridges)
        {
            if (bridge.BridgeType == type)
                result.Add(bridge);
        }
        return result;
    }

    public Bridge GetClosestBridge(Vector2 position)
    {
        if (bridges.Count == 0) return null;

        Bridge closest = bridges[0];
        float minDist = Vector2.Distance(position, closest.Position);

        foreach (var bridge in bridges)
        {
            float dist = Vector2.Distance(position, bridge.Position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = bridge;
            }
        }
        return closest;
    }
}

public class Bridge : MonoBehaviour
{
    public string BridgeName { get; private set; }
    public Vector2 Position { get; private set; }
    public BridgeType BridgeType { get; private set; }
    public float Length { get; private set; }
    public bool IsElevated { get; private set; }
    public float ClearanceHeight { get; private set; }

    private SpriteRenderer deckRenderer;
    private SpriteRenderer pillarRenderer;
    private BoxCollider2D deckCollider;
    private List<GameObject> stairs = new List<GameObject>();

    public void Initialize(string name, Vector2 position, BridgeType type, float length, bool isElevated, float clearanceHeight)
    {
        BridgeName = name;
        Position = position;
        BridgeType = type;
        Length = length;
        IsElevated = isElevated;
        ClearanceHeight = clearanceHeight;

        CreateBridgeVisuals();
        
        if (type == BridgeType.Footbridge && isElevated && clearanceHeight > 0)
        {
            CreateStairs();
        }
    }

    private void CreateBridgeVisuals()
    {
        Color bridgeColor = GetBridgeColor();
        float bridgeWidth = GetBridgeWidth();
        float bridgeHeight = GetBridgeHeight();

        Vector3 deckPosition = transform.position;
        if (IsElevated && BridgeType == BridgeType.Footbridge && ClearanceHeight > 0)
        {
            deckPosition = transform.position + Vector3.up * ClearanceHeight;
        }
        
        GameObject deck = new GameObject("Deck");
        deck.transform.position = deckPosition;
        deck.transform.parent = transform;

        deckRenderer = deck.AddComponent<SpriteRenderer>();
        deckRenderer.sprite = SpriteHelper.GetDefaultSprite();
        deckRenderer.color = bridgeColor;
        deckRenderer.sortingOrder = 15;
        deck.transform.localScale = new Vector3(bridgeWidth, bridgeHeight, 1f);

        if (BridgeType != BridgeType.Footbridge)
        {
            deckCollider = deck.AddComponent<BoxCollider2D>();
            deckCollider.size = Vector2.one;
            deckCollider.isTrigger = true;
        }

        CreatePillars();
        CreateRailings();
    }

    private Color GetBridgeColor()
    {
        return BridgeType switch
        {
            BridgeType.RailwayViaduct => new Color(0.25f, 0.2f, 0.15f, 1f),
            BridgeType.RoadBridge => new Color(0.4f, 0.4f, 0.4f, 1f),
            BridgeType.Footbridge => new Color(0.6f, 0.3f, 0.2f, 1f),
            BridgeType.CanalBridge => new Color(0.5f, 0.35f, 0.2f, 1f),
            _ => new Color(0.5f, 0.5f, 0.5f, 1f)
        };
    }

    private float GetBridgeWidth()
    {
        return BridgeType switch
        {
            BridgeType.RailwayViaduct => 12f,
            BridgeType.RoadBridge => 10f,
            BridgeType.Footbridge => 4f,
            BridgeType.CanalBridge => 6f,
            _ => 8f
        };
    }

    private float GetBridgeHeight()
    {
        return BridgeType switch
        {
            BridgeType.RailwayViaduct => 2f,
            BridgeType.RoadBridge => 1.5f,
            BridgeType.Footbridge => 1f,
            BridgeType.CanalBridge => 1f,
            _ => 1.5f
        };
    }

    private void CreatePillars()
    {
        if (IsElevated && BridgeType == BridgeType.Footbridge)
        {
            CreateFootbridgePillars();
            return;
        }
        
        int pillarCount = BridgeType == BridgeType.RailwayViaduct ? 5 : 2;
        float spacing = Length / (pillarCount + 1);

        for (int i = 1; i <= pillarCount; i++)
        {
            float offset = -Length / 2 + i * spacing;
            Vector2 pillarPos = Position + new Vector2(offset, -GetPillarHeight() / 2 - 3f);

            GameObject pillar = new GameObject($"Pillar_{i}");
            pillar.transform.position = (Vector3)pillarPos;
            pillar.transform.parent = transform;

            SpriteRenderer pillarSr = pillar.AddComponent<SpriteRenderer>();
            pillarSr.sprite = SpriteHelper.GetDefaultSprite();
            pillarSr.color = BridgeType == BridgeType.RailwayViaduct ? 
                new Color(0.3f, 0.25f, 0.2f, 1f) : new Color(0.5f, 0.5f, 0.5f, 1f);
            pillarSr.sortingOrder = 5;
            pillar.transform.localScale = new Vector3(1.5f, GetPillarHeight(), 1f);
        }
    }
    
    private void CreateFootbridgePillars()
    {
        float pillarWidth = 1f;
        float pillarHeight = ClearanceHeight;
        
        for (int side = -1; side <= 1; side += 2)
        {
            float xOffset = side * (GetBridgeWidth() / 2f + 1f);
            
            GameObject pillar = new GameObject($"FootbridgePillar_{side}");
            pillar.transform.position = transform.position + Vector3.right * xOffset + Vector3.up * pillarHeight / 2f;
            pillar.transform.parent = transform;
            
            SpriteRenderer pillarSr = pillar.AddComponent<SpriteRenderer>();
            pillarSr.sprite = SpriteHelper.GetDefaultSprite();
            pillarSr.color = new Color(0.5f, 0.45f, 0.4f, 1f);
            pillarSr.sortingOrder = 10;
            pillar.transform.localScale = new Vector3(pillarWidth, pillarHeight, 1f);
        }
    }
    
    private void CreateStairs()
    {
        CreateStaircase("LeftStairs", -GetBridgeWidth() / 2f - 2f);
        CreateStaircase("RightStairs", GetBridgeWidth() / 2f + 2f);
    }
    
    private void CreateStaircase(string name, float xOffset)
    {
        GameObject stairContainer = new GameObject(name);
        stairContainer.transform.parent = transform;
        
        int steps = Mathf.RoundToInt(ClearanceHeight * 1.5f);
        steps = Mathf.Clamp(steps, 6, 15);
        float stepHeight = ClearanceHeight / steps;
        float stepDepth = 1.2f;
        float stepWidth = 3f;
        
        for (int i = 0; i < steps; i++)
        {
            GameObject step = new GameObject($"Step_{i}");
            step.transform.parent = stairContainer.transform;
            
            float yPos = i * stepHeight + stepHeight / 2f;
            float zPos = -i * stepDepth * 0.5f;
            
            step.transform.localPosition = new Vector3(xOffset, yPos, zPos);
            
            SpriteRenderer sr = step.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.color = new Color(0.5f, 0.4f, 0.3f, 1f);
            sr.sortingOrder = 12;
            step.transform.localScale = new Vector3(stepWidth, stepHeight * 1.3f, 1f);
            
            stairs.Add(step);
        }
        
        GameObject platform = new GameObject("LandingPlatform");
        platform.transform.parent = stairContainer.transform;
        platform.transform.localPosition = new Vector3(xOffset, ClearanceHeight, -steps * stepDepth * 0.5f);
        
        SpriteRenderer platformSr = platform.AddComponent<SpriteRenderer>();
        platformSr.sprite = SpriteHelper.GetDefaultSprite();
        platformSr.color = new Color(0.5f, 0.4f, 0.3f, 1f);
        platformSr.sortingOrder = 12;
        platform.transform.localScale = new Vector3(stepWidth * 1.5f, 1.5f, 1f);
        
        stairs.Add(platform);
    }

    private float GetPillarHeight()
    {
        if (IsElevated && BridgeType == BridgeType.Footbridge)
        {
            return ClearanceHeight;
        }
        
        return BridgeType switch
        {
            BridgeType.RailwayViaduct => 15f,
            BridgeType.RoadBridge => 8f,
            BridgeType.Footbridge => 6f,
            BridgeType.CanalBridge => 5f,
            _ => 8f
        };
    }

    private void CreateRailings()
    {
        if (BridgeType == BridgeType.RailwayViaduct || BridgeType == BridgeType.RoadBridge)
            return;

        float railHeight = 1.5f;
        float railWidth = GetBridgeWidth();
        float railY = IsElevated && BridgeType == BridgeType.Footbridge ? ClearanceHeight + railHeight / 2f : railHeight * 0.5f;

        for (int side = -1; side <= 1; side += 2)
        {
            GameObject railing = new GameObject($"Railing_{side}");
            railing.transform.position = transform.position + Vector3.up * railY + Vector3.right * (side * railWidth / 2f);
            railing.transform.parent = transform;

            SpriteRenderer railSr = railing.AddComponent<SpriteRenderer>();
            railSr.sprite = SpriteHelper.GetDefaultSprite();
            railSr.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            railSr.sortingOrder = 16;
            railing.transform.localScale = new Vector3(railWidth, 0.2f, 1f);

            GameObject post = new GameObject($"Post_{side}");
            post.transform.position = transform.position + Vector3.up * (railY + railHeight) + Vector3.right * (side * railWidth / 2f);
            post.transform.parent = transform;

            SpriteRenderer postSr = post.AddComponent<SpriteRenderer>();
            postSr.sprite = SpriteHelper.GetDefaultSprite();
            postSr.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            postSr.sortingOrder = 16;
            post.transform.localScale = new Vector3(0.3f, railHeight, 1f);
        }
    }
    
    public bool CanVehiclePassUnder(Vector2 vehiclePosition)
    {
        if (!IsElevated) return true;
        
        float distAlongBridge = Vector2.Distance(vehiclePosition, Position);
        if (distAlongBridge > Length / 2f + 2f) return true;
        
        return true;
    }
    
    public Vector2 GetStairPosition(bool leftSide)
    {
        float xOffset = leftSide ? -GetBridgeWidth() / 2f - 2f : GetBridgeWidth() / 2f + 2f;
        return Position + new Vector2(xOffset, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 5f);
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 10f, BridgeName);
#endif
    }
}
