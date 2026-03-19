using UnityEngine;

/// <summary>
/// Locations and interactions for the three mission hubs
/// </summary>
public class HubLocation : MonoBehaviour
{
    [SerializeField] public string hubLocationName;
    [SerializeField] public MissionHub.MissionHubType locationType;
    [SerializeField] public MissionHubCharacter character;
    [SerializeField] public float interactionRange = 3f;
    [SerializeField] private SpriteRenderer locationMarker;

    private void Start()
    {
        // Marker for mission hub locations
        if (locationMarker == null)
            locationMarker = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ShowHubPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HideHubPrompt();
        }
    }

    private void ShowHubPrompt()
    {
        Debug.Log($"Press E to interact with {hubLocationName}");
    }

    private void HideHubPrompt()
    {
        Debug.Log("Left interaction range");
    }

    public MissionHubCharacter GetCharacter()
    {
        return character;
    }
}
