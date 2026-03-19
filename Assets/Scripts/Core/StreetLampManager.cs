using UnityEngine;
using System.Collections.Generic;

public class StreetLampManager : MonoBehaviour
{
    public static StreetLampManager Instance { get; private set; }

    [Header("Street Lamp Configuration")]
    public GameObject lampPrefab;
    public float lightRadius = 15f;
    public Color dayColor = new Color(1f, 0.95f, 0.8f, 0.3f);
    public Color nightColor = new Color(1f, 0.9f, 0.7f, 1f);

    private List<StreetLamp> lamps = new List<StreetLamp>();
    private bool isNight = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        CreateStreetLamps();
    }

    private void Update()
    {
        if (DayNightCycleManager.Instance != null)
        {
            bool nowNight = DayNightCycleManager.Instance.CurrentTimeOfDay == DayNightCycleManager.TimeOfDay.Night ||
                           DayNightCycleManager.Instance.CurrentTimeOfDay == DayNightCycleManager.TimeOfDay.Dusk;

            if (nowNight != isNight)
            {
                isNight = nowNight;
                UpdateLampStates();
            }
        }
    }

    private void CreateStreetLamps()
    {
        Vector2[] lampPositions = GetWolverhamptonLampPositions();

        for (int i = 0; i < lampPositions.Length; i++)
        {
            CreateLamp(i, lampPositions[i]);
        }

        Debug.Log($"Created {lamps.Count} street lamps in Wolverhampton");
    }

    private Vector2[] GetWolverhamptonLampPositions()
    {
        List<Vector2> positions = new List<Vector2>();

        for (float x = 200f; x <= 1000f; x += 40f)
        {
            positions.Add(new Vector2(x, 200f));
            positions.Add(new Vector2(x, 400f));
            positions.Add(new Vector2(x, 600f));
            positions.Add(new Vector2(x, 800f));
        }

        for (float y = 250f; y <= 850f; y += 40f)
        {
            positions.Add(new Vector2(200f, y));
            positions.Add(new Vector2(400f, y));
            positions.Add(new Vector2(600f, y));
            positions.Add(new Vector2(800f, y));
            positions.Add(new Vector2(1000f, y));
        }

        Vector2[] cityCentreLamps = new Vector2[]
        {
            new Vector2(350f, 450f), new Vector2(370f, 450f), new Vector2(390f, 450f),
            new Vector2(350f, 470f), new Vector2(370f, 470f), new Vector2(390f, 470f),
            new Vector2(350f, 490f), new Vector2(370f, 490f), new Vector2(390f, 490f),
            new Vector2(410f, 450f), new Vector2(430f, 450f), new Vector2(450f, 450f),
            new Vector2(410f, 470f), new Vector2(430f, 470f), new Vector2(450f, 470f),
            new Vector2(410f, 490f), new Vector2(430f, 490f), new Vector2(450f, 490f),
            
            new Vector2(320f, 520f), new Vector2(340f, 520f), new Vector2(360f, 520f),
            new Vector2(380f, 520f), new Vector2(400f, 520f), new Vector2(420f, 520f),
            new Vector2(440f, 520f), new Vector2(460f, 520f), new Vector2(480f, 520f),
            new Vector2(500f, 520f), new Vector2(520f, 520f),
            
            new Vector2(450f, 380f), new Vector2(470f, 380f), new Vector2(490f, 380f),
            new Vector2(510f, 380f), new Vector2(530f, 380f),
            new Vector2(450f, 400f), new Vector2(470f, 400f), new Vector2(490f, 400f),
            new Vector2(510f, 400f), new Vector2(530f, 400f),
            
            new Vector2(550f, 450f), new Vector2(570f, 450f), new Vector2(590f, 450f),
            new Vector2(550f, 470f), new Vector2(570f, 470f), new Vector2(590f, 470f),
            new Vector2(550f, 490f), new Vector2(570f, 490f), new Vector2(590f, 490f),
            
            new Vector2(300f, 550f), new Vector2(320f, 550f), new Vector2(340f, 550f),
            new Vector2(360f, 550f), new Vector2(380f, 550f), new Vector2(400f, 550f),
            new Vector2(420f, 550f), new Vector2(440f, 550f), new Vector2(460f, 550f),
            new Vector2(480f, 550f), new Vector2(500f, 550f), new Vector2(520f, 550f),
            
            new Vector2(350f, 600f), new Vector2(370f, 600f), new Vector2(390f, 600f),
            new Vector2(410f, 600f), new Vector2(430f, 600f), new Vector2(450f, 600f),
            new Vector2(470f, 600f), new Vector2(490f, 600f), new Vector2(510f, 600f),
            
            new Vector2(280f, 650f), new Vector2(300f, 650f), new Vector2(320f, 650f),
            new Vector2(340f, 650f), new Vector2(360f, 650f), new Vector2(380f, 650f),
            new Vector2(400f, 650f), new Vector2(420f, 650f), new Vector2(440f, 650f),
            new Vector2(460f, 650f), new Vector2(480f, 650f), new Vector2(500f, 650f),
            
            new Vector2(350f, 700f), new Vector2(370f, 700f), new Vector2(390f, 700f),
            new Vector2(410f, 700f), new Vector2(430f, 700f), new Vector2(450f, 700f),
            new Vector2(470f, 700f), new Vector2(490f, 700f),
            
            new Vector2(300f, 750f), new Vector2(320f, 750f), new Vector2(340f, 750f),
            new Vector2(360f, 750f), new Vector2(380f, 750f), new Vector2(400f, 750f),
            new Vector2(420f, 750f), new Vector2(440f, 750f), new Vector2(460f, 750f),
            new Vector2(480f, 750f), new Vector2(500f, 750f), new Vector2(520f, 750f),
            
            new Vector2(550f, 550f), new Vector2(570f, 550f), new Vector2(590f, 550f),
            new Vector2(610f, 550f), new Vector2(630f, 550f), new Vector2(650f, 550f),
            new Vector2(670f, 550f), new Vector2(690f, 550f),
            
            new Vector2(600f, 600f), new Vector2(620f, 600f), new Vector2(640f, 600f),
            new Vector2(660f, 600f), new Vector2(680f, 600f), new Vector2(700f, 600f),
            new Vector2(720f, 600f), new Vector2(740f, 600f),
            
            new Vector2(650f, 650f), new Vector2(670f, 650f), new Vector2(690f, 650f),
            new Vector2(710f, 650f), new Vector2(730f, 650f), new Vector2(750f, 650f),
            new Vector2(770f, 650f), new Vector2(790f, 650f),
            
            new Vector2(700f, 700f), new Vector2(720f, 700f), new Vector2(740f, 700f),
            new Vector2(760f, 700f), new Vector2(780f, 700f), new Vector2(800f, 700f),
            
            new Vector2(750f, 500f), new Vector2(770f, 500f), new Vector2(790f, 500f),
            new Vector2(810f, 500f), new Vector2(830f, 500f), new Vector2(850f, 500f),
            new Vector2(870f, 500f), new Vector2(890f, 500f),
            
            new Vector2(800f, 450f), new Vector2(820f, 450f), new Vector2(840f, 450f),
            new Vector2(860f, 450f), new Vector2(880f, 450f), new Vector2(900f, 450f),
            
            new Vector2(850f, 400f), new Vector2(870f, 400f), new Vector2(890f, 400f),
            new Vector2(910f, 400f), new Vector2(930f, 400f), new Vector2(950f, 400f),
        };
        
        positions.AddRange(cityCentreLamps);

        Vector2[] ringRoadLamps = new Vector2[]
        {
            new Vector2(250f, 350f), new Vector2(250f, 370f), new Vector2(250f, 390f),
            new Vector2(250f, 410f), new Vector2(250f, 430f),
            new Vector2(650f, 350f), new Vector2(650f, 370f), new Vector2(650f, 390f),
            new Vector2(650f, 410f), new Vector2(650f, 430f),
            new Vector2(350f, 250f), new Vector2(370f, 250f), new Vector2(390f, 250f),
            new Vector2(410f, 250f), new Vector2(430f, 250f),
            new Vector2(550f, 250f), new Vector2(570f, 250f), new Vector2(590f, 250f),
            new Vector2(610f, 250f), new Vector2(630f, 250f),
            new Vector2(350f, 750f), new Vector2(370f, 750f), new Vector2(390f, 750f),
            new Vector2(410f, 750f), new Vector2(430f, 750f),
            new Vector2(550f, 750f), new Vector2(570f, 750f), new Vector2(590f, 750f),
            new Vector2(610f, 750f), new Vector2(630f, 750f),
        };
        
        positions.AddRange(ringRoadLamps);

        Vector2[] bilstonArea = new Vector2[]
        {
            new Vector2(200f, 300f), new Vector2(220f, 300f), new Vector2(240f, 300f),
            new Vector2(200f, 320f), new Vector2(220f, 320f), new Vector2(240f, 320f),
            new Vector2(180f, 280f), new Vector2(200f, 280f), new Vector2(220f, 280f),
            new Vector2(160f, 260f), new Vector2(180f, 260f), new Vector2(200f, 260f),
        };
        
        positions.AddRange(bilstonArea);

        Vector2[] wednesfieldArea = new Vector2[]
        {
            new Vector2(850f, 600f), new Vector2(870f, 600f), new Vector2(890f, 600f),
            new Vector2(910f, 600f), new Vector2(930f, 600f),
            new Vector2(850f, 620f), new Vector2(870f, 620f), new Vector2(890f, 620f),
            new Vector2(910f, 620f), new Vector2(930f, 620f),
            new Vector2(870f, 580f), new Vector2(890f, 580f), new Vector2(910f, 580f),
        };
        
        positions.AddRange(wednesfieldArea);

        Vector2[] tettenhallArea = new Vector2[]
        {
            new Vector2(750f, 750f), new Vector2(770f, 750f), new Vector2(790f, 750f),
            new Vector2(810f, 750f),
            new Vector2(750f, 770f), new Vector2(770f, 770f), new Vector2(790f, 770f),
            new Vector2(810f, 770f),
            new Vector2(760f, 730f), new Vector2(780f, 730f), new Vector2(800f, 730f),
        };
        
        positions.AddRange(tettenhallArea);

        Vector2[] parkAreas = new Vector2[]
        {
            new Vector2(400f, 350f), new Vector2(420f, 350f), new Vector2(440f, 350f),
            new Vector2(400f, 370f), new Vector2(420f, 370f), new Vector2(440f, 370f),
            new Vector2(400f, 550f), new Vector2(420f, 550f), new Vector2(440f, 550f),
            new Vector2(400f, 570f), new Vector2(420f, 570f), new Vector2(440f, 570f),
            new Vector2(500f, 650f), new Vector2(520f, 650f),
            new Vector2(650f, 700f), new Vector2(670f, 700f),
        };
        
        positions.AddRange(parkAreas);

        Vector2[] stationArea = new Vector2[]
        {
            new Vector2(500f, 420f), new Vector2(520f, 420f), new Vector2(540f, 420f),
            new Vector2(560f, 420f), new Vector2(580f, 420f),
            new Vector2(500f, 440f), new Vector2(520f, 440f), new Vector2(540f, 440f),
            new Vector2(560f, 440f), new Vector2(580f, 440f),
            new Vector2(510f, 400f), new Vector2(530f, 400f), new Vector2(550f, 400f),
        };
        
        positions.AddRange(stationArea);

        Vector2[] retailParks = new Vector2[]
        {
            new Vector2(650f, 300f), new Vector2(670f, 300f), new Vector2(690f, 300f),
            new Vector2(710f, 300f), new Vector2(730f, 300f),
            new Vector2(650f, 320f), new Vector2(670f, 320f), new Vector2(690f, 320f),
            new Vector2(710f, 320f), new Vector2(730f, 320f),
            new Vector2(200f, 550f), new Vector2(220f, 550f), new Vector2(240f, 550f),
            new Vector2(200f, 570f), new Vector2(220f, 570f), new Vector2(240f, 570f),
            new Vector2(900f, 300f), new Vector2(920f, 300f), new Vector2(940f, 300f),
            new Vector2(900f, 320f), new Vector2(920f, 320f), new Vector2(940f, 320f),
        };
        
        positions.AddRange(retailParks);

        return positions.ToArray();
    }

    private void CreateLamp(int index, Vector2 position)
    {
        GameObject lampGO = new GameObject($"StreetLamp_{index}");
        lampGO.transform.position = (Vector3)position;
        lampGO.transform.parent = transform;

        SpriteRenderer pole = lampGO.AddComponent<SpriteRenderer>();
        pole.sprite = SpriteHelper.GetDefaultSprite();
        pole.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        pole.sortingOrder = 1;
        pole.transform.localScale = new Vector3(2f, 15f, 1f);
        pole.transform.localPosition = new Vector3(0f, -7.5f, 0f);

        GameObject lightGO = new GameObject("LampLight");
        lightGO.transform.parent = lampGO.transform;
        lightGO.transform.localPosition = new Vector3(0f, 8f, 0f);

        SpriteRenderer lightSr = lightGO.AddComponent<SpriteRenderer>();
        lightSr.sprite = SpriteHelper.GetDefaultSprite();
        lightSr.color = nightColor;
        lightSr.sortingOrder = 3;
        lightSr.transform.localScale = new Vector3(lightRadius, lightRadius, 1f);

        StreetLamp lamp = lampGO.AddComponent<StreetLamp>();
        lamp.Initialize(lightSr, isNight);
        
        lamps.Add(lamp);
    }

    private void UpdateLampStates()
    {
        foreach (var lamp in lamps)
        {
            lamp.SetNightMode(isNight);
        }
    }
}

public class StreetLamp : MonoBehaviour
{
    private SpriteRenderer lightRenderer;
    private bool isNight;
    private Color nightColor = new Color(1f, 0.9f, 0.7f, 1f);
    private Color dayColor = new Color(1f, 0.95f, 0.8f, 0.05f);

    public void Initialize(SpriteRenderer lightRenderer, bool isNight)
    {
        this.lightRenderer = lightRenderer;
        this.isNight = isNight;
        UpdateVisual();
    }

    public void SetNightMode(bool night)
    {
        isNight = night;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (lightRenderer != null)
        {
            lightRenderer.color = isNight ? nightColor : dayColor;
            lightRenderer.gameObject.SetActive(true);
        }
    }
}
