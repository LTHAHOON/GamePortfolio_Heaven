using UnityEngine;
using UnityEngine.UI;

public class AttackSpawnTargetController : Singleton<AttackSpawnTargetController>
{
    public void OnClickAttackSpawnTargetButton(RespawnPositionsData respawnPositionsData)
    {
        Vector3? randomRespawnPosition = respawnPositionsData.GetRandomRespawnPosition();
        RespawnPositionType respawnPositionType = respawnPositionsData.GetRespawnPositionType();
        if (randomRespawnPosition.HasValue)
        {
            if(ModeButtonManager.Instance.CurStrategy is BaseDriveButtonController driveButtonController)
                driveButtonController.SetGoalProcess(randomRespawnPosition.Value, respawnPositionType);
        }
    }
}
