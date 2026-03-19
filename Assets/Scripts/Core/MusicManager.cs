using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Settings")]
    public bool musicEnabled = true;
    public bool sfxEnabled = true;
    public float musicVolume = 0.7f;
    public float sfxVolume = 0.8f;

    [Header("Music Tracks")]
    public bool isPlayingMenuMusic = false;
    public bool isPlayingGameMusic = false;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    
    private AudioClip menuMusic;
    private AudioClip gameMusic;
    private AudioClip[] gameTracks;

    private float menuMusicTimer = 0f;
    private float gameMusicTimer = 0f;
    private bool isMenuTrackPlaying = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        musicSource = GetComponent<AudioSource>();
        SetupMusicSource();
        
        sfxSource = gameObject.AddComponent<AudioSource>();
        SetupSfxSource();

        CreateMusicTracks();
    }

    private void SetupMusicSource()
    {
        musicSource.loop = false;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;
    }

    private void SetupSfxSource()
    {
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 1f;
        sfxSource.volume = sfxVolume;
    }

    private void CreateMusicTracks()
    {
        menuMusic = CreateRetroMenuMusic();
        gameMusic = CreateRetroGameMusic();
        
        gameTracks = new AudioClip[3];
        gameTracks[0] = CreateRetroGameTrack1();
        gameTracks[1] = CreateRetroGameTrack2();
        gameTracks[2] = CreateRetroGameTrack3();
    }

    private AudioClip CreateRetroMenuMusic()
    {
        int sampleRate = 44100;
        int duration = 16;
        int sampleLength = sampleRate * duration;
        float[] samples = new float[sampleLength];
        
        float bpm = 120f;
        float beatTime = 60f / bpm;
        
        int[] bassLine = new int[] { 55, 55, 73, 73, 65, 65, 82, 82 };
        int bassIndex = 0;
        
        for (int i = 0; i < sampleLength; i++)
        {
            float t = (float)i / sampleRate;
            float beat = (t % beatTime) / beatTime;
            
            float bassFreq = bassLine[bassIndex % bassLine.Length];
            float bass = Mathf.Sin(2 * Mathf.PI * bassFreq * t) * 0.4f;
            if (beat < 0.1f) bass *= 1.5f;
            
            float melody = Mathf.Sin(2 * Mathf.PI * 220f * t) * 0.15f;
            melody += Mathf.Sin(2 * Mathf.PI * 330f * t) * 0.1f;
            melody += Mathf.Sin(2 * Mathf.PI * 440f * t) * 0.08f;
            
            float drum = 0f;
            if (beat < 0.05f)
            {
                drum = Random.Range(-0.3f, 0.3f) * (1f - beat / 0.05f);
            }
            
            float pad = (Mathf.Sin(2 * Mathf.PI * 110f * t) + Mathf.Sin(2 * Mathf.PI * 165f * t)) * 0.08f;
            
            samples[i] = (bass + melody + drum + pad) * 0.6f;
            
            if (t > 0 && t % (beatTime * 8) < 0.01f)
            {
                bassIndex++;
            }
        }
        
        AudioClip clip = AudioClip.Create("RetroMenu", sampleLength, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateRetroGameMusic()
    {
        int sampleRate = 44100;
        int duration = 20;
        int sampleLength = sampleRate * duration;
        float[] samples = new float[sampleLength];
        
        float bpm = 100f;
        float beatTime = 60f / bpm;
        
        for (int i = 0; i < sampleLength; i++)
        {
            float t = (float)i / sampleRate;
            float beat = (t % beatTime) / beatTime;
            
            float bass = Mathf.Sin(2 * Mathf.PI * 65f * t) * 0.25f;
            
            float lead = Mathf.Sin(2 * Mathf.PI * 196f * t) * 0.12f;
            lead += Mathf.Sin(2 * Mathf.PI * 247f * t) * 0.1f;
            lead += Mathf.Sin(2 * Mathf.PI * 294f * t) * 0.08f;
            
            float arp = Mathf.Sin(2 * Mathf.PI * 392f * (t * 4 % 1)) * 0.1f * (beat < 0.3f ? 1 : 0.3f);
            
            float kick = beat < 0.1f ? Random.Range(-0.2f, 0.2f) * (1f - beat / 0.1f) : 0;
            
            samples[i] = (bass + lead + arp + kick) * 0.5f;
        }
        
        AudioClip clip = AudioClip.Create("RetroGame", sampleLength, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateRetroGameTrack1()
    {
        return CreateRetroGameMusicTrack(110f, 130f, 90f);
    }

    private AudioClip CreateRetroGameTrack2()
    {
        return CreateRetroGameMusicTrack(98f, 123f, 82f);
    }

    private AudioClip CreateRetroGameTrack3()
    {
        return CreateRetroGameMusicTrack(130f, 164f, 98f);
    }

    private AudioClip CreateRetroGameMusicTrack(float bassFreq, float midFreq, float highFreq)
    {
        int sampleRate = 44100;
        int duration = 24;
        int sampleLength = sampleRate * duration;
        float[] samples = new float[sampleLength];
        
        float bpm = 110f;
        float beatTime = 60f / bpm;
        
        for (int i = 0; i < sampleLength; i++)
        {
            float t = (float)i / sampleRate;
            float beat = (t % beatTime) / beatTime;
            
            float bass = Mathf.Sin(2 * Mathf.PI * bassFreq * t) * 0.3f;
            
            float mid = Mathf.Sin(2 * Mathf.PI * midFreq * t) * 0.15f;
            mid += Mathf.Sin(2 * Mathf.PI * midFreq * 1.5f * t) * 0.1f;
            
            float high = Mathf.Sin(2 * Mathf.PI * highFreq * t) * 0.1f;
            high += Mathf.Sin(2 * Mathf.PI * highFreq * 2f * t) * 0.05f;
            
            float perc = beat < 0.1f ? Random.Range(-0.15f, 0.15f) : 0;
            
            samples[i] = (bass + mid + high + perc) * 0.5f;
        }
        
        AudioClip clip = AudioClip.Create("GameTrack", sampleLength, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private void Update()
    {
        if (!musicEnabled) return;

        if (isPlayingMenuMusic)
        {
            UpdateMenuMusic();
        }
        else if (isPlayingGameMusic)
        {
            UpdateGameMusic();
        }
    }

    private void UpdateMenuMusic()
    {
        menuMusicTimer += Time.deltaTime;
        
        if (!musicSource.isPlaying || menuMusicTimer > 16f)
        {
            PlayMenuMusic();
        }
    }

    private void UpdateGameMusic()
    {
        gameMusicTimer += Time.deltaTime;
        
        float trackLength = 20f;
        
        if (!musicSource.isPlaying || gameMusicTimer > trackLength)
        {
            PlayRandomGameTrack();
        }
    }

    public void StartMenuMusic()
    {
        if (!musicEnabled) return;
        
        isPlayingMenuMusic = true;
        isPlayingGameMusic = false;
        PlayMenuMusic();
    }

    public void StopMenuMusic()
    {
        isPlayingMenuMusic = false;
        musicSource.Stop();
    }

    public void StartGameMusic()
    {
        if (!musicEnabled) return;
        
        isPlayingMenuMusic = false;
        isPlayingGameMusic = true;
        gameMusicTimer = 0f;
        PlayRandomGameTrack();
    }

    public void StopGameMusic()
    {
        isPlayingGameMusic = false;
        musicSource.Stop();
    }

    private void PlayMenuMusic()
    {
        if (menuMusic != null && musicEnabled)
        {
            musicSource.Stop();
            musicSource.clip = menuMusic;
            musicSource.Play();
            menuMusicTimer = 0f;
        }
    }

    private void PlayRandomGameTrack()
    {
        if (gameTracks.Length > 0 && musicEnabled)
        {
            int trackIndex = Random.Range(0, gameTracks.Length);
            musicSource.Stop();
            musicSource.clip = gameTracks[trackIndex];
            musicSource.Play();
            gameMusicTimer = 0f;
        }
    }

    public void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (sfxEnabled && clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlaySfxPitch(AudioClip clip, float pitchVariation = 0.1f)
    {
        if (sfxEnabled && clip != null)
        {
            sfxSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            sfxSource.PlayOneShot(clip);
            sfxSource.pitch = 1f;
        }
    }

    public void SetMusicEnabled(bool enabled)
    {
        musicEnabled = enabled;
        if (!enabled)
        {
            musicSource.Stop();
            isPlayingMenuMusic = false;
            isPlayingGameMusic = false;
        }
    }

    public void SetSfxEnabled(bool enabled)
    {
        sfxEnabled = enabled;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

    public void ToggleMusic()
    {
        SetMusicEnabled(!musicEnabled);
    }

    public void ToggleSfx()
    {
        SetSfxEnabled(!sfxEnabled);
    }

    public float GetSfxVolume()
    {
        return sfxEnabled ? sfxVolume : 0f;
    }

    public float GetMusicVolume()
    {
        return musicEnabled ? musicVolume : 0f;
    }
}
