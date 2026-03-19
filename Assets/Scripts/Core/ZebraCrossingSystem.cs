using UnityEngine;
using System.Collections.Generic;

public class ZebraCrossingManager : MonoBehaviour
{
    public static ZebraCrossingManager Instance { get; private set; }
    
    private List<ZebraCrossing> crossings = new List<ZebraCrossing>();
    
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
        CreateAllCrossings();
    }
    
    private void CreateAllCrossings()
    {
        CreateCrossing("City Centre 1", new Vector2(500, 490), true, 20f);
        CreateCrossing("City Centre 2", new Vector2(490, 500), false, 18f);
        CreateCrossing("Station Approach", new Vector2(475, 530), true, 15f);
        CreateCrossing("Ring Road 1", new Vector2(520, 500), true, 22f);
        CreateCrossing("Ring Road 2", new Vector2(480, 540), false, 20f);
        CreateCrossing("Bilston Road", new Vector2(320, 380), true, 18f);
        CreateCrossing("Dudley Road", new Vector2(360, 360), true, 16f);
        CreateCrossing("Tettenhall Road", new Vector2(600, 680), true, 15f);
        CreateCrossing("Wednesfield", new Vector2(700, 400), true, 18f);
        CreateCrossing("Bus Station", new Vector2(485, 510), true, 20f);
        
        CreateCrossing("Residential 1", new Vector2(150, 200), true, 12f);
        CreateCrossing("Residential 2", new Vector2(300, 300), true, 12f);
        CreateCrossing("Residential 3", new Vector2(400, 400), true, 12f);
        CreateCrossing("Residential 4", new Vector2(600, 600), true, 12f);
        CreateCrossing("Residential 5", new Vector2(750, 500), true, 12f);
        
        Debug.Log($"Created {crossings.Count} zebra crossings");
    }
    
    private void CreateCrossing(string name, Vector2 position, bool isHorizontal, float width)
    {
        GameObject crossingGO = new GameObject($"ZebraCrossing_{name}");
        crossingGO.transform.position = (Vector3)position;
        crossingGO.tag = "ZebraCrossing";
        
        ZebraCrossing crossing = crossingGO.AddComponent<ZebraCrossing>();
        crossing.Initialize(name, position, isHorizontal, width);
        
        crossingGO.transform.parent = transform;
        crossings.Add(crossing);
    }
    
    public List<ZebraCrossing> GetAllCrossings() => crossings;
    
    public ZebraCrossing GetNearestCrossing(Vector2 position, float maxDistance = 20f)
    {
        ZebraCrossing nearest = null;
        float minDist = maxDistance;
        
        foreach (var crossing in crossings)
        {
            float dist = Vector2.Distance(position, crossing.Position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = crossing;
            }
        }
        
        return nearest;
    }
}

public class ZebraCrossing : MonoBehaviour
{
    public string Name { get; private set; }
    public Vector2 Position { get; private set; }
    public bool IsHorizontal { get; private set; }
    public float Width { get; private set; }
    
    private List<GameObject> stripes = new List<GameObject>();
    private float stripeWidth = 1f;
    private float stripeSpacing = 1.5f;
    private bool isPedestrianWaiting = false;
    
    public void Initialize(string name, Vector2 position, bool isHorizontal, float width)
    {
        Name = name;
        Position = position;
        IsHorizontal = isHorizontal;
        Width = width;
        
        CreateCrossingVisuals();
    }
    
