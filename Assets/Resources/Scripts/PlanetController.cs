using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    [SerializeField]
    private GameObject _opponentPlanet;
    [SerializeField]
    private GameObject _myPlanet;
    void Awake()
    {
        _opponentPlanet = _opponentPlanet.transform.GetChild(0).gameObject;
        _myPlanet = _myPlanet.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        RotatePlanet();
    }

    [SerializeField]
    private float _rotateSpeed = 5f;
    private void RotatePlanet()
    {
        float yAngle = Time.deltaTime * _rotateSpeed;
        _opponentPlanet.transform.Rotate(0, yAngle, 0);
        _myPlanet.transform.Rotate(0, yAngle, 0);
    }
}
