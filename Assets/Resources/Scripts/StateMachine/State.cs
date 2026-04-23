using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class State<TEnumState, TOwner>
{
    //활성 시 Enter를 들어가지 못하게 합니다.
    public bool _bOnceEnter = false;
    public virtual TEnumState EState { get; }

    public virtual void InitState(StateMachine<TEnumState, TOwner> stateMachine) { }
    //자동 함수구현 할 수 있게끔 abstract로 만들어줍니다.
    public abstract void EnterState(StateMachine<TEnumState, TOwner> stateMachine);
    public abstract void UpdateState(StateMachine<TEnumState, TOwner> stateMachine);
    public abstract void ExitState(StateMachine<TEnumState, TOwner> stateMachine);
}
