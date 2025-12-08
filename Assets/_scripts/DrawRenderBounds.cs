using UnityEngine;

public class DrawRendererBounds : MonoBehaviour
{
    public Color gizmoColor = Color.blue; // Customizable color for the bounding box

    void OnDrawGizmosSelected()
    {
        Renderer r = GetComponent<Renderer>();
        if (r == null)
        {
            return; // No renderer found, nothing to draw
        }

        Bounds bounds = r.bounds; // Get the world-space bounds of the renderer

        Gizmos.matrix = Matrix4x4.identity; // Ensure Gizmos are drawn in world space
        Gizmos.color = gizmoColor; // Set the desired color

        // Draw a wireframe cube representing the bounds
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}