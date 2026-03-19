using UnityEngine;
using System.Collections.Generic;

public enum VehicleSoundType
{
    Engine,
    Horn,
    Brake,
    Crash,
    Siren
}

public enum EngineSoundProfile
{
    Motorcycle,
    SportBike,
    Scooter,
    CompactCar,
    Sedan,
    SportsCar,
    Truck,
    Bus,
    Train,
    Emergency,
    Hatchback,
    SUV,
    Pickup,
    Offroad,
    Van,
    Convertible,
    Armored,
    Rally,
    Drift,
    HotRod
}

[RequireComponent(typeof(AudioSource))]
public class VehicleSoundSystem : MonoBehaviour
{
    public EngineSoundProfile engineProfile = EngineSoundProfile.Sedan;
    public bool isEngineOn = true;
    public bool isEmergency = false;
    
    private AudioSource engineSource;
    private AudioSource hornSource;
    private AudioSource sirenSource;
    
    private float basePitch = 1f;
    private float currentPitch;
    private float targetPitch;
    private float idlePitch = 0.5f;
    private float maxPitch = 2.5f;
    
    private float speed;
    private bool isHonking;
    private float hornDuration = 0.5f;
    private float hornTimer;

    private void Awake()
    {
        engineSource = GetComponent<AudioSource>();
        SetupEngineSource();
        
        hornSource = gameObject.AddComponent<AudioSource>();
        SetupHornSource();
        
        if (isEmergency)
        {
            sirenSource = gameObject.AddComponent<AudioSource>();
            SetupSirenSource();
        }
    }

    private void SetupEngineSource()
    {
        engineSource.loop = true;
        engineSource.playOnAwake = true;
        engineSource.spatialBlend = 1f;
        engineSource.minDistance = 5f;
        engineSource.maxDistance = 50f;
        engineSource.volume = 0.3f;
        
        SetEngineProfile();
    }

    private void SetupHornSource()
    {
        hornSource.loop = false;
        hornSource.playOnAwake = false;
        hornSource.spatialBlend = 1f;
        hornSource.minDistance = 10f;
        hornSource.maxDistance = 80f;
        hornSource.volume = 0.5f;
    }

    private void SetupSirenSource()
    {
        sirenSource.loop = true;
        sirenSource.playOnAwake = false;
        sirenSource.spatialBlend = 1f;
        sirenSource.minDistance = 20f;
        sirenSource.maxDistance = 150f;
        sirenSource.volume = 0.7f;
    }

