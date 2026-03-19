using UnityEngine;
using TMPro;

/// <summary>
/// HUD display for time and weather information
/// </summary>
public class EnvironmentHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeDisplay;
    [SerializeField] private TextMeshProUGUI weatherDisplay;
    [SerializeField] private TextMeshProUGUI temperatureDisplay;
    [SerializeField] private EnvironmentManager environmentManager;

    private float temperatureBase = 15f;
    private float currentTemperature = 15f;

    private void Start()
    {
        if (environmentManager == null)
            environmentManager = FindObjectOfType<EnvironmentManager>();
    }

    private void Update()
    {
        if (environmentManager == null) return;

        UpdateTimeDisplay();
        UpdateWeatherDisplay();
        UpdateTemperatureDisplay();
    }

    private void UpdateTimeDisplay()
    {
        if (timeDisplay != null)
        {
            DayNightCycleManager dayNightManager = environmentManager.GetDayNightManager();
            string timeStr = dayNightManager.GetTimeString();
            string timeOfDay = dayNightManager.CurrentTimeOfDay.ToString();
            timeDisplay.text = $"{timeStr} ({timeOfDay})";

            // Change color based on time
            if (dayNightManager.CurrentTimeOfDay == DayNightCycleManager.TimeOfDay.Night || 
                dayNightManager.CurrentTimeOfDay == DayNightCycleManager.TimeOfDay.Dusk)
            {
                timeDisplay.color = new Color(0.7f, 0.7f, 1f); // Bluish tint
            }
            else
            {
                timeDisplay.color = Color.white;
            }
        }
    }

    private void UpdateWeatherDisplay()
    {
        if (weatherDisplay != null)
        {
            WeatherSystem weather = environmentManager.GetWeatherSystem();
            weatherDisplay.text = $"Weather: {weather.GetWeatherDescription()}";

            // Weather color coding
            switch (weather.CurrentWeather)
            {
                case WeatherSystem.WeatherType.Clear:
                    weatherDisplay.color = Color.yellow;
                    break;
                case WeatherSystem.WeatherType.Rain:
                    weatherDisplay.color = Color.cyan;
                    break;
                case WeatherSystem.WeatherType.Snow:
                    weatherDisplay.color = new Color(0.8f, 0.8f, 1f);
                    break;
                case WeatherSystem.WeatherType.Fog:
                    weatherDisplay.color = Color.gray;
                    break;
                case WeatherSystem.WeatherType.Storm:
                    weatherDisplay.color = new Color(0.5f, 0.5f, 1f);
                    break;
            }
        }
    }

    private void UpdateTemperatureDisplay()
    {
        if (temperatureDisplay != null)
        {
            // Temperature affected by time of day and weather
            WeatherSystem weather = environmentManager.GetWeatherSystem();
            DayNightCycleManager dayNight = environmentManager.GetDayNightManager();

            float baseTemp = 15f;

            // Time of day affects temperature
            if (dayNight.CurrentTimeOfDay == DayNightCycleManager.TimeOfDay.Day)
                baseTemp = 22f;
            else if (dayNight.CurrentTimeOfDay == DayNightCycleManager.TimeOfDay.Night)
                baseTemp = 8f;

            // Weather effects
            switch (weather.CurrentWeather)
            {
                case WeatherSystem.WeatherType.Snow:
                    baseTemp -= 5f;
                    break;
                case WeatherSystem.WeatherType.Storm:
                    baseTemp -= 3f;
                    break;
                case WeatherSystem.WeatherType.Rain:
                    baseTemp -= 2f;
                    break;
            }

            currentTemperature = Mathf.Lerp(currentTemperature, baseTemp, Time.deltaTime * 0.5f);
            temperatureDisplay.text = $"Temp: {currentTemperature:F1}°C";

            // Temperature color coding
            if (currentTemperature < 5f)
                temperatureDisplay.color = new Color(0f, 0.5f, 1f); // Cold - blue
            else if (currentTemperature > 25f)
                temperatureDisplay.color = new Color(1f, 0.5f, 0f); // Hot - orange
            else
                temperatureDisplay.color = Color.white;
        }
    }
}
