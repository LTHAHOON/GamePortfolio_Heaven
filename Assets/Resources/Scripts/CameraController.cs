using System;
using Cinemachine;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [Serializable]
    public struct ScrollBlockPos
    {
        [Range(0, 500)]
        public float _minDistance;
        [Range(0, 500)]
        public float _maxDistance;
    }
    [SerializeField]
    private ScrollBlockPos _scrollBockPos;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFramingTransposer _vcFramingTransposer;
    [SerializeField]
    private float _cameraMoveSpeed = 20f;
    [SerializeField]
    private float _cameraScrollSpeed = 10f;
    private float m_CameraDistance;
    [SerializeField]
    private MouseCursorController _mouseCursorController;
    [SerializeField]
    private CameraTransformData _cameraTransformData;

    private float xRotate;
    private float yRotate;
    private float _startYRotation = -48f;
    private float _mouseSpeed = 700f;
    private Vector3 pos;
    private void Awake()
    {
        _mouseCursorController.ShowCursor(true);
        _vcFramingTransposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void Start()
    {
        CinemachineCore.GetInputAxis = (axis) =>
        {
            if (UIManager.Instance.IsSubCameraActive)
            {
                return 0;
            }
            if (Input.GetMouseButton(1))
            {
                _mouseCursorController.ShowCursor(false);
                Cursor.lockState = CursorLockMode.Locked;
                CameraMove();
                return Input.GetAxis(axis);
            }
            else
            {
                _mouseCursorController.ShowCursor(true);
                return 0;
            }
        };
    }

    private void LateUpdate()
    {
        BlockMoveByClamp();
        if (UIManager.Instance.IsSubCameraActive)
        {
            return;
        }
        ScrollMoveUpdate();
    }

    private void ScrollMoveUpdate()
    {
        float inputScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (inputScrollWheel != 0)
        {
            float offset = inputScrollWheel * _cameraScrollSpeed * Time.fixedTime;
            _vcFramingTransposer.m_CameraDistance = Mathf.Clamp(_vcFramingTransposer.m_CameraDistance + offset,
                _scrollBockPos._minDistance, _scrollBockPos._maxDistance);
        }
    }

    private void BlockMoveByClamp()
    {
        pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, _cameraTransformData.min_x, _cameraTransformData.max_x);
        pos.y = Mathf.Clamp(transform.position.y, _cameraTransformData.min_y, _cameraTransformData.max_y);
        pos.z = Mathf.Clamp(transform.position.z, _cameraTransformData.min_z, _cameraTransformData.max_z);
        transform.position = pos;
    }

    private void CameraMove()
    {

        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");
        Vector3 movedir = new Vector3(xMove, 0, zMove);
        movedir.Normalize();
        Vector3 _worldMovedir = _camera.transform.TransformDirection(movedir);
        _worldMovedir.y = 0;
        transform.Translate(_cameraMoveSpeed * _worldMovedir * Time.deltaTime, Space.World);

        float yMove = Input.GetAxis("Jump") - Input.GetAxis("LeftShift");
        Vector3 yMovedir = new Vector3(xMove, yMove, yMove);
        yMovedir.Normalize();
        yMovedir = new Vector3(0, yMovedir.y, 0);
        transform.Translate(_cameraMoveSpeed * yMovedir * Time.deltaTime, Space.World);
    }
    private void MouseMove()
    {

        float mouseX = Input.GetAxisRaw("Mouse X") * _mouseSpeed * Time.deltaTime;
        float mouseY = -Input.GetAxisRaw("Mouse Y") * _mouseSpeed * Time.deltaTime;
        xRotate += mouseY;
        yRotate += mouseX;
        xRotate = Mathf.Clamp(xRotate, -85, 85);
        Quaternion quat = Quaternion.Euler(new Vector3(xRotate, yRotate + _startYRotation, 0));
        transform.rotation = Quaternion.Slerp(transform.rotation, quat, Time.deltaTime * 20f);
    }
}