    private void SetEngineProfile()
    {
        switch (engineProfile)
        {
            case EngineSoundProfile.Motorcycle:
                idlePitch = 0.8f;
                maxPitch = 3.5f;
                basePitch = 1.2f;
                break;
            case EngineSoundProfile.SportBike:
                idlePitch = 0.9f;
                maxPitch = 4.0f;
                basePitch = 1.4f;
                break;
            case EngineSoundProfile.Scooter:
                idlePitch = 0.7f;
                maxPitch = 2.8f;
                basePitch = 1.0f;
                break;
            case EngineSoundProfile.CompactCar:
                idlePitch = 0.5f;
                maxPitch = 2.2f;
                basePitch = 0.9f;
                break;
            case EngineSoundProfile.Sedan:
                idlePitch = 0.45f;
                maxPitch = 2.0f;
                basePitch = 0.85f;
                break;
            case EngineSoundProfile.SportsCar:
                idlePitch = 0.6f;
                maxPitch = 3.0f;
                basePitch = 1.1f;
                break;
            case EngineSoundProfile.Truck:
                idlePitch = 0.35f;
                maxPitch = 1.5f;
                basePitch = 0.6f;
                break;
            case EngineSoundProfile.Bus:
                idlePitch = 0.3f;
                maxPitch = 1.3f;
                basePitch = 0.55f;
                break;
            case EngineSoundProfile.Train:
                idlePitch = 0.25f;
                maxPitch = 0.8f;
                basePitch = 0.4f;
                break;
            case EngineSoundProfile.Emergency:
                idlePitch = 0.7f;
                maxPitch = 2.5f;
                basePitch = 1.0f;
                break;
            case EngineSoundProfile.Hatchback:
                idlePitch = 0.55f;
                maxPitch = 2.4f;
                basePitch = 0.95f;
                break;
            case EngineSoundProfile.SUV:
                idlePitch = 0.45f;
                maxPitch = 2.0f;
                basePitch = 0.8f;
                break;
            case EngineSoundProfile.Pickup:
                idlePitch = 0.4f;
                maxPitch = 1.8f;
                basePitch = 0.75f;
                break;
            case EngineSoundProfile.Offroad:
                idlePitch = 0.5f;
                maxPitch = 2.2f;
                basePitch = 0.85f;
                break;
            case EngineSoundProfile.Van:
                idlePitch = 0.4f;
                maxPitch = 1.9f;
                basePitch = 0.7f;
                break;
            case EngineSoundProfile.Convertible:
                idlePitch = 0.55f;
                maxPitch = 2.8f;
                basePitch = 1.05f;
                break;
            case EngineSoundProfile.Armored:
                idlePitch = 0.3f;
                maxPitch = 1.5f;
                basePitch = 0.6f;
                break;
            case EngineSoundProfile.Rally:
                idlePitch = 0.65f;
                maxPitch = 3.2f;
                basePitch = 1.15f;
                break;
            case EngineSoundProfile.Drift:
                idlePitch = 0.6f;
                maxPitch = 2.9f;
                basePitch = 1.1f;
                break;
            case EngineSoundProfile.HotRod:
                idlePitch = 0.5f;
                maxPitch = 2.6f;
                basePitch = 0.95f;
                break;
        }
        
        currentPitch = idlePitch;
        targetPitch = idlePitch;
    }

    private void Start()
    {
        if (isEngineOn && engineSource != null)
        {
            engineSource.Play();
        }
    }

    private void Update()
    {
        UpdateEngineSound();
        UpdateHornSound();
        
        if (isEmergency && sirenSource != null && !sirenSource.isPlaying)
        {
            sirenSource.Play();
        }
    }

