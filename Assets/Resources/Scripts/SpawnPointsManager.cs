using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPointsManager : Singleton<SpawnPointsManager>
{
    [SerializeField] 
    private List<SpawnPointsController> _spawnPointControllers;
    private Dictionary<int, SpawnPointsController> _dicSpawnPoints = new Dictionary<int, SpawnPointsController>();
    [SerializeField] 
    private GameObject _spawnPointParent;

    private void Awake()
    {
        for (int i = 0; i < _spawnPointControllers.Count; i++)
        {
            _dicSpawnPoints.Add((int)_spawnPointControllers[i].ModeType, _spawnPointControllers[i]);
            _spawnPointControllers[i].SetUpButton();
        }
    }

    /// <summary>
    /// ModeType은 AttackDriveMode 또는 DefenseDriveMode 중 하나이어야 합니다.
    /// </summary>
    public void SetActiveSpawPointsControl(bool isActive, ModeType modeType)
    {
        _spawnPointParent.gameObject.SetActive(isActive);
        if (_dicSpawnPoints.TryGetValue((int)modeType, out SpawnPointsController spawnPointController))
        {
            spawnPointController.gameObject.SetActive(isActive);
        }
    }

    public int FindIndexSpawnPoint(Button button ,ModeType modeType)
    {
         List<Button> spawnPointButtons = _dicSpawnPoints[(int)modeType].GetSpawnPointButtons();
        for (int i = 0; i < spawnPointButtons.Count; i++)
        {
            if(spawnPointButtons[i] == button)
            {
                return i;
            }
        }
        return -1;
    }
    
    public void SetInteractableSpawnPointButton(bool isActive, int index, ModeType modeType)
    {
        if (_dicSpawnPoints.TryGetValue((int)modeType, out SpawnPointsController spawnPointController))
        {
            List<Button> spawnPointButtons =  spawnPointController.GetSpawnPointButtons();
            spawnPointButtons[index].interactable = isActive;
        }
    }
}