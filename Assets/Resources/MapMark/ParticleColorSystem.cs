using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColorSystem : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] _ps;

    public void ChangeParticleColor(Color color)
    {
        for (int i = 0; i < _ps.Length; i++)
        {
            var main = _ps[i].main;
            main.startColor = color;
        }
    }
}
