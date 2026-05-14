using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public enum LadingPointType
{
    LandingForward,
    LandingBackward,
    LandingLeft,
    LandingRight
}

public class LandingPointData
{
    public Transform Data { get; set; }
    public LandingPointDatasController Owner { get; set; }
    
    public LandingPointData(Transform data, LandingPointDatasController owner)
    {
        Data = data;
        Owner = owner;
    }
}
public class LandingPointDatasController : MonoBehaviour
{
    [SerializeField]
    private Transform[] _landingPoints;
    [SerializeField]
    private LadingPointType _landingPointType;
    private readonly Dictionary<LandingPointData, bool> _dicLandingPointDatas = new();
    private LandingPointData[] _unUsedLandingPointDatas;
    private void Awake()
    {
        InitLadingPointDatas();
    }
    public LadingPointType GetRespawnPositionType()
    {
        return _landingPointType;
    }

    public LandingPointData GetRandomRespawnData()
    {
        _unUsedLandingPointDatas = GetUnUsedRespawnDatas();
        if (_unUsedLandingPointDatas.Length > 0)
        {
            int randomIndex = Random.Range(0, _unUsedLandingPointDatas.Length);
            LandingPointData ladingPosData = _unUsedLandingPointDatas[randomIndex];
            _dicLandingPointDatas[ladingPosData] = true;
            return ladingPosData;
        }
        return null;
    }
    public void ReturnLandingPosition(LandingPointData respawnPosition)
    {
        if (_dicLandingPointDatas.Count <= 0) return;
        if (!_dicLandingPointDatas.ContainsKey(respawnPosition))
        _dicLandingPointDatas[respawnPosition] = false;
    }

    public int GetUnUsedRespawnDatasCount()
    {
        return _unUsedLandingPointDatas.Length;
    }
    public LandingPointData[] GetUnUsedRespawnDatas()
    {
        return _dicLandingPointDatas
                .Where(pair => pair.Value == false) 
                .Select(pair => pair.Key)           
                .ToArray();                        
    }

    public void InitLadingPointDatas()
    {
        if(_landingPoints.Length <= 0)
        {
            _landingPoints = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                _landingPoints[i] = transform.GetChild(i).transform;
            }
        }
        foreach (Transform data in _landingPoints)
        {
            if (!data) return;
            _dicLandingPointDatas.Add(new LandingPointData(data, this), false);
        }
    }

    public LadingPointType RespawnPositionType => _landingPointType;
}
