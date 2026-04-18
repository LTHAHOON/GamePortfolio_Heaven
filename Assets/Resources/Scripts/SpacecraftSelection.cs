using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftSelection : MonoBehaviour
{
    [SerializeField]
    private LayerMask _targetLayerMask;
    private void Update()
    {
        if (CreatureControl._isSelect && !MiniMapController.IsPointerOverMiniMap && !CreateCountController.IsActive())
        {
            Vector3? worldMousePos = InputManager.Instance.GetWolrdMousePosByRaycast(UIManager.Instance.CurrentUICamera, _targetLayerMask);
            if (worldMousePos.HasValue)
            {
                if (InputManager.Instance.TrySelectUnitBySphereCast(KeyCode.F, UIManager.Instance.CurrentUICamera, 
                                                                            _targetLayerMask,UnitType.Spacecraft ,out GameObject target, true))
                {
                    Debug.Log(target);
                }
            }
        }
    }
}
