using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoorController : MonoBehaviour
{
    [SerializeField]
    private AutoDoorTriggerBox _triggerBox;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private string _isCloseParm = "IsClose";
    private int _isCloseHash;
    private void Awake()
    {
        _isCloseHash = Animator.StringToHash(_isCloseParm);
        _animator.SetBool(_isCloseHash, true);
        _triggerBox.OnAutoDoorTrigger = TriggerAutoDoor;
    }

    public void TriggerAutoDoor(bool isClose)
    {
        if (isClose != _animator.GetBool(_isCloseHash))
        {
            _animator.SetBool(_isCloseHash, isClose);
        }
    }
}
