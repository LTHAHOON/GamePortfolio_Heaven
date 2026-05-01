using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitPrefabDB))]
public class UnitPrefabDatabaseEditor : Editor
{
    private const string CreatureFolderPath = "Assets/Resources/Prefab/Creature";
    private const string SpacecraftFolderPath = "Assets/Resources/Prefab/Spacecraft";
    private const string HomeFolderPath = "Assets/Resources/Prefab/Home";
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        UnitPrefabDB db = (UnitPrefabDB)target;
        if(db)
        {
            GUILayout.Space(10);

            if (GUILayout.Button("Auto Fill Prefabs"))
            {
                AutoFill(db);
            }
        }

    }

    private void AutoFill(UnitPrefabDB db)
    {
        db.UnitPrefabs.Clear();
        UnitType prefabType = db.UnitDataBaseType;
        string[] guids;
        switch (prefabType)
        {
            case UnitType.Creature:
                guids = AssetDatabase.FindAssets("t:Prefab", new[] { CreatureFolderPath });
                break;
            case UnitType.Spacecraft:
                guids = AssetDatabase.FindAssets("t:Prefab", new[] { SpacecraftFolderPath });
                break;
            case UnitType.Home:
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
            Unit prefab = AssetDatabase.LoadAssetAtPath<Unit>(path);
            if (prefab)
                db.UnitPrefabs.Add(prefab); ;
        }
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();

        Debug.Log("PrefabDatabase Auto Filled!");
    }
}