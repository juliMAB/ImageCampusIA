using System;
using System.Collections.Generic;

[Serializable]
public class Site
{
    static uint num = 0;

    [ReadOnly] public string name;

    public vec3 pos;
    public float weight;

    public Site(vec3 pos, float weight)
    {
        this.pos = pos;
        this.weight = weight;
        num++;
        name = "S:" + num;
    }
}
