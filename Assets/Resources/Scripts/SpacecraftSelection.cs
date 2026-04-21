using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpacecraftSelection : MonoBehaviour
{
    [SerializeField]
    private LayerMask _targetLayerMask;
    [SerializeField]
    private float _elapsedTime = 3;
    private GameObject _target;
    private void Update()
    {
        if (CreatureControl._isSelect && !MiniMapController.IsPointerOverMiniMap && !CreateCountController.IsActive())
        {
            Vector3? worldMousePos = InputManager.Instance.GetWolrdMousePosByRaycast(Camera.main, _targetLayerMask);
            if (worldMousePos.HasValue)
            {
                if (InputManager.Instance.TrySelectUnitBySphereCast(KeyCode.F, Camera.main, 
                                                                            _targetLayerMask,UnitType.Spacecraft ,out GameObject target, true))
                {
                    StartCoroutine(IEBoading(_elapsedTime));
                    _target = target;
                    Debug.Log(target);
                }
                else if(Input.GetKeyUp(KeyCode.F))
                {
                    StopCoroutine(IEBoading(_elapsedTime));
                    Debug.Log("快林急 鸥百 秒家");
                }
            }
        }
    }

    private IEnumerator IEBoading(float elapsedTime)
    {
        yield return new WaitForSeconds(elapsedTime);
        Boarding();
    }
    private void Boarding()
    {
        List<CreatureFSM> creatureFSMList = CreatureSelection.GetSelectionCharacters<CreatureFSM>();
        for (int i = 0; i < creatureFSMList.Count; i++)
        {
            if(NavMesh.SamplePosition(_target.transform.position, out NavMeshHit hit, 50f, NavMesh.AllAreas))
            {
                creatureFSMList[i].TargetPosition = hit.position;
                creatureFSMList[i].SetIsAttackMode(true);
                creatureFSMList[i].SetCreatureState(CreatureFSM.CreatureState.Boarding);
            }
        }
        CreatureSelection.ClearSelectedCreatures();
    }
}
