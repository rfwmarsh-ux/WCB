using UnityEngine;
using System.Collections.Generic;

public class ChurchManager : MonoBehaviour
{
    public static ChurchManager Instance { get; private set; }

    [Header("Church Settings")]
    [SerializeField] private Vector2 churchPosition = new Vector2(500f, 500f);
    [SerializeField] private float churchRadius = 15f;

    private bool player1StartedAtChurch = false;

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
        CreateChurch();
    }

    private void CreateChurch()
    {
        GameObject church = new GameObject("Wolverhampton Central Church");
        church.transform.position = (Vector3)churchPosition;

        SpriteRenderer sr = church.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.9f, 0.85f, 0.7f, 1f);
        sr.sortingOrder = 2;

        church.transform.localScale = new Vector3(25f, 20f, 1f);

        CircleCollider2D collider = church.AddComponent<CircleCollider2D>();
        collider.radius = churchRadius;
        collider.isTrigger = true;

        church.AddComponent<ChurchData>();

        church.transform.parent = transform;
        Debug.Log("Church created at city center (500, 500)");
    }

    public void SetPlayer1StartPosition(GameObject player)
    {
        if (!player1StartedAtChurch)
        {
            player.transform.position = (Vector3)churchPosition + Vector3.up * 12f;
            player1StartedAtChurch = true;
            Debug.Log("Player 1 spawned at the church");
        }
    }

    public Vector2 GetChurchPosition() => churchPosition;

    public bool IsAtChurch(Vector2 position)
    {
        return Vector2.Distance(position, churchPosition) < churchRadius;
    }
}

public class ChurchData : MonoBehaviour
{
}

public class VeterinaryHeal : MonoBehaviour
{
    public static VeterinaryHeal Instance { get; private set; }

    [SerializeField] private float healCost = 100f;
    [SerializeField] private float healAmount = 100f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void TryHeal()
    {
        GameManager gm = GameManager.Instance;
        PlayerManager pm = PlayerManager.Instance;

        if (pm.GetHealth() >= pm.GetMaxHealth())
        {
            Debug.Log("Already at full health!");
            return;
        }

        if (gm.CanAfford(healCost))
        {
            gm.AddMoney(-healCost);
            pm.HealDamage(healAmount);
            Debug.Log($"Healed at veterinary centre. Cost: ${healCost}");
        }
        else
        {
            Debug.Log($"Cannot afford healing. Need: ${healCost}, Have: ${gm.Money}");
        }
    }

    public void TryHealPlayer2()
    {
        GameManager gm = GameManager.Instance;
        Player2Manager p2m = Player2Manager.Instance;

        if (p2m == null)
        {
            Debug.Log("No player 2!");
            return;
        }

        if (p2m.GetHealth() >= p2m.GetMaxHealth())
        {
            Debug.Log("Player 2 already at full health!");
            return;
        }

        if (gm.CanAfford(healCost))
        {
            gm.AddMoney(-healCost);
            p2m.HealDamage(healAmount);
            Debug.Log($"Player 2 healed at veterinary centre. Cost: ${healCost}");
        }
        else
        {
            Debug.Log($"Cannot afford healing for Player 2. Need: ${healCost}, Have: ${gm.Money}");
        }
    }

    public float GetCost() => healCost;
}