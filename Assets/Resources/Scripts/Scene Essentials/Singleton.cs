using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new();

    private static bool _isQuitting = false;
    public static T Instance
    {
        get
        {
            if(_isQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance {typeof(T)} already destroyed");
                return null;
            }

            //동기화
            lock (_lock)
            {
               if(_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    //같은 T 스크립트가 여러개일 경우
                    if(FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogWarning("[Singleton] there should never be more than 1 singleton");
                        return _instance;
                    }

                    //인스펙터에 T 스크립트가 없을 경우 
                    if(_instance == null)
                    {
                        GameObject singleton = new();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = $"(singleton) {typeof(T)}";
                        
                    }
                }

                return _instance;

            }
        }
    }

    private static bool IsDontDestroyOnLoad()
    {
        if(_instance == null)
        {
            return false;
        }

        //HideFlags.DonSave : 씬 로드할때 해당 오브젝트를 저장하지 않고 독립적으로 존재한다.
        //HideFlags.HideInHierarchy : 해당 오브젝트는 하이어라키에 안보이게 된다.
        if ((_instance.gameObject.hideFlags & HideFlags.DontSave) == HideFlags.DontSave)
        {
            return true;
        }
        return false;
    }

    protected virtual void OnDestory()
    {
        if(IsDontDestroyOnLoad())
        {
            _isQuitting = true;
        }
    }
}
