using UnityEngine;

public class SaveLoadUI : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveLoadSystem.Instance.SaveGame("savegame");
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            if (SaveLoadSystem.Instance.LoadGame("savegame"))
            {
                Debug.Log("Game loaded successfully!");
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, Screen.height - 60, 300, 50));
        GUILayout.Label("F5 = Save | F9 = Load");
        GUILayout.EndArea();
    }
}