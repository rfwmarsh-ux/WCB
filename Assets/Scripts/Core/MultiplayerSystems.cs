using UnityEngine;

public class VsModeSystem : MonoBehaviour
{
    public static VsModeSystem Instance { get; private set; }

    private int player1Score = 0;
    private int player2Score = 0;
    private int player1Wins = 0;
    private int player2Wins = 0;
    private bool player1Alive = true;
    private bool player2Alive = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddScore(int player, int points)
    {
        if (player == 1) player1Score += points;
        else player2Score += points;

        Debug.Log($"P{player} score: {points} (Total: P1={player1Score}, P2={player2Score})");
    }

    public void Player1Died()
    {
        player1Alive = false;
        player2Wins++;
        Debug.Log("Player 2 wins the round!");
        
        Invoke("RestartRound", 3f);
    }

    public void Player2Died()
    {
        player2Alive = false;
        player1Wins++;
        Debug.Log("Player 1 wins the round!");
        
        Invoke("RestartRound", 3f);
    }

    private void RestartRound()
    {
        player1Alive = true;
        player2Alive = true;
        
        PlayerManager pm = FindObjectOfType<PlayerManager>();
        if (pm != null)
        {
            pm.Respawn(new Vector2(200, 500));
        }

        Player2Manager p2m = Player2Manager.Instance;
        if (p2m != null)
        {
            p2m.Respawn(new Vector2(800, 500));
        }

        Debug.Log("Round restarted!");
    }

    public void DealDamageToPlayer(int targetPlayer, float damage)
    {
        if (targetPlayer == 1)
        {
            PlayerManager pm = FindObjectOfType<PlayerManager>();
            pm?.TakeDamage(damage);
            Debug.Log($"P1 took {damage} damage from P2");
        }
        else
        {
            Player2Manager p2m = Player2Manager.Instance;
            p2m?.TakeDamage(damage);
            Debug.Log($"P2 took {damage} damage from P1");
        }
    }

    public void SabotageMission(int player)
    {
        int sabotageCost = 50;
        
        if (player == 1)
        {
            GameManager gm = GameManager.Instance;
            if (gm.Money >= sabotageCost)
            {
                gm.AddMoney(-sabotageCost);
                gm.WantedLevel += 2;
                Debug.Log("P1 sabotaged the mission! P2's wanted level increased.");
            }
        }
        else
        {
            GameManager gm = GameManager.Instance;
            if (gm.Money >= sabotageCost)
            {
                gm.AddMoney(-sabotageCost);
                gm.WantedLevel -= 1;
                Debug.Log("P2 sabotaged the mission! P1's wanted level decreased.");
            }
        }
    }

    public void ExplodePlayerVehicle(int player)
    {
        Debug.Log($"Player {player}'s vehicle was destroyed!");
        
        if (player == 1)
        {
            PlayerManager pm = FindObjectOfType<PlayerManager>();
            pm?.TakeDamage(50);
        }
        else
        {
            Player2Manager p2m = Player2Manager.Instance;
            p2m?.TakeDamage(50);
        }

        AddScore(player == 1 ? 2 : 1, 100);
    }

    public (int p1Score, int p2Score, int p1Wins, int p2Wins) GetScores()
    {
        return (player1Score, player2Score, player1Wins, player2Wins);
    }

    public bool IsPlayer1Alive() => player1Alive;
    public bool IsPlayer2Alive() => player2Alive;
}

public class CoopModeSystem : MonoBehaviour
{
    public static CoopModeSystem Instance { get; private set; }

    private int sharedMoney = 1000;
    private int combinedWantedLevel = 0;
    private bool missionInProgress = false;
    private string currentMissionType = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AcceptMissionAsSolo(string missionType)
    {
        currentMissionType = missionType;
        missionInProgress = true;
        Debug.Log($"Solo mission accepted: {missionType}");
    }

    public void AcceptMissionAsCoop(string missionType)
    {
        currentMissionType = missionType;
        missionInProgress = true;
        Debug.Log($"Coop mission accepted: {missionType}");
        
        PlayerManager p1 = FindObjectOfType<PlayerManager>();
        Player2Manager p2 = Player2Manager.Instance;
        
        if (p1 != null && p2 != null)
        {
            Debug.Log("Both players working together!");
        }
    }

    public void CompleteMission(int reward)
    {
        sharedMoney += reward;
        missionInProgress = false;
        currentMissionType = "";
        Debug.Log($"Mission complete! Reward: ${reward}");
    }

    public void FailMission()
    {
        missionInProgress = false;
        combinedWantedLevel += 1;
        Debug.Log("Mission failed!");
    }

    public void AddSharedMoney(int amount)
    {
        sharedMoney += amount;
    }

    public bool SpendSharedMoney(int amount)
    {
        if (sharedMoney >= amount)
        {
            sharedMoney -= amount;
            return true;
        }
        return false;
    }

    public int GetSharedMoney() => sharedMoney;
    public bool IsMissionInProgress() => missionInProgress;
    public string GetCurrentMissionType() => currentMissionType;
}