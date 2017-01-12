using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public ArrayList Path;

    private ArrayList _path;

    private SpawnerNode StartNode, EndNode;

    public Pathfinding(SpawnerNode startNode, SpawnerNode endNode)
    {
        StartNode = startNode;
        EndNode = endNode;
        _path.Add(StartNode);
        GetPath();
        Path = _path;
    }

    ArrayList GetPath()
    {
        var currentNode = _path[_path.Count];
        //follow connection from startNode
        //get connections linked to next node
        //repeat for all those connected nodes untill endNode is found
        //save path
        return _path;
    }
}
