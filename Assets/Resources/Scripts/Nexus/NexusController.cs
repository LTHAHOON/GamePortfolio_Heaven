using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NexusController : Unit
{
    [SerializeField]
    private Faction _faction;
    [SerializeField]
    private Transform _nexusTransform;
    [SerializeField]
    private SurroundPosStatData _surroundPosStatData;
    private SurroundPosGroup _surroundPosGroup;
    protected override void Awake()
    {
        base.Awake();
        StatusManager.Instance.TryAddStatusData(UnitInfo);
        _surroundPosGroup = SurroundPosManager.AssignCenterTargetPosition(gameObject, _nexusTransform.transform.position, _surroundPosStatData._radiusFromCenter, _surroundPosStatData._distanceFromUnit,
            _surroundPosStatData._firstRingCount, false);
    }
    private void Start()
    {
        SetUpUnit();
    }
    public SurroundPosGroup NexusSurroundPosGroup => _surroundPosGroup;
    public Transform NexusTransform => _nexusTransform;
    public Faction faction => _faction;
}
