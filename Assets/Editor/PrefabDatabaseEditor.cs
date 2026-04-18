using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabDatabase))]
public class PrefabDatabaseEditor : Editor
{
    private const string creatureFolderPath = "Assets/Resources/Prefab/Creature";
    private const string SpacecraftFolderPath = "Assets/Resources/Prefab/Spacecraft";
    private const string HomeFolderPath = "Assets/Resources/Prefab/Home";
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        PrefabDatabase db = (PrefabDatabase)target;
        if(db)
        {
            GUILayout.Space(10);

            if (GUILayout.Button("Auto Fill Prefabs"))
            {
                AutoFill(db);
            }
        }

    }

    private void AutoFill(PrefabDatabase db)
    {
        db.prefabs.Clear();
        PrefabType prefabType = db.GetPrefabType();
        string[] guids;
        switch (prefabType)
        {
            case PrefabType.Creature:
                guids = AssetDatabase.FindAssets("t:Prefab", new[] { creatureFolderPath });
                break;
            case PrefabType.Spacecraft:
                guids = AssetDatabase.FindAssets("t:Prefab", new[] { SpacecraftFolderPath });
                break;
            case PrefabType.Home:
                guids = AssetDatabase.FindAssets("t:Prefab", new[] { HomeFolderPath });
                break;
            default:
                guids = Array.Empty<string>();
                Debug.LogError("Unknown Prefab Type!");
                break;
        }


        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
                db.prefabs.Add(prefab); ;

        }
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();

        Debug.Log("PrefabDatabase Auto Filled!");
    }
}