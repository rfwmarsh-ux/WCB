using UnityEngine;
using System.Collections.Generic;

public class SirenManager : MonoBehaviour
{
    public static SirenManager Instance { get; private set; }

    private Dictionary<GameObject, SirenData> activeSirens = new Dictionary<GameObject, SirenData>();
    private AudioSource globalSirenSource;

    [Header("Siren Clips")]
    private AudioClip policeSirenClip;
    private AudioClip ambulanceSirenClip;

    [Header("Settings")]
    public float sirenRange = 100f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        globalSirenSource = gameObject.AddComponent<AudioSource>();
        globalSirenSource.loop = true;
        globalSirenSource.spatialBlend = 0f;
        globalSirenSource.playOnAwake = false;

        CreateSirenClips();
    }

    private void CreateSirenClips()
    {
        policeSirenClip = CreatePoliceSirenClip();
        ambulanceSirenClip = CreateAmbulanceSirenClip();
    }

    private AudioClip CreatePoliceSirenClip()
    {
        int sampleRate = 44100;
        float duration = 4f;
        int sampleLength = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleLength];

        float wailSpeed = 2f;
        float baseFreq = 600f;
        float freqRange = 400f;

        for (int i = 0; i < sampleLength; i++)
        {
            float t = (float)i / sampleRate;
            float freq = baseFreq + Mathf.Sin(t * wailSpeed * Mathf.PI) * freqRange;

            float sine = Mathf.Sin(2 * Mathf.PI * freq * t);
            float sine2 = Mathf.Sin(2 * Mathf.PI * freq * 1.5f * t) * 0.3f;
            float sine3 = Mathf.Sin(2 * Mathf.PI * freq * 2f * t) * 0.1f;

            float envelope = 1f;
            float cyclePos = (t * wailSpeed) % 1f;
            if (cyclePos < 0.1f)
            {
                envelope = cyclePos / 0.1f;
            }
            else if (cyclePos > 0.9f)
            {
                envelope = (1f - cyclePos) / 0.1f;
            }

            samples[i] = (sine + sine2 + sine3) * 0.3f * envelope;
        }

        AudioClip clip = AudioClip.Create("PoliceSiren", sampleLength, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateAmbulanceSirenClip()
    {
        int sampleRate = 44100;
        float duration = 2f;
        int sampleLength = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleLength];

        float wailSpeed = 4f;
        float baseFreq = 800f;
        float freqRange = 200f;

        for (int i = 0; i < sampleLength; i++)
        {
            float t = (float)i / sampleRate;
            float freq = baseFreq + Mathf.Sin(t * wailSpeed * Mathf.PI * 2f) * freqRange;

            float sawtooth = 0f;
            for (int h = 1; h <= 5; h++)
            {
                sawtooth += Mathf.Sin(2 * Mathf.PI * freq * h * t) / h;
            }
            sawtooth *= 0.5f;

            float pulse = 1f;
            if ((t * wailSpeed) % 1f < 0.5f)
            {
                pulse = 1f;
            }
            else
            {
                pulse = 0.6f;
            }

            samples[i] = sawtooth * 0.4f * pulse;
        }

        AudioClip clip = AudioClip.Create("AmbulanceSiren", sampleLength, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    public void RegisterSiren(GameObject vehicle, SirenType type, bool isActive = true)
    {
        if (activeSirens.ContainsKey(vehicle))
        {
            activeSirens[vehicle].isActive = isActive;
            UpdateSirenAudio();
            return;
        }

        AudioSource source = vehicle.AddComponent<AudioSource>();
        source.loop = true;
        source.spatialBlend = 1f;
        source.playOnAwake = false;
        source.volume = 0f;

        AudioClip clip = type == SirenType.Police ? policeSirenClip : ambulanceSirenClip;
        source.clip = clip;

        activeSirens[vehicle] = new SirenData
        {
            source = source,
            type = type,
            isActive = isActive,
            lightTimer = 0f,
            lightsOn = false
        };

        if (isActive)
        {
            source.Play();
        }

        UpdateSirenAudio();
    }

    public void SetSirenActive(GameObject vehicle, bool active)
    {
        if (activeSirens.TryGetValue(vehicle, out SirenData data))
        {
            data.isActive = active;
            if (active && !data.source.isPlaying)
            {
                data.source.Play();
            }
            else if (!active && data.source.isPlaying)
            {
                data.source.Stop();
            }
            UpdateSirenAudio();
        }
    }

    public void UnregisterSiren(GameObject vehicle)
    {
        if (activeSirens.TryGetValue(vehicle, out SirenData data))
        {
            data.source.Stop();
            if (data.source != null)
            {
                Destroy(data.source);
            }
            activeSirens.Remove(vehicle);
            UpdateSirenAudio();
        }
    }

    private void UpdateSirenAudio()
    {
        bool anyActive = false;
        foreach (var kvp in activeSirens)
        {
            if (kvp.Value.isActive)
            {
                anyActive = true;
                break;
            }
        }

        if (anyActive && !globalSirenSource.isPlaying)
        {
            globalSirenSource.Play();
        }
        else if (!anyActive && globalSirenSource.isPlaying)
        {
            globalSirenSource.Stop();
        }
    }

    public void UpdateSirenVolumes()
    {
        float sfxVol = MusicManager.Instance != null ? MusicManager.Instance.GetSfxVolume() : 1f;

        foreach (var kvp in activeSirens)
        {
            if (kvp.Value.source != null)
            {
                kvp.Value.source.volume = kvp.Value.isActive ? sfxVol : 0f;
            }
        }

        globalSirenSource.volume = sfxVol;
    }

    private void Update()
    {
        UpdateSirenLights();
    }

    private void UpdateSirenLights()
    {
        float deltaTime = Time.deltaTime;

        foreach (var kvp in activeSirens)
        {
            SirenData data = kvp.Value;
            if (!data.isActive) continue;

            data.lightTimer += deltaTime;
            if (data.lightTimer >= 0.3f)
            {
                data.lightTimer = 0f;
                data.lightsOn = !data.lightsOn;
                UpdateVehicleLights(kvp.Key, data);
            }
        }
    }

    private void UpdateVehicleLights(GameObject vehicle, SirenData data)
    {
        SpriteRenderer[] renderers = vehicle.GetComponentsInChildren<SpriteRenderer>();
        Color lightColor = data.type == SirenType.Police ?
            (data.lightsOn ? Color.red : Color.blue) :
            (data.lightsOn ? Color.red : Color.white);

        foreach (var renderer in renderers)
        {
            if (renderer.gameObject.name.Contains("Siren") || renderer.gameObject.name.Contains("Light"))
            {
                renderer.color = lightColor;
            }
        }
    }

    public void CreateSirenLights(GameObject vehicle, SirenType type)
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject light = new GameObject($"SirenLight_{i}");
            light.transform.parent = vehicle.transform;
            light.transform.localPosition = new Vector3(-1.5f + i * 3f, 1.5f, 0);

            SpriteRenderer sr = light.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.sortingOrder = 20;
            sr.color = type == SirenType.Police ? Color.red : Color.red;
        }
    }

    public int GetActiveSirenCount()
    {
        int count = 0;
        foreach (var kvp in activeSirens)
        {
            if (kvp.Value.isActive)
                count++;
        }
        return count;
    }
}

public enum SirenType
{
    Police,
    Ambulance
}

public class SirenData
{
    public AudioSource source;
    public SirenType type;
    public bool isActive;
    public float lightTimer;
    public bool lightsOn;
}
