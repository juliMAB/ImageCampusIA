using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] LinearFunction A;
    [SerializeField] LinearFunction B;
    [SerializeField][ReadOnly] Vector3 cutPoint;

    [SerializeField] Vector3 Apos;
    [SerializeField] Vector3 Bpos;

    [SerializeField] Edge newEdge = new Edge();
    public void CustomCall()
    {
        cutPoint = LinearFunction.PreguntarSiSeCortan(A, B);
    }

    public void CustomCall2()
    {
        newEdge.lf = CreateTheNewEdge(Apos, Bpos);
    }

    public LinearFunction CreateTheNewEdge(Vector3 A, Vector3 B)
    {
        Vector3 midpos = LinearFunction.GetMidPoint(A, B);
        float slope = LinearFunction.GetSlope(A, B);

        if (float.IsNaN(slope))
            slope = float.PositiveInfinity;
        else if (float.IsInfinity(slope))
            slope = float.NaN;
        else
            slope = -1 / slope;

        return new LinearFunction(slope, midpos);
    }
    private void OnDrawGizmos()
    {
        if (A!=null)
        {
            A.DrawGizmo();
        }
        if (B!=null)
        {
            B.DrawGizmo();
        }
        if (newEdge.lf!=null)
        {
            newEdge.lf.DrawGizmo();
        }
    }
}

[CustomEditor(typeof(Test))]
public class TestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Test test = (Test)target;

        if (GUILayout.Button("CustomCall"))
            test.CustomCall();

        if (GUILayout.Button("CustomCall2"))
            test.CustomCall2();
    }
}
