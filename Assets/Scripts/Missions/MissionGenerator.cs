using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates missions specific to each hub character
/// </summary>
public class MissionGenerator : MonoBehaviour
{
    public static List<Mission> GenerateDiveBarMissions(DiveBarOwner rusty)
    {
        List<Mission> missions = new List<Mission>();

        missions.Add(new Mission
        {
            Name = "Bar Fight Debt",
            Description = "Some guy owes my bar tabs. Get him to pay up.",
            Reward = 150f,
            Type = MissionType.Robbery
        });

        missions.Add(new Mission
        {
            Name = "Delivery Run",
            Description = "Take these smokes to the warehouse. Don't let anyone stop you.",
            Reward = 200f,
            Type = MissionType.Delivery
        });

        missions.Add(new Mission
        {
            Name = "Muscle Work",
            Description = "Some punks are moving in on my territory. Teach 'em a lesson.",
            Reward = 300f,
            Type = MissionType.Assassination
        });

        return missions;
    }

    public static List<Mission> GenerateTaxiRankMissions(TaxiRankOwner veronica)
    {
        List<Mission> missions = new List<Mission>();

        missions.Add(new Mission
        {
            Name = "Corporate Transport",
            Description = "Escort a business client across town safely. No damage to the car.",
            Reward = 250f,
            Type = MissionType.Escort
        });

        missions.Add(new Mission
        {
            Name = "Stolen Vehicle Recovery",
            Description = "That fancy Cadillac in the parking lot is mine. Go get it back.",
            Reward = 500f,
            Type = MissionType.StealCar
        });

        missions.Add(new Mission
        {
            Name = "Rival Dispatch",
            Description = "Sabotage a rival taxi company's fleet. They've been cutting into my profits.",
            Reward = 400f,
            Type = MissionType.Robbery
        });

        missions.Add(new Mission
        {
            Name = "Deliver Package Discreetly",
            Description = "Pick up a package and deliver to an address. No questions, no cops.",
            Reward = 350f,
            Type = MissionType.Delivery
        });

        return missions;
    }

    public static List<Mission> GenerateCopMissions(MeetupCop cop)
    {
        List<Mission> missions = new List<Mission>();

        missions.Add(new Mission
        {
            Name = "Evidence Disposal",
            Description = "Get rid of some evidence I need gone. Nobody can know about it.",
            Reward = 400f,
            Type = MissionType.Robbery
        });

        missions.Add(new Mission
        {
            Name = "Transport Contraband",
            Description = "Move some seized goods from one location to another. Use back roads.",
            Reward = 600f,
            Type = MissionType.Delivery
        });

        missions.Add(new Mission
        {
            Name = "Intimidate Witness",
            Description = "Make sure a witness doesn't testify. Non-lethal preferred.",
            Reward = 800f,
            Type = MissionType.Assassination
        });

        missions.Add(new Mission
        {
            Name = "Grand Theft Auto",
            Description = "Steal a specific car for resale. The paperwork won't track it.",
            Reward = 900f,
            Type = MissionType.StealCar
        });

        return missions;
    }
}
