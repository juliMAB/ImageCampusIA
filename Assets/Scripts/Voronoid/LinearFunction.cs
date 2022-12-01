using System;
using UnityEngine;
using static UnityEditor.PlayerSettings;


/// <summary>
/// Tener siempre presente:
/// 
/// Y - moveY = slope*(x - moveX);
/// 
/// </summary>
[Serializable]
public class LinearFunction
{
    


    public float moveY;
    public float moveX;

    public float slope;

    public void DrawGizmo()
    {
        float bigNumber = 9999f;
        if (float.IsNaN(slope))
        {
            Vector3 origin = new Vector3(-bigNumber, moveY);
            Vector3 direct = new Vector3(bigNumber, moveY);
            Gizmos.DrawLine(origin, direct);
        }
        else if (float.IsInfinity(slope))
        {
            Vector3 origin = new Vector3(moveX, -bigNumber);
            Vector3 direct = new Vector3(moveX, bigNumber);
            Gizmos.DrawLine(origin, direct);
        }
        else
        {
            float farY = bigNumber;
            float farX = GetX(farY);
            Vector3 origin = new Vector3(farX, farY);
            farX = GetX(-farY);
            Vector3 direct = new Vector3(farX, -farY);
            Gizmos.DrawLine(origin, direct);
        }
    }