    private void UpdateEngineSound()
    {
        if (engineSource == null || !isEngineOn) return;

        float speedFactor = Mathf.Abs(speed) / 20f;
        targetPitch = Mathf.Lerp(idlePitch, maxPitch, speedFactor) * basePitch;
        
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * 3f);
        engineSource.pitch = currentPitch;
    }

    private void UpdateHornSound()
    {
        if (hornTimer > 0)
        {
            hornTimer -= Time.deltaTime;
            if (hornTimer <= 0)
            {
                isHonking = false;
            }
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void HonkHorn(bool randomVariation = false)
    {
        if (isHonking) return;
        
        isHonking = true;
        hornDuration = randomVariation ? Random.Range(0.3f, 1.2f) : 0.5f;
        hornTimer = hornDuration;
        
        PlayHornSound();
    }

    private void PlayHornSound()
    {
        if (hornSource == null) return;
        
        float[] frequencies = GetHornFrequenciesForProfile(engineProfile);
        float freq = frequencies[Random.Range(0, frequencies.Length)];
        
        hornSource.pitch = Random.Range(0.8f, 1.2f);
        hornSource.PlayOneShot(CreateToneClip(freq, hornDuration), 0.6f);
    }

    private float[] GetHornFrequenciesForProfile(EngineSoundProfile profile)
    {
        return profile switch
        {
            EngineSoundProfile.Motorcycle => new float[] { 520f, 580f, 650f },
            EngineSoundProfile.SportBike => new float[] { 580f, 640f, 720f },
            EngineSoundProfile.Scooter => new float[] { 480f, 520f, 580f },
            EngineSoundProfile.CompactCar => new float[] { 400f, 450f, 500f },
            EngineSoundProfile.Sedan => new float[] { 380f, 420f, 460f },
            EngineSoundProfile.SportsCar => new float[] { 550f, 600f, 680f },
            EngineSoundProfile.Truck => new float[] { 280f, 320f, 350f },
            EngineSoundProfile.Bus => new float[] { 300f, 340f, 380f },
            EngineSoundProfile.Train => new float[] { 200f, 250f, 280f },
            EngineSoundProfile.Emergency => new float[] { 600f, 700f, 800f },
            EngineSoundProfile.Hatchback => new float[] { 420f, 470f, 520f },
            EngineSoundProfile.SUV => new float[] { 360f, 400f, 440f },
            EngineSoundProfile.Pickup => new float[] { 340f, 380f, 420f },
            EngineSoundProfile.Offroad => new float[] { 380f, 440f, 500f },
            EngineSoundProfile.Van => new float[] { 350f, 400f, 450f },
            EngineSoundProfile.Convertible => new float[] { 480f, 540f, 600f },
            EngineSoundProfile.Armored => new float[] { 260f, 300f, 340f },
            EngineSoundProfile.Rally => new float[] { 580f, 640f, 720f },
            EngineSoundProfile.Drift => new float[] { 500f, 560f, 620f },
            EngineSoundProfile.HotRod => new float[] { 440f, 500f, 560f },
            _ => new float[] { 400f, 450f, 500f }
        };
    }

    private AudioClip CreateToneClip(float frequency, float duration)
    {
        int sampleRate = 44100;
        int sampleLength = (int)(sampleRate * duration);
        float[] samples = new float[sampleLength];
        
        for (int i = 0; i < sampleLength; i++)
        {
            float t = (float)i / sampleRate;
            samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * 0.5f;
            
            float envelope = 1f;
            if (i < sampleLength * 0.1f)
                envelope = i / (sampleLength * 0.1f);
            else if (i > sampleLength * 0.7f)
                envelope = (sampleLength - i) / (sampleLength * 0.3f);
            
            samples[i] *= envelope;
        }
        
        AudioClip clip = AudioClip.Create("Horn", sampleLength, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    public void PlayBrakeSound()
    {
        if (hornSource == null) return;
        hornSource.pitch = 0.3f;
        hornSource.PlayOneShot(CreateNoiseClip(0.2f), 0.4f);
    }

    public void PlayCrashSound()
    {
        if (hornSource == null) return;
        hornSource.pitch = 0.5f;
        hornSource.PlayOneShot(CreateNoiseClip(0.5f), 0.8f);
    }

    private AudioClip CreateNoiseClip(float duration)
    {
        int sampleRate = 44100;
        int sampleLength = (int)(sampleRate * duration);
        float[] samples = new float[sampleLength];
        
        for (int i = 0; i < sampleLength; i++)
        {
            samples[i] = Random.Range(-1f, 1f) * 0.3f;
        }
        
        AudioClip clip = AudioClip.Create("Noise", sampleLength, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    public void ToggleEngine(bool on)
    {
        isEngineOn = on;
        if (on)
            engineSource.Play();
        else
            engineSource.Stop();
    }
}

public class VehicleHornManager : MonoBehaviour
{
    public static VehicleHornManager Instance { get; private set; }
    
    private Dictionary<VehicleType, float> hornProbability = new Dictionary<VehicleType, float>();
    private Dictionary<VehicleType, float> hornCooldowns = new Dictionary<VehicleType, float>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        InitializeHornProbabilities();
    }

    private void InitializeHornProbabilities()
    {
        hornProbability[VehicleType.Motorcycle] = 0.4f;
        hornProbability[VehicleType.CompactCar] = 0.25f;
        hornProbability[VehicleType.EconomyCar] = 0.2f;
        hornProbability[VehicleType.TaxiCab] = 0.35f;
        hornProbability[VehicleType.SedanCar] = 0.15f;
        hornProbability[VehicleType.SportsCar] = 0.3f;
        hornProbability[VehicleType.MuscleCar] = 0.35f;
        hornProbability[VehicleType.SuperCar] = 0.4f;
        hornProbability[VehicleType.PoliceCruiser] = 0.1f;
        hornProbability[VehicleType.Bus] = 0.2f;
        hornProbability[VehicleType.SlowTruck] = 0.15f;
        
        hornProbability[VehicleType.Hatchback] = 0.28f;
        hornProbability[VehicleType.SUV] = 0.22f;
        hornProbability[VehicleType.PickupTruck] = 0.25f;
        hornProbability[VehicleType.OffroadVehicle] = 0.3f;
        hornProbability[VehicleType.Van] = 0.2f;
        hornProbability[VehicleType.Convertible] = 0.35f;
        hornProbability[VehicleType.ArmoredCar] = 0.12f;
        hornProbability[VehicleType.RallyCar] = 0.45f;
        hornProbability[VehicleType.DriftCar] = 0.4f;
        hornProbability[VehicleType.HotRod] = 0.38f;
    }

    private void Update()
    {
        UpdateCooldowns();
    }

    private void UpdateCooldowns()
    {
        var keys = new List<VehicleType>(hornCooldowns.Keys);
        foreach (var key in keys)
        {
            hornCooldowns[key] -= Time.deltaTime;
            if (hornCooldowns[key] <= 0)
                hornCooldowns.Remove(key);
        }
    }

    public void OnVehicleCutOff(VehicleType vehicleType)
    {
        float baseProb = hornProbability.ContainsKey(vehicleType) ? hornProbability[vehicleType] : 0.2f;
        
        if (Random.value < baseProb * 2f)
        {
            TriggerHorn(vehicleType);
        }
    }

    public void OnRandomHorn(VehicleType vehicleType)
    {
        if (hornCooldowns.ContainsKey(vehicleType)) return;
        
        float baseProb = hornProbability.ContainsKey(vehicleType) ? hornProbability[vehicleType] : 0.2f;
        
        if (Random.value < baseProb * 0.1f)
        {
            TriggerHorn(vehicleType);
            hornCooldowns[vehicleType] = Random.Range(5f, 15f);
        }
    }

    private void TriggerHorn(VehicleType vehicleType)
    {
        var vehicles = FindObjectsOfType<VehicleSoundSystem>();
        foreach (var vehicle in vehicles)
        {
            if (IsMatchingType(vehicle.engineProfile, vehicleType))
            {
                vehicle.HonkHorn(true);
            }
        }
    }

    private bool IsMatchingType(EngineSoundProfile profile, VehicleType type)
    {
        return type switch
        {
            VehicleType.Motorcycle => profile == EngineSoundProfile.Motorcycle,
            VehicleType.CompactCar or VehicleType.EconomyCar => profile == EngineSoundProfile.CompactCar,
            VehicleType.SedanCar or VehicleType.LuxurySedan => profile == EngineSoundProfile.Sedan,
            VehicleType.SportsCar or VehicleType.SuperCar or VehicleType.MuscleCar => profile == EngineSoundProfile.SportsCar,
            VehicleType.SlowTruck => profile == EngineSoundProfile.Truck,
            VehicleType.Bus => profile == EngineSoundProfile.Bus,
            VehicleType.Train or VehicleType.Metro => profile == EngineSoundProfile.Train,
            VehicleType.PoliceCruiser or VehicleType.Ambulance => profile == EngineSoundProfile.Emergency,
            _ => profile == EngineSoundProfile.Sedan
        };
    }
    
    public void SetDamagedEngine(bool isDamaged)
    {
        if (engineSource != null)
        {
            if (isDamaged)
            {
                engineSource.pitch = Mathf.Min(engineSource.pitch * 0.7f, 0.5f);
                engineSource.volume = 0.5f;
            }
            else
            {
                engineSource.pitch = currentPitch;
                engineSource.volume = 0.3f;
            }
        }
    }
}
