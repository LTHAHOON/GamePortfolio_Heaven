using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(UnitSpawnComponent))]
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(UnitInfo))]
[RequireComponent(typeof(UnitChipState))]
public class UnitButtonController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameText;
    private UnitSpawnComponent _unitSpawnComponent;
    private UnitInfo _unitInfo;
    private UnitChipState _unitChipState;
    private Button _button;

    private void Awake()
    {
        _unitInfo = GetComponent<UnitInfo>();
        _unitChipState = GetComponent<UnitChipState>();
        _unitSpawnComponent = GetComponent<UnitSpawnComponent>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClickUnitButton);
    }

    public static Unit GetSelectedUnitPrefab()
    {
        return _selectedUnitPrefab;
    }

    private static Unit _selectedUnitPrefab;
    public void OnClickUnitButton()
    {
        _selectedUnitPrefab = _unitSpawnComponent.GetSpawnPrefab(_unitInfo);
       if (!_unitChipState.GetIsAddedUnitChip())
           return;
        UnitSpawnManager.Instance.TryAddSpawnHeightOffset(_selectedUnitPrefab.ID, _unitSpawnComponent.SpawnHeightOffset);
        //TODO: MPData 추가하기(false 반환시 이미 추가된 상태)
        MPDataManager.Instance.TryAddMPData(_selectedUnitPrefab.ID, _selectedUnitPrefab.UnitMPInitData);
        //TODO: StatusData 추가하기(false 반환시 이미 추가된 상태)
        StatusManager.Instance.TryAddStatusData(_selectedUnitPrefab.UnitInfo);
        //TODO: StatusSliderController 객체 가져오기
        StatusSlidersController statusSlidersController = StatusManager.Instance.GetStatusSlidersController(_selectedUnitPrefab.UnitInfo.SliderType);
        //TODO: Status만큼 Slider 설정하기
        statusSlidersController.SetStatusSliders(_selectedUnitPrefab.ID);
        //TODO: SelectedUnitImage 설정하기 
        SelectedUnitPopController.Instance.SetSelectedUnit(_nameText.text,gameObject, _unitInfo);
        //TODO: SelectedUnitPop 보이게 하기
        SelectedUnitPopController.Instance.ShowSeletedUnitPop(_unitInfo.Type);
    }
}
