using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    [Serializable]
    public enum Methods
    {
        BreadthFirst,
        dephFirst,
        Dijkstra,
        AStar
    }

    public Methods methods = Methods.AStar;

    public List<Vector2Int> GetPath(Node[] map, Node origin, Node destination)
    {
        List<int> openNodesID = new List<int>();
        Vector2Int destinationPosition;

        openNodesID.Add(origin.ID);
        destinationPosition = destination.position;
        Node currentNode = origin;
        while (currentNode.position != destination.position)
        {
            currentNode = GetNextNode(map, destinationPosition, openNodesID);
            if (currentNode == null)
                return new List<Vector2Int>();
            for (int i = 0; i < currentNode.adjacentNodeIDs.Count; i++)
            {
                if (currentNode.adjacentNodeIDs[i] != -1)
                {
                    if (map[currentNode.adjacentNodeIDs[i]].state == Node.NodeState.Ready)
                    {
                        map[currentNode.adjacentNodeIDs[i]].Open(currentNode.ID, currentNode.totalWeight);
                        openNodesID.Add(map[currentNode.adjacentNodeIDs[i]].ID);
                    }
                }
            }
            currentNode.state = Node.NodeState.Closed;
            openNodesID.Remove(currentNode.ID);

        }
        List<Vector2Int> path = GeneratePath(map, currentNode);

        return path;
    }

    private List<Vector2Int> GeneratePath(Node[] map, Node current)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (current.openerID != -1)
        {
            path.Add(current.position);
            current = map[current.openerID];
        }
        path.Add(current.position);
        path.Reverse();
        return path;
    }

    private Node GetNextNode(Node[] map, Vector2Int destinationPosition, List<int> openNodesID)
    {
        switch (methods)
        {
            case Methods.BreadthFirst:
                return map[openNodesID[0]];
            case Methods.dephFirst:
                return map[openNodesID[openNodesID.Count - 1]];
            case Methods.Dijkstra:
                {
                    Node n = null;
                    int currentMaxWeight = int.MaxValue;
                    for (int i = 0; i < openNodesID.Count; i++)
                        if (map[openNodesID[i]].totalWeight < currentMaxWeight)
                        {
                            n = map[openNodesID[i]];
                            currentMaxWeight = map[openNodesID[i]].totalWeight;
                        }
                    return n;
                }
            case Methods.AStar:
                {
                    Node n = null;
                    int currentMaxWeightAndDistance = int.MaxValue;
                    for (int i = 0; i < openNodesID.Count; i++)
                        if (map[openNodesID[i]].totalWeight + GetManhattanDistance(map[openNodesID[i]].position, destinationPosition) < currentMaxWeightAndDistance)
                        {
                            n = map[openNodesID[i]];
                            currentMaxWeightAndDistance = map[openNodesID[i]].totalWeight + GetManhattanDistance(map[openNodesID[i]].position, destinationPosition);
                        }
                    return n;
                }
        }
        return null;
    }

    private int GetManhattanDistance(Vector2Int origin, Vector2Int destination)
    {
        int disX = Mathf.Abs(origin.x - destination.x);
        int disY = Mathf.Abs(origin.y - destination.y);
        return disX + disY;
    }
}
