using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Diciembre
{
    public class MineroPath
    {
        #region PRIVATE_FIELDS
        private List<Vector2Int> path = new List<Vector2Int>();
        private float radiusDestination = 0.9f;
        public bool firstCall = true;
        private int pathIndex = 1;
        private Transform agentTr = null;
        private Flocking floking;
        #endregion

        #region PRIVATE_METHODS
        private void CalculatePath(Vector3 saliendoDe, Vector3 llendoA)
        {
            Vector2Int basePos = new Vector2Int((int)saliendoDe.x, (int)saliendoDe.y);
            Vector2Int resutPos = new Vector2Int((int)llendoA.x, (int)llendoA.y);

            path = Main.Get().GetShortestPath(basePos, resutPos);
            agentTr.position = saliendoDe;
        }
        private void UpdatePath(Action end, Vector3 puntoDeLLegada, float speed)
        {
            if (Vector3.Distance(agentTr.position, puntoDeLLegada) > radiusDestination && pathIndex < path.Count)
            {
                if ((Vector3.Distance(agentTr.position, new Vector3(path[pathIndex].x, path[pathIndex].y)) <= 0.1f))
                    pathIndex++;
                else
                {
                    Vector3 dir = (new Vector3(path[pathIndex].x, path[pathIndex].y) - agentTr.position);
                    dir.Normalize();
                    floking.SetTarget(new Vector3(Mathf.RoundToInt(agentTr.position.x), Mathf.RoundToInt(agentTr.position.y),0)  + new Vector3(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y),0));
                    //Vector3 movement = speed * dir * Time.deltaTime * Level.GetSpeedInTerrain(agentTr.position);
                    //agentTr.position += new Vector3(movement.x, movement.y);
                }
            }
            else
            {   //reiniciar el path.
                pathIndex = 1;
                path.Reverse();
                firstCall = true;
                floking.StopMoving();
                end?.Invoke();
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public MineroPath(float radiusDestination, Transform agentTr,Flocking agentFlocking)
        {
            this.radiusDestination = radiusDestination;
            this.agentTr = agentTr;
            this.floking = agentFlocking;
        }

        public void CallPath(float speed, Vector3 saliendoDe, Vector3 llendoA, Action onEnd)
        {
            if (firstCall)
            {
                CalculatePath(saliendoDe, llendoA);
                firstCall = false;
                pathIndex = 1;
            }
            UpdatePath(onEnd, llendoA, speed);
        }

        public void OnDrawGizmos()
        {
            if (path == null)
                return;
            Gizmos.color = Color.green;

            for (int i = 0; i < path.Count - 1; i++)
                Gizmos.DrawLine(new Vector3(path[i].x, path[i].y, 0), new Vector3(path[i + 1].x, path[i + 1].y, 0));
            
        }
        #endregion
    }
}
