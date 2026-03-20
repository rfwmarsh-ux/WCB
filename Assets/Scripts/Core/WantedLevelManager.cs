using UnityEngine;
using System;
using System.Collections.Generic;

public class WantedLevelManager : MonoBehaviour
{
    public static WantedLevelManager Instance { get; private set; }

    public enum CrimeType
    {
        VehicleTheft,
        Assault,
        Murder,
        Robbery,
        HitAndRun,
        RecklessDriving,
        BreakingAndEntering,
        PoliceAssault,
        PoliceVehicleRamming,
        DestructionOfProperty,
        AttemptedVehicleTheft,
        VehicleTowing,
        AttemptedVehicleTowing,
        Trespassing
    }

    private Dictionary<CrimeType, int> crimeSeverity = new Dictionary<CrimeType, int>()
    {
        { CrimeType.VehicleTheft, 2 },
        { CrimeType.Assault, 1 },
        { CrimeType.Murder, 2 },
        { CrimeType.Robbery, 2 },
        { CrimeType.HitAndRun, 1 },
        { CrimeType.RecklessDriving, 1 },
        { CrimeType.BreakingAndEntering, 1 },
        { CrimeType.PoliceAssault, 3 },
        { CrimeType.PoliceVehicleRamming, 3 },
        { CrimeType.DestructionOfProperty, 1 },
        { CrimeType.AttemptedVehicleTheft, 1 },
        { CrimeType.VehicleTowing, 2 },
        { CrimeType.AttemptedVehicleTowing, 1 },
        { CrimeType.Trespassing, 1 }
    };

    private int player1WantedLevel = 0;
    private int player2WantedLevel = 0;

    private float player1WantedTimer = 0f;
    private float player2WantedTimer = 0f;

    private float wantedDecayTime = 30f;
    private float player1LastCrimeTime = 0f;
    private float player2LastCrimeTime = 0f;

    public int MaxWantedLevel { get; } = 5;
    
    public event Action<int, int> OnWantedLevelChanged;

    public int Player1WantedLevel => player1WantedLevel;
    public int Player2WantedLevel => player2WantedLevel;

