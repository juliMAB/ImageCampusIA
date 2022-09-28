using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VoronoiDiagram : MonoBehaviour
{
    [SerializeField] List<Site> sites = new List<Site>();
    [SerializeField] List<Cell> cells = new List<Cell>();
    [SerializeField] Vector2Int LimitSize;

    [SerializeField] Transform p1;
    [SerializeField] Transform p2;
    

    [ReadOnly][SerializeField] RectInt limits=new RectInt();
    [SerializeField] Edge nuevoEdge = null;

    [SerializeField] Vector3 posToAdd;

    private void OnValidate()
    {
        ResizeLimits();
    }

    public void ResizeLimits()
    {
        limits = new RectInt(new Vector2Int(LimitSize.x/2, LimitSize.y / 2), LimitSize);
        limits.xMin = 0;
        limits.yMin = 0;
        limits.size = LimitSize;
    }
    public void AddRandomSite()
    {
        Site newSite = new Site(GetRandomPointOnLimits(), UnityEngine.Random.Range(0f, 1f));
        
        RecalculateVoronoi(newSite);
        sites.Add(newSite);
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
        sites.Add(newSite);
    }

    public void ClearLists()
    {
        sites.Clear();
        cells.Clear();
    }
    public vec3 GetRandomPointOnLimits()
    {
        return new vec3 (new Vector3(UnityEngine.Random.Range(limits.xMin, limits.xMax), UnityEngine.Random.Range(limits.yMin, limits.yMax)));
    }

    public void RecalculateVoronoi(Site newSite)
    {
        nuevoEdge = new Edge();
        Cell newCell = new Cell();
        newCell.Site = newSite;
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].Inside(newSite.pos.pos))
            {
                vec3 localSitePos = cells[i].Site.pos;
                Vector3 midpos = LinearFunction.GetMidPoint(newSite.pos.pos, localSitePos.pos);
                float slope = LinearFunction.GetSlope(newSite.pos.pos, localSitePos.pos);
                if (float.IsNaN(slope))
                    slope = float.PositiveInfinity;
                else if (float.IsInfinity(slope))
                    slope = float.NaN;
                else
                slope = -1 / slope;
                Edge newEdge;
                Vertice newV1 = null;
                Vertice newV2 = null;

                LinearFunction function = new LinearFunction(slope, midpos);
                for (int j = 0; j < cells[i].edges.Count; j++)
                {
                    Edge localEdge = null;
                    Edge main_Edge = null;

                    if (!LinearFunction.PreguntarSiSeCortan(function, cells[i].edges[j]))
                        continue;

                    Vector3 cutpointBase = LinearFunction.ObtenerPuntoDeCorte(function, cells[i].edges[j]);
                    vec3 cutpoint = new vec3(cutpointBase);

                    if (cutpoint.pos == cells[i].edges[j].origin.pos || cutpoint.pos == cells[i].edges[j].destination.pos)
                        continue;


                    if ((!LinearFunction.NanOrInfin(cutpoint.pos)) && PerteneceALimites(cutpoint.pos))
                    {
                        if (newV1!=null)
                        {
                            if (LinearFunction.PreguntarSiSeCortan(cutpoint.pos, cells[i].edges[j]))
                            {
                                newV2 = new Vertice(cutpoint);
                                newCell.vertices.Add(newV2);
                                cells[i].vertices.Add(newV2);
                                if (Vector3.Distance(newSite.pos.pos, cells[i].edges[j].origin.pos) < Vector3.Distance(localSitePos.pos, cells[i].edges[j].origin.pos))
                                {
                                    localEdge = new Edge(cells[i].edges[j].destination, newV2.pos);
                                    main_Edge = new Edge(newV2.pos, cells[i].edges[j].origin);
                                    cells[i].edges.Add(localEdge);
                                    newCell.edges.Add(main_Edge);
                                    var c = cells[i].vertices.Find(x => x.pos == cells[i].edges[j].destination);
                                    cells[i].vertices.Remove(c);
                                }
                                else
                                {
                                    localEdge = new Edge(cells[i].edges[j].origin, newV2.pos);
                                    main_Edge = new Edge(newV2.pos, cells[i].edges[j].destination);
                                    cells[i].edges.Add(localEdge);
                                    newCell.edges.Add(main_Edge);
                                    var c = cells[i].vertices.Find(x => x.pos == cells[i].edges[j].origin);
                                    cells[i].vertices.Remove(c);
                                }
                                cells[i].edges[j] = null;
                            }
                        }
                        else
                        {
                            

                            if (LinearFunction.PreguntarSiSeCortan(cutpoint.pos, cells[i].edges[j]))
                            {
                                newV1 = new Vertice(cutpoint);
                                newCell.vertices.Add(newV1);
                                cells[i].vertices.Add(newV1);
                                if (Vector3.Distance(newSite.pos.pos, cells[i].edges[j].origin.pos) < Vector3.Distance(localSitePos.pos, cells[i].edges[j].origin.pos))
                                {
                                    localEdge = new Edge(cells[i].edges[j].destination, newV1.pos);
                                    main_Edge = new Edge(newV1.pos, cells[i].edges[j].origin);
                                    cells[i].edges.Add(localEdge);
                                    newCell.edges.Add(main_Edge);
                                    var c = cells[i].vertices.Find(x => x.pos == cells[i].edges[j].destination);
                                    cells[i].vertices.Remove(c);
                                }
                                else
                                {
                                    localEdge = new Edge(cells[i].edges[j].origin, newV1.pos);
                                    main_Edge = new Edge(newV1.pos, cells[i].edges[j].destination);
                                    cells[i].edges.Add(localEdge);
                                    newCell.edges.Add(main_Edge);
                                    var c = cells[i].vertices.Find(x => x.pos == cells[i].edges[j].origin);
                                    cells[i].vertices.Remove(c);
                                }
                                


                                cells[i].edges[j] = null;
                            }
                        }
                    }
                }
                if (newV1!= null && newV2 != null)
                {
                    newEdge = new Edge(newV1.pos, newV2.pos);
                    cells[i].edges.Add(newEdge);
                    cells[i].vertices.Add(newV1);
                    cells[i].vertices.Add(newV2);
                    newCell.vertices.Add(newV1);
                    newCell.vertices.Add(newV2);
                    newCell.edges.Add(newEdge);
                }
                List<Edge> edgetToRemove = new List<Edge>();
                for (int j = 0; j < cells[i].edges.Count; j++)
                {
                    if (cells[i].edges[j] == null)
                        continue;
                    Vector3 midPoint = LinearFunction.GetMidPoint(cells[i].edges[j].origin.pos, cells[i].edges[j].destination.pos);

                    if (Vector3.Distance(newCell.Site.pos.pos,midPoint) < Vector3.Distance(localSitePos.pos, midPoint))
                    {
                        newCell.edges.Add(cells[i].edges[j]);
                        edgetToRemove.Add(cells[i].edges[j]);
                    }
                }
                for (int j = 0; j < edgetToRemove.Count; j++)
                {
                    cells[i].edges.Remove(edgetToRemove[j]);
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

            edge[0].origin      = vertices[0].pos;
            edge[0].destination = vertices[1].pos;
            edge[1].origin      = vertices[1].pos;
            edge[1].destination = vertices[2].pos;
            edge[2].origin      = vertices[2].pos;
            edge[2].destination = vertices[3].pos;
            edge[3].origin      = vertices[3].pos;
            edge[3].destination = vertices[0].pos;
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

    
    public void AddLastSiteInList()
    {
        if (sites!=null)
        {
            if (sites.Count >= 2)
            {
                RecalculateVoronoi(sites[sites.Count-1]);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (cells == null || cells.Count==0)
        {
        Gizmos.DrawLine(new Vector3 (limits.xMin,limits.yMin), new Vector3(limits.xMin, limits.yMax));
        Gizmos.DrawLine(new Vector3 (limits.xMin, limits.yMax), new Vector3(limits.xMax, limits.yMax));
        Gizmos.DrawLine(new Vector3 (limits.xMax,limits.xMax), new Vector3(limits.xMax, limits.yMin));
        Gizmos.DrawLine(new Vector3 (limits.xMax,limits.yMin), new Vector3(limits.xMin, limits.yMin));
            return;
        }

        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i]!=null)
            {
                for (int j = 0; j < cells[i].edges.Count; j++)
                {
                    if (cells[i].edges[j] !=null)
                    {
                        Gizmos.DrawLine(cells[i].edges[j].origin.pos, cells[i].edges[j].destination.pos);
                    }
                }
                if (cells[i].Site!= null)
                {
                    Gizmos.DrawWireSphere(cells[i].Site.pos.pos, 1);
                }
                if (cells[i].vertices!=null)
                {
                    Gizmos.DrawWireSphere(cells[i].Site.pos.pos, 0.1f);
                }
            }
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

    }
}