using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPointsController : MonoBehaviour
{
    [SerializeField]
    private ModeType _modeType;
    [SerializeField]
    private List<Button> _spawnPointButtons;
    
    public void SetUpButton()
    {
        for (int i = 0; i < _spawnPointButtons.Count; i++)
        {
            _spawnPointButtons[i].onClick.AddListener(() => OnClickSpawnPointButton(_spawnPointButtons[i]));
        }
    }
    //AttackDriveMode의 SpawnPointButton과 DefenseDriveMode의 SpawnPointButton을 토글 방식으로 활성 또는 비활성화합니다.
    private void OnClickSpawnPointButton(Button button)
    {
        button.interactable = !(button.IsInteractable());
        if (_modeType == ModeType.AttackDriveMode)
        {
            int curButtonIndex = SpawnPointsManager.Instance.FindIndexSpawnPoint(button, _modeType);
            if (curButtonIndex < 0) return;
            SpawnPointsManager.Instance.SetInteractableSpawnPointButton(!(button.interactable), curButtonIndex, ModeType.DefenseDirveMode);
        }
    }
    
    public void OnClickSpawnPointButton(RespawnPositionsData respawnPositionsData)
    {
        Vector3? randomRespawnPosition = respawnPositionsData.GetRandomRespawnPosition();
        RespawnPositionType respawnPositionType = respawnPositionsData.GetRespawnPositionType();
        if (randomRespawnPosition.HasValue)
        {
            if(ModeButtonManager.Instance.CurStrategy is BaseDriveButtonController driveButtonController)
                driveButtonController.SetGoalProcess(randomRespawnPosition.Value, respawnPositionType);
        }
    }

    public List<Button> GetSpawnPointButtons() => _spawnPointButtons;
    public ModeType ModeType => _modeType;
}
