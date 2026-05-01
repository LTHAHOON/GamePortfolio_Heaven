using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NexusController : Unit
{
    protected void Awake()
    {
        base.Awake();
        StatusComponent.InitializeStatus(UnitInfo);
        StatusDataMng.Instance.AddStatusData(UnitInfo.ID, StatusComponent);
    }
    private void Start()
    {
        SetUp();
    }
}
