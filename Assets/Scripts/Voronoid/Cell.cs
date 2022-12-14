using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Cell 
{
    public static uint num = 0;

    [ReadOnly] public string name;

    public Site Site;
    public List<Edge> edges = new List<Edge>();
    public List<Vertice> vertices = new List<Vertice>();
    public bool Inside(Vector3 objectToCompare)
    {
        for (int i = 0; i < edges.Count; i++)
        {
            float slope = LinearFunction.GetSlope(edges[i].Origin.pos.pos, edges[i].Destination.pos.pos);
            Vector3 midPoint = LinearFunction.GetMidPoint(edges[i].Origin.pos.pos, edges[i].Destination.pos.pos);
            Vector3 positivemidPoint;
            Vector3 negativemidPoint;
            if (float.IsNaN(slope)|| slope==0)
            {
                positivemidPoint = midPoint + Vector3.up;
                negativemidPoint = midPoint - Vector3.up;

                if (objectToCompare.y > midPoint.y && Site.pos.pos.y > midPoint.y)
                    continue;
                if (objectToCompare.y < midPoint.y && Site.pos.pos.y < midPoint.y)
                    continue;

            }
            else if (float.IsInfinity(slope))
            {
                positivemidPoint = midPoint + Vector3.right;
                negativemidPoint = midPoint - Vector3.right;

                if (objectToCompare.x > midPoint.x && Site.pos.pos.x > midPoint.x)
                    continue;
                if (objectToCompare.x < midPoint.x && Site.pos.pos.x < midPoint.x)
                    continue;
            }
            else
            {
                float y = 0;
                float x = 0;
                float slopeB = -1 / slope;
                y = slopeB;
                x = 1;
                positivemidPoint = midPoint + new Vector3(x, y);
                negativemidPoint = midPoint + new Vector3(-x, -y);
            }
            if (Vector3.Distance(Site.pos.pos, positivemidPoint) > Vector3.Distance(Site.pos.pos, negativemidPoint))
            {
                if (Vector3.Distance(objectToCompare, positivemidPoint) > Vector3.Distance(objectToCompare, negativemidPoint))
                {
                    continue;
                }
                else if (Vector3.Distance(objectToCompare, positivemidPoint) < Vector3.Distance(objectToCompare, negativemidPoint))
                {
                    return false;
                }
                else //esta sobre el edge.
                {
                    continue;
                }
            }
            else if (Vector3.Distance(Site.pos.pos, positivemidPoint) < Vector3.Distance(Site.pos.pos, negativemidPoint))
            {
                if (Vector3.Distance(objectToCompare, positivemidPoint) > Vector3.Distance(objectToCompare, negativemidPoint))
                {
                    return false;
                }
                else if (Vector3.Distance(objectToCompare, positivemidPoint) < Vector3.Distance(objectToCompare, negativemidPoint))
                {
                    continue;
                }
                else //esta sobre el edge.
                {
                    continue;
                }
            }
        }
        return true;
    }

    public void ResetVertices (List<Vertice> vertices)
    {
        vertices.Clear();
        this.vertices = vertices;
    }
    public Cell()
    {
        num++;
        name = "Cel:" + num.ToString();
    }
}
