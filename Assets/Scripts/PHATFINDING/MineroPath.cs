using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MineroPath : MonoBehaviour
{
    [SerializeField] private List<Vector2Int> path = new List<Vector2Int>();

    [SerializeField] float radiusDestination = 0.9f;
    [SerializeField] public bool firstCall = true;

    int pathIndex=1;
    /// <summary>
    /// llamar una vez.
    /// </summary>
    private void UpdatePath(Vector3 saliendoDe, Vector3 llendoA)
    {
        Vector2Int basePos = new Vector2Int((int)saliendoDe.x, (int)saliendoDe.y);
        Vector2Int resutPos = new Vector2Int((int)llendoA.x, (int)llendoA.y);

        path = NodeGenerator.Get().GetShortestPath(basePos, resutPos);
        transform.position = saliendoDe;
    }

    public void CallPath(float speed, Vector3 saliendoDe, Vector3 llendoA, Action onEnd)
    {
        if (firstCall)
        {
            UpdatePath(saliendoDe, llendoA);
            firstCall = false;
            pathIndex = 1;
        }
        goingTo(onEnd, llendoA,speed);
    }


    /// <summary>
    /// llamar en update.
    /// </summary>
    void goingTo(Action end, Vector3 puntoDeLLegada, float speed)
    {
        if (Vector3.Distance(transform.position, puntoDeLLegada) > radiusDestination && pathIndex < path.Count)
        {
            if ((Vector3.Distance(transform.position, new Vector3(path[pathIndex].x, path[pathIndex].y)) <= 0.1f))
            {
                pathIndex++;
                
            }
            else
            {

                    Vector3 dir = (new Vector3(path[pathIndex].x, path[pathIndex].y) - transform.position);
                    dir.Normalize();
                    Vector3 movement = speed * dir * Time.deltaTime * Level.GetSpeedInTerrain(transform.position);
                    transform.position += new Vector3(movement.x, movement.y);
                    
            }
        }
        else
        {
            pathIndex = 1;
            path.Reverse();
            firstCall = true;
            end?.Invoke();
        }
    }


    private void OnDrawGizmos()
    {
        if (path != null)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(new Vector3(path[i].x, path[i].y,0), new Vector3(path[i + 1].x, path[i + 1].y,0));
            }
        }
    }
}
