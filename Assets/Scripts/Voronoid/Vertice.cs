using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Vertice
{
    static uint num = 0;

    [ReadOnly] public string name;

    public vec3 pos = new vec3();

    public Vertice(vec3 pos)
    {
        this.pos = pos;
        num++;
        name = "V:" + num.ToString();
    }
}

[Serializable]
public class vec3
{
    public Vector3 pos;

    public vec3(Vector3 pos)
    {
        this.pos = pos;
    }
    public vec3()
    {
        this.pos = Vector3.zero;
    }
}
