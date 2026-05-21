using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UpdateToChangeOffset : MonoBehaviour
{
    [SerializeField]
    private Renderer _renderer;
    [SerializeField]
    private string _uvOffsetPropertyName = "_UV_Offset";
    [SerializeField]
    private Vector2 _offsetSpeed;
    [SerializeField]
    private Vector2 _curOffset = Vector2.zero;
    private int _uvOffsetID = 0;

    private void Awake()
    {
        _uvOffsetID = Shader.PropertyToID(_uvOffsetPropertyName);
    }
    void Update()
    {
        Rotate();
    }


    private void Rotate()
    {
        _curOffset += Time.deltaTime * _offsetSpeed;
        MPBPropertyControl.ChangeMaterialProperty(_renderer, _uvOffsetID, _curOffset);
    }
}
