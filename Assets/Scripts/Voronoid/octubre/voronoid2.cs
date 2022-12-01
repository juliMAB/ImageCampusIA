using Unity.VisualScripting;
using UnityEngine;

public class voronoid2 : MonoBehaviour
{
    public float size;
    const int NUMBERS_OF_POINTS = 12;
    Vector2[] points = new Vector2[NUMBERS_OF_POINTS] {
         new Vector2(0.1f, 0.1f)
        ,new Vector2(0.2f, 0.3f)
        ,new Vector2(0.3f, 0.5f)
        ,new Vector2(0.6f, 0.4f)
        ,new Vector2(0.8f, 1.0f)
        ,new Vector2(0.9f, 0.1f)
        ,new Vector2(1.0f, 0.9f)
        ,new Vector2(0.5f, 0.2f)
        ,new Vector2(0.7f, 0.6f)
        ,new Vector2(0.4f, 0.7f)
        ,new Vector2(0.1f, 0.8f)
        ,new Vector2(0.2f, 0.3f) };


    Vector2 puntoPerteneciente(Vector2 askFor)
    {
        Vector2 uv = askFor;

        // find the closest point from the set
        int nearest = 0;
        for (int i = 0; i < NUMBERS_OF_POINTS; i++)
        {
            float d1 = Vector2.Distance(uv, points[i]);
            float d2 = Vector2.Distance(uv, points[nearest]);
            if ( d1< d2)
                nearest = i;
        }
        return points[nearest];
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < NUMBERS_OF_POINTS; i++)
        {
            Gizmos.DrawWireSphere(new Vector3(points[i].x, points[i].y, 0), size);
        }
    }
}
