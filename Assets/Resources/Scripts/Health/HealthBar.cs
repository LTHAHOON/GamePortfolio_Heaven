using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    UIFollowObject _uiFollowObject;
    [SerializeField]
    private Image _fill;
    [SerializeField]
    private Slider _hpBarSlider;
    [SerializeField]
    private Slider _hpChipBarSlider;
    [Header("HealthBar Lerp 도달 시간")]
    [SerializeField]
    private float _healthTimeSpeed = 1f;
    [Header("ChipBar Lerp 도달 시간")]
    [SerializeField]
    private float _chipTimeSpeed = 1f;
    [Header("ChipBar Delay")]
    [SerializeField]
    private float _chipTimeDelay = 0.2f;
    private float _curChipTime = 0;
    private bool _bLerpHealthBar = false;
    private float _curPct = 1;
    public Health _health;

    private void Update()
    {
        UpdateHealthBarPosition();
        UpdateHealthBar(_curPct);
        UpdateChipBar(_curPct);

    }

    private void UpdateHealthBar(float pct)
    {
        if (_bLerpHealthBar)
        {
            if (_hpBarSlider.value != pct)
            {
                _hpBarSlider.value = Mathf.MoveTowards(_hpBarSlider.value, _curPct, Time.deltaTime * _healthTimeSpeed);
            }
            else
            {
                _hpBarSlider.value = pct;
                _bLerpHealthBar = false;
            }
        }
    }


    private void UpdateChipBar(float pct)
    {
        if (_hpChipBarSlider.value != pct)
        {
            if(_curChipTime < _chipTimeDelay)
            {
                _curChipTime += Time.deltaTime;
            }
            else
            {
                _hpChipBarSlider.value = Mathf.MoveTowards(_hpChipBarSlider.value, _curPct, Time.deltaTime * _chipTimeSpeed);
            }
        }
        else
        {
            _hpChipBarSlider.value = pct;
            if (CheckDead(pct))
            {
                ObjectVisbilitySystem.RemoveToList(this);
                Destroy(gameObject);
            }
        }
    }

    public void SetHealth(Health health)
    {
        this._health = health;
        health.OnHealthPctChanged = HandleHealthPctChanged;
    }


    public bool CheckDead(float pct)
    {
        return pct <= 0;
    }

    private void HandleHealthPctChanged(float pct, bool useLerp)
    {
        _curPct = pct;
        _curChipTime = 0;
        _bLerpHealthBar = useLerp;
        if(!_bLerpHealthBar)
        {
            _hpBarSlider.value = _curPct;
        }
    }


    public void UpdateHealthBarPosition()
    {
        if (_health != null)
        {
            _uiFollowObject.FollowObject(UIManager.Instance.CurrentUICamera, _health.gameObject, gameObject);
        }
    }

    public void SetFillColor(Color color) =>_fill.color = color;
    private void OnDestroy()
    {
        _health.OnHealthPctChanged -= HandleHealthPctChanged;
    }
}
