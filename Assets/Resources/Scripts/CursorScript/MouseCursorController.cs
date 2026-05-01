using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseCursorController : MonoBehaviour
{
    [SerializeField]
    private LayerMask _groundLayer;
    private CursorMode _cursorMode = CursorMode.ForceSoftware;
    private Vector2 _cursorHotspot = Vector2.zero;

    public void SetCursor(MouseCursorData cursorInstance)
    {
        SpriteRenderer spriteRenderer = cursorInstance.GetFollwingSpriteRenderer();
        if(spriteRenderer)
        {
            spriteRenderer.enabled = true;
        }
        
        var cursor = cursorInstance.GetCursor();
        Cursor.SetCursor(cursor, _cursorHotspot, _cursorMode);
    }


    public void ShowCursor(bool visible)
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = visible;
    }

    public void SpriteFollowMouse(CursorContraintPos cursorContraintPos, SpriteRenderer spriteRenderer, Camera camera)
    {
        Vector3 mousePosition = Input.mousePosition;

        if (mousePosition.x >= cursorContraintPos._maxX || mousePosition.x <= cursorContraintPos._minX 
        || mousePosition.y >= cursorContraintPos._maxY || mousePosition.y <= cursorContraintPos._minY)
        {
            spriteRenderer.enabled = false;
            ShowCursor(true);
        }
        else
        {
            spriteRenderer.enabled = true;
            ShowCursor(false);
        }

        mousePosition.z = camera.farClipPlane;
        Ray ray = camera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, mousePosition.z, _groundLayer))
        {
            Debug.DrawRay(camera.transform.position, ray.direction * mousePosition.z, Color.red);
            Vector3 point = hit.point;

            point.y += 2f;
            spriteRenderer.transform.position = point;
        }
    }

}
