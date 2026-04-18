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
        Vector3 slopeNormal = CalculateGroundNormal();
        // 경사면의 방향 벡터 추출
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, slopeNormal);
        transform.rotation =  targetRotation;
    }

    private Vector3 CalculateGroundNormal()
    {
        Vector3 raycastStart = _characterController.transform.position + Vector3.up * _characterController.skinWidth;
        Vector3 raycastDirection = Vector3.down;
        if (Physics.Raycast(raycastStart, raycastDirection, out RaycastHit hit))
        {
            // 레이캐스트로 부터 얻은 경사면의 법선 벡터
            Vector3 slopeNormal = hit.normal;

            return slopeNormal;
        }
        return Vector3.up;
    }

    private float _degree = 360;
    public void ChangeHP(float curHP, float maxHP = 1)
    {
        float changedHP = (_degree - (curHP / maxHP) * _degree); //a:b = c:d -> a*d = b*c
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
