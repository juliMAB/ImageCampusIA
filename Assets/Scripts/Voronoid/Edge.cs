using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Edge
{
    public static uint num = 0;

    [ReadOnly] public string name;

    public Vertice origin;
    public Vertice destination;

    public Edge(Vertice origin, Vertice destination)
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