    private void CreateCrossingVisuals()
    {
        int stripeCount = Mathf.FloorToInt(Width / (stripeWidth + stripeSpacing));
        float totalWidth = stripeCount * (stripeWidth + stripeSpacing);
        float startOffset = -totalWidth / 2f;
        
        for (int i = 0; i < stripeCount; i++)
        {
            GameObject stripe = new GameObject($"Stripe_{i}");
            stripe.transform.parent = transform;
            
            if (IsHorizontal)
            {
                stripe.transform.position = transform.position + new Vector3(startOffset + i * (stripeWidth + stripeSpacing), 0, 0);
                stripe.transform.localScale = new Vector3(stripeWidth, 4f, 1f);
            }
            else
            {
                stripe.transform.position = transform.position + new Vector3(0, startOffset + i * (stripeWidth + stripeSpacing), 0);
                stripe.transform.localScale = new Vector3(4f, stripeWidth, 1f);
            }
            
            SpriteRenderer sr = stripe.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.color = Color.white;
            sr.sortingOrder = 3;
            
            stripes.Add(stripe);
        }
    }
    
    public bool IsPlayerNear()
    {
        if (PlayerManager.Instance != null)
        {
            float dist = Vector2.Distance(PlayerManager.Instance.transform.position, Position);
            if (dist < Width)
            {
                return true;
            }
        }
        
        if (Player2Manager.Instance != null)
        {
            float dist = Vector2.Distance(Player2Manager.Instance.transform.position, Position);
            if (dist < Width)
            {
                return true;
            }
        }
        
        return false;
    }
    
    public bool HasPedestrianNear()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(Position, Width / 2f);
        
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Pedestrian"))
            {
                return true;
            }
        }
        
        return false;
    }
    
    public Vector2 GetNearestLaneEdge()
    {
        if (IsHorizontal)
        {
            return new Vector2(Position.x, Position.y - 2f);
        }
        else
        {
            return new Vector2(Position.x - 2f, Position.y);
        }
    }
}

public class ZebraCrossingProtocol : MonoBehaviour
{
    public static void HandlePedestrianCrossing(Pedestrian pedestrian, ZebraCrossing crossing)
    {
        if (crossing == null) return;
        
        if (crossing.HasPedestrianNear())
        {
            return;
        }
        
        Debug.Log($"Pedestrian crossing at {crossing.Name}");
    }
    
    public static bool ShouldVehicleStop(ZebraCrossing crossing, bool isErraticDriver)
    {
        if (crossing == null) return false;
        
        if (isErraticDriver && Random.value < 0.4f)
        {
            Debug.Log("Erratic driver ignored zebra crossing!");
            return false;
        }
        
        return crossing.HasPedestrianNear() || crossing.IsPlayerNear();
    }
}

public class FootbridgeSystem : MonoBehaviour
{
    public static FootbridgeSystem Instance { get; private set; }
    
    private List<Footbridge> footbridges = new List<Footbridge>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void RegisterFootbridge(Footbridge footbridge)
    {
        if (!footbridges.Contains(footbridge))
        {
            footbridges.Add(footbridge);
        }
    }
    
    public Footbridge GetNearestFootbridge(Vector2 position, float maxDistance = 30f)
    {
        Footbridge nearest = null;
        float minDist = maxDistance;
        
        foreach (var fb in footbridges)
        {
            float dist = Vector2.Distance(position, fb.Position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = fb;
            }
        }
        
        return nearest;
    }
    
    public List<Footbridge> GetFootbridgesNear(Vector2 position, float radius)
    {
        List<Footbridge> result = new List<Footbridge>();
        
        foreach (var fb in footbridges)
        {
            if (Vector2.Distance(position, fb.Position) <= radius)
            {
                result.Add(fb);
            }
        }
        
        return result;
    }
}

public class Footbridge : MonoBehaviour
{
    public string Name { get; private set; }
    public Vector2 Position { get; private set; }
    
    private GameObject bridgeDeck;
    private List<GameObject> stairs = new List<GameObject>();
    private float bridgeWidth = 4f;
    private float bridgeLength = 25f;
    private float bridgeHeight = 8f;
    
    public void Initialize(string name, Vector2 position)
    {
        Name = name;
        Position = position;
        
        CreateFootbridge();
        
        if (FootbridgeSystem.Instance != null)
        {
            FootbridgeSystem.Instance.RegisterFootbridge(this);
        }
    }
    
