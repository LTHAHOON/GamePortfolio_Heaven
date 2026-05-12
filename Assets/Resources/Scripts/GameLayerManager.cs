using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayerManager : Singleton<GameLayerManager>
{
    [Header("행성 밖(우주) 레이어")]
    public LayerMask _outPlanetLayer;
    [Header("상대 레이어")]
    public LayerMask _enemyTargetLayer;
}


public static class GameLayer
{
    private static int _outPlanetLayerCache = -1;
    private static int _enemyTargetLayerCache = -1;

    public static int OutPlanetLayer => _outPlanetLayerCache.GetLayer(GameLayerManager.Instance._outPlanetLayer);
    public static int EnemyTargetLayer => _enemyTargetLayerCache.GetLayer(GameLayerManager.Instance._enemyTargetLayer);
    public static int GetLayer(this ref int layerCache, LayerMask layerMask)
    {
        if(layerCache < 0)
        {
            layerCache = (int)Mathf.Log(layerMask.value, 2);
        }
        return layerCache;
    }
}
