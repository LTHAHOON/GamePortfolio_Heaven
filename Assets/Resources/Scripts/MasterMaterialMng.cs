using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public enum SurfaceType
{
    Translucent = 0,
    Opaque = 1,
}

public class MasterMaterialMng : Singleton<MasterMaterialMng>
{
    private readonly Dictionary<int, int> _dicPrevUnitLayer = new();
    [Header("SG_Master Alpha 프로퍼티 이름")]
    [SerializeField]
    private string _alphaPropertyName = "_Alpha";
    [Header("SG_Master BaseColor 프로퍼티 이름")]
    [SerializeField]
    private string _baseColorPropertyName = "_BaseColor";
    [SerializeField]
    private Color _translucentColor;

    private int _alphaPropertyID = -1;
    private int _baseColorPropertyID = -1;

    private void Awake()
    {
        _alphaPropertyID = Shader.PropertyToID(_alphaPropertyName);
        _baseColorPropertyID = Shader.PropertyToID(_baseColorPropertyName);
    }

    public void SetQpaqueOrTranslucent(Unit unit, SurfaceType surface)
    {
        if(!unit)
            return;
        int layer;
        float alpha;
        Color color = Color.white;
        int unitID = unit.GetInstanceID();
        switch (surface)
        {
            case SurfaceType.Opaque:
                {
                    if (!_dicPrevUnitLayer.TryGetValue(unitID, out var prevLayer))
                    {
                        return;
                    }
                    layer = prevLayer;
                    _dicPrevUnitLayer.Remove(unitID);
                    alpha = 1f;
                    break;
                }
            case SurfaceType.Translucent:
                {
                    if(_dicPrevUnitLayer.ContainsKey(unitID))
                    {
                        return;
                    }
                    _dicPrevUnitLayer.Add(unitID, unit.gameObject.layer);
                    layer = GameLayer.RemovedOutlineLayer;
                    alpha = 0.35f;
                    color = _translucentColor;
                    break;
                }
            default:
                return;
        }

        ChangeAlpha(unit, alpha, color, layer);
    }
    
    
    private void ChangeAlpha(Unit unit, float alpha, Color color, int layer)
    {
        Renderer[] renderers = unit.GetRenderers();

        if (renderers == null)
            return;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].gameObject.layer = layer;
            MPBPropertyControl.ChangeMaterialProperty(renderers[i], _alphaPropertyID, alpha);
            MPBPropertyControl.ChangeMaterialProperty(renderers[i], _baseColorPropertyID, color);
        }
    }
}
