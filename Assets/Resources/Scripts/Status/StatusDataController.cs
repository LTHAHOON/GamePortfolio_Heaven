using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public class StatusDataController : MonoBehaviour
{
    private string _unitName;
    [HideInInspector]
    public UnitChipState _selectedUnitButton;
    [SerializeField]
    private TextMeshProUGUI _nameText;

    public static StatusComponent GetSelectedStatusComponent()
    {
        return _selectedStatusComponent;
    }

    private static StatusComponent _selectedStatusComponent;
    public void OnClickUnitButton(StatusComponent statusComponent)
    {
        _unitName = _nameText.text;
        _selectedStatusComponent = statusComponent;
        UnitData unitData = _selectedStatusComponent.GetUnitData();
        List<UnitChipState> addedUnitChipList = Unit_MyRoomMng.Instance.FindAddedUnitChipWithType(unitData.Type);
        for (int i = 0; i < addedUnitChipList.Count; i++)
        {
            if (addedUnitChipList[i].name == transform.parent.name)
            {
                _selectedUnitButton = addedUnitChipList[i]; 
                break;
            }
        }

        //TODO: StatusData 추가하기
        bool isAdded = StatusDataMng.AddStatusData(_selectedUnitButton.gameObject,statusComponent);
        //TODO: 새롭게 추가될 경우(존재하지 않았을 경우) Status 초기 설정하기 
        if (isAdded) 
        { 
           // InitialStatus(_selectedUnitButton.tag);
           
        }
        //TODO: Status만큼 Slider 설정하기
        StatusSliderController.SetStatusSliders(_selectedUnitButton.gameObject);
        //TODO: SelectedUnitImage 설정하기 
        SelectedUnitPopController.Instance.SetSelectedUnit(_unitName, _selectedUnitButton.gameObject, statusComponent.GetUnitData());
        //TODO: SelectedUnitPop 보이게 하기
        SelectedUnitPopController.Instance.ShowSeletedUnitPop(unitData.Type);
    }
}
