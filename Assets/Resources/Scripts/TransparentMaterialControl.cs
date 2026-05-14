using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TransparentMaterialControl : MonoBehaviour
{
    public enum SurfaceType 
    {
        Opaque,
        Transparent,
    }

    private static Material _newMaterial;
    private static SurfaceType _surface;
    private static Color _changedColor;
    public static GameObject SetQpaqueOrTransparentControl(GameObject unitPrefab, UnitType unitType,SurfaceType surface, Color changedColor)
    {
        _surface = surface;
        _changedColor = changedColor;
        MeshRenderer renderer;
        if (unitPrefab.TryGetComponent(out renderer))
        {
            ChangeMaterialColor(renderer, unitType);
        }
        for (int i = 0; i < unitPrefab.transform.childCount; i++)
        {
            if (unitPrefab.transform.GetChild(i).TryGetComponent(out renderer))
            {
                ChangeMaterialColor(renderer, unitType);
            }
        }
        return unitPrefab;
    }

    private static void ChangeMaterialColor(MeshRenderer renderer, UnitType unitType)
    {
        Material loadMaterial = Resources.Load<Material>($"UnitPrefab/Prefab/{unitType}/{_surface}Material/{renderer.sharedMaterial.name}");
        loadMaterial.SetColor("_BaseColor", _changedColor);

        renderer.sharedMaterial = loadMaterial;
    }

}
