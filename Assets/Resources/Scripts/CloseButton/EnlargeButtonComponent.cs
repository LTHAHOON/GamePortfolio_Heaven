using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Transform))]
public class EnlargeButtonComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 baseScale;
    private void Awake()
    {
        baseScale = GetComponent<Transform>().localScale;
        baseScale.z = 1f;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
    }
    public void OnDisable()
    {
        transform.localScale = baseScale;
        isPointerOver = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
    }

    bool isPointerOver = false;
    private void Update()
    {
        if (isPointerOver)
        {
            UIManager.Instance.OnPointerEnterScaleUp(transform, baseScale);
        }
        else
        {
            UIManager.Instance.OnPointerExitScaleDown(transform, baseScale);
        }
    }


}
