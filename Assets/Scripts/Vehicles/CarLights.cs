using UnityEngine;

public class CarLights : MonoBehaviour
{
    [Header("Light Configuration")]
    public float headlightDistance = 40f;
    public float headlightAngle = 30f;
    public float taillightDistance = 15f;

    private SpriteRenderer headlightLeft;
    private SpriteRenderer headlightRight;
    private SpriteRenderer taillightLeft;
    private SpriteRenderer taillightRight;
    private SpriteRenderer headlightBeamLeft;
    private SpriteRenderer headlightBeamRight;

    private bool lightsOn = false;
    private bool manualOverride = false;
    private bool autoMode = true;

    private void Awake()
    {
        CreateLights();
    }

    private void Start()
    {
        UpdateLightState();
    }

    private void Update()
    {
        if (autoMode && DayNightCycleManager.Instance != null)
        {
            bool shouldBeNight = DayNightCycleManager.Instance.CurrentTimeOfDay == DayNightCycleManager.TimeOfDay.Night ||
                               DayNightCycleManager.Instance.CurrentTimeOfDay == DayNightCycleManager.TimeOfDay.Dusk;

            if (shouldBeNight != lightsOn && !manualOverride)
            {
                lightsOn = shouldBeNight;
                UpdateLightState();
            }
        }
    }

    private void CreateLights()
    {
        headlightLeft = CreateLight("HeadlightLeft", new Vector2(-4f, 8f), new Color(1f, 1f, 0.9f, 1f), 12f);
        headlightRight = CreateLight("HeadlightRight", new Vector2(4f, 8f), new Color(1f, 1f, 0.9f, 1f), 12f);

        taillightLeft = CreateLight("TaillightLeft", new Vector2(-4f, -8f), new Color(1f, 0.1f, 0.1f, 1f), 8f);
        taillightRight = CreateLight("TaillightRight", new Vector2(4f, -8f), new Color(1f, 0.1f, 0.1f, 1f), 8f);

        headlightBeamLeft = CreateLight("HeadlightBeamLeft", new Vector2(-3f, 12f), new Color(1f, 1f, 0.8f, 0.15f), headlightDistance);
        headlightBeamRight = CreateLight("HeadlightBeamRight", new Vector2(3f, 12f), new Color(1f, 1f, 0.8f, 0.15f), headlightDistance);

        if (headlightBeamLeft != null)
            headlightBeamLeft.transform.localRotation = Quaternion.Euler(0, 0, -10);
        if (headlightBeamRight != null)
            headlightBeamRight.transform.localRotation = Quaternion.Euler(0, 0, 10);
    }

    private SpriteRenderer CreateLight(string name, Vector2 offset, Color color, float size)
    {
        GameObject lightGO = new GameObject(name);
        lightGO.transform.parent = transform;
        lightGO.transform.localPosition = (Vector3)offset;

        SpriteRenderer sr = lightGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = color;
        sr.sortingOrder = 10;
        sr.transform.localScale = new Vector3(size, size, 1f);
        sr.enabled = false;

        return sr;
    }

    public void ToggleLights()
    {
        manualOverride = true;
        autoMode = false;
        lightsOn = !lightsOn;
        UpdateLightState();
    }

    public void SetLightsOn(bool on)
    {
        manualOverride = true;
        autoMode = false;
        lightsOn = on;
        UpdateLightState();
    }

    public void EnableAutoMode()
    {
        autoMode = true;
        manualOverride = false;
    }

    public bool AreLightsOn()
    {
        return lightsOn;
    }

    private void UpdateLightState()
    {
        bool isOn = lightsOn;

        if (headlightLeft != null) headlightLeft.enabled = isOn;
        if (headlightRight != null) headlightRight.enabled = isOn;
        if (taillightLeft != null) taillightLeft.enabled = isOn;
        if (taillightRight != null) taillightRight.enabled = isOn;
        if (headlightBeamLeft != null) headlightBeamLeft.enabled = isOn;
        if (headlightBeamRight != null) headlightBeamRight.enabled = isOn;
    }
}
