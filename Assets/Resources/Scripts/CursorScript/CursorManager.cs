using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum CursorType
{
    Origin,
    Attack,
    Defend,
    NONE
}
[Serializable]
public struct CursorContraintPos
{
    public float _minX; //100f;
    public float _maxX; //1800f;
    public float _minY; //180f;
    public float _maxY; //1000f;
    public float _scale;
    public void ChangeContraintPos(float scale)
    {
        _minX = 100f * scale;
        _maxX = 1800f * scale;
        _minY = 180f * scale;
        _maxY = 1000 * scale;
        _scale = scale;
    }

    public CursorContraintPos(float scale)
    {
        _minX = 100f * scale;
        _maxX = 1800f * scale;
        _minY = 180f * scale;
        _maxY = 1000 * scale;
        _scale = scale;
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
    private CursorContraintPos _cursorContraintPos;
    [SerializeField]
    private CursorData[] _cursorDatas;
    private static Dictionary<CursorType, MouseCursorData> _dicCursor = new();
    [SerializeField]
    private MouseCursorController _mouseCursorController;
    private CanvasScaler _hudCanvasScaler;
    private CursorType _curCursorType = CursorType.NONE;
    private void Awake()
    {
        _hudCanvasScaler = UIManager.Instance.HUDCanvasScaler;
        _cursorContraintPos = new(_hudCanvasScaler.scaleFactor);
        for (int i = 0; i < _cursorDatas.Length; ++i)
        {
            _dicCursor.Add(_cursorDatas[i].GetCursorType(), _cursorDatas[i].GetCursorInstance());
        }
    }
    private void Update()
    {
       if(_cursorContraintPos._scale != _hudCanvasScaler.scaleFactor)
        {
            _cursorContraintPos.ChangeContraintPos(_hudCanvasScaler.scaleFactor);
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
        _mouseCursorController.SpriteFollowMouse(_cursorContraintPos, spriteRenderer, Camera.main);
    }

    public CursorContraintPos CursorContraintPos => _cursorContraintPos;
}
