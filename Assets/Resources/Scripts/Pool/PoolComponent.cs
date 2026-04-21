using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolComponent 
{
    private int _initialPoolSize = 5;
    private int _maxPoolSize = 10;
    private GameObject _poolObjPrefab;
    private Transform _poolParent;
    private Stack<GameObject> _poolStack = new();
    public PoolComponent(GameObject poolObjPrefab, Transform parent = null, int initalPoolSize = 5, int maxPoolSize = 10)
    {
        _poolObjPrefab = poolObjPrefab;
        _initialPoolSize = initalPoolSize;
        _maxPoolSize = maxPoolSize;
        _poolParent = parent;
        if (parent != null )
        {
            SetUpPool();
        }
    }

    private void SetUpPool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            GameObject poolObject = UnityEngine.Object.Instantiate(_poolObjPrefab, _poolParent);
            poolObject.SetActive(false);
            _poolStack.Push(poolObject);
        }
    }

    public GameObject PopPoolObject()
    {
        if(_poolStack.Count <= 0)
        {
            GameObject newPoolObject = UnityEngine.Object.Instantiate(_poolObjPrefab, _poolParent);
            return newPoolObject;
        }
        GameObject poolObject = _poolStack.Pop();
        poolObject.SetActive(true);
        return poolObject;
    }

    public void ReturnPoolObject(GameObject poolObject, float delayTime)
    {
        if (poolObject == null) return;
        if (delayTime > 0)
        {
            PoolManager.Instance.DelayReturnPoolObject(ReturnPoolObject, poolObject, delayTime);
        }
        else
        {
            ReturnPoolObject(poolObject);
        }
    }
    public void ReturnPoolObject(GameObject poolObject)
    {
        if (poolObject == null) return;
        if (_poolStack.Count < _maxPoolSize)
        {
            poolObject.SetActive(false);
            _poolStack.Push(poolObject);
        }
        else
        {
            UnityEngine.Object.Destroy(poolObject);
        }
    }


}
