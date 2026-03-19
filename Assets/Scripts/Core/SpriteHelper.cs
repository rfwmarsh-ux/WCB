using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class SpriteHelper
{
    private static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    private static bool initialized = false;

    private static void Initialize()
    {
        if (initialized) return;
        initialized = true;
        
        LoadSpritesFromDisk();
    }

    private static void LoadSpritesFromDisk()
    {
        string basePath = Application.dataPath + "/Imports";
        
        if (!Directory.Exists(basePath))
        {
            Debug.Log("SpriteHelper: Imports folder not found, using placeholder sprites");
            return;
        }

        LoadSpritesFromFolder(basePath + "/PNG/Cars", "Cars");
        LoadSpritesFromFolder(basePath + "/PNG/Characters", "Characters");
        LoadSpritesFromFolder(basePath + "/PNG/Props", "Props");
        
        string[] charFolders = Directory.GetDirectories(basePath);
        foreach (string folder in charFolders)
        {
            string folderName = Path.GetFileName(folder);
            if (folderName == "PNG" || folderName == "Spritesheet") continue;
            
            LoadCharacterPoses(folder, folderName);
        }
        
        Debug.Log($"SpriteHelper: Loaded {spriteCache.Count} sprites");
    }

    private static void LoadSpritesFromFolder(string path, string category)
    {
        if (!Directory.Exists(path)) return;
        
        string[] files = Directory.GetFiles(path, "*.png");
        foreach (string file in files)
        {
            string spriteName = Path.GetFileNameWithoutExtension(file);
            string cacheKey = $"{category}/{spriteName}";
            
            if (!spriteCache.ContainsKey(cacheKey))
            {
                Sprite sprite = LoadSpriteFromFile(file);
                if (sprite != null)
                {
                    spriteCache[cacheKey] = sprite;
                }
            }
        }
    }

    private static void LoadCharacterPoses(string basePath, string characterType)
    {
        string[] poseFolders = new string[]
        {
            basePath + "/PNG/Poses",
            basePath + "/PNG/Poses HD"
        };
        
        foreach (string posesPath in poseFolders)
        {
            if (!Directory.Exists(posesPath)) continue;
            
            string[] files = Directory.GetFiles(posesPath, "*.png");
            foreach (string file in files)
            {
                string spriteName = Path.GetFileNameWithoutExtension(file);
                string cacheKey = $"Character/{characterType}_{spriteName}";
                
                if (!spriteCache.ContainsKey(cacheKey))
                {
                    Sprite sprite = LoadSpriteFromFile(file);
                    if (sprite != null)
                    {
                        spriteCache[cacheKey] = sprite;
                    }
                }
            }
        }
    }

    private static Sprite LoadSpriteFromFile(string filePath)
    {
        try
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            
            string name = Path.GetFileNameWithoutExtension(filePath);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            sprite.name = name;
            
            return sprite;
        }
        catch
        {
            return null;
        }
    }

    public static Sprite GetDefaultSprite()
    {
        Initialize();
        return GetCachedSprite("default");
    }

    public static Sprite GetPlayerSprite()
    {
        Initialize();
        return GetCachedSprite("player");
    }

    public static Sprite GetPlayer2Sprite()
    {
        Initialize();
        return GetCachedSprite("player2");
    }

    public static Sprite GetVehicleSprite(VehicleType type)
    {
        Initialize();
        
        string[] spriteOptions = GetVehicleSpriteNames(type);
        foreach (string name in spriteOptions)
        {
            if (spriteCache.TryGetValue($"Cars/{name}", out var sprite))
                return sprite;
        }
        
        return GetCachedSprite($"vehicle_{type}");
    }

    private static string[] GetVehicleSpriteNames(VehicleType type)
    {
        return type switch
        {
            VehicleType.Motorcycle => new string[] { "cycle", "cycle_low" },
            VehicleType.SportBike => new string[] { "formula", "kart" },
            VehicleType.Scooter => new string[] { "scooter" },
            VehicleType.CompactCar => new string[] { "sedan", "sedan_blue" },
            VehicleType.EconomyCar => new string[] { "sedan_blue", "rounded_green" },
            VehicleType.TaxiCab => new string[] { "taxi" },
            VehicleType.ClassicCoupe => new string[] { "sedan_vintage", "vintage" },
            VehicleType.SedanCar => new string[] { "sedan", "station" },
            VehicleType.LuxurySedan => new string[] { "sedan_blue", "sports_yellow" },
            VehicleType.MuscleCar => new string[] { "sports_red", "rounded_red" },
            VehicleType.SportsCar => new string[] { "sports_race", "sports_red", "sports_green" },
            VehicleType.SuperCar => new string[] { "formula", "sports_race" },
            VehicleType.PoliceCruiser => new string[] { "police" },
            VehicleType.Hatchback => new string[] { "sedan", "rounded_yellow" },
            VehicleType.SUV => new string[] { "suv", "suv_closed", "suv_green" },
            VehicleType.PickupTruck => new string[] { "truck", "truckcabin" },
            VehicleType.OffroadVehicle => new string[] { "buggy", "suv_military" },
            VehicleType.Van => new string[] { "van", "van_small" },
            VehicleType.Convertible => new string[] { "convertible", "sports_convertible" },
            VehicleType.ArmoredCar => new string[] { "transport", "riot" },
            VehicleType.RallyCar => new string[] { "sports_race", "kart" },
            VehicleType.DriftCar => new string[] { "sports_green", "sports_red" },
            VehicleType.HotRod => new string[] { "vintage", "sedan_vintage" },
            VehicleType.SlowVan => new string[] { "van_small", "van_flat" },
            VehicleType.MediumVan => new string[] { "van", "vendor" },
            VehicleType.FastVan => new string[] { "van_large", "van" },
            VehicleType.Ambulance => new string[] { "ambulance" },
            VehicleType.Bus => new string[] { "bus", "bus_school" },
            VehicleType.SlowTruck => new string[] { "truck", "truckdark" },
            VehicleType.Train => new string[] { "truck_trailer", "trucktank" },
            VehicleType.Metro => new string[] { "truckdelivery", "truckcabin_vintage" },
            _ => new string[] { "sedan" }
        };
    }

    public static Sprite GetPedestrianSprite()
    {
        Initialize();
        
        var maleKeys = new List<string>();
        var femaleKeys = new List<string>();
        
        foreach (var key in spriteCache.Keys)
        {
            if (key.Contains("Male person") && key.Contains("idle"))
                maleKeys.Add(key);
            if (key.Contains("Female person") && key.Contains("idle"))
                femaleKeys.Add(key);
        }
        
        if (maleKeys.Count > 0 && Random.value > 0.5f)
            return spriteCache[maleKeys[Random.Range(0, maleKeys.Count)]];
        
        if (femaleKeys.Count > 0)
            return spriteCache[femaleKeys[Random.Range(0, femaleKeys.Count)]];
        
        if (maleKeys.Count > 0)
            return spriteCache[maleKeys[Random.Range(0, maleKeys.Count)]];
        
        return GetCachedSprite("pedestrian");
    }

    public static Sprite GetPoliceSprite()
    {
        Initialize();
        if (spriteCache.TryGetValue("Cars/police", out var sprite))
            return sprite;
        return GetCachedSprite("police");
    }

    public static Sprite GetBuildingSprite(string type)
    {
        Initialize();
        return GetCachedSprite($"building_{type}");
    }

    public static Sprite GetParkSprite()
    {
        Initialize();
        return GetCachedSprite("park");
    }

    public static Sprite GetWaterSprite()
    {
        Initialize();
        return GetCachedSprite("water");
    }

    public static Sprite GetRoadSprite()
    {
        Initialize();
        return GetCachedSprite("road");
    }

    public static Sprite GetBridgeSprite()
    {
        Initialize();
        return GetCachedSprite("bridge");
    }

    public static Sprite GetTreeSprite()
    {
        Initialize();
        return GetCachedSprite("tree");
    }

    public static Sprite GetBusSprite()
    {
        Initialize();
        if (spriteCache.TryGetValue("Props/sign_blue", out var sprite))
            return sprite;
        return GetCachedSprite("bus_stop");
    }

    public static Sprite GetMetroSprite()
    {
        Initialize();
        if (spriteCache.TryGetValue("Props/sign_street", out var sprite))
            return sprite;
        return GetCachedSprite("metro_station");
    }

    public static Sprite GetTrainSprite()
    {
        Initialize();
        if (spriteCache.TryGetValue("Props/highway", out var sprite))
            return sprite;
        return GetCachedSprite("train_station");
    }

    private static Sprite GetCachedSprite(string key)
    {
        if (spriteCache.TryGetValue(key, out var cached))
            return cached;

        var sprite = CreatePlaceholderSprite(key);
        spriteCache[key] = sprite;
        return sprite;
    }

    private static Sprite CreatePlaceholderSprite(string key)
    {
        Color color = GetColorForKey(key);
        int width = 32, height = 32;
        
        if (key.Contains("vehicle") || key.Contains("car")) { width = 60; height = 30; }
        else if (key.Contains("player")) { width = 24; height = 24; }
        else if (key.Contains("pedestrian") || key.Contains("character")) { width = 20; height = 32; }
        else if (key.Contains("building")) { width = 40; height = 40; }
        else if (key.Contains("park")) { width = 60; height = 50; }
        else if (key.Contains("tree")) { width = 15; height = 20; }
        
        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float edgeDist = 1f;
                
                if (width > height)
                {
                    float xEdge = (float)x / width;
                    edgeDist = Mathf.Min(edgeDist, xEdge, 1f - xEdge);
                }
                else
                {
                    float yEdge = (float)y / height;
                    edgeDist = Mathf.Min(edgeDist, yEdge, 1f - yEdge);
                }
                
                float edge = Mathf.Clamp01(edgeDist * 8f);
                
                pixels[y * width + x] = color * edge;
                pixels[y * width + x].a = edge > 0.1f ? 1f : 0f;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }

    private static Color GetColorForKey(string key)
    {
        if (key.Contains("player")) return new Color(0.2f, 0.8f, 0.2f);
        if (key.Contains("player2")) return new Color(0.3f, 0.3f, 0.9f);
        if (key.Contains("police")) return new Color(0.1f, 0.15f, 0.3f);
        if (key.Contains("park")) return new Color(0.3f, 0.7f, 0.3f);
        if (key.Contains("water")) return new Color(0.2f, 0.4f, 0.7f, 0.8f);
        if (key.Contains("road")) return new Color(0.35f, 0.35f, 0.38f);
        if (key.Contains("tree")) return new Color(0.2f, 0.5f, 0.2f);
        if (key.Contains("bridge")) return new Color(0.5f, 0.45f, 0.4f);
        
        if (key.Contains("building"))
        {
            if (key.Contains("shop")) return new Color(0.8f, 0.6f, 0.4f);
            if (key.Contains("house")) return new Color(0.7f, 0.5f, 0.4f);
            if (key.Contains("pub")) return new Color(0.5f, 0.3f, 0.2f);
            if (key.Contains("church")) return new Color(0.6f, 0.6f, 0.6f);
            if (key.Contains("police")) return new Color(0.3f, 0.35f, 0.5f);
            if (key.Contains("hospital")) return Color.white;
            if (key.Contains("vet")) return new Color(0.4f, 0.7f, 0.5f);
            return new Color(0.6f, 0.55f, 0.5f);
        }
        
        return Color.white;
    }
}
