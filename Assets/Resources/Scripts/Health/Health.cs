using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
    private float _curStatusCON;
    public Collider _collider;
    private float _currentHealth;
    public delegate void HealthPctChanged(float currentHealthPct, bool useLerp);
    public HealthPctChanged OnHealthPctChanged;
    private HealthBar _healthBar;
    private const float _baseHP = 1000f;

    private void Start()
    {
        if (_autoInitHealth)
        {
          //  InitHealth(GetComponent<StatusComponent>().GetStatus());
        }
    }
    public void InitHealth(RuntimeUnitStatus status)
    {
        _status = status;
        if (_status != null)
        {
            SetMaxHP();
            _currentHealth = _maxHealth;
        }

       OnHealthAdded(this, fraction);
       _healthBar = OnHealthFinder?.Invoke(this);
       ObjectVisbilitySystem.Instance.AddToList(_healthBar);
    }
    private void Update()
    {
        if (!_healthBar) return;
        if (_status == null) return;

        UpdateMaxHP();
    }

    private void UpdateMaxHP()
    {
        if (_curStatusCON != _status.CON)
        {
            SetMaxHP();
            SetHealthPct(_currentHealth);
        }
    }

    private void SetMaxHP()
    {
        _curStatusCON = _status.CON;
        float afterMaxHP = _baseHP + _curStatusCON * _fixedHP;
        _maxHealth = afterMaxHP;
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
        SetHealth(amount);
        if (_healthBar && _healthBar.isActiveAndEnabled)
        {
            SetHealthPct(_currentHealth);
        }
        if (amount < 0)
        {
            OnDamageHit?.Invoke();
            if ((_currentHealth) <= 0)
            {
                StopAllCoroutines();
                OnDie?.Invoke();
                enabled = false;
            }
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
        OnHealthRemoved?.Invoke(this);
    }

    public HealthBar HealthBar => _healthBar;
    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;
    public float CurrentHealthPct => _currentHealth / _maxHealth;
    public void SetActiveHealthBar(bool isActive) => _healthBar.gameObject.SetActive(isActive); 
}
