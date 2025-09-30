using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D crosshairTexture; // Assign your crosshair texture here
    public Vector2 hotspot = Vector2.zero; // Adjust the hotspot if needed

    void Start()
    {
        Cursor.SetCursor(crosshairTexture, hotspot, CursorMode.Auto);
    }
}