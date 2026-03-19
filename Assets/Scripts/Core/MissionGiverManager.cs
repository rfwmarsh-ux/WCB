using UnityEngine;
using System.Collections.Generic;

public enum MissionGiverType
{
    Dave,
    Crystal,
    Lorna,
    MickDick
}

public class MissionGiverCharacter : MonoBehaviour
{
    public MissionGiverType characterType;
    public string characterName;
    public string title;
    public Color skinColor;
    public Color hairColor;
    public float bodySize = 1f;
    public bool hasMoped;
    public bool isSexy;
    public bool isWornOut;
    public bool isHard;
    public string voiceDescription;

    public void Initialize(MissionGiverType type)
    {
        characterType = type;
        CreateCaricature();
    }

    private void CreateCaricature()
    {
        switch (characterType)
        {
            case MissionGiverType.Dave:
                characterName = "Dave";
                title = "Motorcycle Gang Leader";
                skinColor = new Color(0.9f, 0.75f, 0.65f);
                hairColor = new Color(0.2f, 0.1f, 0.05f);
                bodySize = 1.8f;
                hasMoped = true;
                isHard = true;
                voiceDescription = "Gruff biker voice";
                break;

            case MissionGiverType.Crystal:
                characterName = "Crystal";
                title = "Taxi Rank Queen";
                skinColor = new Color(0.85f, 0.7f, 0.55f);
                hairColor = new Color(0.9f, 0.6f, 0.1f);
                bodySize = 0.9f;
                isSexy = true;
                voiceDescription = "Seductive, confident";
                break;

            case MissionGiverType.Lorna:
                characterName = "Lorna";
                title = "Dodgy Copper";
                skinColor = new Color(0.8f, 0.7f, 0.6f);
                hairColor = new Color(0.3f, 0.2f, 0.15f);
                bodySize = 1.1f;
                isWornOut = true;
                voiceDescription = "Tired, cynical";
                break;

            case MissionGiverType.MickDick:
                characterName = "Mick Dick";
                title = "Dive Bar Owner";
                skinColor = new Color(0.75f, 0.6f, 0.5f);
                hairColor = new Color(0.1f, 0.1f, 0.1f);
                bodySize = 1.3f;
                isHard = true;
                voiceDescription = "Gravelly, intimidating";
                break;
        }

        CreateVisuals();
    }

    private void CreateVisuals()
    {
        float yOffset = 0;
        
        GameObject body = new GameObject("Body");
        body.transform.parent = transform;
        
        SpriteRenderer bodySr = body.AddComponent<SpriteRenderer>();
        bodySr.sprite = SpriteHelper.GetDefaultSprite();
        bodySr.sortingOrder = 5;
        
        if (characterType == MissionGiverType.Dave)
        {
            bodySr.color = new Color(0.3f, 0.2f, 0.15f);
            body.transform.localScale = new Vector3(3f * bodySize, 4f * bodySize, 1f);
            yOffset = -1f;
        }
        else if (characterType == MissionGiverType.Crystal)
        {
            bodySr.color = new Color(0.9f, 0.2f, 0.3f);
            body.transform.localScale = new Vector3(2f, 3f, 1f);
            yOffset = 0.5f;
        }
        else if (characterType == MissionGiverType.Lorna)
        {
            bodySr.color = new Color(0.2f, 0.3f, 0.5f);
            body.transform.localScale = new Vector3(2.2f, 3f, 1f);
            yOffset = 0f;
        }
        else if (characterType == MissionGiverType.MickDick)
        {
            bodySr.color = new Color(0.4f, 0.35f, 0.3f);
            body.transform.localScale = new Vector3(2.5f, 3.5f, 1f);
            yOffset = -0.5f;
        }
        body.transform.localPosition = new Vector3(0, yOffset, 0);

        GameObject head = new GameObject("Head");
        head.transform.parent = transform;
        
        SpriteRenderer headSr = head.AddComponent<SpriteRenderer>();
        headSr.sprite = SpriteHelper.GetDefaultSprite();
        headSr.sortingOrder = 6;
        headSr.color = skinColor;
        
        float headSize = characterType == MissionGiverType.Dave ? 2.5f : 1.8f;
        head.transform.localScale = new Vector3(headSize, headSize, 1f);
        head.transform.localPosition = new Vector3(0, 2.5f + yOffset, 0);

        CreateHair(head);
        
        if (characterType == MissionGiverType.Crystal)
        {
            CreateMakeup(head);
        }
        
        if (characterType == MissionGiverType.Lorna)
        {
            CreateTiredEyes(head);
        }

        if (characterType == MissionGiverType.MickDick)
        {
            CreateToughFace(head);
        }

        if (hasMoped)
        {
            CreateMoped();
        }
    }

