using UnityEngine;
using System.Collections.Generic;

public static class GTASpriteGenerator
{
    private static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

    public static Sprite GetSprite(string key)
    {
        if (!sprites.ContainsKey(key))
        {
            sprites[key] = CreateSprite(key);
        }
        return sprites[key];
    }

    private static Sprite CreateSprite(string key)
    {
        if (key.StartsWith("weapon_"))
        {
            return CreateWeaponSprite(key);
        }
        else if (key.StartsWith("building_"))
        {
            return CreateBuildingSprite(key);
        }
        else if (key.StartsWith("street_"))
        {
            return CreateStreetSprite(key);
        }
        else if (key.StartsWith("pickup_"))
        {
            return CreatePickupSprite(key);
        }
        else if (key.StartsWith("icon_"))
        {
            return CreateIconSprite(key);
        }
        return CreatePlaceholderSprite(key);
    }

    private static Sprite CreateWeaponSprite(string key)
    {
        int size = 32;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color[size * size];
        
        Color darkGun = new Color(0.15f, 0.15f, 0.18f);
        Color metal = new Color(0.4f, 0.4f, 0.45f);
        Color handle = new Color(0.25f, 0.18f, 0.12f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int idx = y * size + x;
                pixels[idx] = Color.clear;
            }
        }

        void SetPixel(int x, int y, Color c)
        {
            if (x >= 0 && x < size && y >= 0 && y < size)
                pixels[y * size + x] = c;
        }

        void SetRect(int x, int y, int w, int h, Color c)
        {
            for (int py = y; py < y + h; py++)
                for (int px = x; px < x + w; px++)
                    SetPixel(px, py, c);
        }

