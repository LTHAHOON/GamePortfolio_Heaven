using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PrefabType
{
    Creature,
    Spacecraft,
    Home
}

[CreateAssetMenu(menuName = "Database/PrefabDB")]
public class PrefabDatabase : ScriptableObject
{
    [SerializeField]
    private PrefabType _prefabType;

    public List<GameObject> prefabs = new();

    public PrefabType GetPrefabType()
    {
        return _prefabType;
    }

    public GameObject Get(long id)
    {
    
        for (int i = 0; i < prefabs.Count; i++)
        {
            if (prefabs[i].TryGetComponent(out StatusComponent statusComponent))
            {
                if(statusComponent.GetUnitData().ID == id)
                {
                    return prefabs[i];
                }
            }
        }

        return new GameObject();
    }
}