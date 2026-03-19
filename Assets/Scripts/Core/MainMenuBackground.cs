using UnityEngine;

public class MainMenuBackground : MonoBehaviour
{
    private GameObject sky;
    private GameObject grass;
    private GameObject buildingsContainer;
    private GameObject charactersContainer;
    private GameObject cloudsContainer;

    private void Start()
    {
        CreateBackground();
    }

    private void CreateBackground()
    {
        CreateSky();
        CreateClouds();
        CreateBuildings();
        CreateGrass();
        CreateCharacters();
    }

    private void CreateSky()
    {
        sky = new GameObject("Sky");
        sky.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 + 200, 10);
        
        SpriteRenderer sr = sky.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.sortingOrder = -100;
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(0.4f, 0.7f, 1f), 0.0f),
                new GradientColorKey(new Color(0.6f, 0.85f, 1f), 0.5f),
                new GradientColorKey(new Color(0.7f, 0.9f, 1f), 1.0f)
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );
        
        sr.color = gradient.Evaluate(0.5f);
        sky.transform.localScale = new Vector3(Screen.width, Screen.height * 1.5f, 1f);
    }

    private void CreateClouds()
    {
        cloudsContainer = new GameObject("Clouds");
        cloudsContainer.transform.parent = sky.transform;

        for (int i = 0; i < 8; i++)
        {
            CreateCloud(
                Random.Range(50, Screen.width - 50),
                Screen.height - Random.Range(30, 100)
            );
        }
    }

    private void CreateCloud(float x, float y)
    {
        GameObject cloud = new GameObject($"Cloud_{x}_{y}");
        cloud.transform.parent = cloudsContainer.transform;
        cloud.transform.position = new Vector3(x, y, -5);

        int numPuffs = Random.Range(3, 6);
        for (int i = 0; i < numPuffs; i++)
        {
            GameObject puff = new GameObject("Puff");
            puff.transform.parent = cloud.transform;
            puff.transform.localPosition = new Vector3(i * 30 - numPuffs * 15, Random.Range(-10, 10), 0);
            
            SpriteRenderer sr = puff.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.sortingOrder = -50;
            sr.color = new Color(1f, 1f, 1f, 0.9f);
            puff.transform.localScale = new Vector3(Random.Range(40, 80), Random.Range(25, 45), 1f);
        }
    }

    private void CreateBuildings()
    {
        buildingsContainer = new GameObject("Buildings");
        buildingsContainer.transform.position = new Vector3(0, 0, -2);

        float[] buildingPositions = new float[] { 50, 150, 250, 350, 450, 550, 650, 750, 850, 950 };
        
        foreach (float x in buildingPositions)
        {
            if (Random.value > 0.3f)
            {
                CreateBuilding(x, Screen.height * 0.35f);
            }
        }
    }

    private void CreateBuilding(float x, float baseY)
    {
        float height = Random.Range(80, 250);
        float width = Random.Range(40, 100);
        
        GameObject building = new GameObject($"Building_{x}");
        building.transform.position = new Vector3(x, baseY + height / 2, 0);

        Color buildingColor = GetRandomBuildingColor();
        
        GameObject mainBuilding = new GameObject("Main");
        mainBuilding.transform.parent = building.transform;
        mainBuilding.transform.localPosition = Vector3.zero;
        
        SpriteRenderer sr = mainBuilding.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.sortingOrder = -10;
        sr.color = buildingColor;
        mainBuilding.transform.localScale = new Vector3(width, height, 1f);

        CreateWindows(mainBuilding, width, height);

        if (Random.value > 0.5f && height > 100)
        {
            CreateRoof(mainBuilding, width, height);
        }
    }

    private Color GetRandomBuildingColor()
    {
        Color[] colors = new Color[]
        {
            new Color(0.6f, 0.5f, 0.4f),
            new Color(0.5f, 0.45f, 0.4f),
            new Color(0.7f, 0.6f, 0.5f),
            new Color(0.55f, 0.5f, 0.45f),
            new Color(0.65f, 0.55f, 0.45f),
            new Color(0.5f, 0.4f, 0.35f)
        };
        return colors[Random.Range(0, colors.Length)];
    }

    private void CreateWindows(GameObject parent, float width, float height)
    {
        int windowsX = (int)(width / 25);
        int windowsY = (int)(height / 30);

        for (int x = 0; x < windowsX; x++)
        {
            for (int y = 0; y < windowsY; y++)
            {
                if (Random.value > 0.3f)
                {
                    GameObject window = new GameObject("Window");
                    window.transform.parent = parent.transform;
                    window.transform.localPosition = new Vector3(
                        -width/2 + 15 + x * 22,
                        -height/2 + 15 + y * 28,
                        -0.1f
                    );
                    
                    SpriteRenderer sr = window.AddComponent<SpriteRenderer>();
                    sr.sprite = SpriteHelper.GetDefaultSprite();
                    sr.sortingOrder = 1;
                    sr.color = Random.value > 0.7f 
                        ? new Color(1f, 0.95f, 0.5f, 0.8f)
                        : new Color(0.3f, 0.35f, 0.45f, 0.6f);
                    window.transform.localScale = new Vector3(15, 20, 1f);
                }
            }
        }
    }

    private void CreateRoof(GameObject parent, float width, float height)
    {
        GameObject roof = new GameObject("Roof");
        roof.transform.parent = parent.transform;
        roof.transform.localPosition = new Vector3(0, height / 2 + 10, -0.1f);
        
        SpriteRenderer sr = roof.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.sortingOrder = 1;
        sr.color = new Color(0.4f, 0.25f, 0.2f, 1f);
        roof.transform.localScale = new Vector3(width + 10, 20, 1f);
    }

    private void CreateGrass()
    {
        grass = new GameObject("Grass");
        grass.transform.position = new Vector3(Screen.width / 2, 40, 1);
        
        SpriteRenderer sr = grass.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.sortingOrder = 10;
        sr.color = new Color(0.3f, 0.8f, 0.2f, 1f);
        grass.transform.localScale = new Vector3(Screen.width + 200, 120, 1f);

        CreateGrassBlades();
    }

    private void CreateGrassBlades()
    {
        for (int i = 0; i < 50; i++)
        {
            GameObject blade = new GameObject($"Blade_{i}");
            blade.transform.parent = grass.transform;
            blade.transform.position = new Vector3(
                Random.Range(0, Screen.width) - Screen.width/2,
                50 + Random.Range(0, 20),
                -0.1f
            );
            
            SpriteRenderer sr = blade.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteHelper.GetDefaultSprite();
            sr.sortingOrder = 11;
            sr.color = new Color(0.25f, 0.7f + Random.value * 0.2f, 0.15f, 1f);
            blade.transform.localScale = new Vector3(4, 15 + Random.Range(0, 15), 1f);
            blade.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-15, 15));
        }
    }

    private void CreateCharacters()
    {
        charactersContainer = new GameObject("MenuCharacters");
        charactersContainer.transform.position = new Vector3(Screen.width / 2, Screen.height - 180, -1);

        CreateMenuCharacter(MissionGiverType.Dave, -350, 0);
        CreateMenuCharacter(MissionGiverType.Crystal, -120, 10);
        CreateMenuCharacter(MissionGiverType.Lorna, 120, -5);
        CreateMenuCharacter(MissionGiverType.MickDick, 350, -10);
    }

    private void CreateMenuCharacter(MissionGiverType type, float xOffset, float yOffset)
    {
        GameObject charContainer = new GameObject($"MenuChar_{type}");
        charContainer.transform.parent = charactersContainer.transform;
        charContainer.transform.localPosition = new Vector3(xOffset, yOffset, 0);

        switch (type)
        {
            case MissionGiverType.Dave:
                CreateDaveCaricature(charContainer);
                break;
            case MissionGiverType.Crystal:
                CreateCrystalCaricature(charContainer);
                break;
            case MissionGiverType.Lorna:
                CreateLornaCaricature(charContainer);
                break;
            case MissionGiverType.MickDick:
                CreateMickDickCaricature(charContainer);
                break;
        }
    }

    private void CreateDaveCaricature(GameObject parent)
    {
        GameObject body = CreateSprite(parent, new Vector3(0, -30, 0), new Vector3(100, 140, 1), 
            new Color(0.3f, 0.2f, 0.15f), 5);
        
        GameObject head = CreateSprite(parent, new Vector3(0, 60, 0), new Vector3(80, 80, 1),
            new Color(0.9f, 0.75f, 0.65f), 6);

        GameObject hair = CreateSprite(parent, new Vector3(0, 75, -0.1f), new Vector3(85, 35, 1),
            new Color(0.2f, 0.1f, 0.05f), 7);

        GameObject beard = CreateSprite(parent, new Vector3(0, 35, -0.1f), new Vector3(70, 30, 1),
            new Color(0.2f, 0.1f, 0.05f), 7);

        GameObject moped = CreateSprite(parent, new Vector3(70, -50, 1), new Vector3(100, 50, 1),
            Color.white, 4);

        GameObject mopedWheel1 = CreateSprite(parent, new Vector3(45, -80, 0.5f), new Vector3(30, 30, 1),
            new Color(0.2f, 0.2f, 0.2f), 5);
        
        GameObject mopedWheel2 = CreateSprite(parent, new Vector3(95, -80, 0.5f), new Vector3(30, 30, 1),
            new Color(0.2f, 0.2f, 0.2f), 5);
    }

    private void CreateCrystalCaricature(GameObject parent)
    {
        GameObject body = CreateSprite(parent, new Vector3(0, -20, 0), new Vector3(60, 90, 1),
            new Color(0.9f, 0.2f, 0.3f), 5);

        GameObject head = CreateSprite(parent, new Vector3(0, 50, 0), new Vector3(55, 60, 1),
            new Color(0.85f, 0.7f, 0.55f), 6);

        GameObject hair = CreateSprite(parent, new Vector3(0, 70, -0.1f), new Vector3(80, 40, 1),
            new Color(0.9f, 0.6f, 0.1f), 7);

        GameObject lips = CreateSprite(parent, new Vector3(0, 35, -0.1f), new Vector3(25, 10, 1),
            new Color(0.8f, 0.1f, 0.2f), 8);

        GameObject eyes = CreateSprite(parent, new Vector3(0, 50, -0.1f), new Vector3(40, 12, 1),
            new Color(0.1f, 0.1f, 0.1f), 8);

        GameObject legs = CreateSprite(parent, new Vector3(-10, -80, 0), new Vector3(18, 60, 1),
            new Color(0.2f, 0.15f, 0.3f), 5);
        
        GameObject legs2 = CreateSprite(parent, new Vector3(10, -80, 0), new Vector3(18, 60, 1),
            new Color(0.2f, 0.15f, 0.3f), 5);
    }

    private void CreateLornaCaricature(GameObject parent)
    {
        GameObject body = CreateSprite(parent, new Vector3(0, -20, 0), new Vector3(65, 100, 1),
            new Color(0.2f, 0.3f, 0.5f), 5);

        GameObject head = CreateSprite(parent, new Vector3(0, 55, 0), new Vector3(55, 60, 1),
            new Color(0.8f, 0.7f, 0.6f), 6);

        GameObject hair = CreateSprite(parent, new Vector3(0, 75, -0.1f), new Vector3(50, 35, 1),
            new Color(0.3f, 0.2f, 0.15f), 7);

        GameObject eyeBags = CreateSprite(parent, new Vector3(0, 48, -0.1f), new Vector3(45, 12, 1),
            new Color(0.5f, 0.4f, 0.4f), 7);

        GameObject eyes = CreateSprite(parent, new Vector3(0, 52, -0.15f), new Vector3(40, 8, 1),
            new Color(0.4f, 0.3f, 0.3f), 8);

        GameObject hat = CreateSprite(parent, new Vector3(0, 85, -0.2f), new Vector3(60, 8, 1),
            new Color(0.1f, 0.1f, 0.1f), 9);

        GameObject badge = CreateSprite(parent, new Vector3(15, 10, -0.1f), new Vector3(15, 15, 1),
            new Color(1f, 0.8f, 0f), 8);
    }

    private void CreateMickDickCaricature(GameObject parent)
    {
        GameObject body = CreateSprite(parent, new Vector3(0, -25, 0), new Vector3(80, 120, 1),
            new Color(0.4f, 0.35f, 0.3f), 5);

        GameObject head = CreateSprite(parent, new Vector3(0, 55, 0), new Vector3(60, 65, 1),
            new Color(0.75f, 0.6f, 0.5f), 6);

        GameObject beard = CreateSprite(parent, new Vector3(0, 30, -0.1f), new Vector3(55, 40, 1),
            new Color(0.1f, 0.1f, 0.1f), 7);

        GameObject scar = CreateSprite(parent, new Vector3(18, 55, -0.15f), new Vector3(20, 5, 1),
            new Color(0.7f, 0.5f, 0.5f), 8);

        GameObject nose = CreateSprite(parent, new Vector3(0, 50, -0.1f), new Vector3(15, 15, 1),
            new Color(0.7f, 0.55f, 0.45f), 7);

        GameObject tattoo = CreateSprite(parent, new Vector3(-20, 20, -0.1f), new Vector3(20, 30, 1),
            new Color(0.2f, 0.4f, 0.5f, 0.8f), 7);

        GameObject apron = CreateSprite(parent, new Vector3(0, -10, -0.05f), new Vector3(60, 50, 1),
            new Color(0.3f, 0.3f, 0.35f), 6);
    }

    private GameObject CreateSprite(GameObject parent, Vector3 localPos, Vector3 scale, Color color, int sortingOrder)
    {
        GameObject obj = new GameObject("Sprite");
        obj.transform.parent = parent.transform;
        obj.transform.localPosition = localPos;
        
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = color;
        sr.sortingOrder = sortingOrder;
        obj.transform.localScale = scale;
        
        return obj;
    }
}
