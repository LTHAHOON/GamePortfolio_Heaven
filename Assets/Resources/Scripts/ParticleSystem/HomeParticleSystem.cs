using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeParticleSystem : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _healingCircle_PS;
    
    public void PlayHealingCircle()
    {
        if(_healingCircle_PS.isStopped)
        {
            _healingCircle_PS.Play();
        }
    }

    public void StopHealingCircle()
    {
        if(_healingCircle_PS.isPlaying)
        {
            _healingCircle_PS.Stop();
        }
    }
}
