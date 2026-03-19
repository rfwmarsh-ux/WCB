using UnityEngine;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    private List<Mission> activeMissions = new List<Mission>();
    private Mission currentMission = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void InitializeMissions()
    {
        Debug.Log("Mission system initialized");
    }

    public void AcceptMission(string missionType, bool isCoop)
    {
        Debug.Log($"Mission accepted: {missionType} (Coop: {isCoop})");
        
        if (isCoop && CoopModeSystem.Instance != null)
        {
            CoopModeSystem.Instance.AcceptMissionAsCoop(missionType);
        }
    }

    public void CompleteMission(int reward)
    {
        Debug.Log($"Mission completed! Reward: ${reward}");
        
        if (CoopModeSystem.Instance != null)
        {
            CoopModeSystem.Instance.CompleteMission(reward);
        }
    }

    public void FailMission()
    {
        Debug.Log("Mission failed!");
        
        if (CoopModeSystem.Instance != null)
        {
            CoopModeSystem.Instance.FailMission();
        }
    }

    public Mission GetCurrentMission() => currentMission;
    public bool HasActiveMission() => currentMission != null;
}

public class Mission
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Reward { get; set; }
    public bool IsCoop { get; set; }
    public bool IsComplete { get; set; }
}
