using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseCursorController : MonoBehaviour
{

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

    public void SpriteFollowMouse(SpriteContraintPos spriteContraintPos, SpriteRenderer spriteRenderer, Camera camera)
    {
        Vector3 mousePosition = Input.mousePosition;

        if (mousePosition.x >= spriteContraintPos._maxX || mousePosition.x <= spriteContraintPos._minX 
        || mousePosition.y >= spriteContraintPos._maxY || mousePosition.y <= spriteContraintPos._minY)
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
        if (Physics.Raycast(ray, out hit,mousePosition.z))
        {
            Debug.DrawRay(camera.transform.position, ray.direction * mousePosition.z, Color.red);
            Vector3 point = hit.point;

            point.y += 2f;
            spriteRenderer.transform.position = point;
        }
    }

}
