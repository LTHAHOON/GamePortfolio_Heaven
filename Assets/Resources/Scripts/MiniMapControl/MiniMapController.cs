using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniMapController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private RectTransform _miniMapCameraRectTransform;
    [SerializeField]
    private Transform _miniMapViewPort;
    [SerializeField]
    private Camera _renderCamera;
    [SerializeField]
    private RectTransform _zoomTargetMiniMap;
    [SerializeField]
    private Scrollbar _zoomScrollBar;

    private float _curZoomScrollValue;
    private Vector3 baseLocalScale;
    void Awake()
    {
        _curZoomScrollValue = _zoomScrollBar.value;
        baseLocalScale = _zoomTargetMiniMap.transform.localScale;
    }

    void Update()
    {
        UpdateZoomScroll(_curZoomScrollValue > _zoomScrollBar.value);

    }
    public void ChangeMiniMapCameraRectTransform(Vector3 position, Vector3 otherPosition)
    {
        position.x = _miniMapCameraRectTransform.localPosition.x;
        otherPosition.x = _miniMapCameraRectTransform.localPosition.x;
        position.y = 0;
        otherPosition.y = 0;
        if(_miniMapCameraRectTransform.localPosition == position)
        {
            _miniMapCameraRectTransform.localPosition = otherPosition;
        }
        else
        {
            _miniMapCameraRectTransform.localPosition = position;
        }
    }

    [SerializeField]
    private float _epsilonZoomValue = 0.01f;
    [SerializeField]
    private float _epsilonZoomPivotValue = 0.2f;
    [SerializeField]
    private Vector2 _miniMapBasePivot = new(0.5f, 0.5f);
    [SerializeField]
    private float _zoomSpeed = 10f;
    [SerializeField]
    private float _zoomPivotSpeed = 2f;
    private void UpdateZoomScroll(bool isZoomOut)
    {
        Vector3 worldPosition = _miniMapViewPort.transform.position;
        Vector2 viewportPoint = _renderCamera.WorldToViewportPoint(worldPosition);
        if (isZoomOut)
        {
            if (_curZoomScrollValue <= _epsilonZoomPivotValue)
            {
                viewportPoint = _miniMapBasePivot;
            }
        }
        if (Mathf.Abs(_curZoomScrollValue - _zoomScrollBar.value) > _epsilonZoomValue)
        {
            UpdateZoom();
            _zoomTargetMiniMap.pivot = Vector3.Lerp(_zoomTargetMiniMap.pivot, viewportPoint, Time.deltaTime * _zoomSpeed);
        }
        else
        {
            _curZoomScrollValue = _zoomScrollBar.value;
            UpdateZoomPivot(viewportPoint);
        }
    }

    private void UpdateZoom()
    {
        _curZoomScrollValue = Mathf.Lerp(_curZoomScrollValue, _zoomScrollBar.value, Time.deltaTime * _zoomSpeed);
        Vector3 localScale = GetIncreasedZoomScale(_curZoomScrollValue);
        _zoomTargetMiniMap.localScale = localScale;
    }
    private void UpdateZoomPivot(Vector3 targetPivot)
    {
        if (_curZoomScrollValue > _epsilonZoomPivotValue)
        {
            _zoomTargetMiniMap.pivot = Vector3.Lerp(_zoomTargetMiniMap.pivot, targetPivot, Time.deltaTime * _zoomPivotSpeed);
        }
    }

    public static bool IsPointerOverMiniMap;
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsPointerOverMiniMap = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsPointerOverMiniMap = false;
    }

    private Vector3 GetIncreasedZoomScale(float zoomScaleDelta)
    {
        return new Vector3(baseLocalScale.x + zoomScaleDelta, baseLocalScale.y + zoomScaleDelta, 1f);
    }

    [SerializeField]
    private float _zoomValue;
    public void OnClickZoomIn()
    {
        if (_zoomScrollBar.value < 1f)
        {
            _zoomScrollBar.value += _zoomValue;
        }
      
    }

    public void OnClickZoomOut()
    {
        if (_zoomScrollBar.value >= _zoomValue)
        {
            _zoomScrollBar.value -= _zoomValue;
        }
    }

}
