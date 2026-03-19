using UnityEngine;

/// <summary>
/// Visualizes points of interest on the map (optional debug feature)
/// </summary>
public class POIVisualizer : MonoBehaviour
{
    [SerializeField] private PedestrianManager pedestrianManager;
    [SerializeField] private bool showPOIZones = false;
    [SerializeField] private bool showPOINames = false;

    private void OnDrawGizmosSelected()
    {
        if (!showPOIZones) return;

        if (pedestrianManager == null)
            pedestrianManager = FindObjectOfType<PedestrianManager>();

        if (pedestrianManager == null) return;

        var pois = pedestrianManager.GetPointsOfInterest();

        foreach (var poi in pois)
        {
            // Draw circle for each POI
            Gizmos.color = GetColorForPOIType(poi.type);
            DrawCircle(poi.location, poi.radius, 30);

            // Draw center point
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(new Vector3(poi.location.x, poi.location.y, 0), 1f);

            // Draw name if enabled
            if (showPOINames)
            {
#if UNITY_EDITOR
                UnityEditor.Handles.Label(new Vector3(poi.location.x, poi.location.y + poi.radius + 3, 0), poi.poiName);
#endif
            }
        }
    }

    private void DrawCircle(Vector2 center, float radius, int segments)
    {
        float angle = 0f;
        float angleStep = 360f / segments;
        
        Vector3 lastPoint = center + new Vector2(radius, 0);

        for (int i = 0; i < segments; i++)
        {
            angle += angleStep;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);
            
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }

    private Color GetColorForPOIType(PointOfInterest.POIType type)
    {
        return type switch
        {
            PointOfInterest.POIType.Shop => Color.yellow,
            PointOfInterest.POIType.ShoppingCentre => new Color(1f, 1f, 0.5f),
            PointOfInterest.POIType.School => Color.blue,
            PointOfInterest.POIType.University => new Color(0.5f, 0.5f, 1f),
            PointOfInterest.POIType.Park => Color.green,
            PointOfInterest.POIType.Restaurant => new Color(1f, 0.5f, 0f),
            PointOfInterest.POIType.Bar => Color.red,
            PointOfInterest.POIType.Library => new Color(0.5f, 0f, 0.5f),
            _ => Color.white
        };
    }

    public void TogglePOIZones()
    {
        showPOIZones = !showPOIZones;
    }

    public void TogglePOINames()
    {
        showPOINames = !showPOINames;
    }
}
