using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public interface IStateData { }

public interface IStateData<T> : IStateData
{
    T GetData();
}

#region 둘러쌓은 위치를 구하기 위한 데이터
[Serializable]
public class SurroundPosStatData : IStateData<SurroundPosStatData>
{
    public float _distanceFromUnit = 6;
    public float _radiusFromCenter = 6.5f;
    public int _firstRingCount = 9;
    public SurroundPosStatData GetData()
    {
        return this;
    }
}
#endregion

#region NavMesh 데이터
[Serializable]
public class NavMeshStatData : IStateData<NavMeshStatData>
{
    public NavMeshObstacle _navMeshObstacle;
    public NavMeshAgentStatData _navmeshAgentData;
    public NavMeshStatData GetData()
    {
        return this;
    }
}
[Serializable]
public class NavMeshAgentStatData : IStateData<NavMeshAgentStatData>
{
    public NavMeshAgent _navMeshAgent;
    [Header("이동 가능한 거리")]
    public float _traceDistance;
    [Header("적 추적 가능한 사이즈")]
    public float _traceRaidus;
    [Header("적 넥서스 공격 가능한 거리")]
    public float _nexusAttackDistance;

    public bool IsNavPathInValid => _navMeshAgent.hasPath && _navMeshAgent.path.status != NavMeshPathStatus.PathComplete;

    public NavMeshAgentStatData GetData() 
    { 
        return this;
    }
}
#endregion

#region 공격 발동 확률 데이터
[Serializable]
public class AttackActivationStatData : IStateData<AttackActivationStatData>
{
    public Dictionary<int, float> _dicAttackActivationRate = new();
    public RandomProb _attackRandomProb = new();
    [Header("기본 공격 발동 확률")]
    public float _normalAttackActivationRate = 80f;
    public AttackActivationStatData GetData()
    {
        return this;
    }
}
#endregion

#region 애니메이터 데이터
[Serializable]
public class AnimatorStatData : IStateData<AnimatorStatData>
{
    public Animator _animator;
    [Header("애니메이션 속도 보완값")]
    public float animatorSpeedMultiplier = 0.2f;
    public readonly Dictionary<AnimParameter, int> _dicAnimParameterHash = new()
    {
        { AnimParameter.IsWalk, Animator.StringToHash(AnimParameter.IsWalk.ToString())},
        { AnimParameter.NormalAttack, Animator.StringToHash(AnimParameter.NormalAttack.ToString())},
        { AnimParameter.WalkSpeed, Animator.StringToHash(AnimParameter.WalkSpeed.ToString())},
        { AnimParameter.GetHit, Animator.StringToHash(AnimParameter.GetHit.ToString())},
        { AnimParameter.Die, Animator.StringToHash(AnimParameter.Die.ToString())}
    };
    public AnimatorStatData GetData()
    {
        return this;
    }
}
public enum AnimParameter
{
    IsWalk,
    WalkSpeed,
    NormalAttack,
    GetHit,
    Die,
}
#endregion

#region 행성 및 우주 레이어 데이터
[Serializable]
public class LayerStatData : IStateData<LayerStatData>
{
    [Header("상대 레이어")]
    public LayerMask _enemyTargetLayer;
    public LayerStatData GetData()
    {
        return this;
    }
}
#endregion

#region 죽을 때 필요한 데이터
public class DieStatData : IStateData<DieStatData>
{
    [Header("사망 후 오브젝트 제거 딜레이 시간")]
    public float _dieDelayTime = 4f;
    public DieStatData GetData()
    {
        return this;
    }
}
#endregion


