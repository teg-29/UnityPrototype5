using UnityEngine;

// Remplace le curseur par défaut par un curseur personnalisé
public class CustomCursor : MonoBehaviour
{
    public Texture2D cursor; // Image du curseur personnalisé
    public Vector2 hotspot = Vector2.zero; // Point de clic sur le curseur

    void Start()
    {
        // Applique le curseur personnalisé
        Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
    }
}