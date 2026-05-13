using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayerManager : Singleton<GameLayerManager>
{
    [Header("상대 행성 밖(우주) 레이어")]
    public LayerMask _enemyOutPlanetLayer;
    [Header("행성 밖(우주) 레이어")]
    public LayerMask _outPlanetLayer;
    [Header("상대 생물체 레이어")]
    public LayerMask _enemyCreatureLayer;
    [Header("본인 생물체 레이어")]
    public LayerMask _creatureLayer;
}

public static class GameLayerMask
{
    public static int EnemyOutPlanetLayerMask => GameLayerManager.Instance._enemyOutPlanetLayer;
    public static int EnemyCreatureLayerMask => GameLayerManager.Instance._enemyCreatureLayer;
}
public static class GameLayer
{
    private static int _outPlanetLayerCache = -1;
    private static int _creatureLayerCache = -1;
    public static int OutPlanetLayer => _outPlanetLayerCache.GetLayer(GameLayerManager.Instance._outPlanetLayer);
    public static int CreatureLayer => _creatureLayerCache.GetLayer(GameLayerManager.Instance._creatureLayer);
    public static int GetLayer(this ref int layerCache, LayerMask layerMask)
    {
        if(layerCache < 0)
        {
            layerCache = (int)Mathf.Log(layerMask.value, 2);
        }
        return layerCache;
    }
}
