using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedUnitPopController : Singleton<SelectedUnitPopController>
{
    [SerializeField]
    private CreateButtonController _createButtonController;
    [SerializeField]
    private AttackButtonController _attackButtonController;
    [SerializeField]
    private DefenseButtonController _defendButtonController;
    [SerializeField]
    private HomeDecriptionMng _homeDecriptionMng;
    [SerializeField]
    private GameObject _statusPop;
    [SerializeField]
    private GameObject _homeStatusPop;
    
    [SerializeField]
    private GameObject _selectedUnitPop;
    private List<GameObject> _selectedUnitPopChildList = new();
    [SerializeField]
    private Image _selectedUnitImage;
    [SerializeField]
    private Image _selectedUnitPropertyImage;
    [SerializeField]
    private TextMeshProUGUI _selectedUnitNameText;

    [HideInInspector]
    public GameObject _selectedUnitButton;
    private void Awake()
    {
        for (int i = 0; i < _selectedUnitPop.transform.childCount; i++)
        {
            _selectedUnitPopChildList.Add(_selectedUnitPop.transform.GetChild(i).gameObject);
        }
        _selectedUnitPop.SetActive(false);
    }


    public void SetSelectedUnit(string unitName, GameObject selectedUnitButton, UnitData selectedUnitData)
    {
        Sprite unitSprite = SpriteController.GetUnitSprite(unitName);
        _selectedUnitImage.sprite = unitSprite;
        Sprite unitPropertySprite = SpriteController.GetUnitPropertySprite(selectedUnitData.Property);
        _selectedUnitPropertyImage.sprite = unitPropertySprite;
        _selectedUnitButton = selectedUnitButton;
        _selectedUnitNameText.text = unitName;
        _selectedUnitImage.rectTransform.sizeDelta = new Vector2(_selectedUnitImage.sprite.bounds.size.x + 225f, _selectedUnitImage.sprite.bounds.size.y + 250f);
    }


    public void ShowSeletedUnitPop(UnitType unitType)
    {
        SetActiveOfSelectedUnitPop(false);

        if (unitType == UnitType.Creature)
        {
            for (int i = 0; i < _selectedUnitPopChildList.Count; i++)
            {
                if (_createButtonController.gameObject == _selectedUnitPopChildList[i])
                {
                    _selectedUnitPopChildList[i].SetActive(false);
                }

                else if (_homeStatusPop == _selectedUnitPopChildList[i])
                {
                     _selectedUnitPopChildList[i].SetActive(false);
                }
                else if (_statusPop == _selectedUnitPopChildList[i])
                {
                    _selectedUnitPopChildList[i].SetActive(true);
                }
                else
                {
                    _selectedUnitPopChildList[i].SetActive(true);
                }
            }
        }
        else
        {
            for (int i = 0; i < _selectedUnitPopChildList.Count; i++)
            {
                if (_attackButtonController.gameObject == _selectedUnitPopChildList[i])
                {
                    _selectedUnitPopChildList[i].SetActive(false);
                }
                else if (_defendButtonController.gameObject == _selectedUnitPopChildList[i])
                {
                    _selectedUnitPopChildList[i].SetActive(false);
                }
                else
                {
                    _selectedUnitPopChildList[i].SetActive(true);
                }
            }

            if (unitType == UnitType.Home)
            {
                _homeDecriptionMng.SetHomeDecription(_selectedUnitButton);
                _homeStatusPop.SetActive(true);
                _statusPop.SetActive(false);
            }                                                       
            else
            {
                _homeStatusPop.SetActive(false);
                _statusPop.SetActive(true);
            }
        }
        ModeButton.SetUnitPrefab(true, unitType);
        SetActiveOfSelectedUnitPop(true);
    }

    public void SetActiveOfSelectedUnitPop(bool active)
    {
        _selectedUnitPop.SetActive(active);
    }
}
