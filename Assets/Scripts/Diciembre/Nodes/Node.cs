using System.Collections.Generic;
using UnityEngine;

namespace Diciembre
{
    public class Node
    {
    public enum NodeState
    {
        Open,    //Abiertos por otro nodo pero no visitados
        Closed,  //ya visitados
        Ready,   //no abiertos por nadie
        Obstacle = -1

    }

        #region PUBLIC_FIELDS
        public int ID;
        public int openerID;
        public int weight = 1;
        private int originalWigth;
        public int totalWeight;

        public List<int> adjacentNodeIDs;

        public Vector2Int position;

        public NodeState state;
        #endregion

        #region CONSTRUCTOR
        public Node(int ID, Vector2Int position)
        {
        this.ID = ID;
        this.position = position;
        this.adjacentNodeIDs = NodeUtils.GetAdjacentsNodesIDs(position);
        this.state = NodeState.Ready;
        openerID = -1;
        }
        #endregion

        #region PUBLIC_METHOD

        public void SetWeight(int weight)
        {
            if (weight == -1)
                state = NodeState.Obstacle;
            this.weight = weight;
            originalWigth = weight;
        }

        public void Open(int openerID, int parentWeight)
        {
            state = NodeState.Open;
            this.openerID = openerID;
            totalWeight = parentWeight + weight;
        }

        public void Reset()
        {
            if (this.state != NodeState.Obstacle)
            {
                this.state = NodeState.Ready;
                this.openerID = -1;
                weight = originalWigth;
            }
        }
        #endregion
    }
    public static class NodeUtils
        {
        #region PUBLIC_METHODS

        public static List<int> GetAdjacentsNodesIDs(Vector2Int position)
        {
            List<int> IDs = new List<int>();
            IDs.Add(PositionToIndex(new Vector2Int(position.x + 1, position.y)));
            IDs.Add(PositionToIndex(new Vector2Int(position.x, position.y - 1)));
            IDs.Add(PositionToIndex(new Vector2Int(position.x - 1, position.y)));
            IDs.Add(PositionToIndex(new Vector2Int(position.x, position.y + 1)));
            return IDs;
        }

        public static int PositionToIndex(Vector2Int position)
        {
            if (position.x < 0 || position.x >= Main.MapSize.x ||
                position.y < 0 || position.y >= Main.MapSize.y)
                return -1;

            return position.x * Main.MapSize.y + position.y;
        }

        public static Vector2Int GetVec3IntFromVector3(Vector3 vector)
        {
            return new Vector2Int
            {
                x = Mathf.RoundToInt(vector.x),
                y = Mathf.RoundToInt(vector.y)
            };
        }
        #endregion
    }
}