using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DefenseButtonController : ModeButtonController
{
    private MouseCursorData _cursorData;
    private Unit _curSpawnedUnit;

    public override void OnEnter()
    {
        base.OnEnter();
        CursorManager.Instance.SetCursor(CursorType.Defend);
        _cursorData = CursorManager.GetCursorData(CursorType.Defend);
        _planetButtonController.SetToggleIsOn(0, true);
        _curSpawnedUnit = UnitSpawnManager.Instance.Spawn(_selectedUnitPrefab);
        InitCreateCount(_curSpawnedUnit.UnitMPInitData); //MPData로 생성 카운트 세팅(MPData 필요)
        _curSpawnedUnit.gameObject.SetActive(false);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!_curSpawnedUnit || _cursorData == null)
            return;
        _createCountController.RefreshCreateCount(_curSpawnedUnit.UnitMPInitData);
        CursorManager.Instance.SpriteFollowMouse(_cursorData.GetFollwingSpriteRenderer());
        if (Input.GetMouseButtonDown(0) && _cursorData.GetFollwingSpriteRenderer().enabled)
        {
            OnExecute();
        }
    }

    public override void OnExecute()
    {
        base.OnExecute();

        MouseCursorData data = CursorManager.GetCursorData(CursorType.Defend);
        _curSpawnedUnit.transform.position = data.GetFollwingSpriteRenderer().transform.position;
        _curSpawnedUnit.gameObject.SetActive(true);
        MPDataController.Instance.UseUpMP(_curSpawnedUnit.UnitMPInitData, 1); //MPҸ

        _createCountController.ConsumeCurCreateCount(1);
        _curSpawnedUnit = UnitSpawnManager.Instance.Spawn(_curSpawnedUnit);
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
            MyUnitPrefabDataManager.Instance.RemoveUnitPrefabToList(_curSpawnedUnit.UnitType, _curSpawnedUnit);
        }
    }

    public override void RefreshModeButton()
    {
        Unit selectedUnitPrefab = UnitButtonController.GetSelectedUnitPrefab();
        if (!selectedUnitPrefab)
            return;
        MPData mpData = MPDataManager.Instance.FindUnitMPData(selectedUnitPrefab.ID);
        if (mpData == null)
            return;
        MPDataController.Instance.UpdateButtonToMPData(mpData, ref _thisButton, ref _buttonImage, ref _buttonText);
    }
}