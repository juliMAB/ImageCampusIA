using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Voronoid3 : MonoBehaviour
{
    public float sizedot;

    public List<Vector3> points = new List<Vector3>();

    public List<Vector3> midPoint = new List<Vector3>();

    public Vector3 CustomAddPoint;

    public List <PerpendicularBisectriz> f = new List<PerpendicularBisectriz>();

    public List< Vector3 >CtPoints = new List<Vector3 >();

    private void CustomRun()
    {
        int q = points.Count;
        points.Add(CustomAddPoint);
        if(q>=1)
        {
            for (int i = 0; i < q; i++)
            {
                f.Add(new PerpendicularBisectriz(points[i], CustomAddPoint));
                for (int w = 0; w < f.Count-1; w++)
                {
                    Vector3 nP = LinearFunction.PreguntarSiSeCortan(f[f.Count - 1].function, f[w].function);
                    bool any = CtPoints.Any(p => p == nP);
                    if (!nanOrInf(nP) && !any)
                        CtPoints.Add(nP);
                }
            }
        }
    }
    private bool nanOrInf(Vector3 A)
    {
        return (nanOrInf(A.x) || nanOrInf(A.y) || nanOrInf(A.z));
    }
    private bool nanOrInf(float A)
    {
        if (A == 0)
            return false;
        return !float.IsNormal(A);
    }
    public void ClearLists()
    {
        points.Clear();
        midPoint.Clear();
        f.Clear();
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawWireSphere(points[i], sizedot);
        }
        for (int i = 0; i < f.Count; i++)
        {
            f[i].function.DrawGizmo();
        }
        for (int i = 0; i < CtPoints.Count; i++)
        {
            Gizmos.DrawWireSphere(CtPoints[i], sizedot);
        }
    }
    [CustomEditor(typeof(Voronoid3))]
    public class Voronoi3Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Voronoid3 voronoi = (Voronoid3)target;

            if (GUILayout.Button("CustomRun"))
                voronoi.CustomRun();
            if (GUILayout.Button("ClearLists"))
                voronoi.ClearLists();

        }
    }
}
