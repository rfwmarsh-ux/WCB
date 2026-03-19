using UnityEngine;
using UnityEditor;
using System.IO;

public class ImportKenneyAssets : EditorWindow
{
    [MenuItem("Tools/Import Kenney Assets")]
    public static void ShowWindow()
    {
        GetWindow<ImportKenneyAssets>("Import Kenney Assets");
    }

    void OnGUI()
    {
        GUILayout.Label("Kenney Asset Importer", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Import All Sprites"))
        {
            ImportAllSprites();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Import Vehicles"))
        {
            ImportSpritesFromFolder("PNG/Cars", "Vehicles");
        }
        
        if (GUILayout.Button("Import Characters"))
        {
            ImportSpritesFromFolder("PNG/Characters", "Characters");
            ImportCharacterFolders();
        }
        
        if (GUILayout.Button("Import Props"))
        {
            ImportSpritesFromFolder("PNG/Props", "Props");
        }
    }

    void ImportAllSprites()
    {
        ImportSpritesFromFolder("Assets/Imports/PNG/Cars", "Assets/Sprites/Vehicles");
        ImportSpritesFromFolder("Assets/Imports/PNG/Characters", "Assets/Sprites/Characters");
        ImportSpritesFromFolder("Assets/Imports/PNG/Props", "Assets/Sprites/Props");
        ImportCharacterFolders();
        
        AssetDatabase.Refresh();
        Debug.Log("All Kenney sprites imported!");
    }

    void ImportSpritesFromFolder(string sourcePath, string destPath)
    {
        string fullSourcePath = Application.dataPath + "/" + sourcePath;
        string fullDestPath = Application.dataPath + "/" + destPath;
        
        if (!Directory.Exists(fullSourcePath))
        {
            Debug.LogWarning($"Source folder not found: {fullSourcePath}");
            return;
        }
        
        if (!Directory.Exists(fullDestPath))
        {
            Directory.CreateDirectory(fullDestPath);
        }
        
        string[] files = Directory.GetFiles(fullSourcePath, "*.png");
        
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            string destFile = fullDestPath + "/" + fileName;
            
            File.Copy(file, destFile, true);
        }
        
        Debug.Log($"Imported {files.Length} sprites from {sourcePath}");
    }

    void ImportCharacterFolders()
    {
        string basePath = Application.dataPath + "/Assets/Imports";
        string destBase = Application.dataPath + "/Assets/Sprites/Characters";
        
        if (!Directory.Exists(destBase))
        {
            Directory.CreateDirectory(destBase);
        }
        
        string[] charFolders = new string[]
        {
            "Male person",
            "Female person", 
            "Male adventurer",
            "Female adventurer",
            "Zombie",
            "Robot"
        };
        
        foreach (string charFolder in charFolders)
        {
            string sourcePath = basePath + "/" + charFolder + "/PNG/Poses";
            string destPath = destBase + "/" + charFolder;
            
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                
                string[] files = Directory.GetFiles(sourcePath, "*.png");
                
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = destPath + "/" + fileName;
                    
                    File.Copy(file, destFile, true);
                }
                
                Debug.Log($"Imported {files.Length} poses from {charFolder}");
            }
        }
    }
}
