using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SurroundPosGroup
{
    public Dictionary<int, SurrondPos> _dicAssignedPos = new();
    public HashSet<int> _assignedPosIndices = new();
    public int _maxCount = 100;
    public Vector3[] _positions = { };
}

public struct SurrondPos
{
    public Vector3 _position;
    public int _positionIndex;

    public SurrondPos(Vector3 position, int positionIndex)
    {
        _position = position;
        _positionIndex = positionIndex;
    }
}

public class SurroundPosManager : MonoBehaviour
{
    private static Dictionary<GameObject, SurroundPosGroup> _dicSurroundPosGroups = new();
    private void OnDestroy()
    {
        _dicSurroundPosGroups.Clear();
    }

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

    public static int GetAssignedNextIndex(SurroundPosGroup group)
    {
        int index = 0;
        for (int i = 0; i < group._assignedPosIndices.Count + 1; i++)
        {
            if (!group._assignedPosIndices.Contains(i))
            {
                return i;
            }
        }
        return index;
    }


/// <summary>
/// AssignTargetPosition을 하기 전에 AssignCenterTargetPosition을 호출하여 초기 등록을 해야 합니다.
/// </summary>
    public static void AssignTargetPosition(GameObject target, SurroundPosGroup group)
    {
        if (!IsContainTargetPos(target,group))
        {
            int positionIndex  = GetAssignedNextIndex(group);
            if (positionIndex < group._positions.Length)
            {
                group._dicAssignedPos.Add(target.GetInstanceID(), new SurrondPos(group._positions[positionIndex], positionIndex));
                group._assignedPosIndices.Add(positionIndex);
                Debug.Log($"등록: {target.name} | Index: {positionIndex} | Pos: {group._positions[positionIndex]}");
            }
        }
    }
    
    //초기 등록(SurroundPosGroup(Key) 반환)
    public static SurroundPosGroup AssignCenterTargetPosition(GameObject centerTarget,Vector3 startPosition, float radiusFromCenter,
                                                                float distanceFromUnit, int firstRingCount, bool bUseCenter = false)
    {
        if (!IsCotainCenterTarget(centerTarget))
        {
            _dicSurroundPosGroups.Add(centerTarget, new SurroundPosGroup());
            SurroundPosGroup group = _dicSurroundPosGroups[centerTarget];
            int positionIndex  = GetAssignedNextIndex(group);

            if (group._positions.Length <= 0 || positionIndex > group._positions.Length)
            {
                if (positionIndex > group._positions.Length)
                {
                    ++group._maxCount;
                }
                float[] distanceArray = DistanceArrayByCharacterCount(group._maxCount, distanceFromUnit, radiusFromCenter, firstRingCount);
                int[] positionCountArray = GetPositionCountArray(group._maxCount, firstRingCount);
                group._positions = GetTargetPositionsAround(startPosition, distanceArray, positionCountArray, bUseCenter);
                if (bUseCenter)
                {
                    group._dicAssignedPos.Add(centerTarget.GetInstanceID(), new SurrondPos(group._positions[positionIndex], positionIndex));
                    group._assignedPosIndices.Add(positionIndex);    
                }
                return group;
            }
        }
        return _dicSurroundPosGroups[centerTarget];
    }

    public static void ReleaseTargetPosition(GameObject target, SurroundPosGroup group)
    {
        if(group == null)
            return;
        int id = target.GetInstanceID();
        if (group._dicAssignedPos.TryGetValue(id, out SurrondPos data))
        {
            group._assignedPosIndices.Remove(data._positionIndex);
            group._dicAssignedPos.Remove(id);
            Debug.Log($"등록해제: {target.name} | Index: {data._positionIndex}");
        }
    }
    public static void ReleaseSurroundPosGroup(GameObject key)
    {
        if (_dicSurroundPosGroups.ContainsKey(key))
        {
            _dicSurroundPosGroups.Remove(key);
        }
    }
    
    public static void ReleaseSurroundPosGroup(SurroundPosGroup value)
    {
        foreach (var pair in _dicSurroundPosGroups)
        {
            if(pair.Value == value)
            {
                _dicSurroundPosGroups.Remove(pair.Key);
                return;
            }
        }
    }
    
    public static bool TryGetAssignedTargetPositionAround(GameObject target, SurroundPosGroup group ,out Vector3 assigendPos)
    {
        if (group != null)
        {
            int id = target.GetInstanceID();
            if (group._dicAssignedPos.TryGetValue(id, out SurrondPos surrondPos))
            {
                assigendPos = surrondPos._position;
                return true;
            }     
        }
        assigendPos = default;
        return false;
    }

    private static bool IsCotainCenterTarget(GameObject centerTarget)
    {
        return _dicSurroundPosGroups.ContainsKey(centerTarget);
    }
    public static bool IsContainTargetPos(GameObject target, SurroundPosGroup group)
    {
        if(group == null)
            return false;
        int id = target.GetInstanceID();
        return group._dicAssignedPos.ContainsKey(id);
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