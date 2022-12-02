using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
class nodeVisual
{
    public bool showNodes;
    public bool showLabel;

    public int sizeLabel = 10;
    public Vector2 offsetLabel = new Vector2(-0.45f, -0.45f);
    public float alphaColor = 0.5f;
}

public class NodeGenerator : MonoBehaviourSingleton<NodeGenerator>
{
    public Level level = null;
    public static Node[] map;
    private PathFinding pathfinding;
    [SerializeField] private nodeVisual nv;

    void Start()
    {
        level.RestoreData();
        ResetMap();
        pathfinding = new PathFinding();
        NodeUtils.MapSize = new Vector2Int(level.columns,level.rows);
        map = new Node[level.columns * level.rows];
        int ID = 0;
        for (int i = 0; i < level.rows; i++)
        {
            for (int j = 0; j < level.columns; j++)
            {
                map[ID] = new Node(ID, new Vector2Int(j, i));
                ID++;
            }
        }
        for (int i = 0; i < level.rows; i++)
        {
            for (int j = 0; j < level.columns; j++)
            {
                if (level.board[i,j]==-1)
                {
                    map[NodeUtils.PositionToIndex(new Vector2Int(i, j))].state = Node.NodeState.Obstacle;
                }
                else
                {
                    map[NodeUtils.PositionToIndex(new Vector2Int(i, j))].SetWeight(level.board[i, j]);
                }
            }
        }
    }


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
        return pathfinding.GetPath(map,
            map[v1],
            map[v2]);
    }

    private static void ResetMap()
    {
        if(map!=null)
        foreach (Node node in map)
        {
            node.Reset();
        }
    }
    private void OnDrawGizmos()
    {
        if (map == null)
            return;
        if (!nv.showNodes)
            return;
        Color color = Color.green;
        GUIStyle style = new GUIStyle() { fontSize = nv.sizeLabel };
        foreach (Node node in map)
        {
            switch (node.state)
            {
                case Node.NodeState.Open:
                    color = Color.green;
                    break;
                case Node.NodeState.Closed:
                    color = Color.black;
                    break;
                case Node.NodeState.Ready:
                    color = Color.white;
                    break;
                case Node.NodeState.Obstacle:
                    color = Color.red;
                    break;
                default:
                    break;
            }

            color.a = nv.alphaColor;
            Gizmos.color = color;


            if (nv.showLabel)
            {
                string label = node.position.ToString() + "\nID: " + node.ID + "\n Peso: " + node.weight;
                Handles.Label(node.position + nv.offsetLabel, label, style);
            }

            Gizmos.color = Color.black;
            switch (node.weight)
            {
                case 1:
                    Gizmos.color = Color.green;
                    break;
                case 2:
                    Gizmos.color = Color.gray;
                    break;
                case 3:
                    Gizmos.color = Color.blue;
                    break;
                case 4:
                    Gizmos.color = Color.yellow;
                    break;
                default:
                    Gizmos.color = Color.black;
                    break;
            }
            Gizmos.DrawWireCube(new Vector3 (node.position.x, node.position.y), new Vector3(1, 1, 0));

            Vector3 worldPosition = new Vector3(node.position.x, node.position.y, 0.0f);
            Gizmos.DrawWireSphere(worldPosition, 0.2f);
            //Gizmos.color = Color.black;
            Handles.Label(worldPosition, node.ID.ToString(), style);
        }
    }
}
