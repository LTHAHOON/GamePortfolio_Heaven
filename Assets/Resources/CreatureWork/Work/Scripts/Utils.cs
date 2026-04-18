using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    static Texture2D _whiteTexture;
    public static Texture2D WhiteTexture()
    {
        if (_whiteTexture == null)
        {
            _whiteTexture = new Texture2D(1, 1);
            _whiteTexture.SetPixel(0, 0, Color.white);
            _whiteTexture.Apply();
        }

        return _whiteTexture;
    }

    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture());
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // 위
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // 왼쪽
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // 오른쪽
        Utils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // 아래
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }


    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // 스크린 길이를 빼서 스크린 좌표를 상단->하단 또는 하단->상단으로 변환시킴
        // (즉, 왼쪽하단(스크린 원점)을 왼쪽상단으로 생각하면 됨)
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // 꼭짓점 위치 결정
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // 두 꼭짓점을 통해 사각형(rect)를 만듬
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}
