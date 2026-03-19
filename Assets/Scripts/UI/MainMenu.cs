using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private int selectedOption = 0;
    private string[] menuOptions = new string[]
    {
        "New Game",
        "Load Game",
        "Settings",
        "Quit Game"
    };

    private bool inSettings = false;
    private int selectedSetting = 0;
    private System.Collections.Generic.List<string> settingsOptions = new System.Collections.Generic.List<string>();
    private bool settingsInitialized = false;

    private bool showQuitConfirm = false;
    private int quitConfirmSelection = 1;
    private string[] quitConfirmOptions = new string[] { "Yes", "No" };

    private float inputCooldown = 0.15f;
    private float inputTimer = 0f;
    private bool usingGamepad = false;
    private float axisDeadzone = 0.5f;

    private void Start()
    {
        Cursor.visible = false;
        
        if (FindObjectOfType<MainMenuBackground>() == null)
        {
            new GameObject("MainMenuBackground").AddComponent<MainMenuBackground>();
        }
        
        InitializeSettingsOptions();
    }

    private void InitializeSettingsOptions()
    {
        if (settingsInitialized) return;
        settingsInitialized = true;
        UpdateSettingsOptions();
    }

    private void UpdateSettingsOptions()
    {
        settingsOptions.Clear();
        settingsOptions.Add($"Player 1: {GetPlayer1ControllerDisplay()}");
        settingsOptions.Add($"Player 2: {GetPlayer2ControllerDisplay()}");
        settingsOptions.Add("Back");
    }

    private string GetPlayer1ControllerDisplay()
    {
        if (GameInputManager.Instance != null)
            return GameInputManager.Instance.GetPlayer1ControllerName();
        return "Keyboard";
    }

    private string GetPlayer2ControllerDisplay()
    {
        if (GameInputManager.Instance != null)
            return GameInputManager.Instance.GetPlayer2ControllerName();
        return "Auto";
    }

    private void CyclePlayer1Controller()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.CyclePlayer1Controller();
        }
    }

    private void CyclePlayer2Controller()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.CyclePlayer2Controller();
        }
    }

    private void Update()
    {
        CheckForGamepadInput();
        inputTimer -= Time.deltaTime;
        if (inputTimer > 0) return;

        if (showQuitConfirm)
        {
            HandleQuitConfirmInput();
        }
        else if (inSettings)
        {
            HandleSettingsInput();
        }
        else
        {
            HandleMenuInput();
        }
    }

    private void CheckForGamepadInput()
    {
        if (GameModeManager.Instance != null)
        {
            usingGamepad = GameModeManager.Instance.IsMenuUsingGamepad();
        }
    }

    private string GetMenuPrefix()
    {
        if (GameModeManager.Instance != null)
            return GameModeManager.Instance.GetMenuControllerPrefix();
        return "Gamepad1";
    }

    private bool WasUpPressed()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            return true;
        string prefix = GetMenuPrefix();
        if (usingGamepad && Input.GetAxis($"{prefix}_Vertical") > axisDeadzone)
            return true;
        return false;
    }

    private bool WasDownPressed()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            return true;
        string prefix = GetMenuPrefix();
        if (usingGamepad && Input.GetAxis($"{prefix}_Vertical") < -axisDeadzone)
            return true;
        return false;
    }

    private bool WasSelectPressed()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            return true;
        string prefix = GetMenuPrefix();
        if (usingGamepad && Input.GetButtonDown($"{prefix}_Fire"))
            return true;
        return false;
    }

    private bool WasBackPressed()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            return true;
        string prefix = GetMenuPrefix();
        if (usingGamepad && (Input.GetButtonDown($"{prefix}_B") || Input.GetButtonDown("Back")))
            return true;
        return false;
    }

    private void HandleMenuInput()
    {
        if (WasUpPressed())
        {
            selectedOption = (selectedOption - 1 + menuOptions.Length) % menuOptions.Length;
            inputTimer = inputCooldown;
        }

        if (WasDownPressed())
        {
            selectedOption = (selectedOption + 1) % menuOptions.Length;
            inputTimer = inputCooldown;
        }

        if (WasSelectPressed())
        {
            SelectMenuOption();
            inputTimer = inputCooldown;
        }
    }

    private void HandleSettingsInput()
    {
        if (!settingsInitialized)
        {
            InitializeSettingsOptions();
        }

        if (WasUpPressed())
        {
            selectedSetting = (selectedSetting - 1 + settingsOptions.Count) % settingsOptions.Count;
            inputTimer = inputCooldown;
        }

        if (WasDownPressed())
        {
            selectedSetting = (selectedSetting + 1) % settingsOptions.Count;
            inputTimer = inputCooldown;
        }

        if (WasSelectPressed())
        {
            SelectSettingOption();
            inputTimer = inputCooldown;
        }

        if (WasBackPressed())
        {
            inSettings = false;
            selectedSetting = 0;
            inputTimer = inputCooldown;
        }
    }

    private void HandleQuitConfirmInput()
    {
        if (WasUpPressed())
        {
            quitConfirmSelection = 0;
            inputTimer = inputCooldown;
        }

        if (WasDownPressed())
        {
            quitConfirmSelection = 1;
            inputTimer = inputCooldown;
        }

        if (WasSelectPressed())
        {
            if (quitConfirmSelection == 0)
            {
                ConfirmQuit();
            }
            else
            {
                showQuitConfirm = false;
            }
            inputTimer = inputCooldown;
        }

        if (WasBackPressed())
        {
            showQuitConfirm = false;
            inputTimer = inputCooldown;
        }
    }

    private void SelectMenuOption()
    {
        switch (selectedOption)
        {
            case 0: // New Game
                GameModeManager.Instance.SetGameMode(GameMode.SinglePlayer);
                LoadGameScene();
                break;
            case 1: // Load Game
                ShowLoadGameMenu();
                break;
            case 2: // Settings
                inSettings = true;
                selectedSetting = 0;
                break;
            case 3: // Quit Game
                showQuitConfirm = true;
                quitConfirmSelection = 1;
                break;
        }
    }

    private void SelectSettingOption()
    {
        if (!settingsInitialized)
        {
            InitializeSettingsOptions();
        }

        switch (selectedSetting)
        {
            case 0: // Cycle Player 1 Controller
                CyclePlayer1Controller();
                break;
            case 1: // Cycle Player 2 Controller
                CyclePlayer2Controller();
                break;
            case 2: // Back
                inSettings = false;
                selectedSetting = 0;
                break;
        }
        
        UpdateSettingsOptions();
    }

    private void ShowLoadGameMenu()
    {
        LoadGameMenu loadMenu = FindObjectOfType<LoadGameMenu>();
        if (loadMenu == null)
        {
            GameObject loadMenuObj = new GameObject("LoadGameMenu");
            loadMenu = loadMenuObj.AddComponent<LoadGameMenu>();
        }
        loadMenu.ShowMenu();
        gameObject.SetActive(false);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void OnGUI()
    {
        if (showQuitConfirm)
        {
            DrawQuitConfirmDialog();
            return;
        }

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 50;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.UpperCenter;
        titleStyle.normal.textColor = Color.white;

        float titleY = Screen.height / 5f;

        GUI.Label(new Rect(0, titleY - 30, Screen.width, 60), "WOLVERHAMPTON\nCRIME CITY", titleStyle);

        float yPos = Screen.height / 2.5f;

        GUIStyle optionStyle = new GUIStyle(GUI.skin.label);
        optionStyle.fontSize = 26;
        optionStyle.alignment = TextAnchor.UpperCenter;

        if (inSettings)
        {
            if (!settingsInitialized)
            {
                InitializeSettingsOptions();
            }

            for (int i = 0; i < settingsOptions.Count; i++)
            {
                Color textColor = i == selectedSetting ? Color.yellow : Color.white;
                optionStyle.normal.textColor = textColor;
                string prefix = i == selectedSetting ? "> " : "  ";
                GUI.Label(new Rect(0, yPos + i * 50, Screen.width, 40), prefix + settingsOptions[i], optionStyle);
            }

            string hint = usingGamepad ? "A/B: Select | Back: Go Back" : "ENTER: Select | ESC: Go Back";
            GUIStyle backHintStyle = new GUIStyle(GUI.skin.label);
            backHintStyle.fontSize = 16;
            backHintStyle.normal.textColor = Color.gray;
            backHintStyle.alignment = TextAnchor.LowerCenter;
            GUI.Label(new Rect(0, Screen.height - 80, Screen.width, 30), hint);
        }
        else
        {
            for (int i = 0; i < menuOptions.Length; i++)
            {
                Color textColor = i == selectedOption ? Color.yellow : Color.white;
                optionStyle.normal.textColor = textColor;
                string prefix = i == selectedOption ? "> " : "  ";
                GUI.Label(new Rect(0, yPos + i * 50, Screen.width, 40), prefix + menuOptions[i], optionStyle);
            }
        }

        string controls = usingGamepad ? "DPad/Left Stick: Move | A/Fire: Select" : "W/S: Move | ENTER: Select";
        GUIStyle instructionsStyle = new GUIStyle(GUI.skin.label);
        instructionsStyle.fontSize = 16;
        instructionsStyle.normal.textColor = Color.gray;
        instructionsStyle.alignment = TextAnchor.LowerCenter;
        GUI.Label(new Rect(0, Screen.height - 50, Screen.width, 30), controls);
    }

    private void DrawQuitConfirmDialog()
    {
        float boxWidth = 400f;
        float boxHeight = 180f;
        float boxX = (Screen.width - boxWidth) / 2f;
        float boxY = (Screen.height - boxHeight) / 2f;

        GUI.color = new Color(0, 0, 0, 0.9f);
        GUI.DrawTexture(new Rect(boxX - 5, boxY - 5, boxWidth + 10, boxHeight + 10), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUI.Box(new Rect(boxX, boxY, boxWidth, boxHeight), "");

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 24;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.UpperCenter;
        titleStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(boxX, boxY + 15, boxWidth, 40), "QUIT GAME?", titleStyle);

        GUIStyle messageStyle = new GUIStyle(GUI.skin.label);
        messageStyle.fontSize = 16;
        messageStyle.alignment = TextAnchor.UpperCenter;
        messageStyle.normal.textColor = Color.gray;
        GUI.Label(new Rect(boxX + 20, boxY + 55, boxWidth - 40, 40), "Are you sure?", messageStyle);

        float buttonWidth = 100f;
        float buttonHeight = 40f;
        float buttonY = boxY + boxHeight - 60;
        float yesX = boxX + (boxWidth - buttonWidth * 2 - 20) / 2f;
        float noX = yesX + buttonWidth + 20;

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 18;

        Color yesColor = quitConfirmSelection == 0 ? Color.yellow : Color.white;
        Color noColor = quitConfirmSelection == 1 ? Color.yellow : Color.white;

        GUI.color = yesColor;
        if (GUI.Button(new Rect(yesX, buttonY, buttonWidth, buttonHeight), "Yes", buttonStyle))
        {
            quitConfirmSelection = 0;
            ConfirmQuit();
        }

        GUI.color = noColor;
        if (GUI.Button(new Rect(noX, buttonY, buttonWidth, buttonHeight), "No", buttonStyle))
        {
            quitConfirmSelection = 1;
            showQuitConfirm = false;
        }
        
        GUI.color = Color.white;
    }

    private void ConfirmQuit()
    {
        Debug.Log("Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

public class LoadGameMenu : MonoBehaviour
{
    private int selectedSlot = 0;
    private int saveSlotCount = 5;
    private float inputCooldown = 0.15f;
    private float inputTimer = 0f;
    private bool menuActive = true;
    private bool usingGamepad = false;
    private float axisDeadzone = 0.5f;

    private void Start()
    {
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (!menuActive) return;

        CheckForGamepadInput();
        inputTimer -= Time.unscaledDeltaTime;
        if (inputTimer > 0) return;

        if (WasUpPressed())
        {
            selectedSlot = (selectedSlot - 1 + saveSlotCount + 1) % (saveSlotCount + 1);
            inputTimer = inputCooldown;
        }

        if (WasDownPressed())
        {
            selectedSlot = (selectedSlot + 1) % (saveSlotCount + 1);
            inputTimer = inputCooldown;
        }

        if (WasSelectPressed())
        {
            SelectSlot();
            inputTimer = inputCooldown;
        }

        if (WasBackPressed())
        {
            CloseMenu();
            inputTimer = inputCooldown;
        }
    }

    private void CheckForGamepadInput()
    {
        if (GameModeManager.Instance != null)
        {
            usingGamepad = GameModeManager.Instance.IsMenuUsingGamepad();
        }
    }

    private string GetMenuPrefix()
    {
        if (GameModeManager.Instance != null)
            return GameModeManager.Instance.GetMenuControllerPrefix();
        return "Gamepad1";
    }

    private bool WasUpPressed()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            return true;
        string prefix = GetMenuPrefix();
        if (usingGamepad && Input.GetAxis($"{prefix}_Vertical") > axisDeadzone)
            return true;
        return false;
    }

    private bool WasDownPressed()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            return true;
        string prefix = GetMenuPrefix();
        if (usingGamepad && Input.GetAxis($"{prefix}_Vertical") < -axisDeadzone)
            return true;
        return false;
    }

    private bool WasSelectPressed()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            return true;
        string prefix = GetMenuPrefix();
        if (usingGamepad && Input.GetButtonDown($"{prefix}_Fire"))
            return true;
        return false;
    }

    private bool WasBackPressed()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            return true;
        string prefix = GetMenuPrefix();
        if (usingGamepad && (Input.GetButtonDown($"{prefix}_B") || Input.GetButtonDown("Back")))
            return true;
        return false;
    }

    public void ShowMenu()
    {
        menuActive = true;
        Time.timeScale = 0f;
        gameObject.SetActive(true);
    }

    private void SelectSlot()
    {
        if (selectedSlot < saveSlotCount)
        {
            string slotName = $"slot_{selectedSlot}";
            if (SaveLoadSystem.Instance != null && SaveLoadSystem.Instance.SaveExists(slotName))
            {
                GameModeManager.Instance.SetGameMode(GameMode.SinglePlayer);
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");

                StartCoroutine(LoadAfterScene());
            }
        }
        else
        {
            CloseMenu();
        }
    }

    private System.Collections.IEnumerator LoadAfterScene()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        if (SaveLoadSystem.Instance != null)
        {
            SaveLoadSystem.Instance.LoadGame(selectedSlot);
        }
    }

    private void CloseMenu()
    {
        menuActive = false;
        Time.timeScale = 1f;
        MainMenu mainMenu = FindObjectOfType<MainMenu>();
        if (mainMenu != null)
        {
            mainMenu.gameObject.SetActive(true);
        }
        Destroy(gameObject);
    }

    private void OnGUI()
    {
        if (!menuActive) return;

        float boxWidth = 400f;
        float boxHeight = 350f;
        float boxX = (Screen.width - boxWidth) / 2f;
        float boxY = (Screen.height - boxHeight) / 2f;

        GUI.color = new Color(0, 0, 0, 0.95f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUI.Box(new Rect(boxX, boxY, boxWidth, boxHeight), "");

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 28;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.UpperCenter;
        titleStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(boxX, boxY + 15, boxWidth, 40), "LOAD GAME", titleStyle);

        float slotY = boxY + 70;
        float slotHeight = 40f;

        for (int i = 0; i < saveSlotCount; i++)
        {
            bool hasSave = SaveLoadSystem.Instance != null && SaveLoadSystem.Instance.SaveExists($"slot_{i}");

            Color textColor = i == selectedSlot ? Color.yellow : Color.white;
            string slotText = hasSave ? $"Slot {i + 1}" : $"Slot {i + 1} (Empty)";

            if (!hasSave)
            {
                textColor = i == selectedSlot ? Color.gray : new Color(0.5f, 0.5f, 0.5f);
                slotText = $"Slot {i + 1} (Empty)";
            }

            GUIStyle slotStyle = new GUIStyle(GUI.skin.label);
            slotStyle.fontSize = 22;
            slotStyle.alignment = TextAnchor.MiddleCenter;
            slotStyle.normal.textColor = textColor;

            string prefix = i == selectedSlot ? "> " : "  ";
            GUI.Label(new Rect(boxX, slotY + i * slotHeight, boxWidth, slotHeight), prefix + slotText, slotStyle);
        }

        GUIStyle backStyle = new GUIStyle(GUI.skin.label);
        backStyle.fontSize = 20;
        backStyle.alignment = TextAnchor.MiddleCenter;
        Color backColor = selectedSlot == saveSlotCount ? Color.yellow : Color.white;
        backStyle.normal.textColor = backColor;
        string backPrefix = selectedSlot == saveSlotCount ? "> " : "  ";
        GUI.Label(new Rect(boxX, slotY + saveSlotCount * slotHeight, boxWidth, slotHeight), backPrefix + "Back", backStyle);

        string hint = usingGamepad ? "DPad: Select | A: Load | B: Back" : "W/S: Select | ENTER: Load | ESC: Back";
        GUIStyle hintStyle = new GUIStyle(GUI.skin.label);
        hintStyle.fontSize = 14;
        hintStyle.normal.textColor = Color.gray;
        hintStyle.alignment = TextAnchor.LowerCenter;
        GUI.Label(new Rect(boxX, boxY + boxHeight - 35, boxWidth, 30), hint);
    }
}
