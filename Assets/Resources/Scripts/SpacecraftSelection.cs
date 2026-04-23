using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

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
                    Debug.Log("우주선 타겟 취소");
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
        bool bGetChild = MyUnitPrefabDataControl.Instance.TryGetChild(out GameObject child, UnitType.Creature);
        if (!_target.TryGetComponent(out SpacecraftController spacecraftController))
            return;
        if (!bGetChild)
            return;
        List<CreatureFSM> creatureFSMList = CreatureSelection.GetSelectionCharacters<CreatureFSM>();
        Transform creatureParent = child.transform;
        NavMeshPath path = new();
        for (int i = 0; i < creatureFSMList.Count; i++)
        {
            if(NavMesh.SamplePosition(spacecraftController.transform.position, out NavMeshHit hit, 30f, NavMesh.AllAreas))
            {
                NavMesh.CalculatePath(creatureFSMList[i].transform.position, hit.position, NavMesh.AllAreas, path);
                if (path.status != NavMeshPathStatus.PathComplete)
                {
                    Debug.Log($"creatureFSMList[{i}] 길 막혔습니다");
                    continue;
                }
                creatureFSMList[i].OnDeSelected();
                creatureFSMList[i].TargetPosition = hit.position;
                creatureFSMList[i].StateMachine.ChangeState(CreatureState.Boarding);
                creatureFSMList[i].SetIsAttackMode(true);

                spacecraftController.AddPassenger(creatureFSMList[i],1 , creatureParent);
            }
        }
        CreatureSelection.ClearSelectedCreatures();
    }
}
