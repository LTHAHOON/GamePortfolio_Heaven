using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DefenseButtonController : ModeButton
{
    [Header("InstancePos(Parent)")]
    [SerializeField]
    private GameObject _myUnitPrefab;

    public override void OnEnable()
    {
        base.OnEnable();
        PlanetInternalPopController.OnDefenseModeOpen += OpenData;
        PlanetInternalPopController.OnDefenseModeClose += CloseData;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PlanetInternalPopController.OnDefenseModeOpen -= OpenData;
        PlanetInternalPopController.OnDefenseModeClose -= CloseData;
    }

    private MouseCursorData _cursorData;
    void Update()
    {
        if (_bGetUnitPrefab && _unitMPData != null)
        {
            GameManager.Instance.UpdateButtonToMPData(_unitMPData.Value, ref _thisButton, ref _buttonImage, ref _buttonText);
        }

        if (_bReadyPrefab && _unitPrefab)
        {
            _createCountController.RefreshCreateCount(_unitMPData.Value);
            if(_cursorData == null)
            {
                CursorManager.Instance.SetCursor(CursorType.Defend);
                _cursorData = CursorManager.GetCursorData(CursorType.Defend);
            }
            else
            {
                CursorManager.Instance.SpriteFollowMouse(_cursorData.GetFollwingSpriteRenderer());
            }
            CreatePrefab();
        }
    }

    private GameObject _curUnitPrefab;
    public void ReadyPrefab()
    {
        if (_selectedUnitType == UnitType.Creature)
        {
            Transform instantiateParent = GetSelectedUnitParentTransform(_selectedUnitType);
            _curUnitPrefab = Instantiate<GameObject>(_unitPrefab, instantiateParent);

            _curUnitPrefab.SetActive(false);
        }

        _bReadyPrefab = true;
    }

    private void CreatePrefab()
    {
        if (Input.GetMouseButtonDown(0) && _cursorData.GetFollwingSpriteRenderer().enabled)
        {
            if (_curUnitPrefab.TryGetComponent(out CreatureFSM fsm))
            {
                fsm.SetStatus(StatusSliderController._status);
                fsm.SetIsAttackMode(false);
                MyUnitPrefabDataControl.Instance.AddUnitPrefabToList(_selectedUnitType, fsm);
            }
            MouseCursorData data = CursorManager.GetCursorData(CursorType.Defend);
            _curUnitPrefab.transform.position = data.GetFollwingSpriteRenderer().transform.position;
            _curUnitPrefab.SetActive(true);
            MPController.Instance.UseUpMP(_unitMPData.Value.MP_ConsValue, 1); //MP�Ҹ�
            _curUnitPrefab = null;

            _createCountController.ConsumeCurCreateCount(1);
            if (_createCountController.GetCurCreateCount() >= 1)
            {
                ReadyPrefab();
            }
            else
            {
                PlanetInternalPopController.CloseMode(ModeType);
            }
        }
    }
    protected override void OpenData()
    {
        base.OpenData();
        _planetButtonController.SetToggleIsOn(0, true);
    }
    protected override void CloseData()
    {
        base.CloseData();
        _planetButtonController.SetToggleIsOn(0, false);
        _cursorData = null;
        if (_curUnitPrefab != null)
        {
            Destroy(_curUnitPrefab);
            _curUnitPrefab = null;
        }
    }

}
