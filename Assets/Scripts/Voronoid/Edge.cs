using System;

[Serializable]
public class Edge
{
    public static uint num = 0;

    [ReadOnly] public string name;

    [UnityEngine.SerializeField] private Vertice origin;
    [UnityEngine.SerializeField] private Vertice destination;

    [UnityEngine.SerializeField] public LinearFunction lf = null;
    public Vertice Origin {
        get => origin; 
        set 
        {
            origin = value;
            if(origin!=null && destination != null)
                lf = new LinearFunction(origin.pos.pos, Destination.pos.pos);
        }
    }

    public Vertice Destination { get => destination; set { destination = value; if (origin != null && destination != null) lf = new LinearFunction(origin.pos.pos, Destination.pos.pos); } }

    public Edge(Vertice origin, Vertice destination)
    {
        this.Origin = origin;
        this.Destination = destination;

        num++;
        name = "E:" + num.ToString();
    }
    public Edge()
    {
        num++;
        name = "E:" + num.ToString();
    }
}
