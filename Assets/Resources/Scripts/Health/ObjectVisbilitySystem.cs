using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ObjectVisbilitySystem : Singleton<ObjectVisbilitySystem>
{
    [SerializeField]
    private Canvas _ScreenUICanvas;
    [SerializeField]
    private LayerMask _checkObjectLayerMask;
    [SerializeField]
    private LayerMask _outPlanetLayerMask;
    [SerializeField]
    private float _maxCheckTime = 0.5f;

    private int _outPlanetLayer;
    private float _curCheckTime = 0;
    private List<ICullingUI> _cullingUIList = new();
    private Dictionary<ICullingUI, Collider> _dicTargetObj = new();
    private void Awake()
    {
        _outPlanetLayer = (int)Mathf.Log(_outPlanetLayerMask.value, 2);
        Debug.Log(_outPlanetLayer);
    }
    private void Update()
    {
        _curCheckTime += Time.deltaTime;
        if (_curCheckTime > _maxCheckTime)
        {
            ObjectVisibleProcess();
            ObjectVisibleProcess();
            _curCheckTime = 0;
        }
    }

    public void AddToList(ICullingUI cullingUI)
    {
        if (_cullingUIList.Contains(cullingUI) == false)
        {
            _cullingUIList.Add(cullingUI);
            _dicTargetObj.Add(cullingUI, cullingUI.ColliderForCulling);
        }
    }
    public void RemoveToList(ICullingUI loadingText)
    {
        if (_cullingUIList.Contains(loadingText))
        {
            _cullingUIList.Remove(loadingText);
            _dicTargetObj.Remove(loadingText);
        }
    }

    private void ObjectVisibleProcess()
    {
        bool isSubCameraActive = UIManager.Instance.IsSubCameraActive;
        Camera camera = Camera.main;

        _ScreenUICanvas.sortingOrder = isSubCameraActive ? 2 : 0;
        for (int i = 0; i < _cullingUIList.Count; i++)
        {
            ICullingUI cullingUI = _cullingUIList[i];
            if(cullingUI == null || !_dicTargetObj.TryGetValue(cullingUI, out Collider collider))
            {
                continue;
            }
            
            bool shouldActive;

            if (isSubCameraActive)
            {
                shouldActive = CheckFrustrum(collider, camera);
            }
            else
            {
                shouldActive = (collider.gameObject.layer == _outPlanetLayer)
                               && CheckFrustrum(collider, camera)
                               && CheckObjectVisible(camera, collider);
            }
            if(cullingUI.Owner)
            {
                if (cullingUI.Owner.activeSelf != shouldActive && !cullingUI.IsForceHideUI)
                {
                    cullingUI.Owner.SetActive(shouldActive);
                }

            }
        }

    }
    private bool CheckObjectVisible(Camera cam, Collider target)
    {
        Vector3 dir = (target.bounds.center - cam.transform.position);
        float distance = dir.magnitude;
        if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, distance, _checkObjectLayerMask))
        {
            return hit.collider == target;
        }

        return false;
    }
    private readonly Plane[] _planes = new Plane[6];
    private bool CheckFrustrum(Collider collider, Camera camera)
    {
        //오브젝트가 카메라 시야에 있는 지 확인하는 방법

        // TODO: 카메라 시야를 나타내는 6개의 평면 구하기
        GeometryUtility.CalculateFrustumPlanes(camera, _planes);

        // TODO: 콜라이더 경계 상자 가져오기 
        Bounds bounds = collider.bounds;

        // TODO: 6개의 평면과 콜라이더 경계 상자 간의 교차 확인하기
        bool isVisible = GeometryUtility.TestPlanesAABB(_planes, bounds);
        return isVisible;
    }
}
