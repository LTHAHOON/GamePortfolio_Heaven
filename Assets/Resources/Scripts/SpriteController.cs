using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using UnityEngine;

public static class SpriteController 
{
    private static Dictionary<string, Sprite> _dicUnitSprite = new();
    private static Dictionary<string, Sprite> _dicUnitProperty = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/AllUnitSprites");
        foreach (var sprite in sprites)
        {
            _dicUnitSprite[sprite.name] = sprite;
        }
        sprites = Resources.LoadAll<Sprite>("Sprites/Unit_UI/UnitProperty");
        foreach (var sprite in sprites)
        {
            _dicUnitProperty[sprite.name] = sprite;
        }
    }

    public static Sprite GetUnitSprite(string name)
    {
        if(_dicUnitSprite.ContainsKey(name))
        {
            return _dicUnitSprite[name];    
        }
        return default;
    }

    public static Sprite GetUnitPropertySprite(UnitProperty unitProperty)
    {
        string property = unitProperty.ToString();
        if (_dicUnitProperty.ContainsKey(property))
        {
            return _dicUnitProperty[property];
        }
        return default;
    }

}
