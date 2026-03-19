using UnityEngine;
using System.Collections.Generic;

public enum RoadType
{
    Residential,
    Major,
    RingRoad,
    DualCarriageway
}

public enum RoadDirection
{
    TwoWay,
    OneWayEast,
    OneWayWest,
    OneWayNorth,
    OneWaySouth
}

public class RoadSegment
{
    public Rect bounds;
    public RoadType type;
    public RoadDirection direction;
    public Vector2 center;

    public RoadSegment(Rect r, RoadType t, RoadDirection d = RoadDirection.TwoWay)
    {
        bounds = r;
        type = t;
        direction = d;
        center = new Vector2(r.x + r.width / 2, r.y + r.height / 2);
    }

    public bool IsHorizontal() => bounds.width > bounds.height;
    public bool IsVertical() => bounds.height >= bounds.width;

    public bool CanTravelInDirection(Vector2 direction)
    {
        if (direction == RoadDirection.TwoWay) return true;

        if (IsHorizontal())
        {
            if (direction == RoadDirection.OneWayEast) return true;
            if (direction == RoadDirection.OneWayWest) return false;
        }
        else
        {
            if (direction == RoadDirection.OneWayNorth) return true;
            if (direction == RoadDirection.OneWaySouth) return false;
        }
        return true;
    }

    public bool IsInSegment(Vector2 position)
    {
        return bounds.Contains(position);
    }
}

public class MapSystem : MonoBehaviour
{
    public static MapSystem Instance { get; private set; }

    [Header("Map Settings")]
    [SerializeField] private Vector2 mapCenter = new Vector2(500, 500);
    [SerializeField] private float mapWidth = 700f;
    [SerializeField] private float mapHeight = 700f;

    [Header("Road Widths (based on real UK roads)")]
    [SerializeField] private float residentialWidth = 16f;
    [SerializeField] private float majorRoadWidth = 28f;
    [SerializeField] private float ringRoadWidth = 36f;
    [SerializeField] private float dualCarriagewayWidth = 42f;
    [SerializeField] private float pavementWidth = 4f;
    [SerializeField] private float laneWidth = 4f;
    [SerializeField] private float shoulderWidth = 2f;
    [SerializeField] private float grassStripWidth = 5f;

    [Header("UK Traffic Settings")]
    [SerializeField] private float ukLeftLaneOffset = 4f;

    public const bool UK_LEFT_HAND_TRAFFIC = true;