    public int GetPlayerWantedLevel(int playerId)
    {
        return playerId == 1 ? player1WantedLevel : player2WantedLevel;
    }

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
        UpdateWantedDecay();
    }

    private void UpdateWantedDecay()
    {
        if (player1WantedLevel > 0 && Time.time - player1LastCrimeTime > wantedDecayTime)
        {
            player1WantedLevel = Mathf.Max(0, player1WantedLevel - 1);
            player1LastCrimeTime = Time.time;
            OnWantedLevelChanged?.Invoke(1, player1WantedLevel);
            Debug.Log($"Player 1 wanted level decreased to {player1WantedLevel}");
        }

        if (player2WantedLevel > 0 && Time.time - player2LastCrimeTime > wantedDecayTime)
        {
            player2WantedLevel = Mathf.Max(0, player2WantedLevel - 1);
            player2LastCrimeTime = Time.time;
            OnWantedLevelChanged?.Invoke(2, player2WantedLevel);
            Debug.Log($"Player 2 wanted level decreased to {player2WantedLevel}");
        }
    }

    public void IncreaseWantedLevel(int playerId, int amount = 1)
    {
        int oldLevel = playerId == 1 ? player1WantedLevel : player2WantedLevel;
        int currentLevel = oldLevel;
        
        if (currentLevel == 0)
        {
            amount = Mathf.Max(amount, 1);
        }
        
        if (playerId == 1)
        {
            player1WantedLevel = Mathf.Min(MaxWantedLevel, player1WantedLevel + amount);
            player1LastCrimeTime = Time.time;
            Debug.Log($"Player 1 wanted level increased to {player1WantedLevel}");
        }
        else
        {
            player2WantedLevel = Mathf.Min(MaxWantedLevel, player2WantedLevel + amount);
            player2LastCrimeTime = Time.time;
            Debug.Log($"Player 2 wanted level increased to {player2WantedLevel}");
        }
        
        int newLevel = playerId == 1 ? player1WantedLevel : player2WantedLevel;
        if (oldLevel != newLevel)
        {
            OnWantedLevelChanged?.Invoke(playerId, newLevel);
        }
    }

    public void ReportPoliceWitnessedCrime(int playerId, WantedLevelManager.CrimeType crime)
    {
        int currentLevel = playerId == 1 ? player1WantedLevel : player2WantedLevel;
        int severity = crimeSeverity.ContainsKey(crime) ? crimeSeverity[crime] : 1;
        
        if (currentLevel == 0)
        {
            severity = Mathf.Max(severity, 1);
        }
        
        IncreaseWantedLevel(playerId, severity);
        Debug.Log($"Police witnessed {crime}! Player {playerId} wanted level increased by {severity}");
    }

    public void DecreaseWantedLevel(int playerId, int amount = 1)
    {
        int oldLevel = playerId == 1 ? player1WantedLevel : player2WantedLevel;
        
        if (playerId == 1)
        {
            player1WantedLevel = Mathf.Max(0, player1WantedLevel - amount);
            Debug.Log($"Player 1 wanted level decreased to {player1WantedLevel}");
        }
        else
        {
            player2WantedLevel = Mathf.Max(0, player2WantedLevel - amount);
            Debug.Log($"Player 2 wanted level decreased to {player2WantedLevel}");
        }
        
        int newLevel = playerId == 1 ? player1WantedLevel : player2WantedLevel;
        if (oldLevel != newLevel)
        {
            OnWantedLevelChanged?.Invoke(playerId, newLevel);
        }
    }

    public void ClearWantedLevel(int playerId)
    {
        int oldLevel = playerId == 1 ? player1WantedLevel : player2WantedLevel;
        
        if (playerId == 1)
        {
            player1WantedLevel = 0;
            Debug.Log("Player 1 wanted level cleared");
        }
        else
        {
            player2WantedLevel = 0;
            Debug.Log("Player 2 wanted level cleared");
        }
        
        if (oldLevel != 0)
        {
            OnWantedLevelChanged?.Invoke(playerId, 0);
        }
    }

    public void SetWantedLevel(int playerId, int level)
    {
        int oldLevel = playerId == 1 ? player1WantedLevel : player2WantedLevel;
        
        if (playerId == 1)
        {
            player1WantedLevel = Mathf.Clamp(level, 0, MaxWantedLevel);
            player1LastCrimeTime = Time.time;
        }
        else
        {
            player2WantedLevel = Mathf.Clamp(level, 0, MaxWantedLevel);
            player2LastCrimeTime = Time.time;
        }
        
        Debug.Log($"Player {playerId} wanted level set to {level}");
        
        if (oldLevel != level)
        {
            int newLevel = playerId == 1 ? player1WantedLevel : player2WantedLevel;
            OnWantedLevelChanged?.Invoke(playerId, newLevel);
        }
    }

    public void ClearAllWantedLevels()
    {
        SetWantedLevel(1, 0);
        SetWantedLevel(2, 0);
    }

    public void ReportCrime(int playerId, CrimeType crime)
    {
        int severity = crimeSeverity.ContainsKey(crime) ? crimeSeverity[crime] : 1;
        IncreaseWantedLevel(playerId, severity);
        Debug.Log($"Player {playerId} committed {crime}. Wanted level increased by {severity}");
    }

    public int GetMaxWantedLevelInGame()
    {
        return Mathf.Max(player1WantedLevel, player2WantedLevel);
    }

    public Vector2 GetLastKnownPlayerPosition(int playerId)
    {
        if (playerId == 1)
        {
            PlayerManager pm = FindObjectOfType<PlayerManager>();
            return pm != null ? pm.transform.position : Vector2.zero;
        }
        else
        {
            Player2Manager p2m = Player2Manager.Instance;
            return p2m != null ? p2m.transform.position : Vector2.zero;
        }
    }
    
    public void ReportAttemptedTheft(int playerId)
    {
        ReportCrime(playerId, CrimeType.AttemptedVehicleTheft);
    }
    
    public void ReportSuccessfulTheft(int playerId)
    {
        IncreaseWantedLevel(playerId, 2);
    }
    
    public void ReportAttemptedTowing(int playerId)
    {
        ReportCrime(playerId, CrimeType.AttemptedVehicleTowing);
    }
    
    public void ReportSuccessfulTowing(int playerId)
    {
        IncreaseWantedLevel(playerId, 2);
    }
}
