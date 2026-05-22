using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPBPropertyControl 
{
    private static MaterialPropertyBlock _mpb;
    
    public static void ChangeMaterialProperty<T>(Renderer renderer, int propertyNameID, T value)
    {
        if(_mpb == null)
        {
            _mpb = new();
        }

        renderer.GetPropertyBlock(_mpb);
        switch (value)
        {
            case float fValue:
                _mpb.SetFloat(propertyNameID, fValue); 
                break;
            case Color color:
                _mpb.SetColor(propertyNameID, color);
                break;
            case int iValue:
                _mpb.SetInt(propertyNameID, iValue);
                break;
            case Vector2 vecValue:
                _mpb.SetVector(propertyNameID, vecValue);
                break;
            case Texture texValue:
                _mpb.SetTexture(propertyNameID, texValue);
                break;
            default:
                return;
        }
        renderer.SetPropertyBlock(_mpb);
        _mpb.Clear();
    }
}
