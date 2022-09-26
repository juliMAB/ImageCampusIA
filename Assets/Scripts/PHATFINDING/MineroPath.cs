using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MineroPath : MonoBehaviour
{
    [SerializeField] private List<Vector2Int> path = new List<Vector2Int>();
    //[SerializeField] private List<Vector2Int> pathBack = new List<Vector2Int>();

    [SerializeField] public Transform placeToGo;
    [SerializeField] public Transform placeToBack;
    [SerializeField] bool goinForward;
    [SerializeField] float radiusDestination = 0.9f;
    int pathIndex=1;


    private void Start()
    {
        UpdatePath();
    }
    private void UpdatePath()
    {
        Vector2Int basePos = new Vector2Int((int)placeToBack.position.x, (int)placeToBack.position.y);
        Vector2Int resutPos = new Vector2Int((int)placeToGo.position.x, (int)placeToGo.position.y);

        path = NodeGenerator.Get().GetShortestPath(basePos, resutPos);
        transform.position = placeToBack.position;
    }


    void Update()
    {
        if (path==null)
            return;
        if (goinForward)
            goingTo(placeToGo.position);
        else
            goingTo(placeToBack.position);
    }

    void goingTo(Vector3 onGo)
    {
        if (Vector3.Distance(transform.position, onGo) > radiusDestination && pathIndex < path.Count)
        {
            if ((Vector3.Distance(transform.position, new Vector3(path[pathIndex].x, path[pathIndex].y)) <= 0.1f))
            {
                pathIndex++;
                Debug.Log(pathIndex);
            }
            else
            {

                    Vector3 dir = (new Vector3(path[pathIndex].x, path[pathIndex].y) - transform.position);
                    dir.Normalize();
                    Vector3 movement = dir * Time.deltaTime * Level.GetSpeedInTerrain(transform.position);
                    transform.position += new Vector3(movement.x, movement.y);
                    Debug.Log("moving");
            }
        }
        else
        {
            //transform.position = onGo;
            goinForward = !goinForward;
            Debug.Log("resetPath");
            pathIndex = 1;
            path.Reverse();
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

        //if (pathBack != null)
        //{
        //    Gizmos.color = Color.red;
        //    for (int i = 0; i < pathBack.Count - 1; i++)
        //    {
        //        Gizmos.DrawLine(new Vector3(pathBack[i].x, pathBack[i].y, 0), new Vector3(pathBack[i + 1].x, pathBack[i + 1].y, 0));
        //    }
        //}
    }
}
