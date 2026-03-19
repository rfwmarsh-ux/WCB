using UnityEngine;
using System.Collections.Generic;

public class TunnelManager : MonoBehaviour
{
    public static TunnelManager Instance { get; private set; }

    private List<Tunnel> tunnels = new List<Tunnel>();
    private List<TunnelEntrance> entrances = new List<TunnelEntrance>();

    [Header("Tunnel Settings")]
    public int numberOfTunnels = 4;
    public float tunnelWidth = 15f;
    public float tunnelHeight = 8f;
    public float undergroundDepth = -50f;
    public Color tunnelColor = new Color(0.1f, 0.1f, 0.12f, 1f);
    public Color tunnelEntranceColor = new Color(0.15f, 0.15f, 0.18f, 1f);

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
        CreateAllTunnels();
    }

    private void CreateAllTunnels()
    {
        CreateRailwayTunnel("North Railway Tunnel", new Vector2(50, 550), new Vector2(950, 550), true, true);
        CreateRailwayTunnel("South Railway Tunnel", new Vector2(950, 450), new Vector2(50, 450), true, true);
        
        CreateVehicleTunnel("Central Vehicle Tunnel", new Vector2(100, 500), new Vector2(900, 500), false, false);
        CreateVehicleTunnel("East Vehicle Tunnel", new Vector2(700, 100), new Vector2(700, 900), false, false);
        
        CreateUndergroundPassage("Metro Tunnel A", new Vector2(200, 300), new Vector2(800, 700), true, true);
        
        Debug.Log($"Created {tunnels.Count} underground tunnels");
        Debug.Log($"Created {entrances.Count} tunnel entrances");
    }

    private void CreateRailwayTunnel(string name, Vector2 start, Vector2 end, bool showStart, bool showEnd)
    {
        GameObject tunnelGO = new GameObject(name);
        tunnelGO.transform.parent = transform;

        float length = Vector2.Distance(start, end);
        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
        Vector2 midpoint = (start + end) / 2f;

        tunnelGO.transform.position = new Vector3(midpoint.x, undergroundDepth, 0);
        tunnelGO.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        CreateTunnelVisual(tunnelGO, length, tunnelWidth + 5f, tunnelHeight + 3f);

        Tunnel tunnel = tunnelGO.AddComponent<Tunnel>();
        tunnel.Initialize(name, start, end, length, TunnelType.Railway);
        tunnels.Add(tunnel);

        if (showStart) CreateTunnelEntrance(start, angle, TunnelType.Railway);
        if (showEnd) CreateTunnelEntrance(end, angle, TunnelType.Railway);
    }

    private void CreateVehicleTunnel(string name, Vector2 start, Vector2 end, bool showStart, bool showEnd)
    {
        GameObject tunnelGO = new GameObject(name);
        tunnelGO.transform.parent = transform;

        float length = Vector2.Distance(start, end);
        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
        Vector2 midpoint = (start + end) / 2f;

        tunnelGO.transform.position = new Vector3(midpoint.x, undergroundDepth, 0);
        tunnelGO.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        CreateTunnelVisual(tunnelGO, length, tunnelWidth, tunnelHeight);

        Tunnel tunnel = tunnelGO.AddComponent<Tunnel>();
        tunnel.Initialize(name, start, end, length, TunnelType.Vehicle);
        tunnels.Add(tunnel);

        if (showStart) CreateTunnelEntrance(start, angle, TunnelType.Vehicle);
        if (showEnd) CreateTunnelEntrance(end, angle, TunnelType.Vehicle);
    }

    private void CreateUndergroundPassage(string name, Vector2 start, Vector2 end, bool showStart, bool showEnd)
    {
        GameObject tunnelGO = new GameObject(name);
        tunnelGO.transform.parent = transform;

        float length = Vector2.Distance(start, end);
        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
        Vector2 midpoint = (start + end) / 2f;

        tunnelGO.transform.position = new Vector3(midpoint.x, undergroundDepth * 0.7f, 0);
        tunnelGO.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        CreateTunnelVisual(tunnelGO, length, tunnelWidth - 3f, tunnelHeight - 2f);

        Tunnel tunnel = tunnelGO.AddComponent<Tunnel>();
        tunnel.Initialize(name, start, end, length, TunnelType.Metro);
        tunnels.Add(tunnel);

        if (showStart) CreateTunnelEntrance(start, angle, TunnelType.Metro);
        if (showEnd) CreateTunnelEntrance(end, angle, TunnelType.Metro);
    }

    private void CreateTunnelVisual(GameObject tunnelGO, float length, float width, float height)
    {
        GameObject ceiling = new GameObject("Ceiling");
        ceiling.transform.parent = tunnelGO.transform;
        ceiling.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = ceiling.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = tunnelColor;
        sr.sortingOrder = -5;

        ceiling.transform.localScale = new Vector3(length, width, 1f);

        GameObject floor = new GameObject("Floor");
        floor.transform.parent = tunnelGO.transform;
        floor.transform.localPosition = new Vector3(0, 0, -2f);

        SpriteRenderer floorSr = floor.AddComponent<SpriteRenderer>();
        floorSr.sprite = SpriteHelper.GetDefaultSprite();
        floorSr.color = new Color(0.08f, 0.08f, 0.1f, 1f);
        floorSr.sortingOrder = -4;

        floor.transform.localScale = new Vector3(length, width * 0.8f, 1f);

        BoxCollider2D floorCollider = floor.AddComponent<BoxCollider2D>();
        floorCollider.size = new Vector2(length, width * 0.8f);
        floorCollider.isTrigger = true;

        for (int i = 0; i < length / 20f; i++)
        {
            GameObject support = new GameObject($"Support_{i}");
            support.transform.parent = tunnelGO.transform;
            support.transform.localPosition = new Vector3(-length / 2f + i * 20f + 10f, 0, -1f);

            SpriteRenderer supportSr = support.AddComponent<SpriteRenderer>();
            supportSr.sprite = SpriteHelper.GetDefaultSprite();
            supportSr.color = new Color(0.2f, 0.2f, 0.22f, 1f);
            supportSr.sortingOrder = -3;

            support.transform.localScale = new Vector3(2f, width, 1f);
        }

        GameObject lighting = new GameObject("Lighting");
        lighting.transform.parent = tunnelGO.transform;
        lighting.transform.localPosition = Vector3.zero;

        for (int i = 0; i < length / 30f; i++)
        {
            GameObject light = new GameObject($"Light_{i}");
            light.transform.parent = lighting.transform;
            light.transform.localPosition = new Vector3(-length / 2f + i * 30f + 15f, width / 2f - 2f, 1f);

            SpriteRenderer lightSr = light.AddComponent<SpriteRenderer>();
            lightSr.sprite = SpriteHelper.GetDefaultSprite();
            lightSr.color = new Color(1f, 0.9f, 0.6f, 0.8f);
            lightSr.sortingOrder = 5;

            light.transform.localScale = new Vector3(3f, 1f, 1f);
        }
    }

    private void CreateTunnelEntrance(Vector2 position, float angle, TunnelType type)
    {
        GameObject entranceGO = new GameObject($"{type}TunnelEntrance");
        entranceGO.transform.position = (Vector3)position;
        entranceGO.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject frame = new GameObject("EntranceFrame");
        frame.transform.parent = entranceGO.transform;
        frame.transform.localPosition = Vector3.zero;

        SpriteRenderer frameSr = frame.AddComponent<SpriteRenderer>();
        frameSr.sprite = SpriteHelper.GetDefaultSprite();
        frameSr.color = tunnelEntranceColor;
        frameSr.sortingOrder = 2;

        float frameWidth = type == TunnelType.Railway ? tunnelWidth + 10f : tunnelWidth + 5f;
        float frameHeight = type == TunnelType.Railway ? tunnelHeight + 5f : tunnelHeight + 3f;
        frame.transform.localScale = new Vector3(frameWidth, frameHeight, 1f);

        GameObject opening = new GameObject("TunnelOpening");
        opening.transform.parent = entranceGO.transform;
        opening.transform.localPosition = new Vector3(5f, 0, 0.1f);

        SpriteRenderer openingSr = opening.AddComponent<SpriteRenderer>();
        openingSr.sprite = SpriteHelper.GetDefaultSprite();
        openingSr.color = tunnelColor;
        openingSr.sortingOrder = -1;

        float openingWidth = type == TunnelType.Railway ? tunnelWidth + 5f : tunnelWidth;
        float openingHeight = type == TunnelType.Railway ? tunnelHeight + 3f : tunnelHeight;
        opening.transform.localScale = new Vector3(openingWidth, openingHeight, 1f);

        CircleCollider2D trigger = entranceGO.AddComponent<CircleCollider2D>();
        trigger.radius = type == TunnelType.Railway ? 8f : 6f;
        trigger.isTrigger = true;

        TunnelEntrance entrance = entranceGO.AddComponent<TunnelEntrance>();
        entrance.Initialize(type);
        entrances.Add(entranceGO);

        entranceGO.transform.parent = transform;
    }

    public List<Tunnel> GetAllTunnels() => tunnels;
    public List<TunnelEntrance> GetAllEntrances() => entrances;

    public Tunnel GetTunnelAtPosition(Vector2 position)
    {
        foreach (var tunnel in tunnels)
        {
            if (tunnel.IsInsideTunnel(position))
                return tunnel;
        }
        return null;
    }

    public bool IsInTunnel(Vector2 position)
    {
        return GetTunnelAtPosition(position) != null;
    }

    public TunnelType? GetTunnelTypeAtPosition(Vector2 position)
    {
        Tunnel tunnel = GetTunnelAtPosition(position);
        return tunnel?.TunnelType;
    }
}

