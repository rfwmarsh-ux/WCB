using UnityEngine;

/// <summary>
/// Manages all environmental and visual effects systems
/// </summary>
public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private DayNightCycleManager dayNightCycleManager;
    [SerializeField] private WeatherSystem weatherSystem;
    [SerializeField] private SpriteRenderer skyRenderer;

    private void Start()
    {
        if (dayNightCycleManager == null)
            dayNightCycleManager = GetComponent<DayNightCycleManager>();
        
        if (weatherSystem == null)
            weatherSystem = GetComponent<WeatherSystem>();
    }

    private void Update()
    {
        UpdateEnvironmentalEffects();
    }

    private void UpdateEnvironmentalEffects()
    {
        // Apply weather color overlay to create visual effect
        if (skyRenderer != null && weatherSystem != null)
        {
            Color weatherColor = weatherSystem.GetWeatherColorOverlay();
            skyRenderer.color = weatherColor;
        }
    }

    public DayNightCycleManager GetDayNightManager()
    {
        return dayNightCycleManager;
    }

    public WeatherSystem GetWeatherSystem()
    {
        return weatherSystem;
    }

    public string GetEnvironmentInfo()
    {
        string timeStr = dayNightCycleManager.GetTimeString();
        string timeOfDay = dayNightCycleManager.CurrentTimeOfDay.ToString();
        string weather = weatherSystem.GetWeatherDescription();
        
        return $"Time: {timeStr} ({timeOfDay}) | Weather: {weather}";
    }
}
