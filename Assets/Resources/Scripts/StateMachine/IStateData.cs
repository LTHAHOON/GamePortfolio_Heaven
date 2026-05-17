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
#region 탑승 데이터
[Serializable]
public class BoardingStatData : IStateData<BoardingStatData>
{
    public int _maxCount = 7;
    [HideInInspector]
    public List<IPassenger> _boardingPassengerList = new();
    [HideInInspector]
    public DriveButton driveButton;


    // 길이 막혀서 탑승 못하는 생명체를 제외한 최종 탑승 가능한 생명체 수
    [HideInInspector]
    public int _finalMaxCount = 0;
    [HideInInspector]
    public int _curCount = 0;
    public void Clear()
    {
        _boardingPassengerList.Clear();
        _finalMaxCount = 0;
        _curCount = 0;
    }
    public BoardingStatData GetData()
    {
        return this;
    }
}
#endregion

#region 베지어 곡선 데이터
[Serializable]
public class BezierCurveStatData : IStateData<BezierCurveStatData>
{
    public float _curTime = 0;
    public bool _move = false;
    public Vector3 _startPoint;
    public Vector3 _middlePoint;
    public Vector3 _endPoint;
    public readonly float _maxTime = 15f;

    public BezierCurveStatData GetData()
    {
        return this;
    }
}
#endregion

#region 둘러쌓은 위치를 구하기 위한 데이터
[Serializable]
public class SurroundPosStatData : IStateData<SurroundPosStatData>
{
    //직렬화 안에서는 참조가 안되며 복사가 되어버립니다.
    public SurroundPosGroup _surroundPosGroup;
    public float _distanceFromUnit = 6;
    public float _radiusFromCenter = 6.5f;
    public int _firstRingCount = 9;
    public SurroundPosStatData GetData()
    {
        return this;
    }
}
#endregion

[Serializable]
public class BaseFSMStatData : IStateData<BaseFSMStatData>
{
    private Collider _targetCollider;
    public float _traceDistance = 10f;
    public float _attackDistance = 10f;
    
    public void SetTarget(Collider target)
    {
        _targetCollider = target;
    }
    public Collider TargetCollider => _targetCollider;
    public Vector3 TargetPosition => _targetCollider.transform.position;
    public BaseFSMStatData GetData()
    {
        return this;
    }
}

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
public class CreatureAnimatorStatData : IStateData<CreatureAnimatorStatData>
{
    public Animator _animator;
    [Header("애니메이션 속도 보완값")]
    public float _animatorSpeedMultiplier = 0.2f;
    public readonly Dictionary<CreatureAnimParameter, int> _dicAnimParameterHash = new()
    {
        { CreatureAnimParameter.IsWalk, Animator.StringToHash(CreatureAnimParameter.IsWalk.ToString())},
        { CreatureAnimParameter.NormalAttack, Animator.StringToHash(CreatureAnimParameter.NormalAttack.ToString())},
        { CreatureAnimParameter.WalkSpeed, Animator.StringToHash(CreatureAnimParameter.WalkSpeed.ToString())},
        { CreatureAnimParameter.GetHit, Animator.StringToHash(CreatureAnimParameter.GetHit.ToString())},
        { CreatureAnimParameter.Die, Animator.StringToHash(CreatureAnimParameter.Die.ToString())}
    };
    public CreatureAnimatorStatData GetData()
    {
        return this;
    }
}
public enum CreatureAnimParameter
{
    IsWalk,
    WalkSpeed,
    NormalAttack,
    GetHit,
    Die,
}
#endregion

#region 행성 및 우주 레이어 타겟 데이터
[Serializable]
public class LayerTargetStatData : IStateData<LayerTargetStatData>
{

    public LayerList _layerTargetList = new();
    public LayerTargetStatData GetData()
    {
        return this;
    }
}
#endregion

#region 죽을 때 필요한 데이터
[Serializable]
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


