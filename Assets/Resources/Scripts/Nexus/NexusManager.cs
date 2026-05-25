using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NexusManager : Singleton<NexusManager>
{
    [SerializeField]
    private List<NexusController> _nexusList = new();
    private readonly Dictionary<int, NexusController> _nexusDictionary = new();
    
    private void Awake()
    {
        for (int i = 0; i < _nexusList.Count; i++)
        {
            _nexusDictionary.Add((int)_nexusList[i].faction, _nexusList[i]);
        }
    }
    
    public SurroundPosGroup GetNexusSurroundPosGroup(Faction faction)
    {
        return _nexusDictionary[(int)faction].NexusSurroundPosGroup;
    }
    
    public Vector3 GetNexusPosByFraction(Faction faction)
    {
        return _nexusDictionary[(int)faction].NexusTransform.position;
    }
}
