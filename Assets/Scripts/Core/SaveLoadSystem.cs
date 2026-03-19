using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadSystem : MonoBehaviour
{
    public static SaveLoadSystem Instance { get; private set; }

    private string savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        savePath = Application.persistentDataPath + "/saves/";
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
    }

    public void SaveGame(string slotName = "savegame")
    {
        GameData data = new GameData();

        GameManager gm = GameManager.Instance;
        data.money = gm.Money;
        data.wantedLevel = gm.WantedLevel;

        PlayerManager pm = PlayerManager.Instance;
        if (pm != null)
        {
            data.playerHealth = pm.GetHealth();
            data.playerMaxHealth = pm.GetMaxHealth();
            data.playerArmour = pm.GetArmour();
            data.playerPosition = pm.transform.position;
        }

        DayNightCycleManager dayNight = gm.GetDayNightManager();
        if (dayNight != null)
        {
            data.currentTime = dayNight.GetCurrentTime();
        }

        VehicleManager vm = gm.GetVehicleManager();
        if (vm != null)
        {
            data.ownedVehicles = vm.GetOwnedVehicles();
            data.currentVehicleId = vm.GetCurrentVehicleId();
        }

        string json = JsonUtility.ToJson(data);
        string filePath = savePath + slotName + ".json";
        File.WriteAllText(filePath, json);
        Debug.Log($"Game saved to {filePath}");
    }

    public bool LoadGame(string slotName = "savegame")
    {
        string filePath = savePath + slotName + ".json";
        if (!File.Exists(filePath))
        {
            Debug.Log("No save file found");
            return false;
        }

        string json = File.ReadAllText(filePath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        GameManager gm = GameManager.Instance;
        gm.Money = data.money;
        gm.WantedLevel = data.wantedLevel;

        PlayerManager pm = PlayerManager.Instance;
        if (pm != null)
        {
            pm.transform.position = data.playerPosition;
        }

        Debug.Log($"Game loaded from {filePath}");
        return true;
    }

    public bool SaveExists(string slotName = "savegame")
    {
        return File.Exists(savePath + slotName + ".json");
    }

    public void SaveGame(int slot)
    {
        SaveGame($"slot_{slot}");
    }

    public bool LoadGame(int slot)
    {
        return LoadGame($"slot_{slot}");
    }

    public bool HasSaveData(int slot)
    {
        return SaveExists($"slot_{slot}");
    }

    public void DeleteSave(string slotName = "savegame")
    {
        string filePath = savePath + slotName + ".json";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Save deleted: {filePath}");
        }
    }
}

[System.Serializable]
public class GameData
{
    public float money;
    public int wantedLevel;
    public float playerHealth;
    public float playerMaxHealth;
    public float playerArmour;
    public Vector3 playerPosition;
    public float currentTime;
    public string[] ownedVehicles;
    public string currentVehicleId;
}