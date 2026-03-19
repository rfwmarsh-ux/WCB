using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages gang missions and interactions with the player
/// </summary>
public class GangMissionSystem : MonoBehaviour
{
    [SerializeField] private MotorcycleGang motorcycleGang;

    private List<GangMission> availableMissions = new List<GangMission>();
    private GangMission currentGangMission;

    private void Start()
    {
        if (motorcycleGang == null)
            motorcycleGang = GetComponent<MotorcycleGang>();

        InitializeGangMissions();
    }

    private void InitializeGangMissions()
    {
        // Friendly/Allied missions
        availableMissions.Add(new GangMission
        {
            Name = "Turf Protection",
            Description = "Defend our territory from rival gang",
            Reward = 500f,
            Type = Mission.MissionType.Assassination,
            RequiredStance = MotorcycleGang.GangStance.Friendly,
            Difficulty = GangMission.MissionDifficulty.Hard
        });

        availableMissions.Add(new GangMission
        {
            Name = "Bike Run",
            Description = "Lead a high-speed convoy through town",
            Reward = 300f,
            Type = Mission.MissionType.Delivery,
            RequiredStance = MotorcycleGang.GangStance.Friendly,
            Difficulty = GangMission.MissionDifficulty.Medium
        });

        availableMissions.Add(new GangMission
        {
            Name = "Rival Takedown",
            Description = "Eliminate members of rival motorcycle club",
            Reward = 750f,
            Type = Mission.MissionType.Assassination,
            RequiredStance = MotorcycleGang.GangStance.Allied,
            Difficulty = GangMission.MissionDifficulty.Very_Hard
        });

        availableMissions.Add(new GangMission
        {
            Name = "Heist Crew",
            Description = "Join the gang for a major robbery",
            Reward = 1000f,
            Type = Mission.MissionType.Robbery,
            RequiredStance = MotorcycleGang.GangStance.Allied,
            Difficulty = GangMission.MissionDifficulty.Very_Hard
        });

        // Hostile missions (if player becomes enemy)
        availableMissions.Add(new GangMission
        {
            Name = "Gang War",
            Description = "Battle against gang members seeking revenge",
            Reward = 0f, // Survival is the reward
            Type = Mission.MissionType.Assassination,
            RequiredStance = MotorcycleGang.GangStance.Hostile,
            Difficulty = GangMission.MissionDifficulty.Very_Hard
        });
    }

    public List<GangMission> GetAvailableMissions()
    {
        List<GangMission> compatible = new List<GangMission>();
        
        foreach (var mission in availableMissions)
        {
            if (mission.RequiredStance == motorcycleGang.CurrentStance)
                compatible.Add(mission);
        }

        return compatible;
    }

    public void AcceptGangMission(GangMission mission)
    {
        currentGangMission = mission;
        Debug.Log($"Gang mission accepted: {mission.Name}");
    }

    public void CompleteGangMission()
    {
        if (currentGangMission != null)
        {
            motorcycleGang.WorkWithPlayer();
            GameManager.Instance.AddMoney(currentGangMission.Reward);
            Debug.Log($"Gang mission completed: {currentGangMission.Name}");
            currentGangMission = null;
        }
    }

    public void FailGangMission()
    {
        if (currentGangMission != null)
        {
            motorcycleGang.BetrayGang();
            Debug.Log($"Gang mission failed: {currentGangMission.Name}");
            currentGangMission = null;
        }
    }

    public GangMission GetCurrentMission() => currentGangMission;
}

[System.Serializable]
public class GangMission : Mission
{
    public MotorcycleGang.GangStance RequiredStance;
    public MissionDifficulty Difficulty;

    public enum MissionDifficulty
    {
        Easy,
        Medium,
        Hard,
        Very_Hard
    }
}
