using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStrategy
{
    //OpenData, ReadyPrefab
    void OnEnter();
    //Choose
    void OnUpdate();
    //CreatePrefab
    void OnExecute();
    //CloseData
    void OnExit(bool bExitCompletely);
}
