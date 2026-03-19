using UnityEngine;

/// <summary>
/// Manages mission hub interactions and character encounters
/// </summary>
public class MissionHubManager : MonoBehaviour
{
    [SerializeField] private DiveBarOwner rusty;
    [SerializeField] private TaxiRankOwner veronica;
    [SerializeField] private MeetupCop morganCop;
    
    [SerializeField] private Vector2 diveBarLocation = new Vector2(150, 450);      // The Factory - West End
    [SerializeField] private Vector2 taxiRankLocation = new Vector2(500, 500);     // Crimson Cab Station - City Centre
    [SerializeField] private Vector2 copFirstMeetup = new Vector2(750, 350);       // Variable locations for cop

    private PlayerController player;

    private void Start()
    {
        InitializeHubs();
        SetupCopMeetupLocations();
    }

    private void InitializeHubs()
    {
        player = FindObjectOfType<PlayerController>();
        
        // Create and position mission hub characters
        if (rusty != null)
        {
            rusty.transform.position = diveBarLocation;
        }

        if (veronica != null)
        {
            veronica.transform.position = taxiRankLocation;
        }

        if (morganCop != null)
        {
            morganCop.transform.position = copFirstMeetup;
        }
    }

    private void SetupCopMeetupLocations()
    {
        // Define various meeting points around the city
        var meetupLocations = new System.Collections.Generic.List<Vector2>()
        {
            new Vector2(750, 350),      // Alley behind East Side club
            new Vector2(200, 300),      // Warehouse in West End
            new Vector2(500, 100),      // Parking lot in Southside
            new Vector2(600, 800),      // Industrial area in Northside
            new Vector2(400, 600)       // Park in City Centre
        };

        morganCop.SetMeetupLocations(meetupLocations);
    }

    public void InteractWithRusty()
    {
        if (Vector2.Distance(player.transform.position, diveBarLocation) < 5f)
        {
            rusty.ShowDialogue();
            // Show available missions from Rusty
        }
    }

    public void InteractWithVeronica()
    {
        if (Vector2.Distance(player.transform.position, taxiRankLocation) < 5f)
        {
            veronica.ShowDialogue();
            // Show available missions from Veronica
            // Option to visit brothel if unlocked
        }
    }

    public void InteractWithCop()
    {
        if (Vector2.Distance(player.transform.position, morganCop.transform.position) < 3f)
        {
            morganCop.ShowDialogue();
            // Show cop missions
        }
    }

    public DiveBarOwner GetRusty() => rusty;
    public TaxiRankOwner GetVeronica() => veronica;
    public MeetupCop GetCop() => morganCop;
}
