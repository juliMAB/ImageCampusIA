using System;
using UnityEngine;

[Serializable]
public class PerpendicularBisectriz
{
    public LinearFunction function;
    public Vector3 Site1,Site2;
    public PerpendicularBisectriz(Vector3 A, Vector3 B)
    {
        Site1 = A;
        Site2 = B;
        Vector3 mid = LinearFunction.GetMidPoint(A, B);
        float slope = LinearFunction.GetSlope(A, B);
        function = new LinearFunction(slope,mid);
    }
}