public enum TunnelType
{
    Vehicle,
    Railway,
    Metro
}

public class Tunnel : MonoBehaviour
{
    public string TunnelName { get; private set; }
    public Vector2 StartPoint { get; private set; }
    public Vector2 EndPoint { get; private set; }
    public float Length { get; private set; }
    public TunnelType TunnelType { get; private set; }

    public void Initialize(string name, Vector2 start, Vector2 end, float length, TunnelType type)
    {
        TunnelName = name;
        StartPoint = start;
        EndPoint = end;
        Length = length;
        TunnelType = type;
    }

    public bool IsInsideTunnel(Vector2 position)
    {
        float perpendicularDist = GetPerpendicularDistance(position);
        float alongLine = GetPositionAlongLine(position);

        float maxDist = TunnelType == TunnelType.Railway ? 12f : 10f;

        return alongLine >= -5f && alongLine <= Length + 5f && perpendicularDist < maxDist;
    }

    private float GetPerpendicularDistance(Vector2 point)
    {
        Vector2 lineDir = (EndPoint - StartPoint).normalized;
        Vector2 pointDir = point - StartPoint;
        return Vector2.Distance(point, StartPoint + Vector2.Dot(pointDir, lineDir) * lineDir);
    }

    private float GetPositionAlongLine(Vector2 point)
    {
        Vector2 lineDir = (EndPoint - StartPoint).normalized;
        Vector2 pointDir = point - StartPoint;
        return Vector2.Dot(pointDir, lineDir);
    }
}

public class TunnelEntrance : MonoBehaviour
{
    public TunnelType Entrancetype { get; private set; }
    private bool playerInside = false;

    public void Initialize(TunnelType type)
    {
        Entrancetype = type;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Vehicle"))
        {
            playerInside = true;
            Debug.Log($"Entered {Entrancetype} tunnel");

            if (TunnelManager.Instance != null)
            {
                Tunnel tunnel = TunnelManager.Instance.GetTunnelAtPosition(transform.position);
                if (tunnel != null)
                {
                    SetPlayerUnderground(other.gameObject, true, tunnel.TunnelType);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Vehicle"))
        {
            playerInside = false;
            Debug.Log($"Exited tunnel");

            SetPlayerUnderground(other.gameObject, false, Entrancetype);
        }
    }

    private void SetPlayerUnderground(GameObject obj, bool underground, TunnelType type)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in renderers)
        {
            if (underground)
            {
                sr.sortingOrder -= 50;
                Color c = sr.color;
                c.a = 0.7f;
                sr.color = c;
            }
            else
            {
                sr.sortingOrder += 50;
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }
        }
    }
}
