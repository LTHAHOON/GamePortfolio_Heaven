using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ISelectableOwner))]
public class Selectable : MonoBehaviour, ISelectable
{
    protected ISelectableOwner _owner;
    public ISelectableOwner Owner => _owner;
    private void Awake()
    {
        _owner = GetComponent<ISelectableOwner>();
    }
    public void OnSelected()
    {
        _owner?.OnSelected();
    }
    public void OnDeSelected()
    {
        _owner?.OnDeSelected();
    }

}
