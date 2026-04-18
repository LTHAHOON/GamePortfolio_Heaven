using UnityEngine;

public class SubCameraController : MonoBehaviour
{
    [SerializeField]
    private float _cameraMoveSpeed;
    [SerializeField]
    private float _cameraZoomSize;
    [SerializeField]
    private float _cameraZoomSpeed;
    [SerializeField]
    private Camera _thisCamera;

    private float _MaxY = -400f;
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
        transform.localPosition = _curSubCameraPosition;
    }
    void Update()
    {
        _curSubCameraPosition = transform.localPosition;
        CameraMove();
    }

    public void CameraMoveToOtherPlanet()
    {
        transform.localPosition = _beforeSubCameraPosition;
        _miniMapController.ChangeMiniMapCameraRectTransform(_MyMapSubCameraPosition, _opponentMapSubCameraPosition);
        _beforeSubCameraPosition = _curSubCameraPosition;
    }


    private void CameraMove()
    {
        float mouseX = Input.GetAxis("Mouse X") * _cameraMoveSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * -_cameraMoveSpeed * Time.deltaTime;
        if (Input.GetMouseButton(2))
        {
            transform.position += new Vector3(mouseY, 0, mouseX);

        }
        else
        {
            CameraScroll();
        }
    }

    public float _curCameraZoomSpeed;
    private void CameraScroll()
    {
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel") * -_cameraZoomSize * Time.deltaTime;
        _curCameraZoomSpeed = Mathf.Lerp(_curCameraZoomSpeed, scroll, Time.deltaTime * _cameraZoomSpeed);
        if (Mathf.Approximately(_curCameraZoomSpeed, scroll))
        {
            _curCameraZoomSpeed = 0f;
        }

        float localPosY = Mathf.Clamp(_thisCamera.transform.localPosition.y + _curCameraZoomSpeed, _MinY, _MaxY);
        _thisCamera.transform.localPosition = new Vector3(
                _thisCamera.transform.localPosition.x,
                localPosY,
                _thisCamera.transform.localPosition.z);
    }

    //Plane Raycast이용
    public static Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    public bool GetCameraScreenPointOnGround(Vector3 screenPoint, out Vector3 hitPoint)
    {
        hitPoint = Vector3.one * float.NaN;
        
        Ray ray = _thisCamera.ScreenPointToRay(screenPoint);
        float rayDistance;
        if(groundPlane.Raycast(ray, out rayDistance))
        {
            hitPoint = ray.GetPoint(rayDistance);
            return true;
        }
        return false;
    }
}
