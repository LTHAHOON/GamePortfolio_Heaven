using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIZoomResizer : MonoBehaviour
{
    private RectTransform rectTransform;
    private float _maxDiff = 62f;
    private float _minDiff = 50f;
    [SerializeField]
    private float size = 0.5f;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        ZoomResize();
    }

    private void ZoomResize()
    {
        float dist;
        float diff;
        if (UIManager.Instance.IsSubCameraActive)
        {
            dist = Mathf.Abs(UIManager.Instance.CurrentUICamera.transform.position.y);
            diff = dist * size;
            diff = Mathf.Clamp(diff, _minDiff, _maxDiff);
        }
        else
        {
            dist = Vector3.Distance(UIManager.Instance.CurrentUICamera.transform.position, transform.position);
            diff = (500f / dist) * size;
        }
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, diff);
    }
}
