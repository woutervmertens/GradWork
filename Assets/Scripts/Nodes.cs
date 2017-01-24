using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    Connection,
    Spawner,
    Intersection
}

public class Nodes : MonoBehaviour {

	public Type NodeType;

    public Nodes Parent { get; set; }

    public float GetFScore()
    {
        return 1;
    }

    public List<int> GetConnections()
    {
        return (NodeType == Type.Spawner)? GetComponent<SpawnerNode>().Connections: GetComponent<IntersectionNode>().Connections;
    }
}
