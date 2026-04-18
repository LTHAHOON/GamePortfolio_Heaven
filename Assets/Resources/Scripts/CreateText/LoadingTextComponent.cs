using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public abstract class LoadingTextComponent : MonoBehaviour
{
    [SerializeField]
    private List<string> _loadingTexts = new();
    [SerializeField]
    private TextMeshProUGUI _loadingText;
    private int _curIndex = 0;
    private float _loadingTime = 0;
    private float _curLoadingTime = 0;
    private float _delayTime = 0;
    private bool _isLoading = false;
    public virtual void Update()
    {
        if (_loadingTime <= 0) return;
        if (!_isLoading)
        {
            StartCoroutine(IEUpdateLoadingText());
        }
        if(Time.time >= _curLoadingTime)
        {
            _loadingTime = 0;
            Destroy(gameObject);
        }
    }
    public virtual void StartLoadingText(float loadingTime, float delayTime)
    {
        _curIndex = 0;
        _loadingTime = loadingTime;
        _delayTime = delayTime;
        _curLoadingTime = Time.time + _loadingTime;
    }
    public virtual void StartLoadingText<T>(T target, float loadingTime, float delayTime) 
    {
        StartLoadingText(loadingTime, delayTime);
    }
    
    private IEnumerator IEUpdateLoadingText()
    {
        _isLoading = true;
        _loadingText.text = _loadingTexts[_curIndex];
        yield return new WaitForSeconds(_delayTime);
        _curIndex = (_curIndex + 1) % _loadingTexts.Count;
        _isLoading = false;
    }    
}
