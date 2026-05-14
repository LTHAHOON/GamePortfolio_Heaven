using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPMaterialInstance : MonoBehaviour
{
    [SerializeField]
    private GameObject _creatureHP;
    [Header("CreatureHP_Green")]
    [SerializeField]
    private GameObject _creatureCircleHP;
    [SerializeField]
    private CharacterController _characterController;

    private Material _hpMaterial;

    private void Awake()
    {
        _hpMaterial = _creatureCircleHP.GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        SetSlopeRotation();
    }

    private void SetSlopeRotation()
    {
        Vector3 raycastStart = _characterController.transform.position + Vector3.up * _characterController.skinWidth;
        Vector3 raycastDirection = Vector3.down;

        Vector3 slopeNormal = InputManager.Instance.GetNormalByRaycast(raycastStart, raycastDirection);
        // 경사면의 방향 벡터 추출
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, slopeNormal);
        transform.rotation =  targetRotation;
    }

    

    private float _degree = 360;
    public void ChangeHP(float curHP, float maxHP = 1)
    {
        float changedHP = (_degree - (curHP / maxHP) * _degree);
        Debug.Log(changedHP);
        _hpMaterial.SetFloat("_Arc2", changedHP);
    }

    public void SetHPColor(Color color)
    {
        _hpMaterial.SetColor("_Color", color);
    }

    public GameObject GetCreatureHP() => _creatureHP;
    public GameObject GetCreatureCircleHP() => _creatureCircleHP;

}
