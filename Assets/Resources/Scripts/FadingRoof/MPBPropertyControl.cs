using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPBPropertyControl 
{
    private static MaterialPropertyBlock _mpb;

    public static void ChangeMaterialProperty<T>(Renderer renderer, string propertyName, T value)
    {
        if(_mpb == null)
        {
            _mpb = new();
        }

        renderer.GetPropertyBlock(_mpb);
        switch (value)
        {
            case float fValue:
                _mpb.SetFloat(propertyName, fValue); 
                break;
            case Color color:
                _mpb.SetColor(propertyName, color);
                break;
            case int iValue:
                _mpb.SetInt(propertyName, iValue);
                break;
            case Vector3 vecValue:
                _mpb.SetVector(propertyName, vecValue);
                break;
            case Texture texValue:
                _mpb.SetTexture(propertyName, texValue);
                break;
            default:
                return;
        }
        renderer.SetPropertyBlock(_mpb);
        _mpb.Clear();
    }
}
