using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private void Start()
    {
        InitializeGameMode();
    }

    private void InitializeGameMode()
    {
        GameMode mode = GameModeManager.Instance.GetGameMode();
        Debug.Log($"Initializing game in {mode} mode");

        switch (mode)
        {
            case GameMode.SinglePlayer:
                InitializeSinglePlayer();
                break;
            case GameMode.Coop:
                InitializeCoop();
                break;
            case GameMode.Vs:
                InitializeVs();
                break;
        }
    }

    private void InitializeSinglePlayer()
    {
        GameObject player1 = GameObject.FindGameObjectWithTag("Player");
        if (player1 == null)
        {
            player1 = CreatePlayer("Player1", new Vector2(300, 500), Color.white);
        }

        if (FindObjectOfType<PlayerController>() == null)
        {
            player1.AddComponent<PlayerController>();
        }

        DisablePlayer2();
    }

    private void InitializeCoop()
    {
        GameObject player1 = GameObject.FindGameObjectWithTag("Player");
        if (player1 == null)
        {
            player1 = CreatePlayer("Player1", new Vector2(280, 500), Color.white);
        }

        if (player1.GetComponent<PlayerController>() == null)
        {
            player1.AddComponent<PlayerController>();
        }

        GameObject player2 = CreatePlayer("Player2", new Vector2(320, 500), new Color(0.3f, 0.3f, 0.9f, 1f));
        
        if (player2.GetComponent<Player2Controller>() == null)
        {
            player2.AddComponent<Player2Controller>();
        }

        if (player2.GetComponent<Player2Manager>() == null)
        {
            player2.AddComponent<Player2Manager>();
        }

        if (FindObjectOfType<CoopModeSystem>() == null)
        {
            gameObject.AddComponent<CoopModeSystem>();
        }

        Debug.Log("Coop mode initialized - both players active");
    }

    private void InitializeVs()
    {
        GameObject player1 = GameObject.FindGameObjectWithTag("Player");
        if (player1 == null)
        {
            player1 = CreatePlayer("Player1", new Vector2(200, 500), Color.white);
        }

        if (player1.GetComponent<PlayerController>() == null)
        {
            player1.AddComponent<PlayerController>();
        }

        GameObject player2 = CreatePlayer("Player2", new Vector2(800, 500), new Color(1f, 0.3f, 0.3f, 1f));
        
        if (player2.GetComponent<Player2Controller>() == null)
        {
            player2.AddComponent<Player2Controller>();
        }

        if (player2.GetComponent<Player2Manager>() == null)
        {
            player2.AddComponent<Player2Manager>();
        }

        if (FindObjectOfType<VsModeSystem>() == null)
        {
            gameObject.AddComponent<VsModeSystem>();
        }

        Debug.Log("VS mode initialized - players can attack each other!");
    }

    private GameObject CreatePlayer(string name, Vector2 position, Color color)
    {
        GameObject player = new GameObject(name);
        player.transform.position = (Vector3)position;

        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = color;
        sr.sortingOrder = 5;

        CircleCollider2D collider = player.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;

        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotationZ;

        if (name == "Player1")
        {
            player.tag = "Player";
            player.AddComponent<PlayerManager>();
        }

        return player;
    }

    private void DisablePlayer2()
    {
        GameObject player2 = GameObject.Find("Player2");
        if (player2 != null)
        {
            player2.SetActive(false);
        }
    }
}