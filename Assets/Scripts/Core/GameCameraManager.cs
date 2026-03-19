using UnityEngine;

public class GameCameraManager : MonoBehaviour
{
    public static GameCameraManager Instance { get; private set; }

    [Header("Camera Settings")]
    public float followSpeed = 8f;
    public float lookAheadDistance = 2f;
    public Vector3 offset = new Vector3(0, 0, -10);
    public bool keepPlayerCentered = true;

    private Camera mainCamera;
    private Transform playerTarget;
    private Vector3 currentVelocity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            mainCamera = gameObject.AddComponent<Camera>();
        }
        mainCamera.orthographic = false;
        mainCamera.tag = "MainCamera";
    }

    private void Start()
    {
        if (PlayerManager.Instance != null)
        {
            playerTarget = PlayerManager.Instance.transform;
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void LateUpdate()
    {
        if (playerTarget == null)
        {
            if (PlayerManager.Instance != null)
            {
                playerTarget = PlayerManager.Instance.transform;
            }
            else
            {
                return;
            }
        }

        Vector3 targetPosition = playerTarget.position + offset;

        if (keepPlayerCentered)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1f / followSpeed);
        }
        else
        {
            Vector3 desiredPosition = targetPosition;
            float lookAhead = lookAheadDistance;

            Rigidbody2D rb = playerTarget.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 velocity = rb.velocity.normalized;
                desiredPosition += new Vector3(velocity.x, velocity.y, 0) * lookAhead;
            }

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 1f / followSpeed);
        }

        transform.LookAt(playerTarget.position);
    }

    public void SetTarget(Transform target)
    {
        playerTarget = target;
    }

    public void SetFollowSpeed(float speed)
    {
        followSpeed = Mathf.Clamp(speed, 1f, 20f);
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    public float GetPlayerDirection()
    {
        if (playerTarget == null) return 0f;

        Rigidbody2D rb = playerTarget.GetComponent<Rigidbody2D>();
        if (rb != null && rb.velocity.magnitude > 0.5f)
        {
            return Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        }

        return playerTarget.eulerAngles.z;
    }

    public Vector3 GetPlayerScreenPosition()
    {
        if (playerTarget == null || mainCamera == null)
            return Vector3.zero;

        return mainCamera.WorldToScreenPoint(playerTarget.position);
    }
}
