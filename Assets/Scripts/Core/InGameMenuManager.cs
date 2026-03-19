using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public enum MenuState
{
    None,
    Main,
    SaveGame,
    SaveWithName,
    LoadGame,
    Settings,
    Map,
    Minimap,
    Cannabis,
    ConfirmQuit,
    ConfirmReturn
}

public class InGameMenuManager : MonoBehaviour
{
    public static InGameMenuManager Instance { get; private set; }

    [Header("Menu State")]
    public MenuState currentState = MenuState.None;
    public bool isMenuOpen = false;
    public bool isMapOpen = false;

    [Header("Menu Options")]
    public int selectedIndex = 0;
    public int saveSlotCount = 5;
    public int settingsIndex = 0;
    public int confirmSelection = 1;

    [Header("Map Settings")]
    public float mapZoom = 1f;
    public float minZoom = 0.5f;
    public float maxZoom = 3f;
    public float zoomStep = 0.25f;
    public Vector2 mapPanOffset = Vector2.zero;

    [Header("Player Pins")]
    public List<MapPin> player1Pins = new List<MapPin>();
    public List<MapPin> player2Pins = new List<MapPin>();
    public int maxPinsPerPlayer = 5;

    [Header("Mission Objectives")]
    public List<MapMarker> missionMarkers = new List<MapMarker>();
    public bool showRouteHighlight = true;

    [Header("Mini-Map")]
    public float miniMapSize = 150f;
    public float miniMapScale = 0.15f;

    [Header("Cannabis Menu")]
    public int currentHouseIndex;
    public string currentHouseName = "";
    public string[] currentStrains = new string[3];
    public int selectedStrainIndex = 0;
    public int selectedQuantityIndex = 0;

    [Header("Volume Settings")]
    public float musicVolumeLevel = 0.7f;
    public float sfxVolumeLevel = 0.8f;
    public float volumeStep = 0.1f;

    [Header("Input Settings")]
    public float menuInputCooldown = 0.15f;
    private float lastInputTime = 0f;
    private bool usingGamepad = false;
    private float axisDeadzone = 0.5f;

