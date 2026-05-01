using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UnitButtonController : MonoBehaviour
{
    [SerializeField]
    private UnitSpawnComponent _unitSpawnComponent;
    [FormerlySerializedAs("_unitData")] [SerializeField]
    private UnitInfo unitInfo;
    private string _unitName;
    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private UnitChipState _unitChipState;
    private Button _button;

    private void Awake()
    {
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
        _unitName = _nameText.text;
        _selectedUnitPrefab = _unitSpawnComponent.GetSpawnPrefab(unitInfo);
        _selectedUnitPrefab.StatusComponent.InitializeStatus(unitInfo);
       if (!_unitChipState.GetIsAddedUnitChip())
           return;
       //TODO: StatusData 추가하기(false 반환시 이미 추가된 상태)
        StatusDataMng.Instance.AddStatusData(_selectedUnitPrefab.ID, _selectedUnitPrefab.StatusComponent);
        //TODO: Status만큼 Slider 설정하기
        StatusSliderController.SetStatusSliders(_selectedUnitPrefab.ID);
        //TODO: SelectedUnitImage 설정하기 
        SelectedUnitPopController.Instance.SetSelectedUnit(_unitName,gameObject, unitInfo);
        //TODO: SelectedUnitPop 보이게 하기
        SelectedUnitPopController.Instance.ShowSeletedUnitPop(unitInfo.Type);
    }
}
