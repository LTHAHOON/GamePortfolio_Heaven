using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private float startDelayTime;
    [SerializeField]
    private float endDelayTime;
    bool _isEnable = false;
    private void Awake()
    {
        NoneAttackEffectEvent();
    }
    void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(1).IsName("Attack02"))
        {
            if (_isEnable == false)
            {
                _isEnable = true; 
                StartCoroutine(AttackEffectEvent());
            }
        }
        else
        {
            _isEnable = false;
        }
    }

    private IEnumerator AttackEffectEvent()
    {

        yield return new WaitForSeconds(startDelayTime);
        transform.Find("AttackEffect02").GetComponent<TrailRenderer>().enabled = true;
        yield return new WaitForSeconds(endDelayTime);
        transform.Find("AttackEffect02").GetComponent<TrailRenderer>().enabled = false;
    }
    private void NoneAttackEffectEvent()
    {
        transform.Find("AttackEffect02").GetComponent<TrailRenderer>().enabled = false;
        _isEnable = false;
    }

}
