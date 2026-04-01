using UnityEngine;

public class ArcVisualization : MonoBehaviour
{
    public Vector2[][] arcs;

    [SerializeField] private Color gizmoLineColor = Color.green; // Allow user to determine the color they want to see the arc lines

    void OnDrawGizmos()
    {
        if (arcs == null) return;

        Gizmos.color = gizmoLineColor;

        foreach (var arc in arcs)
        {
            for (int i = 0; i < arc.Length - 1; i++)
            {
                Vector3 pointA = new Vector3(arc[i].x, arc[i].y, 0);
                Vector3 pointB = new Vector3(arc[i + 1].x, arc[i + 1].y, 0);
                
               Gizmos.DrawLine(pointA, pointB);
            }
        }
    }
}
