using UnityEngine;
using System.Collections.Generic;

public class NavigationSystem : MonoBehaviour
{
    public static NavigationSystem Instance { get; private set; }

    private Vector2? currentTarget;
    private string targetName;
    private BuildingIconType targetType;
    private List<Vector2> currentRoute = new List<Vector2>();
    private bool isNavigating = false;
    private float routeRefreshInterval = 2f;
    private float routeRefreshTimer = 0f;

    private GameObject routeLineObject;
    private LineRenderer lineRenderer;
    private GameObject destinationMarker;

    [Header("Route Settings")]
    public Color routeColor = new Color(0.2f, 0.6f, 1f, 0.8f);
    public float routeLineWidth = 0.3f;
    public int routeSegments = 50;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (!isNavigating) return;

        routeRefreshTimer += Time.deltaTime;
        if (routeRefreshTimer >= routeRefreshInterval)
        {
            RefreshRoute();
            routeRefreshTimer = 0f;
        }

        UpdateRouteLine();
    }

    public void SetNavigationTarget(Vector2 target, string name, BuildingIconType type)
    {
        currentTarget = target;
        targetName = name;
        targetType = type;
        isNavigating = true;

        CalculateRoute();

        CreateRouteLine();
        CreateDestinationMarker();

        Debug.Log($"Navigation set to {name} at {target}");
    }

    public void ClearNavigation()
    {
        currentTarget = null;
        targetName = null;
        isNavigating = false;
        currentRoute.Clear();

        if (routeLineObject != null)
        {
            Destroy(routeLineObject);
            routeLineObject = null;
        }

        if (destinationMarker != null)
        {
            Destroy(destinationMarker);
            destinationMarker = null;
        }

        Debug.Log("Navigation cleared");
    }

    private void CalculateRoute()
    {
        currentRoute.Clear();

        Vector2 playerPos = GetPlayerPosition();
        Vector2 target = currentTarget.Value;

        float[] roadYs = GetRoadYPositions();
        float[] roadXs = GetRoadXPositions();

        Vector2 nearestPlayerRoadY = GetNearestHorizontalRoad(playerPos.y, roadYs);
        Vector2 nearestPlayerRoadX = GetNearestVerticalRoad(playerPos.x, roadXs);
        Vector2 nearestTargetRoadY = GetNearestHorizontalRoad(target.y, roadYs);
        Vector2 nearestTargetRoadX = GetNearestVerticalRoad(target.x, roadXs);

        currentRoute.Add(playerPos);

        Vector2 playerToRoad = new Vector2(nearestPlayerRoadX.x, nearestPlayerRoadY.y);
        currentRoute.Add(playerToRoad);

        if (Mathf.Abs(nearestPlayerRoadY.y - nearestTargetRoadY.y) > 5f)
        {
            float step = nearestTargetRoadY.y > nearestPlayerRoadY.y ? 100f : -100f;
            for (float y = nearestPlayerRoadY.y + step; step > 0 ? y <= nearestTargetRoadY.y : y >= nearestTargetRoadY.y; y += step)
            {
                if (y != nearestPlayerRoadY.y && y != nearestTargetRoadY.y)
                {
                    currentRoute.Add(new Vector2(nearestPlayerRoadX.x, y));
                }
            }
        }

        if (Mathf.Abs(nearestPlayerRoadX.x - nearestTargetRoadX.x) > 5f)
        {
            float step = nearestTargetRoadX.x > nearestPlayerRoadX.x ? 100f : -100f;
            for (float x = nearestPlayerRoadX.x + step; step > 0 ? x <= nearestTargetRoadX.x : x >= nearestTargetRoadX.x; x += step)
            {
                if (x != nearestPlayerRoadX.x && x != nearestTargetRoadX.x)
                {
                    currentRoute.Add(new Vector2(x, nearestTargetRoadY.y));
                }
            }
        }

        Vector2 targetToRoad = new Vector2(nearestTargetRoadX.x, nearestTargetRoadY.y);
        currentRoute.Add(targetToRoad);
        currentRoute.Add(target);
    }

    private float[] GetRoadYPositions()
    {
        if (MapSystem.Instance != null)
        {
            return new float[] { 200f, 300f, 400f, 500f, 600f, 700f, 800f };
        }
        return new float[] { 200f, 300f, 400f, 500f, 600f, 700f, 800f };
    }

    private float[] GetRoadXPositions()
    {
        if (MapSystem.Instance != null)
        {
            return new float[] { 100f, 200f, 300f, 400f, 500f, 600f, 700f, 800f };
        }
        return new float[] { 100f, 200f, 300f, 400f, 500f, 600f, 700f, 800f };
    }

    private Vector2 GetNearestHorizontalRoad(float y, float[] roadYs)
    {
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
        return new Vector2(0, nearest);
    }

    private Vector2 GetNearestVerticalRoad(float x, float[] roadXs)
    {
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
        return new Vector2(nearest, 0);
    }

    private void RefreshRoute()
    {
        if (currentTarget == null) return;
        CalculateRoute();
    }

    private void CreateRouteLine()
    {
        if (routeLineObject != null)
        {
            Destroy(routeLineObject);
        }

        routeLineObject = new GameObject("NavigationRoute");
        lineRenderer = routeLineObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = routeColor;
        lineRenderer.endColor = routeColor;
        lineRenderer.startWidth = routeLineWidth;
        lineRenderer.endWidth = routeLineWidth * 0.5f;
        lineRenderer.positionCount = 0;
        lineRenderer.sortingOrder = 20;
    }

    private void CreateDestinationMarker()
    {
        if (destinationMarker != null)
        {
            Destroy(destinationMarker);
        }

        destinationMarker = new GameObject("NavigationMarker");
        destinationMarker.transform.position = (Vector3)currentTarget.Value + Vector3.up * 3f;

        SpriteRenderer sr = destinationMarker.AddComponent<SpriteRenderer>();
        sr.sprite = GTASpriteGenerator.GetSprite("icon_navigation");
        sr.color = routeColor;
        sr.sortingOrder = 16;

        destinationMarker.AddComponent<DestinationPulse>();
    }

    private void UpdateRouteLine()
    {
        if (lineRenderer == null || currentRoute.Count < 2) return;

        List<Vector3> points = new List<Vector3>();
        float totalLength = 0f;

        for (int i = 0; i < currentRoute.Count; i++)
        {
            points.Add((Vector3)currentRoute[i] + Vector3.up * 2f);
        }

        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }

    private Vector2 GetPlayerPosition()
    {
        if (PlayerManager.Instance != null)
        {
            return PlayerManager.Instance.transform.position;
        }
        return Vector2.zero;
    }

    public bool IsNavigating() => isNavigating;
    public Vector2? GetCurrentTarget() => currentTarget;
    public string GetTargetName() => targetName;
    public BuildingIconType GetTargetType() => targetType;
    public List<Vector2> GetCurrentRoute() => currentRoute;
}

public class DestinationPulse : MonoBehaviour
{
    private float pulseSpeed = 3f;
    private float maxScale = 1.5f;
    private float minScale = 0.8f;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        float pulse = Mathf.Sin(Time.time * pulseSpeed);
        float scale = Mathf.Lerp(minScale, maxScale, (pulse + 1f) / 2f);
        transform.localScale = originalScale * scale;
    }
}
