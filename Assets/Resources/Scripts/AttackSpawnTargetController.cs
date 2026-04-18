using UnityEngine;
using UnityEngine.UI;

public class AttackSpawnTargetController : Singleton<AttackSpawnTargetController>
{
    [SerializeField]
    private AttackButtonController _attackButtonController;


    public void OnClickAttackSpawnTargetButton(RespawnPositionsData respawnPositionsData)
    {
        Vector3? randomRespawnPosition = respawnPositionsData.GetRandomRespawnPosition();
        RespawnPositionType respawnPositionType = respawnPositionsData.GetRespawnPositionType();
        if (randomRespawnPosition.HasValue)
        {
            _attackButtonController.SetGoalProcess(randomRespawnPosition.Value, respawnPositionType);
        }
    }
}
