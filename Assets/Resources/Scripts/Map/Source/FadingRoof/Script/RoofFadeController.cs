using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class RoofFadeController : MonoBehaviour
{
    [SerializeField]
    private string _ditherPowerRef = "_DitherPower";
    [SerializeField]
    private float _radius = 100f;
    [SerializeField]
    private float _maxOpacity = 2f;
    [SerializeField]
    private float _minOpacity = 0.5f;
    [SerializeField]
    private float _adjustmentValue = 80f;
    [SerializeField]
    private float _fadeSpeed = 2f;

    private Collider[] _hitResults;
    private RaycastHit _hit;
    private Camera _camera;
    private readonly HashSet<Collider> _curHitResults = new();
    private readonly HashSet<Collider> _prevHitResults = new();
    private readonly HashSet<Collider> _prevHitResultsCache = new();
    private static readonly Dictionary<Collider, float> _dicCurOpacity = new();
    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (!UIManager.Instance.IsSubCameraActive)
            return;
        FadeUpdateProcess();
    }

    private void OnDestroy()
    {
        _dicCurOpacity.Clear();
    }

    public static bool ContainFadeRoof(Collider roofCollider)
    {
        return _dicCurOpacity.ContainsKey(roofCollider);
    }
    private void FadeUpdateProcess()
    {
        #region 이전 Roof HashSet에서 사용하지 않는 Roof는 Clear시키기
        ClearNotUsedRoof();
        #endregion

        Vector3 raycastStart = _camera.transform.position;
        Vector3 raycastDirection = _camera.transform.forward;
        bool bGetHit = InputManager.Instance.TryGetByRaycast(out _hit, raycastStart, raycastDirection, _camera, GameLayerMask.RoofLayerMask);
        if (!bGetHit)
        {
            //히트가 안될 경우 모든 Roof는 원래대로
            #region 모든 이전 Roof를 MaxOpacity로 되돌리기
            if (_prevHitResults.Count <= 0) return;
            foreach(Collider prevHitResult in _prevHitResults)
            {
                if (!prevHitResult)
                    continue;
                if (!_dicCurOpacity.ContainsKey(prevHitResult))
                    continue;
                if (prevHitResult.gameObject.TryGetComponent(out Renderer renderer))
                {
                    float curOpacity = MoveTowardsOpacity(prevHitResult, _maxOpacity);
                    MPBPropertyControl.ChangeMaterialProperty<float>(renderer, _ditherPowerRef, curOpacity);
                }
            }
            #endregion
        }
        if (!_hit.collider) return;

        _hitResults = InputManager.Instance.GetByOverlapCast(out int count, _camera, _hit.point, _radius, GameLayerMask.RoofLayerMask);
        if (count <= 0) return;

        #region OverlapCast된 Roof를 Fade시키기(현재 Roof HashSet에 현재 Roof 추가하기)
        _curHitResults.Clear();
        for (int i = 0; i < count; i++)
        {
            if (!_hitResults[i])
                continue;
            if (_hitResults[i].gameObject.TryGetComponent(out Renderer renderer))
            {
                if (!_dicCurOpacity.ContainsKey(_hitResults[i]))
                {
                    _dicCurOpacity.Add(_hitResults[i], _maxOpacity);
                }
                _curHitResults.Add(_hitResults[i]);
                float dist = Vector3.Distance(_hitResults[i].transform.position, _hit.transform.position);
                float targetOpacity = Mathf.Clamp(dist / _adjustmentValue, _minOpacity, _maxOpacity);
                float curOpacity = MoveTowardsOpacity(_hitResults[i], targetOpacity);
                MPBPropertyControl.ChangeMaterialProperty<float>(renderer, _ditherPowerRef, curOpacity);
            }
        }
        #endregion

        //배열이 바뀔 경우 포함되지 않은 이전 Roof는 원래대로
        #region 현재 Roof HashSet에 포함안된 이전 Roof를 MaxOpacity로 되돌리기
        if (_prevHitResults.Count > 0)
        {
            foreach (Collider prevHitResult in _prevHitResults)
            {
                if (!prevHitResult)
                    continue;
                if (_curHitResults.Contains(prevHitResult))
                    continue;
                if (!prevHitResult.gameObject.TryGetComponent(out Renderer renderer))
                    continue;
                float curOpacity = MoveTowardsOpacity(prevHitResult, _maxOpacity);
                MPBPropertyControl.ChangeMaterialProperty<float>(renderer, _ditherPowerRef, curOpacity);
            }
        }
        #endregion

        #region 현재 Roof를 이전 Roof HashSet에 추가하기
        for (int i = 0; i < count; i++)
        {
            if (!_hitResults[i])
                continue;
            _prevHitResults.Add(_hitResults[i]);
        }
        #endregion
    }

    private float MoveTowardsOpacity(Collider hit, float targetOpacity)
    {
        float curOpacity = _maxOpacity;
        if (_dicCurOpacity.ContainsKey(hit))
        {
            curOpacity = _dicCurOpacity[hit];
            curOpacity = Mathf.MoveTowards(curOpacity, targetOpacity, Time.deltaTime * _fadeSpeed);
            _dicCurOpacity[hit] = curOpacity;
        }
        return curOpacity;
    }


    private void ClearNotUsedRoof()
    {
        if (_prevHitResults.Count > 0)
        {
            _prevHitResultsCache.Clear();
            _prevHitResultsCache.AddRange(_prevHitResults);
            foreach (Collider prevCollider in _prevHitResultsCache)
            {
                if (_dicCurOpacity.ContainsKey(prevCollider))
                {
                    float curOpacity = _dicCurOpacity[prevCollider];
                    if (IsMaxOpacity(curOpacity))
                    {
                        _dicCurOpacity.Remove(prevCollider);
                        _prevHitResults.Remove(prevCollider);
                    }
                }
                else
                {
                    _prevHitResults.Remove(prevCollider);
                }
            }
        }
    }

    private bool IsMaxOpacity(float curOpaicty) => curOpaicty >= _maxOpacity || (_maxOpacity - curOpaicty) < 0.001f;
    

    void OnDrawGizmos()
    {
        if (_hit.collider == null) return;

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(_hit.transform.position, _radius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
