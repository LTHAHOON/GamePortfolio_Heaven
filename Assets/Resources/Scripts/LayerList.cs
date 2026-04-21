using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LayerList
{
    [SerializeField]
    private List<GameObject> _layerList = new();
    private int _curLayer = -1;
    public void SetLayerList(GameObject callerObj, int layer)
    {
        if (_layerList.Count <= 0)
        {
            _layerList.Add(callerObj);
            return;
        }

        for (int i = 0; i < _layerList.Count; i++)
        {
            _layerList[i].layer = layer;
        }
    }

    public void SetLayerList(GameObject callerObj, bool bAddCallerObjToList,int layer)
    {
        if(!_layerList.Contains(callerObj) && bAddCallerObjToList)
        {
            _layerList.Add(callerObj);
        }
        SetLayerList(callerObj, layer);
    }

    public int GetCurrentLayer()
    {
        if (_curLayer < 0)
        {
            Debug.LogError("Error: Layer < 0");
        }
        return _curLayer;
    }

}
