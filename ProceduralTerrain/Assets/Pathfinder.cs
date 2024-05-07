using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Connection{
    public float cost;
    public NodeRecord fromNode = new NodeRecord();
    public NodeRecord toNode = new NodeRecord();
}

public class NodeRecord
{
    public Tile node;
    public float costSoFar;
    public float estimatedTotalCost;
    public Connection connection;
    public List<Connection> outgoingConnections = new List<Connection>();

    public TileMapController tileMapController;
    
    public void InitOutgoingConnections()
    {
        List<Tile> neighbors = tileMapController.GetNeighborsFromGeneratedMap(node);
        foreach(Tile tile in neighbors)
        {
            if(tile == null) continue;
            Connection connectionToAdd = new Connection();
            connectionToAdd.cost = 1f;
            connectionToAdd.fromNode.node = node;
            connectionToAdd.toNode.node = tile;
            outgoingConnections.Add(connectionToAdd);
        }
    }
}

public class Pathfinder : MonoBehaviour
{
    public GameObject objectToMove;

    public TileMapController tileMapController;

    public Vector2Int startPosition;
    public Vector2Int endPosition;
    public TileBase tileBase;
    public Tilemap tilemap;

    private float estimateCostFromTile(Tile targetTile, Tile endTile)
    {
        //if(targetTile == null) Debug.Log("target tile is null");
        //if(endTile == null) Debug.Log("end tile is null");
        int dx = Mathf.Abs(targetTile.position.x - endTile.position.x);
        int dy = Mathf.Abs(targetTile.position.y - endTile.position.y);
        return Mathf.Sqrt((dx*dx) + (dy*dy));
    }

    private NodeRecord GetSmallestEstimatedTotalCost(List<NodeRecord> list)
    {
        NodeRecord smallest = list[0];
        foreach(NodeRecord record in list)
        {
            if(record.estimatedTotalCost <= smallest.estimatedTotalCost) smallest = record;
        }
        return smallest;
    }

    private NodeRecord FindRecordByNode(List<NodeRecord> list, Tile tile)
    {
        NodeRecord targetNodeRecord = null;
        foreach(NodeRecord record in list)
        {
            if(record.node == tile) targetNodeRecord = record;
        }
        return targetNodeRecord;
    }

    public void followPath()
    {
        Tile startTile = tileMapController.GetTileWithPosition(startPosition, tileMapController.generatedTilemapBottomLayerList);
        Tile endTile = tileMapController.GetTileWithPosition(endPosition, tileMapController.generatedTilemapBottomLayerList);
        List<Tile> path = findPath(startTile, endTile);
        if(path == null) Debug.Log("path is null");
        foreach(Tile tile in path)
        {
            tilemap.SetTile(new Vector3Int(tile.position.x, tile.position.y, 0), tileBase);
        }
    }

    private void printConnections(NodeRecord nodeRecord)
    {
        string output = $"The current node at {nodeRecord.node.position} has connections to nodes ";
        foreach(Connection connection in nodeRecord.outgoingConnections)
        {
            output = output + $"{connection.toNode.node.position} ";
        }
        Debug.Log(output);
    }

    public List<Tile> findPath(Tile startTile, Tile endTile)
    {
        NodeRecord startRecord = new NodeRecord();

        if(startTile == null) Debug.Log("start tile null");
        startRecord.node = startTile;
        startRecord.costSoFar = 0f;
        startRecord.estimatedTotalCost = estimateCostFromTile(startTile, endTile);
        startRecord.connection = null;
        startRecord.tileMapController = tileMapController;
        startRecord.InitOutgoingConnections();

        List<NodeRecord> open = new List<NodeRecord>();
        open.Add(startRecord);
        List<NodeRecord> closed = new List<NodeRecord>();

        NodeRecord current = null;

        while(open.Count > 0)
        {
            current = GetSmallestEstimatedTotalCost(open);
            printConnections(current);

            if(current.node.position == endTile.position) 
            {
                Debug.Log("path found");
                break;
            }

            foreach(Connection connection in current.outgoingConnections)
            {
                //Debug.Log($"current node comes from {connection.fromNode.node.position}");

                Tile endNode = connection.toNode.node;
                float endNodeCost = current.costSoFar + connection.cost;

                NodeRecord endNodeRecord = null;
                float endNodeHeuristic = 0f;
                if(FindRecordByNode(closed, endNode) != null)
                {
                    endNodeRecord = FindRecordByNode(closed, endNode);
                    if(endNodeRecord.costSoFar <= endNodeCost) continue;
                    closed.Remove(endNodeRecord);
                    endNodeHeuristic = estimateCostFromTile(endNode, endTile);
                }
                else if(FindRecordByNode(open, endNode) != null)
                {
                    endNodeRecord = FindRecordByNode(open, endNode);
                    if(endNodeRecord.costSoFar <= endNodeCost) continue;
                    endNodeHeuristic = estimateCostFromTile(endNode, endTile);
                }
                else
                {
                    endNodeRecord = new NodeRecord();
                    endNodeRecord.node = endNode;
                    endNodeHeuristic = estimateCostFromTile(endNode, endTile);
                }

                Connection tempConnection = new Connection();
                tempConnection.fromNode = current;
                tempConnection.toNode = endNodeRecord;
                endNodeRecord.connection = tempConnection;
                endNodeRecord.estimatedTotalCost = endNodeCost + endNodeHeuristic;
                endNodeRecord.tileMapController = tileMapController;
                endNodeRecord.InitOutgoingConnections();

                if(FindRecordByNode(open, endNode) == null) open.Add(endNodeRecord);
            }


            open.Remove(current);
            closed.Add(current);
        }

        if(current.node.position != endTile.position)
        {
            Debug.Log("No solution");
            return null;
        }
        else
        {
            List<Tile> path = new List<Tile>();
            while(current.node.position != startTile.position)
            {
                path.Add(current.node);

                if(current.connection == null) Debug.Log("current connection is null");
                else Debug.Log("current connection is not null");

                current = current.connection.fromNode;
            }
            path.Reverse();
            return path;
        }
    }
}

