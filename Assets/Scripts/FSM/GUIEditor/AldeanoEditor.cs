using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FSM;

[CustomEditor(typeof(Aldeano))]
public class AldeanoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Aldeano aldeano = (Aldeano)target;

        if (GUILayout.Button("Buscar Mina"))
        {
            aldeano.ForceToWork();
        }
    }
}
