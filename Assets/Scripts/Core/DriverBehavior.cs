using UnityEngine;
using System.Collections.Generic;

public enum DriverSkillLevel
{
    VerySlow,
    Slow,
    Normal,
    Fast,
    VeryFast,
    Erratic
}

public class DriverBehavior : MonoBehaviour
{
    public DriverSkillLevel skillLevel = DriverSkillLevel.Normal;
    public float baseSpeed = 10f;
    public float speedMultiplier = 1f;
    public bool isErratic = false;
    
    private float erraticTimer = 0f;
    private float erraticActionDuration = 0f;
    private Vector2 erraticDirection;
    private float normalSpeed;
    private bool wasCutOff = false;
    private float cutOffTimer = 0f;

    private void Start()
    {
        InitializeDriver();
    }

    private void InitializeDriver()
    {
        normalSpeed = baseSpeed;
        SetSkillLevel();
    }

    private void SetSkillLevel()
    {
        float roll = Random.value;
        
        if (roll < 0.08f)
        {
            skillLevel = DriverSkillLevel.VerySlow;
            speedMultiplier = Random.Range(0.35f, 0.5f);
        }
        else if (roll < 0.22f)
        {
            skillLevel = DriverSkillLevel.Slow;
            speedMultiplier = Random.Range(0.55f, 0.7f);
        }
        else if (roll < 0.68f)
        {
            skillLevel = DriverSkillLevel.Normal;
            speedMultiplier = Random.Range(0.8f, 1.1f);
        }
        else if (roll < 0.88f)
        {
            skillLevel = DriverSkillLevel.Fast;
            speedMultiplier = Random.Range(1.2f, 1.5f);
        }
        else if (roll < 0.96f)
        {
            skillLevel = DriverSkillLevel.VeryFast;
            speedMultiplier = Random.Range(1.6f, 2.0f);
        }
        else
        {
            skillLevel = DriverSkillLevel.Erratic;
            speedMultiplier = Random.Range(0.6f, 1.4f);
            isErratic = true;
        }
        
        baseSpeed *= speedMultiplier;
    }

    private void Update()
    {
        if (isErratic)
        {
            UpdateErraticBehavior();
        }
        
        if (wasCutOff)
        {
            cutOffTimer -= Time.deltaTime;
            if (cutOffTimer <= 0)
            {
                wasCutOff = false;
            }
        }
    }

    private void UpdateErraticBehavior()
    {
        erraticTimer -= Time.deltaTime;
        
        if (erraticTimer <= 0)
        {
            ChooseErraticAction();
        }
    }

    private void ChooseErraticAction()
    {
        erraticActionDuration = Random.Range(0.5f, 2f);
        
        float actionRoll = Random.value;
        
        if (actionRoll < 0.25f)
        {
            erraticDirection = Vector2.right;
        }
        else if (actionRoll < 0.5f)
        {
            erraticDirection = Vector2.left;
        }
        else if (actionRoll < 0.75f)
        {
            erraticDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f)).normalized;
        }
        else
        {
            erraticDirection = Vector2.zero;
        }
        
        erraticTimer = erraticActionDuration;
    }

    public Vector2 GetErraticDirection()
    {
        return erraticDirection;
    }

    public float GetModifiedSpeed()
    {
        float speed = baseSpeed;
        
        if (isErratic && erraticDirection != Vector2.zero)
        {
            speed *= Random.Range(0.5f, 1.5f);
        }
        
        if (wasCutOff)
        {
            speed *= 0.7f;
        }
        
        return speed;
    }

    public void OnCutOff()
    {
        wasCutOff = true;
        cutOffTimer = 2f;
        
        if (isErratic)
        {
            erraticTimer = Random.Range(1f, 3f);
            erraticDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }
    }

    public string GetDriverDescription()
    {
        return skillLevel switch
        {
            DriverSkillLevel.VerySlow => "Very Slow Driver",
            DriverSkillLevel.Slow => "Slow Driver",
            DriverSkillLevel.Normal => "Normal Driver",
            DriverSkillLevel.Fast => "Fast Driver",
            DriverSkillLevel.VeryFast => "Reckless Driver",
            DriverSkillLevel.Erratic => "Erratic Driver",
            _ => "Unknown"
        };
    }
}

public class TrafficDriverManager : MonoBehaviour
{
    public static TrafficDriverManager Instance { get; private set; }

    private List<DriverBehavior> activeDrivers = new List<DriverBehavior>();
    private int verySlowCount = 0;
    private int slowCount = 0;
    private int normalCount = 0;
    private int fastCount = 0;
    private int veryFastCount = 0;
    private int erraticCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        CountDrivers();
    }

    private void CountDrivers()
    {
        activeDrivers.Clear();
        verySlowCount = slowCount = normalCount = fastCount = veryFastCount = erraticCount = 0;
        
        var drivers = FindObjectsOfType<DriverBehavior>();
        foreach (var driver in drivers)
        {
            activeDrivers.Add(driver);
            
            switch (driver.skillLevel)
            {
                case DriverSkillLevel.VerySlow: verySlowCount++; break;
                case DriverSkillLevel.Slow: slowCount++; break;
                case DriverSkillLevel.Normal: normalCount++; break;
                case DriverSkillLevel.Fast: fastCount++; break;
                case DriverSkillLevel.VeryFast: veryFastCount++; break;
                case DriverSkillLevel.Erratic: erraticCount++; break;
            }
        }
    }

    public void RegisterDriver(DriverBehavior driver)
    {
        if (!activeDrivers.Contains(driver))
        {
            activeDrivers.Add(driver);
        }
    }

    public void OnDriverCutOff(DriverBehavior driver)
    {
        if (driver != null)
        {
            driver.OnCutOff();
        }
    }

    public void LogDriverStatistics()
    {
        Debug.Log($"=== Driver Statistics ===\n" +
            $"Very Slow: {verySlowCount}\n" +
            $"Slow: {slowCount}\n" +
            $"Normal: {normalCount}\n" +
            $"Fast: {fastCount}\n" +
            $"Very Fast: {veryFastCount}\n" +
            $"Erratic: {erraticCount}");
    }
}
