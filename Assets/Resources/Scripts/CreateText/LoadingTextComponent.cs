using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


public class LoadingTask
{
    public float _curLoadingTime;
    public float DelayTime;
    public bool IsDoneLoading => Time.time >= _curLoadingTime;
    public Action OnDoneLoadingTask;
    public LoadingTask(float curLoadingTime ,float delayTime)
    {
        _curLoadingTime = curLoadingTime;
        DelayTime = delayTime;
    }
    
}

public class LoadingTextComponent<TLoad> : MonoBehaviour where TLoad : Component
{
    [SerializeField]
    private List<string> _loadingTexts = new();
    [SerializeField]
    private TextMeshProUGUI _loadingText;
    [HideInInspector]
    public TLoad _load;
    private int _curIndex = 0;
    private bool _bChangeLoadingText = false;
    private LoadingTask _loadingTask;
    public virtual void Update()
    {
        if (_loadingTask == null) return;
        if (_loadingTask._curLoadingTime <= 0) return;
        if (!_loadingTask.IsDoneLoading && _bChangeLoadingText)
        {
            StartCoroutine(IEUpdateLoadingText());
        }
        else if(_loadingTask.IsDoneLoading)
        {
            _loadingTask._curLoadingTime = 0;
            _loadingTask.OnDoneLoadingTask?.Invoke();
            Destroy(gameObject);
        }
    }

    public LoadingTask StartLoadingText(TLoad loadTarget, float loadingTime, float delayTime) 
    {
        _load = loadTarget;
        _bChangeLoadingText = true;
        _curIndex = 0;
        _loadingTask = new(Time.time + loadingTime, delayTime);
        return _loadingTask;
    }
    
    private IEnumerator IEUpdateLoadingText()
    {
        _bChangeLoadingText = false;
        _loadingText.text = _loadingTexts[_curIndex];
        yield return new WaitForSeconds(_loadingTask.DelayTime);
        _curIndex = (_curIndex + 1) % _loadingTexts.Count;
        _bChangeLoadingText = true;
    }    
}
