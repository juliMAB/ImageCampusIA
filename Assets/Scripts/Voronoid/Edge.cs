using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Edge
{
    static uint num = 0;

    [ReadOnly] public string name;

    public vec3 origin;
    public vec3 destination;

    public Edge(vec3 origin, vec3 destination)
    {
        this.origin = origin;
        this.destination = destination;

        num++;
        name = "E:" + num.ToString();
    }
    public Edge()
    {
        num++;
        name = "E:" + num.ToString();
    }
}
