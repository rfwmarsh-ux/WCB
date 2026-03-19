using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Dynamic weather system with visual effects
/// </summary>
public class WeatherSystem : MonoBehaviour
{
    [SerializeField] private ParticleSystem rainParticles;
    [SerializeField] private ParticleSystem snowParticles;
    [SerializeField] private ParticleSystem fogParticles;
    [SerializeField] private Camera mainCamera;

    private float weatherChangeTimer = 0f;
    private float nextWeatherChangeTime = 60f; // Change weather every ~1 minute

    public enum WeatherType
    {
        Clear,
        Rain,
        Snow,
        Fog,
        Storm
    }

    public WeatherType CurrentWeather { get; private set; } = WeatherType.Clear;
    private WeatherType nextWeather = WeatherType.Clear;
    private float weatherTransitionTime = 0f;
    private float weatherTransitionDuration = 10f;

    private Dictionary<WeatherType, Color> weatherColorOverlays = new Dictionary<WeatherType, Color>()
    {
        { WeatherType.Clear, Color.white },
        { WeatherType.Rain, new Color(0.8f, 0.8f, 0.9f, 1f) },
        { WeatherType.Snow, new Color(0.95f, 0.95f, 1f, 1f) },
        { WeatherType.Fog, new Color(0.7f, 0.7f, 0.75f, 1f) },
        { WeatherType.Storm, new Color(0.6f, 0.6f, 0.7f, 1f) }
    };

    private Color currentWeatherColor = Color.white;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        CurrentWeather = WeatherType.Clear;
        UpdateWeatherVisuals();
    }

    private void Update()
    {
        weatherChangeTimer += Time.deltaTime;

        // Randomly change weather
        if (weatherChangeTimer >= nextWeatherChangeTime)
        {
            ChangeWeather();
            weatherChangeTimer = 0f;
            nextWeatherChangeTime = Random.Range(60f, 180f); // 1-3 minutes between changes
        }

        // Smooth weather transition
        if (weatherTransitionTime > 0f)
        {
            weatherTransitionTime -= Time.deltaTime;
            UpdateWeatherTransition();
        }
    }

    private void ChangeWeather()
    {
        WeatherType[] weatherTypes = System.Enum.GetValues(typeof(WeatherType)) as WeatherType[];
        nextWeather = weatherTypes[Random.Range(0, weatherTypes.Length)];
        weatherTransitionTime = weatherTransitionDuration;
    }

    private void UpdateWeatherTransition()
    {
        float transitionProgress = 1f - (weatherTransitionTime / weatherTransitionDuration);
        
        Color targetColor = weatherColorOverlays[nextWeather];
        currentWeatherColor = Color.Lerp(weatherColorOverlays[CurrentWeather], targetColor, transitionProgress);

        if (weatherTransitionTime <= 0f)
        {
            CurrentWeather = nextWeather;
            UpdateWeatherVisuals();
        }
    }

    private void UpdateWeatherVisuals()
    {
        // Stop all weather effects
        if (rainParticles != null)
            rainParticles.Stop();
        if (snowParticles != null)
            snowParticles.Stop();
        if (fogParticles != null)
            fogParticles.Stop();

        // Enable appropriate effect
        switch (CurrentWeather)
        {
            case WeatherType.Clear:
                currentWeatherColor = weatherColorOverlays[WeatherType.Clear];
                break;

            case WeatherType.Rain:
                if (rainParticles != null)
                    rainParticles.Play();
                currentWeatherColor = weatherColorOverlays[WeatherType.Rain];
                break;

            case WeatherType.Snow:
                if (snowParticles != null)
                    snowParticles.Play();
                currentWeatherColor = weatherColorOverlays[WeatherType.Snow];
                break;

            case WeatherType.Fog:
                if (fogParticles != null)
                    fogParticles.Play();
                currentWeatherColor = weatherColorOverlays[WeatherType.Fog];
                break;

            case WeatherType.Storm:
                if (rainParticles != null)
                {
                    rainParticles.Play();
                    var emission = rainParticles.emission;
                    emission.rateOverTime = 100f; // Heavier rain
                }
                currentWeatherColor = weatherColorOverlays[WeatherType.Storm];
                break;
        }
    }

    public void SetWeather(WeatherType weather)
    {
        CurrentWeather = weather;
        UpdateWeatherVisuals();
    }

    public Color GetWeatherColorOverlay()
    {
        return currentWeatherColor;
    }

    public string GetWeatherDescription()
    {
        switch (CurrentWeather)
        {
            case WeatherType.Clear:
                return "Clear skies";
            case WeatherType.Rain:
                return "Rainy";
            case WeatherType.Snow:
                return "Snowing";
            case WeatherType.Fog:
                return "Foggy";
            case WeatherType.Storm:
                return "Thunderstorm";
            default:
                return "Unknown";
        }
    }

    public bool IsWeatherHazardous()
    {
        return CurrentWeather == WeatherType.Storm || CurrentWeather == WeatherType.Fog;
    }
}
