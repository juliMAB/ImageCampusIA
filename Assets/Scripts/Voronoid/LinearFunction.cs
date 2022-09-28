using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Tener siempre presente:
/// 
/// Y - moveY = slope*(x - moveX);
/// 
/// </summary>
public class LinearFunction
{
    


    public float moveY;
    public float moveX;

    public float slope;

    public static float GetSlope(Vector3 p1, Vector3 p2)
    {
        return (p2.y - p1.y) / (p2.x - p1.x);
    }
    public static Vector3 GetMidPoint(Vector3 p1, Vector3 p2)
    {
        return new Vector3((p1.x + p2.x) / 2, (p1.y + p2.y) / 2, (p1.z + p2.z) / 2);
    }

    public static float PreguntarSiSeCortanX(LinearFunction a, LinearFunction b)
    {
        if (float.IsNaN(a.slope))
        {
            if (float.IsNaN(b.slope))
            {
                return float.NaN; //nunca se van a cruzar.
            }
            else
                return -b.moveX;
        }
        else if (float.IsInfinity(a.slope))
        {
            if (float.IsInfinity(b.slope))
            {
                return float.PositiveInfinity;
            }
            else
                return -a.moveX;
        }
        else
        {
            if (float.IsNaN(b.slope))
            {
                return float.NaN;
            }
            if (float.IsInfinity(b.slope))
            {
                return -a.moveX;
            }
            else
                return ((b.slope * (-b.moveX) + b.moveY) - (a.slope * (-a.moveX) + a.moveY)) / (a.slope - b.slope);
        }
        
    }
    public static float PreguntarSiSeCortanY(LinearFunction a, LinearFunction b)
    {
        if (float.IsNaN(a.slope))
        {
            if (float.IsNaN(b.slope))
            {
                return float.NaN;
            }
            else
                return -a.moveY;
        }
        else if (float.IsInfinity(a.slope))
        {
            if (float.IsInfinity(b.slope))
            {
                return float.PositiveInfinity;
            }
            else
                return -b.moveY;
        }
        else
        {
            if (float.IsNaN(b.slope))
            {
                return -b.moveY;
            }
            if (float.IsInfinity(b.slope))
            {
                return -a.moveY;
            }
            else
                return (-a.slope * ((b.slope * (-b.moveX) + b.moveY)) + ((a.slope * (-a.moveX) + a.moveY)) * b.slope) / (b.slope - a.slope);
        }
    }

    public static Vector3 PreguntarSiSeCortan(LinearFunction a, LinearFunction b)
    {
        float x = PreguntarSiSeCortanX(a, b);
        float y = PreguntarSiSeCortanY(a, b);
        return new Vector3(x, y);
    }

