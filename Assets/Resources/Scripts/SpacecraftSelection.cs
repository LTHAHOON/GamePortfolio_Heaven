using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

public class SpacecraftSelection : Selection<SpacecraftController>
{
    [SerializeField]
    private LayerMask _clickColliderLayer;
    [SerializeField]
    private float _elapsedTime = 3;
    private SpacecraftController _target;
    private CreatureSelection _creatureSelection;
    public void Start()
    {
        SelectionManager.Instance.TryGetSelection(out _creatureSelection);
    }
    public void Update()
    {
        if (_isSelected && _creatureSelection.IsSelected)
        {
            ProcessOnSelected();
        }
    }
    public override void AddToSelectedList(ISelectableOwner selectedTarget)
    {
        ClearSelectedList();
        base.AddToSelectedList(selectedTarget);
    }
    private Coroutine _boadingCoroutine;
    public void ProcessOnSelected()
    {
        Vector3? worldMousePos = InputManager.Instance.GetWolrdMousePosByRaycast(Camera.main, _clickColliderLayer);
        if (worldMousePos.HasValue)
        {
            if(!_target)
            {
                if (InputManager.Instance.TrySelectUnitBySphereCast(KeyCode.F, Camera.main, 1.2f,
                                                                            _clickColliderLayer, UnitType.Spacecraft, out GameObject target, true))
                {
                    Debug.Log("Click");
                    if (!target.TryGetComponent(out SpacecraftController spacecraftController))
                        return;
                    _target = spacecraftController;
                    _boadingCoroutine = StartCoroutine(_target.IEBoading(_elapsedTime));
                    Debug.Log(target);
                }
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                StopCoroutine(_boadingCoroutine);
                _target = null;
                _isSelected = false;
                Debug.Log("快林急 鸥百 秒家");
            }
        }
    }
}