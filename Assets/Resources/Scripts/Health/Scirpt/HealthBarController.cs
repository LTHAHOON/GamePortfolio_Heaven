using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    [SerializeField]
    private HealthBar[] _arrHealthBarPrefab = new HealthBar[4];

    private readonly Dictionary<Health, HealthBar> _healthBars = new();
    private void Awake()
    {
        Health.OnHealthAdded += AddHealthBar;
        Health.OnHealthRemoved += RemoveHealthBar;
        Health.OnHealthFinder += FindHealthBar;
    }

    private void AddHealthBar(Health health, Fraction fraction)
    {
        Debug.Log(health);
        for (int i = 0; i < _arrHealthBarPrefab.Length; i++)
        {
            if (_healthBars.ContainsKey(health) == false)
            {
                if (health.gameObject.layer == _arrHealthBarPrefab[i].gameObject.layer)
                {
                    HealthBar healthBar = Instantiate(_arrHealthBarPrefab[i], transform);
                    if (fraction == Fraction.Ally)
                    {
                        healthBar.SetFillColor(UIManager.Instance.GetFactionPlayerColor());

                    }
                    else if(fraction == Fraction.Enemy)
                    {
                        healthBar.SetFillColor(UIManager.Instance.GetFactionEnemyColor());
                    }
                    _healthBars.Add(health, healthBar);
                    healthBar.SetHealth(health);
                    break;
                }
                
            }
        }
           
    }

    private void RemoveHealthBar(Health health)
    {
        if (_healthBars.ContainsKey(health) == true)
        {
            _healthBars.Remove(health);
        }
    }

    private HealthBar FindHealthBar(Health health)
    {
        if(_healthBars.ContainsKey(health) == true)
        {
            return _healthBars[health];
        }
        else
        {
            Debug.Log("ERROR: You can't find HealthBar");
            return null;
        }
    }
}