    [Header("Colors")]
    [SerializeField] private Color grassColor = new Color(0.2f, 0.5f, 0.2f, 1f);
    [SerializeField] private Color roadColor = new Color(0.25f, 0.25f, 0.25f, 1f);
    [SerializeField] private Color majorRoadColor = new Color(0.22f, 0.22f, 0.25f, 1f);
    [SerializeField] private Color dualCarriagewayColor = new Color(0.2f, 0.2f, 0.22f, 1f);
    [SerializeField] private Color pavementColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] private Color barrierColor = new Color(0.7f, 0.1f, 0.1f, 1f);
    [SerializeField] private Color laneMarkingColor = new Color(1f, 1f, 1f, 0.8f);
    [SerializeField] private Color centerLineColor = new Color(1f, 0.85f, 0f, 1f);

    private List<Rect> roadSegments = new List<Rect>();
    private Dictionary<Rect, RoadType> roadTypes = new Dictionary<Rect, RoadType>();
    private Dictionary<Rect, RoadDirection> roadDirections = new Dictionary<Rect, RoadDirection>();
    private List<RoadSegment> allRoadSegments = new List<RoadSegment>();
    private List<Vector2> junctionPositions = new List<Vector2>();

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
        CreateGround();
        CreateRoadNetwork();
        CreateBarriers();
    }

    private void CreateGround()
    {
        GameObject ground = new GameObject("Ground");
        ground.transform.position = new Vector3(mapCenter.x, mapCenter.y, 10);
        
        SpriteRenderer sr = ground.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = grassColor;
        sr.sortingOrder = -10;
        
        ground.transform.localScale = new Vector3(mapWidth, mapHeight, 1);
        ground.transform.parent = transform;
    }

    private void CreateRoadNetwork()
    {
        float left = mapCenter.x - mapWidth / 2;
        float right = mapCenter.x + mapWidth / 2;
        float top = mapCenter.y + mapHeight / 2;
        float bottom = mapCenter.y - mapHeight / 2;

        // === RING ROAD (A4150 Wolverhampton Ring Road) ===
        // Outer ring - 6 lane dual carriageway
        CreateRoad(new Rect(left - dualCarriagewayWidth, top - dualCarriagewayWidth/2, mapWidth + dualCarriagewayWidth*2, dualCarriagewayWidth), RoadType.DualCarriageway);
        CreateRoad(new Rect(left - dualCarriagewayWidth, bottom - dualCarriagewayWidth/2, mapWidth + dualCarriagewayWidth*2, dualCarriagewayWidth), RoadType.DualCarriageway);
        CreateRoad(new Rect(left - dualCarriagewayWidth/2, bottom - dualCarriagewayWidth, dualCarriagewayWidth, mapHeight + dualCarriagewayWidth*2), RoadType.DualCarriageway);
        CreateRoad(new Rect(right - dualCarriagewayWidth/2, bottom - dualCarriagewayWidth, dualCarriagewayWidth, mapHeight + dualCarriagewayWidth*2), RoadType.DualCarriageway);

        // === MAJOR ROADS (4 lanes) ===
        // Central E-W major road (main thoroughfare)
        CreateRoad(new Rect(left - majorRoadWidth, 500 - majorRoadWidth/2, mapWidth + majorRoadWidth*2, majorRoadWidth), RoadType.Major);
        // Central N-S major road
        CreateRoad(new Rect(500 - majorRoadWidth/2, bottom - majorRoadWidth, majorRoadWidth, mapHeight + majorRoadWidth*2), RoadType.Major);
        
        // Secondary major roads
        CreateRoad(new Rect(left - majorRoadWidth, 300 - majorRoadWidth/2, mapWidth + majorRoadWidth*2, majorRoadWidth), RoadType.Major);
        CreateRoad(new Rect(left - majorRoadWidth, 700 - majorRoadWidth/2, mapWidth + majorRoadWidth*2, majorRoadWidth), RoadType.Major);
        CreateRoad(new Rect(300 - majorRoadWidth/2, bottom - majorRoadWidth, majorRoadWidth, mapHeight + majorRoadWidth*2), RoadType.Major);
        CreateRoad(new Rect(700 - majorRoadWidth/2, bottom - majorRoadWidth, majorRoadWidth, mapHeight + majorRoadWidth*2), RoadType.Major);

        // === RESIDENTIAL STREETS (2 lanes) ===
        // East-West residential
        CreateRoad(new Rect(left - residentialWidth, 200 - residentialWidth/2, mapWidth + residentialWidth*2, residentialWidth), RoadType.Residential);
        CreateRoad(new Rect(left - residentialWidth, 400 - residentialWidth/2, mapWidth + residentialWidth*2, residentialWidth), RoadType.Residential);
        CreateRoad(new Rect(left - residentialWidth, 600 - residentialWidth/2, mapWidth + residentialWidth*2, residentialWidth), RoadType.Residential);
        CreateRoad(new Rect(left - residentialWidth, 800 - residentialWidth/2, mapWidth + residentialWidth*2, residentialWidth), RoadType.Residential);

        // North-South residential
        CreateRoad(new Rect(100 - residentialWidth/2, bottom - residentialWidth, residentialWidth, mapHeight + residentialWidth*2), RoadType.Residential);
        CreateRoad(new Rect(200 - residentialWidth/2, bottom - residentialWidth, residentialWidth, mapHeight + residentialWidth*2), RoadType.Residential);
        CreateRoad(new Rect(400 - residentialWidth/2, bottom - residentialWidth, residentialWidth, mapHeight + residentialWidth*2), RoadType.Residential);
        CreateRoad(new Rect(600 - residentialWidth/2, bottom - residentialWidth, residentialWidth, mapHeight + residentialWidth*2), RoadType.Residential);
        CreateRoad(new Rect(800 - residentialWidth/2, bottom - residentialWidth, residentialWidth, mapHeight + residentialWidth*2), RoadType.Residential);

        // === JUNCTIONS ===
        for (float x = 100; x <= 800; x += 100)
        {
            for (float y = 200; y <= 800; y += 100)
            {
                if (x >= left && x <= right && y >= bottom && y <= top)
                {
                    junctionPositions.Add(new Vector2(x, y));
                }
            }
        }

        CreateOneWayStreets();
    }

    private void CreateOneWayStreets()
    {
        // Wolverhampton city center one-way streets
        // These are ADDITIONAL one-way roads, not replacing existing roads
        
        // City Centre ring road one-way connections (slip roads)
        CreateOneWayStreet(820f, 850f, 400f, 600f, RoadDirection.OneWayEast);
        CreateOneWayStreet(850f, 880f, 400f, 600f, RoadDirection.OneWayWest);
        
        // Bilston Road one-way section
        CreateOneWayStreet(280f, 300f, 250f, 350f, RoadDirection.OneWayNorth);
        CreateOneWayStreet(300f, 320f, 250f, 350f, RoadDirection.OneWayNorth);
        
        // Cleveland Road one-way
        CreateOneWayStreet(680f, 700f, 450f, 550f, RoadDirection.OneWaySouth);
        CreateOneWayStreet(700f, 720f, 450f, 550f, RoadDirection.OneWaySouth);
        
        // Newhampton Road one-way section  
        CreateOneWayStreet(620f, 650f, 150f, 250f, RoadDirection.OneWayNorth);
        
        // Merridale Road area
        CreateOneWayStreet(250f, 280f, 550f, 650f, RoadDirection.OneWayEast);
        
        // Penn Road area one-way
        CreateOneWayStreet(150f, 180f, 350f, 450f, RoadDirection.OneWayWest);
        
        Debug.Log($"Created {GetOneWayStreetCount()} one-way street segments");
    }

    private void CreateOneWayStreet(float x1, float x2, float y1, float y2, RoadDirection direction)
    {
        float width, height;
        if (direction == RoadDirection.OneWayEast || direction == RoadDirection.OneWayWest)
        {
            width = x2 - x1;
            height = residentialWidth;
        }
        else
        {
            width = residentialWidth;
            height = y2 - y1;
        }
        
        Rect rect = new Rect(x1, y1, width, height);
        CreateRoad(rect, RoadType.Residential, direction);
    }

    private int GetOneWayStreetCount()
    {
        int count = 0;
        foreach (var kvp in roadDirections)
        {
            if (kvp.Value != RoadDirection.TwoWay)
                count++;
        }
        return count;
    }

    private void CreateRoad(Rect rect, RoadType type, RoadDirection direction = RoadDirection.TwoWay)
    {
        roadSegments.Add(rect);
        roadTypes[rect] = type;
        roadDirections[rect] = direction;

        RoadSegment roadSeg = new RoadSegment(rect, type, direction);
        allRoadSegments.Add(roadSeg);

        GameObject road = new GameObject($"Road_{type}_{rect.x}_{rect.y}");
        if (direction != RoadDirection.TwoWay)
        {
            road.name += $"_{direction}";
        }
        road.transform.position = new Vector3(rect.x + rect.width / 2, rect.y + rect.height / 2, 1);
        
        SpriteRenderer sr = road.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        
        switch (type)
        {
            case RoadType.DualCarriageway:
                sr.color = dualCarriagewayColor;
                break;
            case RoadType.RingRoad:
                sr.color = dualCarriagewayColor;
                break;
            case RoadType.Major:
                sr.color = majorRoadColor;
                break;
            default:
                sr.color = roadColor;
                break;
        }
        
        sr.sortingOrder = -5;
        road.transform.localScale = new Vector3(rect.width, rect.height, 1);
        road.transform.parent = transform;

        // Add lane markings
        CreateLaneMarkings(rect, type, direction);

        // Add pavement edges with grass strips
        if (rect.width > rect.height)
        {
            // Top pavement and grass
            CreateGrassStrip(rect.x - grassStripWidth, rect.y + rect.height/2, grassStripWidth, rect.height);
            CreatePavementSegment(rect.x, rect.y + rect.height/2, pavementWidth, rect.height);
            CreateGrassStrip(rect.x + pavementWidth, rect.y + rect.height/2, shoulderWidth, rect.height);
            
            // Bottom pavement and grass
            CreateGrassStrip(rect.x + rect.width - pavementWidth - shoulderWidth, rect.y + rect.height/2 - pavementWidth - shoulderWidth, grassStripWidth, rect.height);
            CreatePavementSegment(rect.x + rect.width - pavementWidth, rect.y + rect.height/2 - pavementWidth/2 - shoulderWidth, pavementWidth, rect.height);
            CreateGrassStrip(rect.x + rect.width - pavementWidth - shoulderWidth, rect.y + rect.height/2 - pavementWidth/2 - shoulderWidth, shoulderWidth, rect.height);
        }
        else
        {
            // Left pavement and grass
            CreateGrassStrip(rect.x + rect.width/2, rect.y - grassStripWidth, rect.width, grassStripWidth);
            CreatePavementSegment(rect.x + rect.width/2 - pavementWidth/2, rect.y, rect.width, pavementWidth);
            CreateGrassStrip(rect.x + rect.width/2 - pavementWidth/2, rect.y + pavementWidth, rect.width, shoulderWidth);
            
            // Right pavement and grass
            CreateGrassStrip(rect.x + rect.width/2 - pavementWidth/2 - shoulderWidth, rect.y + rect.height - pavementWidth - shoulderWidth, rect.width, grassStripWidth);
            CreatePavementSegment(rect.x + rect.width/2 - pavementWidth/2, rect.y + rect.height - pavementWidth, rect.width, pavementWidth);
            CreateGrassStrip(rect.x + rect.width/2 - pavementWidth/2, rect.y + rect.height - pavementWidth - shoulderWidth, rect.width, shoulderWidth);
        }
    }

    private void CreateLaneMarkings(Rect rect, RoadType type, RoadDirection direction = RoadDirection.TwoWay)
    {
        float markingLength = laneWidth * 2f;
        float gapLength = laneWidth;
        
        if (direction != RoadDirection.TwoWay)
        {
            CreateOneWayArrows(rect, direction);
            return;
        }
        
        if (type == RoadType.Residential)
        {
            if (rect.width > rect.height)
            {
                CreateDashedLine(rect.x + markingLength/2, rect.y + rect.height/2, rect.width - markingLength, true, true);
            }
            else
            {
                CreateDashedLine(rect.x + rect.width/2, rect.y + markingLength/2, rect.height - markingLength, false, true);
            }
        }
        else if (type == RoadType.Major)
        {
            if (rect.width > rect.height)
            {
                float offset = laneWidth * 0.5f;
                CreateSolidLine(rect.x, rect.y + rect.height/2 - offset, rect.width, true, centerLineColor);
                CreateSolidLine(rect.x, rect.y + rect.height/2 + offset, rect.width, true, centerLineColor);
                
                CreateDashedLine(rect.x + markingLength/2, rect.y + laneWidth, rect.width - markingLength, true, false);
                CreateDashedLine(rect.x + markingLength/2, rect.y + rect.height - laneWidth, rect.width - markingLength, true, false);
            }
            else
            {
                float offset = laneWidth * 0.5f;
                CreateSolidLine(rect.x + rect.width/2 - offset, rect.y, rect.height, false, centerLineColor);
                CreateSolidLine(rect.x + rect.width/2 + offset, rect.y, rect.height, false, centerLineColor);
                
                CreateDashedLine(rect.x + laneWidth, rect.y + markingLength/2, rect.height - markingLength, false, false);
                CreateDashedLine(rect.x + rect.width - laneWidth, rect.y + markingLength/2, rect.height - markingLength, false, false);
            }
        }
        else if (type == RoadType.DualCarriageway || type == RoadType.RingRoad)
        {
            if (rect.width > rect.height)
            {
                float centerY = rect.y + rect.height/2;
                CreateSolidLine(rect.x, centerY - 1.5f, rect.width, true, Color.white);
                CreateSolidLine(rect.x, centerY + 1.5f, rect.width, true, Color.white);
                
                CreateDashedLine(rect.x + markingLength/2, rect.y + laneWidth * 1.5f, rect.width - markingLength, true, false);
                CreateDashedLine(rect.x + markingLength/2, rect.y + rect.height - laneWidth * 1.5f, rect.width - markingLength, true, false);
            }
            else
            {
                float centerX = rect.x + rect.width/2;
                CreateSolidLine(centerX - 1.5f, rect.y, rect.height, false, Color.white);
                CreateSolidLine(centerX + 1.5f, rect.y, rect.height, false, Color.white);
                
                CreateDashedLine(rect.x + laneWidth * 1.5f, rect.y + markingLength/2, rect.height - markingLength, false, false);
                CreateDashedLine(rect.x + rect.width - laneWidth * 1.5f, rect.y + markingLength/2, rect.height - markingLength, false, false);
            }
        }
    }

    private void CreateOneWayArrows(Rect rect, RoadDirection direction)
    {
        if (direction == RoadDirection.OneWayEast || direction == RoadDirection.OneWayWest)
        {
            float centerY = rect.y + rect.height / 2;
            float arrowY = centerY;
            int numArrows = Mathf.FloorToInt(rect.width / 80f);
            
            for (int i = 0; i < numArrows; i++)
            {
                float arrowX = rect.x + 40f + i * 80f;
                if (direction == RoadDirection.OneWayWest)
                {
                    arrowX = rect.xMax - 40f - i * 80f;
                }
                CreateArrow(arrowX, arrowY, direction == RoadDirection.OneWayWest, true);
            }
        }
        else
        {
            float centerX = rect.x + rect.width / 2;
            int numArrows = Mathf.FloorToInt(rect.height / 80f);
            
            for (int i = 0; i < numArrows; i++)
            {
                float arrowY = rect.y + 40f + i * 80f;
                if (direction == RoadDirection.OneWaySouth)
                {
                    arrowY = rect.yMax - 40f - i * 80f;
                }
                CreateArrow(centerX, arrowY, direction == RoadDirection.OneWaySouth, false);
            }
        }
    }

    private void CreateArrow(float x, float y, bool flipHorizontal, bool horizontal)
    {
        GameObject arrow = new GameObject("OneWayArrow");
        arrow.transform.position = new Vector3(x, y, 2);
        arrow.transform.parent = transform;
        
        SpriteRenderer sr = arrow.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = Color.white;
        sr.sortingOrder = -3;
        
        if (horizontal)
        {
            arrow.transform.localScale = new Vector3(8f, 3f, 1);
            if (flipHorizontal)
            {
                arrow.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else
            {
                arrow.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
        }
        else
        {
            arrow.transform.localScale = new Vector3(8f, 3f, 1);
            if (flipHorizontal)
            {
                arrow.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }
    }

    private void CreateDashedLine(float x, float y, float length, bool horizontal, bool isCenterLine)
    {
        float markingLength = laneWidth * 2f;
        float gapLength = laneWidth;
        float dashLength = markingLength + gapLength;
        
        int numDashes = Mathf.FloorToInt(length / dashLength);
        Color color = isCenterLine ? centerLineColor : laneMarkingColor;
        float thickness = isCenterLine ? 0.4f : 0.3f;
        
        for (int i = 0; i < numDashes; i++)
        {
            float startPos = horizontal ? x + i * dashLength : y + i * dashLength;
            
            GameObject marking = new GameObject("LaneMarking");
            if (horizontal)
                marking.transform.position = new Vector3(startPos + markingLength/2, y, 0.3f);
            else
                marking.transform.position = new Vector3(x, startPos + markingLength/2, 0.3f);
            
            SpriteRenderer sr = marking.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.color = color;
            sr.sortingOrder = -3;
            
            if (horizontal)
                marking.transform.localScale = new Vector3(markingLength, thickness, 1);
            else
                marking.transform.localScale = new Vector3(thickness, markingLength, 1);
        }
    }

    private void CreateSolidLine(float x, float y, float length, bool horizontal, Color color)
    {
        GameObject line = new GameObject("SolidLine");
        if (horizontal)
            line.transform.position = new Vector3(x + length/2, y, 0.3f);
        else
            line.transform.position = new Vector3(x, y + length/2, 0.3f);
        
        SpriteRenderer sr = line.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = color;
        sr.sortingOrder = -3;
        
        if (horizontal)
            line.transform.localScale = new Vector3(length, 0.4f, 1);
        else
            line.transform.localScale = new Vector3(0.4f, length, 1);
    }

    private void CreatePavementSegment(float x, float y, float width, float height)
    {
        GameObject pavement = new GameObject("Pavement");
        pavement.transform.position = new Vector3(x + width/2, y + height/2, 0.5f);
        
        SpriteRenderer sr = pavement.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = pavementColor;
        sr.sortingOrder = -4;
        
        pavement.transform.localScale = new Vector3(width, height, 1);
        pavement.transform.parent = transform;
    }

    private void CreateGrassStrip(float x, float y, float width, float height)
    {
        if (width <= 0 || height <= 0) return;
        
        GameObject grass = new GameObject("GrassStrip");
        grass.transform.position = new Vector3(x + width/2, y + height/2, 0.6f);
        
        SpriteRenderer sr = grass.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.18f, 0.45f, 0.18f, 1f);
        sr.sortingOrder = -3;
        
        grass.transform.localScale = new Vector3(width, height, 1);
        grass.transform.parent = transform;
    }

    private void CreateBarriers()
    {
        float left = mapCenter.x - mapWidth / 2;
        float right = mapCenter.x + mapWidth / 2;
        float top = mapCenter.y + mapHeight / 2;
        float bottom = mapCenter.y - mapHeight / 2;
        float thickness = 3f;

        CreateBarrierLine(new Vector2(left - thickness, bottom), new Vector2(right + thickness, bottom), thickness);
        CreateBarrierLine(new Vector2(right + thickness, bottom), new Vector2(right + thickness, top), thickness);
        CreateBarrierLine(new Vector2(right + thickness, top), new Vector2(left - thickness, top), thickness);
        CreateBarrierLine(new Vector2(left - thickness, top), new Vector2(left - thickness, bottom), thickness);
    }

    private void CreateBarrierLine(Vector2 start, Vector2 end, float thickness)
    {
        float length = Vector2.Distance(start, end);
        Vector2 midpoint = (start + end) / 2;
        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;

        GameObject barrier = new GameObject("Barrier");
        barrier.transform.position = (Vector3)midpoint;
        barrier.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        SpriteRenderer sr = barrier.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = barrierColor;
        sr.sortingOrder = 100;

        BoxCollider2D col = barrier.AddComponent<BoxCollider2D>();
        col.size = new Vector2(length, thickness);
        col.isTrigger = false;

        barrier.transform.parent = transform;
    }

    public bool IsOnRoad(Vector2 position)
    {
        foreach (Rect road in roadSegments)
        {
            if (road.Contains(position))
                return true;
        }
        return false;
    }

    public RoadType GetRoadType(Vector2 position)
    {
        foreach (var kvp in roadTypes)
        {
            if (kvp.Key.Contains(position))
                return kvp.Value;
        }
        return RoadType.Residential;
    }

    public int GetLanesForRoadType(RoadType type)
    {
        switch (type)
        {
            case RoadType.Residential: return 2;
            case RoadType.Major: return 4;
            case RoadType.RingRoad: return 6;
            case RoadType.DualCarriageway: return 6;
            default: return 2;
        }
    }

    public float GetRoadWidth(RoadType type)
    {
        switch (type)
        {
            case RoadType.Residential: return residentialWidth;
            case RoadType.Major: return majorRoadWidth;
            case RoadType.RingRoad: return ringRoadWidth;
            case RoadType.DualCarriageway: return dualCarriagewayWidth;
            default: return residentialWidth;
        }
    }

    public RoadDirection GetRoadDirectionAtPosition(Vector2 position)
    {
        foreach (RoadSegment segment in allRoadSegments)
        {
            if (segment.IsInSegment(position))
            {
                return segment.direction;
            }
        }
        return RoadDirection.TwoWay;
    }

    public RoadSegment GetRoadSegmentAtPosition(Vector2 position)
    {
        foreach (RoadSegment segment in allRoadSegments)
        {
            if (segment.IsInSegment(position))
            {
                return segment;
            }
        }
        return null;
    }

    public bool CanTravelFromTo(Vector2 from, Vector2 to)
    {
        RoadDirection direction = GetRoadDirectionAtPosition(from);
        if (direction == RoadDirection.TwoWay) return true;

        Vector2 travelDir = (to - from).normalized;
        
        if (direction == RoadDirection.OneWayEast && travelDir.x <= 0) return false;
        if (direction == RoadDirection.OneWayWest && travelDir.x >= 0) return false;
        if (direction == RoadDirection.OneWayNorth && travelDir.y <= 0) return false;
        if (direction == RoadDirection.OneWaySouth && travelDir.y >= 0) return false;

        return true;
    }

    public RoadDirection GetOppositeDirection(RoadDirection dir)
    {
        switch (dir)
        {
            case RoadDirection.OneWayEast: return RoadDirection.OneWayWest;
            case RoadDirection.OneWayWest: return RoadDirection.OneWayEast;
            case RoadDirection.OneWayNorth: return RoadDirection.OneWaySouth;
            case RoadDirection.OneWaySouth: return RoadDirection.OneWayNorth;
            default: return RoadDirection.TwoWay;
        }
    }

    public Vector2[] GetHorizontalRoads() => new float[] { 200f, 300f, 400f, 500f, 600f, 700f, 800f };
    public Vector2[] GetVerticalRoads() => new float[] { 100f, 200f, 300f, 400f, 500f, 600f, 700f, 800f };

    public float GetNearestHorizontalRoad(float y)
    {
        float[] roadYs = { 200f, 300f, 400f, 500f, 600f, 700f, 800f };
        
        float nearest = roadYs[0];
        float minDist = Mathf.Abs(y - nearest);
        
        foreach (float roadY in roadYs)
        {
            float dist = Mathf.Abs(y - roadY);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = roadY;
            }
        }
        
        return nearest;
    }

    public float GetNearestVerticalRoad(float x)
    {
        float[] roadXs = { 100f, 200f, 300f, 400f, 500f, 600f, 700f, 800f };
        
        float nearest = roadXs[0];
        float minDist = Mathf.Abs(x - nearest);
        
        foreach (float roadX in roadXs)
        {
            float dist = Mathf.Abs(x - roadX);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = roadX;
            }
        }
        
        return nearest;
    }

    public Vector2 GetRandomJunction()
    {
        if (junctionPositions.Count == 0)
            return mapCenter;
        return junctionPositions[Random.Range(0, junctionPositions.Count)];
    }

    public Vector2 GetMapCenter() => mapCenter;
    public Vector2 GetMapSize() => new Vector2(mapWidth, mapHeight);
    public float GetLaneWidth() => laneWidth;
    public float GetUkLeftLaneOffset() => ukLeftLaneOffset;
    public List<Vector2> GetJunctions() => junctionPositions;
    public List<Rect> GetRoadSegments() => roadSegments;

    public Vector2 GetLeftLanePosition(Vector2 position)
    {
        RoadSegment segment = GetRoadSegmentAtPosition(position);
        if (segment == null) return position;

        if (segment.IsHorizontal())
        {
            return new Vector2(position.x, segment.bounds.yMax - ukLeftLaneOffset);
        }
        else
        {
            return new Vector2(segment.bounds.xMin + ukLeftLaneOffset, position.y);
        }
    }

    public Vector2 GetRightLanePosition(Vector2 position)
    {
        RoadSegment segment = GetRoadSegmentAtPosition(position);
        if (segment == null) return position;

        if (segment.IsHorizontal())
        {
            return new Vector2(position.x, segment.bounds.yMin + ukLeftLaneOffset);
        }
        else
        {
            return new Vector2(segment.bounds.xMax - ukLeftLaneOffset, position.y);
        }
    }
}