using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    public static SplitScreenManager Instance { get; private set; }

    [Header("Split Screen Settings")]
    public bool isSplitScreenActive = false;
    public SplitScreenMode splitMode = SplitScreenMode.None;

    [Header("Camera Settings")]
    public Camera player1Camera;
    public Camera player2Camera;
    public float cameraSize = 15f;

    public enum SplitScreenMode
    {
        None,
        Vertical,
        Horizontal
    }

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
        SetupSplitScreen();
    }

    private void SetupSplitScreen()
    {
        if (GameModeManager.Instance == null) return;

        if (GameModeManager.Instance.IsCoop() || GameModeManager.Instance.IsVs())
        {
            isSplitScreenActive = true;
            splitMode = SplitScreenMode.Vertical;
            ApplyVerticalSplit();
        }
        else
        {
            isSplitScreenActive = false;
            splitMode = SplitScreenMode.None;
            ApplySingleScreen();
        }
    }

    private void ApplyVerticalSplit()
    {
        int width = Screen.width / 2;
        int height = Screen.height;

        if (player1Camera != null)
        {
            player1Camera.rect = new Rect(0, 0, 0.5f, 1f);
            player1Camera.orthographicSize = cameraSize;
        }

        if (player2Camera != null)
        {
            player2Camera.rect = new Rect(0.5f, 0, 0.5f, 1f);
            player2Camera.orthographicSize = cameraSize;
        }

        RepositionHUDs();
    }

    private void ApplySingleScreen()
    {
        if (player1Camera != null)
        {
            player1Camera.rect = new Rect(0, 0, 1f, 1f);
            player1Camera.orthographicSize = cameraSize;
        }

        if (player2Camera != null)
        {
            player2Camera.gameObject.SetActive(false);
        }
    }

    private void RepositionHUDs()
    {
        PlayerHUD[] hudList = FindObjectsOfType<PlayerHUD>();
        foreach (var hud in hudList)
        {
            if (hud.playerId == 1)
            {
                hud.p1MiniMapPosition = new Vector2(-170, 170);
            }
            else if (hud.playerId == 2)
            {
                hud.p2MiniMapPosition = new Vector2(-170 - (Screen.width / 2), 170);
            }
        }
    }

    private void Update()
    {
        if (!isSplitScreenActive) return;

        UpdateCameraPositions();
    }

    private void UpdateCameraPositions()
    {
        if (player1Camera != null && PlayerManager.Instance != null)
        {
            Vector3 targetPos = PlayerManager.Instance.transform.position;
            targetPos.z = player1Camera.transform.position.z;
            player1Camera.transform.position = Vector3.Lerp(player1Camera.transform.position, targetPos, Time.deltaTime * 5f);
        }

        if (player2Camera != null && Player2Manager.Instance != null)
        {
            Vector3 targetPos = Player2Manager.Instance.transform.position;
            targetPos.z = player2Camera.transform.position.z;
            player2Camera.transform.position = Vector3.Lerp(player2Camera.transform.position, targetPos, Time.deltaTime * 5f);
        }
    }

    public void CreateSplitScreenCameras()
    {
        Camera mainCamera = Camera.main;
        
        if (mainCamera != null)
        {
            player1Camera = mainCamera;
            player1Camera.name = "Player1Camera";
        }

        GameObject cam2GO = new GameObject("Player2Camera");
        player2Camera = cam2GO.AddComponent<Camera>();
        player2Camera.orthographic = true;
        player2Camera.orthographicSize = cameraSize;
        cam2GO.tag = "MainCamera";
        
        CameraSetup cam2Setup = cam2GO.AddComponent<CameraSetup>();
        cam2Setup.targetPlayer = 2;
    }

    public void SetCameraTarget(int playerId, Transform target)
    {
        Camera cam = playerId == 1 ? player1Camera : player2Camera;
        if (cam != null)
        {
            CameraFollow follow = cam.GetComponent<CameraFollow>();
            if (follow == null)
            {
                follow = cam.gameObject.AddComponent<CameraFollow>();
            }
            follow.target = target;
        }
    }
}

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}

public class CameraSetup : MonoBehaviour
{
    public int targetPlayer = 1;
}