    public static float GetSlope(Vector3 p1, Vector3 p2)
    {
        float slope = (p2.y - p1.y) / (p2.x - p1.x);
        if (float.IsNaN(slope))
            slope = float.PositiveInfinity;
        else if (float.IsInfinity(slope))
            slope = float.NaN;
        else
            slope = -1 / slope;
        return slope;
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
            if (float.IsInfinity(b.slope))
            {
                return b.moveX;
            }
            else //b.slope es normal=> b.movey,b.movex es normal.
                return b.GetX(a.moveY);
        }
        else if (float.IsInfinity(a.slope))
        {
            if (float.IsInfinity(b.slope))
            {
                return float.PositiveInfinity; // nunca se van a cruzar.
            }
            else
                return a.moveX;
        }
        else //a.slope es normal.
        {
            if (float.IsNaN(b.slope))
            {
                return a.GetX (b.moveY);
            }
            if (float.IsInfinity(b.slope))
            {
                return b.moveX;
            }
            else // a y b es normal.
                return (-b.moveX * b.slope + b.moveY - a.moveY + a.moveX * a.slope) / (a.slope - b.slope);
                //return ((b.slope * (b.moveX) + b.moveY) - (a.slope * (a.moveX) + a.moveY)) / (a.slope - b.slope);
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
            else if (float.IsInfinity(b.slope))
            {
                return a.moveY;
            }
            else
                return a.moveY;
        }
        else if (float.IsInfinity(a.slope))
        {
            if (float.IsInfinity(b.slope))
            {
                return float.PositiveInfinity;
            }
            if (float.IsNaN(b.slope))
            {
                return b.moveY;
            }
            else
                return b.GetY (a.moveX);
        }
        else
        {
            if (float.IsNaN(b.slope))
            {
                return b.moveY;
            }
            if (float.IsInfinity(b.slope))
            {
                return a.GetY(b.moveX);
            }
            else
                return (a.slope * ((b.slope * (-b.moveX) + b.moveY)) + ((a.slope * (-a.moveX) + a.moveY)) * b.slope) / (b.slope - a.slope);
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
            moveX = mp.x;
            return;
        }
        else if (float.IsNaN(slope))
        {
            moveY = mp.y;
            moveX = 0;
            return;
        }
        else
        {
            moveX = mp.x;
            moveY = mp.y;
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
            moveX = mp.x;
            return;
        }
        else if(float.IsNaN(slope))
        {
            moveY = mp.y;
            moveX = 0;
            return;
        }
        else
        {
            moveX = mp.x;
            moveY = mp.y;
            return;
        }
    }
    public static Vector3 PreguntarSiSeCortan(Edge a, Edge b)
    {
        LinearFunction functionB = new LinearFunction(b.Origin.pos.pos, b.Destination.pos.pos);
        LinearFunction functionA = new LinearFunction(a.Origin.pos.pos, a.Destination.pos.pos);
        float x = PreguntarSiSeCortanX(functionA, functionB);
        float y = PreguntarSiSeCortanY(functionA, functionB);
        Rect localLimitB = new Rect();
        if (b.Origin.pos.pos.x < b.Destination.pos.pos.x)
        {
            localLimitB.xMin = b.Origin.pos.pos.x;
            localLimitB.xMax = b.Destination.pos.pos.x;
        }
        else
        {
            localLimitB.xMin = b.Destination.pos.pos.x;
            localLimitB.xMax = b.Origin.pos.pos.x;
        }
        if (b.Origin.pos.pos.y < b.Destination.pos.pos.y)
        {
            localLimitB.yMin = b.Origin.pos.pos.y;
            localLimitB.yMax = b.Destination.pos.pos.y;
        }
        else
        {
            localLimitB.yMin = b.Destination.pos.pos.y;
            localLimitB.yMax = b.Origin.pos.pos.y;
        }
        Rect localLimitA = new Rect();

        if (a.Origin.pos.pos.x < a.Destination.pos.pos.x)
        {
            localLimitA.xMin = a.Origin.pos.pos.x;
            localLimitA.xMax = a.Destination.pos.pos.x;
        }
        else
        {
            localLimitA.xMin = a.Destination.pos.pos.x;
            localLimitA.xMax = a.Origin.pos.pos.x;
        }
        if (a.Origin.pos.pos.y < a.Destination.pos.pos.y)
        {
            localLimitA.yMin = a.Origin.pos.pos.y;
            localLimitA.yMax = a.Destination.pos.pos.y;
        }
        else
        {
            localLimitA.yMin = a.Destination.pos.pos.y;
            localLimitA.yMax = a.Origin.pos.pos.y;
        }


        if (localLimitA.xMax >= localLimitB.xMin &&
            localLimitB.xMax >= localLimitA.xMin &&
            localLimitA.yMax >= localLimitB.yMin &&
            localLimitB.yMax >= localLimitA.yMin)
            return new Vector3(x, y);
        return new Vector3(float.NaN, float.NaN);
        
    }

    public static bool PreguntarSiSeCortan(LinearFunction a, Edge b)
    {
        LinearFunction functionB = new LinearFunction(b.Origin.pos.pos, b.Destination.pos.pos);

        float x = PreguntarSiSeCortanX(a, functionB);
        if (float.IsNaN(x) || float.IsInfinity(x))
            return false;

        float y = PreguntarSiSeCortanY(a, functionB);
        if (float.IsNaN(y)||float.IsInfinity(y))
            return false;

        Vector3 cut = new Vector3(x, y);
        //CORREGUIR ESTO!:
        float DistanciaEntrePuntos = Vector3.Distance(b.Origin.pos.pos, b.Destination.pos.pos);
        float DistanciaACutOrigen = Vector3.Distance(b.Origin.pos.pos, cut);
        if (DistanciaEntrePuntos < DistanciaACutOrigen)
            return false;
        float DistanciaACutDestin = Vector3.Distance(b.Destination.pos.pos, cut);
        if (DistanciaEntrePuntos < DistanciaACutDestin)
            return false;
        return true;
    }

    public static Vector3 ObtenerPuntoDeCorte(LinearFunction a, Edge b)
    {
        LinearFunction functionB = new LinearFunction(b.Origin.pos.pos, b.Destination.pos.pos);
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
        LinearFunction functionB = new LinearFunction(b.Origin.pos.pos, b.Destination.pos.pos);
        if (float.IsNaN(functionB.slope))
        {
            if (a.y != functionB.moveY)
                return false;
            return true;
        }
        if (float.IsInfinity(functionB.slope))
        {
            if (a.x != functionB.moveX)
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
        if (float.IsNaN(slope))
            return float.NaN;
        else if (float.IsInfinity(slope))
            return -moveX;
        else
            return (y - moveY) / slope + moveX;
            //return (y - ((slope * (moveX + moveY))) / slope);
    }

    public float GetY(float x)
    {
        if (float.IsInfinity(slope))
            return float.PositiveInfinity;
        else if (float.IsNaN(slope))
            return -moveY;
        else
            return slope * (x - moveX) + moveY;
            //return  slope * (x + moveX) - moveY;
    }

}