    public LinearFunction(Vector3 p1, Vector3 p2)
    {
        slope = GetSlope(p1, p2);
        Vector3 mp = GetMidPoint(p1, p2);
        if (slope==0)
            slope = float.NaN;

        if (float.IsInfinity(slope))
        {
            moveY = 0;
            moveX = -mp.x;
            return;
        }
        else if (float.IsNaN(slope))
        {
            moveY = -mp.y;
            moveX = 0;
            return;
        }
        else
        {
            moveX = -mp.x;
            moveY = -mp.y;
            return;
        }
    }
    public LinearFunction(float slop, Vector3 midpoint)
    {
        slope = slop;
        Vector3 mp = midpoint;
        if (slope == 0)
            slope = float.NaN;

        if (float.IsInfinity (slop))
        {
            moveY = 0;
            moveX = -mp.x;
            return;
        }
        else if(float.IsNaN(slope))
        {
            moveY = -mp.y;
            moveX = 0;
            return;
        }
        else
        {
            moveX = -mp.x;
            moveY = -mp.y;
            return;
        }
    }
    public static Vector3 PreguntarSiSeCortan(Edge a, Edge b)
    {
        LinearFunction functionB = new LinearFunction(b.origin.pos, b.destination.pos);
        LinearFunction functionA = new LinearFunction(a.origin.pos, a.destination.pos);
        float x = PreguntarSiSeCortanX(functionA, functionB);
        float y = PreguntarSiSeCortanY(functionA, functionB);
        Rect localLimitB = new Rect();
        if (b.origin.pos.x < b.destination.pos.x)
        {
            localLimitB.xMin = b.origin.pos.x;
            localLimitB.xMax = b.destination.pos.x;
        }
        else
        {
            localLimitB.xMin = b.destination.pos.x;
            localLimitB.xMax = b.origin.pos.x;
        }
        if (b.origin.pos.y < b.destination.pos.y)
        {
            localLimitB.xMin = b.origin.pos.y;
            localLimitB.xMax = b.destination.pos.y;
        }
        else
        {
            localLimitB.xMin = b.destination.pos.y;
            localLimitB.xMax = b.origin.pos.y;
        }
        Rect localLimitA = new Rect();

        if (a.origin.pos.x < a.destination.pos.x)
        {
            localLimitA.xMin = a.origin.pos.x;
            localLimitA.xMax = a.destination.pos.x;
        }
        else
        {
            localLimitA.xMin = a.destination.pos.x;
            localLimitA.xMax = a.origin.pos.x;
        }
        if (a.origin.pos.y < a.destination.pos.y)
        {
            localLimitA.xMin = a.origin.pos.y;
            localLimitA.xMax = a.destination.pos.y;
        }
        else
        {
            localLimitA.xMin = a.destination.pos.y;
            localLimitA.xMax = a.origin.pos.y;
        }
        if ((localLimitA.xMin >= localLimitB.xMax && localLimitA.xMax >= localLimitB.xMin) && 
            (localLimitA.yMin >= localLimitB.yMax && localLimitA.yMax >= localLimitB.yMin))
        {
            return new Vector3(x, y);
        }
        return new Vector3(float.NaN, float.NaN);
        
    }

    public static bool PreguntarSiSeCortan(LinearFunction a, Edge b)
    {
        LinearFunction functionB = new LinearFunction(b.origin.pos, b.destination.pos);
        float x = PreguntarSiSeCortanX(a, functionB);
        float y = PreguntarSiSeCortanY(a, functionB);
        if (float.IsNaN(x)||float.IsInfinity(x)||float.IsNaN(y)||float.IsInfinity(y))
            return false;

        Vector3 cut = new Vector3(x, y);

        float DistanciaEntrePuntos = Vector3.Distance(b.origin.pos, b.destination.pos);
        float DistanciaACutOrigen = Vector3.Distance(b.origin.pos, cut);
        if (DistanciaEntrePuntos < DistanciaACutOrigen)
            return false;
        float DistanciaACutDestin = Vector3.Distance(b.destination.pos, cut);
        if (DistanciaEntrePuntos < DistanciaACutDestin)
            return false;
        return true;
    }

    public static Vector3 ObtenerPuntoDeCorte(LinearFunction a, Edge b)
    {
        LinearFunction functionB = new LinearFunction(b.origin.pos, b.destination.pos);
        float x = PreguntarSiSeCortanX(a, functionB);
        float y = PreguntarSiSeCortanY(a, functionB);

            return new Vector3(x, y);
    }
    public static bool NanOrInfin(Vector3 pos)
    {
        return (float.IsNaN(pos.x) || float.IsInfinity(pos.x) || float.IsNaN(pos.y) || float.IsInfinity(pos.y));
    }
    public static bool PreguntarSiSeCortan(Vector3 a, Edge b)
    {
        LinearFunction functionB = new LinearFunction(b.origin.pos, b.destination.pos);
        if (float.IsNaN(functionB.slope))
        {
            if (a.y != -functionB.moveY)
                return false;
            return true;
        }
        if (float.IsInfinity(functionB.slope))
        {
            if (a.x != -functionB.moveX)
                return false;
            return true;
        }
        if (!(float.IsNaN(functionB.slope)) && !(float.IsInfinity(functionB.slope)))
        {
            float x = functionB.GetX(a.y);
            return x == a.x;
        }
        return false;


    }
    /// <summary>
    /// Preguntar siempre que sea diferente de nan y infinito.
    /// </summary>
    public float GetX(float y)
    {
        return (y - ((slope * (moveX + moveY))) / slope);
    }

}
