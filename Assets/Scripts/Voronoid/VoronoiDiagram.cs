using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class VoronoiDiagram : MonoBehaviour
{
    [SerializeField] List<Site> sites = new List<Site>();
    [SerializeField] List<Cell> cells = new List<Cell>();
    [SerializeField] Vector2Int LimitSize;

    [SerializeField] Transform p1;
    [SerializeField] Transform p2;


    [ReadOnly][SerializeField] RectInt limits = new RectInt();

    [SerializeField] Vector3 posToAdd;

    [SerializeField] Edge testEdge1;
    [SerializeField] Edge testEdge2;

    [SerializeField] bool testing;

    private void OnValidate()
    {
        ResizeLimits();
    }

    public void TestEdges()
    {
        Vector3 v = LinearFunction.PreguntarSiSeCortan(testEdge1, testEdge2);
        Debug.Log(v);
    }

    public void ResizeLimits()
    {
        limits = new RectInt(new Vector2Int(LimitSize.x / 2, LimitSize.y / 2), LimitSize);
        limits.xMin = 0;
        limits.yMin = 0;
        limits.size = LimitSize;
    }
    public void AddRandomSite()
    {
        Site newSite = new Site(GetRandomPointOnLimits(), UnityEngine.Random.Range(0f, 1f));

        RecalculateVoronoi(newSite);
        //sites.Add(newSite);
    }
    public void getSlope()
    {
        Debug.Log(LinearFunction.GetSlope(p1.position, p1.position));
    }
    public void AddSite()
    {
        addSite(posToAdd);
    }
    public void addSite(Vector3 pos)
    {
        Site newSite = new Site(new vec3(pos), UnityEngine.Random.Range(0f, 1f));

        RecalculateVoronoi(newSite);
        //sites.Add(newSite);
    }

    public void ClearLists()
    {
        //sites.Clear();
        cells.Clear();
        Site.num = 0;
        Edge.num = 0;
        Cell.num = 0;
        Vertice.num = 0;
    }
    public vec3 GetRandomPointOnLimits()
    {
        return new vec3(new Vector3(UnityEngine.Random.Range(limits.xMin, limits.xMax), UnityEngine.Random.Range(limits.yMin, limits.yMax)));
    }

    public LinearFunction CreateTheNewEdge(Vector3 A, Vector3 B)
    {
        Vector3 midpos = LinearFunction.GetMidPoint(A, B);
        float slope = LinearFunction.GetSlope(A, B);
        //newEdge.lf = 
        if (float.IsNaN(slope))
            slope = float.PositiveInfinity;
        else if (float.IsInfinity(slope))
            slope = float.NaN;
        else
            slope = -1 / slope;

        return new LinearFunction(slope, midpos);
    }

    public void RecalculateVoronoi(Site newSite)
    {
        
        Cell newCell = new Cell();
        newCell.Site = newSite;
        Vector3 newSitePos = newSite.pos.pos;
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].Inside(newSite.pos.pos))   
            {
                Cell currentCell = cells[i];
                Vector3 localSitePos = currentCell.Site.pos.pos;
                Edge newEdge = new Edge();

                Vertice newV1 = null;
                Vertice newV2 = null;
                newEdge.lf = CreateTheNewEdge(newSitePos, localSitePos);
                for (int j = 0; j < currentCell.edges.Count; j++)
                {
                    Edge current_Edge = currentCell.edges[j];
                    vec3 cutpointBase = new vec3 (LinearFunction.PreguntarSiSeCortan(newEdge.lf, current_Edge.lf));
                    if (newV1 != null && newV2 != null)
                        break;

                    if (!PerteneceALimites(cutpointBase.pos))
                        continue;

                    if (LinearFunction.NanOrInfin(cutpointBase.pos))
                        continue;

                    Vertice origin = current_Edge.Origin;
                    Vertice destin = current_Edge.Destination;

                    Edge localEdge = null;
                    Edge main_Edge = null;
                    Vector3 orig_pos = origin.pos.pos;
                    if (newV1 != null && newV2 == null)
                    {
                        if (LinearFunction.PreguntarSiSeCortan(cutpointBase.pos, current_Edge))
                        {
                            //preguntar a que punto esta mas cerca el segmento
                            newV2 = new Vertice(new vec3(cutpointBase.pos));
                            for (int w = 0; w < currentCell.vertices.Count; w++)
                                if (cutpointBase.pos == currentCell.vertices[w].pos.pos)
                                    if (newV2.pos.pos == currentCell.vertices[w].pos.pos)
                                        newV2 = currentCell.vertices[w];

                            if (Vector3.Distance(newSitePos, orig_pos) < Vector3.Distance(localSitePos, orig_pos))
                            {
                                localEdge = new Edge(destin, newV2);
                                main_Edge = new Edge(newV2, origin);
                            }
                            else
                            {
                                localEdge = new Edge(origin, newV2);
                                main_Edge = new Edge(newV2, destin);
                            }
                            //if (!currentCell.edges.Any(e => (e.Origin == main_Edge.Origin || e.Destination == main_Edge.Destination)))
                                currentCell.edges.Add(localEdge);
                            //if (!newCell.edges.Any(e => (e.Origin == main_Edge.Origin || e.Destination == main_Edge.Destination)))
                                newCell.edges.Add(main_Edge);

                            current_Edge = null;
                        }
                    }
                    if (newV1 == null && newV2 == null)
                    {
                        if (LinearFunction.PreguntarSiSeCortan(cutpointBase.pos, current_Edge))
                        {
                            newV1 = new Vertice(new vec3(cutpointBase.pos));
                            for (int w = 0; w < currentCell.vertices.Count; w++)
                                if (newV1.pos.pos == currentCell.vertices[w].pos.pos)
                                    newV1 = currentCell.vertices[w];


                            if (Vector3.Distance(newSitePos, orig_pos) < Vector3.Distance(localSitePos, orig_pos))
                            {
                                localEdge = new Edge(destin, newV1);
                                main_Edge = new Edge(newV1, origin);
                            }
                            else
                            {
                                localEdge = new Edge(origin, newV1);
                                main_Edge = new Edge(newV1, destin);
                            }
                            //if (!currentCell.edges.Any(e => (e.Origin == localEdge.Origin || e.Destination == localEdge.Destination)))
                                currentCell.edges.Add(localEdge);
                            //if (!newCell.edges.Any(e => (e.Origin == main_Edge.Origin || e.Destination == main_Edge.Destination)))
                                newCell.edges.Add(main_Edge);

                            current_Edge = null;

                        }
                    }
                }
                if (newV1!= null && newV2 != null)
                {
                    newEdge = new Edge(newV1, newV2);
                    currentCell.edges.Add(newEdge);
                    newCell.edges.Add(newEdge);
                }
                List<Edge> edgetToRemove = new List<Edge>();
                List<Edge> edgetToRemoveNull = new List<Edge>();


                for (int j = 0; j < currentCell.edges.Count; j++)
                {
                    if (currentCell.edges[j]!=null)
                    edgetToRemoveNull.Add(currentCell.edges[j]);
                }
                currentCell.edges.Clear();
                for (int j = 0; j < edgetToRemoveNull.Count; j++)
                {
                    currentCell.edges.Add(edgetToRemoveNull[j]);
                }
                for (int j = 0; j < currentCell.edges.Count; j++)
                {
                    Vector3 midPoint = LinearFunction.GetMidPoint(currentCell.edges[j].Origin.pos.pos, currentCell.edges[j].Destination.pos.pos);

                    if (Vector3.Distance(newCell.Site.pos.pos,midPoint) < Vector3.Distance(localSitePos, midPoint))
                    {
                        newCell.edges.Add(currentCell.edges[j]);
                        edgetToRemove.Add(currentCell.edges[j]);
                    }
                }
                for (int j = 0; j < edgetToRemove.Count; j++)
                {
                    currentCell.edges.Remove(edgetToRemove[j]);
                }

                currentCell.vertices.Clear();
                
                for (int j = 0; j < currentCell.edges.Count; j++)
                {
                    if (currentCell.vertices.Count>0)
                    {
                
                    if(!currentCell.vertices.Any(x => x == currentCell.edges[j].Origin))
                        currentCell.vertices.Add(currentCell.edges[j].Origin);
                
                    if(!currentCell.vertices.Any(x => x == currentCell.edges[j].Destination))
                        currentCell.vertices.Add(currentCell.edges[j].Destination);
                    }
                    else
                    {
                        currentCell.vertices.Add(currentCell.edges[j].Origin);
                        currentCell.vertices.Add(currentCell.edges[j].Destination);
                    }
                }
                newCell.vertices.Clear();
                for (int j = 0; j < newCell.edges.Count; j++)
                {
                    if (newCell.vertices.Count > 0)
                    {

                        if (!newCell.vertices.Any(x => x == newCell.edges[j].Origin))
                            newCell.vertices.Add(newCell.edges[j].Origin);

                        if (!newCell.vertices.Any(x => x == newCell.edges[j].Destination))
                            newCell.vertices.Add(newCell.edges[j].Destination);
                    }
                    else
                    {
                        newCell.vertices.Add(newCell.edges[j].Origin);
                        newCell.vertices.Add(newCell.edges[j].Destination);
                    }
                }
            }
        }
        if (cells.Count == 0)
        {
            Vertice[] vertices = new Vertice[4];
            Edge[] edge = new Edge[4];
            vec3[] limitsPos = new vec3[4];

            limitsPos[0] = new vec3(new Vector3(limits.xMin, limits.yMin));
            limitsPos[1] = new vec3(new Vector3(limits.xMin, limits.yMax));
            limitsPos[2] = new vec3(new Vector3(limits.xMax, limits.yMax));
            limitsPos[3] = new vec3(new Vector3(limits.xMax, limits.yMin));
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vertice(limitsPos[i]);
                edge[i] = new Edge();
            }

            edge[0].Origin      = vertices[0];
            edge[0].Destination = vertices[1];
            edge[1].Origin      = vertices[1];
            edge[1].Destination = vertices[2];
            edge[2].Origin      = vertices[2];
            edge[2].Destination = vertices[3];
            edge[3].Origin      = vertices[3];
            edge[3].Destination = vertices[0];
            for (int i = 0; i < edge.Length; i++)
            {
                newCell.edges.Add(edge[i]);
                newCell.vertices.Add(vertices[i]);
            }
        }
        cells.Add(newCell);
    }
    private bool PerteneceALimites(Vector3 pos)
    {
        return !(pos.x < limits.xMin || pos.x > limits.xMax || pos.y < limits.yMin || pos.y > limits.yMax);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i]!=null)
            {
                for (int j = 0; j < cells[i].edges.Count; j++)
                {
                    if (cells[i].edges[j] !=null)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(cells[i].edges[j].Origin.pos.pos, cells[i].edges[j].Destination.pos.pos);
                    }
                }
                if (cells[i].Site!= null)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(cells[i].Site.pos.pos, 1);
                }
                if (cells[i].vertices!=null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(cells[i].Site.pos.pos, 0.1f);
                }
            }
        }
        if (testing)
        {
            Gizmos.DrawLine(testEdge1.Origin.pos.pos, testEdge1.Destination.pos.pos);
            Gizmos.DrawLine(testEdge2.Origin.pos.pos, testEdge2.Destination.pos.pos);
        }

        if (cells == null || cells.Count == 0)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(new Vector3(limits.xMin, limits.yMin), new Vector3(limits.xMin, limits.yMax));
            Gizmos.DrawLine(new Vector3(limits.xMin, limits.yMax), new Vector3(limits.xMax, limits.yMax));
            Gizmos.DrawLine(new Vector3(limits.xMax, limits.xMax), new Vector3(limits.xMax, limits.yMin));
            Gizmos.DrawLine(new Vector3(limits.xMax, limits.yMin), new Vector3(limits.xMin, limits.yMin));
            return;
        }
    }
}
[CustomEditor(typeof(VoronoiDiagram))]
public class VoronoiDiagramEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        VoronoiDiagram voronoi = (VoronoiDiagram)target;

        if (GUILayout.Button("resizeLimits"))
            voronoi.ResizeLimits();

        if (GUILayout.Button("AddRandom"))
            voronoi.AddRandomSite();
        if (GUILayout.Button("addSitePos"))
            voronoi.AddSite();
        if (GUILayout.Button("ClearLists"))
            voronoi.ClearLists();
        if (GUILayout.Button("TestEdges"))
            voronoi.TestEdges();

    }
}