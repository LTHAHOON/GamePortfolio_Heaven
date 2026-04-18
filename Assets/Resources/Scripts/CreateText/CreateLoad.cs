using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CreateLoad : MonoBehaviour
{
    [SerializeField]
    private float _createTime = 10;
    [SerializeField]
    private float _loadingDelayTime = 0.5f;
    public static event Action<CreateLoad, float, float> OnStartCreateLoad;
    public LoadingTextComponent _loadingTextComponent;
    private bool _isLoading = false;
    private bool _isLoadReady = true;
    public Collider _collider;
    public Vector3 _screenOffset;
    public Vector3 _localOffset;
    private void Awake()
    {
        if (_collider) return;
        _collider = GetComponent<Collider>();
    }
    public void StartCreateLoad()
    {
        _isLoading = true;
        OnStartCreateLoad(this, _createTime, _loadingDelayTime);
    }
    
    private void Update()
    {
        if(_isLoading)
        {
            if (_loadingTextComponent == null)
            {
                _isLoadReady = false;
                _isLoading = false;
            }
        }
    }

    public bool IsLoading => _isLoading;
    public bool IsLoadReady => _isLoadReady;
    public bool SetLoadReady(bool isReady) => _isLoadReady = isReady;
}
