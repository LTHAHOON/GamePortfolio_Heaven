using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [HideInInspector]
    public DragSelectable _dragSelectable;
    [HideInInspector]
    public Selectable _selectable;

    protected virtual void Awake()
    {
        #region Selectable 컴포넌트 할당(없으면 NULL)
        TryGetComponent(out _dragSelectable);
        TryGetComponent(out _selectable);
        #endregion
    }
}