    private void CreateHair(GameObject head)
    {
        GameObject hair = new GameObject("Hair");
        hair.transform.parent = head.transform;
        
        SpriteRenderer hairSr = hair.AddComponent<SpriteRenderer>();
        hairSr.sprite = SpriteHelper.GetDefaultSprite();
        hairSr.sortingOrder = 7;
        hairSr.color = hairColor;
        
        float hairSize = characterType == MissionGiverType.Dave ? 2.2f : 1.5f;
        
        if (characterType == MissionGiverType.Crystal)
        {
            hair.transform.localScale = new Vector3(2.5f, 1f, 1f);
            hair.transform.localPosition = new Vector3(0, 0.6f, 0);
        }
        else if (characterType == MissionGiverType.Dave)
        {
            hair.transform.localScale = new Vector3(hairSize, 1.2f, 1f);
            hair.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
        else
        {
            hair.transform.localScale = new Vector3(hairSize, hairSize, 1f);
            hair.transform.localPosition = new Vector3(0, 0.4f, 0);
        }
    }

    private void CreateMakeup(GameObject head)
    {
        GameObject lips = new GameObject("Lips");
        lips.transform.parent = head.transform;
        
        SpriteRenderer lipsSr = lips.AddComponent<SpriteRenderer>();
        lipsSr.sprite = SpriteHelper.GetDefaultSprite();
        lipsSr.sortingOrder = 8;
        lipsSr.color = new Color(0.8f, 0.1f, 0.2f);
        lips.transform.localScale = new Vector3(0.8f, 0.3f, 1f);
        lips.transform.localPosition = new Vector3(0, -0.3f, 0);

        GameObject eyes = new GameObject("Eyes");
        eyes.transform.parent = head.transform;
        
        SpriteRenderer eyesSr = eyes.AddComponent<SpriteRenderer>();
        eyesSr.sprite = SpriteHelper.GetDefaultSprite();
        eyesSr.sortingOrder = 8;
        eyesSr.color = new Color(0.1f, 0.1f, 0.1f);
        eyes.transform.localScale = new Vector3(1.2f, 0.4f, 1f);
        eyes.transform.localPosition = new Vector3(0, 0.2f, 0);
    }

    private void CreateTiredEyes(GameObject head)
    {
        GameObject eyes = new GameObject("TiredEyes");
        eyes.transform.parent = head.transform;
        
        SpriteRenderer eyesSr = eyes.AddComponent<SpriteRenderer>();
        eyesSr.sprite = SpriteHelper.GetDefaultSprite();
        eyesSr.sortingOrder = 8;
        eyesSr.color = new Color(0.4f, 0.3f, 0.3f);
        eyes.transform.localScale = new Vector3(1.3f, 0.3f, 1f);
        eyes.transform.localPosition = new Vector3(0, 0.2f, 0);

        GameObject bags = new GameObject("EyeBags");
        bags.transform.parent = head.transform;
        
        SpriteRenderer bagsSr = bags.AddComponent<SpriteRenderer>();
        bagsSr.sprite = SpriteHelper.GetDefaultSprite();
        bagsSr.sortingOrder = 7;
        bagsSr.color = new Color(0.5f, 0.4f, 0.4f);
        bags.transform.localScale = new Vector3(1.4f, 0.4f, 1f);
        bags.transform.localPosition = new Vector3(0, 0.0f, 0);
    }

    private void CreateToughFace(GameObject head)
    {
        GameObject beard = new GameObject("Beard");
        beard.transform.parent = head.transform;
        
        SpriteRenderer beardSr = beard.AddComponent<SpriteRenderer>();
        beardSr.sprite = SpriteHelper.GetDefaultSprite();
        beardSr.sortingOrder = 7;
        beardSr.color = new Color(0.1f, 0.1f, 0.1f);
        beard.transform.localScale = new Vector3(1.8f, 1.2f, 1f);
        beard.transform.localPosition = new Vector3(0, -0.5f, 0);

        GameObject scar = new GameObject("Scar");
        scar.transform.parent = head.transform;
        
        SpriteRenderer scarSr = scar.AddComponent<SpriteRenderer>();
        scarSr.sprite = SpriteHelper.GetDefaultSprite();
        scarSr.sortingOrder = 8;
        scarSr.color = new Color(0.7f, 0.5f, 0.5f);
        scar.transform.localScale = new Vector3(0.6f, 0.15f, 1f);
        scar.transform.localPosition = new Vector3(0.5f, 0.3f, 0);
    }

    private void CreateMoped()
    {
        GameObject moped = new GameObject("Moped");
        moped.transform.parent = transform;
        
        SpriteRenderer mopedSr = moped.AddComponent<SpriteRenderer>();
        mopedSr.sprite = SpriteHelper.GetDefaultSprite();
        mopedSr.sortingOrder = 4;
        mopedSr.color = Color.white;
        moped.transform.localScale = new Vector3(4f, 2f, 1f);
        moped.transform.localPosition = new Vector3(2.5f, -2f, 0);

        GameObject wheels = new GameObject("Wheels");
        wheels.transform.parent = moped.transform;
        
        SpriteRenderer wheelSr = wheels.AddComponent<SpriteRenderer>();
        wheelSr.sprite = SpriteHelper.GetDefaultSprite();
        wheelSr.sortingOrder = 5;
        wheelSr.color = new Color(0.2f, 0.2f, 0.2f);
        wheels.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}

public class MissionGiverManager : MonoBehaviour
{
    public static MissionGiverManager Instance { get; private set; }

    private Dictionary<MissionGiverType, MissionGiverCharacter> givers = new Dictionary<MissionGiverType, MissionGiverCharacter>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        CreateAllMissionGivers();
    }

    private void CreateAllMissionGivers()
    {
        CreateMissionGiver(MissionGiverType.Dave, new Vector2(200f, 300f));
        CreateMissionGiver(MissionGiverType.Crystal, new Vector2(500f, 480f));
        CreateMissionGiver(MissionGiverType.Lorna, new Vector2(480f, 510f));
        CreateMissionGiver(MissionGiverType.MickDick, new Vector2(150f, 450f));
    }

    private void CreateMissionGiver(MissionGiverType type, Vector2 position)
    {
        GameObject go = new GameObject($"MissionGiver_{type}");
        go.transform.position = (Vector3)position;

        MissionGiverCharacter giver = go.AddComponent<MissionGiverCharacter>();
        giver.Initialize(type);

        givers[type] = giver;
    }

    public MissionGiverCharacter GetGiver(MissionGiverType type)
    {
        return givers.ContainsKey(type) ? givers[type] : null;
    }
}
