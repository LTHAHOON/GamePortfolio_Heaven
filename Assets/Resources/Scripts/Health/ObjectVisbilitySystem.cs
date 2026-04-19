using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ObjectVisbilitySystem : MonoBehaviour
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
    private static List<HealthBar> _healthBarList = new();
    private static List<CreateLoadComponent> _createLoadingTextList = new();
    private static Dictionary<object, Collider> _dicTargetObj = new();
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
            ObjectVisibleProcess(_healthBarList);
            ObjectVisibleProcess(_createLoadingTextList);
            _curCheckTime = 0;
        }
    }

    public static void AddToList(HealthBar healthBar)
    {
        if (_healthBarList.Contains(healthBar) == false)
        {
            _healthBarList.Add(healthBar);
            _dicTargetObj.Add(healthBar, healthBar._health._collider);
        }
    }
    public static void AddToList(CreateLoadComponent loadingText)
    {
        if (_createLoadingTextList.Contains(loadingText) == false)
        {
            _createLoadingTextList.Add(loadingText);
            _dicTargetObj.Add(loadingText, loadingText._createLoad._collider);
        }
    }

    public static void RemoveToList(HealthBar healthBar)
    {
        if (_healthBarList.Contains(healthBar))
        {
            _healthBarList.Remove(healthBar);
            _dicTargetObj.Remove(healthBar);
        }
    }
    public static void RemoveToList(CreateLoadComponent loadingText)
    {
        if (_createLoadingTextList.Contains(loadingText))
        {
            _createLoadingTextList.Remove(loadingText);
            _dicTargetObj.Remove(loadingText);
        }
    }

    private void ObjectVisibleProcess<T>(List<T> objList) where T : Component
    {
        bool isSubCameraActive = UIManager.Instance.IsSubCameraActive;
        Camera camera = Camera.main;

        _ScreenUICanvas.sortingOrder = isSubCameraActive ? 2 : 0;
        for (int i = 0; i < objList.Count; i++)
        {
            T obj = objList[i];
            if(!obj || !_dicTargetObj.TryGetValue(obj, out Collider collider))
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
            if(obj.gameObject)
            {
                if (obj.gameObject.activeSelf != shouldActive)
                {
                    obj.gameObject.SetActive(shouldActive);
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
    private bool CheckFrustrum(Collider collider, Camera camera)
    {
        //오브젝트가 카메라 시야에 있는 지 확인하는 방법

        // TODO: 카메라 시야를 나타내는 6개의 평면 구하기
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

        // TODO: 콜라이더 경계 상자 가져오기 
        Bounds bounds = collider.bounds;

        // TODO: 6개의 평면과 콜라이더 경계 상자 간의 교차 확인하기
        bool isVisible = GeometryUtility.TestPlanesAABB(planes, bounds);
        return isVisible;
    }
}
