using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpacecraftDieState : State<SpacecraftState, SpacecraftController>
{
    private DieStatData _dieStatData;
    public override SpacecraftState EState => SpacecraftState.Die;
    private Renderer[] _renderers;
    private float _dissolveValue = 0f;
    public override void InitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        stateMachine.TryGetStateData(out _dieStatData);
    }
    

    public override void EnterState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        SpacecraftController spacecraftController = stateMachine.GetOwner();
        SpacecraftSelection.Instance.RemoveToSelectedCharacters(spacecraftController);
        _renderers = spacecraftController.GetRenderers();

        _bOnceEnter = true;
    }

    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        if(_renderers == null)
            return;
        _dissolveValue += Time.deltaTime *  (1 / _dieStatData._dieDelayTime);
        if (_dissolveValue >= 2f)
        {
            SpacecraftController spacecraftController = stateMachine.GetOwner();
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].gameObject.layer = 0;
                MPBPropertyControl.ChangeMaterialProperty(_renderers[i], _dieStatData.AlphaPropertyID, 0);
            }

            _dieStatData.PlayDieParticle(out float totalTime);
            UnitStorageManager.Instance.RemoveUnitToStorageList(spacecraftController.GetHealth().Faction, UnitType.Spacecraft, spacecraftController, totalTime);
            _renderers = null;
            return;
        }
        for (int i = 0; i < _renderers.Length; i++)
        {
            MPBPropertyControl.ChangeMaterialProperty(_renderers[i], _dieStatData.LavaDissolvePropertyID, _dissolveValue);
        }
    }

    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine) { }
}