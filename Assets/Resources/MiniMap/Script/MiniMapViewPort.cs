using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

[RequireComponent(typeof(MeshFilter))]
public class MiniMapViewPort : MonoBehaviour
{
    [SerializeField]
    private SubCameraController _subCameraController;
    void Awake()
    {
        _viewPortMeshFilter = GetComponent<MeshFilter>();
    }

    void Update()
    {
        CreateViewPortPlaneMesh();
    }

    [SerializeField]
    [Range(0.1f, 100f)]
    private float _viewPortBorderSize;
    private MeshFilter _viewPortMeshFilter;
    //다른 사람 프로젝트에 있는 ViewPort명으로 된 스크립트 참고했음.
    private void CreateViewPortPlaneMesh()
    {
        if (_subCameraController == null) { return; }
        if (Camera.main == null) { return; }
        if (_viewPortMeshFilter == null) { return; }

        //현재 해상도
        Vector3 resolution = new Vector2(Screen.width, Screen.height);

        //resolution.ToScale(Vector3 scale)은 scale이 해상도에 맞는 screenPoint로 변환
        Vector3 hitPoint;
        _subCameraController.GetCameraScreenPointOnGround(resolution.ToScale(Camera.main.rect.position), out hitPoint);
        Vector3 bottomLeft = hitPoint;
        _subCameraController.GetCameraScreenPointOnGround(resolution.ToScale(Camera.main.rect.position + Camera.main.rect.width * Vector2.right), out hitPoint);
        Vector3 bottomRight = hitPoint;
        _subCameraController.GetCameraScreenPointOnGround(resolution.ToScale((Camera.main.rect.position + Camera.main.rect.height * Vector2.up)), out hitPoint);
        Vector3 topLeft = hitPoint;
        _subCameraController.GetCameraScreenPointOnGround(resolution.ToScale((Camera.main.rect.position + Camera.main.rect.height * Vector2.up) + Camera.main.rect.width * Vector2.right), out hitPoint);
        Vector3 topRight = hitPoint;

        //꼭짓점을 통해 모서리의 중심점 구하기 (방향벡터를 정규화한후 2로 나누면 됨)
        Vector3 leftLine = (topLeft - bottomLeft).normalized / 2f;  
        Vector3 rightLine = (topRight - bottomRight).normalized / 2f;
        Vector3 bottomLine = (bottomRight - bottomLeft).normalized / 2f;
        Vector3 topLine = (topRight - topLeft).normalized / 2f;

        //윤곽선 크기 조절하기(-1를 곱하면 방향이 반대로 작용해서 모양 비율 유지가능함) 중요!!!!
        Vector3 bottomLeftOffset = (bottomLine + leftLine) * -1f * _viewPortBorderSize;
        Vector3 bottomRightOffset = (rightLine - bottomLine) * -1f * _viewPortBorderSize;
        Vector3 topLeftOffset = (topLine - leftLine) * -1f * _viewPortBorderSize;
        Vector3 topRightOffset = (-topLine - rightLine) * -1f * _viewPortBorderSize;

        //윤곽선을 위한 두번째 중심점
        Vector3 bottomLeft2 = bottomLeftOffset + bottomLeft;
        Vector3 bottomRight2 = bottomRightOffset + bottomRight;
        Vector3 topLeft2 = topLeftOffset + topLeft;
        Vector3 topRight2 = topRightOffset + topRight;

        //꼭짓점 리스트
        List<Vector3> vertexArray = new Vector3[] {
            bottomLeft2, // index = 0
            bottomRight2, // index = 1
            topLeft2, // index = 2
            topRight2, // index = 3
            bottomLeft, // index = 4
            bottomRight, // index = 5
            topLeft,  // index = 6
            topRight, // index = 7
        }.ToList();

        //방향 리스트
        List<Vector3> normalArray = new List<Vector3>(vertexArray.Count);
        for (int i = 0; i < vertexArray.Count; i++)
        {
            normalArray.Add(Vector3.up);
        }

        //인덱스를 통해 그리는 순서 구하기
        int[] indexArray = new int[] { 0, 4, 6, 2, 0, 1, 5, 4, 5, 1, 3, 7, 6, 7, 3, 2 }.Reverse().ToArray();

        //LocalPosition
        for (int i = 0; i < vertexArray.Count; i++)
        {
            vertexArray[i] = Camera.main.transform.InverseTransformPoint(vertexArray[i]);
        }

        if(_viewPortMeshFilter.sharedMesh == null) 
        {
            _viewPortMeshFilter.sharedMesh = new Mesh();
        }

        Mesh mesh = _viewPortMeshFilter.sharedMesh;
        mesh.name = "MiniMap View Port Mesh";
        mesh.Clear();
        mesh.SetVertices(vertexArray);
        mesh.SetNormals (normalArray);
        mesh.SetIndices(indexArray, MeshTopology.Quads, 0);
    }

}
