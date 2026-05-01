using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DefenseButtonController : ModeButtonController
{
    private MouseCursorData _cursorData;
    private Transform instantiateParent;
    
    private Unit _curSpawnedUnit;
    public override void OnEnter()
    {
        base.OnEnter();
        _planetButtonController.SetToggleIsOn(0, true);
        _curSpawnedUnit = UnitSpawnManager.Instance.Spawn(_selectedUnitPrefab);
        _curSpawnedUnit.gameObject.SetActive(false);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_bGetUnit && _curSpawnedUnit)
        {
            MPDataController.Instance.UpdateButtonToMPData(_curSpawnedUnit.MPData, ref _thisButton, ref _buttonImage, ref _buttonText);
        }

        if (_bReadyUnit && _curSpawnedUnit)
        {
            _createCountController.RefreshCreateCount(_curSpawnedUnit.MPData);
            if (_cursorData == null)
            {
                CursorManager.Instance.SetCursor(CursorType.Defend);
                _cursorData = CursorManager.GetCursorData(CursorType.Defend);
            }
            else
            {
                CursorManager.Instance.SpriteFollowMouse(_cursorData.GetFollwingSpriteRenderer());
            }
            if (Input.GetMouseButtonDown(0) && _cursorData.GetFollwingSpriteRenderer().enabled)
            {
                OnExecute();
            }
        }
    }
    public override void OnExecute()
    {
        base.OnExecute();
        if (_curSpawnedUnit.TryGetComponent(out Creature creature))
        {
           // creature.SetStatus(StatusSliderController._status);
            creature.SetIsAttackMode(false);
            MyUnitPrefabDataControl.Instance.AddUnitPrefabToList(creature.UnitType, creature);
        }
        MouseCursorData data = CursorManager.GetCursorData(CursorType.Defend);
        _curSpawnedUnit.transform.position = data.GetFollwingSpriteRenderer().transform.position;
        _curSpawnedUnit.gameObject.SetActive(true);
        MPDataController.Instance.UseUpMP(_curSpawnedUnit.MPData.MP_ConsValue, 1); //MPҸ

        _createCountController.ConsumeCurCreateCount(1);
        _curSpawnedUnit = UnitSpawnManager.Instance.Spawn(_curSpawnedUnit.UnitInfo);
        _curSpawnedUnit.gameObject.SetActive(false);
        if (_createCountController.GetCurCreateCount() <= 0)
        {
            OnExit(true);
        }
    }

    public override void OnExit(bool bExitCompletely)
    {
        base.OnExit(bExitCompletely);
        _planetButtonController.SetToggleIsOn(0, false);
        _cursorData = null;
        if (_curSpawnedUnit)
        {
            Destroy(_curSpawnedUnit.gameObject);
        }
    }
}
