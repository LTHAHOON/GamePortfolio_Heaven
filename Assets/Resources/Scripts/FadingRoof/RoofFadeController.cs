using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class RoofFadeController : MonoBehaviour
{
    [Header("지붕 레이어마스크")]
    [SerializeField]
    private LayerMask _roofLayerMask;
    [SerializeField]
    private string _ditherPowerRef = "_DitherPower";
    [SerializeField]
    private float _radius = 30f;
    [SerializeField]
    private float _maxOpacity = 2f;
    [SerializeField]
    private float _minOpacity = 0.5f;
    [SerializeField]
    private float _adJustmentValue = 10;
    private Collider[] _hitResults;
    private RaycastHit _hit;
    private Camera _camera;
    private readonly HashSet<Collider> _curHitResults = new();
    private readonly HashSet<Collider> _prevHitResults = new();

    private void Awake()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        Ray ray = new(_camera.transform.position, _camera.transform.forward);
        bool bGetHit = InputManager.Instance.TryGetByRaycast(out _hit,_camera, ray, _roofLayerMask);
        if (!bGetHit)
        {
            if (_hitResults == null) return;
            for (int i = 0; i < _hitResults.Length; i++)
            {
                if(!_hitResults[i])
                    continue;
                if (_hitResults[i].gameObject.TryGetComponent(out Renderer renderer))
                {
                    MPBPropertyControl.ChangeMaterialProperty<float>(renderer, _ditherPowerRef, _maxOpacity);
                }
            }
        }
        if (!_hit.collider) return;
        
        _hitResults = InputManager.Instance.GetByOverlapCast(out int count, _camera, _hit.point, _radius, _roofLayerMask);
        if(count <= 0) return;
        _curHitResults.Clear();
        for (int i = 0; i < count; i++)
        {
            if(!_hitResults[i])
                continue;
            if(_hitResults[i].gameObject.TryGetComponent(out Renderer renderer))
            {
                float dist = Vector3.Distance(_hitResults[i].transform.position, _hit.transform.position);
                float opacity = Mathf.Clamp(dist / _adJustmentValue, _minOpacity, _maxOpacity);
                MPBPropertyControl.ChangeMaterialProperty<float>(renderer, _ditherPowerRef, opacity);
            }
            _curHitResults.Add(_hitResults[i]);
        }
        if (_prevHitResults.Count > 0)
        {
            foreach (Collider prevHitResult in _prevHitResults)
            {
                if(!prevHitResult)
                    continue;
                if (!_curHitResults.Contains(prevHitResult))
                {
                    if (prevHitResult.gameObject.TryGetComponent(out Renderer renderer))
                    {
                        MPBPropertyControl.ChangeMaterialProperty<float>(renderer, _ditherPowerRef, _maxOpacity);
                    }
                }
            }
        }
        _prevHitResults.Clear();
        for (int i = 0; i < count; i++)
        {
            if(!_hitResults[i])
                continue;
            _prevHitResults.Add(_hitResults[i]);
        }
    }
    void OnDrawGizmos()
    {
        if (_hit.collider == null) return;

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(_hit.transform.position, _radius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
