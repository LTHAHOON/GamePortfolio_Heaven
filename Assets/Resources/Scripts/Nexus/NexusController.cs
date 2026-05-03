using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NexusController : Unit
{
    protected override void Awake()
    {
        base.Awake();
        StatusManager.Instance.TryAddStatusData(UnitInfo);
    }
    private void Start()
    {
        SetUpUnit();
    }
}
