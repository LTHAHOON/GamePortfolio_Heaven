using System;
using Cinemachine;
using UnityEngine;

public class SubCameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _subVcam;
    [SerializeField]
    private float _cameraMoveSpeed;
    [SerializeField]
    private float _cameraZoomSize;
    [SerializeField]
    private float _cameraZoomSpeed;

    [SerializeField]
    private MiniMapController _miniMapController;
    [Header("My Map SubCamera Position")]
    [SerializeField]
    private Vector3 _MyMapSubCameraPosition;
    [Header("Opponent Map SubCamera Position")]
    [SerializeField]
    private Vector3 _opponentMapSubCameraPosition;
    [SerializeField]
    private Vector3 _subCameraPositionDelta;
    [SerializeField]
    private CameraTransformData _allyTransformData;
    [SerializeField]
    private CameraTransformData _enemyTransformData;
    
    private Vector3 _pos;
    private Vector3 _curSubCameraPosition;
    private Vector3 _beforeSubCameraPosition;
    private Faction _curMapFaction = Faction.Ally;
    private void Awake()
    {
         _curSubCameraPosition = _MyMapSubCameraPosition + _subCameraPositionDelta;
        _beforeSubCameraPosition = _opponentMapSubCameraPosition + _subCameraPositionDelta;
        transform.position = _curSubCameraPosition;
    }
    private void LateUpdate()
    {
        if (!UIManager.Instance.IsSubCameraActive)
        {
            return;
        }
        
        if (_curMapFaction == Faction.Ally)
        {
            BlockMoveByClamp(_allyTransformData);
        }
        else
        {
            BlockMoveByClamp(_enemyTransformData);
        }
        _curSubCameraPosition = transform.position;
        CameraMove();
        CameraScroll();
    }

    private void Update()
    {
        if (UIManager.Instance.IsSubCameraActive == _subVcam.enabled)
            return;
        
        _subVcam.enabled = UIManager.Instance.IsSubCameraActive;
    }

    public void CameraMoveToOtherPlanet()
    {
        if (_curMapFaction == Faction.Ally)
        {
            _curMapFaction = Faction.Enemy;
        }
        else
        {
            _curMapFaction = Faction.Ally;
        }
        Vector3 oldPos = transform.position;
        transform.position = _beforeSubCameraPosition;
        _subVcam.PreviousStateIsValid = false;
        Vector3 delta = transform.position - oldPos;
        
        _subVcam.OnTargetObjectWarped(transform, delta);

        _miniMapController.ChangeMiniMapCameraRectTransform(_MyMapSubCameraPosition, _opponentMapSubCameraPosition);
        _beforeSubCameraPosition = _curSubCameraPosition;

    }


    private void CameraMove()
    {
        float mouseX = Input.GetAxis("Mouse X") * _cameraMoveSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * -_cameraMoveSpeed;
        if (Input.GetMouseButton(2))
        {
            transform.position += new Vector3(mouseY, 0, mouseX);

        }
    }

    public float _curCameraZoomSpeed;
    private void CameraScroll()
    {
        
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel") * -_cameraZoomSize;
        _curCameraZoomSpeed = Mathf.Lerp(_curCameraZoomSpeed, scroll, Time.deltaTime * _cameraZoomSpeed);
        if (Mathf.Approximately(_curCameraZoomSpeed, scroll))
        {
            _curCameraZoomSpeed = 0f;
        }

        float localPosY = transform.position.y + _curCameraZoomSpeed;
        transform.position = new Vector3(
                transform.transform.position.x,
                localPosY,
                transform.transform.position.z);

    }
    
    private void BlockMoveByClamp(CameraTransformData cameraTransformData)
    {
        _pos = transform.position;
        _pos.x = Mathf.Clamp(transform.position.x, cameraTransformData.min_x, cameraTransformData.max_x);
        _pos.y = Mathf.Clamp(transform.position.y, cameraTransformData.min_y, cameraTransformData.max_y);
        _pos.z = Mathf.Clamp(transform.position.z, cameraTransformData.min_z, cameraTransformData.max_z);
        transform.position = _pos;
    }
    
    //Plane Raycast이용
    public static Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    public bool GetCameraScreenPointOnGround(Vector3 screenPoint, out Vector3 hitPoint)
    {
        hitPoint = Vector3.one;
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        float rayDistance = 1000f;
        if(groundPlane.Raycast(ray, out rayDistance))
        {
            hitPoint = ray.GetPoint(rayDistance);
            return true;
        }
        return false;
    }
}
