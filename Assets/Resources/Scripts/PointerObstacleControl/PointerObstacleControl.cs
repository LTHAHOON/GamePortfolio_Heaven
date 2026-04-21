using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IPointerObstacle : IPointerEnterHandler, IPointerExitHandler { }

public class PointerObstacleControl : MonoBehaviour, IPointerObstacle
{
    private Action _onPointerObstacleEnter;
    private Action _onPointerObstacleExit;
    private bool _bDragging = false;
    private string _obstacleTag = "ObstacleUI";

    public void SetPointerObstacle(Action enter, Action exit)
    {
        _onPointerObstacleEnter = enter;
        _onPointerObstacleExit = exit;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(_bDragging)
        {
            _onPointerObstacleExit?.Invoke();
            _bDragging = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.CompareTag(_obstacleTag))
        {
            _onPointerObstacleEnter?.Invoke();
            _bDragging = true;
        }
    }
}
