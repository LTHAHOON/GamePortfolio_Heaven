using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public enum SkillAnimName
{
    Skill01,
    Skill02,
    Skill03   
}
public enum SkillActionType
{
    Melee,
    Projectile,
    Buff,
    Debuff
}
public enum SkillTargetType
{
    Self,
    Ally,
    Enemy,
    AOE
}
[Serializable]
public struct SkillType
{
    [SerializeField]
    private SkillActionType _skillActionType;
    [SerializeField]
    private SkillTargetType _skillTargetType;
}

[CreateAssetMenu(fileName = "SkillData", menuName = "SkillData")]
public class SkillData : ScriptableObject
{
    [SerializeField]
    private string _skillOwnerName;
    [SerializeField]
    private SkillType[] _skillType; //스킬이 하나라도 여러 효과를 줄수 있는 경우를 생각해서 배열로 넣습니다.
    [SerializeField]
    private SkillAnimName _skillAnimName;
    [SerializeField]
    private float _damage;
    [SerializeField]
    private float _range;
    [Header("발동 확률")]
    [SerializeField]
    private float _activationRate;
    
    public float Damage => _damage;
    public float Range => _range;
    public float ActivationRate => _activationRate;
    private static int _skillHash = 0;
    public int GetSkillKeyToHash()
    {
        if(_skillHash == 0)
        {
            _skillHash = Animator.StringToHash(_skillAnimName.ToString());
        }
        return _skillHash;
    }
    public SkillType[] GetSkillType()
    {
        return _skillType;
    }
}
