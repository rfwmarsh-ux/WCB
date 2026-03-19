using UnityEngine;

public enum ControllerType
{
    Keyboard,
    Gamepad1,
    Gamepad2,
    Auto
}

public enum GameMode
{
    SinglePlayer,
    Coop,
    Vs
}

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }

    [SerializeField] private GameMode currentMode = GameMode.SinglePlayer;
    [SerializeField] private bool player2Active = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetGameMode(GameMode mode)
    {
        currentMode = mode;
        player2Active = (mode == GameMode.Coop || mode == GameMode.Vs);
        
        Debug.Log($"Game Mode set to: {mode}");
    }

    public GameMode GetGameMode() => currentMode;
    public bool IsPlayer2Active() => player2Active;
    public bool IsCoop() => currentMode == GameMode.Coop;
    public bool IsVs() => currentMode == GameMode.Vs;
    public bool IsSinglePlayer() => currentMode == GameMode.SinglePlayer;
}

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }

    public ControllerType player1Controller = ControllerType.Auto;
    public ControllerType player2Controller = ControllerType.Auto;

    private float axisDeadzone = 0.2f;
    private string[] joystickNames;

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
        RefreshJoystickNames();
    }

    private void Update()
    {
        RefreshJoystickNames();
    }

    private void RefreshJoystickNames()
    {
        joystickNames = Input.GetJoystickNames();
    }

    public void SetPlayer1Controller(ControllerType type)
    {
        player1Controller = type;
        Debug.Log($"Player 1 controller set to: {type}");
    }

    public void SetPlayer2Controller(ControllerType type)
    {
        player2Controller = type;
        Debug.Log($"Player 2 controller set to: {type}");
    }

    public void CyclePlayer1Controller()
    {
        if (player1Controller == ControllerType.Auto)
            player1Controller = ControllerType.Gamepad1;
        else if (player1Controller == ControllerType.Gamepad1)
            player1Controller = joystickNames.Length >= 2 ? ControllerType.Gamepad2 : ControllerType.Keyboard;
        else if (player1Controller == ControllerType.Gamepad2)
            player1Controller = ControllerType.Keyboard;
        else
            player1Controller = ControllerType.Auto;
    }

    public void CyclePlayer2Controller()
    {
        if (player2Controller == ControllerType.Auto)
            player2Controller = ControllerType.Gamepad2;
        else if (player2Controller == ControllerType.Gamepad2)
            player2Controller = ControllerType.Gamepad1;
        else if (player2Controller == ControllerType.Gamepad1)
            player2Controller = ControllerType.Keyboard;
        else
            player2Controller = ControllerType.Auto;
    }

    public string GetControllerDisplayName(ControllerType type)
    {
        return type switch
        {
            ControllerType.Keyboard => "Keyboard",
            ControllerType.Gamepad1 => joystickNames.Length >= 1 && !string.IsNullOrEmpty(joystickNames[0]) ? $"Gamepad 1 ({joystickNames[0]})" : "Gamepad 1",
            ControllerType.Gamepad2 => joystickNames.Length >= 2 && !string.IsNullOrEmpty(joystickNames[1]) ? $"Gamepad 2 ({joystickNames[1]})" : "Gamepad 2",
            ControllerType.Auto => "Auto",
            _ => "Unknown"
        };
    }

    public string GetPlayer1ControllerName() => GetControllerDisplayName(player1Controller);
    public string GetPlayer2ControllerName() => GetControllerDisplayName(player2Controller);

    public string GetMenuControllerPrefix()
    {
        return GetJoystickPrefix(player1Controller);
    }

    public bool IsMenuUsingGamepad()
    {
        return IsUsingGamepad(player1Controller);
    }

    private bool IsUsingGamepad(ControllerType type)
    {
        if (type == ControllerType.Keyboard) return false;
        if (type == ControllerType.Auto)
        {
            return joystickNames.Length > 0 && !string.IsNullOrEmpty(joystickNames[0]);
        }
        return true;
    }

    private bool IsButtonPressed(string button)
    {
        return Input.GetButton(button);
    }

    private bool IsButtonDown(string button)
    {
        return Input.GetButtonDown(button);
    }

    private float GetAxis(string axis)
    {
        return Input.GetAxis(axis);
    }

    private float GetAxisRaw(string axis)
    {
        return Input.GetAxisRaw(axis);
    }

    public Vector2 GetPlayer1Move()
    {
        if (IsUsingGamepad(player1Controller))
        {
            string prefix = GetJoystickPrefix(player1Controller);
            float h = GetAxisRaw($"{prefix}_Horizontal");
            float v = GetAxisRaw($"{prefix}_Vertical");

            float dpadH = GetAxisRaw($"{prefix}_DPadHorizontal");
            float dpadV = GetAxisRaw($"{prefix}_DPadVertical");

            h = Mathf.Abs(h) > axisDeadzone ? h : dpadH;
            v = Mathf.Abs(v) > axisDeadzone ? v : dpadV;

            return new Vector2(h, v);
        }

        float x = 0, y = 0;
        if (Input.GetKey(KeyCode.W)) y = 1;
        if (Input.GetKey(KeyCode.S)) y = -1;
        if (Input.GetKey(KeyCode.A)) x = -1;
        if (Input.GetKey(KeyCode.D)) x = 1;
        return new Vector2(x, y).normalized;
    }

    public bool GetPlayer1Run()
    {
        Vector2 move = GetPlayer1Move();
        return move.magnitude > 0.1f;
    }

    public Vector2 GetPlayer2Move()
    {
        if (IsUsingGamepad(player2Controller))
        {
            string prefix = GetJoystickPrefix(player2Controller);
            float h = GetAxisRaw($"{prefix}_Horizontal");
            float v = GetAxisRaw($"{prefix}_Vertical");

            float dpadH = GetAxisRaw($"{prefix}_DPadHorizontal");
            float dpadV = GetAxisRaw($"{prefix}_DPadVertical");

            h = Mathf.Abs(h) > axisDeadzone ? h : dpadH;
            v = Mathf.Abs(v) > axisDeadzone ? v : dpadV;

            return new Vector2(h, v);
        }

        float x = 0, y = 0;
        if (Input.GetKey(KeyCode.UpArrow)) y = 1;
        if (Input.GetKey(KeyCode.DownArrow)) y = -1;
        if (Input.GetKey(KeyCode.LeftArrow)) x = -1;
        if (Input.GetKey(KeyCode.RightArrow)) x = 1;
        return new Vector2(x, y).normalized;
    }

    public bool GetPlayer2Run()
    {
        Vector2 move = GetPlayer2Move();
        return move.magnitude > 0.1f;
    }

    private string GetJoystickPrefix(ControllerType type)
    {
        if (type == ControllerType.Gamepad1) return "Gamepad1";
        if (type == ControllerType.Gamepad2) return "Gamepad2";
        if (type == ControllerType.Auto) return "Gamepad1";
        return "Keyboard";
    }

    public Vector2 GetPlayer1Aim()
    {
        if (IsUsingGamepad(player1Controller))
        {
            string prefix = GetJoystickPrefix(player1Controller);
            return new Vector2(GetAxis($"{prefix}_RightX"), GetAxis($"{prefix}_RightY"));
        }
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mousePos - PlayerController.Instance.transform.position).normalized;
    }

    public Vector2 GetPlayer2Aim()
    {
        if (IsUsingGamepad(player2Controller))
        {
            string prefix = GetJoystickPrefix(player2Controller);
            return new Vector2(GetAxis($"{prefix}_RightX"), GetAxis($"{prefix}_RightY"));
        }
        return Vector2.right;
    }

    public bool GetPlayer1Fire()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonPressed("Gamepad1_A");
        }
        return Input.GetMouseButton(0);
    }

    public bool GetPlayer2Fire()
    {
        if (IsUsingGamepad(player2Controller))
        {
            return IsButtonPressed("Gamepad2_A");
        }
        return Input.GetKey(KeyCode.RightControl);
    }

    public bool GetPlayer1FireDown()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonDown("Gamepad1_A");
        }
        return Input.GetMouseButtonDown(0);
    }

    public bool GetPlayer2FireDown()
    {
        if (IsUsingGamepad(player2Controller))
        {
            return IsButtonDown("Gamepad2_A");
        }
        return Input.GetKeyDown(KeyCode.RightControl);
    }

    public bool GetPlayer1Steal()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonDown("Gamepad1_X");
        }
        return Input.GetKeyDown(KeyCode.F);
    }

    public bool GetPlayer2Steal()
    {
        if (IsUsingGamepad(player2Controller))
        {
            return IsButtonDown("Gamepad2_X");
        }
        return Input.GetKeyDown(KeyCode.Keypad7);
    }

    public bool GetPlayer1CycleWeaponUp()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonDown("Gamepad1_RightBumper");
        }
        return Input.GetKeyDown(KeyCode.E);
    }

    public bool GetPlayer1CycleWeaponDown()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonDown("Gamepad1_LeftBumper");
        }
        return Input.GetKeyDown(KeyCode.Q);
    }

    public bool GetPlayer2CycleWeaponUp()
    {
        if (IsUsingGamepad(player2Controller))
        {
            return IsButtonDown("Gamepad2_RightBumper");
        }
        return Input.GetKeyDown(KeyCode.Keypad9);
    }

    public bool GetPlayer2CycleWeaponDown()
    {
        if (IsUsingGamepad(player2Controller))
        {
            return IsButtonDown("Gamepad2_LeftBumper");
        }
        return Input.GetKeyDown(KeyCode.Keypad8);
    }

    public float GetPlayer1Accelerate()
    {
        if (IsUsingGamepad(player1Controller))
        {
            float trigger = GetAxis("Gamepad1_RightTrigger");
            return trigger > axisDeadzone ? trigger : 0f;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) return 1f;
        return 0f;
    }

    public float GetPlayer1Brake()
    {
        if (IsUsingGamepad(player1Controller))
        {
            float trigger = GetAxis("Gamepad1_LeftTrigger");
            return trigger > axisDeadzone ? trigger : 0f;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) return 1f;
        return 0f;
    }

    public float GetPlayer2Accelerate()
    {
        if (IsUsingGamepad(player2Controller))
        {
            float trigger = GetAxis("Gamepad2_RightTrigger");
            return trigger > axisDeadzone ? trigger : 0f;
        }
        if (Input.GetKey(KeyCode.UpArrow)) return 1f;
        return 0f;
    }

    public float GetPlayer2Brake()
    {
        if (IsUsingGamepad(player2Controller))
        {
            float trigger = GetAxis("Gamepad2_LeftTrigger");
            return trigger > axisDeadzone ? trigger : 0f;
        }
        if (Input.GetKey(KeyCode.DownArrow)) return 1f;
        return 0f;
    }

    public bool GetPlayer1Handbrake()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonDown("Gamepad1_B");
        }
        return Input.GetKeyDown(KeyCode.Space);
    }

    public bool GetPlayer2Handbrake()
    {
        if (IsUsingGamepad(player2Controller))
        {
            return IsButtonDown("Gamepad2_B");
        }
        return Input.GetKeyDown(KeyCode.KeypadEnter);
    }

    public bool GetPlayer1Horn()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonDown("Gamepad1_Y");
        }
        return Input.GetKeyDown(KeyCode.H);
    }

    public bool GetPlayer2Horn()
    {
        if (IsUsingGamepad(player2Controller))
        {
            return IsButtonDown("Gamepad2_Y");
        }
        return Input.GetKeyDown(KeyCode.Keypad5);
    }

    public bool GetPlayer1Grenade()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonDown("Gamepad1_Start");
        }
        return Input.GetKeyDown(KeyCode.G);
    }

    public bool GetPlayer2Grenade()
    {
        if (IsUsingGamepad(player2Controller))
        {
            return IsButtonDown("Gamepad2_Start");
        }
        return Input.GetKeyDown(KeyCode.Keypad3);
    }

    public bool GetPlayer1Reload()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonDown("Gamepad1_Back");
        }
        return Input.GetKeyDown(KeyCode.R);
    }

    public bool GetPlayer2Reload()
    {
        if (IsUsingGamepad(player2Controller))
        {
            return IsButtonDown("Gamepad2_Back");
        }
        return Input.GetKeyDown(KeyCode.Keypad4);
    }

    public bool GetPlayer1Interact()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonDown("Gamepad1_DPadUp");
        }
        return Input.GetKeyDown(KeyCode.E);
    }

    public bool GetPlayer2Interact()
    {
        if (IsUsingGamepad(player2Controller))
        {
            return IsButtonDown("Gamepad2_DPadUp");
        }
        return Input.GetKeyDown(KeyCode.Keypad0);
    }

    public bool GetPlayer1Sprint()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonPressed("Gamepad1_LeftStick");
        }
        return Input.GetKey(KeyCode.LeftShift);
    }

    public bool GetPlayer2Sprint()
    {
        if (IsUsingGamepad(player2Controller))
        {
            return IsButtonPressed("Gamepad2_LeftStick");
        }
        return Input.GetKey(KeyCode.RightShift);
    }

    public bool GetMenuOpen()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonDown("Gamepad1_Start");
        }
        return Input.GetKeyDown(KeyCode.Escape);
    }

    public bool GetMapOpen()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonDown("Gamepad1_Select");
        }
        return Input.GetKeyDown(KeyCode.M);
    }

    public bool GetMenuBack()
    {
        if (IsUsingGamepad(player1Controller))
        {
            return IsButtonDown("Gamepad1_B");
        }
        return Input.GetKeyDown(KeyCode.Escape);
    }

    public int GetConnectedGamepadCount()
    {
        int count = 0;
        for (int i = 0; i < joystickNames.Length; i++)
        {
            if (!string.IsNullOrEmpty(joystickNames[i]))
                count++;
        }
        return count;
    }
}
