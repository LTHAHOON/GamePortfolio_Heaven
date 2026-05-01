using System;
using UnityEngine;

public class GridIndicatorController : MonoBehaviour
{
    public enum GridIndicatorProperty
    {
        _Color,
        _Tiling,
    }

    

    [SerializeField]
    private MeshRenderer _meshRenderer;
    [SerializeField]
    private BoxCollider _boxCollider;
    [SerializeField]
    private PlacementSystem _placementSystem;

    private Color _blueColor;
    private Color _redColor;
    private float _redIntensity = 5f;
    private bool _isTriggerEnter = false;
    private void Start()
    {
        _blueColor = _meshRenderer.material.color;
        _redColor = Color.red * _redIntensity;

    }

    private void OnDisable()
    {
        _checkLocalPositions = null;
    }

    private void Update()
    {
        if(!_isTriggerEnter)
        {
            CheckPosInBoxCollider();

        }
        else
        {
            _isCreatableUnit = false;
        }
        if(_isCreatableUnit)
        {
            SetGridIncdicatorColor(_blueColor);
        }
        else
        {
            SetGridIncdicatorColor(_redColor);
        }
    }

    private Vector3[] GetArrayVertexVector()
    {
        Vector3 adjustedSize = Vector3.Scale(_boxCollider.size, transform.localScale);

        Vector3 boxSize = adjustedSize;


        float posX_Forward = boxSize.x * _halfSizeMultiplier; //박스 앞 위치
        float posX_BackWard = -boxSize.x * _halfSizeMultiplier; //박스 뒤 위치

        float PosY_Fiexed = boxSize.y * 0.5f; //박스 높이 위치

        float posZ_Left = boxSize.z * _halfSizeMultiplier; //박스 왼쪽 위치
        float posZ_Right = -boxSize.z * _halfSizeMultiplier; //박스 오른쪽 위치

        return new Vector3[] { new Vector3(posX_Forward, PosY_Fiexed, posZ_Left), new Vector3(posX_Forward, PosY_Fiexed, posZ_Right),
                               new Vector3(posX_BackWard, PosY_Fiexed, posZ_Left), new Vector3(posX_BackWard, PosY_Fiexed, posZ_Right)};
    }


    private Vector3[] _checkLocalPositions;
    private const float _halfSizeMultiplier = 0.5f;
    public static bool _isCreatableUnit;
    private void CheckPosInBoxCollider()
    {
        if(_checkLocalPositions == null)
        {
            //TODO: 꼭짓점 로컬 좌표 구하기
            _checkLocalPositions = GetArrayVertexVector();
        }
        else
        {
            for (int i = 0; i < _checkLocalPositions.Length; i++)
            {
                //꼭짓점 구하는 공식(비트연산자 응용)
                /*  Vector3 cornerOffset = new Vector3(
                        (i & 1) == 0 ? boxSize.x * 0.5f : -boxSize.x * 0.5f,
                        (i & 2) == 0 ? boxSize.y * 0.5f : -boxSize.y * 0.5f,
                        (i & 4) == 0 ? boxSize.z * 0.5f : -boxSize.z * 0.5f
                );
                    checkPositions[i] = transform.TransformPoint(cornerOffset);
                */

                //TODO: _checkLocalPositions의 로컬 좌표를 월드 좌표로 변환
                Vector3 checkWorldPositions = transform.TransformPoint(_checkLocalPositions[i]);
                if (_placementSystem.ComparePosInBoxCollider(checkWorldPositions) || _placementSystem.IsEqualToPos(transform.position))
                {
                    _isCreatableUnit = false;
                    return;
                }
            }
            _isCreatableUnit = true;
           
        }
    }
 

    public static void SetIsCreatableUnit(bool isCreatableUnit)
    {
        _isCreatableUnit = isCreatableUnit;
    }

    private void SetGridIncdicatorColor(Color color)
    {
        string colorProperty2 = Enum.GetName(typeof(GridIndicatorProperty), GridIndicatorProperty._Color);
        _meshRenderer.material.SetColor(colorProperty2, color);
    }

    public void ChangeGridIndicator(int horizontal, int vertical)
    {
        Debug.Log("asdasd");
        transform.localScale = new Vector3(horizontal, 1f, vertical);
        //_meshRenderer.material.SetVector("_Tiling", new Vector2(vertical, horizontal));
    }
    private void OnTriggerStay(Collider other)
    {
        _isTriggerEnter = true;
    }
    private void OnTriggerExit(Collider other)
    {
        _isTriggerEnter = false;
    }

    public static bool IsCreatableUnit => _isCreatableUnit;

}
