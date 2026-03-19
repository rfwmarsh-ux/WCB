using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages day/night cycle and ambient lighting changes
/// </summary>
public class DayNightCycleManager : MonoBehaviour
{
    [SerializeField] private float dayDurationInSeconds = 600f; // Full day cycle = 10 minutes
    [SerializeField] private Light2D mainLight;
    [SerializeField] private Camera mainCamera;

    private float currentTime = 0f;
    private float sunriseTime = 6f;      // 6 AM
    private float sunsetTime = 18f;      // 6 PM
    private float dayStartTime = 8f;     // 8 AM - full daylight
    private float nightStartTime = 20f;  // 8 PM - full night

    public enum TimeOfDay
    {
        Night,
        Sunrise,
        Day,
        Sunset,
        Dusk
    }

    public TimeOfDay CurrentTimeOfDay { get; private set; } = TimeOfDay.Day;
    public float CurrentHour { get; private set; } = 12f;

    private Color dayAmbientColor = new Color(1f, 1f, 1f, 1f);
    private Color sunriseAmbientColor = new Color(1f, 0.7f, 0.5f, 1f);
    private Color sunsetAmbientColor = new Color(1f, 0.6f, 0.4f, 1f);
    private Color nightAmbientColor = new Color(0.3f, 0.35f, 0.5f, 1f);
    private Color duskAmbientColor = new Color(0.5f, 0.4f, 0.6f, 1f);

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Initialize time to noon
        currentTime = 12f;
        UpdateTimeOfDay();
    }

    private void Update()
    {
        // Progress time (1 real second = 1 game minute)
        currentTime += Time.deltaTime / 60f;

        // Wrap around at 24 hours
        if (currentTime >= 24f)
            currentTime -= 24f;

        CurrentHour = currentTime;
        UpdateTimeOfDay();
        UpdateLighting();
    }

    private void UpdateTimeOfDay()
    {
        if (currentTime >= dayStartTime && currentTime < sunsetTime)
        {
            CurrentTimeOfDay = TimeOfDay.Day;
        }
        else if (currentTime >= sunriseTime && currentTime < dayStartTime)
        {
            CurrentTimeOfDay = TimeOfDay.Sunrise;
        }
        else if (currentTime >= sunsetTime && currentTime < nightStartTime)
        {
            CurrentTimeOfDay = TimeOfDay.Sunset;
        }
        else if (currentTime >= nightStartTime || currentTime < sunriseTime)
        {
            if (currentTime >= nightStartTime && currentTime < 22f)
                CurrentTimeOfDay = TimeOfDay.Dusk;
            else
                CurrentTimeOfDay = TimeOfDay.Night;
        }
    }

    private void UpdateLighting()
    {
        Color targetColor = dayAmbientColor;
        float lightIntensity = 1f;

        switch (CurrentTimeOfDay)
        {
            case TimeOfDay.Day:
                targetColor = dayAmbientColor;
                lightIntensity = 1f;
                break;

            case TimeOfDay.Sunrise:
                float sunriseProgress = (currentTime - sunriseTime) / (dayStartTime - sunriseTime);
                targetColor = Color.Lerp(nightAmbientColor, sunriseAmbientColor, sunriseProgress);
                lightIntensity = Mathf.Lerp(0.4f, 1f, sunriseProgress);
                break;

            case TimeOfDay.Sunset:
                float sunsetProgress = (currentTime - sunsetTime) / (nightStartTime - sunsetTime);
                targetColor = Color.Lerp(dayAmbientColor, sunsetAmbientColor, sunsetProgress);
                lightIntensity = Mathf.Lerp(1f, 0.6f, sunsetProgress);
                break;

            case TimeOfDay.Dusk:
                float duskProgress = (currentTime - nightStartTime) / 2f;
                targetColor = Color.Lerp(sunsetAmbientColor, duskAmbientColor, duskProgress);
                lightIntensity = Mathf.Lerp(0.6f, 0.3f, duskProgress);
                break;

            case TimeOfDay.Night:
                targetColor = nightAmbientColor;
                lightIntensity = 0.3f;
                break;
        }

        RenderSettings.ambientLight = targetColor;
        if (mainLight != null)
            mainLight.intensity = lightIntensity;

        // Adjust camera background color slightly
        Color bgColor = Color.Lerp(Color.white, new Color(0.2f, 0.2f, 0.3f), 1f - lightIntensity);
        mainCamera.backgroundColor = bgColor;
    }

    public string GetTimeString()
    {
        int hour = (int)currentTime;
        int minute = (int)((currentTime - hour) * 60f);
        return $"{hour:00}:{minute:00}";
    }

    public float GetLightIntensity()
    {
        return RenderSettings.ambientLight.grayscale;
    }

    public void SetTime(float hour)
    {
        currentTime = Mathf.Clamp(hour, 0f, 24f);
        UpdateTimeOfDay();
    }

    public float GetCurrentTime() => currentTime;
}