        if (key == "weapon_pistol")
        {
            SetRect(4, 13, 20, 6, darkGun);
            SetRect(8, 19, 8, 10, handle);
            SetRect(6, 14, 4, 4, metal);
        }
        else if (key == "weapon_revolver")
        {
            SetRect(4, 12, 22, 7, darkGun);
            SetRect(8, 19, 10, 10, handle);
            SetRect(22, 13, 4, 5, metal);
        }
        else if (key == "weapon_smg")
        {
            SetRect(2, 10, 26, 8, darkGun);
            SetRect(4, 18, 12, 12, handle);
            SetRect(20, 8, 8, 4, new Color(0.12f, 0.12f, 0.15f));
            SetRect(8, 11, 16, 3, metal);
        }
        else if (key == "weapon_shotgun")
        {
            SetRect(2, 13, 28, 5, new Color(0.25f, 0.25f, 0.25f));
            SetRect(2, 14, 28, 3, new Color(0.12f, 0.12f, 0.12f));
            SetRect(10, 18, 8, 10, handle);
            SetRect(26, 14, 4, 3, metal);
        }
        else if (key == "weapon_assault")
        {
            SetRect(1, 10, 28, 8, darkGun);
            SetRect(3, 8, 10, 4, new Color(0.2f, 0.2f, 0.25f));
            SetRect(12, 18, 10, 12, handle);
            SetRect(22, 12, 6, 4, metal);
            SetRect(5, 11, 18, 4, metal);
        }
        else if (key == "weapon_sniper")
        {
            SetRect(0, 14, 32, 4, darkGun);
            SetRect(0, 15, 32, 2, metal);
            SetRect(14, 18, 8, 10, handle);
            SetRect(28, 12, 4, 6, new Color(0.25f, 0.4f, 0.25f));
        }
        else if (key == "weapon_rpg")
        {
            SetRect(0, 12, 30, 8, new Color(0.35f, 0.4f, 0.18f));
            SetRect(0, 14, 30, 4, new Color(0.28f, 0.35f, 0.12f));
            SetRect(20, 20, 8, 8, handle);
            SetRect(28, 10, 4, 10, metal);
        }
        else if (key == "weapon_knife")
        {
            SetRect(14, 4, 4, 18, new Color(0.5f, 0.5f, 0.55f));
            SetRect(10, 20, 12, 6, handle);
            SetRect(15, 4, 2, 16, metal);
        }
        else if (key == "weapon_fist")
        {
            SetRect(10, 8, 12, 14, new Color(0.55f, 0.4f, 0.35f));
            SetRect(8, 10, 4, 8, new Color(0.55f, 0.4f, 0.35f));
            SetRect(20, 10, 4, 8, new Color(0.55f, 0.4f, 0.35f));
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    private static Sprite CreateBuildingSprite(string key)
    {
        int width = 48;
        int height = 48;
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        var pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                pixels[y * width + x] = Color.clear;

        void SetPixel(int x, int y, Color c)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                pixels[y * width + x] = c;
        }

        void SetRect(int x, int y, int w, int h, Color c)
        {
            for (int py = y; py < y + h; py++)
                for (int px = x; px < x + w; px++)
                    SetPixel(px, py, c);
        }

        Color shadowColor = new Color(0.08f, 0.08f, 0.1f, 0.6f);

        if (key == "building_house")
        {
            SetRect(4, 8, 40, 36, new Color(0.32f, 0.25f, 0.2f));
            SetRect(4, 4, 40, 8, new Color(0.35f, 0.18f, 0.15f));
            SetRect(18, 28, 12, 16, new Color(0.12f, 0.1f, 0.08f));
            SetRect(8, 14, 8, 10, new Color(0.35f, 0.5f, 0.55f, 0.5f));
            SetRect(32, 14, 8, 10, new Color(0.35f, 0.5f, 0.55f, 0.5f));
            SetRect(6, 36, 8, 8, new Color(0.25f, 0.25f, 0.3f));
            SetRect(6, 34, 8, 4, shadowColor);
        }
        else if (key == "building_shop")
        {
            SetRect(2, 4, 44, 40, new Color(0.28f, 0.26f, 0.24f));
            SetRect(2, 0, 44, 8, new Color(0.2f, 0.18f, 0.16f));
            SetRect(4, 20, 40, 22, new Color(0.2f, 0.35f, 0.35f, 0.4f));
            SetRect(18, 8, 12, 8, new Color(0.45f, 0.38f, 0.22f));
            SetRect(2, 42, 44, 2, shadowColor);
        }
        else if (key == "building_pub")
        {
            SetRect(2, 6, 44, 38, new Color(0.25f, 0.18f, 0.15f));
            SetRect(2, 2, 44, 8, new Color(0.18f, 0.15f, 0.12f));
            SetRect(8, 24, 32, 18, new Color(0.12f, 0.1f, 0.08f));
            SetRect(2, 42, 44, 2, shadowColor);
            SetRect(36, 8, 6, 12, new Color(0.45f, 0.45f, 0.3f, 0.4f));
        }
        else if (key == "building_office")
        {
            SetRect(2, 2, 44, 42, new Color(0.25f, 0.28f, 0.32f));
            SetRect(2, 0, 44, 6, new Color(0.18f, 0.2f, 0.25f));
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    SetRect(6 + col * 10, 10 + row * 8, 8, 6, new Color(0.28f, 0.38f, 0.45f, 0.5f));
                }
            }
            SetRect(18, 30, 12, 14, new Color(0.1f, 0.1f, 0.15f));
        }
        else if (key == "building_warehouse")
        {
            SetRect(2, 4, 44, 40, new Color(0.26f, 0.26f, 0.24f));
            SetRect(2, 0, 44, 8, new Color(0.2f, 0.2f, 0.18f));
            SetRect(8, 20, 14, 20, new Color(0.12f, 0.12f, 0.12f));
            SetRect(28, 20, 14, 20, new Color(0.12f, 0.12f, 0.12f));
            SetRect(20, 30, 8, 10, new Color(0.1f, 0.1f, 0.1f));
            SetRect(2, 42, 44, 2, shadowColor);
        }
        else if (key == "building_church")
        {
            SetRect(10, 10, 28, 34, new Color(0.32f, 0.32f, 0.35f));
            SetRect(14, 6, 20, 8, new Color(0.22f, 0.22f, 0.25f));
            SetRect(20, 0, 8, 10, new Color(0.25f, 0.25f, 0.3f));
            SetRect(18, 30, 12, 14, new Color(0.2f, 0.15f, 0.1f));
            SetRect(4, 40, 40, 4, new Color(0.32f, 0.32f, 0.35f));
        }
        else if (key == "building_hospital")
        {
            SetRect(2, 4, 44, 40, new Color(0.7f, 0.72f, 0.75f));
            SetRect(2, 0, 44, 8, new Color(0.58f, 0.62f, 0.65f));
            SetRect(18, 16, 12, 20, new Color(0.35f, 0.5f, 0.6f, 0.5f));
            SetRect(22, 36, 4, 8, new Color(0.15f, 0.15f, 0.15f));
            SetRect(4, 10, 6, 4, new Color(0.65f, 0.15f, 0.15f));
            SetRect(38, 10, 6, 4, new Color(0.65f, 0.15f, 0.15f));
        }
        else if (key == "building_police")
        {
            SetRect(2, 4, 44, 40, new Color(0.18f, 0.22f, 0.3f));
            SetRect(2, 0, 44, 8, new Color(0.12f, 0.18f, 0.25f));
            SetRect(18, 24, 12, 18, new Color(0.1f, 0.1f, 0.15f));
            SetRect(6, 10, 8, 8, new Color(0.22f, 0.38f, 0.52f, 0.5f));
            SetRect(34, 10, 8, 8, new Color(0.22f, 0.38f, 0.52f, 0.5f));
            SetRect(4, 44, 40, 2, shadowColor);
        }
        else if (key == "building_gym")
        {
            SetRect(2, 4, 44, 40, new Color(0.28f, 0.18f, 0.25f));
            SetRect(2, 0, 44, 8, new Color(0.2f, 0.12f, 0.2f));
            SetRect(8, 12, 10, 8, new Color(0.35f, 0.5f, 0.55f, 0.5f));
            SetRect(30, 12, 10, 8, new Color(0.35f, 0.5f, 0.55f, 0.5f));
            SetRect(18, 28, 12, 14, new Color(0.12f, 0.1f, 0.08f));
        }
        else if (key == "building_farm")
        {
            SetRect(4, 12, 40, 32, new Color(0.35f, 0.28f, 0.22f));
            SetRect(8, 8, 32, 8, new Color(0.28f, 0.18f, 0.1f));
            SetRect(16, 28, 16, 16, new Color(0.2f, 0.18f, 0.15f));
            SetRect(6, 40, 10, 4, new Color(0.45f, 0.25f, 0.15f));
        }
        else
        {
            SetRect(2, 4, 44, 40, new Color(0.28f, 0.26f, 0.24f));
            SetRect(4, 16, 12, 12, new Color(0.35f, 0.42f, 0.5f, 0.4f));
            SetRect(32, 16, 12, 12, new Color(0.35f, 0.42f, 0.5f, 0.4f));
            SetRect(20, 28, 8, 14, new Color(0.12f, 0.1f, 0.08f));
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), width);
    }

    private static Sprite CreateIconSprite(string key)
    {
        int size = 32;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                pixels[y * size + x] = Color.clear;

        void SetPixel(int x, int y, Color c)
        {
            if (x >= 0 && x < size && y >= 0 && y < size)
                pixels[y * size + x] = c;
        }

        void SetRect(int x, int y, int w, int h, Color c)
        {
            for (int py = y; py < y + h; py++)
                for (int px = x; px < x + w; px++)
                    SetPixel(px, py, c);
        }

        void SetCircle(int cx, int cy, int r, Color c)
        {
            for (int y = -r; y <= r; y++)
                for (int x = -r; x <= r; x++)
                    if (x * x + y * y <= r * r)
                        SetPixel(cx + x, cy + y, c);
        }

        switch (key)
        {
            case "icon_gunshop":
                SetCircle(16, 16, 12, new Color(0.8f, 0.2f, 0.2f, 0.9f));
                SetRect(12, 10, 8, 3, new Color(0.1f, 0.1f, 0.12f));
                SetRect(14, 13, 4, 6, new Color(0.15f, 0.12f, 0.1f));
                SetRect(10, 12, 12, 2, new Color(0.4f, 0.4f, 0.45f));
                break;

            case "icon_scrapyard":
                SetCircle(16, 16, 12, new Color(0.6f, 0.45f, 0.25f, 0.9f));
                SetRect(8, 14, 16, 2, new Color(0.3f, 0.25f, 0.2f));
                SetRect(10, 12, 2, 4, new Color(0.3f, 0.25f, 0.2f));
                SetRect(20, 12, 2, 4, new Color(0.3f, 0.25f, 0.2f));
                SetRect(12, 10, 8, 4, new Color(0.35f, 0.28f, 0.22f));
                break;

            case "icon_hospital":
                SetCircle(16, 16, 12, new Color(0.9f, 0.9f, 0.95f, 0.9f));
                SetRect(14, 8, 4, 16, Color.red);
                SetRect(8, 14, 16, 4, Color.red);
                break;

            case "icon_police":
                SetCircle(16, 16, 12, new Color(0.15f, 0.25f, 0.5f, 0.9f));
                SetRect(10, 8, 12, 6, new Color(0.1f, 0.15f, 0.25f));
                SetRect(12, 14, 8, 10, new Color(0.12f, 0.18f, 0.3f));
                break;

            case "icon_church":
                SetCircle(16, 16, 12, new Color(0.7f, 0.68f, 0.6f, 0.9f));
                SetRect(16, 6, 2, 16, new Color(0.5f, 0.5f, 0.55f));
                SetRect(10, 10, 12, 10, new Color(0.55f, 0.55f, 0.6f));
                SetRect(12, 12, 8, 6, new Color(0.35f, 0.35f, 0.4f));
                break;

            case "icon_clothes":
                SetCircle(16, 16, 12, new Color(0.6f, 0.35f, 0.6f, 0.9f));
                SetRect(8, 8, 16, 14, new Color(0.5f, 0.25f, 0.5f));
                SetRect(10, 6, 12, 4, new Color(0.55f, 0.3f, 0.55f));
                break;

            case "icon_barber":
                SetCircle(16, 16, 12, new Color(0.7f, 0.55f, 0.3f, 0.9f));
                SetRect(10, 8, 12, 14, new Color(0.55f, 0.4f, 0.2f));
                SetRect(12, 6, 8, 4, new Color(0.6f, 0.48f, 0.3f));
                break;

            case "icon_restaurant":
                SetCircle(16, 16, 12, new Color(0.8f, 0.5f, 0.2f, 0.9f));
                SetRect(8, 10, 16, 12, new Color(0.65f, 0.38f, 0.15f));
                SetRect(12, 8, 8, 4, new Color(0.7f, 0.45f, 0.18f));
                SetRect(14, 14, 4, 4, new Color(0.5f, 0.3f, 0.1f));
                break;

            case "icon_taxi":
                SetCircle(16, 16, 12, new Color(1f, 0.85f, 0f, 0.9f));
                SetRect(8, 12, 16, 8, new Color(0.15f, 0.12f, 0.1f));
                SetRect(12, 8, 8, 6, new Color(0.2f, 0.18f, 0.15f));
                break;

            case "icon_bus":
                SetCircle(16, 16, 12, new Color(0.2f, 0.5f, 0.8f, 0.9f));
                SetRect(6, 10, 20, 12, new Color(0.15f, 0.4f, 0.65f));
                SetRect(8, 12, 4, 4, new Color(0.5f, 0.7f, 0.9f));
                SetRect(14, 12, 4, 4, new Color(0.5f, 0.7f, 0.9f));
                SetRect(20, 12, 4, 4, new Color(0.5f, 0.7f, 0.9f));
                break;

            case "icon_train":
                SetCircle(16, 16, 12, new Color(0.4f, 0.35f, 0.3f, 0.9f));
                SetRect(4, 10, 24, 10, new Color(0.3f, 0.25f, 0.2f));
                SetRect(6, 8, 6, 6, new Color(0.35f, 0.32f, 0.28f));
                SetRect(20, 8, 6, 6, new Color(0.35f, 0.32f, 0.28f));
                break;

            case "icon_mission":
                SetCircle(16, 16, 12, new Color(1f, 0.8f, 0f, 0.9f));
                SetRect(12, 8, 8, 14, new Color(0.7f, 0.55f, 0f));
                SetRect(8, 12, 14, 8, new Color(0.7f, 0.55f, 0f));
                break;

            case "icon_navigation":
                SetCircle(16, 16, 12, new Color(0.2f, 0.6f, 1f, 0.9f));
                SetRect(16, 6, 2, 16, new Color(0.95f, 0.95f, 0.95f));
                SetCircle(16, 6, 3, new Color(0.1f, 0.4f, 0.8f));
                break;

            default:
                SetCircle(16, 16, 10, new Color(0.5f, 0.5f, 0.55f, 0.9f));
                break;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    private static Sprite CreateStreetSprite(string key)
    {
        int size = 24;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                pixels[y * size + x] = Color.clear;

        void SetPixel(int x, int y, Color c)
        {
            if (x >= 0 && x < size && y >= 0 && y < size)
                pixels[y * size + x] = c;
        }

        void SetRect(int x, int y, int w, int h, Color c)
        {
            for (int py = y; py < y + h; py++)
                for (int px = x; px < x + w; px++)
                    SetPixel(px, py, c);
        }

        if (key == "street_manhole")
        {
            SetRect(6, 6, 12, 12, new Color(0.15f, 0.15f, 0.18f));
            SetRect(8, 8, 8, 8, new Color(0.2f, 0.2f, 0.22f));
            SetRect(10, 10, 4, 4, new Color(0.1f, 0.1f, 0.12f));
        }
        else if (key == "street_crosswalk")
        {
            for (int i = 0; i < 5; i++)
            {
                SetRect(2 + i * 4, 0, 3, 24, Color.white);
            }
        }
        else if (key == "street_arrow")
        {
            SetRect(8, 4, 8, 8, Color.yellow);
            SetRect(4, 10, 16, 4, Color.yellow);
            SetRect(8, 14, 4, 8, Color.yellow);
            SetRect(12, 14, 4, 8, Color.yellow);
        }
        else if (key == "street_stopping")
        {
            SetRect(2, 8, 20, 8, Color.white);
        }
        else if (key == "street_drain")
        {
            SetRect(8, 8, 8, 8, new Color(0.2f, 0.2f, 0.22f));
            SetRect(10, 10, 4, 4, new Color(0.15f, 0.15f, 0.17f));
        }
        else
        {
            SetRect(0, 0, size, size, new Color(0.3f, 0.3f, 0.32f));
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    private static Sprite CreatePickupSprite(string key)
    {
        int size = 24;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                pixels[y * size + x] = Color.clear;

        void SetPixel(int x, int y, Color c)
        {
            if (x >= 0 && x < size && y >= 0 && y < size)
                pixels[y * size + x] = c;
        }

        void SetRect(int x, int y, int w, int h, Color c)
        {
            for (int py = y; py < y + h; py++)
                for (int px = x; px < x + w; px++)
                    SetPixel(px, py, c);
        }

        if (key == "pickup_cash")
        {
            SetRect(4, 6, 16, 12, new Color(0.15f, 0.45f, 0.15f));
            SetRect(6, 8, 12, 8, new Color(0.1f, 0.35f, 0.1f));
            SetRect(8, 10, 8, 4, new Color(0.08f, 0.28f, 0.08f));
        }
        else if (key == "pickup_burger")
        {
            SetRect(4, 8, 16, 8, new Color(0.6f, 0.38f, 0.15f));
            SetRect(6, 6, 12, 4, new Color(0.7f, 0.45f, 0.22f));
            SetRect(4, 14, 16, 4, new Color(0.45f, 0.22f, 0.08f));
        }
        else if (key == "pickup_pizza")
        {
            SetRect(6, 6, 12, 12, new Color(0.7f, 0.55f, 0.32f));
            SetRect(8, 8, 8, 8, new Color(0.6f, 0.22f, 0.08f));
            SetRect(10, 10, 4, 4, new Color(0.7f, 0.15f, 0.08f));
        }
        else if (key == "pickup_hotdog")
        {
            SetRect(4, 10, 16, 6, new Color(0.75f, 0.55f, 0.4f));
            SetRect(6, 8, 12, 4, new Color(0.6f, 0.35f, 0.15f));
            SetRect(4, 12, 16, 2, new Color(0.55f, 0.25f, 0.1f));
        }
        else if (key == "pickup_sandwich")
        {
            SetRect(4, 8, 16, 10, new Color(0.65f, 0.58f, 0.42f));
            SetRect(6, 10, 12, 3, new Color(0.4f, 0.6f, 0.25f));
            SetRect(6, 14, 12, 3, new Color(0.6f, 0.35f, 0.2f));
        }
        else if (key == "pickup_crisps")
        {
            SetRect(6, 4, 12, 16, new Color(0.75f, 0.75f, 0.2f));
            SetRect(8, 6, 8, 12, new Color(0.6f, 0.6f, 0.15f));
        }
        else if (key == "pickup_chocolate")
        {
            SetRect(4, 6, 16, 12, new Color(0.35f, 0.22f, 0.08f));
            SetRect(6, 8, 12, 8, new Color(0.45f, 0.28f, 0.12f));
        }
        else if (key == "pickup_soda")
        {
            SetRect(6, 4, 12, 16, new Color(0.7f, 0.15f, 0.15f));
            SetRect(8, 6, 8, 4, new Color(0.5f, 0.5f, 0.5f));
        }
        else if (key == "pickup_coffee")
        {
            SetRect(6, 6, 12, 14, new Color(0.45f, 0.3f, 0.15f));
            SetRect(8, 8, 8, 10, new Color(0.3f, 0.18f, 0.08f));
            SetRect(4, 10, 4, 6, new Color(0.4f, 0.25f, 0.12f));
        }
        else if (key == "pickup_icecream")
        {
            SetRect(8, 4, 8, 14, new Color(0.75f, 0.68f, 0.75f));
            SetRect(10, 16, 4, 4, new Color(0.55f, 0.45f, 0.35f));
        }
        else if (key == "pickup_fries")
        {
            SetRect(6, 8, 12, 12, new Color(0.75f, 0.68f, 0.22f));
            SetRect(8, 4, 2, 6, new Color(0.7f, 0.62f, 0.18f));
            SetRect(11, 4, 2, 6, new Color(0.7f, 0.62f, 0.18f));
            SetRect(14, 4, 2, 6, new Color(0.7f, 0.62f, 0.18f));
        }
        else if (key == "pickup_grenade")
        {
            SetRect(8, 4, 8, 16, new Color(0.22f, 0.26f, 0.22f));
            SetRect(6, 4, 12, 4, new Color(0.38f, 0.3f, 0.15f));
            SetRect(10, 16, 4, 4, new Color(0.3f, 0.26f, 0.15f));
        }
        else if (key == "pickup_ammo")
        {
            SetRect(4, 6, 6, 12, new Color(0.45f, 0.38f, 0.22f));
            SetRect(14, 6, 6, 12, new Color(0.45f, 0.38f, 0.22f));
            SetRect(8, 4, 8, 16, new Color(0.52f, 0.45f, 0.3f));
        }
        else if (key == "pickup_health")
        {
            SetRect(10, 4, 4, 16, new Color(0.6f, 0.15f, 0.15f));
            SetRect(4, 10, 16, 4, new Color(0.6f, 0.15f, 0.15f));
            SetRect(11, 5, 2, 14, new Color(0.75f, 0.3f, 0.3f));
            SetRect(5, 11, 14, 2, new Color(0.75f, 0.3f, 0.3f));
        }
        else
        {
            SetRect(4, 4, 16, 16, new Color(0.35f, 0.35f, 0.38f));
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    private static Sprite CreatePlaceholderSprite(string key)
    {
        int size = 32;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color[size * size];

        Color color = new Color(0.4f, 0.4f, 0.45f);
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                pixels[y * size + x] = color;

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    public static Sprite GetWeaponSprite(Gun.GunType type)
    {
        string key = type switch
        {
            Gun.GunType.Pistol => "weapon_pistol",
            Gun.GunType.SMG => "weapon_smg",
            Gun.GunType.Shotgun => "weapon_shotgun",
            Gun.GunType.AssaultRifle => "weapon_assault",
            Gun.GunType.SniperRifle => "weapon_sniper",
            Gun.GunType.Revolver => "weapon_revolver",
            Gun.GunType.RPG => "weapon_rpg",
            _ => "weapon_fist"
        };
        return GetSprite(key);
    }

    public static Sprite GetPickupSprite(string type)
    {
        return GetSprite($"pickup_{type}");
    }

    public static Sprite GetStreetSprite(string type)
    {
        return GetSprite($"street_{type}");
    }
}
