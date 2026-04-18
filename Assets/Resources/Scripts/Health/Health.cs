using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Fraction
{
    Ally,
    Enemy
}


[RequireComponent(typeof(StatusComponent))]
public class Health : MonoBehaviour
{
    [SerializeField]
    private Fraction fraction = Fraction.Ally;
    [SerializeField]
    private float _maxHealth = 100;
    [SerializeField]
    private float _fixedHP = 100f;
    [SerializeField]
    private bool _autoInitHealth = true;

    public static event Action<Health, Fraction> OnHealthAdded;
    public static event Action<Health> OnHealthRemoved;
    public static event Func<Health, HealthBar> OnHealthFinder;
    public event Action OnDamageHit;
    public event Action OnHealHit;
    public event Action OnDie;

    private RuntimeUnitStatus _status;
    public Collider _collider;
    public float _currentHealth;
    public delegate void HealthPctChanged(float currentHealthPct, bool useLerp);
    public HealthPctChanged OnHealthPctChanged;
    private HealthBar _healthBar;
    private const float _baseHP = 1000f;
    private bool _isStatusAdded = false;
    private bool _bHit = false;

    private void Start()
    {
        if (_autoInitHealth)
        {
            InitHealth();
        }
    }
    public void InitHealth()
    {
        _status = GetComponent<StatusComponent>().GetStatus();
        if (_status != null)
        {
            float afterMaxHP = _baseHP + _status.CON * _fixedHP;
            _maxHealth = afterMaxHP;
            _currentHealth = _maxHealth;
        }

        OnHealthAdded(this, fraction);
        _healthBar = OnHealthFinder?.Invoke(this);
        ObjectVisbilitySystem.AddToList(_healthBar);
    }
    private void Update()
    {

        if (_healthBar == null) return;
        if (_status == null) return;
        if ((_currentHealth) <= 0)
        {
            OnDie?.Invoke();
            if (!_healthBar.isActiveAndEnabled)
            {
                Destroy(_healthBar.gameObject);
            }
            enabled = false;
        }

        if (_healthBar.isActiveAndEnabled)
        {
            if (_bHit)
            {
                SetHealthPct(_currentHealth);
                _bHit = false;
            }

            if (_status != null)
            {
                if (_isStatusAdded)
                {
                    SetHealthPct(_currentHealth);
                    _isStatusAdded = false;
                }
            }
        }
        else
        {
        }

        if (_maxHealth != _baseHP + _status.CON * _fixedHP)
        {
            float afterMaxHP = _baseHP + _status.CON * _fixedHP;
            _maxHealth = afterMaxHP;
            _isStatusAdded = true;
        }
    }
    private void SetHealthPct(float currentHealth)
    {
        float currentHealthPct = currentHealth / _maxHealth;
        OnHealthPctChanged?.Invoke(currentHealthPct, true);
    }
    private void SetHealth(float amountToAdd)
    {
        _currentHealth += amountToAdd;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
    }
    public void ModifyHealth(float amount)
    {
        if (!_healthBar) return;
        _bHit = true;
        SetHealth(amount);
        if (_healthBar.isActiveAndEnabled)
        {
            SetHealthPct(_currentHealth);
        }
        if (amount < 0)
        {
            OnDamageHit?.Invoke();
        }
        else 
        { 
            OnHealHit?.Invoke();
        }
    }

    public void ModifyHealth(float amount, float delaySecond)
    {
        if (_isModifyingHealth) return;
        _isModifyingHealth = true;
        StartCoroutine(IEModifyHealth(amount, delaySecond));
    }

    [HideInInspector]
    public bool _isModifyingHealth = false;
    private IEnumerator IEModifyHealth(float amount, float delaySecond)
    {
        ModifyHealth(amount);
        yield return new WaitForSeconds(delaySecond);
        _isModifyingHealth = false;
    }

    private void OnDestroy()
    {
        ObjectVisbilitySystem.RemoveToList(_healthBar);
        OnHealthRemoved?.Invoke(this);
    }
    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;
}
