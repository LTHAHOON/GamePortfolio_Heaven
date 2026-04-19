using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowObject : MonoBehaviour
{
    [SerializeField]
    private float _cameraUpOffset;
    [SerializeField]
    private Vector3 _screenOffset;
    public Vector3 _localOffset;

    public void FollowObject(Camera camera, GameObject target, GameObject ui, Vector3 screenOffset, Vector3 localOffset)
    {
        Vector3 worldPoint = target.transform.TransformPoint(localOffset);
        Vector3 sizeVector = camera.transform.up;

        Vector3 viewportPoint = camera.WorldToViewportPoint(worldPoint);
        Vector3 viewportSizePoint = camera.WorldToViewportPoint(worldPoint + sizeVector);

        float diff = (viewportSizePoint - viewportPoint).magnitude * 100f;

        viewportPoint -= 0.5f * Vector3.one;
        viewportPoint.z = 0;

        Rect rect = transform.parent.GetComponent<RectTransform>().rect;
        viewportPoint.x *= rect.width;
        viewportPoint.y *= rect.height;

        ui.transform.localPosition = (viewportPoint + screenOffset) + (_cameraUpOffset * diff * Vector3.up);
    }
    public void FollowObject(Camera camera, GameObject target, GameObject ui)
    {
        FollowObject(Camera.main, target, ui, _screenOffset, _localOffset);
    }
}