    private void CreateFootbridge()
    {
        CreateBridgeDeck();
        CreateStairs();
        CreateRailings();
    }
    
    private void CreateBridgeDeck()
    {
        bridgeDeck = new GameObject("BridgeDeck");
        bridgeDeck.transform.position = transform.position + Vector3.up * bridgeHeight;
        bridgeDeck.transform.parent = transform;
        
        SpriteRenderer sr = bridgeDeck.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.6f, 0.3f, 0.2f, 1f);
        sr.sortingOrder = 15;
        bridgeDeck.transform.localScale = new Vector3(bridgeWidth, bridgeLength, 1f);
    }
    
    private void CreateStairs()
    {
        CreateStaircase("LeftStairs", -bridgeWidth / 2f - 2f);
        CreateStaircase("RightStairs", bridgeWidth / 2f + 2f);
    }
    
    private void CreateStaircase(string name, float xOffset)
    {
        GameObject stairContainer = new GameObject(name);
        stairContainer.transform.position = transform.position;
        stairContainer.transform.parent = transform;
        
        int steps = 12;
        float stepHeight = bridgeHeight / steps;
        float stepDepth = 1.5f;
        float stepWidth = 3f;
        
        for (int i = 0; i < steps; i++)
        {
            GameObject step = new GameObject($"Step_{i}");
            step.transform.parent = stairContainer.transform;
            
            float yPos = i * stepHeight + stepHeight / 2f;
            float xPos = xOffset + (xOffset > 0 ? i * stepDepth / steps : -i * stepDepth / steps);
            
            step.transform.localPosition = new Vector3(xOffset, yPos, 0);
            
            SpriteRenderer sr = step.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.color = new Color(0.5f, 0.4f, 0.3f, 1f);
            sr.sortingOrder = 14;
            step.transform.localScale = new Vector3(stepWidth, stepHeight * 1.2f, 1f);
            
            stairs.Add(step);
        }
        
        GameObject platform = new GameObject("Platform");
        platform.transform.parent = stairContainer.transform;
        platform.transform.localPosition = new Vector3(xOffset, bridgeHeight, 0);
        
        SpriteRenderer platformSr = platform.AddComponent<SpriteRenderer>();
        platformSr.sprite = SpriteHelper.GetDefaultSprite();
        platformSr.color = new Color(0.5f, 0.4f, 0.3f, 1f);
        platformSr.sortingOrder = 14;
        platform.transform.localScale = new Vector3(stepWidth * 1.5f, 2f, 1f);
        
        stairs.Add(platform);
    }
    
    private void CreateRailings()
    {
        for (int side = -1; side <= 1; side += 2)
        {
            GameObject railing = new GameObject($"Railing_{side}");
            railing.transform.position = transform.position + Vector3.up * (bridgeHeight + 1f) + Vector3.right * (side * bridgeWidth / 2f);
            railing.transform.parent = transform;
            
            SpriteRenderer sr = railing.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            sr.sortingOrder = 16;
            railing.transform.localScale = new Vector3(0.2f, bridgeLength, 1f);
        }
    }
    
    public bool IsPlayerOnFootbridge(Vector2 playerPos)
    {
        float dist = Vector2.Distance(playerPos, transform.position + Vector3.up * bridgeHeight);
        return dist < bridgeLength / 2f;
    }
    
    public Vector2 GetStairPosition(bool leftSide)
    {
        float xOffset = leftSide ? -bridgeWidth / 2f - 2f : bridgeWidth / 2f + 2f;
        return transform.position + new Vector3(xOffset, 0, 0);
    }
    
    public bool CanPlayerUseFootbridge(Vector2 playerPos)
    {
        float distToBase = Vector2.Distance(playerPos, transform.position);
        float distToStairs = Mathf.Min(
            Vector2.Distance(playerPos, GetStairPosition(true)),
            Vector2.Distance(playerPos, GetStairPosition(false))
        );
        
        return distToStairs < 5f;
    }
}
