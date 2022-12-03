using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Diciembre
{
    public class Main : MonoBehaviourSingleton<Main>
    {
        #region EXPOSED_FIELD
        [SerializeField] private UI myUI;
        [SerializeField] private ResourceSpawner rS;
        [SerializeField] private AgentSpawner aS;

        [SerializeField] private Level mapEditor;
        [SerializeField] private bool showNodes = false;
        [SerializeField] private int GuiStyleSize = 10;
        #endregion

        #region PUBLIC_FIELD
        public static Node[] mainMap;
        public static Vector2Int MapSize;
        #endregion

        #region PRIVATE_FIELD
        private PathFinding pathfinding;
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            mapEditor.MyStart();
            pathfinding = new PathFinding();
            MapSize = new Vector2Int(mapEditor.columns, mapEditor.rows);
            InitAllMap();
            mapEditor.gameObject.SetActive(false);
            VoronoiController.Init();
            myUI.Init(() => aS.SpawnAldeano(), () => { rS.SpawnResource(); VoronoiController.SetVoronoi(ResourceSpawner.Resources); });
        }
        private void OnDrawGizmos()
        {
            if (mainMap == null)
                return;
            if (!showNodes)
                return;
            GUIStyle style = new GUIStyle() { fontSize = GuiStyleSize };
            foreach (Node node in mainMap)
            {
                if (node == null)
                    continue;

                Gizmos.color = Level.GetColor((TILE_TYPE)node.weight);

                Vector3 nodePos = new Vector3(node.position.x, node.position.y);

                Gizmos.DrawWireCube(nodePos,Vector3.one);

                Handles.Label(nodePos-Vector3.one/2f, node.ID.ToString(), style);
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private void InitMap() => mainMap = new Node[mapEditor.columns * mapEditor.rows];
        private void InitNodesMap()
        {
            int ID = 0;
            for (int i = 0; i < mapEditor.columns; i++)
                for (int j = 0; j < mapEditor.rows; j++)
                {
                    mainMap[ID] = new Node(ID, new Vector2Int(i, j));
                    mainMap[ID].SetWeight((int)mapEditor.GetValueByIndexData(ID));
                    ID++;
                }
        }
        private void InitAllMap()
        {
            InitMap();
            InitNodesMap();
        }

        private static void ResetMap()
        {
            if (mainMap != null)
                foreach (Node node in mainMap)
                {
                    node.Reset();
                }
        }
        #endregion

        #region PUBLIC_METHODS
        public List<Vector2Int> GetShortestPath(Vector2Int origin, Vector2Int destination)
        {
            ResetMap();
            int v1 = NodeUtils.PositionToIndex(origin);
            int v2 = NodeUtils.PositionToIndex(destination);
            if (v1 == -1 || v2 == -1)
            {
                Debug.Log("la posicion no pertenece a la grid");
                return null;
            }
            return pathfinding.GetPath(mainMap,
                mainMap[v1],
                mainMap[v2]);
        }
        #endregion

    }
}
