using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RespawnPositionType
{
    RespawnForward,
    RespawnBackward,
    RespawnLeft,
    RespawnRight
}

public class RespawnPositionsData : MonoBehaviour
{
    [SerializeField]
    private Transform[] respawnPositions;
    [SerializeField]
    private RespawnPositionType respawnPositionType;
    private List<Transform> respawnPositionList = new List<Transform>();
    private void Awake()
    {
        InitRespawnPositionList();
    }
    public RespawnPositionType GetRespawnPositionType()
    {
        return respawnPositionType;
    }

    public Vector3? GetRandomRespawnPosition()
    {
        if(respawnPositionList.Count > 0)
        {
            int randomIndex = Random.Range(0, respawnPositionList.Count);
            Transform respawnPosition = respawnPositionList[randomIndex];
            respawnPositionList.RemoveAt(randomIndex);
            return respawnPosition.position;
        }
        return null;
    }


    public void InitRespawnPositionList()
    {
        foreach (Transform respawnPosition in respawnPositions)
        {
            respawnPositionList.Add(respawnPosition);
        }
    }
}
