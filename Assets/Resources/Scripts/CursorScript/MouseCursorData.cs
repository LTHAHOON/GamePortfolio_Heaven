using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



[Serializable]
public class MouseCursorData 
{
    [SerializeField]
    private Texture2D cursorTexture;
    [SerializeField]
    private SpriteRenderer followingSpriteRenderer;
    public Texture2D GetCursor()
    {
        return cursorTexture;
    }
    public SpriteRenderer GetFollwingSpriteRenderer()
    {
        return followingSpriteRenderer;
    }
}
