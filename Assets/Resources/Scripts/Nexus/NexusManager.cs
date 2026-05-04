using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NexusManager : Singleton<NexusManager>
{
    [SerializeField]
    private List<NexusController> _nexusList = new List<NexusController>();
    private readonly Dictionary<int, NexusController> _nexusDictionary = new();
    
    private void Awake()
    {
        for (int i = 0; i < _nexusList.Count; i++)
        {
            _nexusDictionary.Add((int)_nexusList[i].Fraction, _nexusList[i]);
        }
    }
    
    public SurroundPosGroup GetNexusSurroundPosGroup(Fraction fraction)
    {
        return _nexusDictionary[(int)fraction].NexusSurroundPosGroup;
    }
    
    public NexusController GetNexusByFraction(Fraction fraction)
    {
        return _nexusDictionary[(int)fraction];
    }
    public Vector3 GetNexusPosByFraction(Fraction fraction)
    {
        return _nexusDictionary[(int)fraction].NexusTransform.position;
    }
}
