using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base class for mission hub characters
/// </summary>
public abstract class MissionHubCharacter : MonoBehaviour
{
    [SerializeField] protected string characterName;
    [SerializeField] protected string description;
    [SerializeField] protected List<string> dialogueLines = new List<string>();
    
    protected List<Mission> availableMissions = new List<Mission>();

    public virtual void ShowDialogue()
    {
        Debug.Log($"{characterName}: {dialogueLines[Random.Range(0, dialogueLines.Count)]}");
    }

    public abstract void GiveMission(Mission mission);
    public string GetCharacterName() => characterName;
    public string GetDescription() => description;
}

/// <summary>
/// The worn-out dive bar owner - "Rusty" at The Factory (West End)
/// </summary>
public class DiveBarOwner : MissionHubCharacter
{
    [SerializeField] private float smokeParticleChance = 0.3f;
    private ParticleSystem cigaretteSmoke;

    private void Start()
    {
        characterName = "Rusty";
        description = "A worn-out 60-year-old man, perpetually chain-smoking. Runs 'The Factory' dive bar in the West End.";
        
        dialogueLines.Add("*cough* Yeah, I got work if you need it...");
        dialogueLines.Add("Another beer? Whiskey's cheaper...");
        dialogueLines.Add("*lights cigarette* Listen kid, you looking for jobs or just wasting my time?");
        dialogueLines.Add("This place ain't much, but it pays the bills... barely.");
        dialogueLines.Add("*squints through smoke* You got the guts for real work?");
    }

    public override void ShowDialogue()
    {
        base.ShowDialogue();
        EmitSmoke();
    }

    private void EmitSmoke()
    {
        if (Random.value < smokeParticleChance)
        {
            // Particle effect for cigarette smoke
            Debug.Log($"{characterName} takes a long drag from cigarette");
        }
    }

    public override void GiveMission(Mission mission)
    {
        availableMissions.Add(mission);
        Debug.Log($"{characterName} offers: {mission.Name} - Reward: ${mission.Reward}");
    }
}

/// <summary>
/// The seductive taxi rank owner and brothel operator - "Veronica" at Crimson Cab Station (City Centre)
/// </summary>
public class TaxiRankOwner : MissionHubCharacter
{
    [SerializeField] private Brothel brothelReference;
    public bool BrothelUnlocked { get; set; } = false;

    private void Start()
    {
        characterName = "Veronica";
        description = "A strikingly attractive woman in her mid-40s. Runs 'Crimson Cab Station' taxi rank in the City Centre and discreetly operates a high-end brothel upstairs.";
        
        dialogueLines.Add("*flips hair* Looking for a ride... or something else?");
        dialogueLines.Add("I've got plenty of work for someone with your... talents.");
        dialogueLines.Add("*smiles knowingly* You interested in the full package?");
        dialogueLines.Add("My cab drivers know everything happening in this city. Very useful.");
        dialogueLines.Add("*leans in close* Do we understand each other?");
    }

    public override void ShowDialogue()
    {
        base.ShowDialogue();
    }

    public override void GiveMission(Mission mission)
    {
        availableMissions.Add(mission);
        Debug.Log($"{characterName} offers: {mission.Name} - Reward: ${mission.Reward}");
    }

    public void UnlockBrothel()
    {
        BrothelUnlocked = true;
        Debug.Log($"{characterName}: Welcome to my... other business. Enjoy your stay.");
    }

    public Brothel GetBrothelAccess()
    {
        if (BrothelUnlocked)
            return brothelReference;
        return null;
    }
}

/// <summary>
/// The dodgy cop who moves around town - "Detective Morgan" meets at random locations
/// </summary>
public class MeetupCop : MissionHubCharacter
{
    [SerializeField] private List<Vector2> meetupLocations = new List<Vector2>();
    private Vector2 currentMeetupLocation;
    private int nextMeetupIndex = 0;

    private void Start()
    {
        characterName = "Detective Morgan";
        description = "A woman in her early 50s, a cop looking to earn extra cash before retirement. Always paranoid, always moving.";
        
        dialogueLines.Add("*looks around nervously* Keep your voice down.");
        dialogueLines.Add("I'm risking my pension for this. Better be worth it.");
        dialogueLines.Add("*checks watch and surroundings* We don't have much time.");
        dialogueLines.Add("Nobody can know about this. Nobody.");
        dialogueLines.Add("*pulls coat collar up* Next meeting location will be different. Remember that.");
    }

    public override void ShowDialogue()
    {
        base.ShowDialogue();
        RotateMeetupLocation();
    }

    private void RotateMeetupLocation()
    {
        if (meetupLocations.Count > 0)
        {
            currentMeetupLocation = meetupLocations[nextMeetupIndex];
            nextMeetupIndex = (nextMeetupIndex + 1) % meetupLocations.Count;
            Debug.Log($"{characterName}: Next meeting at {currentMeetupLocation}");
        }
    }

    public override void GiveMission(Mission mission)
    {
        availableMissions.Add(mission);
        Debug.Log($"{characterName} whispers: {mission.Name} - Reward: ${mission.Reward}");
    }

    public void SetMeetupLocations(List<Vector2> locations)
    {
        meetupLocations = locations;
    }

    public Vector2 GetNextMeetupLocation()
    {
        return currentMeetupLocation;
    }
}

/// <summary>
/// Brothel system - accessible through Veronica
/// </summary>
public class Brothel : MonoBehaviour
{
    public string brothelName = "Velvet Dreams";
    [SerializeField] private List<BrothelWorker> workers = new List<BrothelWorker>();
    [SerializeField] private List<float> serviceRates = new List<float>() { 100f, 250f, 500f };

    private void Start()
    {
        InitializeWorkers();
    }

    private void InitializeWorkers()
    {
        // Populate with various workers
        for (int i = 0; i < 5; i++)
        {
            BrothelWorker worker = new BrothelWorker($"Worker_{i + 1}", 20 + (i * 5));
            workers.Add(worker);
        }
    }

    public List<BrothelWorker> GetAvailableWorkers()
    {
        return workers;
    }

    public float GetServiceRate(int tier)
    {
        if (tier >= 0 && tier < serviceRates.Count)
            return serviceRates[tier];
        return 0f;
    }

    public void UseService(BrothelWorker worker, int serviceTier)
    {
        float cost = GetServiceRate(serviceTier);
        if (GameManager.Instance.CanAfford(cost))
        {
            GameManager.Instance.AddMoney(-cost);
            Debug.Log($"Service with {worker.name} completed. Cost: ${cost}");
        }
        else
        {
            Debug.Log("You can't afford this service.");
        }
    }
}

[System.Serializable]
public class BrothelWorker
{
    public string name;
    public int age;

    public BrothelWorker(string name, int age)
    {
        this.name = name;
        this.age = age;
    }
}
