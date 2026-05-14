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
    public static event Func<CreateLoad, float, float, LoadingTask> OnStartCreateLoad;
    public event Action OnDoneCreateLoad; 
    public LoadingTextComponent<CreateLoad> _loadingTextComponent;
    private bool _isLoading = false;
    private bool _isLoadReady = true;
    public Collider _collider;
    public Vector3 _screenOffset;
    public Vector3 _localOffset;
    private LoadingTask loadingTask;
    private void Awake()
    {
        if (_collider) return;
        _collider = GetComponent<Collider>();
    }
    private void Update()
    {
        if (loadingTask == null) return;
        if(loadingTask.IsDoneLoading)
        {
            loadingTask = null;
            OnDoneLoading();
        }
    }
    public void StartCreateLoad(Action endAction)
    {
        _isLoading = true;
        loadingTask = OnStartCreateLoad(this, _createTime, _loadingDelayTime);
        loadingTask.OnDoneLoadingTask = OnDoneLoading;
        OnDoneCreateLoad = endAction;
    }
    
    public void OnDoneLoading()
    {
        _isLoadReady = false;
        _isLoading = false;
        OnDoneCreateLoad?.Invoke();
    }

    public bool IsLoading => _isLoading;
    public bool IsLoadReady => _isLoadReady;
    public bool SetLoadReady(bool isReady) => _isLoadReady = isReady;
}
