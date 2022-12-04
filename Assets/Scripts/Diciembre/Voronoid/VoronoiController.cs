using System.Collections.Generic;
using UnityEngine;

namespace Diciembre
{
    public static class VoronoiController
    {
    #region PRIVATE_FIELDS
    private static List<Limit> limits = null;
    private static List<Sector> sectors = null;
    #endregion

    #region PUBLIC_METHODS
    public static void Init()
    {
        sectors = new List<Sector>();
        InitLimits();
    }

    public static void SetVoronoi(List<Resource> mines)
    {
        sectors.Clear();
        if (mines.Count == 0) 
            return;

        for (int i = 0; i < mines.Count; i++)
            sectors.Add(new Sector(mines[i]));
        

        for (int i = 0; i < sectors.Count; i++)
            sectors[i].AddSegmentLimits(limits);
        

        for (int i = 0; i < mines.Count; i++)
        {
            for (int j = 0; j < mines.Count; j++)
            {
                if (i == j) 
                    continue;
                sectors[i].AddSegment(mines[i].transform.position, mines[j].transform.position); //on this point if is posible modific this by the weight.
                }
        }

        for (int i = 0; i < sectors.Count; i++)
            sectors[i].SetIntersections();
        
    }

    public static Resource GetMineCloser(Vector3 minerPos)
    {
        if (sectors != null)
            for (int i = 0; i < sectors.Count; i++)
                if (sectors[i].CheckPointInSector(minerPos))
                    return sectors[i].Mine;
        return null;
    }
        public static void DrawSectors()
    {
        if (sectors == null) return;

        for (int i = 0; i < sectors.Count; i++)
        {
            sectors[i].DrawSector();
        }
    }
        public static void DrawSegments()
        {
            if (sectors == null)
                return;
            for (int i = 0; i < sectors.Count; i++)
            {
                sectors[i].DrawSegments();
            }
        }

    #endregion

    #region PRIVATE_METHODS
        private static void InitLimits()
        {
            limits = new List<Limit>();

                Vector2 offset = Vector2.zero;

            limits.Add(new Limit(Vector2.zero + offset, DIRECTION.LEFT));
            limits.Add(new Limit(new Vector2(0, Main.MapSize.y) + offset, DIRECTION.UP));
            limits.Add(new Limit(new Vector2(Main.MapSize.x, Main.MapSize.y) + offset, DIRECTION.RIGHT));
            limits.Add(new Limit(new Vector2(Main.MapSize.x, 0) + offset, DIRECTION.DOWN));
        }

    #endregion
    }
}