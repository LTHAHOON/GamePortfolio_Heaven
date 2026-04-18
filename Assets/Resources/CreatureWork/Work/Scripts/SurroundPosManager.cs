using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SurroundPosManager : MonoBehaviour
{
    private struct SurrondPos
    {
        public Vector3 _position;
        public int _positionIndex;

        public SurrondPos(Vector3 position, int positionIndex)
        {
            _position = position;
            _positionIndex = positionIndex;
        }
    }

    private static Dictionary<int, SurrondPos> _dicAssignedPos = new();
    private static HashSet<int> _assignedPosIndices = new();

    public static float[] DistanceArrayByCharacterCount(int chracterCount, float distanceFromUnit = 5f, float radiusFromCenter = -1f, int firstRingCount = 10)
    {
        int characterCount = chracterCount;
        int ringCount;
        if (characterCount > firstRingCount)
        {
            ringCount = ((characterCount / firstRingCount) + 1);
        }
        else
        {
            return radiusFromCenter < 0 ? new float[] { distanceFromUnit } : new float[] { radiusFromCenter };
        }
        float[] distanceArray = new float[ringCount];
        for (int i = 0; i < distanceArray.Length; i++)
        {
            if (radiusFromCenter < 0)
            {
                distanceArray[i] = ((i + 1) * distanceFromUnit);
            }
            else
            {
                distanceArray[i] = ((i + 1) * radiusFromCenter);
                radiusFromCenter = -1;
            }
        }

        return distanceArray;
    }

    public static int[] GetPositionCountArray(int chracterCount, int firstRingCount = 10)
    {
        int ringCount;
        if (chracterCount > firstRingCount)
        {
            ringCount = ((chracterCount / firstRingCount) + 1);
        }
        else
        {
            return new int[] { firstRingCount };
        }
        int[] positionCountArray = new int[ringCount];

        for (int i = 0; i < positionCountArray.Length; i++)
        {
            positionCountArray[i] = firstRingCount * (i + 1);
        }

        return positionCountArray;
    }


    private static Vector3[] GetTargetPositionsAround(Vector3 startPosition, float distance, int positionCount, float radiusFromCenter = -1f)
    {
        if (positionCount < 0) return new Vector3[0];
        Vector3[] positions = new Vector3[positionCount];
        for (int i = 0; i < positionCount; i++)
        {
            float angle = i * (360f / positionCount);
            Vector3 dir = ApplyRotationToVector(Vector3.forward, angle);
            Vector3 postition = startPosition + dir * distance;
            positions[i] = postition;
        }
        return positions;
    }


    public static Vector3[] GetTargetPositionsAround(Vector3 startPosition, float[] distanceArray, int[] positionCountArray, bool bUseCenter = true)
    {
        if (distanceArray.Length != positionCountArray.Length)
            return Array.Empty<Vector3>();

        List<Vector3> positions = new();
        if (bUseCenter)
        {
            positions.Add(startPosition);
        }

        for (int i = 0; i < positionCountArray.Length; ++i)
        {
            Vector3[] positionRange = GetTargetPositionsAround(startPosition, distanceArray[i], positionCountArray[i]);
            positions.AddRange(positionRange);
        }
        for (int i = 0; i < positions.Count; i++)
        {
            if (TryGetValidNavMeshPosition(positions[i], 5f, out Vector3 resultPos))
            {
                positions[i] = resultPos;
            }
        }


        return positions.ToArray();
    }

    public static int GetAssignedNextIndex()
    {
        int index = 0;
        for (int i = 0; i < _assignedPosIndices.Count + 1; i++)
        {
            if (!_assignedPosIndices.Contains(i))
            {
                return i;
            }
        }
        return index;
    }

    private static Vector3[] positions = { };
    private static int _maxCharacterCount = 100;
    public static void AssignTargetPosition(GameObject unit, Vector3 startPosition, float radiusFromCenter, float distanceFromUnit, int firstRingCount)
    {
        if (!IsContainTargetPos(unit))
        {
            int positionIndex = GetAssignedNextIndex();

            if (positions.Length <= 0 || positionIndex > positions.Length)
            {
                if (positionIndex > positions.Length)
                {
                    ++_maxCharacterCount;
                }
                float[] distanceArray = DistanceArrayByCharacterCount(_maxCharacterCount, distanceFromUnit, radiusFromCenter, firstRingCount);
                int[] positionCountArray = GetPositionCountArray(_maxCharacterCount, firstRingCount);
                positions = GetTargetPositionsAround(startPosition, distanceArray, positionCountArray, bUseCenter: false);
            }

            if (positionIndex < positions.Length)
            {
                _dicAssignedPos.Add(unit.GetInstanceID(), new SurrondPos(positions[positionIndex], positionIndex));
                _assignedPosIndices.Add(positionIndex);
                Debug.Log($"등록: {unit.name} | Index: {positionIndex} | Pos: {positions[positionIndex]}");
            }
        }
    }

    public static void ReleaseTargetPosition(GameObject unit)
    {
        int id = unit.GetInstanceID();
        if (_dicAssignedPos.TryGetValue(id, out SurrondPos data))
        {
            _assignedPosIndices.Remove(data._positionIndex);
            _dicAssignedPos.Remove(id);
            Debug.Log($"등록해제: {unit.name} | Index: {data._positionIndex}");
        }
    }

    public static bool TryGetAssignedTargetPositionAround(GameObject unit, out Vector3 assigendPos)
    {
        if (_dicAssignedPos.TryGetValue(unit.GetInstanceID(), out SurrondPos surrondPos))
        {
            assigendPos = surrondPos._position;
            return true;
        }
        assigendPos = default;
        return false;
    }

    public static bool IsContainTargetPos(GameObject unit)
    {
        return _dicAssignedPos.ContainsKey(unit.GetInstanceID());
    }

    private static Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, angle, 0) * vec;
    }

    public static bool IsPositionWalkable(Vector3 pos, float obstacleCheckRadius, LayerMask obstacleMask)
    {
        bool blocked = Physics.CheckSphere(pos, obstacleCheckRadius, obstacleMask);
        return !blocked;
    }
    public static bool TryGetValidNavMeshPosition(Vector3 targetPos, float validRadius, out Vector3 resultPos)
    {
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, validRadius, NavMesh.AllAreas))
        {
            resultPos = hit.position;
            return true;
        }
        resultPos = targetPos;
        return false;
    }
}