using UnityEngine;
using System.Collections.Generic;

public class TrafficLightManager : MonoBehaviour
{
    public static TrafficLightManager Instance { get; private set; }

    private List<TrafficLight> trafficLights = new List<TrafficLight>();

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
        InitializeTrafficLights();
    }

    private void InitializeTrafficLights()
    {
        CreateTrafficLight("City Centre North", new Vector2(500, 520), 15f);
        CreateTrafficLight("City Centre South", new Vector2(500, 480), 15f);
        CreateTrafficLight("City Centre East", new Vector2(540, 500), 12f);
        CreateTrafficLight("City Centre West", new Vector2(460, 500), 12f);

        CreateTrafficLight("Ring Road North", new Vector2(480, 540), 18f);
        CreateTrafficLight("Ring Road South", new Vector2(520, 500), 18f);
        CreateTrafficLight("Ring Road East", new Vector2(500, 560), 15f);
        CreateTrafficLight("Ring Road West", new Vector2(470, 470), 15f);

        CreateTrafficLight("Bilston Road", new Vector2(320, 380), 20f);
        CreateTrafficLight("Dudley Road", new Vector2(360, 360), 18f);
        CreateTrafficLight("Wolverhampton Road", new Vector2(350, 340), 15f);

        CreateTrafficLight("Wednesfield North", new Vector2(700, 420), 15f);
        CreateTrafficLight("Wednesfield South", new Vector2(680, 380), 15f);

        CreateTrafficLight("Tettenhall Road", new Vector2(620, 700), 12f);
        CreateTrafficLight("Pendeford", new Vector2(750, 600), 15f);

        CreateTrafficLight("Bushbury", new Vector2(200, 640), 12f);
        CreateTrafficLight("Low Hill", new Vector2(400, 640), 15f);
        CreateTrafficLight("Heath Town", new Vector2(540, 740), 12f);

        CreateTrafficLight("Station Approach", new Vector2(470, 540), 10f);
        CreateTrafficLight("Railway Drive", new Vector2(490, 530), 10f);

        CreateTrafficLight("St George's", new Vector2(520, 470), 12f);
        
        CreateTrafficLight("Junction 100-200", new Vector2(100, 200), 12f);
        CreateTrafficLight("Junction 100-400", new Vector2(100, 400), 12f);
        CreateTrafficLight("Junction 100-600", new Vector2(100, 600), 12f);
        CreateTrafficLight("Junction 100-800", new Vector2(100, 800), 12f);
        
        CreateTrafficLight("Junction 200-200", new Vector2(200, 200), 12f);
        CreateTrafficLight("Junction 200-400", new Vector2(200, 400), 12f);
        CreateTrafficLight("Junction 200-600", new Vector2(200, 600), 12f);
        CreateTrafficLight("Junction 200-800", new Vector2(200, 800), 12f);
        
        CreateTrafficLight("Junction 400-200", new Vector2(400, 200), 12f);
        CreateTrafficLight("Junction 400-400", new Vector2(400, 400), 14f);
        CreateTrafficLight("Junction 400-600", new Vector2(400, 600), 12f);
        CreateTrafficLight("Junction 400-800", new Vector2(400, 800), 12f);
        
        CreateTrafficLight("Junction 600-200", new Vector2(600, 200), 12f);
        CreateTrafficLight("Junction 600-400", new Vector2(600, 400), 14f);
        CreateTrafficLight("Junction 600-600", new Vector2(600, 600), 12f);
        CreateTrafficLight("Junction 600-800", new Vector2(600, 800), 12f);
        
        CreateTrafficLight("Junction 800-200", new Vector2(800, 200), 12f);
        CreateTrafficLight("Junction 800-400", new Vector2(800, 400), 12f);
        CreateTrafficLight("Junction 800-600", new Vector2(800, 600), 12f);
        CreateTrafficLight("Junction 800-800", new Vector2(800, 800), 12f);
        
        CreateTrafficLight("Major 300-500", new Vector2(300, 500), 14f);
        CreateTrafficLight("Major 500-300", new Vector2(500, 300), 14f);
        CreateTrafficLight("Major 500-700", new Vector2(500, 700), 14f);
        CreateTrafficLight("Major 700-500", new Vector2(700, 500), 14f);
        
        CreateTrafficLight("Bilston Central", new Vector2(280, 340), 15f);
        CreateTrafficLight("Bilston East", new Vector2(300, 350), 12f);
        CreateTrafficLight("Ettingshall", new Vector2(370, 380), 12f);
        
        CreateTrafficLight("Ring Top", new Vector2(500, 820), 18f);
        CreateTrafficLight("Ring Bottom", new Vector2(500, 150), 18f);
        CreateTrafficLight("Ring Left", new Vector2(100, 500), 18f);
        CreateTrafficLight("Ring Right", new Vector2(900, 500), 18f);

        Debug.Log($"Initialized {trafficLights.Count} traffic lights");
    }

    private void CreateTrafficLight(string name, Vector2 position, float cycleTime)
    {
        GameObject lightGO = new GameObject($"TrafficLight_{name}");
        lightGO.transform.position = (Vector3)position;

        TrafficLight trafficLight = lightGO.AddComponent<TrafficLight>();
        trafficLight.Initialize(name, position, cycleTime);

        lightGO.transform.parent = transform;
        trafficLights.Add(trafficLight);
    }

    public List<TrafficLight> GetAllTrafficLights() => trafficLights;
}

