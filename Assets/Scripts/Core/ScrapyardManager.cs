using UnityEngine;

public class ScrapyardManager : MonoBehaviour
{
    public static ScrapyardManager Instance { get; private set; }

    [Header("Scrapyard Settings")]
    [SerializeField] private float vehicleChangeCost = 500f;
    [SerializeField] private float doorCloseDelay = 1f;
    [SerializeField] private float colorChangeDelay = 2f;
    [SerializeField] private float rotationDelay = 1f;
    [SerializeField] private float doorOpenDelay = 1f;

    [Header("Building Settings")]
    [SerializeField] private Transform doorLeft;
    [SerializeField] private Transform doorRight;
    [SerializeField] private float doorCloseDistance = 2f;
    [SerializeField] private float doorSpeed = 5f;

    private bool isPlayerInside = false;
    private bool isProcessing = false;
    private Vehicle playerVehicle;
    private Vector3 originalVehicleRotation;
    private Vector3 exitRotation;
    private Vector3 exitPosition;
    private bool doorsClosing = false;
    private bool doorsOpening = false;
    private bool colorChanged = false;
    private bool rotationChanged = false;
    private float processingTimer = 0f;
    private SpriteRenderer vehicleSpriteRenderer;

    private Color[] randomColors = new Color[]
    {
        new Color(0.8f, 0.2f, 0.2f),    // Red
        new Color(0.2f, 0.4f, 0.8f),    // Blue
        new Color(0.2f, 0.7f, 0.3f),    // Green
        new Color(0.9f, 0.8f, 0.1f),    // Yellow
        new Color(0.7f, 0.3f, 0.8f),    // Purple
        new Color(0.1f, 0.8f, 0.8f),    // Cyan
        new Color(0.9f, 0.5f, 0.2f),    // Orange
        new Color(0.5f, 0.5f, 0.5f),    // Grey
        new Color(0.1f, 0.1f, 0.1f),    // Black
        new Color(0.9f, 0.9f, 0.9f),    // White
        new Color(0.6f, 0.3f, 0.1f),    // Brown
        new Color(0.0f, 0.6f, 0.4f),    // Teal
    };

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
        if (doorLeft != null)
            doorLeftOriginalPos = doorLeft.position;
        if (doorRight != null)
            doorRightOriginalPos = doorRight.position;
    }

    private Vector3 doorLeftOriginalPos;
    private Vector3 doorRightOriginalPos;

    private void Update()
    {
        if (doorsClosing)
        {
            CloseDoors();
        }
        else if (doorsOpening)
        {
            OpenDoors();
        }
        else if (isProcessing)
        {
            UpdateProcessing();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isProcessing || doorsClosing || doorsOpening) return;

        if (other.CompareTag("Player") || other.CompareTag("Player2"))
        {
            PlayerManager pm = other.GetComponent<PlayerManager>();
            if (pm != null && pm.IsInVehicle && pm.GetCurrentVehicle() != null)
            {
                playerVehicle = pm.GetCurrentVehicle();
                StartProcessing();
            }
        }
    }

    private void StartProcessing()
    {
        if (playerVehicle == null) return;

        PlayerManager pm = FindObjectOfType<PlayerManager>();
        if (pm == null) return;

        float playerMoney = pm.GetMoney();
        if (playerMoney < vehicleChangeCost)
        {
            Debug.Log($"Not enough money for vehicle change. Need ${vehicleChangeCost}, have ${playerMoney}");
            return;
        }

        isProcessing = true;
        isPlayerInside = true;
        colorChanged = false;
        rotationChanged = false;
        doorsClosing = true;
        processingTimer = 0f;

        vehicleSpriteRenderer = playerVehicle.GetComponent<SpriteRenderer>();
        originalVehicleRotation = playerVehicle.transform.rotation.eulerAngles;

        exitRotation = new Vector3(0, 0, 270f);
        exitPosition = playerVehicle.transform.position + new Vector3(0, 15f, 0);

        Debug.Log($"Scrapyard processing started. Cost: ${vehicleChangeCost}");
    }

    private void UpdateProcessing()
    {
        processingTimer += Time.deltaTime;

        if (!colorChanged && processingTimer >= doorCloseDelay + colorChangeDelay)
        {
            ChangeVehicleColor();
            colorChanged = true;
        }

        if (!rotationChanged && processingTimer >= doorCloseDelay + colorChangeDelay + rotationDelay)
        {
            RotateVehicleForExit();
            rotationChanged = true;
        }

        if (processingTimer >= doorCloseDelay + colorChangeDelay + rotationDelay + doorOpenDelay)
        {
            FinishProcessing();
        }
    }

    private void ChangeVehicleColor()
    {
        if (vehicleSpriteRenderer != null)
        {
            Color newColor = randomColors[Random.Range(0, randomColors.Length)];
            vehicleSpriteRenderer.color = newColor;
            Debug.Log($"Vehicle color changed to {newColor}");
        }

        PlayerManager pm = FindObjectOfType<PlayerManager>();
        if (pm != null)
        {
            pm.RemoveMoney(vehicleChangeCost);
            Debug.Log($"Paid ${vehicleChangeCost} for vehicle change");
        }

        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.SetWantedLevel(0, 0);
            WantedLevelManager.Instance.SetWantedLevel(0, 1);
            Debug.Log("Wanted level cleared!");
        }
    }

    private void RotateVehicleForExit()
    {
        if (playerVehicle != null)
        {
            playerVehicle.transform.rotation = Quaternion.Euler(exitRotation);
        }
    }

    private void FinishProcessing()
    {
        isProcessing = false;
        isPlayerInside = false;
        doorsOpening = true;
        processingTimer = 0f;

        Debug.Log("Processing complete!");
    }

    private void CloseDoors()
    {
        bool leftDone = false;
        bool rightDone = false;

        if (doorLeft != null)
        {
            float targetX = doorLeftOriginalPos.x - doorCloseDistance;
            doorLeft.position = Vector3.MoveTowards(doorLeft.position, 
                new Vector3(targetX, doorLeft.position.y, doorLeft.position.z), 
                doorSpeed * Time.deltaTime);
            if (Vector3.Distance(doorLeft.position, new Vector3(targetX, doorLeft.position.y, doorLeft.position.z)) < 0.1f)
                leftDone = true;
        }
        else
        {
            leftDone = true;
        }

        if (doorRight != null)
        {
            float targetX = doorRightOriginalPos.x + doorCloseDistance;
            doorRight.position = Vector3.MoveTowards(doorRight.position, 
                new Vector3(targetX, doorRight.position.y, doorRight.position.z), 
                doorSpeed * Time.deltaTime);
            if (Vector3.Distance(doorRight.position, new Vector3(targetX, doorRight.position.y, doorRight.position.z)) < 0.1f)
                rightDone = true;
        }
        else
        {
            rightDone = true;
        }

        if (leftDone && rightDone)
        {
            doorsClosing = false;
        }
    }

    private void OpenDoors()
    {
        bool leftDone = false;
        bool rightDone = false;

        if (doorLeft != null)
        {
            doorLeft.position = Vector3.MoveTowards(doorLeft.position, doorLeftOriginalPos, 
                doorSpeed * Time.deltaTime);
            if (Vector3.Distance(doorLeft.position, doorLeftOriginalPos) < 0.1f)
                leftDone = true;
        }
        else
        {
            leftDone = true;
        }

        if (doorRight != null)
        {
            doorRight.position = Vector3.MoveTowards(doorRight.position, doorRightOriginalPos, 
                doorSpeed * Time.deltaTime);
            if (Vector3.Distance(doorRight.position, doorRightOriginalPos) < 0.1f)
                rightDone = true;
        }
        else
        {
            rightDone = true;
        }

        if (leftDone && rightDone)
        {
            doorsOpening = false;
            playerVehicle = null;
        }
    }

    public bool IsProcessing() => isProcessing;
    public float GetCost() => vehicleChangeCost;
}
