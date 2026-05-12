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
    private float _MaxY = -400f;
    [SerializeField]
    private float _MinY = -450f;

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
    private Vector3 _curSubCameraPosition;
    private Vector3 _beforeSubCameraPosition;
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
        _curSubCameraPosition = transform.position;
        CameraMove();
        CameraScroll();
    }

    public void CameraMoveToOtherPlanet()
    {
        Vector3 oldPos = transform.position;

        transform.position = _beforeSubCameraPosition;

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

        float localPosY = Mathf.Clamp(transform.position.y + _curCameraZoomSpeed, _MinY, _MaxY);
        transform.position = new Vector3(
                transform.transform.position.x,
                localPosY,
                transform.transform.position.z);

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
