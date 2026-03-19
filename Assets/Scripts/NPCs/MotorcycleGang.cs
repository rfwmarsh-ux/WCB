using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Main motorcycle gang system managing gang presence and player relationships
/// </summary>
public class MotorcycleGang : MonoBehaviour
{
    [SerializeField] private string gangName = "The Crimson Riders";
    [SerializeField] private Vector2 gangHQLocation = new Vector2(300f, 300f);
    [SerializeField] private int maxGangMembers = 15;
    [SerializeField] private float territoryRadius = 150f;

    private List<GangMember> activeMembers = new List<GangMember>();
    private int gangLeaderId = -1;

    // Reputation system: -100 to +100
    // Negative = enemies, Positive = allies, Zero = neutral
    private int playerReputation = 0;
    private const int REP_MIN = -100;
    private const int REP_MAX = 100;

    public enum GangStance
    {
        Hostile,    // Trying to kill player
        Neutral,    // Ignoring player
        Friendly,   // Will help player
        Allied      // Will collaborate on missions
    }

    public GangStance CurrentStance { get; private set; } = GangStance.Neutral;

    private float missionInteractionTimer = 0f;
    private float missionInteractionCooldown = 600f; // 10 minutes between interactions

    private void Start()
    {
        InitializeGang();
    }

    private void Update()
    {
        UpdateGangPresence();
        UpdatePlayerInteractions();
    }

    private void InitializeGang()
    {
        // Spawn initial gang members
        for (int i = 0; i < maxGangMembers / 3; i++)
        {
            SpawnGangMember(GangMember.GangRole.Crew);
        }

        // Spawn lieutenants
        for (int i = 0; i < 2; i++)
        {
            SpawnGangMember(GangMember.GangRole.Lieutenant);
        }

        // Spawn leader
        SpawnGangMember(GangMember.GangRole.Leader);

        Debug.Log($"{gangName} initialized with {activeMembers.Count} members");
    }

    private void SpawnGangMember(GangMember.GangRole role)
    {
        Vector3 spawnPosition = (Vector3)gangHQLocation + (Vector3)Random.insideUnitCircle * territoryRadius;
        spawnPosition.z = 0f;

        GameObject memberGO = new GameObject($"GangMember_{activeMembers.Count}");
        memberGO.transform.position = spawnPosition;
        memberGO.tag = "GangMember";

        SpriteRenderer spriteRenderer = memberGO.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
        spriteRenderer.color = new Color(0.6f, 0f, 0f); // Gang red
        spriteRenderer.sortingOrder = 2;

        CircleCollider2D collider = memberGO.AddComponent<CircleCollider2D>();
        collider.radius = 0.3f;

        Rigidbody2D rb = memberGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;

        GangMember member = memberGO.AddComponent<GangMember>();
        member.Role = role;
        member.MemberName = $"{gangName}_{role}_{activeMembers.Count}";
        member.ToughLevel = role == GangMember.GangRole.Leader ? 5 : (role == GangMember.GangRole.Lieutenant ? 3 : 1);

        if (role == GangMember.GangRole.Leader)
            gangLeaderId = activeMembers.Count;

        memberGO.transform.parent = transform;
        activeMembers.Add(member);
    }

    private void UpdateGangPresence()
    {
        // Keep gang members alive - respawn if killed
        while (activeMembers.Count < (maxGangMembers - 3)) // Leave room for key members
        {
            SpawnGangMember(GangMember.GangRole.Crew);
        }

        activeMembers.RemoveAll(member => member == null || !member.gameObject.activeSelf);
    }

    private void UpdatePlayerInteractions()
    {
        missionInteractionTimer += Time.deltaTime;

        if (missionInteractionTimer >= missionInteractionCooldown)
        {
            AttemptGangInteraction();
            missionInteractionTimer = 0f;
            missionInteractionCooldown = Random.Range(600f, 900f); // 10-15 minutes
        }

        UpdateStance();
    }

    private void UpdateStance()
    {
        CurrentStance = playerReputation switch
        {
            < -50 => GangStance.Hostile,
            < 0 => GangStance.Neutral,
            < 50 => GangStance.Friendly,
            >= 50 => GangStance.Allied,
            _ => GangStance.Neutral
        };
    }

    private void AttemptGangInteraction()
    {
        if (CurrentStance == GangStance.Hostile)
        {
            // Aggro players
            foreach (var member in activeMembers)
            {
                member.StartPursue();
            }
        }
        else if (CurrentStance == GangStance.Allied)
        {
            // Could offer a mission
            Debug.Log($"{gangName}: We got a job for you, if you're interested...");
        }
    }

    public void ChangeReputation(int amount)
    {
        playerReputation = Mathf.Clamp(playerReputation + amount, REP_MIN, REP_MAX);
        Debug.Log($"{gangName} reputation: {playerReputation} ({CurrentStance})");
    }

    public void WorkWithPlayer()
    {
        ChangeReputation(20);
    }

    public void BetrayGang()
    {
        ChangeReputation(-30);
    }

    public int GetPlayerReputation() => playerReputation;
    public string GetGangName() => gangName;
    public Vector2 GetHQLocation() => gangHQLocation;
    public List<GangMember> GetMembers() => activeMembers;
    public int GetMemberCount() => activeMembers.Count;
    public GangMember GetLeader() => gangLeaderId >= 0 && gangLeaderId < activeMembers.Count ? activeMembers[gangLeaderId] : null;
}