public class TrafficLight : MonoBehaviour
{
    public string LightName { get; private set; }
    public Vector2 Position { get; private set; }

    public enum LightState { Red, Green, Yellow }
    private LightState currentState = LightState.Red;

    private float cycleTime = 15f;
    private float timer = 0f;

    private GameObject redLight;
    private GameObject yellowLight;
    private GameObject greenLight;

    public void Initialize(string name, Vector2 position, float cycle)
    {
        LightName = name;
        Position = position;
        cycleTime = cycle;
        timer = 0f;

        CreateLightVisuals();
        SetState(LightState.Green);
    }

    private void CreateLightVisuals()
    {
        redLight = CreateLightSphere("Red", new Color(1f, 0f, 0f), Vector3.up * 2f);
        yellowLight = CreateLightSphere("Yellow", new Color(1f, 1f, 0f), Vector3.zero);
        greenLight = CreateLightSphere("Green", new Color(0f, 1f, 0f), Vector3.down * 2f);
    }

    private GameObject CreateLightSphere(string name, Color color, Vector3 offset)
    {
        GameObject lightGO = new GameObject(name);
        lightGO.transform.position = transform.position + offset;
        lightGO.transform.parent = transform;

        SpriteRenderer sr = lightGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(color.r, color.g, color.b, 0.3f);
        sr.sortingOrder = 10;
        lightGO.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

        return lightGO;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= cycleTime)
        {
            timer = 0f;
            CycleLights();
        }
    }

    private void CycleLights()
    {
        switch (currentState)
        {
            case LightState.Green:
                SetState(LightState.Yellow);
                break;
            case LightState.Yellow:
                SetState(LightState.Red);
                break;
            case LightState.Red:
                SetState(LightState.Green);
                break;
        }
    }

    private void SetState(LightState newState)
    {
        currentState = newState;

        if (redLight != null) redLight.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, currentState == LightState.Red ? 1f : 0.3f);
        if (yellowLight != null) yellowLight.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0f, currentState == LightState.Yellow ? 1f : 0.3f);
        if (greenLight != null) greenLight.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, currentState == LightState.Green ? 1f : 0.3f);
    }

    public bool CanPass()
    {
        return currentState == LightState.Green;
    }
    
    public bool IsGreen()
    {
        return currentState == LightState.Green;
    }
    
    public bool IsRed()
    {
        return currentState == LightState.Red;
    }
    
    public bool IsYellow()
    {
        return currentState == LightState.Yellow;
    }

    public LightState GetState() => currentState;
}
