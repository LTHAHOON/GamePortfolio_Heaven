using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
public struct Goal
{
    public Vector3 _spacecraftGoalPos;
    public Vector3 _passengerGoalPos;
    public RespawnPositionType _respawnPositionType;
}

[RequireComponent(typeof(StatusComponent))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class SpacecraftController : PassengerController
{
    [SerializeField]
    private LayerList _layerList = new();
    [SerializeField]
    private Health _health;
    [SerializeField]
    private CreateLoad _createLoad;
    private UnitType _unitType;
    private RuntimeUnitStatus _status;

    [SerializeField]
    private float gravity = -9.81f;

    private BoxCollider _collider;
    private Rigidbody _rigidbody;

    [HideInInspector]
    public Vector3 colliderSizeData;
    private event Action<GameObject> OnReturnAttackMark;
    private int _unitTypeLayer;
    public void SetStatus(RuntimeUnitStatus status)
    {
        _status = status;
    }
    void Awake()
    {
        _unitTypeLayer = LayerMask.NameToLayer(UnitType.Spacecraft.ToString());
        _status = GetComponent<StatusComponent>().GetStatus();
        _unitType = GetComponent<StatusComponent>().GetUnitData().Type;
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
        colliderSizeData = _collider.size;
        _collider.isTrigger = true;
    }

    void FixedUpdate()
    {
        if (_isGravity && !_rigidbody.isKinematic)
        {
            GravityMove();
        }

    }

    private float _distanceFromCent = 1f;
    private bool _bIsDriving = false;
    private bool _bOnceInit = true;
    private void Update()
    {
        if (_createLoad.IsLoadReady)
        {
            return;
        }
        else if (_bOnceInit)
        {
            MyUnitPrefabDataControl.Instance.AddUnitPrefabToList(_unitType, this);
            _health.InitHealth();
            TransparentMaterialControl.SetQpaqueOrTransparentControl(gameObject, _unitType, TransparentMaterialControl.SurfaceType.Opaque, new Color32(255, 255, 255, 255));
            _bOnceInit = false;
        }

        if (_move)
        {
            _move = MoveToGoal();
            if (_move == false)
            {
                Landing();
            }
        }
    }
    private void Landing()
    {
        transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        transform.position = _goalData._spacecraftGoalPos;
        _layerList.SetLayerList(gameObject, true, _unitTypeLayer);
        _isGravity = true;
    }

    private void GetOff()
    {
        List<PassengerData> passengerList = GetPassengerDatas();
        for (int i = 0; i < passengerList.Count; i++)
        {
            int[] positionCountArray = SurroundPosManager.GetPositionCountArray(passengerList[i]._passengerCount, 10);
            float[] distancesArray = SurroundPosManager.DistanceArrayByCharacterCount(passengerList[i]._passengerCount, 5, 7, 10);
            Vector3[] arrGoalPos = SurroundPosManager.GetTargetPositionsAround(_goalData._passengerGoalPos, distancesArray, positionCountArray);
            for (int j = 0; j < passengerList[i]._passengerCount; j++)
            {
                if (NavMesh.SamplePosition(_goalData._spacecraftGoalPos + Random.onUnitSphere * _distanceFromCent, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                {
                    passengerList[i]._passenger.transform.position = hit.position;
                }
                CreatureFSM fsm = Instantiate(passengerList[i]._passenger, _passengerParent);
                MyUnitPrefabDataControl.Instance.AddUnitPrefabToList(UnitType.Creature, fsm);
                fsm.SetStatus(StatusSliderController._status);
                fsm.TargetPosition = arrGoalPos[j];
                if (j == 0)
                {
                    fsm._attackMark = _attackMark;
                    fsm.OnReturnAttackMark += OnReturnAttackMark;
                }
                fsm.SetEnemyNexusTargetPos(_enemytNexusPos);
                fsm.SetIsAttackTarget(true);
                fsm.SetIsAttackMode(true);
            }
        }

    }


    private void GravityMove()
    {
        _rigidbody.velocity += gravity * Time.fixedDeltaTime * Vector3.up;
    }

    private Quaternion _rotationForGoal;
    public void SaveRotation(Vector3 euler)
    {
        _rotationForGoal = Quaternion.Euler(euler);
    }

    private float t = 0;
    private bool _move = false;
    private Vector3 _startPoint;
    private Vector3 _middlePoint;
    private Vector3 _endPoint;
    private const float _maxTime = 15f;
    private bool MoveToGoal()
    {
        if (_middlePoint == Vector3.zero)
        {
            Vector3 p = ((1 - t / _maxTime) * _startPoint) + (t / _maxTime * _endPoint);
            t += Time.deltaTime;
            transform.position = p;
        }
        else
        {
            Vector3 e = ((1 - t / _maxTime) * _startPoint) + (t / _maxTime * _middlePoint);
            Vector3 f = ((1 - t / _maxTime) * _middlePoint) + (t / _maxTime * _endPoint);
            Vector3 p = ((1 - t / _maxTime) * e) + (t / _maxTime * f);
            if (_status != null)
            {
                t += Time.deltaTime * (_status.DEX / 1.2f);
            }
            else
            {
                t += Time.deltaTime;
            }

            Vector3 dir = p - transform.position;
            Quaternion rot = Quaternion.LookRotation(dir.normalized);
            Vector3 changedEulerAngle = rot.eulerAngles;
            Vector3 curEulerAngle = transform.rotation.eulerAngles;
            changedEulerAngle.x = curEulerAngle.x;
            rot = Quaternion.Euler(changedEulerAngle);

            transform.rotation = rot;
            transform.position = p;
        }
        return t < _maxTime;
    }

    private Goal _goalData;
    private Vector3 _enemytNexusPos;
    public void SetGoal(Vector3 startPoint, Vector3 endPoint, Vector3 middlePoint, Goal goalData, Vector3 enemyNexusPos)
    {
        _startPoint = startPoint;
        _endPoint = endPoint;
        _goalData = goalData;
        _goalData._spacecraftGoalPos.y = 5f;
        _enemytNexusPos = enemyNexusPos;
        _middlePoint = middlePoint;
        _move = true;
    }

    private Transform _passengerParent;
    public void SetPassenger(CreatureFSM creature, int passengerCount, Transform parent)
    {
        _bIsDriving = true;
        _passengerParent = parent;
        SetPassengerInData(creature, passengerCount);
    }

    private GameObject _attackMark;
    public void SetReturnAttackMark(GameObject attckMark, Action<GameObject> returnAttackmark)
    {
        _attackMark = attckMark;
        OnReturnAttackMark += returnAttackmark;
    }


    public int GetPassengerCount(long id)
    {
        return GetPassengerCountInData(id);
    }

    public bool _isGravity = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") && _isGravity)
        {
            if (!_bIsDriving)
            {
                _createLoad.StartCreateLoad();
            }
            _collider.isTrigger = false;
            _rigidbody.isKinematic = true;
            _bIsDriving = false;
            GetOff();
            ClearPassengerDatas();
        }
    }

    public CreateLoad GetCreateLoad() => _createLoad;
    public LayerList GetLayerList() => _layerList;

}
