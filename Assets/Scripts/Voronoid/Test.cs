using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    public void CustomCall()
    {
        //Debug.Log(float.IsNormal(0.000000000001f));
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
    }
}
