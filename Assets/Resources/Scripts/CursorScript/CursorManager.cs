using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum CursorType
{
    Origin,
    Attack,
    Defend,
    NONE
}
[Serializable]
public struct SpriteContraintPos
{
    public float _minX; //100f;
    public float _maxX; //1800f;
    public float _minY; //180f;
    public float _maxY; //1000f;

    public static SpriteContraintPos Default()
    {
        return new()
        {
            _minX = 100f,
            _maxX = 1800f,
            _minY = 180f,
            _maxY = 1000f
        };
    }
}

[Serializable]
public struct CursorData
{
    [SerializeField]
    private CursorType cursorType;
    [SerializeField]
    private MouseCursorData cursorInstance;

    public CursorType GetCursorType()
    {
        return cursorType;
    }
    public MouseCursorData GetCursorInstance()
    {
        return cursorInstance;
    }
}

public class CursorManager : Singleton<CursorManager>
{
    [SerializeField]
    private Camera _subCamera;
    [SerializeField]
    private SpriteContraintPos _spriteContraintPos = SpriteContraintPos.Default();
    [SerializeField]
    private CursorData[] _cursorDatas;
    private static Dictionary<CursorType, MouseCursorData> _dicCursor = new();
    [SerializeField]
    private MouseCursorController _mouseCursorController;

    private CursorType _curCursorType = CursorType.NONE;
    private void Awake()
    {
        for (int i = 0; i < _cursorDatas.Length; ++i)
        {
            _dicCursor.Add(_cursorDatas[i].GetCursorType(), _cursorDatas[i].GetCursorInstance());
        }
    }
    private void Start()
    {
        SetCursor(CursorType.Origin);
    }
    public void SetCursor(CursorType cursorType)
    {
        if (cursorType == _curCursorType || cursorType == CursorType.NONE)
        {
            _mouseCursorController.ShowCursor(true);
            return;
        }

        if (_curCursorType != CursorType.NONE)
        {
            SetSpriteRendererEnableProcess(cursorType);
        }
        _curCursorType = cursorType;
        if (_dicCursor.Count > 0 && _dicCursor.ContainsKey(cursorType))
        {
            _mouseCursorController.ShowCursor(true);
            _mouseCursorController.SetCursor(_dicCursor[cursorType]);
        }
    }

    private void SetSpriteRendererEnableProcess(CursorType cursorType)
    {
        SetSpriteRendererEnabled(_curCursorType, false); //전 스프라이트 끄기
        SetSpriteRendererEnabled(cursorType, true);
    }

    public static void SetSpriteRendererEnabled(CursorType cursorType, bool enabled)
    {
        MouseCursorData data = GetCursorData(cursorType);
        SpriteRenderer spriteRenderer = data.GetFollwingSpriteRenderer();
        if (spriteRenderer)
        {
            spriteRenderer.enabled = enabled;
        }
    }

    public static MouseCursorData GetCursorData(CursorType cursorType)
    {
        if (_dicCursor.Count > 0 && _dicCursor.ContainsKey(cursorType))
        {
            return _dicCursor[cursorType];
        }
        return null;
    }


    public void SpriteFollowMouse(SpriteRenderer spriteRenderer)
    {
        _mouseCursorController.SpriteFollowMouse(_spriteContraintPos, spriteRenderer, _subCamera);
    }
}