    [Header("Save Name Input")]
    private string saveNameInput = "";
    private int saveNameIndex = 0;
    private string[] saveNameChars = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "_", "SPACE", "DELETE", "OK" };

    private List<string> mainMenuOptions = new List<string> { "Save Game", "Load Game", "Respawn", "Settings", "Return to Main Menu", "Quit Game" };
    private string[] quantityOptions = new string[] { "Henry - $35", "Q - $60", "Halfer - $100", "Ozzy - $180" };

    private List<string> settingsOptions = new List<string>();
    private bool settingsInitialized = false;

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
        settingsOptions.Add($"Music Volume: {Mathf.RoundToInt(musicVolumeLevel * 100)}%");
        settingsOptions.Add($"Sound FX Volume: {Mathf.RoundToInt(sfxVolumeLevel * 100)}%");
        settingsOptions.Add("Show Minimap: ON");
        settingsOptions.Add("Show Route: ON");
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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        CheckForGamepadInput();
        HandleMenuInput();
        
        if (isMapOpen)
        {
            HandleMapInput();
        }
    }

    private void CheckForGamepadInput()
    {
        if (GameInputManager.Instance != null)
        {
            usingGamepad = GameInputManager.Instance.IsMenuUsingGamepad();
        }
    }

    private string GetMenuPrefix()
    {
        if (GameInputManager.Instance != null)
            return GameInputManager.Instance.GetMenuControllerPrefix();
        return "Gamepad1";
    }

    private bool CanProcessInput()
    {
        if (Time.time - lastInputTime >= menuInputCooldown)
        {
            lastInputTime = Time.time;
            return true;
        }
        return false;
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

    private bool WasLeftPressed()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            return true;
        string prefix = GetMenuPrefix();
        if (usingGamepad && Input.GetAxis($"{prefix}_Horizontal") < -axisDeadzone)
            return true;
        return false;
    }

    private bool WasRightPressed()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            return true;
        string prefix = GetMenuPrefix();
        if (usingGamepad && Input.GetAxis($"{prefix}_Horizontal") > axisDeadzone)
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

    private bool WasMapPressed()
    {
        if (Input.GetKeyDown(KeyCode.M))
            return true;
        string prefix = GetMenuPrefix();
        if (usingGamepad && Input.GetButtonDown($"{prefix}_Submit"))
            return true;
        return false;
    }

    private bool WasMenuPressed()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            return true;
        if (usingGamepad && Input.GetButtonDown("Start"))
            return true;
        return false;
    }

    private void HandleMenuInput()
    {
        if (!isMenuOpen && !isMapOpen)
        {
            if (WasMenuPressed())
            {
                OpenMenu();
                return;
            }
            else if (WasMapPressed())
            {
                OpenMap();
                return;
            }
            return;
        }

        if (isMenuOpen)
        {
            if (currentState == MenuState.Cannabis)
            {
                HandleCannabisMenuInput();
                return;
            }

            if (currentState == MenuState.ConfirmQuit || currentState == MenuState.ConfirmReturn)
            {
                HandleConfirmInput();
                return;
            }

            if (currentState == MenuState.SaveWithName)
            {
                HandleSaveNameInput();
                return;
            }

            if (WasBackPressed())
            {
                if (CannabisPurchaseManager.Instance != null)
                {
                    CloseCannabisMenu();
                }
                else
                {
                    CloseMenu();
                }
                return;
            }

            if (WasUpPressed())
            {
                NavigateMenu(-1);
            }
            else if (WasDownPressed())
            {
                NavigateMenu(1);
            }

            if (WasSelectPressed())
            {
                SelectMenuOption();
            }

            if (currentState == MenuState.Settings)
            {
                if (WasLeftPressed())
                {
                    if (selectedIndex == 1) AdjustMusicVolume(-1);
                    else if (selectedIndex == 2) AdjustSfxVolume(-1);
                }
                else if (WasRightPressed())
                {
                    if (selectedIndex == 1) AdjustMusicVolume(1);
                    else if (selectedIndex == 2) AdjustSfxVolume(1);
                }
            }
        }

        if (isMapOpen)
        {
            if (WasBackPressed() || WasMapPressed() || WasMenuPressed())
            {
                CloseMap();
            }
        }
    }

    private void HandleMapInput()
    {
        if (usingGamepad)
        {
            string prefix = GetMenuPrefix();
            float hAxis = Input.GetAxis($"{prefix}_Horizontal");
            float vAxis = Input.GetAxis($"{prefix}_Vertical");
            
            if (Mathf.Abs(hAxis) > axisDeadzone)
            {
                mapPanOffset += new Vector2(-hAxis * 5f, 0);
            }
            if (Mathf.Abs(vAxis) > axisDeadzone)
            {
                mapPanOffset += new Vector2(0, vAxis * 5f);
            }

            if (Input.GetButtonDown($"{prefix}_ShoulderL") || Input.GetKeyDown(KeyCode.Minus))
            {
                mapZoom = Mathf.Clamp(mapZoom - zoomStep, minZoom, maxZoom);
            }
            if (Input.GetButtonDown($"{prefix}_ShoulderR") || Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                mapZoom = Mathf.Clamp(mapZoom + zoomStep, minZoom, maxZoom);
            }
        }
        else
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                mapZoom = Mathf.Clamp(mapZoom + scroll * zoomStep * 2, minZoom, maxZoom);
            }

            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                mapZoom = Mathf.Clamp(mapZoom + zoomStep, minZoom, maxZoom);
            }
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                mapZoom = Mathf.Clamp(mapZoom - zoomStep, minZoom, maxZoom);
            }

            if (Input.GetMouseButton(1))
            {
                mapPanOffset += new Vector2(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) * 2f;
            }
        }
    }

    public void OpenMenu()
    {
        isMenuOpen = true;
        currentState = MenuState.Main;
        selectedIndex = 0;
        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        isMenuOpen = false;
        currentState = MenuState.None;
        Time.timeScale = 1f;
    }

    public void OpenMap()
    {
        isMapOpen = true;
        currentState = MenuState.Map;
        mapZoom = 1f;
        mapPanOffset = Vector2.zero;
    }

    public void CloseMap()
    {
        isMapOpen = false;
        currentState = MenuState.None;
    }

    public void OpenCannabisMenu(int houseIndex, string houseName)
    {
        currentHouseIndex = houseIndex;
        currentHouseName = houseName;
        
        if (CannabisHouseManager.Instance != null)
        {
            currentStrains = CannabisHouseManager.Instance.GetStrainsForHouse(houseIndex);
        }
        
        isMenuOpen = true;
        currentState = MenuState.Cannabis;
        selectedIndex = 0;
        selectedStrainIndex = 0;
        selectedQuantityIndex = 0;
        Time.timeScale = 0f;
    }

    public void CloseCannabisMenu()
    {
        isMenuOpen = false;
        currentState = MenuState.None;
        Time.timeScale = 1f;
    }

    private void HandleCannabisMenuInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCannabisMenu();
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + 6) % 6;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % 6;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectedIndex < 3)
            {
                selectedStrainIndex = (selectedStrainIndex - 1 + 3) % 3;
            }
            else
            {
                selectedQuantityIndex = (selectedQuantityIndex - 1 + 4) % 4;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectedIndex < 3)
            {
                selectedStrainIndex = (selectedStrainIndex + 1) % 3;
            }
            else
            {
                selectedQuantityIndex = (selectedQuantityIndex + 1) % 4;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (selectedIndex == 3)
            {
                if (CannabisPurchaseManager.Instance != null)
                {
                    CannabisPurchaseManager.Instance.Purchase(selectedStrainIndex, selectedQuantityIndex);
                }
                CloseCannabisMenu();
            }
            else if (selectedIndex == 4)
            {
                if (CannabisHouseManager.Instance != null)
                {
                    CannabisHouseManager.Instance.RefreshStrains(currentHouseIndex);
                    currentStrains = CannabisHouseManager.Instance.GetStrainsForHouse(currentHouseIndex);
                }
            }
            else if (selectedIndex == 5)
            {
                CloseCannabisMenu();
            }
        }
    }

    private void NavigateMenu(int direction)
    {
        int maxIndex = GetCurrentOptionCount();
        selectedIndex = (selectedIndex + direction + maxIndex) % maxIndex;
    }

    private int GetCurrentOptionCount()
    {
        return currentState switch
        {
            MenuState.Main => mainMenuOptions.Count,
            MenuState.SaveGame => saveSlotCount + 1,
            MenuState.SaveWithName => 2,
            MenuState.LoadGame => saveSlotCount + 1,
            MenuState.Settings => settingsOptions.Count,
            MenuState.ConfirmQuit => 2,
            MenuState.ConfirmReturn => 2,
            _ => 0
        };
    }

    private void SelectMenuOption()
    {
        switch (currentState)
        {
            case MenuState.Main:
                HandleMainMenuSelection();
                break;
            case MenuState.SaveGame:
                HandleSaveSelection();
                break;
            case MenuState.LoadGame:
                HandleLoadSelection();
                break;
            case MenuState.Settings:
                HandleSettingsSelection();
                break;
        }
    }

    private void HandleMainMenuSelection()
    {
        switch (selectedIndex)
        {
            case 0:
                currentState = MenuState.SaveWithName;
                saveNameInput = "";
                saveNameIndex = 0;
                selectedIndex = 0;
                break;
            case 1:
                currentState = MenuState.LoadGame;
                selectedIndex = 0;
                break;
            case 2:
                RespawnPlayer();
                break;
            case 3:
                currentState = MenuState.Settings;
                selectedIndex = 0;
                break;
            case 4:
                currentState = MenuState.ConfirmReturn;
                confirmSelection = 1;
                break;
            case 5:
                currentState = MenuState.ConfirmQuit;
                confirmSelection = 1;
                break;
        }
    }

    private void RespawnPlayer()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.RespawnPlayer();
            Debug.Log("Player respawned!");
        }
        else if (PlayerController.Instance != null)
        {
            PlayerController.Instance.RespawnAtSafeLocation();
            Debug.Log("Player respawned!");
        }
        CloseMenu();
    }

    private void HandleSaveSelection()
    {
        if (selectedIndex < saveSlotCount)
        {
            SaveLoadSystem.Instance.SaveGame(selectedIndex);
            Debug.Log($"Game saved to slot {selectedIndex}");
            CloseMenu();
        }
        else
        {
            currentState = MenuState.Main;
            selectedIndex = 0;
        }
    }

    private void HandleLoadSelection()
    {
        if (selectedIndex < saveSlotCount)
        {
            if (SaveLoadSystem.Instance.SaveExists($"slot_{selectedIndex}"))
            {
                SaveLoadSystem.Instance.LoadGame(selectedIndex);
                CloseMenu();
            }
        }
        else
        {
            currentState = MenuState.Main;
            selectedIndex = 0;
        }
    }

    private void HandleSettingsSelection()
    {
        if (!CanProcessInput()) return;

        if (currentState == MenuState.Settings)
        {
            InitializeSettingsOptions();
        }

        if (selectedIndex == settingsOptions.Count - 1)
        {
            currentState = MenuState.Main;
            selectedIndex = 0;
        }
        else if (selectedIndex == 0)
        {
            CyclePlayer1Controller();
        }
        else if (selectedIndex == 1)
        {
            CyclePlayer2Controller();
        }
        else if (selectedIndex == 2)
        {
            AdjustMusicVolume(1);
        }
        else if (selectedIndex == 3)
        {
            AdjustSfxVolume(1);
        }
        else if (selectedIndex == 4)
        {
            settingsOptions[4] = settingsOptions[4].Contains("ON") 
                ? "Show Minimap: OFF" 
                : "Show Minimap: ON";
        }
        else if (selectedIndex == 5)
        {
            settingsOptions[5] = settingsOptions[5].Contains("ON") 
                ? "Show Route: OFF" 
                : "Show Route: ON";
            showRouteHighlight = settingsOptions[5].Contains("ON");
        }
        
        UpdateSettingsOptions();
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

    private void AdjustMusicVolume(int direction)
    {
        musicVolumeLevel = Mathf.Clamp01(musicVolumeLevel + direction * volumeStep);
        int percent = Mathf.RoundToInt(musicVolumeLevel * 100);
        
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetMusicVolume(musicVolumeLevel);
            if (musicVolumeLevel > 0)
            {
                MusicManager.Instance.SetMusicEnabled(true);
            }
        }
    }

    private void AdjustSfxVolume(int direction)
    {
        sfxVolumeLevel = Mathf.Clamp01(sfxVolumeLevel + direction * volumeStep);
        
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetSfxVolume(sfxVolumeLevel);
            if (sfxVolumeLevel > 0)
            {
                MusicManager.Instance.SetSfxEnabled(true);
            }
        }
        
        UpdateSettingsOptions();
    }

    private void HandleConfirmInput()
    {
        if (!CanProcessInput()) return;

        if (WasLeftPressed() || WasUpPressed())
        {
            confirmSelection = 0;
        }
        else if (WasRightPressed() || WasDownPressed())
        {
            confirmSelection = 1;
        }
        else if (WasSelectPressed())
        {
            if (confirmSelection == 0)
            {
                if (currentState == MenuState.ConfirmQuit)
                {
                    ConfirmQuit();
                }
                else if (currentState == MenuState.ConfirmReturn)
                {
                    ConfirmReturnToMainMenu();
                }
            }
            else
            {
                currentState = MenuState.Main;
                confirmSelection = 1;
            }
        }
        else if (WasBackPressed())
        {
            currentState = MenuState.Main;
            confirmSelection = 1;
        }
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

    private void ConfirmReturnToMainMenu()
    {
        Debug.Log("Returning to main menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    private void HandleSaveNameInput()
    {
        if (!CanProcessInput()) return;

        if (WasUpPressed())
        {
            saveNameIndex = (saveNameIndex - 1 + saveNameChars.Length) % saveNameChars.Length;
        }
        else if (WasDownPressed())
        {
            saveNameIndex = (saveNameIndex + 1) % saveNameChars.Length;
        }
        else if (WasLeftPressed())
        {
            if (saveNameInput.Length > 0)
            {
                saveNameInput = saveNameInput.Substring(0, saveNameInput.Length - 1);
            }
        }
        else if (WasRightPressed() || WasSelectPressed())
        {
            string selectedChar = saveNameChars[saveNameIndex];
            if (selectedChar == "DELETE")
            {
                if (saveNameInput.Length > 0)
                {
                    saveNameInput = saveNameInput.Substring(0, saveNameInput.Length - 1);
                }
            }
            else if (selectedChar == "OK")
            {
                if (!string.IsNullOrEmpty(saveNameInput))
                {
                    SaveWithCustomName(saveNameInput);
                }
                currentState = MenuState.Main;
            }
            else if (selectedChar == "SPACE")
            {
                saveNameInput += "_";
            }
            else
            {
                AddCharacter(selectedChar);
            }
        }
        else if (WasBackPressed())
        {
            currentState = MenuState.Main;
        }
    }

    private void AddCharacter(string c)
    {
        if (saveNameInput.Length < 20)
        {
            saveNameInput += c;
        }
    }

    private void SaveWithCustomName(string name)
    {
        if (SaveLoadSystem.Instance != null)
        {
            SaveLoadSystem.Instance.SaveGame(name);
            Debug.Log($"Game saved as: {name}");
        }
        CloseMenu();
    }

    public void AddPin(int playerId, Vector2 position, string label)
    {
        var pins = playerId == 1 ? player1Pins : player2Pins;
        if (pins.Count >= maxPinsPerPlayer) return;

        pins.Add(new MapPin { position = position, label = label });
    }

    public void RemovePin(int playerId, int index)
    {
        var pins = playerId == 1 ? player1Pins : player2Pins;
        if (index >= 0 && index < pins.Count)
        {
            pins.RemoveAt(index);
        }
    }

    public List<MapPin> GetVisiblePins(int playerId, bool isCoop)
    {
        if (isCoop)
        {
            var allPins = new List<MapPin>(player1Pins);
            allPins.AddRange(player2Pins);
            return allPins;
        }
        return playerId == 1 ? player1Pins : player2Pins;
    }

    public void AddMissionMarker(Vector2 position, string label, MissionMarkerType type)
    {
        missionMarkers.Add(new MapMarker { position = position, label = label, type = type });
    }

    public void ClearMissionMarkers()
    {
        missionMarkers.Clear();
    }

    public void OnGUI()
    {
        if (!isMenuOpen && !isMapOpen) return;

        GUI.skin.label.fontSize = 20;
        GUI.skin.button.fontSize = 18;

        bool isSplit = SplitScreenManager.Instance != null && SplitScreenManager.Instance.isSplitScreenActive;

        if (isMenuOpen)
        {
            if (currentState == MenuState.Cannabis)
            {
                DrawCannabisMenu();
                return;
            }

            if (currentState == MenuState.ConfirmQuit)
            {
                DrawConfirmDialog("QUIT GAME?", "Are you sure?\nAny unsaved progress will be lost!", true);
                return;
            }

            if (currentState == MenuState.ConfirmReturn)
            {
                DrawConfirmDialog("RETURN TO MENU?", "Are you sure?\nAny unsaved progress will be lost!", true);
                return;
            }

            if (currentState == MenuState.SaveWithName)
            {
                DrawSaveNameMenu();
                return;
            }

            if (isSplit)
            {
                DrawMenuForPlayer(1, 0, 0, Screen.width / 2, Screen.height);
                DrawMenuForPlayer(2, Screen.width / 2, 0, Screen.width / 2, Screen.height);
            }
            else
            {
                DrawMenu();
            }
        }

        if (isMapOpen)
        {
            if (isSplit)
            {
                DrawFullScreenMapForPlayer(1, 0, 0, Screen.width / 2, Screen.height);
                DrawFullScreenMapForPlayer(2, Screen.width / 2, 0, Screen.width / 2, Screen.height);
            }
            else
            {
                DrawFullScreenMap();
            }
        }
    }

    private void DrawConfirmDialog(string title, string message, bool hasWarning)
    {
        float boxWidth = 450f;
        float boxHeight = hasWarning ? 220f : 180f;
        float boxX = (Screen.width - boxWidth) / 2f;
        float boxY = (Screen.height - boxHeight) / 2f;

        GUI.color = new Color(0, 0, 0, 0.95f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUI.Box(new Rect(boxX, boxY, boxWidth, boxHeight), "");

        GUI.color = new Color(0, 0, 0, 0.5f);
        GUI.DrawTexture(new Rect(boxX + 5, boxY + 5, boxWidth - 10, boxHeight - 10), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 28;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.UpperCenter;
        titleStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(boxX, boxY + 15, boxWidth, 40), title, titleStyle);

        GUIStyle messageStyle = new GUIStyle(GUI.skin.label);
        messageStyle.fontSize = 18;
        messageStyle.alignment = TextAnchor.UpperCenter;
        messageStyle.normal.textColor = hasWarning ? Color.yellow : Color.gray;
        GUI.Label(new Rect(boxX + 20, boxY + 60, boxWidth - 40, 50), message, messageStyle);

        float buttonWidth = 120f;
        float buttonHeight = 45f;
        float buttonY = boxY + boxHeight - 65;
        float yesX = boxX + (boxWidth - buttonWidth * 2 - 20) / 2f;
        float noX = yesX + buttonWidth + 20;

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 20;

        Color yesColor = confirmSelection == 0 ? Color.yellow : Color.white;
        Color noColor = confirmSelection == 1 ? Color.yellow : Color.white;

        GUI.color = yesColor;
        if (GUI.Button(new Rect(yesX, buttonY, buttonWidth, buttonHeight), "Yes", buttonStyle))
        {
            confirmSelection = 0;
            if (currentState == MenuState.ConfirmQuit)
                ConfirmQuit();
            else if (currentState == MenuState.ConfirmReturn)
                ConfirmReturnToMainMenu();
        }

        GUI.color = noColor;
        if (GUI.Button(new Rect(noX, buttonY, buttonWidth, buttonHeight), "No", buttonStyle))
        {
            confirmSelection = 1;
            currentState = MenuState.Main;
        }

        GUI.color = Color.white;
    }

    private void DrawSaveNameMenu()
    {
        float boxWidth = 500f;
        float boxHeight = 400f;
        float boxX = (Screen.width - boxWidth) / 2f;
        float boxY = (Screen.height - boxHeight) / 2f;

        GUI.color = new Color(0, 0, 0, 0.95f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUI.Box(new Rect(boxX, boxY, boxWidth, boxHeight), "");

        GUI.color = new Color(0, 0, 0, 0.5f);
        GUI.DrawTexture(new Rect(boxX + 5, boxY + 5, boxWidth - 10, boxHeight - 10), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 26;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.UpperCenter;
        titleStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(boxX, boxY + 10, boxWidth, 35), "SAVE GAME", titleStyle);

        GUIStyle inputStyle = new GUIStyle(GUI.skin.label);
        inputStyle.fontSize = 22;
        inputStyle.alignment = TextAnchor.UpperCenter;
        inputStyle.normal.textColor = Color.cyan;
        GUI.Label(new Rect(boxX, boxY + 50, boxWidth, 30), "Enter save name:", inputStyle);

        string displayName = string.IsNullOrEmpty(saveNameInput) ? "_" : saveNameInput;
        if (saveNameInput.Length >= 20)
        {
            displayName = saveNameInput.Substring(0, 20) + "_";
        }

        GUIStyle nameStyle = new GUIStyle(GUI.skin.label);
        nameStyle.fontSize = 26;
        nameStyle.fontStyle = FontStyle.Bold;
        nameStyle.alignment = TextAnchor.UpperCenter;
        nameStyle.normal.textColor = Color.yellow;

        GUI.color = new Color(0, 0, 0, 0.3f);
        GUI.DrawTexture(new Rect(boxX + 50, boxY + 80, boxWidth - 100, 35), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(boxX + 50, boxY + 80, boxWidth - 100, 35), displayName, nameStyle);

        int charsPerRow = 13;
        int rows = 3;
        float charButtonSize = 32f;
        float startX = boxX + (boxWidth - charsPerRow * charButtonSize) / 2f;
        float startY = boxY + 125;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < charsPerRow; j++)
            {
                int index = i * charsPerRow + j;
                if (index >= saveNameChars.Length) break;

                string c = saveNameChars[index];
                float x = startX + j * charButtonSize;
                float y = startY + i * charButtonSize;

                Color charColor = index == saveNameIndex ? Color.yellow : Color.white;
                if (c == "DELETE") charColor = Color.red;
                else if (c == "OK") charColor = Color.green;
                else if (c == "SPACE") c = "_";

                GUIStyle charStyle = new GUIStyle(GUI.skin.button);
                charStyle.fontSize = 14;
                charStyle.fontStyle = FontStyle.Bold;

                GUI.color = charColor;
                if (GUI.Button(new Rect(x, y, charButtonSize - 2, charButtonSize - 2), c, charStyle))
                {
                    saveNameIndex = index;
                    if (c == "DELETE")
                    {
                        if (saveNameInput.Length > 0)
                            saveNameInput = saveNameInput.Substring(0, saveNameInput.Length - 1);
                    }
                    else if (c == "_")
                    {
                        saveNameInput += "_";
                    }
                    else if (c == "OK")
                    {
                        if (!string.IsNullOrEmpty(saveNameInput))
                        {
                            SaveWithCustomName(saveNameInput);
                        }
                        currentState = MenuState.Main;
                    }
                    else
                    {
                        AddCharacter(c);
                    }
                }
            }
        }

        GUIStyle hintStyle = new GUIStyle(GUI.skin.label);
        hintStyle.fontSize = 14;
        hintStyle.alignment = TextAnchor.LowerCenter;
        hintStyle.normal.textColor = Color.gray;
        GUI.Label(new Rect(boxX, boxY + boxHeight - 40, boxWidth, 20), "W/S: Select | A: Delete | D: Add | ENTER: Save | ESC: Cancel", hintStyle);

        GUI.color = Color.white;
    }

    private void DrawMenuForPlayer(int playerId, int x, int y, int width, int height)
    {
        GUI.color = playerId == 1 ? Color.white : Color.cyan;
        GUI.Box(new Rect(x, y, width, height), $"PLAYER {playerId} - {GetMenuTitle()}");
        GUI.color = Color.white;

        int boxWidth = width - 40;
        int boxHeight = height - 60;
        int startX = x + 20;
        int startY = y + 40;
        var options = GetCurrentOptions();

        for (int i = 0; i < options.Count; i++)
        {
            Color originalColor = GUI.color;
            if (i == selectedIndex)
            {
                GUI.color = Color.yellow;
            }

            if (GUI.Button(new Rect(startX, startY + i * 35, boxWidth, 30), options[i]))
            {
                selectedIndex = i;
                SelectMenuOption();
            }

            GUI.color = originalColor;
        }
    }

    private void DrawMenu()
    {
        int boxWidth = 400;
        int boxHeight = 300;
        int x = (Screen.width - boxWidth) / 2;
        int y = (Screen.height - boxHeight) / 2;

        GUI.Box(new Rect(x, y, boxWidth, boxHeight), GetMenuTitle());

        int startY = y + 40;
        var options = GetCurrentOptions();

        for (int i = 0; i < options.Count; i++)
        {
            Color originalColor = GUI.color;
            if (i == selectedIndex)
            {
                GUI.color = Color.yellow;
            }

            if (GUI.Button(new Rect(x + 20, startY + i * 35, boxWidth - 40, 30), options[i]))
            {
                selectedIndex = i;
                SelectMenuOption();
            }

            GUI.color = originalColor;
        }
    }

    private void DrawCannabisMenu()
    {
        int boxWidth = 500;
        int boxHeight = 450;
        int x = (Screen.width - boxWidth) / 2;
        int y = (Screen.height - boxHeight) / 2;

        GUI.Box(new Rect(x, y, boxWidth, boxHeight), $"CANNABIS - {currentHouseName}");

        GUI.skin.label.fontSize = 16;
        
        GUI.Label(new Rect(x + 20, y + 35, boxWidth - 40, 25), "SELECT STRAIN (Left/Right to change):");
        
        for (int i = 0; i < 3; i++)
        {
            Color strainColor = (i == selectedStrainIndex) ? Color.green : Color.white;
            string marker = (i == selectedStrainIndex) ? ">> " : "   ";
            GUI.color = strainColor;
            GUI.Label(new Rect(x + 30, y + 60 + i * 25, boxWidth - 60, 25), $"{marker}{currentStrains[i]}");
        }

        GUI.color = Color.white;
        GUI.Label(new Rect(x + 20, y + 145, boxWidth - 40, 25), "SELECT QUANTITY (Left/Right to change):");
        
        for (int i = 0; i < 4; i++)
        {
            Color quantColor = (i == selectedQuantityIndex) ? Color.yellow : Color.white;
            string marker = (i == selectedQuantityIndex) ? ">> " : "   ";
            GUI.color = quantColor;
            GUI.Label(new Rect(x + 30, y + 170 + i * 25, boxWidth - 60, 25), $"{marker}{quantityOptions[i]}");
        }

        GUI.color = Color.white;
        string info = CannabisHouseManager.Instance != null 
            ? $"+{CannabisHouseManager.Instance.healthBoostAmount} Health Boost | Duration: {CannabisHouseManager.Instance.durationMinutes[selectedQuantityIndex]} min"
            : "+50 Health Boost";
        GUI.Label(new Rect(x + 20, y + 280, boxWidth - 40, 25), info);

        GUI.skin.label.fontSize = 18;
        
        bool isOutOfStock = CannabisHouseManager.Instance != null && CannabisHouseManager.Instance.IsHouseOutOfStock(currentHouseIndex);
        
        if (isOutOfStock)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(x + 20, y + 310, boxWidth - 40, 25), "SORRY - WE'RE ALL OUT OF STOCK!");
            GUI.color = Color.white;
            if (GUI.Button(new Rect(x + 20, y + 350, boxWidth - 40, 30), "Leave (Esc)"))
            {
                CloseCannabisMenu();
            }
        }
        else
        {
            string[] cannabisOptions = new string[] { "Buy (Enter)", "Leave (Esc)" };
            
            for (int i = 0; i < 2; i++)
            {
                Color optColor = (i + 3 == selectedIndex) ? Color.green : Color.white;
                GUI.color = optColor;
                string prefix = (i + 3 == selectedIndex) ? "> " : "  ";
                if (GUI.Button(new Rect(x + 20, y + 320 + i * 35, boxWidth - 40, 30), prefix + cannabisOptions[i]))
                {
                    selectedIndex = i + 3;
                    if (i == 0)
                    {
                        if (CannabisPurchaseManager.Instance != null)
                            CannabisPurchaseManager.Instance.Purchase(selectedStrainIndex, selectedQuantityIndex);
                        CloseCannabisMenu();
                    }
                    else
                    {
                        CloseCannabisMenu();
                    }
                }
            }
        }

        GUI.color = Color.white;
    }

    private string GetMenuTitle()
    {
        return currentState switch
        {
            MenuState.Main => "PAUSED",
            MenuState.SaveGame => "SAVE GAME",
            MenuState.LoadGame => "LOAD GAME",
            MenuState.Settings => "SETTINGS",
            MenuState.SaveWithName => "SAVE GAME",
            MenuState.ConfirmQuit => "QUIT GAME?",
            MenuState.ConfirmReturn => "RETURN TO MENU?",
            _ => "MENU"
        };
    }

    private List<string> GetCurrentOptions()
    {
        return currentState switch
        {
            MenuState.Main => mainMenuOptions,
            MenuState.Settings => settingsOptions,
            MenuState.SaveGame => GetSaveSlotOptions(),
            MenuState.LoadGame => GetLoadSlotOptions(),
            _ => new List<string>()
        };
    }

    private List<string> GetSaveSlotOptions()
    {
        var options = new List<string>();
        for (int i = 0; i < saveSlotCount; i++)
        {
            bool hasSave = SaveLoadSystem.Instance != null && SaveLoadSystem.Instance.SaveExists($"slot_{i}");
            options.Add(hasSave ? $"Slot {i + 1} (Overwrite)" : $"Slot {i + 1}");
        }
        options.Add("Back");
        return options;
    }

    private List<string> GetLoadSlotOptions()
    {
        var options = new List<string>();
        for (int i = 0; i < saveSlotCount; i++)
        {
            bool hasSave = SaveLoadSystem.Instance != null && SaveLoadSystem.Instance.SaveExists($"slot_{i}");
            options.Add(hasSave ? $"Slot {i + 1}" : $"Slot {i + 1} (Empty)");
        }
        options.Add("Back");
        return options;
    }

    private void DrawFullScreenMap()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
        
        int mapSize = Mathf.Min(Screen.width, Screen.height) - 100;
        int mapX = (Screen.width - mapSize) / 2 + (int)mapPanOffset.x;
        int mapY = (Screen.height - mapSize) / 2 + (int)mapPanOffset.y;

        GUI.Box(new Rect(mapX - 10, mapY - 10, mapSize + 20, mapSize + 20), "MAP");

        DrawMapContent(mapX, mapY, mapSize);

        string zoomText = $"Zoom: {mapZoom:F1}x";
        GUI.Label(new Rect(mapX, mapY + mapSize + 10, 200, 30), zoomText);

        string controls = usingGamepad 
            ? "L/R Triggers: Zoom | Left Stick: Pan | B/Start: Close"
            : "Scroll/Plus-Minus: Zoom | Right Click: Pan | M/Esc: Close";
        GUI.Label(new Rect(mapX, mapY - 35, 500, 30), controls);
    }

    private void DrawFullScreenMapForPlayer(int playerId, int x, int y, int width, int height)
    {
        GUI.color = playerId == 1 ? Color.white : Color.cyan;
        GUI.Box(new Rect(x, y, width, height), $"PLAYER {playerId} - MAP");
        GUI.color = Color.white;
        
        int mapSize = Mathf.Min(width, height) - 80;
        int mapX = x + (width - mapSize) / 2 + (int)mapPanOffset.x;
        int mapY = y + (height - mapSize) / 2 + (int)mapPanOffset.y;

        DrawMapContentForPlayer(playerId, mapX, mapY, mapSize, x, y);

        string zoomText = $"Zoom: {mapZoom:F1}x";
        GUI.Label(new Rect(mapX, mapY + mapSize + 10, 200, 30), zoomText);

        string controls = "Scroll: Zoom | Back/Esc: Close";
        GUI.Label(new Rect(mapX, mapY - 35, 300, 30), controls);
    }

    private void DrawMapContentForPlayer(int playerId, int x, int y, int size, int viewX, int viewY)
    {
        float scale = size / 1000f * mapZoom;
        Vector2 center = new Vector2(x + size / 2, y + size / 2);

        Vector2 playerPos = playerId == 1 
            ? (PlayerManager.Instance?.transform.position ?? Vector2.zero)
            : (Player2Manager.Instance?.transform.position ?? Vector2.zero);

        if (AreaManager.Instance != null)
        {
            foreach (var area in AreaManager.Instance.GetAreasOfType(AreaType.Park))
            {
                DrawMapMarkerForPlayer(area.Position, playerPos, scale, Color.green, 8f, false, viewX, viewY);
            }
        }

        DrawUsableBuildingMarkersForPlayer(playerPos, center, scale, viewX, viewY);

        if (showRouteHighlight && missionMarkers.Count > 0)
        {
            DrawMissionRoute(center, scale);
        }

        DrawPlayerPinsForPlayer(playerId, center, scale, viewX, viewY);

        DrawPlayersOnMapForPlayer(playerId, center, scale, viewX, viewY, playerPos);
    }

    private void DrawMapContent(int x, int y, int size)
    {
        float scale = size / 1000f * mapZoom;
        Vector2 center = new Vector2(x + size / 2, y + size / 2);

        if (AreaManager.Instance != null)
        {
            foreach (var area in AreaManager.Instance.GetAreasOfType(AreaType.Park))
            {
                DrawMapMarker(area.Position, scale, Color.green, 8f, false);
            }
        }

        DrawUsableBuildingMarkers(center, scale);

        if (showRouteHighlight && missionMarkers.Count > 0)
        {
            DrawMissionRoute(center, scale);
        }

        DrawPlayerPins(center, scale);

        DrawPlayersOnMap(center, scale);
    }

    private void DrawUsableBuildingMarkers(Vector2 center, float scale)
    {
        if (VeterinaryCentreManager.Instance != null)
        {
            foreach (var centre in VeterinaryCentreManager.Instance.GetAllCentres())
            {
                DrawMapMarker(centre.transform.position, scale, Color.cyan, 10f, true, "Vet");
            }
        }

        if (ScrapyardManager.Instance != null)
        {
            foreach (var yard in ScrapyardManager.Instance.GetAllScrapyards())
            {
                DrawMapMarker(yard.transform.position, scale, Color.magenta, 10f, true, "Scrapyard");
            }
        }

        if (GunShopManager.Instance != null)
        {
            foreach (var shop in GunShopManager.Instance.GetAllGunShops())
            {
                DrawMapMarker(shop.transform.position, scale, Color.red, 10f, true, "Gun Shop");
            }
        }

        if (PoliceStationManager.Instance != null)
        {
            foreach (var station in PoliceStationManager.Instance.GetAllStations())
            {
                DrawMapMarker(station.Position, scale, Color.blue, 12f, true, "Police");
            }
        }
    }

    private void DrawMapMarker(Vector2 worldPos, float scale, Color color, float baseSize, bool showLabel, string label = "")
    {
        Vector2 screenPos = new Vector2(worldPos.x * scale, worldPos.y * scale);
        int size = (int)(baseSize * mapZoom);
        GUI.color = color;
        GUI.DrawTexture(new Rect(screenPos.x - size / 2, screenPos.y - size / 2, size, size), Texture2D.whiteTexture);
        
        if (showLabel && mapZoom >= 1.5f && !string.IsNullOrEmpty(label))
        {
            GUI.color = Color.white;
            GUI.Label(new Rect(screenPos.x + size, screenPos.y, 100, 20), label);
        }
        GUI.color = Color.white;
    }

    private void DrawMissionRoute(Vector2 center, float scale)
    {
        if (missionMarkers.Count < 2) return;

        for (int i = 0; i < missionMarkers.Count - 1; i++)
        {
            Vector2 start = missionMarkers[i].position;
            Vector2 end = missionMarkers[i + 1].position;
            
            Vector2 startPos = new Vector2(start.x * scale, start.y * scale);
            Vector2 endPos = new Vector2(end.x * scale, end.y * scale);
        }
    }

    private void DrawPlayerPins(Vector2 center, float scale)
    {
        var pins = GetVisiblePins(1, GameModeManager.Instance?.IsCoop() ?? false);
        foreach (var pin in pins)
        {
            DrawMapMarker(pin.position, scale, Color.yellow, 8f, true, pin.label);
        }
    }

    private void DrawPlayersOnMap(Vector2 center, float scale)
    {
        if (PlayerManager.Instance != null)
        {
            Vector2 p1Pos = PlayerManager.Instance.transform.position;
            DrawMapMarker(p1Pos, scale, Color.white, 15f, true, "P1");
        }

        if (Player2Manager.Instance != null)
        {
            Vector2 p2Pos = Player2Manager.Instance.transform.position;
            DrawMapMarker(p2Pos, scale, Color.blue, 15f, true, "P2");
        }
    }

    public bool ShouldShowMinimap()
    {
        return settingsOptions[3].Contains("ON");
    }

    private void DrawUsableBuildingMarkersForPlayer(Vector2 playerPos, Vector2 center, float scale, int viewX, int viewY)
    {
        if (VeterinaryCentreManager.Instance != null)
        {
            foreach (var centre in VeterinaryCentreManager.Instance.GetAllCentres())
            {
                DrawMapMarkerForPlayer(centre.transform.position, playerPos, scale, Color.cyan, 10f, true, viewX, viewY, "Vet");
            }
        }

        if (ScrapyardManager.Instance != null)
        {
            foreach (var yard in ScrapyardManager.Instance.GetAllScrapyards())
            {
                DrawMapMarkerForPlayer(yard.transform.position, playerPos, scale, Color.magenta, 10f, true, viewX, viewY, "Scrapyard");
            }
        }

        if (GunShopManager.Instance != null)
        {
            foreach (var shop in GunShopManager.Instance.GetAllGunShops())
            {
                DrawMapMarkerForPlayer(shop.transform.position, playerPos, scale, Color.red, 10f, true, viewX, viewY, "Gun Shop");
            }
        }
    }

    private void DrawMapMarkerForPlayer(Vector2 worldPos, Vector2 playerPos, float scale, Color color, float baseSize, bool showLabel, int viewX, int viewY, string label = "")
    {
        Vector2 offset = (worldPos - playerPos) * scale;
        Vector2 screenPos = new Vector2(viewX + Screen.width / 4 + offset.x, viewY + Screen.height / 2 + offset.y);
        int size = (int)(baseSize * mapZoom);
        
        GUI.color = color;
        GUI.DrawTexture(new Rect(screenPos.x - size / 2, screenPos.y - size / 2, size, size), Texture2D.whiteTexture);
        
        if (showLabel && mapZoom >= 1.5f && !string.IsNullOrEmpty(label))
        {
            GUI.color = Color.white;
            GUI.Label(new Rect(screenPos.x + size, screenPos.y, 100, 20), label);
        }
        GUI.color = Color.white;
    }

    private void DrawPlayerPinsForPlayer(int playerId, Vector2 center, float scale, int viewX, int viewY)
    {
        bool isCoop = GameModeManager.Instance?.IsCoop() ?? false;
        var pins = GetVisiblePins(playerId, isCoop);
        
        foreach (var pin in pins)
        {
            Vector2 playerPos = playerId == 1 
                ? (PlayerManager.Instance?.transform.position ?? Vector2.zero)
                : (Player2Manager.Instance?.transform.position ?? Vector2.zero);
            
            DrawMapMarkerForPlayer(pin.position, playerPos, scale, Color.yellow, 8f, true, viewX, viewY, pin.label);
        }
    }

    private void DrawPlayersOnMapForPlayer(int playerId, Vector2 center, float scale, int viewX, int viewY, Vector2 ownPos)
    {
        Vector2 p1Pos = PlayerManager.Instance?.transform.position ?? Vector2.zero;
        Vector2 p2Pos = Player2Manager.Instance?.transform.position ?? Vector2.zero;

        bool isCoop = GameModeManager.Instance?.IsCoop() ?? false;
        
        if (playerId == 1 || isCoop)
        {
            Vector2 offset = (p1Pos - ownPos) * scale;
            Vector2 screenPos = new Vector2(viewX + Screen.width / 4 + offset.x, viewY + Screen.height / 2 + offset.y);
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(screenPos.x - 8, screenPos.y - 8, 16, 16), Texture2D.whiteTexture);
        }

        if (playerId == 2 || isCoop)
        {
            Vector2 offset = (p2Pos - ownPos) * scale;
            Vector2 screenPos = new Vector2(viewX + Screen.width / 4 + offset.x, viewY + Screen.height / 2 + offset.y);
            GUI.color = Color.blue;
            GUI.DrawTexture(new Rect(screenPos.x - 6, screenPos.y - 6, 12, 12), Texture2D.whiteTexture);
        }
        
        GUI.color = Color.white;
    }
}

public class MapPin
{
    public Vector2 position;
    public string label;
}

public class MapMarker
{
    public Vector2 position;
    public string label;
    public MissionMarkerType type;
}

public enum MissionMarkerType
{
    Objective,
    Checkpoint,
    Target,
    Destination
}
