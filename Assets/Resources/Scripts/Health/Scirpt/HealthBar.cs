using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class HealthBar : MonoBehaviour, ICullingUI
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
    [SerializeField]
    private bool _isForceHideUI = false;
    [SerializeField]
    private float _showDuration = 0f;
    private float _curChipTime = 0;
    private bool _bLerpHealthBar = false;
    private float _curPct = 1;
    public Health _health;
    public GameObject ThisGameObject => gameObject;
    public Collider ColliderForCulling => _health._collider;

    private void Awake()
    {
        SetForceHideUI(_isForceHideUI);
    }

    private void Update()
    {
        UpdateHealthBar(_curPct);
        UpdateChipBar(_curPct);
    }

    private void LateUpdate()
    {
        UpdateHealthBarPosition();
    }

    //Active가 켜지는 순간 새로고침
    private void OnEnable()
    {
        if (!_health) return;
        _hpBarSlider.value = _health.CurrentHealthPct;
        _hpChipBarSlider.value = _health.CurrentHealthPct;
    }
    private void OnDisable()
    {
        if (!_health) return;
        _hpBarSlider.value = _health.CurrentHealthPct;
        _hpChipBarSlider.value = _health.CurrentHealthPct;
    }
    private void UpdateHealthBar(float pct)
    {
        if (_bLerpHealthBar)
        {
            if (_hpBarSlider.value != pct)
            {
                _hpBarSlider.value = Mathf.MoveTowards(_hpBarSlider.value, pct, Time.deltaTime * _healthTimeSpeed);
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
                _hpChipBarSlider.value = Mathf.MoveTowards(_hpChipBarSlider.value, pct, Time.deltaTime * _chipTimeSpeed);
            }
        }
        else
        {
            _hpChipBarSlider.value = pct;
            _curChipTime = 0;
            if (CheckDead(pct))
            {
                ObjectVisbilitySystem.Instance.RemoveToList(this);
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
        //_curChipTime = 0;
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
            _uiFollowObject.FollowObject(Camera.main, _health.gameObject, gameObject);
        }
    }

    private bool _playingHide = false;
    public IEnumerator IEShowUI(float showDuration = -1f)
    {
        if (_playingHide) yield break;
        _playingHide = true;
        if(showDuration <= 0)
        {
            showDuration = _showDuration;
        }
        SetForceHideUI(false);
        yield return new WaitForSeconds(showDuration);
        if (!this) yield break;
        SetForceHideUI(true);
        _playingHide = false;
    }


    public void SetForceHideUI(bool isForceHide)
    {
        _isForceHideUI = isForceHide;
        gameObject.SetActive(!isForceHide);
    }

    public void SetFillColor(Color color) =>_fill.color = color;
    public bool IsForceHideUI => _isForceHideUI;

}
