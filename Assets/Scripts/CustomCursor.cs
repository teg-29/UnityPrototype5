using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursor;
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
    }
}
