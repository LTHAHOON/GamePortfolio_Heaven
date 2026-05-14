using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class GameLayerManager : Singleton<GameLayerManager>
{

    #region 게임 레이어 마스크 데이터
    [Serializable]
    public struct GameLayerMaskData 
    {
        [Header("Environment 레이어 마스크")]
        public LayerMask _environmentLayerMask;
        [Header("상대 행성 밖(우주) 레이어 마스크")]
        public LayerMask _enemyOutPlanetLayerMask;
        [Header("상대 생물체 레이어 마스크")]
        public LayerMask _enemyCreatureLayerMask;
        [Header("지붕 레이어 마스크")]
        public LayerMask _roofLayerMask;
    }
    #endregion

    #region 게임 레이어 데이터(하나만 선택되어야 함)
    [Serializable]
    public struct GameLayerData
    {
        [Header("본인 행성 밖(우주) 레이어")]
        public LayerMask _outPlanetLayer;
        [Header("본인 생물체 레이어")]
        public LayerMask _creatureLayer;
        [Header("지붕 레이어")]
        public LayerMask _roofLayer;
        [Header("벽 레이어")]
        public LayerMask _wallLayer;
    }
    #endregion

    [Header("게임 레이어 마스크 데이터")]
    [SerializeField]
    private GameLayerMaskData _gameLayerMaskData;
    [Header("게임 레이어 데이터(하나만 선택되어야 함)")]
    [SerializeField]
    private GameLayerData _gameLayerData;

    public GameLayerMaskData LayerMaskData => _gameLayerMaskData;
    public GameLayerData LayerData => _gameLayerData;

    public LayerMask GetExceptedLayerMask(LayerMask layerMask, int exceptedLayer)
    {
        layerMask &= ~(1 << exceptedLayer);
        return layerMask;
    }
}

public static class GameLayerMask
{
    public static LayerMask EnvironmentLayerMask => GameLayerManager.Instance.LayerMaskData._environmentLayerMask;
    public static int EnemyOutPlanetLayerMask => GameLayerManager.Instance.LayerMaskData._enemyOutPlanetLayerMask;
    public static int EnemyCreatureLayerMask => GameLayerManager.Instance.LayerMaskData._enemyCreatureLayerMask;
    public static LayerMask RoofLayerMask => GameLayerManager.Instance.LayerMaskData._roofLayerMask;
}
public static class GameLayer
{
    private static int _outPlanetLayerCache = -1;
    private static int _creatureLayerCache = -1;
    private static int _roofLayerCache = -1;
    private static int _wallLayerCache = -1;
    public static int RoofLayer => _roofLayerCache.GetLayer(GameLayerManager.Instance.LayerData._roofLayer);
    public static int WallLayer => _wallLayerCache.GetLayer(GameLayerManager.Instance.LayerData._wallLayer);
    public static int OutPlanetLayer => _outPlanetLayerCache.GetLayer(GameLayerManager.Instance.LayerData._outPlanetLayer);
    public static int CreatureLayer => _creatureLayerCache.GetLayer(GameLayerManager.Instance.LayerData._creatureLayer);
    public static int GetLayer(this ref int layerCache, LayerMask layerMask)
    {
        if(layerCache < 0)
        {
            layerCache = (int)Mathf.Log(layerMask.value, 2);
        }
        return layerCache;
    }
}
